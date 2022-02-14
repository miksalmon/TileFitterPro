using System;
using System.IO;
using System.Threading.Tasks;
using TileFitter.Interfaces;
using TileFitter.Models;
using Windows.Storage;

namespace TileFitter.Services
{
    /// <inheritdoc cref="IContainerWriter"/>
    public class ContainerWriter : IContainerWriter
    {
        public async Task WriteOutput(StorageFile file, Container container)
        {
            var resultString = "width,height,top,left\n"+container.GetPlacedTilesString();
            
            await FileIO.WriteTextAsync(file, resultString);
        }
    }
}
