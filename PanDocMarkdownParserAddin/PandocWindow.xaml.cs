using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using FontAwesome6;
using FontAwesome6.Shared.Extensions;
using MahApps.Metro.Controls;
using MarkdownMonster;
using MarkdownMonster.Windows;
using Microsoft.Win32;
using Westwind.Utilities;

namespace PanDocMarkdownParserAddin
{
    /// <summary>
    /// Interaction logic for PasteHref.xaml
    /// </summary>
    public partial class PandocMarkdownParserWindow  : MetroWindow
    {
        public  PandocAddinModel  Model { get; set; }

        public StatusBarHelper Status { get; }

        public PandocMarkdownParserWindow(PanDocMarkdownParserAddin addin)
        {
            InitializeComponent();
            mmApp.SetThemeWindowOverride(this);

            Model = new PandocAddinModel()
            {
                AddinWindow = this,
                Addin = addin
            };

            Model.Configurations = new ObservableCollection<PandocConfigurationItem>();
            foreach (var item in Model.AddinConfiguration.Configurations.OrderBy(kv => kv.Value.Name))
                Model.Configurations.Add(item.Value);

            if (Model.AddinConfiguration.Configurations.Count < 1)            
                InitializeConfigurations();

            if (Model.AddinConfiguration.Configurations.Count > 0)
                Model.ActiveConfiguration = Model.AddinConfiguration.Configurations.First().Value;

            Loaded += PandocMarkdownParserWindow_Loaded;
            Unloaded += CommanderWindow_Unloaded;

            Status = new StatusBarHelper(StatusText, StatusIcon);

            DataContext = Model;
        }

        private void InitializeConfigurations()
        {
            Model.ActiveConfiguration = new PandocConfigurationItem()
            {
                Name = "Markdown Document Conversion",
                CommandLineArguments = "-f markdown -s \"{fileIn}\" -o \"{fileOut}\"",
                PromptForOutputFilename = true,
                Description =
                    "Uses Pandoc to converts the current Markdown document from Markdown to the specified output format. This dialog prompts for a filename, the extension of which determines what format the file is created with. Common formats include: .pdf, .docx, .epub, .html, .odt etc."
            };
            Model.Configurations.Add(Model.ActiveConfiguration);
            var config = new PandocConfigurationItem()
            {
                Name = "Generic Document Conversion",
                CommandLineArguments = "-s \"{fileIn}\" -o \"{fileOut}\"",
                PromptForOutputFilename = true,
                PromptForInputFilename = true,
                Description =
                    "Converts any document type that Pandoc supports for input, to any document type that it supports for output based on extension used.  You are prompted for an input file and an output file."
            };
            Model.Configurations.Add(config);
            config = new PandocConfigurationItem()
            {
                Name = "HTML Document Conversion",
                CommandLineArguments = "f html -s \"{fileIn}\" -o \"{fileOut}\"",
                PromptForOutputFilename = true,
                Description =
                    "Converts the current document to HTML using the active Markdown Parser (not necessarily Pandoc) and then uses Pandoc to convert the resulting HTML to the specified output format. This dialog prompts for a filename, the extension of which determines what format the file is created with. Common formats include: .pdf, .docx, .epub, .html, .odt etc."
            };
            Model.Configurations.Add(config);
        }

        private void PandocMarkdownParserWindow_Loaded(object sender, RoutedEventArgs e)
        {                                     
        }


        private void CommanderWindow_Unloaded(object sender, RoutedEventArgs e)
        {
            Model.AddinConfiguration.Configurations.Clear();
            foreach (var item in Model.Configurations)
                Model.AddinConfiguration.Configurations.Add(item.Name, item);

            Model.AddinConfiguration.Write();

            // make sure MM gets activated not the opened preview app
            Model.Addin.Model.Window.Activate();
        }


        private void ListCommands_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var config = ListConfigurations.SelectedItem as PandocConfigurationItem;
            if (config == null)
                return;

            ToolButtonRunConfiguration_Click(sender, null);
        }

        private void ToolButtonNewCommand_Click(object sender, RoutedEventArgs e)
        {
            var item = new PandocConfigurationItem {Name = "New Command"};
            Model.Configurations.Add(item);
            Model.ActiveConfiguration = item;            
            //ListConfigurations.SelectedItems.Add(Model.ActiveConfiguration);
        }


