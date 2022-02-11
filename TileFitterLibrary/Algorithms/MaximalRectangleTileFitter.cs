using System;
using System.Collections.Generic;
using System.Drawing;
using TileFitter.Interfaces;
using TileFitter.Models;

namespace TileFitter.Algorithm
{
    class MaximalRectangleTileFitter : ITileFitter
    {
        private Container Container { get; } = new Container();

        private List<Rectangle> NewFreeRectangles { get; } = new List<Rectangle>();

        private List<Rectangle> FreeRectangles { get; } = new List<Rectangle>();

        private List<Rectangle> UsedRectangles { get; } = new List<Rectangle>();

        public Rectangle PlaceTile(Rectangle tile, MaximalRectangleHeuristic heuristic)
        {
            // reuse tile instead since by value?
            var placedTile = new Rectangle();
            switch(heuristic)
            {
                case MaximalRectangleHeuristic.BestShortSideFit:
                default:
                    placedTile = FindTilePositionByBestShortSideFit(tile);
                    break;

            }

            if (placedTile == Rectangle.Empty)
            {
                throw new Exception("Couldn't fit tile");
            }

            CalculateNewFreeRectangles(placedTile);

            return new Rectangle();
        }

        public Container PlaceTiles(List<Rectangle> tile, MaximalRectangleHeuristic freeRectangleChoiceHeuristic)
        {
            throw new System.NotImplementedException();
        }

        #region Free Rectangles Management

        private void CalculateNewFreeRectangles(Rectangle placedTile)
        {
            foreach(var freeRectangle in FreeRectangles)
            {
                // if intersect
                if(placedTile.IntersectsWith(freeRectangle))
                {
                    CalculateNewFreeRectangles(freeRectangle, placedTile);
                    FreeRectangles.Remove(freeRectangle);
                }
            }

            UpdateFreeRectanglesList();
            UsedRectangles.Add(placedTile);
        }

        // TODO: review conditions (for new free + other fucked up conditions in algo)
        private void CalculateNewFreeRectangles(Rectangle freeRectangle, Rectangle placedTile)
        {
            // new rectangle at the top
            if(placedTile.Y > freeRectangle.Y && placedTile.Y < freeRectangle.Y + freeRectangle.Height)
            {
                Rectangle newFreeRectangle = freeRectangle;
                newFreeRectangle.Height = placedTile.Y - freeRectangle.Y;
                AddNewFreeRectangle(newFreeRectangle);
            }

            // new rectangle at the bottom
            if (placedTile.Y + placedTile.Height < freeRectangle.Y + freeRectangle.Height && placedTile.Y < freeRectangle.Y + freeRectangle.Height)
            {
                Rectangle newFreeRectangle = freeRectangle;
                newFreeRectangle.Height = placedTile.Y - freeRectangle.Y;
                AddNewFreeRectangle(newFreeRectangle);
            }

            // new rectangle on the left
            if (placedTile.X > freeRectangle.X && placedTile.X < freeRectangle.X + freeRectangle.Width)
            {
                Rectangle newFreeRectangle = freeRectangle;
                newFreeRectangle.Height = placedTile.Y + placedTile.Height;
                AddNewFreeRectangle(newFreeRectangle);
            }

            // new rectangle on the right
            if (placedTile.X + placedTile.Width < freeRectangle.X + freeRectangle.Width)
            {
                Rectangle newFreeRectangle = freeRectangle;
                newFreeRectangle.Height = placedTile.Y - freeRectangle.Y;
                AddNewFreeRectangle(newFreeRectangle);
            }
        }

        private void AddNewFreeRectangle(Rectangle newFreeRectangle)
        {
            foreach(var otherNewFreeRectangle in NewFreeRectangles)
            {
                // otherNewFreeRectangle already covers all of newFreeRectangle so it is unnecessary
                if (otherNewFreeRectangle.Contains(newFreeRectangle))
                {
                    return;
                }

                // newFreeRectangle already covers all of freeRectangle so it is unnecessary
                if(newFreeRectangle.Contains(otherNewFreeRectangle))
                {
                    
                }
            }
        }

        private void UpdateFreeRectanglesList()
        {
            foreach(var freeRectangle in FreeRectangles.ToArray())
            {
                foreach(var newFreeRectangle in NewFreeRectangles.ToArray())
                {
                    if(FreeRectangles.Contains(newFreeRectangle))
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

        private Rectangle FindTilePositionByBestShortSideFit(Rectangle tile)
        {
            // reuse tile instead since by value?
            var bestRectangle = Rectangle.Empty;
            var bestShortSideDiff = int.MaxValue;
            var bestLongSideDiff = int.MaxValue;

            foreach(var freeRectangle in FreeRectangles)
            {
                if(freeRectangle.Width >= tile.Width && freeRectangle.Height >= tile.Height)
                {
                    var widthDiff = freeRectangle.Width - tile.Width;
                    var heightDiff = freeRectangle.Height - tile.Height;
                    var shortSideDiff = Math.Min(widthDiff, heightDiff);
                    var longSideDiff = Math.Max(widthDiff, heightDiff);

                    if(shortSideDiff < bestShortSideDiff || (shortSideDiff == bestShortSideDiff && longSideDiff < bestLongSideDiff))
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
}
