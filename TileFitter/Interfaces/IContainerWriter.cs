using System.Threading.Tasks;
using TileFitter.Models;

namespace TileFitter.Interfaces
{
    /// <summary>
    /// Interface defining the behavior of output writing of a <see cref="Container"/>.
    /// </summary>
    internal interface IContainerWriter
    {
        Task WriteOutput(string filePath, Container container);
    }
}
