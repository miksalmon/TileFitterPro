using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TileFitter.Extensions;

namespace TileFitter.Models
{
    public class Container
    {
        public Container(int width, int height, List<Rectangle> tiles)
        {
            Width = width;
            Height = height;
            ContainerRectangle = new Rectangle(0, 0, width, height);
            RemainingTiles = tiles.Select(x => x).ToList();
        }

        // TODO: rename
        public Rectangle ContainerRectangle { get; }

        public List<Rectangle> RemainingTiles { get; } = new List<Rectangle>();

        public List<Rectangle> PlacedTiles { get; } = new List<Rectangle>();

        public int Width { get; set; }

        public int Height { get; set; }

        public int Area => Width * Height;

        public int Perimeter => (2 * Width) + (2 * Height);

        public string GetPlacedTilesString() => string.Join('\n', PlacedTiles.Select(x => x.ToOutputFormat()));

        public bool IsValidSolution()
        {
            if(RemainingTiles.Any())
            {
                return false;
            }    

            foreach(var tile in PlacedTiles)
            {
                if(!ContainerRectangle.Contains(tile))
                {
                    return false;
                }

                if(PlacedTiles.Where(x => x != tile).Any(otherTile => tile.IntersectsWith(otherTile)))
                {
                    return false;
                }
            }    

            return true;
        }

        public Container Clone()
        {
            var newContainer = new Container(Width, Height, new List<Rectangle>());
            newContainer.RemainingTiles.AddRange(RemainingTiles);
            newContainer.PlacedTiles.AddRange(PlacedTiles);

            return newContainer;
        }

        public void GenerateValidRemainingTiles(int numberOfTiles, double packingRatio)
        {
            RemainingTiles.Clear();
            int numberOfFailures = 0;
            RemainingTiles.Add(new Rectangle(0, 0, Width, Height));
            var random = new Random();

            while (RemainingTiles.Count < numberOfTiles && numberOfFailures < numberOfTiles)
            {
                var maxArea = RemainingTiles.Max(x => x.GetArea());
                var tileToSplit = RemainingTiles.First(x => x.GetArea() == maxArea);

                bool shouldSplitWidth;

                if (tileToSplit.Width > 1 && tileToSplit.Height > 1)
                {
                    shouldSplitWidth = tileToSplit.Width > tileToSplit.Height;
                }
                else if(tileToSplit.Width > 1)
                {
                    shouldSplitWidth = true;
                }
                else if(tileToSplit.Height > 1)
                {
                    shouldSplitWidth = false;
                }
                else
                {
                    numberOfFailures++;
                    continue;
                }

                numberOfFailures = 0;

                if(shouldSplitWidth)
                {
                    var splitWidth = random.Next(1, tileToSplit.Width);
                    var splitRectangles = tileToSplit.SplitAlongWidth(splitWidth);
                    RemainingTiles.Add(splitRectangles.rectangle1);
                    RemainingTiles.Add(splitRectangles.rectangle2);
                }
                else
                {
                    var splitHeight = random.Next(1, tileToSplit.Height);
                    var splitRectangles = tileToSplit.SplitAlongHeight(splitHeight);
                    RemainingTiles.Add(splitRectangles.rectangle1);
                    RemainingTiles.Add(splitRectangles.rectangle2);
                }

                RemainingTiles.Remove(tileToSplit);
            }

            var randomIndex = random.Next(0, RemainingTiles.Count);
            RemainingTiles.RemoveAt(randomIndex);
            randomIndex = random.Next(0, RemainingTiles.Count);
            RemainingTiles.RemoveAt(randomIndex);
            randomIndex = random.Next(0, RemainingTiles.Count);
            RemainingTiles.RemoveAt(randomIndex);
        }
    }
}
