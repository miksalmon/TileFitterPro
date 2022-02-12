using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace TileFitter.Models
{
    public class Container
    {
        public Container(int width, int height, List<Rectangle> tiles)
        {
            Width = width;
            Height = height;
            RemainingTiles = tiles.Select(x => x).ToList();
            ;
        }

        public List<Rectangle> RemainingTiles { get; } = new List<Rectangle>();

        public List<Rectangle> PlacedTiles { get; } = new List<Rectangle>();

        public int Width { get; set; }

        public int Height { get; set; }

        public int Area => Width * Height;

        public bool IsFilled => !RemainingTiles.Any();

        public string GetPlacedTilesString() => string.Join('\n', PlacedTiles.Select(x => x.ToString()));
    }
}
