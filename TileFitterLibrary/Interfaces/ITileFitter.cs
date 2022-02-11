using System.Collections.Generic;
using System.Drawing;
using TileFitter.Models;

namespace TileFitter.Interfaces
{
    public interface ITileFitter
    {
        Rectangle PlaceTile(Rectangle tile, MaximalRectangleHeuristic heurustic = MaximalRectangleHeuristic.BestShortSideFit);

        Container PlaceTiles(List<Rectangle> tiles, MaximalRectangleHeuristic heuristic = MaximalRectangleHeuristic.BestShortSideFit);
    }

    public enum MaximalRectangleHeuristic
    {
        BestShortSideFit
    }
}
