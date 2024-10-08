﻿using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using MarkdownMonster;
using Westwind.Utilities;

namespace PanDocMarkdownParserAddin
{
    public class PandocMarkdownParser : MarkdownParserBase, IMarkdownParser
    {
        private PandocAddinConfiguration Configuration = PandocAddinConfiguration.Current;

        public string Output { get; set; }
        public string ErrorOutput { get; set; }


        public override string Parse(string markdown)
        {           
            var tfileIn = Path.ChangeExtension(Path.GetTempFileName(), ".md");
            var tfileOut = Path.ChangeExtension(tfileIn, ".html");

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

            Process process;
            try
            {
                process = Process.Start(pi);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(
                    "Couldn't start PanDoc. Most likely Pandoc is not installed and not registered on the global path.\n" +
                    " Make sure you install it before running this addin.\n" +
                    " Error Info: " + ex.GetBaseException().Message);
            }
            
            Output = process.StandardOutput.ReadToEnd();
            ErrorOutput = process.StandardError.ReadToEnd();

            bool res = process.WaitForExit(10000);
            
            string html = null;
            if (File.Exists(tfileOut))
            {
                html = File.ReadAllText(tfileOut);
                html = StringUtils.ExtractString(html, "<body>\r\n", "\r\n</body>");
                //html = "<small style='color: steelblue;'>Pandoc Markdown Parser</small><hr/>" + html;
            }
            else
            {
                if (!string.IsNullOrEmpty(ErrorOutput))
                    throw new InvalidOperationException(ErrorOutput);
            }

            File.Delete(tfileIn);
            File.Delete(tfileOut);

            if (mmApp.Configuration.Markdown.RenderLinksAsExternal)
                html = ParseExternalLinks(html);

            if (!mmApp.Configuration.Markdown.AllowRenderScriptTags)
                html = HtmlUtils.SanitizeHtml(html);

            return html;
        }

    }
}
