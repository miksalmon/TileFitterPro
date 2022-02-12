using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TileFitter.Algorithms;
using TileFitter.Interfaces;
using TileFitter.Models;

namespace TileFitter.Services
{
    public class TileFitterRunner : ITileFitterRunner
    {
        public Container FindSolution(Container container, TileFitterOptions options)
        {
            var resultContainer = container.Clone();
            switch(options.Algorithm)
            {
                case Algorithm.MaximalRectangles:
                default:
                    return RunMaximalRectangles(resultContainer, options.Heuristic);
            }
        }

        public Task<IEnumerable<Container>> FindAllSolutionsAsync(Container container)
        {
            var resultContainer = container.Clone();
            var runningAlgorithms = new List<Task<Container>>();

            var maxRectsRunningAlgorithms = RunAllMaximalRectanglesHeuristicsAsync(resultContainer);
            runningAlgorithms.AddRange(maxRectsRunningAlgorithms);

            var resultTask = Task.WhenAll(runningAlgorithms).ContinueWith(x =>
            {
                return x.Result.ToList().Where(c => c.IsValidSolution);
            });
            
            return resultTask;
        }

        public IEnumerable<Task<Container>> RunAllMaximalRectanglesHeuristicsAsync(Container container, CancellationToken cancellationToken = default)
        {
            var tasks = new List<Task<Container>>();

            foreach(var heuristic in Enum.GetValues(typeof(Heuristic)) as Heuristic[])
            {
                var resultContainer = container.Clone();
                tasks.Add(Task.Run(() => RunMaximalRectangles(resultContainer, heuristic), cancellationToken));
            }

            return tasks;
        }

        public Container FindFastestSolution(Container container)
        {
            var runningAlgorithms = new List<Task<Container>>();
            var cts = new CancellationTokenSource();
            var cancellationToken = cts.Token;

            var maxRectsRunningAlgorithms = RunAllMaximalRectanglesHeuristicsAsync(container, cancellationToken);
            runningAlgorithms.AddRange(maxRectsRunningAlgorithms);

            Container solution = null;
            Container bestInvalidSolution = container;
            Task.WhenAny(runningAlgorithms).ContinueWith(x =>
            {
                var result = x.Result.Result;
                if (result.IsValidSolution)
                {
                    solution = result;
                    cts.Cancel();
                }
                else if(result.RemainingTiles.Count < bestInvalidSolution.RemainingTiles.Count)
                {
                    bestInvalidSolution = result;
                }
            }).Wait();

            return solution ?? bestInvalidSolution;
        }

        #region MaximalRectangles

        public Container RunMaximalRectangles(Container container, Heuristic heuristic)
        {
            var tileFitter = new MaximalRectanglesTileFitter();
            var maximalRectanglesHeuristic = GetMaximalRectanglesHeuristic(heuristic);
            var result = tileFitter.FitTiles(container, maximalRectanglesHeuristic);
            return result;
        }

        public MaximalRectanglesHeuristic GetMaximalRectanglesHeuristic(Heuristic heuristic)
        {
            switch (heuristic)
            {
                case Heuristic.BottomLeftRule:
                    return MaximalRectanglesHeuristic.BottomLeftRule;
                case Heuristic.BestAreaFit:
                    return MaximalRectanglesHeuristic.BestAreaFit;
                case Heuristic.BestLongSideFit:
                    return MaximalRectanglesHeuristic.BestLongSideFit;
                case Heuristic.BestShortSideFit:
                    return MaximalRectanglesHeuristic.BestShortSideFit;
                default:
                    throw new InvalidCastException("Invalid heuristic for MaximalRectangles algorithm.");
            }
        }

        #endregion
    }
}
