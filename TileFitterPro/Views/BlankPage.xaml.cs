using System;

using TileFitterPro.ViewModels;

using Windows.UI.Xaml.Controls;

namespace TileFitterPro.Views
{
    public sealed partial class BlankPage : Page
    {
        public BlankViewModel ViewModel { get; } = new BlankViewModel();

        public BlankPage()
        {
            InitializeComponent();
        }
    }
}
