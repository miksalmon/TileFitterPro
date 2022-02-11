using CommandLine;
using System;
using System.Threading.Tasks;
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
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            
        }
    }
}
