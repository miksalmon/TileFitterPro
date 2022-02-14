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
        public async Task WriteOutput(string filePath, Container container)
        {
            var fullPath = Path.GetFullPath(filePath);
            var folderPath = Path.GetDirectoryName(fullPath);
            var fileName = Path.GetFileName(fullPath);

            var folder = await StorageFolder.GetFolderFromPathAsync(folderPath);
            var file = await folder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);

            var resultString = "width,height,top,left\n"+container.GetPlacedTilesString();
            
            await FileIO.WriteTextAsync(file, resultString);
        }
    }
}
