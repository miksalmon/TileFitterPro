using CommandLine;
using System;
using TileFitter.Models;
using TileFitter.Services;

namespace TileFitterProConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var arguments = (Parser.Default.ParseArguments<CommandLineArguments>(args) as Parsed<CommandLineArguments>).Value;
            var reader = new TileReader();

            var tiles = reader.ReadTiles(arguments.InputSetFilePath);
        }
    }
}
