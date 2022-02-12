using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TileFitter.Models
{
    internal class TilePlacement
    {
        public Rectangle InitialTile { get; }

        public Rectangle FreeRectangle { get; }

        public Rectangle PlacedTile { get; }

        public HeuristicMetrics Metrics { get; }

        public TilePlacement(Rectangle initialTile, Rectangle freeTile, Rectangle placedTile, HeuristicMetrics metrics)
        {
            InitialTile = initialTile;
            FreeRectangle = freeTile;
            PlacedTile = placedTile;
            Metrics = metrics;
        }
    }
}
