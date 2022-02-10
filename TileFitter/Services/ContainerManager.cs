using TileFitter.Interfaces;
using TileFitter.Models;

namespace TileFitter.Services
{
    internal class ContainerManager : IContainerManager
    {
        public ContainerManager(ITileFitter tileFitter)
        {

        }

        public Container FitTiles(Input input)
        {
            // fail quick

            // loop through input rectangles until all rectangles are placed or fail to place one
            // place tile

            return null;
        }
    }
}
