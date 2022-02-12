using System.Threading.Tasks;
using TileFitter.Models;

namespace TileFitter.Interfaces
{
    internal interface IContainerWriter
    {
        Task WriteOutput(string filePath, Container container);
    }
}
