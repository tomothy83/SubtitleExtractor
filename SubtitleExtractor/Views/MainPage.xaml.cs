using System;

using SubtitleExtractor.ViewModels;

using Windows.UI.Xaml.Controls;

namespace SubtitleExtractor.Views
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