        private void ToolButtonRemoveCommand_Click(object sender, RoutedEventArgs e)
        {
            var config = ListConfigurations.SelectedItem as PandocConfigurationItem;
            if (config == null)
                return;
            Model.Configurations.Remove(config);
        }


        private async void ToolButtonRunConfiguration_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true; 
            await RunConfiguration();            
        }

        private void ToolButtonPandocFormats_Click(object sender, RoutedEventArgs e)
        {
            ShellUtils.GoUrl("https://pandoc.org/");
        }

        private async Task RunConfiguration()
        {

            var item = ListConfigurations.SelectedItem as PandocConfigurationItem;
            if (item == null)
                return;
            var editor = Model.Addin.Model.ActiveEditor;
            if (editor == null)
                return;

            bool generateHtml = item.CommandLineArguments.Contains("-f html");
            var markdown = await Model.Addin.Model.ActiveEditor.GetMarkdown();
            var docFile = Model.Addin.Model.ActiveDocument.Filename;
            var path = Path.GetDirectoryName(docFile);
            TextConsole.Text = "";



            string inputFile;
            if (item.PromptForInputFilename)
            {
                var of = new OpenFileDialog
                {
                    Filter = "Markdown Files (*.md, .markdown)|*.md;*.markdown|Html Files(*.htm,html)|*.html;*.htm|Word Docx Files (*.docx)|*.docx|epub files (*.epub)|*.epub|Open Office ODT Files (*.odt)|*.odt|Open Document XML (*.xml)|*.xml|All Files (*.*)|*.*",
                    FilterIndex = 1,
                    Title = "Open input file for Pandoc Conversion",
                    FileName = null,
                    InitialDirectory = path,
                    CheckFileExists = true,
                    CheckPathExists = true,
                    RestoreDirectory = true
                };

                var result = of.ShowDialog();
                if (result == null || !result.Value)
                    return;
                inputFile = of.FileName;
                path = Path.GetDirectoryName(inputFile);
            }
            else
            {
                if (generateHtml)
                {
                    string html = Model.Addin.Model.ActiveDocument.RenderHtml();
                    inputFile = Model.Addin.Model.ActiveDocument.HtmlRenderFilename;
                    File.WriteAllText(inputFile, html, new UTF8Encoding(false) /* pandoc doesn't like the BOM */);
                }
                else
                {
                    inputFile = Path.ChangeExtension(docFile, ".md");
                    File.WriteAllText(inputFile, markdown, new UTF8Encoding(false) /* pandoc doesn't like the BOM */);
                }
            }



            if (item.PromptForOutputFilename)
            {
                string filename = Path.GetFileName(Path.ChangeExtension(inputFile, item.OutputExtension ?? ".pdf"));
                var sd = new SaveFileDialog
                {
                    Filter = "PDF Files (*.pdf)|*.pdf|Word Docx Files (*.docx)|*.docx|Html Files(*.htm,html)|*.html;*.htm|epub files (*.epub)|*.epub|Open Office ODT Files (*.odt)|*.odt|Open Document XML (*.xml)|*.xml|All Files (*.*)|*.*",
                    FilterIndex = 1,
                    FileName = filename,
                    Title = "Specify output file Pandoc Conversion",
                    InitialDirectory = path,
                    CheckFileExists = false,
                    OverwritePrompt = true,
                    CheckPathExists = true,
                    RestoreDirectory = true
                };
                var result = sd.ShowDialog();
                if (result == null || !result.Value)
                    return;

                try
                {
                    Status.ShowStatusProgress("Document creation in progress...");
                    TextConsole.Text = null;

                    (bool success, string consoleText) = item.Execute(markdown, sd.FileName, inputFile, path, generateHtml);
                    TextConsole.Text = consoleText;

                    if (success)
                        ShellUtils.GoUrl(sd.FileName);

                    Status.ShowStatusSuccess("Output was generated.");
                }
                catch (Exception ex)
                {                    
                    TextConsole.Text = ex.Message;
                    Status.ShowStatusError("Error executing Pandoc configuration.");                    
                }
            }

        }


        private void ListCommands_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return || e.Key == Key.Space)
                RunConfiguration().FireAndForget();
        }

        private void ListCommands_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //RunConfiguration();
        }



    }
}
