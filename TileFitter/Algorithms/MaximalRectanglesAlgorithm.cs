using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.PointOfService;
using Windows.Foundation;
using Windows.Globalization.DateTimeFormatting;
using TileFitter.Models;
using TileFitter.Extensions;

namespace TileFitter.Algorithms
{
    /// <summary>
    /// Implementation of the MaximalRectangles algorithm.
    /// </summary>
    internal class MaximalRectanglesAlgorithm
    {
        private Container Container { get; set; }

        private List<Rectangle> FreeRectangles { get; set; } = new List<Rectangle>();

        public MaximalRectanglesAlgorithm()
        {
        }

        public void Initialize(Container container)
        {
            Container = container.Clone();
            Container.PlacedTiles.Clear();
            FreeRectangles.Clear();
            FreeRectangles.Add(new Rectangle(0, 0, Container.Width, Container.Height));
        }

        public Container FitTilesBlindly(Container container, MaximalRectanglesHeuristic heuristic)
        {
            if (container.RemainingTiles is null || container is null)
            {
                throw new ArgumentNullException(nameof(Container.RemainingTiles));
            }

            if(container.Area < container.RemainingTiles.Sum(x => x.GetArea()))
            {
                return container;
            }

            Initialize(container);

            // Copy since we iterate on them and need to remove them as we go
            var tilesToPlace = Container.RemainingTiles.Select(x => x).ToList();

            foreach (var tile in tilesToPlace)
            {
                var bestTilePlacement = FindTilePlacement(tile, heuristic);

                // Cannot fit tile
                if (bestTilePlacement.PlacedTile == Rectangle.Empty)
                {
                    return Container;
                }

                PlaceTile(bestTilePlacement);
            }

            return Container;
        }


        public Container FitTilesOptimally(Container container, MaximalRectanglesHeuristic heuristic)
        {
            if (container.RemainingTiles is null || container is null)
            {
                throw new ArgumentNullException(nameof(Container.RemainingTiles));
            }

            Initialize(container);

            // while there are tiles to fit
            while (Container.RemainingTiles.Count > 0)
            {
                var bestTilePlacement = FindBestTileToPlace(heuristic);

                // Cannot fit tile
                if (bestTilePlacement.PlacedTile == Rectangle.Empty)
                {
                    return Container;
                }

                PlaceTile(bestTilePlacement);
            }

            return Container;
        }

        #region Tile Placement

        private void PlaceTile(MaximalRectanglesTilePlacement tilePlacement)
        {
            var splitMaximals = CalculateMaximalFreeRectangles(tilePlacement.FreeRectangle, tilePlacement.PlacedTile);
            FreeRectangles.Remove(tilePlacement.FreeRectangle);

            var freeRectangles = RecalculateFreeRectangles(tilePlacement.PlacedTile);
            FreeRectangles = freeRectangles;

            FreeRectangles.AddRange(splitMaximals);

            RemoveRedundantFreeRectangles();

            Container.RemainingTiles.Remove(tilePlacement.InitialTile);
            Container.PlacedTiles.Add(tilePlacement.PlacedTile);
        }

        private MaximalRectanglesTilePlacement FindBestTileToPlace(MaximalRectanglesHeuristic heuristic)
        {
            MaximalRectanglesTilePlacement bestTilePlacement = new MaximalRectanglesTilePlacement(Rectangle.Empty, Rectangle.Empty, Rectangle.Empty, new HeuristicMetrics(int.MaxValue, int.MaxValue));
            foreach (var tile in Container.RemainingTiles)
            {
                var tilePlacement = FindTilePlacement(tile, heuristic);

                if (tilePlacement.Metrics.PrimaryMetric < bestTilePlacement.Metrics.PrimaryMetric ||
                    (tilePlacement.Metrics.PrimaryMetric == bestTilePlacement.Metrics.PrimaryMetric &&
                     tilePlacement.Metrics.SecondaryMetric < bestTilePlacement.Metrics.SecondaryMetric))
                {
                    bestTilePlacement = tilePlacement;
                }
            }

            return bestTilePlacement;
        }

