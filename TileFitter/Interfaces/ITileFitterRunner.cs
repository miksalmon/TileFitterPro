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
        Task<IEnumerable<Container>> FindAllSolutionsAsync(Container container);

        Container FindFastestSolution(Container container);

        Container FindSolution(Container container, TileFitterOptions options);

        Container RunMaximalRectangles(Container container, Heuristic heuristic);

        MaximalRectanglesHeuristic GetMaximalRectanglesHeuristic(Heuristic heurtic);
    }
}
