using BTD_Backend;
using BTD_Backend.IO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Workbench.UserControls;
using BTD_Backend;

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
            TreeView_Handling.TreeItemExpanded += TreeView_Handling_TreeItemExpanded;
        }

        private void MallisTestingWindow_Loaded(object sender, RoutedEventArgs e)
        {
            openJet("BTD5.jet");
        }

        public void openJet(string path) => openJet(new Zip(path));

        public void openJet(Zip jet)
        {
            this.jet = jet;
            //BgThread.AddToQueue(() => jet.Password = jet.TryGetPassword());
            PopulateTreeView(SearchOption.TopDirectoryOnly);
        }

        private void TreeView_Handling_TreeItemExpanded(object sender, TreeView_Handling.TreeView_HandlingEventArgs e)
        {
            TreeViewItem source = (TreeViewItem)e.Source;
            string headerPath = TreeView_Handling.GetHeaderPath(source);
            if (IsFile(headerPath))
            {
                OpenFile(headerPath);
                return;
            }

            PopulateTreeView(source, SearchOption.TopDirectoryOnly, headerPath);
        }

        public bool IsFile(string path) => TreeView_Handling.IsFile(jet, path);

        public void PopulateTreeView(SearchOption searchOption) =>
            TreeView_Handling.PopulateTreeView(jet, JetView, searchOption);

        public void PopulateTreeView(TreeViewItem source, SearchOption searchOption, string treeItemPath) =>
            TreeView_Handling.PopulateTreeView(jet, source, searchOption, treeItemPath);


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