        private MaximalRectanglesTilePlacement FindTilePlacement(Rectangle tile, MaximalRectanglesHeuristic heuristic)
        {
            switch (heuristic)
            {
                case MaximalRectanglesHeuristic.BottomLeftRule:
                    return FindTilePlacementBottomLeftRule(tile);
                case MaximalRectanglesHeuristic.BestAreaFit:
                    return FindTilePlacementBestAreaFit(tile);
                case MaximalRectanglesHeuristic.BestLongSideFit:
                    return FindTilePlacementBestLongSideFit(tile);
                case MaximalRectanglesHeuristic.BestShortSideFit:
                default:
                    return FindTilePlacementBestShortSideFit(tile);
            }
        }

        #endregion

        #region Free Rectangles Management

        private List<Rectangle> CalculateMaximalFreeRectangles(Rectangle freeRectangle, Rectangle placedTile)
        {
            var maximalFreeRectangles = new List<Rectangle>();

            if (placedTile.Left < freeRectangle.Right && placedTile.Right > freeRectangle.Left)
            {
                // New node at the top side of the used node.
                if (placedTile.Top > freeRectangle.Top && placedTile.Top < freeRectangle.Bottom)
                {
                    var newNode = freeRectangle;
                    newNode.Height = freeRectangle.Y - newNode.Y;
                    maximalFreeRectangles.Add(newNode);
                }

                // New node at the bottom side of the used node.
                if (placedTile.Bottom < freeRectangle.Bottom)
                {
                    var newFreeRectangle = freeRectangle;
                    newFreeRectangle.Y = placedTile.Y + placedTile.Height;
                    newFreeRectangle.Height = freeRectangle.Y + freeRectangle.Height - (placedTile.Y + placedTile.Height);
                    maximalFreeRectangles.Add(newFreeRectangle);
                }
            }

            if (placedTile.Top < freeRectangle.Bottom && placedTile.Bottom > freeRectangle.Top)
            {
                // New node at the left side of the used node.
                if (placedTile.Left > freeRectangle.Left && placedTile.Left < freeRectangle.Right)
                {
                    var newFreeRectangle = freeRectangle;
                    newFreeRectangle.Width = placedTile.X - freeRectangle.X;
                    maximalFreeRectangles.Add(newFreeRectangle);
                }

                // New node at the right side of the used node.
                if (placedTile.Right < freeRectangle.Right)
                {
                    var newFreeRectangle = freeRectangle;
                    newFreeRectangle.X = placedTile.X + placedTile.Width;
                    newFreeRectangle.Width = freeRectangle.X + freeRectangle.Width - (placedTile.X + placedTile.Width);
                    maximalFreeRectangles.Add(newFreeRectangle);
                }
            }

            return maximalFreeRectangles;
        }

        private void RemoveRedundantFreeRectangles()
        {
            // Remove any rectangle contained in another one
            for (int i = 0; i < FreeRectangles.Count; ++i)
            {
                var freeRectangle = FreeRectangles[i];
                for (int j = i + 1; j < FreeRectangles.Count; ++j)
                {
                    var otherFreeRectangle = FreeRectangles[j];

                    if (otherFreeRectangle.Contains(freeRectangle))
                    {
                        FreeRectangles.RemoveAt(i--);
                        break;
                    }
                    if (freeRectangle.Contains(otherFreeRectangle))
                    {
                        FreeRectangles.RemoveAt(j--);
                    }
                }
            }
        }

