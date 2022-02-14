using System;
using System.Collections.Generic;
using TileFitter.Models;

namespace TileFitter.Interfaces
{
    public interface IAlgorithm
    {
        List<Heuristic> Heuristics { get; }

        Container RunOptimally(Container container, Heuristic heuristic);

        Container RunBlindly(Container container, Heuristic heuristic);
    }

    public enum Heuristic
    {
        BestShortSideFit,
        BestLongSideFit,
        BestAreaFit,
        BottomLeftRule
    }
}
