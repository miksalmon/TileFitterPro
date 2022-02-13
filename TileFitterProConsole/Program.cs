using CommandLine;
using System;
using System.Threading.Tasks;
using TileFitter.Models;
using TileFitter.Services;
using Windows.ApplicationModel.Core;
using Windows.Storage;
using System.Linq;
using System.IO;
using TileFitter.Interfaces;
using System.Collections.Generic;
using TileFitter.Algorithms;
using System.Threading;

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

                var fullPath = Path.GetFullPath(arguments.InputSetFilePath);
                var file = await StorageFile.GetFileFromPathAsync(fullPath);

                var tiles = (await reader.ReadTilesAsync(file)).ToList();

                var container = new Container(arguments.ContainerWidth, arguments.ContainerHeight, tiles);

                var runner = new TileFitterRunner(new List<IAlgorithm>() { new MaximalRectanglesAlgorithm() });

                var cts = new CancellationTokenSource(TimeSpan.FromMinutes(1));
                var cancellationToken = cts.Token;
                var solutions = await runner.FindAllSolutionsAsync(container, cancellationToken);
                var result = solutions.FirstOrDefault();
                if (result != null && result.IsValidSolution())
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
