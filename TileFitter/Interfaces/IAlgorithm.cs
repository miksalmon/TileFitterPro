using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TileFitter.Models;

namespace TileFitter.Interfaces
{
    public interface IAlgorithm
    {
        Task<Container> ExecuteAllHeuristicsUntilFirstSuccess(Container container, CancellationToken cancellationToken);

        List<Task<Container>> ExecuteAllHeuristicsAsync(Container container, CancellationToken cancellationToken);
    }
}
