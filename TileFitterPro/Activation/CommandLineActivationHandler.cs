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
using TileFitterPro.Models;
using Windows.Storage;

namespace TileFitterPro.Activation
{
    internal class CommandLineActivationHandler : ActivationHandler<CommandLineActivatedEventArgs>
    {
        protected override async Task HandleInternalAsync(CommandLineActivatedEventArgs args)
        {
            try
            {
                CommandLineActivationOperation operation = args.Operation;
                var commandLineArgs = operation.Arguments.Split(' ');
                string activationPath = operation.CurrentDirectoryPath;
                var parsedArguments = (Parser.Default.ParseArguments<CommandLineArguments>(commandLineArgs) as Parsed<CommandLineArguments>).Value;

                var reader = new TileReader();

                var fullPath = Path.GetFullPath(Path.Combine(activationPath, parsedArguments.InputSetFilePath));
                var file = await StorageFile.GetFileFromPathAsync(fullPath);

                var tiles = (await reader.ReadTilesAsync(file)).ToList();

                var container = new Container(parsedArguments.ContainerWidth, parsedArguments.ContainerHeight, tiles.ToList());
                var navigationArguments = new TileFitterNavigationArguments { Container = container, Tiles = tiles, InputFile = file };
                NavigationService.Navigate(typeof(MainPage), navigationArguments);
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
            return args?.Operation.Arguments.Any() ?? false;
        }
    }
}
