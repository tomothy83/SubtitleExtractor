using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using SubtitleExtractor.ViewModels;
using Windows.ApplicationModel;
using Windows.ApplicationModel.AppService;
using Windows.Foundation.Collections;
using Windows.Foundation.Metadata;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;

namespace SubtitleExtractor.Views
{
    public sealed partial class MainPage : Page
    {
        public MainViewModel ViewModel { get; } = new MainViewModel();

        private string TempFolder
        {
            get => ApplicationData.Current.TemporaryFolder.Path;
        }

        public MainPage()
        {
            InitializeComponent();
            if (ApiInformation.IsApiContractPresent("Windows.ApplicationModel.FullTrustAppContract", 1, 0))
            {
                App.AppServiceConnected += MainPage_AppServiceConnected;
            }
        }

        private async void Select_File(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation =
                Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;
            picker.FileTypeFilter.Add(".mp4");
            Windows.Storage.StorageFile file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                this.videoFilePath.Text = file.Path;
            }
        }

        private async void Extract_Subtitles(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (ApiInformation.IsApiContractPresent("Windows.ApplicationModel.FullTrustAppContract", 1, 0))
            {
                await FullTrustProcessLauncher.LaunchFullTrustProcessForCurrentAppAsync();
            }
        }

        /// <summary>
        /// When the desktop process is connected, get ready to send/receive requests
        /// </summary>
        private async void MainPage_AppServiceConnected(object sender, AppServiceTriggerDetails e)
        {
            App.AppServiceConnection.RequestReceived += AppServiceConnection_RequestReceived;
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                btnExtractSubtitles.IsEnabled = false;
            });
        }

        /// <summary>
        /// </summary>
        private async void AppServiceConnection_RequestReceived(AppServiceConnection sender, AppServiceRequestReceivedEventArgs args)
        {
            ValueSet msg = args.Request.Message;
            string command = msg["Command"] as string;
            ValueSet response = new ValueSet();
            switch (command)
            {
                case "GetJob":
                    response = GetJob(msg);
                    break;
                case "NotifySubtitlesExtracted":
                    response = await HandleSubtitles(msg);
                    break;
                default:
                    break;
            }
            await args.Request.SendResponseAsync(response);
        }

        private ValueSet GetJob(ValueSet message)
        {
            ValueSet response = new ValueSet();
            if (message is null)
            {
                return response;
            }
            if (videoFilePath.Text.Length > 0)
            {
                response.Add("Status", "OK");
                response.Add("Command", "ExtractSubtitles");
                response.Add("TempFolder", TempFolder);
                response.Add("FilePath", videoFilePath.Text);
            }
            else
            {
                response.Add("Status", "Error");
            }
            return response;
        }

        private async Task<ValueSet> HandleSubtitles(ValueSet message)
        {
            string subtitlesFilename = message["SubtitlesFileName"] as string;
            string subtitlesFilepath = Path.Combine(TempFolder, subtitlesFilename);
            string subtitles = await File.ReadAllTextAsync(subtitlesFilepath);
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                subtitlesPreview.Text = subtitles;
            });
            ValueSet response = new ValueSet();
            response.Add("Status", "OK");
            return response;
        }
    }
}
