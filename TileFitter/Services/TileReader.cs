using System.Collections.Generic;
using System.Drawing;
using System.IO;
using TileFitter.Interfaces;

namespace TileFitter.Services
{
    public class TileReader : ITileReader
    {
        public IEnumerable<Rectangle> ReadTiles(string filePath)
        {
            var tiles = new List<Rectangle>();
            using (var reader = new StreamReader(filePath))
            {
                // skip first line defining colum names
                reader.ReadLine();

                while(!reader.EndOfStream)
                {
                    var data = reader.ReadLine().Split(',');
                    var tile = new Rectangle()
                    {
                        Width = int.Parse(data[0]),
                        Height = int.Parse(data[1])
                    };
                    tiles.Add(tile);
                }
                
            }

            return tiles;
        }
    }
}
