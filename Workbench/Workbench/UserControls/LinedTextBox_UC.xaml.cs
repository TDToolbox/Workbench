using BTD_Backend;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;

namespace Workbench.UserControls
{
    /// <summary>
    /// Interaction logic for LinedTextBox_UC.xaml
    /// </summary>
    public partial class LinedTextBox_UC : UserControl
    {
        public static List<LinedTextBox_UC> OpenedFiles;
        public string Text { get; set; }
        public string FilePath { get; set; }
        public string TabName { get; set; }
        public TabItem Tab_Owner { get; set; }

        public LinedTextBox_UC()
        {
            InitializeComponent();

            if (OpenedFiles == null)
                OpenedFiles = new List<LinedTextBox_UC>();
            OpenedFiles.Add(this);

            string json = Properties.Resources.BJson;
            using(Stream s = GenerateStreamFromString(json))
            {
                using (XmlTextReader reader = new XmlTextReader(s))
                {
                    TextEditor.SyntaxHighlighting = HighlightingLoader.Load(reader, HighlightingManager.Instance);
                }
            }

        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            TextEditor.Text = Text;
        }

        public static Stream GenerateStreamFromString(string s)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        private void TextEditor_TextChanged(object sender, EventArgs e)
        {
            
        }
    }
}
