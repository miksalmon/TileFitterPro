using System;

using TileFitterPro.ViewModels;

using Windows.UI.Xaml.Controls;

namespace TileFitterPro.Views
{
    public sealed partial class MainPage : Page
    {
        public MainViewModel ViewModel { get; } = new MainViewModel();

        public MainPage()
        {
            InitializeComponent();
        }
    }
}
