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

        public bool IsValidSolution => ValidateSolution();

        public string GetPlacedTilesString() => string.Join('\n', PlacedTiles.Select(x => x.ToOutputFormat()));

        public bool ValidateSolution()
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
    }
}
