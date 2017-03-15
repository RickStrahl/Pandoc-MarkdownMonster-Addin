using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using PanDocMarkdownParserAddin.Annotations;

namespace PanDocMarkdownParserAddin
{
    /// <summary>
    /// Contains a Pandoc Configuration Item that can be fired to run Pandoc
    /// interactively to produce output
    /// </summary>
    public class PandocConfigurationItem : INotifyPropertyChanged
    {

        /// <summary>
        ///  Name of this configuration
        /// </summary>
        public string Name
        {
            get { return _Name; }
            set
            {
                if (value == _Name) return;
                _Name = value;
                OnPropertyChanged(nameof(Name));
            }
        }
        private string _Name = "New Pandoc Configuration";


        /// <summary>
        /// The command line arguments for 
        /// </summary>
        public string CommandLineArguments
        {
            get { return _commandLineArguments; }
            set
            {
                if (value == _commandLineArguments) return;
                _commandLineArguments = value;
                OnPropertyChanged(nameof(CommandLineArguments));
            }
        }
        private string _commandLineArguments = "-f markdown -s \"{fileIn}\" -o \"{fileOut}\"";


        /// <summary>
        /// The extension for the output generated (ie. .pdf, .epub, .html etc.)
        /// </summary>
        public string OutputExtension
        {
            get { return _OutputExtension; }
            set
            {
                if (value == _OutputExtension) return;
                _OutputExtension = value;
                OnPropertyChanged(nameof(OutputExtension));
            }
        }
        private string _OutputExtension = ".html";

        
        
        /// <summary>
        /// If true requires that the output file is 
        /// requested before executing the configuration.
        /// </summary>
        public bool IsFileOutput
        {
            get { return _isFileOutput; }
            set
            {
                if (value == _isFileOutput) return;
                _isFileOutput = value;
                OnPropertyChanged();
            }
        }
        private bool _isFileOutput;

        public bool Execute(string markdown, string outputFile)
        {
            File.Delete(outputFile);

            if (!OutputExtension.StartsWith("."))
                OutputExtension = "." + OutputExtension;
            OutputExtension = OutputExtension.ToLower();

            var tfileIn = Path.ChangeExtension(Path.GetTempFileName(), ".md");
            
            File.WriteAllText(tfileIn, markdown);

            var Configuration = PandocAddinConfiguration.Current;
            var cmdLine =Configuration.PandocCommandLine.Replace("{fileIn}", tfileIn).Replace("{fileOut}", outputFile);

            var pi = new ProcessStartInfo("Pandoc.exe")
            {
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                Arguments = cmdLine,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            
            var process = Process.Start(pi);
            
            string error = process.StandardError.ReadToEnd();

            if (!string.IsNullOrEmpty(error))
                throw new InvalidOperationException(error);

            bool res = process.WaitForExit(10000);
            
            File.Delete(tfileIn);

            return File.Exists(outputFile);
        }


        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}