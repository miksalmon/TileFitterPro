﻿using System;
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
    public class MaximalRectanglesAlgorithmRunner : IAlgorithmRunner
    {
        public List<Task<Container>> RunAllHeuristicsAsync(Container container, CancellationToken cancellationToken = default)
        {
            return ExecuteAllMaximalRectanglesHeuristicsAsync(container, cancellationToken);
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
            var tileFitter = new MaximalRectanglesAlgorithm();
            var result = tileFitter.FitTilesBlindly(container, heuristic);
            return result;
        }

        private Container ExecuteOptimalMaximalRectangles(Container container, MaximalRectanglesHeuristic heuristic)
        {
            var tileFitter = new MaximalRectanglesAlgorithm();
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
