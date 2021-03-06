using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TileFitter.Models
{
    /// <summary>
    /// Model of a tile placement and all relevant data used in the Maximal Rectangles algorithm.
    /// </summary>
    internal class MaximalRectanglesTilePlacement
    {
        public Rectangle InitialTile { get; }

        public Rectangle FreeRectangle { get; }

        public Rectangle PlacedTile { get; }

        public HeuristicMetrics Metrics { get; }

        public MaximalRectanglesTilePlacement(Rectangle initialTile, Rectangle freeTile, Rectangle placedTile, HeuristicMetrics metrics)
        {
            InitialTile = initialTile;
            FreeRectangle = freeTile;
            PlacedTile = placedTile;
            Metrics = metrics;
        }
    }
}
