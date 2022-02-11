using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;

namespace TileFitter.Interfaces
{
    public interface ITileReader
    {
        Task<IEnumerable<Rectangle>> ReadTilesAsync(string filePath);
    }
}
