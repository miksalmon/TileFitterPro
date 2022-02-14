using Microsoft.Toolkit.Mvvm.ComponentModel;
using TileFitter.Models;
using System.Drawing;
using System.Collections.Generic;
using System.Windows.Input;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Microsoft.Graphics.Canvas;
using System;
using Microsoft.Graphics.Canvas.Brushes;
using Windows.UI;
using TileFitterPro.Models;
using Windows.Foundation;
using TileFitterPro.Helpers;
using System.Threading.Tasks;
using TileFitter.Services;
using System.Linq;
using System.Numerics;
using Windows.Storage;
using TileFitter.Interfaces;
using TileFitter.Algorithms;
using System.Threading;

namespace TileFitterPro.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        private double PACKING_RATIO = 0.95;
        private const int MAX_RGB = 200;
        private Random random = new Random();

        public List<int> Sizes { get; } = new List<int>() { 6, 8, 10, 12, 16, 20, 30, 40, 50, 100, 200, 500, 1000 };

        public List<RectangleDensity> RectanglesDensities = new List<RectangleDensity>()
            { RectangleDensity.Low, RectangleDensity.Medium, RectangleDensity.High };

        private Container _container;
        public Container Container
        {
            get { return _container; }
            set { SetProperty(ref _container, value); Reset(); }
        }

        private RectangleDensity _rectanglesToGenerateDensity;
        public RectangleDensity RectanglesToGenerateDensity
        {
            get { return _rectanglesToGenerateDensity; }
            set { SetProperty(ref _rectanglesToGenerateDensity, value); Reset(); }
        }

        private List<Rectangle> _tilesToPlace;
        public List<Rectangle> TilesToPlace
        {
            get { return _tilesToPlace; }
            set { SetProperty(ref _tilesToPlace, value); }
        }

        private StorageFile _currentFile;
        public StorageFile CurrentFile
        {
            get { return _currentFile; }
            set { SetProperty(ref _currentFile, value); }
        }

        private ResultEnum _result;
        public ResultEnum Result
        {
            get { return _result; }
            set { SetProperty(ref _result, value); }
        }

        private string _resultMessage;
        public string ResultMessage
        {
            get { return _resultMessage; }
            set { SetProperty(ref _resultMessage, value); }
        }

        public List<UiTile> TilesToDisplay { get; set; }

        public CanvasControl CanvasElement { get; set; }

        public Grid CanvasContainer { get; set; }

        private ICommand _runCommand;
        public ICommand RunCommand => _runCommand ?? (_runCommand = new AsyncRelayCommand(OnRunCommandAsync));

        private ICommand _importCsvCommand;
        public ICommand ImportCommand => _importCsvCommand ?? (_importCsvCommand = new AsyncRelayCommand(OnImportCsvCommandAsync));

        private ICommand _exportCsvCommand;
        public ICommand ExportCommand => _exportCsvCommand ?? (_exportCsvCommand = new AsyncRelayCommand(OnExportCsvCommandAsync));

        private ICommand _generateCommand;
        public ICommand GenerateCommand =>
            _generateCommand ?? (_generateCommand = new RelayCommand(OnGenerateCommand));

        public MainViewModel()
        {
            TilesToPlace = new List<Rectangle>();
            Container = new Container(Sizes[0], Sizes[0], new List<Rectangle>());
            TilesToDisplay = new List<UiTile>();
        }

        public async Task OnImportCsvCommandAsync()
        {
            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.FileTypeFilter.Add(".csv");
            var file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                CurrentFile = file;
                try
                {
                    var tileReader = new TileReader();
                    TilesToPlace = (await tileReader.ReadTilesAsync(CurrentFile)).ToList();
                    Reset();
                }
                catch (Exception)
                {
                    var dialog = new ContentDialog
                    {
                        Content = "File could not be read.",
                        CloseButtonText = "Close",
                        DefaultButton = ContentDialogButton.Close
                    };
                    var task = dialog.ShowAsync();

                    CurrentFile = null;
                    TilesToPlace.Clear();
                    Reset();
                }
            }
        }

        public async Task OnExportCsvCommandAsync()
        {
            var picker = new Windows.Storage.Pickers.FileSavePicker();
            picker.SuggestedStartLocation =
                Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;
            picker.FileTypeChoices.Add("CSV", new List<string>() { ".csv" });
            picker.SuggestedFileName = $"{CurrentFile?.DisplayName ?? "generated"}_{Container.Width}_{Container.Height}_solution";

            var file = await picker.PickSaveFileAsync();
            if(file != null)
            {
                try
                {
                    var containerWriter = new ContainerWriter();
                    await containerWriter.WriteOutput(file, Container);
                    // Reset?
                }
                catch(Exception e)
                {
                    var dialog = new ContentDialog
                    {
                        Content = "File could not be written.",
                        CloseButtonText = "Close",
                        DefaultButton = ContentDialogButton.Close
                    };
                    var task = dialog.ShowAsync();
                }
            }
        }

        public async Task OnRunCommandAsync()
        {
            Container = new Container(Container.Width, Container.Height, TilesToPlace);

            var runner = new TileFitterRunner(new List<IAlgorithmRunner>() { new MaximalRectanglesAlgorithmRunner() });

            var cts = new CancellationTokenSource(TimeSpan.FromMinutes(1));
            var cancellationToken = cts.Token;
            var solutions = await runner.FindAllSolutionsAsync(Container, cancellationToken);
            var solution = solutions.FirstOrDefault();
            if (solution != null)
            {
                Container = solution;
                BuildTilesToDisplay(solution.PlacedTiles);

                Result = ResultEnum.Success;
                ResultMessage = $"The tiles from {CurrentFile?.Name ?? "generated input"} fit into a {Container.Width}x{Container.Height} container.";
            }
            else
            {
                Result = ResultEnum.Failure;
                ResultMessage = $"The tiles from {CurrentFile?.Name ?? "generated input"} do not fit into a {Container.Width}x{Container.Height} container.";
            }
        }

        public void OnGenerateCommand()
        {
            Container = new Container(Container.Width, Container.Height, new List<Rectangle>());
            Container.GenerateValidRemainingTiles(GetNumberOfRectanglesToGenerate(), PACKING_RATIO);
            TilesToPlace = Container.RemainingTiles;
            BuildTilesToDisplay(Container.RemainingTiles);
        }

        public bool CanRunCommand(Container container, List<Rectangle> tilesToPlace)
        {
            if (container.Width == 0 || container.Height == 0 || tilesToPlace.Count == 0)
            {
                return false;
            }
            return true;
        }

        public bool CanExportCommand(Container container)
        {
            return container.PlacedTiles.Any() && !container.RemainingTiles.Any();
        }

        private void BuildTilesToDisplay(List<Rectangle> tilesToDisplay)
        {
            Reset();
            foreach (var tile in tilesToDisplay)
            {
                Windows.UI.Color color = Windows.UI.Color.FromArgb(byte.MaxValue, (byte)random.Next(MAX_RGB), (byte)random.Next(MAX_RGB), (byte)random.Next(MAX_RGB));
                TilesToDisplay.Add(new UiTile { Rectangle = new Rect(tile.X, tile.Y, tile.Width, tile.Height), Color = color });
            }

            CanvasElement.Invalidate();
        }

        internal void DrawTiles(CanvasDrawingSession drawingSession)
        {
            if (Container == null || TilesToDisplay == null || CanvasContainer == null || CanvasElement == null)
            {
                return;
            }

            double scale = Math.Min(CanvasContainer.ActualWidth / Container.Width, CanvasContainer.ActualHeight / Container.Height);

            var containerRectangle = new Rect(0, 0, Container.Width, Container.Height);
            var scaledContainerRectangle = containerRectangle.Scale(scale);
            drawingSession.FillRectangle(scaledContainerRectangle, Colors.White);
            drawingSession.DrawRectangle(scaledContainerRectangle, Colors.Black, 4);

            foreach (var tile in TilesToDisplay)
            {
                var brush = new CanvasSolidColorBrush(CanvasElement.Device, tile.Color);
                var scaledTile = tile.Rectangle.Scale(scale);
                drawingSession.FillRectangle(scaledTile, brush);
                drawingSession.DrawRectangle(scaledTile, Colors.Black, 2);
            }

            if (Container.Width <= 100 && Container.Height <= 100)
            {
                for (int i = 0; i < Container.Width; i++)
                {
                    float x = (float)(i * scale);
                    float y = (float)(Container.Height * scale);
                    drawingSession.DrawLine(new Vector2(x, 0), new Vector2(x, y), Colors.Black);
                }

                for (int i = 0; i < Container.Height; i++)
                {
                    float x = (float)(Container.Width * scale);
                    float y = (float)(i * scale);
                    drawingSession.DrawLine(new Vector2(0, y), new Vector2(x, y), Colors.Black);
                }
            }
        }

        private void Reset()
        {
            Result = ResultEnum.Undefined;
            TilesToDisplay?.Clear();
            CanvasElement?.Invalidate();
        }

        private int GetNumberOfRectanglesToGenerate()
        {
            switch (RectanglesToGenerateDensity)
            {
                case RectangleDensity.Low:
                    return (int)Math.Sqrt(Container.Area) / 2;
                case RectangleDensity.Medium:
                default:
                    return (int)Math.Sqrt(Container.Area);
                case RectangleDensity.High:
                    return (int)Math.Sqrt(Container.Area);
            }
        }
    }

    public enum ResultEnum
    {
        Undefined,
        Success,
        Failure
    }

    public enum RectangleDensity
    {
        Low,
        Medium,
        High
    }
}