        private List<Rectangle> PruneOverlap(Rectangle freeRectangle, Rectangle overlap)
        {
            var newFreeRectangles = new List<Rectangle>();
            // new free rectangle on the left
            if (overlap.Left > freeRectangle.Left)
            {
                var newFreeRectangle = new Rectangle(freeRectangle.Left, freeRectangle.Top, overlap.Left - freeRectangle.Left, freeRectangle.Height);
                newFreeRectangles.Add(newFreeRectangle);
            }

            // new free rectangle on the right
            if (overlap.Right < freeRectangle.Right)
            {
                var newFreeRectangle = new Rectangle(overlap.Right, freeRectangle.Top, freeRectangle.Right - overlap.Right, freeRectangle.Height);
                newFreeRectangles.Add(newFreeRectangle);
            }

            // new free rectangle on the top
            // overlap.Top < freeRectangle.Bottom ????
            if (overlap.Top > freeRectangle.Top)
            {
                var newFreeRectangle = new Rectangle(freeRectangle.Left, freeRectangle.Top, freeRectangle.Width, overlap.Top - freeRectangle.Top);
                newFreeRectangles.Add(newFreeRectangle);
            }

            // new free rectangle on the bottom
            if (overlap.Bottom < freeRectangle.Bottom)
            {
                var newFreeRectangle = new Rectangle(freeRectangle.Left, overlap.Bottom, freeRectangle.Width, freeRectangle.Bottom - overlap.Bottom);
                newFreeRectangles.Add(newFreeRectangle);
            }

            return newFreeRectangles;
        }

        private List<Rectangle> RecalculateFreeRectangles(Rectangle placedTile)
        {
            var freeRectangles = new List<Rectangle>();
            foreach (var freeRectangle in FreeRectangles)
            {
                if (placedTile.IntersectsWith(freeRectangle))
                {
                    var overlap = Rectangle.Intersect(placedTile, freeRectangle);
                    var newFreeRectangles = PruneOverlap(freeRectangle, overlap);
                    freeRectangles.AddRange(newFreeRectangles);
                }
                else
                {
                    freeRectangles.Add(freeRectangle);
                }
            }

            return freeRectangles;
        }

        #endregion

        #region Heuristics
        // TODO: merge FindTilePlacement* methods

        private MaximalRectanglesTilePlacement FindTilePlacementBottomLeftRule(Rectangle tile)
        {
            var bestFreeRectangle = Rectangle.Empty;
            var bestPlacement = Rectangle.Empty;
            var bestMetrics = new HeuristicMetrics(int.MaxValue, int.MaxValue);
            foreach (var freeRectangle in FreeRectangles)
            {
                if (freeRectangle.CanContain(tile))
                {
                    var metrics = CalculateBottomLeftRuleMetrics(freeRectangle, tile);

                    if (metrics.IsBetter(bestMetrics))
                    {
                        bestFreeRectangle = freeRectangle;
                        bestPlacement = new Rectangle(freeRectangle.X, freeRectangle.Y, tile.Width, tile.Height);
                        bestMetrics = metrics;
                    }
                }
            }

            return new MaximalRectanglesTilePlacement(tile, bestFreeRectangle, bestPlacement, bestMetrics);
        }

        private HeuristicMetrics CalculateBottomLeftRuleMetrics(Rectangle freeRectangle, Rectangle tile)
        {
            var freeRectangleArea = freeRectangle.Width * freeRectangle.Height;
            var tileArea = tile.Width * tile.Height;
            var areaScore = freeRectangleArea - tileArea;

            var yScore = freeRectangle.Y + tile.Height;
            var xScore = freeRectangle.X;

            return new HeuristicMetrics(yScore, xScore);
        }

        private MaximalRectanglesTilePlacement FindTilePlacementBestAreaFit(Rectangle tile)
        {
            var bestFreeRectangle = Rectangle.Empty;
            var bestPlacement = Rectangle.Empty;
            var bestMetrics = new HeuristicMetrics(int.MaxValue, int.MaxValue);
            foreach (var freeRectangle in FreeRectangles)
            {
                if (freeRectangle.CanContain(tile))
                {
                    var metrics = CalculateBestAreaFitMetrics(freeRectangle, tile);

                    if (metrics.IsBetter(bestMetrics))
                    {
                        bestFreeRectangle = freeRectangle;
                        bestPlacement = new Rectangle(freeRectangle.X, freeRectangle.Y, tile.Width, tile.Height);
                        bestMetrics = metrics;
                    }
                }
            }

            return new MaximalRectanglesTilePlacement(tile, bestFreeRectangle, bestPlacement, bestMetrics);
        }

