using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MarkdownMonster;
using MarkdownMonster.AddIns;
using Westwind.Utilities;
using Westwind.Utilities.Configuration;

namespace PanDocMarkdownParserAddin
{
    public class PandocAddinConfiguration : BaseAddinConfiguration<PandocAddinConfiguration>
    {
        protected override string ConfigurationFilePath { get; set; } = "PandocAddin.json";

        //static PandocAddinConfiguration()
        //{
        //    Current = new PandocAddinConfiguration();
        //    Current.Initialize();        
        //}


        /// <summary>
        /// The path to the Pandoc executable or just Pandoc if it
        /// is on the system path (as it is with a full install).
        /// </summary>
        public string PandocPath
        {
            get { return _PandocPath; }
            set
            {
                if (value == _PandocPath) return;
                _PandocPath = value;
                OnPropertyChanged(nameof(PandocPath));
            }
        }
        private string _PandocPath = @"pandoc.exe";


        /// <summary>
        /// Pandoc Command line to use to run markdown conversion
        /// </summary>
        public string PandocCommandLine
        {
            get { return _PandocCommandLine; }
            set
            {
                if (value == _PandocCommandLine) return;
                _PandocCommandLine = value;
                OnPropertyChanged(nameof(PandocCommandLine));
            }
        }
        private string _PandocCommandLine = "-f markdown -s \"{fileIn}\" -o \"{fileOut}\"";


        public Dictionary<string,PandocConfigurationItem> Configurations
        {
            get { return _Configurations; }
            set
            {
                if (value == _Configurations) return;
                _Configurations = value;
                OnPropertyChanged(nameof(Configurations));
            }
        }
        private Dictionary<string,PandocConfigurationItem> _Configurations = new Dictionary<string, PandocConfigurationItem>();

        
    }
}
