using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace FFWrapper
{
    internal class FFExecutor
    {
        private string FFMpegPath { get; set; }

        public FFExecutor()
        {
            string appPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string binDir = Path.Combine(Path.GetDirectoryName(appPath), "FFmpeg");
            FFMpegPath = Path.Combine(binDir, "ffmpeg.exe");
        }

        public bool ExtractSubtitle(string mp4FilePath, string srtFilePath)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo(FFMpegPath, $"-i \"{mp4FilePath}\" -map 0:s:0 \"{srtFilePath}\"")
            {
                CreateNoWindow = true,
                UseShellExecute = false
            };
            using (Process proc = new Process())
            {
                proc.StartInfo = startInfo;
                bool ret = proc.Start();
                if (!ret)
                {
                    return false;
                }
                proc.WaitForExit();
                return proc.ExitCode == 0;
            }
        }
    }
}
