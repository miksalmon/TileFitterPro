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

namespace TileFitterPro.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        private const int MAX_RGB = 200;
        private Random random = new Random();

        public List<int> Sizes { get; } = new List<int>() { 6, 8, 10, 12, 16, 20, 30, 40, 50, 100, 200, 500, 1000 };

        private Container _container;
        public Container Container
        {
            get { return _container; }
            set { SetProperty(ref _container, value); Reset(); }
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

        private bool? _result;
        public bool? Result
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

        public List<UiTile> Solution { get; set; }
        public CanvasControl CanvasElement { get; set; }
        public Grid CanvasContainer { get; set; }

        ICommand _runCommand;
        public ICommand RunCommand => _runCommand ?? (_runCommand = new AsyncRelayCommand(OnRunCommandAsync));

        ICommand _importCsvCommand;
        public ICommand ImportCommand => _importCsvCommand ?? (_importCsvCommand = new AsyncRelayCommand(OnImportCsvCommandAsync));

        public MainViewModel()
        {
            TilesToPlace = new List<Rectangle>();
            Container = new Container(Sizes[0], Sizes[0], new List<Rectangle>());
            Solution = new List<UiTile>();
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
                    // TODO: add error message
                    CurrentFile = null;
                    TilesToPlace.Clear();
                    Reset();
                }
            }
        }

        public async Task OnRunCommandAsync()
        {
            var container = new Container(Container.Width, Container.Height, TilesToPlace);

            var runner = new TileFitterRunner();
            var solutions = await runner.FindAllSolutionsAsync(container);

            BuildSolution(solutions);

            CanvasElement.Invalidate();
        }

        public bool CanRunCommand(Container container, List<Rectangle> tilestoPlace)
        {
            if (container.Width == 0 || container.Height == 0 || tilestoPlace.Count == 0)
            {
                return false;
            }
            return true;
        }

        private void BuildSolution(IEnumerable<Container> solutions)
        {
            if (solutions.Any())
            {
                Result = true;
                ResultMessage = $"The tiles from {CurrentFile.Name} fit into a {Container.Width}x{Container.Height} container.";

                var solution = solutions.First();
                foreach (var placedTile in solution.PlacedTiles)
                {
                    Windows.UI.Color color = Windows.UI.Color.FromArgb(byte.MaxValue, (byte)random.Next(MAX_RGB), (byte)random.Next(MAX_RGB), (byte)random.Next(MAX_RGB));
                    Solution.Add(new UiTile { Rectangle = new Rect(placedTile.X, placedTile.Y, placedTile.Width, placedTile.Height), Color = color });
                }
            }
            else
            {
                Result = false;
                ResultMessage = $"The tiles from {CurrentFile.Name} do not fit into a {Container.Width}x{Container.Height} container.";
            }
        }

        internal void DrawSolution(CanvasDrawingSession drawingSession)
        {
            if (Container == null || Solution == null || CanvasContainer == null || CanvasElement == null)
            {
                return;
            }

            double scale = Math.Min(CanvasContainer.ActualWidth / Container.Width, CanvasContainer.ActualHeight / Container.Height);

            var containerRectangle = new Rect(0, 0, Container.Width, Container.Height);
            var scaledContainerRectangle = containerRectangle.Scale(scale);
            drawingSession.FillRectangle(scaledContainerRectangle, Colors.White);
            drawingSession.DrawRectangle(scaledContainerRectangle, Colors.Black, 4);

            foreach (var tile in Solution)
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
            Solution?.Clear();
            CanvasElement?.Invalidate();
        }
    }
}
