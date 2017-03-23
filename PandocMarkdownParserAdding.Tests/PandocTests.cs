using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PanDocMarkdownParserAddin;
using Westwind.Utilities;

namespace PandocMarkdownParserAdding.Tests
{
    [TestClass]
    public class PandocTests
    {
        [TestMethod]
        public void PandocMarkdownParserBasicTest()
        {
            var parser = new PandocMarkdownParser();
            var html = parser.Parse("Hello **Cruel** ~~world~~!");

            Assert.IsNotNull(html);
            Console.WriteLine(html);
        }


        [TestMethod]
        public void PandocConfigurationItem()
        {
            var item = new PandocConfigurationItem();
            string outputFile = "c:\\temp\\test.pdf";

            item.CommandLineArguments = "-markdown -s \"{fileIn}\" -o \"{fileOut}\"";
            var res = item.Execute("Hello **Cruel** ~~world~~!\n\n* item 1\n* item 2",outputFile,null,"c:\\temp");

            Assert.IsTrue(res.Item1);

            ShellUtils.GoUrl(outputFile);
        }
    }
}
