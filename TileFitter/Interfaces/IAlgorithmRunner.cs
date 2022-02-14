using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TileFitter.Models;

namespace TileFitter.Interfaces
{
    /// <summary>
    /// Interface of an algorithm runner defining algorithm execution behaviors.
    /// </summary>
    public interface IAlgorithmRunner
    {
        List<Task<Container>> RunAllHeuristicsAsync(Container container, CancellationToken cancellationToken);
    }
}
