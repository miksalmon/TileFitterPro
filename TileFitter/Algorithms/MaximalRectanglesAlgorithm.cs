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
    public class MaximalRectanglesAlgorithm : IAlgorithm
    {
        public List<Task<Container>> ExecuteAllHeuristicsAsync(Container container, CancellationToken cancellationToken = default)
        {
            var resultContainer = container.Clone();

            return ExecuteAllMaximalRectanglesHeuristicsAsync(resultContainer, cancellationToken);
        }

        public Task<Container> ExecuteAllHeuristicsUntilFirstSuccess(Container container, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<Container> Execute(Container container, MaximalRectanglesHeuristic heuristic)
        {
            throw new NotImplementedException();
        }

        public List<Task<Container>> ExecuteAllMaximalRectanglesHeuristicsAsync(Container container, CancellationToken cancellationToken = default)
        {
            var tasks = new List<Task<Container>>();

            foreach (var heuristic in Enum.GetValues(typeof(MaximalRectanglesHeuristic)) as MaximalRectanglesHeuristic[])
            {
                var resultContainer = container.Clone();
                tasks.Add(Task.Run(() => ExecuteOptimalMaximalRectangles(resultContainer, heuristic), cancellationToken));
                tasks.AddRange(ExecuteAllBlindMaximalRectangles(resultContainer, heuristic, cancellationToken));
            }

            return tasks;
        }

        private List<Task<Container>> ExecuteAllBlindMaximalRectangles(Container container, MaximalRectanglesHeuristic heuristic, CancellationToken cancellationToken = default)
        {
            var tasks = new List<Task<Container>>();

            foreach (var sortHeuristic in Enum.GetValues(typeof(BlindMaximalRectanglesSortHeuristic)) as BlindMaximalRectanglesSortHeuristic[])
            {
                var resultContainer = container.Clone();

                resultContainer.RemainingTiles.Sort(GetComparerByHeuristic(sortHeuristic));
                
                tasks.Add(Task.Run(() => ExecuteBlindMaximalRectangles(resultContainer, heuristic), cancellationToken));
            }
            return tasks;
        }

        private Container ExecuteBlindMaximalRectangles(Container container, MaximalRectanglesHeuristic heuristic)
        {
            var tileFitter = new MaximalRectanglesTileFitter();
            var result = tileFitter.FitTilesBlindly(container, heuristic);
            return result;
        }

        private Container ExecuteOptimalMaximalRectangles(Container container, MaximalRectanglesHeuristic heuristic)
        {
            var tileFitter = new MaximalRectanglesTileFitter();
            var result = tileFitter.FitTilesOptimally(container, heuristic);
            return result;
        }

        private IComparer<Rectangle> GetComparerByHeuristic(BlindMaximalRectanglesSortHeuristic heuristic)
        {
            switch(heuristic)
            {
                case BlindMaximalRectanglesSortHeuristic.PerimeterDescending:
                    return new RectanglePerimeterComparer();
                case BlindMaximalRectanglesSortHeuristic.WidthDescending:
                    return new RectangleWidthComparer();
                case BlindMaximalRectanglesSortHeuristic.HeightDescending:
                    return new RectangleHeightComparer();
                case BlindMaximalRectanglesSortHeuristic.AreaDescending:
                default:
                    return new RectangleAreaComparer();
            }
        }

        private enum BlindMaximalRectanglesSortHeuristic
        {
            AreaDescending,
            PerimeterDescending,
            WidthDescending,
            HeightDescending
        }
    }
}
