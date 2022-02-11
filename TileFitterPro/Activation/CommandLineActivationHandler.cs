using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using TileFitterPro.Services;
using TileFitterPro.Views;

using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Core;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using CommandLine;
using Microsoft.Toolkit.Mvvm.Input;
using TileFitter.Models;
using TileFitter.Services;
using TileFitterPro.Helpers;

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

            try
            {
                var reader = new TileReader();
                var tiles = reader.ReadTilesAsync(Path.GetFullPath(Path.Combine(activationPath, arguments.InputSetFilePath)));
                NavigationService.Navigate(typeof(MainPage), tiles);
            }
            catch (UnauthorizedAccessException e)
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
