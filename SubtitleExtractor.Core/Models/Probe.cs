using FFMpegCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace SubtitleExtractor.Core.Models
{
    /// <summary>
    /// Probes video information.
    /// </summary>
    public class Probe
    {
        const int outputCapacity = 2147483647;
        private FFOptions Options { get; set; }

        public Probe(string binFolder, string tempFolder)
        {
            Options = new FFOptions { BinaryFolder = binFolder, TemporaryFilesFolder = tempFolder };
        }

        public async void PrintMetadata(string inputPath)
        {
            var mediaInfo = await FFProbe.AnalyseAsync(inputPath, outputCapacity, Options);
        }
    }
}
