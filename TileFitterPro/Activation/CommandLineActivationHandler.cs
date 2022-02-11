using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using TileFitterPro.Services;
using TileFitterPro.Views;

using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Core;
using Windows.System;
using Windows.UI.Xaml.Controls;
using CommandLine;
using Microsoft.Toolkit.Mvvm.Input;
using TileFitter.Models;
using TileFitter.Services;

namespace TileFitterPro.Activation
{
    internal class CommandLineActivationHandler : ActivationHandler<CommandLineActivatedEventArgs>
    {
        // Learn more about these EventArgs at https://docs.microsoft.com/en-us/uwp/api/windows.applicationmodel.activation.commandlineactivatedeventargs
        protected override async Task HandleInternalAsync(CommandLineActivatedEventArgs args)
        {
            try
            {
                CommandLineActivationOperation operation = args.Operation;
                var commandLineArgs = operation.Arguments.Split(' ');
                string activationPath = operation.CurrentDirectoryPath;
                var parsedArguments = (Parser.Default.ParseArguments<CommandLineArguments>(commandLineArgs) as Parsed<CommandLineArguments>).Value;

                var reader = new TileReader();
                var tiles = await reader.ReadTilesAsync(Path.Combine(activationPath, parsedArguments.InputSetFilePath));
                NavigationService.Navigate(typeof(MainPage), tiles);

            }
            catch (UnauthorizedAccessException)
            {
                await Launcher.LaunchUriAsync(new Uri("ms-settings:appsfeatures-app"));

                var dialog = new ContentDialog
                {
                    Title = "File System Access Denied",
                    Content = "Please make sure to enable TileFitterPro App permissions for file system access. The app will now close.",
                    SecondaryButtonText = "Ok",
                    SecondaryButtonCommand = new RelayCommand(() => CoreApplication.Exit())
                };

                await dialog.ShowAsync();
            }

            await Task.CompletedTask;
        }

        protected override bool CanHandleInternal(CommandLineActivatedEventArgs args)
        {
            // Only handle a commandline launch if arguments are passed.
            return args?.Operation.Arguments.Any() ?? false;
        }
    }
}
