using BTD_Backend;
using BTD_Backend.IO;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Workbench.UserControls;
using BTD_Backend.Persistence;
using BTD_Backend.Game;
using System.Windows.Forms;
using BTD_Backend.Game.Jet_Files;
using System.Windows.Documents;
using System.Collections.Generic;

namespace Workbench
{
    /// <summary>
    /// Interaction logic for JetEditor.xaml
    /// </summary>
    public partial class JetEditor : Window
    {
        private Zip jet;

        public static List<JetEditor> OpenedJetEditors = new List<JetEditor>();

        public JetEditor()
        {
            InitializeComponent();
            TreeView_Handling.TreeItemExpanded += TreeView_Handling_TreeItemExpanded;

            OpenedJetEditors.Add(this);
        }

        public JetEditor(string projectPath) : this()
        {
            Wbp project = new Wbp(projectPath);
            GameInfo gameInfo = GameInfo.GetGame(ProjectData.Instance.TargetGame);

            string jetPath = gameInfo.GameDir;

            var game = ProjectData.Instance.TargetGame;
            if (game == GameType.BMC || game == GameType.BTD5 || game == GameType.BTDB)
            {
                jetPath += "\\Assets\\" + gameInfo.JetName;
            }
            else if (game == GameType.BTD6)
            {
                if (File.Exists(jetPath + "\\" + gameInfo.JetName))
                    jetPath += "\\" + gameInfo.JetName;
            }

            if (!File.Exists(jetPath))
            {
                Log.Output("This Jetfile doesn't exist! : " + jetPath);
                throw new Exception("Jetfile: \"" + jetPath + "\" doesnt exist so the window cannot open");
            }

            jet = new Zip(jetPath);
            if (!String.IsNullOrEmpty(ProjectData.Instance.JetPassword))
                jet.Password = ProjectData.Instance.JetPassword;

            Show();
        }

        private void JetEditor_Loaded(object sender, RoutedEventArgs e)
        {
            openJet(jet);

            PopulateModFiles();
        }

        private void PopulateModFiles()
        {
            Zip wbpZip = new Zip(ProjectData.Instance.WBP_Path);

            /*if (!wbpZip.Archive.ContainsEntry("Jet_Mod"))
            {
                Log.Output("Jet_Mod folder does not exist", OutputType.Both);
                return;
            }*/

            var files = wbpZip.GetEntries(Zip.EntryType.All, SearchOption.AllDirectories, "/Jet_Mod");
            var modFiles = new List<string>();
            foreach (var file in files)
            {
                if (!file.StartsWith("Jet_Mod"))
                    continue;

                if (modFiles.Contains(file))
                    continue;

                modFiles.Add(file);
                /*if (file.Replace("/", "") == "Jet_Mod")
                    continue;
                Log.Output(file);*/
            }

            //PopulateTreeView(SearchOption.TopDirectoryOnly, wbpZip, ModFiles_TreeView);
        }

        

        public void openJet(string path) => openJet(new Zip(path));

