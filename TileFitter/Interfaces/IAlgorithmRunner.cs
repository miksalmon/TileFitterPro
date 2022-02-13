using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TileFitter.Models;

namespace TileFitter.Interfaces
{
    public interface IAlgorithmRunner
    {
        Task<Container> RunAllHeuristicsUntilFirstSuccessAsync(Container container, CancellationToken cancellationToken);

        List<Task<Container>> RunAllHeuristicsAsync(Container container, CancellationToken cancellationToken);
    }
}
