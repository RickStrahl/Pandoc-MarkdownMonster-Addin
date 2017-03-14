using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using FontAwesome.WPF;
using MahApps.Metro.Controls;
using MarkdownMonster;

namespace PanDocMarkdownParserAddin
{
    /// <summary>
    /// Interaction logic for PasteHref.xaml
    /// </summary>
    public partial class PandocMarkdownParserWindow  : MetroWindow
    {
        public  PandocAddinModel  Model { get; set; }
        
        public PandocMarkdownParserWindow( PanDocMarkdownParserAddin addin)
        {
            InitializeComponent();
            mmApp.SetThemeWindowOverride(this);

            Model = new PandocAddinModel()
            {
                AddinWindow = this,
                Addin = addin                
            };
            


            Model.Configurations = new ObservableCollection<PandocConfigurationItem>();
            foreach (var item in Model.AddinConfiguration.Configurations.OrderBy(kv=> kv.Value.Name))
                Model.Configurations.Add(item.Value);

            if (Model.AddinConfiguration.Configurations.Count < 1)
            {
                Model.Configurations.Add(new PandocConfigurationItem()
                {
                     Name = "PDF Output",
                     CommandLineArguments = "-markdown -s \"{fileIn}\" -o \"{fileOut}\""
                });
            }

            if (Model.AddinConfiguration.Configurations.Count > 0)
                Model.ActiveConfiguration = Model.AddinConfiguration.Configurations.First().Value;
            
            Loaded += PandocMarkdownParserWindow_Loaded;
            Unloaded += CommanderWindow_Unloaded;

            

            DataContext = Model;            
        }      

        private MarkdownEditorSimple editor;

        private void PandocMarkdownParserWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //string initialValue = null;
            //if (Model.AddinConfiguration.Commands.Count > 0)
            //{
            //    ListCommands.SelectedItem = Model.AddinConfiguration.Commands[0];
            //    initialValue = Model.AddinConfiguration.Commands[0].CommandText;
            //}

            //editor = new MarkdownEditorSimple(WebBrowserCommand, initialValue);            
            //editor.IsDirtyAction =  () =>
            //{ 
            //    string val = editor.GetMarkdown();
            //    if (val != null && Model.ActiveCommand != null)
            //        Model.ActiveCommand.CommandText = val;

            //    return true;
            //};

            //Dispatcher.InvokeAsync(() =>
            //{
            //    ListCommands.Focus();
            //    editor.SetEditorSyntax("csharp");                
            //},System.Windows.Threading.DispatcherPriority.ApplicationIdle);
        }


        private void CommanderWindow_Unloaded(object sender, RoutedEventArgs e)
        {
            Model.AddinConfiguration.Write();
        }


        private void ListCommands_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var command = ListCommands.SelectedItem as PandocConfigurationItem;
            if (command == null)
                return;

            //Model.Addin.RunCommand(command);            
        }

        private void ToolButtonNewCommand_Click(object sender, RoutedEventArgs e)
        {
            //Model.AddinConfiguration.Commands.Insert(0,new CommanderCommand() {Name = "New Command"});
            //ListCommands.SelectedItem = Model.AddinConfiguration.Commands[0];
        }


        private void ToolButtonRemoveCommand_Click(object sender, RoutedEventArgs e)
        {
            //var command = ListCommands.SelectedItem as CommanderCommand;
            //if (command == null)
            //    return;
            //CommanderAddinConfiguration.Current.Commands.Remove(command);
        }


        private void ToolButtonRunCommand_Click(object sender, RoutedEventArgs e)
        {
            //var command = ListCommands.SelectedItem as CommanderCommand;
            //if (command == null)
            //    return;

            //Model.Addin.RunCommand(command);
        }

        
        private void ListCommands_KeyUp(object sender, KeyEventArgs e)
        {
            
            //if (e.Key == Key.Return || e.Key == Key.Space)
            //{
                
            //    var command = ListCommands.SelectedItem as CommanderCommand;
            //    if (command != null)                    
            //        Model.Addin.RunCommand(command);
            //}
        }

        private void ListCommands_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var command = ListCommands.SelectedItem as PandocConfigurationItem;


            //if (command != null)
            //{
            //    try { 
            //        editor?.SetMarkdown(command.CommandText);
            //    }catch { }}
        }


        #region StatusBar

        public void ShowStatus(string message = null, int milliSeconds = 0)
        {
            if (message == null)
                message = "Ready";

            StatusText.Text = message;

            if (milliSeconds > 0)
            {
                var t = new Timer(new TimerCallback((object win) =>
                {
                    var window = win as PandocMarkdownParserWindow;
                    if (window == null)
                        return;

                    window.Dispatcher.Invoke(() => { window.ShowStatus(null, 0); });
                }), this, milliSeconds, Timeout.Infinite);
            }
        }

        /// <summary>
        /// Status the statusbar icon on the left bottom to some indicator
        /// </summary>
        /// <param name="icon"></param>
        /// <param name="color"></param>
        /// <param name="spin"></param>
        public void SetStatusIcon(FontAwesomeIcon icon, Color color, bool spin = false)
        {
            StatusIcon.Icon = icon;
            StatusIcon.Foreground = new SolidColorBrush(color);
            if (spin)
                StatusIcon.SpinDuration = 30;
            StatusIcon.Spin = spin;
        }

        /// <summary>
        /// Resets the Status bar icon on the left to its default green circle
        /// </summary>
        public void SetStatusIcon()
        {
            StatusIcon.Icon = FontAwesomeIcon.Circle;
            StatusIcon.Foreground = new SolidColorBrush(Colors.Green);
            StatusIcon.Spin = false;
            StatusIcon.SpinDuration = 0;
            StatusIcon.StopSpin();
        }
        #endregion
    }
}
