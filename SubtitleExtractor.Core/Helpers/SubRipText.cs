using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SubtitleExtractor.Core.Helpers
{
    public static class SubRipText
    {
        enum Section
        {
            Num,
            Time,
            Text,
        }

        /// <summary>
        /// Extract text from srt file content.
        /// </summary>
        /// <param name="srtFileContent">SubRipText file content</param>
        /// <param name="textStream">Output stream</param>
        /// <returns>Extracted text or null</returns>
        public static async Task<string> ExtractText(string srtFileContent)
        {
            using (StringReader reader = new StringReader(srtFileContent))
            {
                StringBuilder sb = new StringBuilder();
                Section section = Section.Num;
                string line;
                while ((line = await reader.ReadLineAsync()) != null)
                {
                    switch (section)
                    {
                        case Section.Num:
                            if (!CheckNumLine(line))
                            {
                                return null;
                            }
                            section = Section.Time;
                            break;
                        case Section.Time:
                            if (!CheckTimeLine(line))
                            {
                                return null;
                            }
                            section = Section.Text;
                            break;
                        case Section.Text:
                            if (CheckBlankLine(line))
                            {
                                section = Section.Num;
                                continue;
                            }
                            sb.AppendLine(RemoveHtmlTags(line));
                            break;
                    }
                }
                return sb.ToString();
            }
        }

        private static bool CheckNumLine(string line)
        {
            string pattern = @"^[0-9]+$";
            Match m = Regex.Match(line, pattern);
            return m.Success;
        }

        private static bool CheckTimeLine(string line)
        {
            string pattern = @"^\d+:\d\d:\d\d,\d\d\d --> \d+:\d\d:\d\d,\d\d\d";
            Match m = Regex.Match(line, pattern);
            return m.Success;
        }

        private static bool CheckBlankLine(string line)
        {
            return line == string.Empty;
        }

        private static string RemoveHtmlTags(string s)
        {
            return Regex.Replace(s, @"<[^>]+>", string.Empty);
        }
    }
}
