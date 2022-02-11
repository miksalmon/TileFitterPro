using System;
using System.Collections.Generic;
using System.Drawing;

namespace TileFitter.Models
{
    public class Container
    {
        public IList<Rectangle> PlacedTiles { get; } = new List<Rectangle>();

        public int Width { get; set; }

        public int Height { get; set; }

        public void AddPlacedTile(Rectangle tile)
        {
            PlacedTiles.Add(tile);
        }
    }
}
