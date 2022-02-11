using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TileFitter.Interfaces;
using Windows.ApplicationModel.Core;
using Windows.Storage;

namespace TileFitter.Services
{
    public class TileReader : ITileReader
    {
        public async Task<IEnumerable<Rectangle>> ReadTilesAsync(string filePath)
        {
            var tiles = new List<Rectangle>();

            try
            {
                var fullPath = Path.GetFullPath(filePath);
                var file = await StorageFile.GetFileFromPathAsync(fullPath);

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
            }
            catch(UnauthorizedAccessException ex)
            {
                Console.WriteLine($"File system access denied. Please make sure to enable TileFitterPro App permissions for file system access.");
                Console.ReadLine();
                CoreApplication.Exit();

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}. Press any key to exit the app.");
                Console.ReadLine();
                CoreApplication.Exit();
            }
            

            return tiles;
        }
    }
}
