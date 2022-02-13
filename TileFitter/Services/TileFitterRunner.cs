using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TileFitter.Interfaces;
using TileFitter.Models;

namespace TileFitter.Services
{
    public class TileFitterRunner : ITileFitterRunner
    {
        public IEnumerable<IAlgorithm> Algorithms { get; }

        public TileFitterRunner(IEnumerable<IAlgorithm> algorithms)
        {
            Algorithms = algorithms;
        }

        public Task<IEnumerable<Container>> FindAllSolutionsAsync(Container container, CancellationToken cancellationToken = default)
        {
            var resultContainer = container.Clone();
            var runningAlgorithms = new List<Task<List<Container>>>();

            foreach (var algorithm in Algorithms)
            {
                runningAlgorithms.Add(algorithm.ExecuteAll(resultContainer));
            }

            var resultTask = Task.WhenAll(runningAlgorithms).ContinueWith(x =>
            {
                return x.Result.ToList().SelectMany(l => l).Where(c => c.IsValidSolution);
            });
            
            return resultTask;
        }
    }
}
