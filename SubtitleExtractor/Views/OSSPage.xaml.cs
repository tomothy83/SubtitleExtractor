using System;

using SubtitleExtractor.ViewModels;

using Windows.UI.Xaml.Controls;

namespace SubtitleExtractor.Views
{
    public sealed partial class OSSPage : Page
    {
        public OSSViewModel ViewModel { get; } = new OSSViewModel();

        public OSSPage()
        {
            InitializeComponent();
        }
    }
}
