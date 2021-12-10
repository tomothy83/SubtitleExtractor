using System;
using System.IO;
using System.Threading.Tasks;
using Windows.ApplicationModel.AppService;
using Windows.Foundation.Collections;

namespace FFWrapper
{
    class Program
    {
        static async Task Main(string[] args)
        {
            AppServiceConnection conn;
            // Add the connection.
            conn = new AppServiceConnection();
            conn.AppServiceName = "InProcessAppService";
            conn.PackageFamilyName = "020c9a10-9ab9-4ff9-86a1-0a6fdd0d81bd_scezaeye0c67e";

            var status = await conn.OpenAsync();

            if (status != AppServiceConnectionStatus.Success)
            {
                return;
            }

            var message = new ValueSet();
            message.Add("Command", "GetJob");
            AppServiceResponse response = await conn.SendMessageAsync(message);
            message.Clear();

            if (response.Status == AppServiceResponseStatus.Success)
            {
                if (response.Message.ContainsKey("Command") && response.Message["Command"] is string cmd)
                {
                    var executor = new FFExecutor();
                    switch (cmd)
                    {
                        case "ExtractSubtitles":
                            string videoFilePath = response.Message["FilePath"] as string;
                            string tempFolderPath = response.Message["TempFolder"] as string;
                            string tempFileName = Path.ChangeExtension(Path.GetRandomFileName(), ".srt");
                            string tempFilePath = Path.Combine(tempFolderPath, tempFileName);
                            bool result = executor.ExtractSubtitle(videoFilePath, tempFilePath);
                            if (result == true)
                            {
                                message.Add("Command", "NotifySubtitlesExtracted");
                                message.Add("SubtitlesFileName", tempFileName);
                            }
                            else
                            {
                                message.Add("Command", "NotifySubtitlesExtractionFailed");
                            }
                            await conn.SendMessageAsync(message);
                            break;
                        default:
                            break;
                    }
                }
            }
            conn.Dispose();
        }

    }
}
