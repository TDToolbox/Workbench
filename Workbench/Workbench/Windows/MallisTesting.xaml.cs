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

        private void MallisTestingWindow_Loaded(object sender, RoutedEventArgs e)
        {
            openJet("BTD5.jet");
        }

        public void openJet(string path) => openJet(new Zip(path));

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

                //For some reason IonicZip's .Entries skips any folders in the root directory. Need to 
                //remove extra part of path so it only shows folders in root directory
                string itemName = split[split.Length - 2];

                TreeViewItem treeItem = new TreeViewItem();
                treeItem.Header = itemName;
                treeItem.Expanded += TreeViewItem_Expanded;
                JetView.Items.Add(treeItem);
            }
        }

        private void TreeViewItem_Expanded(object sender, RoutedEventArgs e)
        {
            TreeViewItem source = (TreeViewItem)e.Source;
            TreeViewItem current = source;

            string currentFolder = GetCurrentFolder(source.Header.ToString());
            
            if (IsFile(currentFolder))
            {
                OpenFile(currentFolder);
                return;
            }

            string sourceName = source.Header.ToString();
            string[] currentFolderSplit = currentFolder.Split('/');
            
            var entries = jet.Archive.Entries;
            foreach (var entry in entries)
            {
                string item = entry.FileName;
                item = item.TrimEnd('/');

                //Does this contain this source? If not skip
                if (!item.Contains(currentFolder))
                    continue;

                //Is this item the same as the source? If so skip
                string itemName = item.Remove(0, item.LastIndexOf('/') + 1);
                if (itemName == sourceName)
                    continue;

                //Is this item "too deep" in the archive? If so skip
                string[] itemSplit = item.Split('/');
                if (itemSplit.Length > currentFolderSplit.Length+1)
                    continue;

                //Does the source already have this item? If so skip
                if (source.Items.Cast<TreeViewItem>().Any(t => t.Header.ToString() == item))
                    continue;

                TreeViewItem treeItem = new TreeViewItem();
                treeItem.Header = itemName;
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

        private string GetCurrentFolder(string header)
        {
            string currentFolder = "";
            var entries = jet.Archive.Entries;
            foreach (var entry in entries)
            {
                string item = entry.FileName;
                item = item.TrimEnd('/');

                //Does this contain this source? If not skip
                if (!entry.FileName.Contains(header))
                    continue;
                
                //Assume the first entry to contain header is our current folder
                currentFolder = item;
                break;
            }

            //For some reason IonicZip's .Entries skips any folders in the root directory. Need to 
            //remove extra part of path so it only shows folders in root directory
            if (currentFolder.Split('/').Length - 1 == 1)
            {
                string[] split = currentFolder.Split('/');
                string previousFolder = split[split.Length-2];
                
                if (header == previousFolder)
                    return header;
            }

            return currentFolder;
        }

        private bool IsFile(string path) => jet.Archive.EntryFileNames.Contains(path);

        private void OpenFile(string path)
        {
            string filepath = path;

            foreach (var item in LinedTextBox_UC.OpenedFiles)
            {
                if (item.FilePath == filepath)
                {
                    TabTextEditor_UC.TabController.SelectedItem = item.Tab_Owner;
                    return;
                }
            }

            string[] nameSplit = filepath.Split('/');
            TabItem tab = new TabItem();
            tab.Header = nameSplit[nameSplit.Length - 1];

            LinedTextBox_UC textbox = new LinedTextBox_UC()
            {
                Text = jet.ReadFileInZip(filepath),
                FilePath = filepath,
                TabName = tab.Header.ToString(),
                Tab_Owner = tab
            };

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
