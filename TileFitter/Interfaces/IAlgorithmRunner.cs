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
        List<Task<Container>> RunAllHeuristicsAsync(Container container, CancellationToken cancellationToken);
    }
}
