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

        //public async Container FindSolution(Container container, TileFitterOptions options)
        //{
        //    var resultContainer = container.Clone();
        //    switch(options.Algorithm)
        //    {
        //        case Algorithm.MaximalRectangles:
        //        default:
        //            return await (new MaximalRectanglesAlgorithm().Execute(container, options.Heuristic));
        //    }
        //}

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

        

        //public Container FindFastestSolution(Container container)
        //{
        //    var runningAlgorithms = new List<Task<Container>>();
        //    var cts = new CancellationTokenSource();
        //    var cancellationToken = cts.Token;

        //    var maxRectsRunningAlgorithms = RunAllMaximalRectanglesHeuristicsAsync(container, cancellationToken);
        //    runningAlgorithms.AddRange(maxRectsRunningAlgorithms);

        //    Container solution = null;
        //    Container bestInvalidSolution = container;
        //    Task.WhenAny(runningAlgorithms).ContinueWith(x =>
        //    {
        //        var result = x.Result.Result;
        //        if (result.IsValidSolution)
        //        {
        //            solution = result;
        //            cts.Cancel();
        //        }
        //        else if(result.RemainingTiles.Count < bestInvalidSolution.RemainingTiles.Count)
        //        {
        //            bestInvalidSolution = result;
        //        }
        //    }).Wait();

        //    return solution ?? bestInvalidSolution;
        //}
    }
}
