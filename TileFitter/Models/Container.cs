using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace TileFitter.Models
{
    public class Container
    {
        public List<Rectangle> RemainingTiles { get; } = new List<Rectangle>();

        public List<Rectangle> PlacedTiles { get; } = new List<Rectangle>();

        public int Width { get; set; }

        public int Height { get; set; }

        public int Area => Width * Height;

        public bool IsFilled => RemainingTiles.Any();
    }
}
