using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MarkdownMonster;
using Westwind.Utilities;

namespace PanDocMarkdownParserAddin
{
    public class PandocMarkdownParser : IMarkdownParser
    {
        private PandocAddinConfiguration Configuration = PandocAddinConfiguration.Current;

        public string Output { get; set; }
        public string ErrorOutput { get; set; }


        public string Parse(string markdown)
        {
            var tfileIn = Path.ChangeExtension(Path.GetTempFileName(), ".md");
            var tfileOut = Path.ChangeExtension(Path.GetTempFileName(), ".html");

            File.WriteAllText(tfileIn, markdown);

            var cmdLine = Configuration.PandocCommandLine.Replace("{fileIn}", tfileIn).Replace("{fileOut}", tfileOut);

            var pi = new ProcessStartInfo("Pandoc.exe")
            {
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                Arguments = cmdLine,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            StringBuilder sb = new StringBuilder();

            var process = Process.Start(pi);

            Output = process.StandardOutput.ReadToEnd();
            ErrorOutput = process.StandardError.ReadToEnd();

            if (!string.IsNullOrEmpty(ErrorOutput))
                throw new InvalidOperationException(ErrorOutput);

            bool res = process.WaitForExit(10000);
            
            string html = null;
            if (File.Exists(tfileOut))
            {
                html = File.ReadAllText(tfileOut);
                html = StringUtils.ExtractString(html, "<body>\r\n", "\r\n</body>");
                //html = "<small style='color: steelblue;'>Pandoc Markdown Parser</small><hr/>" + html;
            }

            File.Delete(tfileIn);
            File.Delete(tfileOut);

            return html;

        }


    }
}
