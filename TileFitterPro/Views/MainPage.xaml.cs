using System;
using System.Linq;
using TileFitter.Models;
using TileFitterPro.Models;
using TileFitterPro.ViewModels;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace TileFitterPro.Views
{
    public sealed partial class MainPage : Page
    {
        public MainViewModel ViewModel { get; } = new MainViewModel();

        public MainPage()
        {
            InitializeComponent();
        }

        public bool ResultToBool(ResultEnum result, bool invert)
        {
            switch(result)
            {
                case ResultEnum.Success:
                    return invert ? false : true;
                case ResultEnum.Failure:
                    return invert ? true : false;
                case ResultEnum.Undefined:
                default:
                    return false;
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var navigationArguments = e.Parameter as TileFitterNavigationArguments;
            if (navigationArguments != null)
            {
                ViewModel.Container = navigationArguments.Container;
                ViewModel.TilesToPlace = navigationArguments.Tiles;
                ViewModel.CurrentFile = navigationArguments.InputFile;
            }

            ViewModel.CanvasElement = CanvasElement;
            ViewModel.CanvasContainer = CanvasContainer;
        }

        private void Canvas_Draw(Microsoft.Graphics.Canvas.UI.Xaml.CanvasControl sender, Microsoft.Graphics.Canvas.UI.Xaml.CanvasDrawEventArgs args)
        {
            ViewModel.DrawTiles(args.DrawingSession);
        }

        private void Canvas_SizeChanged(object sender, Windows.UI.Xaml.SizeChangedEventArgs e)
        {
            ViewModel.CanvasElement?.Invalidate();
        }

        private void WidthComboBox_TextSubmitted(ComboBox sender, ComboBoxTextSubmittedEventArgs args)
        {
            int width = SizeComboBox_TextSubmitted(sender) ?? ViewModel.Container.Width;
            ViewModel.Container = new Container(width, ViewModel.Container.Height, ViewModel.TilesToPlace);
        }

        private void HeightComboBox_TextSubmitted(ComboBox sender, ComboBoxTextSubmittedEventArgs args)
        {
            int height = SizeComboBox_TextSubmitted(sender) ?? ViewModel.Container.Height;
            ViewModel.Container = new Container(ViewModel.Container.Width, height, ViewModel.TilesToPlace);
        }

        private int? SizeComboBox_TextSubmitted(ComboBox sender)
        {
            bool isInt = int.TryParse(sender.Text, out int newValue);

            if (isInt && (ViewModel.Sizes.Contains(newValue) || newValue > 0))
            {
                sender.SelectedItem = newValue;
                return newValue;
            }
            else
            {
                sender.Text = sender.SelectedValue.ToString();

                var dialog = new ContentDialog
                {
                    Content = "Please enter a valid size.",
                    CloseButtonText = "Close",
                    DefaultButton = ContentDialogButton.Close
                };
                var task = dialog.ShowAsync();

                return null;
            }
        }

        private void WidthComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.FirstOrDefault() is int item)
            {
                ViewModel.Container = new Container(item, ViewModel.Container.Height, ViewModel.TilesToPlace);
            }
        }

        private void HeightComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.FirstOrDefault() is int item)
            {
                ViewModel.Container = new Container(ViewModel.Container.Width, item, ViewModel.TilesToPlace);
            }
        }

        private void NumberOfRectanglesToGenerate_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
            if (Enum.TryParse(e.AddedItems.FirstOrDefault() as string, out RectangleDensity density))
            {
                ViewModel.RectanglesToGenerateDensity = density;
            }
        }
    }
}
