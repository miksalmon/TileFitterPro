using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TileFitter.Models;

namespace TileFitter.Interfaces
{
    public interface IAlgorithm
    {
        Task<Container> ExecuteAllUntilSuccess(Container container);

        Task<List<Container>> ExecuteAll(Container container);
    }
}
