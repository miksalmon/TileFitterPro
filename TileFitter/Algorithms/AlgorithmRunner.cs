using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TileFitter.Comparers;
using TileFitter.Interfaces;
using TileFitter.Models;

namespace TileFitter.Algorithms
{
    /// <inheritdoc cref="IAlgorithmRunner"/>
    /// <summary>
    /// Implementation of <see cref="IAlgorithmRunner"/> for executing all heuristics of the Maximal Rectangles algorithm implemented in <see cref="MaximalRectanglesAlgorithm"/>
    /// </summary>
    public class AlgorithmRunner<T> : IAlgorithmRunner where T : class, IAlgorithm, new()
    {
        public List<Task<Container>> RunAllHeuristicsAsync(Container container, CancellationToken cancellationToken = default)
        {
            var tasks = new List<Task<Container>>();

            var algorithm = new T();
            foreach (var heuristic in algorithm.Heuristics)
            {
                var resultContainer = container.Clone();
                tasks.Add(Task.Run(() =>
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    return ExecuteOptimally(resultContainer, heuristic);
                }, cancellationToken));
                tasks.AddRange(RunAllBlindHeuristicsAsync(resultContainer, heuristic, cancellationToken));
            }

            return tasks;
        }

        private List<Task<Container>> RunAllBlindHeuristicsAsync(Container container, Heuristic heuristic, CancellationToken cancellationToken = default)
        {
            var tasks = new List<Task<Container>>();

            foreach (var sortHeuristic in Enum.GetValues(typeof(SortHeuristic)) as SortHeuristic[])
            {
                var resultContainer = container.Clone();

                resultContainer.RemainingTiles.Sort(GetComparerByHeuristic(sortHeuristic));
                
                tasks.Add(Task.Run(() =>
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    return RunBlindMaximalRectangles(resultContainer, heuristic);
                }, cancellationToken));
            }
            return tasks;
        }

        private Container RunBlindMaximalRectangles(Container container, Heuristic heuristic)
        {
            var tileFitter = new T();
            var result = tileFitter.RunBlindly(container, heuristic);
            return result;
        }

        private Container ExecuteOptimally(Container container, Heuristic heuristic)
        {
            var tileFitter = new T();
            var result = tileFitter.RunOptimally(container, heuristic);
            return result;
        }

        private IComparer<Rectangle> GetComparerByHeuristic(SortHeuristic heuristic)
        {
            switch(heuristic)
            {
                case SortHeuristic.PerimeterDescending:
                    return new RectanglePerimeterComparer();
                case SortHeuristic.WidthDescending:
                    return new RectangleWidthComparer();
                case SortHeuristic.HeightDescending:
                    return new RectangleHeightComparer();
                case SortHeuristic.AreaDescending:
                    return new RectangleAreaComparer();
                default:
                    throw new NotImplementedException();
            }
        }

        private enum SortHeuristic
        {
            AreaDescending,
            PerimeterDescending,
            WidthDescending,
            HeightDescending
        }
    }
}
