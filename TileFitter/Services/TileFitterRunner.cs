using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TileFitter.Algorithms;
using TileFitter.Interfaces;
using TileFitter.Models;

namespace TileFitter.Services
{
    public class TileFitterRunner : ITileFitterRunner
    {
        public Container Run(Container container, TileFitterOptions options)
        {
            // TODO: copy container?
            switch(options.Algorithm)
            {
                case Algorithm.MaximalRectangles:
                default:
                    return RunMaximalRectangles(container, options);
            }
        }

        public Container RunAll(Container container)
        {
            throw new NotImplementedException();
        }

        #region MaximalRectangles

        public Container RunMaximalRectangles(Container container, TileFitterOptions options)
        {
            var tileFitter = new MaximalRectanglesTileFitter();
            var heuristic = GetMaximalRectanglesHeuristic(options.Heuristic);
            var result = tileFitter.FitTiles(container, heuristic);
            return result;
        }

        public MaximalRectanglesHeuristic GetMaximalRectanglesHeuristic(Heuristic heuristic)
        {
            switch (heuristic)
            {
                case Heuristic.BottomLeftRule:
                    return MaximalRectanglesHeuristic.BottomLeftRule;
                case Heuristic.BestAreaFit:
                    return MaximalRectanglesHeuristic.BestAreaFit;
                case Heuristic.BestLongSideFit:
                    return MaximalRectanglesHeuristic.BestLongSideFit;
                case Heuristic.BestShortSideFit:
                    return MaximalRectanglesHeuristic.BestShortSideFit;
                default:
                    throw new InvalidCastException("Invalid heuristic for MaximalRectangles algorithm.");
            }
        }

        #endregion
    }
}