        public void openJet(Zip jet)
        {
            if(this.jet == null)
                this.jet = jet;

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

        public bool IsFile(string path) => TreeView_Handling.IsFile(this.jet, path);

        /*public void PopulateTreeView(SearchOption searchOption)
        {
            if(this.jet != null)
                TreeView_Handling.PopulateTreeView(this.jet, JetView, searchOption, true);
        }*/

        public void PopulateTreeView(SearchOption searchOption) => PopulateTreeView(searchOption, this.jet, JetView);

        public void PopulateTreeView(SearchOption searchOption, Zip zip, System.Windows.Controls.TreeView tree)
        {
            if (this.jet != null)
                TreeView_Handling.PopulateTreeView(zip, tree, searchOption, true);
        }

        public void PopulateTreeView(TreeViewItem source, SearchOption searchOption, string treeItemPath)
        {
            if (this.jet != null)
                TreeView_Handling.PopulateTreeView(this.jet, source, searchOption, treeItemPath, true);
        }

        public void OpenFile(string editorText, string tabName, string filepath = "")
        {
            TabItem tab = new TabItem();
            tab.Header = tabName;

            LinedTextBox_UC textbox = new LinedTextBox_UC()
            {
                Text = editorText,
                FilePath = filepath,
                TabName = tabName,
                Tab_Owner = tab,
                IsFromJet = true
            };

            if (tabName.ToLower().Contains("analyzer"))
                textbox.IsAnalyzerResult = true;

            tab.Content = textbox;

            TabTextEditor_UC.TabController.Items.Add(tab);
            TabTextEditor_UC.TabController.SelectedIndex = TabTextEditor_UC.TabController.Items.Count - 1;
        }

        private void OpenFile(string path)
        {
            //Get the parent tab item for the tab item at this path
            foreach (var item in LinedTextBox_UC.OpenedFiles)
            {
                if (item.FilePath != path)
                    continue;
                
                TabTextEditor_UC.TabController.SelectedItem = item.Tab_Owner;
                return;
            }

            string[] split = path.Split('/');
            OpenFile(jet.ReadFileInZip(path), split[split.Length-1].Replace("/",""), path);
        }


        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scv = (ScrollViewer)sender;
            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
            e.Handled = true;
        }

        private void File_New_Button_Click(object sender, RoutedEventArgs e)
        {
            NewProj_UC newProj = new NewProj_UC();
            newProj.Height = MainWindow.Instance.ContentPanel.ActualHeight;
            MainWindow.Instance = new MainWindow();
            MainWindow.Instance.WindowTitle = "       New Project";
            MainWindow.Instance.Show();
            MainWindow.Instance.ContentPanel.Children.Add(newProj);
        }

        private void File_Open_Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.InitialDirectory = Environment.CurrentDirectory;
            fileDialog.Title = "Select project path";
            fileDialog.CheckFileExists = true;
            fileDialog.CheckPathExists = true;
            fileDialog.Multiselect = false;
            fileDialog.DefaultExt = "wbp";
            fileDialog.Filter = "Workbench projects (*.wbp)|*.wbp";

            var result = fileDialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                if (!UserData.Instance.PreviousProjects.Contains(fileDialog.FileName))
                {
                    UserData.Instance.PreviousProjects.Add(fileDialog.FileName);
                    UserData.SaveUserData();
                }

                Wbp proj = new Wbp(fileDialog.FileName);
                ProjectData data = proj.getProjectData();
                data.WBP_Path = fileDialog.FileName;
                data.LastOpened = DateTime.Now;
                proj.setProjectData(data);

                JetEditor jetEditor = new JetEditor(fileDialog.FileName);
                MainWindow.Instance.Close();
            }
        }

        private void JetEditor_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if ((Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)) &&
                (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift)) && Keyboard.IsKeyDown(Key.S))
            {
                LinedTextBox_UC linedTB = new LinedTextBox_UC();
                linedTB.OnSaveAllOpenedFiles(new LinedTextBox_UC.LinedTBEventArgs());
            }
            else if ((Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)) && Keyboard.IsKeyDown(Key.S))
            {
                var selectedTab = TabTextEditor_UC.TabController.SelectedItem;
                foreach (var tab in LinedTextBox_UC.OpenedFiles)
                {
                    if (tab.Tab_Owner == selectedTab)
                        tab.OnSaveFile(new LinedTextBox_UC.LinedTBEventArgs());
                }
            }
        }

        private void Analysis_Click(object sender, RoutedEventArgs e)
        {
            var analysis = new Analysis_UC();
            analysis.Height = MainWindow.Instance.ContentPanel.ActualHeight;

            MainWindow.Instance = new MainWindow();
            MainWindow.Instance.WindowTitle = "              Analysis";
            MainWindow.Instance.Show();
            MainWindow.Instance.ContentPanel.Children.Add(analysis);
        }

        private void Editor_Closed(object sender, EventArgs e)
        {
            OpenedJetEditors.Remove(this);
        }
    }
}
