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


        public string Description
        {
            get { return _Description; }
            set
            {
                if (value == _Description) return;
                _Description = value;
                OnPropertyChanged(nameof(Description));
            }
        }
        private string _Description;



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
        public bool PromptForOutputFilename
        {
            get { return _promptForOutputFilename; }
            set
            {
                if (value == _promptForOutputFilename) return;
                _promptForOutputFilename = value;
                OnPropertyChanged();
            }
        }
        private bool _promptForOutputFilename;


        /// <summary>
        /// If true requires that the input file is 
        /// requested before executing the configuration.
        /// </summary>
        public bool PromptForInputFilename
        {
            get { return _promptForInputFilename; }
            set
            {
                if (value == _promptForInputFilename) return;
                _promptForInputFilename = value;
                OnPropertyChanged();
            }
        }
        private bool _promptForInputFilename;



        public Tuple<bool,string> Execute(string markdown, string outputFile, string inputFile, string basePath)
        {
            File.Delete(outputFile);

            if (!OutputExtension.StartsWith("."))
                OutputExtension = "." + OutputExtension;
            OutputExtension = OutputExtension.ToLower();

            

            var Configuration = PandocAddinConfiguration.Current;
            var cmdLine = CommandLineArguments.Replace("{fileIn}", inputFile).Replace("{fileOut}", outputFile);

            var pi = new ProcessStartInfo("Pandoc.exe")
            {
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                Arguments = cmdLine,
                UseShellExecute = false,
                CreateNoWindow = true,
                WorkingDirectory = basePath
            };

            
            var process = Process.Start(pi);

            string console = process.StandardOutput.ReadToEnd()
                             + "Error: " + process.StandardError.ReadToEnd();

            bool res = process.WaitForExit(10000);

            if (process.ExitCode != 0)
                throw new InvalidOperationException(console);

            console = process.StandardOutput.ReadToEnd();

            
            
            File.Delete(inputFile);

            return new Tuple<bool, string>( File.Exists(outputFile), console);
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