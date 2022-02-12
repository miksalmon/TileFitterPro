using CommandLine;
using System;
using System.Threading.Tasks;
using TileFitter.Models;
using TileFitter.Services;
using Windows.ApplicationModel.Core;
using System.Linq;
using System.IO;

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

                var result = runner.FindFastestSolution(container);

                if(result.IsValidSolution)
                {
                    var writer = new ContainerWriter();
                    await writer.WriteOutput(arguments.OutputFilePath, result);

                    Console.WriteLine($"Sucess! The solution was written to {Path.GetFullPath(arguments.OutputFilePath)}");
                }
                else
                {
                    Console.WriteLine("Failure! Impossible to fit all tiles in container.");
                }

                Console.WriteLine("Press any key to exit application...");
                Console.ReadLine();
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine($"File system access denied. Please make sure to enable TileFitterPro App permissions for file system access.");
                Console.WriteLine("Press any key to exit application...");
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
