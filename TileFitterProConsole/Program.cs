using CommandLine;
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using TileFitter.Models;
using TileFitter.Services;
using TileFitter.Algorithms;

namespace TileFitterProConsole
{
    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                var arguments = (Parser.Default.ParseArguments<CommandLineArguments>(args) as Parsed<CommandLineArguments>).Value;
                var reader = new TileReader();

                var tiles = await reader.ReadTilesAsync(arguments.InputSetFilePath);

                var tileFitter = new MaximalRectangleTileFitter();

                var container = new Container()
                {
                    Width = arguments.ContainerWidth,
                    Height = arguments.ContainerHeight
                };
                container = tileFitter.PlaceTiles(container, tiles, MaximalRectangleHeuristic.BestShortSideFit);

                Console.WriteLine(container.PlacedTiles);
            }
            catch (UnauthorizedAccessException ex)
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
        }
    }
}
