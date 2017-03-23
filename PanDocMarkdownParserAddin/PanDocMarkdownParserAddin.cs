using System;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Media;
using FontAwesome.WPF;
using MarkdownMonster;
using MarkdownMonster.AddIns;
using MarkdownMonster.Windows;

namespace PanDocMarkdownParserAddin
{
    public class PanDocMarkdownParserAddin : MarkdownMonster.AddIns.MarkdownMonsterAddin
    {
        public override void OnApplicationStart()
        {
            base.OnApplicationStart();

            Id = "PanDocMarkdownParser";
            Name = "Pandoc Markdown Parser";

            // by passing in the add in you automatically
            // hook up OnExecute/OnExecuteConfiguration/OnCanExecute
            var menuItem = new AddInMenuItem(this)
            {
                Caption = "PanDoc MarkdownParserAddin",

                // if an icon is specified it shows on the toolbar
                // if not the add-in only shows in the add-ins menu
                FontawesomeIcon = FontAwesomeIcon.Exchange,
            };
            menuItem.IconImageSource = new ImageSourceConverter()
                .ConvertFromString("pack://application:,,,/PanDocMarkdownParserAddin;component/icon_22.png") as ImageSource;

            // if you don't want to display config or main menu item clear handler
            //menuItem.ExecuteConfiguration = null;

            // Must add the menu to the collection to display menu and toolbar items            
            this.MenuItems.Add(menuItem);
        }
        
        public override IMarkdownParser GetMarkdownParser()
        {
            return new PandocMarkdownParser();
        }

        private PandocMarkdownParserWindow form;

        public override void OnExecute(object sender)
        {
            form = new PandocMarkdownParserWindow(this);
            form.Owner = Model.Window;
            form.Show();
        }

        public override void OnExecuteConfiguration(object sender)
        {
            OpenTab(Path.Combine(mmApp.Configuration.CommonFolder, "PandocAddin.json"));
        }

        public override bool OnCanExecute(object sender)
        {
            return true;
        }

    }
}
