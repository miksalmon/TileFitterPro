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

        public async Task<IEnumerable<Container>> FindAllSolutionsAsync(Container container, CancellationToken cancellationToken = default)
        {
            var resultContainer = container.Clone();
            var runningAlgorithms = new List<Task<Container>>();

            foreach (var algorithm in Algorithms)
            {
                runningAlgorithms.AddRange(algorithm.ExecuteAllHeuristicsAsync(resultContainer, cancellationToken));
            }

            var results = await Task.WhenAll(runningAlgorithms);
            
            return results.ToList().Where(c => c.IsValidSolution());
        }
    }
}
