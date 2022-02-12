using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TileFitter.Algorithms;
using TileFitter.Models;

namespace TileFitter.Interfaces
{
    internal interface ITileFitterRunner
    {
        Container RunAll(Container container);

        Container Run(Container container, TileFitterOptions options);

        Container RunMaximalRectangles(Container container, TileFitterOptions options);

        MaximalRectanglesHeuristic GetMaximalRectanglesHeuristic(Heuristic heurtic);
    }
}
