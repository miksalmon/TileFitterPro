using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TileFitter.Models
{
    public class TileFitterOptions
    {
        public TileFitterOptions(Algorithm algorithm, Heuristic heuristic)
        {
            Algorithm = algorithm;
            Heuristic = heuristic;
        }

        public Algorithm Algorithm { get; }

        public Heuristic Heuristic { get; }
    }

    public enum Algorithm
    {
        MaximalRectangles
    }

    public enum Heuristic
    {
        BestShortSideFit,
        BestLongSideFit,
        BestAreaFit,
        BottomLeftRule
    }
}
