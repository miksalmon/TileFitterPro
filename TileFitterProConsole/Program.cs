using CommandLine;
using System;
using System.Threading.Tasks;
using TileFitter.Models;
using TileFitter.Services;
using Windows.ApplicationModel.Core;
using System.Linq;

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

                var tiles = (await reader.ReadTilesAsync(arguments.InputSetFilePath)).ToList();
                var container = new Container(arguments.ContainerWidth, arguments.ContainerHeight, tiles);

                var runner = new TileFitterRunner();
                var result = runner.Run(container, new TileFitterOptions(Algorithm.MaximalRectangles, Heuristic.BestShortSideFit));

                var writer = new ContainerWriter();
                await writer.WriteOutput(arguments.OutputFilePath, result);

                Console.ReadLine();
            }
            catch (UnauthorizedAccessException)
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
