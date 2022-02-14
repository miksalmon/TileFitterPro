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
using System.Drawing;

namespace TileFitterProConsole
{
    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                var arguments = (Parser.Default.ParseArguments<CommandLineArguments>(args) as Parsed<CommandLineArguments>).Value;

                var tiles = await ReadTiles(arguments.InputSetFilePath);

                var container = new Container(arguments.ContainerWidth, arguments.ContainerHeight, tiles);

                var solution = await GetSolution(container);

                if (solution != null && solution.IsValidSolution())
                {
                    await WriteSolution(arguments.OutputFilePath, solution);
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
                Console.WriteLine($"File system access denied. Please make sure to enable TileFitterProConsole App permissions for file system access.");
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

        static async Task<List<Rectangle>> ReadTiles(string inputFilePath)
        {
            var reader = new TileReader();

            var fullPath = Path.GetFullPath(inputFilePath);
            var file = await StorageFile.GetFileFromPathAsync(fullPath);

            var tiles = (await reader.ReadTilesAsync(file)).ToList();

            return tiles;
        }

        static async Task<Container> GetSolution(Container container)
        {
            var runner = new TileFitterRunner(new List<IAlgorithmRunner>() { new AlgorithmRunner<MaximalRectanglesAlgorithm>() });

            var cts = new CancellationTokenSource(TimeSpan.FromMinutes(1));
            var cancellationToken = cts.Token;
            var solutions = await runner.FindAllSolutionsAsync(container, cancellationToken);
            var result = solutions.FirstOrDefault();

            return result;
        }

        static async Task WriteSolution(string outputFilePath, Container result)
        {
            var writer = new ContainerWriter();
            var outputFullPath = Path.GetFullPath(outputFilePath);
            var outputFolderPath = Path.GetDirectoryName(outputFullPath);
            var outputFileName = Path.GetFileName(outputFullPath);

            var outputFolder = await StorageFolder.GetFolderFromPathAsync(outputFolderPath);
            var outputFile = await outputFolder.CreateFileAsync(outputFileName, CreationCollisionOption.ReplaceExisting);
            await writer.WriteOutput(outputFile, result);

            Console.WriteLine($"Success! The solution was written to {Path.GetFullPath(outputFilePath)}");
        }
    }
}
