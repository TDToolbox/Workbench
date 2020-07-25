using BTD_Backend;
using BTD_Backend.IO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using Workbench.UserControls;

namespace Workbench
{
    /// <summary>
    /// Interaction logic for MallisTesting.xaml
    /// </summary>
    public partial class MallisTesting : Window
    {
        private Zip jet;

        public string Wbp_Path { get; set; }
        public MallisTesting()
        {
            InitializeComponent();
        }

        public void openJet(string path)
        {
            openJet(new Zip(path));
        }
        public void openJet(Zip jet)
        {
            this.jet = jet;
            List<string> entries = jet.GetEntries(Zip.EntryType.Directories, default, System.IO.SearchOption.TopDirectoryOnly);
            entries.ForEach((entry) =>
            {
                TreeViewItem item = new TreeViewItem();
                item.Header = entry;
                item.Expanded += TreeViewItem_Expanded;
                //item.Items.Add(new TreeViewItem());
                JetView.Items.Add(item);
            });
            /*List<string> entries = jet.GetEntries(Zip.EntryType.Directories, default, System.IO.SearchOption.TopDirectoryOnly);
            entries.ForEach((entry) =>
            {
                JetView.Items.Add(recurseFile(entry, jet));
            });*/
        }

        private void MallisTestingWindow_Loaded(object sender, RoutedEventArgs e)
        {
            /*Zip jet = new Zip(Environment.CurrentDirectory + "\\BTD5.jet");
            jet.Password = jet.TryGetPassword();
            //MessageBox.Show("Password: " + jet.Password);

            var entries = jet.GetEntries(Zip.EntryType.Files, "TowerDefinitions");
            //MessageBox.Show("Got Entries");

            var text = jet.ReadFileInZip(entries[0]);
            //MessageBox.Show(text);
            LinedTextBox_UC linedTextBox = new LinedTextBox_UC();
            linedTextBox.TextEditor.Text = text;
            //ContentPanel.Children.Add(linedTextBox);*/
            openJet("BTD5.jet");
        }

        private void TreeViewItem_Expanded(object sender, RoutedEventArgs e)
        {
            TreeViewItem source = (TreeViewItem)e.Source;
            TreeViewItem current = source;
            string path = "";
            while(current.Parent is TreeViewItem || current.Parent is TreeView)
            {
                if(current.Parent is TreeViewItem)
                {
                    path = current.Header + path;
                    current = (TreeViewItem)current.Parent;
                }
                else
                {
                    path = current.Header + path;
                    break;
                }
            }
            //path = path.Substring(0, path.Length - 1);
            Log.Output(path);
            if(current.Items.Count < 2)
            {
                List<string> entries = jet.GetEntries(Zip.EntryType.Directories, path, System.IO.SearchOption.TopDirectoryOnly);
                entries.ForEach((entry) =>
                {
                    TreeViewItem item = new TreeViewItem();
                    item.Header = entry.Replace(path, "");
                    current.Items.Add(item);
                });
            }
            /*TreeViewItem newItem = new TreeViewItem();
            newItem.Header = "Gameing";
            Label l = new Label();
            l.Content = "Gameing";
            newItem.Items.Add(l);
            source.Items.Add(newItem);*/
        }

        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scv = (ScrollViewer)sender;
            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
            e.Handled = true;
        }

    }
}
