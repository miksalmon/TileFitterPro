using CommandLine;

namespace TileFitter.Models
{
    /// <summary>
    /// Command line arguments of TileFitterPro and TileFitterProConsole.
    /// </summary>
    public class CommandLineArguments
    {
        [Option('I', "InputSet", Required = true, HelpText = "Set input set file path.")]
        public string InputSetFilePath { get; set; }

        [Option("Output", Required = true, HelpText = "Set output file path.")]
        public string OutputFilePath { get; set; }

        [Option("ContainerWidth", Required = true, HelpText = "Set container width.")]
        public int ContainerWidth { get; set; }

        [Option("ContainerHeight", Required = true, HelpText = "Set container height.")]
        public int ContainerHeight { get; set; }

        [Option()]
        public string InputSetFilePaths { get; set; }
    }
}