        private HeuristicMetrics CalculateBestAreaFitMetrics(Rectangle freeRectangle, Rectangle tile)
        {
            var freeRectangleArea = freeRectangle.GetArea();
            var tileArea = tile.GetArea();
            var areaScore = freeRectangleArea - tileArea;

            var widthDiff = freeRectangle.Width - tile.Width;
            var heightDiff = freeRectangle.Height - tile.Height;
            var shortSideScore = Math.Min(widthDiff, heightDiff);

            return new HeuristicMetrics(areaScore, shortSideScore);
        }

        private MaximalRectanglesTilePlacement FindTilePlacementBestLongSideFit(Rectangle tile)
        {
            var bestFreeRectangle = Rectangle.Empty;
            var bestPlacement = Rectangle.Empty;
            var bestMetrics = new HeuristicMetrics(int.MaxValue, int.MaxValue);
            foreach (var freeRectangle in FreeRectangles)
            {
                if (freeRectangle.CanContain(tile))
                {
                    var metrics = CalculateBestLongSideFitMetrics(freeRectangle, tile);

                    if (metrics.IsBetter(bestMetrics))
                    {
                        bestFreeRectangle = freeRectangle;
                        bestPlacement = new Rectangle(freeRectangle.X, freeRectangle.Y, tile.Width, tile.Height);
                        bestMetrics = metrics;
                    }
                }
            }

            return new MaximalRectanglesTilePlacement(tile, bestFreeRectangle, bestPlacement, bestMetrics);
        }

        private HeuristicMetrics CalculateBestLongSideFitMetrics(Rectangle freeRectangle, Rectangle tile)
        {
            var widthDiff = freeRectangle.Width - tile.Width;
            var heightDiff = freeRectangle.Height - tile.Height;
            var shortSideDiff = Math.Min(widthDiff, heightDiff);
            var longSideDiff = Math.Max(widthDiff, heightDiff);

            return new HeuristicMetrics(longSideDiff, shortSideDiff);
        }

        private MaximalRectanglesTilePlacement FindTilePlacementBestShortSideFit(Rectangle tile)
        {
            var bestFreeRectangle = Rectangle.Empty;
            var bestPlacement = Rectangle.Empty;
            var bestMetrics = new HeuristicMetrics(int.MaxValue, int.MaxValue);
            foreach (var freeRectangle in FreeRectangles)
            {
                if (freeRectangle.CanContain(tile))
                {
                    var metrics = CalculateBestShortSideFitMetrics(freeRectangle, tile);

                    if (metrics.IsBetter(bestMetrics))
                    {
                        bestFreeRectangle = freeRectangle;
                        bestPlacement = new Rectangle(freeRectangle.X, freeRectangle.Y, tile.Width, tile.Height);
                        bestMetrics = metrics;
                    }
                }
            }

            return new MaximalRectanglesTilePlacement(tile, bestFreeRectangle, bestPlacement, bestMetrics);
        }

        private HeuristicMetrics CalculateBestShortSideFitMetrics(Rectangle freeRectangle, Rectangle tile)
        {
            var widthDiff = freeRectangle.Width - tile.Width;
            var heightDiff = freeRectangle.Height - tile.Height;
            var shortSideDiff = Math.Min(widthDiff, heightDiff);
            var longSideDiff = Math.Max(widthDiff, heightDiff);

            return new HeuristicMetrics(shortSideDiff, longSideDiff);
        }

        #endregion
    }

    public enum MaximalRectanglesHeuristic
    {
        BestShortSideFit,
        BestLongSideFit,
        BestAreaFit,
        BottomLeftRule,
        ContactPointRule
    };
}
