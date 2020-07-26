using BTD_Backend;
using BTD_Backend.IO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
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
            BgThread.AddToQueue(() => jet.Password = jet.TryGetPassword());
            var entries = jet.Archive.Entries;
            foreach (var entry in entries)
            {
                string item = entry.FileName;

                item = item.TrimEnd('/');

                string[] split = item.Split('/');
                if (split.Length > 2)
                    continue;

                TreeViewItem treeItem = new TreeViewItem();
                treeItem.Header = item;
                treeItem.Expanded += TreeViewItem_Expanded;
                JetView.Items.Add(treeItem);
            }
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

            if (IsFile(e.Source.ToString()))
            {
                OpenFile(e.Source.ToString());
                return;
            }
            
            string sourceName = source.Header.ToString();
            string[] sourceSplit = sourceName.Split('/');
            //MessageBox.Show(sourceName);

            var entries = jet.Archive.Entries;
            foreach (var entry in entries)
            {
                string item = entry.FileName;
                item = item.TrimEnd('/');

                //Does this contain this source? If not skip
                if (!entry.FileName.Contains(sourceName))
                    continue;

                //Is this item the same as the source? If so skip
                if (item == source.Header.ToString())
                    continue;

                //Is this item "too deep" in the archive? If so skip
                string[] itemSplit = item.Split('/');
                if (itemSplit.Length > sourceSplit.Length + 1)
                    continue;

                //Does the source already have this item? If so skip
                if (source.Items.Cast<TreeViewItem>().Any(t => t.Header.ToString() == item))
                    continue;

                //MessageBox.Show(item);
                TreeViewItem treeItem = new TreeViewItem();                
                treeItem.Header = item;
                //treeItem.Header = itemSplit[itemSplit.Length - 1];
                //treeItem.Expanded += TreeViewItem_Expanded;

                source.Items.Add(treeItem);
            }

            /*TreeViewItem source = (TreeViewItem)e.Source;
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
            }*/
            /*TreeViewItem newItem = new TreeViewItem();
            newItem.Header = "Gameing";
            Label l = new Label();
            l.Content = "Gameing";
            newItem.Items.Add(l);
            source.Items.Add(newItem);*/
        }

        private bool IsFile(string path)
        {
            string[] split = path.Split(':');
            string filepath = split[split.Length - 2].Replace(" Items.Count", "");
            
            return jet.Archive.EntryFileNames.Contains(filepath);
        }

        private void OpenFile(string path)
        {
            string[] pathSplit = path.Split(':');
            string filepath = pathSplit[pathSplit.Length - 2].Replace(" Items.Count", "");

            foreach (var item in LinedTextBox_UC.OpenedFiles)
            {
                Log.Output("item.Filep = " + item.FilePath);
                Log.Output("ifilepath = " + filepath);
                if (item.FilePath == filepath)
                    return;
            }

            if (String.IsNullOrEmpty(jet.Password))
            {
                Log.Output("pass is null. please wait");
                return;
            }

            string[] nameSplit = filepath.Split('/');
            LinedTextBox_UC textbox = new LinedTextBox_UC()
            {
                Text = jet.ReadFileInZip(filepath),
                FilePath = filepath,
                TabName = nameSplit[nameSplit.Length - 1]
            };

            TabItem tab = new TabItem();
            tab.Header = textbox.TabName;
            tab.Content = textbox;

            TabTextEditor_UC.TabController.Items.Add(tab);
            TabTextEditor_UC.TabController.SelectedIndex = TabTextEditor_UC.TabController.Items.Count - 1;

        }

        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scv = (ScrollViewer)sender;
            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
            e.Handled = true;
        }

    }
}
