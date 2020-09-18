using BTD_Backend.Game;
using BTD_Backend.Game.Jet_Files;
using BTD_Backend.IO;
using BTD_Backend.Persistence;
using Ionic.Zip;
using MaterialDesignColors;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Workbench.UserControls
{
    /// <summary>
    /// Interaction logic for NewProj_UC.xaml
    /// </summary>
    public partial class NewProj_UC : UserControl
    {
        string battlesPass = "";
        public GameType Game { get; set; } = GameType.None;

        public List<ProjectTypes> Projects;
        SolidColorBrush orangeColor = new SolidColorBrush(Color.FromArgb(255, 255, 152, 0));

        public enum ProjectTypes 
        {
            None,
            Jet_Mod,
            Save_Mod,
            NKH_Mod,
            Sprite_Mod
        }

        public NewProj_UC()
        {
            InitializeComponent();
        }
        
        

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            BTD5_LBItem.Width = GameTypes_ListB.ActualWidth;
            BTDB_LBItem.Width = GameTypes_ListB.ActualWidth;
            BMC_LBItem.Width = GameTypes_ListB.ActualWidth;
            BTD6_LBItem.Width = GameTypes_ListB.ActualWidth;

            JetMod_LBItem.Width = ProjTypes_ListB.ActualWidth;
            SaveMod_LBItem.Width = ProjTypes_ListB.ActualWidth;
            NKHMod_LBItem.Width = ProjTypes_ListB.ActualWidth;

            if (!SteamUtils.IsGameInstalled(GameType.BTD5))
                BTD5_LBItem.IsEnabled = false;
            if (!SteamUtils.IsGameInstalled(GameType.BTDB))
                BTDB_LBItem.IsEnabled = false;
            if (!SteamUtils.IsGameInstalled(GameType.BMC))
                BMC_LBItem.IsEnabled = false;
            if (!SteamUtils.IsGameInstalled(GameType.BTD6))
                BTD6_LBItem.IsEnabled = false;
        }

        private void GameTypes_ListB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Projects = new List<ProjectTypes>();
            ResetButtonColors();

            var selected = GameTypes_ListB.SelectedItem;

            if (selected == BTD5_LBItem)
            {
                Game = GameType.BTD5;
                JetMod_LBItem.IsEnabled = true;
                SaveMod_LBItem.IsEnabled = true;
                NKHMod_LBItem.IsEnabled = true;
                ProjPass_TextBox.IsEnabled = false;
                ProjPass_TextBox.Text = "Q%_{6#Px]]";
            }

            if (selected == BTDB_LBItem)
            {
                Game = GameType.BTDB;
                NKHMod_LBItem.IsEnabled = false;
                SaveMod_LBItem.IsEnabled = true;
                ProjPass_TextBox.IsEnabled = true;
                ProjPass_TextBox.Text = "";
                BTDBHandling();
            }

            if (selected == BMC_LBItem)
            {
                Game = GameType.BMC;
                JetMod_LBItem.IsEnabled = true;
                SaveMod_LBItem.IsEnabled = false;
                NKHMod_LBItem.IsEnabled = false;
                
                ProjPass_TextBox.IsEnabled = false;
                ProjPass_TextBox.Text = "Q%_{6#Px]]";
            }

            if (selected == BTD6_LBItem)
            {
                Game = GameType.BTD6;
                JetMod_LBItem.IsEnabled = true;
                SaveMod_LBItem.IsEnabled = false;
                NKHMod_LBItem.IsEnabled = false;

                ProjPass_TextBox.IsEnabled = false;
                ProjPass_TextBox.Text = "";
            }
        }

        private void ResetButtonColors()
        {
            JetMod_LBItem.Background = orangeColor;
            JetMod_LBItem.BorderBrush = orangeColor;
            SaveMod_LBItem.Background = orangeColor;
            SaveMod_LBItem.BorderBrush = orangeColor;
            NKHMod_LBItem.Background = orangeColor;
            NKHMod_LBItem.BorderBrush = orangeColor;
        }

       
        private void BTDBHandling()
        {
            if (!String.IsNullOrEmpty(battlesPass))
            {
                ProjPass_TextBox.Text = battlesPass;
                return;
            }

            string gameDir = SteamUtils.GetGameDir(GameType.BTDB);

            if (String.IsNullOrEmpty(gameDir))
                return;

            string jetPath = "";
            foreach (var item in new DirectoryInfo(gameDir + "\\Assets").GetFiles("*.jet", SearchOption.TopDirectoryOnly))
            {
                if (item.Name.ToLower() != "data.jet")
                    continue;

                jetPath = item.FullName;
            }

            if (String.IsNullOrEmpty(jetPath))
                return;

            Zip btdb = new Zip(jetPath);
            string pass = btdb.TryGetPassword();

            if (String.IsNullOrEmpty(pass))
                return;

            battlesPass = pass;
            ProjPass_TextBox.Text = pass;            
        }

        

        private void JetMod_LBItem_Click(object sender, RoutedEventArgs e)
        {
            if (Projects.Contains(ProjectTypes.Jet_Mod))
            {
                Projects.Remove(ProjectTypes.Jet_Mod);
                JetMod_LBItem.Background = orangeColor;
                JetMod_LBItem.BorderBrush = orangeColor;
            }
            else
            {
                Projects.Add(ProjectTypes.Jet_Mod);
                JetMod_LBItem.Background = Brushes.LimeGreen;
                JetMod_LBItem.BorderBrush = Brushes.Black;
            } 
        }

        private void SaveMod_LBItem_Click(object sender, RoutedEventArgs e)
        {

            if (Projects.Contains(ProjectTypes.Save_Mod))
            {
                Projects.Remove(ProjectTypes.Save_Mod);
                SaveMod_LBItem.Background = orangeColor;
                SaveMod_LBItem.BorderBrush = orangeColor;
            }
            else
            {
                Projects.Add(ProjectTypes.Save_Mod);
                SaveMod_LBItem.Background = Brushes.LimeGreen;
                SaveMod_LBItem.BorderBrush = Brushes.Black;
            }
        }

        private void NKHMod_LBItem_Click(object sender, RoutedEventArgs e)
        {
            if (Projects.Contains(ProjectTypes.NKH_Mod))
            {
                Projects.Remove(ProjectTypes.NKH_Mod);
                NKHMod_LBItem.Background = orangeColor;
                NKHMod_LBItem.BorderBrush = orangeColor;
            }
            else
            {
                Projects.Add(ProjectTypes.NKH_Mod);
                NKHMod_LBItem.Background = Brushes.LimeGreen;
                NKHMod_LBItem.BorderBrush = Brushes.Black;
            }
        }

        private void Back_Button_Click(object sender, RoutedEventArgs e)
        {
            Welcome_UC welcome = new Welcome_UC();
            welcome.Height = MainWindow.Instance.ContentPanel.ActualHeight;
            MainWindow.Instance.ContentPanel.Children.Add(welcome);
            MainWindow.Instance.ContentPanel.Children.Remove(this);
        }

        private void BrowseLocation_Button_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog fileDialog = new SaveFileDialog();
            fileDialog.InitialDirectory = Environment.CurrentDirectory;
            fileDialog.Title = "Select project path";
            fileDialog.CheckFileExists = false;
            fileDialog.CheckPathExists = false;
            fileDialog.DefaultExt = "wbp";
            fileDialog.Filter = "Workbench projects (*.wbp)|*.wbp";

            if (!String.IsNullOrEmpty(ProjName_TextBox.Text))
                fileDialog.FileName = ProjName_TextBox.Text;

            if (fileDialog.ShowDialog().Value)
            {
                ProjLocation_TextBox.Text = fileDialog.FileName;

                if (String.IsNullOrEmpty(ProjName_TextBox.Text))
                {
                    FileInfo f = new FileInfo(fileDialog.FileName);
                    ProjName_TextBox.Text = f.Name.Replace(f.Extension, "");
                }
            }
        }

        private void Create_Button_Click(object sender, RoutedEventArgs e)
        {
            if (Game == GameType.None)
            {
                MessageBox.Show("Please select which game you want to create a project for");
                return;
            }

            if (Projects.Count == 0)
            {
                MessageBox.Show("Please select which type of mod you want to make");
                return;
            }

            if (String.IsNullOrEmpty(ProjPass_TextBox.Text) && Game == GameType.BTDB)
            {
                MessageBox.Show("You need to enter a password for BTD Battles jet file before you can continue");
                return;
            }

            if (ProjName_TextBox.Text.Length <= 0)
            {
                MessageBox.Show("You need to enter a project name before you can continue");
                return;
            }


            string projPath = ProjLocation_TextBox.Text;
            if (String.IsNullOrEmpty(projPath))
            {
                MessageBox.Show("You need to select where your project will be saved before you can continue");
                return;
            }

            if (File.Exists(projPath))
            {
                System.Windows.Forms.DialogResult diag = System.Windows.Forms.MessageBox.Show("A project with that name alread exists! Do you want to continue anyways?", "Overwrite existing project?", System.Windows.Forms.MessageBoxButtons.YesNo);
                if (diag == System.Windows.Forms.DialogResult.No)
                    return;
            }

            if (File.Exists(projPath))
                File.Delete(projPath);

            /* Create project metadata */
            ProjectData.Instance = new ProjectData(ProjName_TextBox.Text, projPath, DateTime.Now, Projects.Contains(ProjectTypes.Jet_Mod), Projects.Contains(ProjectTypes.Save_Mod), Projects.Contains(ProjectTypes.NKH_Mod), Game, null);
            string metaJson = JsonConvert.SerializeObject(ProjectData.Instance);

            ZipFile proj = new ZipFile(projPath);
            proj.AddEntry("meta.json", metaJson);
            foreach (var item in Projects)
                proj.AddDirectoryByName(item.ToString());

            proj.Save();

            bool safe;
            JetEditor jetEditor = new JetEditor(ProjectData.Instance.WBP_Path, out safe);

            if (safe)
            {
                jetEditor.WindowState = WindowState.Maximized;
                jetEditor.Show();
                MainWindow.Instance.Close();
            }
        }
    }
}
