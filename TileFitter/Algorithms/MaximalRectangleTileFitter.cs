using System;
using System.Collections.Generic;
using System.Drawing;
using TileFitter.Interfaces;
using TileFitter.Models;

namespace TileFitter.Algorithms
{
    public class MaximalRectangleTileFitter
    {
        private Container Container { get; }

        private List<Rectangle> NewFreeRectangles { get; } = new List<Rectangle>();

        private List<Rectangle> FreeRectangles { get; } = new List<Rectangle>();

        private List<Rectangle> UsedRectangles { get; } = new List<Rectangle>();

        // Insert
        public Rectangle PlaceTile(Container container, Rectangle tile, MaximalRectangleHeuristic heuristic)
        {
            var placedTile = new Rectangle(0, 0, tile.Width, tile.Height);
            switch (heuristic)
            {
                case MaximalRectangleHeuristic.BestShortSideFit:
                default:
                    placedTile = FindTilePositionByBestShortSideFit(tile);
                    container.AddPlacedTile(placedTile);
                    break;
            }

            if (placedTile == Rectangle.Empty)
            {
                throw new Exception("Couldn't fit tile");
            }

            CalculateNewFreeRectangles(placedTile);

            return new Rectangle();
        }

        // Insert
        public Container PlaceTiles(Container container, IEnumerable<Rectangle> tiles, MaximalRectangleHeuristic freeRectangleChoiceHeuristic)
        {
            foreach (var rectangle in tiles)
            {
                PlaceTile(container, rectangle, freeRectangleChoiceHeuristic);
            }

            return container;
        }

        #region Free Rectangles Management

        // PlaceRect
        private void CalculateNewFreeRectangles(Rectangle placedTile)
        {
            foreach (var freeRectangle in FreeRectangles)
            {
                // if intersect
                if (placedTile.IntersectsWith(freeRectangle))
                {
                    CalculateNewFreeRectangles(freeRectangle, placedTile);
                    FreeRectangles.Remove(freeRectangle);
                }
            }

            UpdateFreeRectanglesList();
            UsedRectangles.Add(placedTile);
        }

        // TODO: review conditions (for new free + other fucked up conditions in algo)
        // SplitFreeNode
        private void CalculateNewFreeRectangles(Rectangle freeRectangle, Rectangle placedTile)
        {
            if (placedTile.X < (freeRectangle.X + freeRectangle.Width) && (placedTile.X + placedTile.Width) > freeRectangle.X)
            {
                // new rectangle at the top
                if (placedTile.Y > freeRectangle.Y && placedTile.Y < freeRectangle.Y + freeRectangle.Height)
                {
                    Rectangle newFreeRectangle = freeRectangle;
                    newFreeRectangle.Height = placedTile.Y - freeRectangle.Y;
                    AddNewFreeRectangle(newFreeRectangle);
                }

                // new rectangle at the bottom
                if (placedTile.Y + placedTile.Height < freeRectangle.Y + freeRectangle.Height)
                {
                    Rectangle newFreeRectangle = freeRectangle;
                    newFreeRectangle.Y = placedTile.Y + placedTile.Height;
                    newFreeRectangle.Height = freeRectangle.Y + freeRectangle.Height - (placedTile.Y + placedTile.Height);
                    AddNewFreeRectangle(newFreeRectangle);
                }
            }

            if (placedTile.Y < (freeRectangle.Y + freeRectangle.Height) && (placedTile.Y + placedTile.Height) > freeRectangle.Y)
            {
                // new rectangle on the left
                if (placedTile.X > freeRectangle.X && placedTile.X < freeRectangle.X + freeRectangle.Width)
                {
                    Rectangle newFreeRectangle = freeRectangle;
                    newFreeRectangle.Width = placedTile.X - freeRectangle.X;
                    AddNewFreeRectangle(newFreeRectangle);
                }

                // new rectangle on the right
                if (placedTile.X + placedTile.Width < freeRectangle.X + freeRectangle.Width)
                {
                    Rectangle newFreeRectangle = freeRectangle;
                    newFreeRectangle.X = placedTile.X + placedTile.Width;
                    newFreeRectangle.Width = freeRectangle.X + freeRectangle.Width - (placedTile.X + placedTile.Width);
                    AddNewFreeRectangle(newFreeRectangle);
                }
            }
        }

        private void AddNewFreeRectangle(Rectangle newFreeRectangle)
        {
            foreach (var otherNewFreeRectangle in NewFreeRectangles)
            {
                // otherNewFreeRectangle already covers all of newFreeRectangle so it is unnecessary
                if (otherNewFreeRectangle.Contains(newFreeRectangle))
                {
                    return;
                }

                // newFreeRectangle already covers all of freeRectangle so it is unnecessary
                if (newFreeRectangle.Contains(otherNewFreeRectangle))
                {
                    FreeRectangles.Remove(otherNewFreeRectangle);
                }
            }
            FreeRectangles.Add(newFreeRectangle);
        }

        // InsertNewFreeRectangle
        private void UpdateFreeRectanglesList()
        {
            foreach (var freeRectangle in FreeRectangles.ToArray())
            {
                foreach (var newFreeRectangle in NewFreeRectangles.ToArray())
                {
                    if (FreeRectangles.Contains(newFreeRectangle))
                    {
                        NewFreeRectangles.Remove(newFreeRectangle);
                    }
                    else
                    {
                        throw new Exception("New Free Rectangle is bigger than older Free Rectangle");
                    }
                }
            }

            FreeRectangles.AddRange(NewFreeRectangles);
            NewFreeRectangles.Clear();
        }

        #endregion

        #region Heuristics

        // FindPositionForNewNodeBestShortSideFit
        private Rectangle FindTilePositionByBestShortSideFit(Rectangle tile)
        {
            // reuse tile instead since by value?
            var bestRectangle = Rectangle.Empty;
            var bestShortSideDiff = int.MaxValue;
            var bestLongSideDiff = int.MaxValue;

            foreach (var freeRectangle in FreeRectangles)
            {
                if (freeRectangle.Width >= tile.Width && freeRectangle.Height >= tile.Height)
                {
                    var widthDiff = freeRectangle.Width - tile.Width;
                    var heightDiff = freeRectangle.Height - tile.Height;
                    var shortSideDiff = Math.Min(widthDiff, heightDiff);
                    var longSideDiff = Math.Max(widthDiff, heightDiff);

                    if (shortSideDiff < bestShortSideDiff || (shortSideDiff == bestShortSideDiff && longSideDiff < bestLongSideDiff))
                    {
                        bestRectangle = new Rectangle(freeRectangle.X, freeRectangle.Y, tile.Width, tile.Height);
                        bestShortSideDiff = shortSideDiff;
                        bestLongSideDiff = longSideDiff;
                    }
                }
            }

            return bestRectangle;
        }

        #endregion
    }

    public enum MaximalRectangleHeuristic
    {
        BestShortSideFit
    }
}
