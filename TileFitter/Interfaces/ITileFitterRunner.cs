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
    internal interface ITileFitterRunner
    {
        IEnumerable<IAlgorithm> Algorithms { get; }

        Task<IEnumerable<Container>> FindAllSolutionsAsync(Container container, CancellationToken cancellationToken);
    }
}
