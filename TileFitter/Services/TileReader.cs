using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TileFitter.Interfaces;
using TileFitter.Models;
using Windows.ApplicationModel.Core;
using Windows.Storage;

namespace TileFitter.Services
{
    /// <inheritdoc cref="ITileReader"/>
    public class TileReader : ITileReader
    {
        public async Task<IEnumerable<Rectangle>> ReadTilesAsync(StorageFile file)
        {
            var tiles = new List<Rectangle>();

            var lines = await FileIO.ReadLinesAsync(file);

            // skip first line defining colum names
            lines = lines.Skip(1).ToList();

            foreach (var line in lines)
            {
                var data = line.Split(',');
                var tile = new Rectangle()
                {
                    Width = int.Parse(data[0]),
                    Height = int.Parse(data[1])
                };
                tiles.Add(tile);
            }

            return tiles;
        }
    }
}
