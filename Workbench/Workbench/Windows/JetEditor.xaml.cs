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

namespace Workbench
{
    /// <summary>
    /// Interaction logic for JetEditor.xaml
    /// </summary>
    public partial class JetEditor : Window
    {
        private Zip jet;
        ProjectData data;

        public JetEditor()
        {
            InitializeComponent();
            TreeView_Handling.TreeItemExpanded += TreeView_Handling_TreeItemExpanded;
        }

        public JetEditor(string projectPath, out bool safe) : this()
        {
            safe = true;
            try
            {
                Wbp project = new Wbp(projectPath);
                data = project.getProjectData();

                GameInfo theGame = GameInfo.GetGame(data.TargetGame);
                string jetPath = theGame.GameDir;

                if (ProjectData.Instance.TargetGame == GameType.BTD6)
                {
                    if (File.Exists(jetPath + "\\" + theGame.JetName))
                        jetPath += "\\" + theGame.JetName;
                    else if (File.Exists(jetPath + "\\BloonsTD6_Data\\" + theGame.JetName))
                        jetPath += "\\BloonsTD6_Data\\" + theGame.JetName;
                    else if (Directory.Exists(jetPath + "\\Assets") && File.Exists(jetPath + "\\Assets\\" + theGame.JetName))
                        jetPath += jetPath + "\\Assets\\" + theGame.JetName;
                }
                else
                {
                    jetPath += "\\Assets\\" + theGame.JetName;
                }

                if (File.Exists(jetPath))
                {
                    jet = new Zip(jetPath);
                    if (!String.IsNullOrEmpty(data.JetPassword))
                        jet.Password = data.JetPassword;
                }
                else
                {
                    Log.Output("Jetfile doesn't exist!");
                    Log.Output("JetPath: "+jetPath);
                    safe = false;
                    throw new Exception("Jetfile doesnt exist so the window cannot open");
                }
            }
            catch(Exception ex)
            {
                Log.Output(ex.Message);
                Log.Output(ex.StackTrace);
            }
        }

        private void JetEditor_Loaded(object sender, RoutedEventArgs e)
        {
            openJet(jet);
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

        public void PopulateTreeView(SearchOption searchOption)
        {
            if(this.jet != null)
                TreeView_Handling.PopulateTreeView(this.jet, JetView, searchOption, true);
        }

        public void PopulateTreeView(TreeViewItem source, SearchOption searchOption, string treeItemPath)
        {
            if (this.jet != null)
                TreeView_Handling.PopulateTreeView(this.jet, source, searchOption, treeItemPath, true);
        }


        private void OpenFile(string path)
        {
            string filepath = path;

            //Get the parent tab item for the tab item at this path
            foreach (var item in LinedTextBox_UC.OpenedFiles)
            {
                if (item.FilePath != filepath)
                    continue;
                
                TabTextEditor_UC.TabController.SelectedItem = item.Tab_Owner;
                return;
            }

            
            string[] nameSplit = filepath.Split('/');
            TabItem tab = new TabItem();
            tab.Header = nameSplit[nameSplit.Length - 1];

            LinedTextBox_UC textbox = new LinedTextBox_UC()
            {
                Text = this.jet.ReadFileInZip(filepath),
                FilePath = filepath,
                TabName = tab.Header.ToString(),
                Tab_Owner = tab,
                IsFromJet = true
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

        private void File_New_Button_Click(object sender, RoutedEventArgs e)
        {
            NewProj_UC newProj = new NewProj_UC();
            newProj.Height = MainWindow.Instance.ContentPanel.ActualHeight;
            MainWindow.Instance = new MainWindow();
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


                bool safe;
                JetEditor jetEditor = new JetEditor(fileDialog.FileName, out safe);

                if (safe)
                {
                    jetEditor.Show();
                    MainWindow.Instance.Close();
                }
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
    }
}
