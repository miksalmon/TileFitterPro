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
        public IEnumerable<IAlgorithmRunner> AlgorithmRunners { get; }

        public TileFitterRunner(IEnumerable<IAlgorithmRunner> runners)
        {
            AlgorithmRunners = runners;
        }

        public async Task<IEnumerable<Container>> FindAllSolutionsAsync(Container container, CancellationToken cancellationToken = default)
        {
            var resultContainer = container.Clone();
            var runningAlgorithms = new List<Task<Container>>();

            foreach (var runner in AlgorithmRunners)
            {
                runningAlgorithms.AddRange(runner.RunAllHeuristicsAsync(resultContainer, cancellationToken));
            }

            var results = await Task.WhenAll(runningAlgorithms);
            
            return results.ToList().Where(c => c.IsValidSolution());
        }

        public Task<Container> FindFastestSolutionAsync(Container container, CancellationToken cancellationToken)
        {
            var resultContainer = container.Clone();
            var runningAlgorithms = new List<Task<Container>>();

            foreach (var runner in AlgorithmRunners)
            {
                runningAlgorithms.AddRange(runner.RunAllHeuristicsAsync(resultContainer, cancellationToken));
            }

            //var results = await Task.WhenAll(runningAlgorithms);

            //return results.ToList().Where(c => c.IsValidSolution());
            return Task.FromResult<Container>(null);
        }
    }
}
