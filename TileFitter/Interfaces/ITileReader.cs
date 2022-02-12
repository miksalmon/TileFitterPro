using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using Windows.Storage;

namespace TileFitter.Interfaces
{
    public interface ITileReader
    {
        Task<IEnumerable<Rectangle>> ReadTilesAsync(StorageFile file);
    }
}
