using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using Windows.Storage;

namespace TileFitter.Interfaces
{
    /// <summary>
    /// Interface for defining the behavior of a reader of tiles from a file.
    /// </summary>
    public interface ITileReader
    {
        Task<IEnumerable<Rectangle>> ReadTilesAsync(StorageFile file);
    }
}
