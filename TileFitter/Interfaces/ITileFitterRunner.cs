using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TileFitter.Algorithms;
using TileFitter.Models;

namespace TileFitter.Interfaces
{
    /// <summary>
    /// Interface of a tile fitter runner defining the behavior of executing all algorithms and getting their solution(s).
    /// </summary>
    internal interface ITileFitterRunner
    {
        IEnumerable<IAlgorithmRunner> AlgorithmRunners { get; }

        Task<IEnumerable<Container>> FindAllSolutionsAsync(Container container, CancellationToken cancellationToken);
    }
}
