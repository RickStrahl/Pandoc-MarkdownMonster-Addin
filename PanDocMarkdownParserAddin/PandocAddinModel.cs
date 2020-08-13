using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PanDocMarkdownParserAddin
{
    public class PandocAddinModel : INotifyPropertyChanged
    {

        public PandocAddinConfiguration AddinConfiguration 
        {
            get { return _addinConfiguration ; }
            set
            {
                if (value == _addinConfiguration ) return;
                _addinConfiguration  = value;
                OnPropertyChanged(nameof(AddinConfiguration ));
            }
        }
        private PandocAddinConfiguration _addinConfiguration = PandocAddinConfiguration.Current;


        public PandocMarkdownParserWindow AddinWindow
        {
            get { return _AddinWindow; }
            set
            {
                if (value == _AddinWindow) return;
                _AddinWindow = value;
                OnPropertyChanged(nameof(AddinWindow));
            }
        }
        private PandocMarkdownParserWindow  _AddinWindow = null;


        public PanDocMarkdownParserAddin Addin
        {
            get { return _Addin; }
            set
            {
                if (value == _Addin) return;
                _Addin = value;
                OnPropertyChanged(nameof(Addin));
            }
        }
        private PanDocMarkdownParserAddin _Addin = null;




        public ObservableCollection<PandocConfigurationItem> Configurations { get; set; } =
            new ObservableCollection<PandocConfigurationItem>();


        public PandocConfigurationItem ActiveConfiguration
        {
            get { return _activeConfiguration; }
            set
            {
                if (Equals(value, _activeConfiguration)) return;
                _activeConfiguration = value;
                OnPropertyChanged(nameof(ActiveConfiguration));
            }
        }
        private PandocConfigurationItem _activeConfiguration;



        public event PropertyChangedEventHandler PropertyChanged;

        
        protected virtual void OnPropertyChanged( string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}