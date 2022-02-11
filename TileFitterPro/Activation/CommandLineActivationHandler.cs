using System.Linq;
using System.Threading.Tasks;

using TileFitterPro.Services;
using TileFitterPro.Views;

using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using CommandLine;
using TileFitter.Models;

namespace TileFitterPro.Activation
{
    internal class CommandLineActivationHandler : ActivationHandler<CommandLineActivatedEventArgs>
    {
        // Learn more about these EventArgs at https://docs.microsoft.com/en-us/uwp/api/windows.applicationmodel.activation.commandlineactivatedeventargs
        protected override async Task HandleInternalAsync(CommandLineActivatedEventArgs args)
        {
            CommandLineActivationOperation operation = args.Operation;

            // Because these are supplied by the caller, they should be treated as untrustworthy.
            var commandLineArgs = operation.Arguments.Split(' ');

            // The directory where the command-line activation request was made.
            // This is typically not the install location of the app itself, but could be any arbitrary path.
            string activationPath = operation.CurrentDirectoryPath;

            var arguments = (Parser.Default.ParseArguments<CommandLineArguments>(commandLineArgs) as Parsed<CommandLineArguments>).Value;

            NavigationService.Navigate(typeof(MainPage), commandLineArgs);

            await Task.CompletedTask;
        }

        protected override bool CanHandleInternal(CommandLineActivatedEventArgs args)
        {
            // Only handle a commandline launch if arguments are passed.
            return args?.Operation.Arguments.Any() ?? false;
        }
    }
}
