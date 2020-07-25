using BTD_Backend.Game;
using BTD_Backend.Game.Jet_Files;
using BTD_Backend.IO;
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
        public GameType Game { get; set; } = GameType.None;
        public string ModType { get; set; }
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
            ModType = "";
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
        }

        string battlesPass = "";
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

        private void ResetButtonColors(byte alpha = 255)
        {
            var color = new SolidColorBrush(Color.FromArgb(alpha, 255, 152, 0));
            JetMod_LBItem.Background = color;
            JetMod_LBItem.BorderBrush = color;
            SaveMod_LBItem.Background = color;
            SaveMod_LBItem.BorderBrush = color;
            NKHMod_LBItem.Background = color;
            NKHMod_LBItem.BorderBrush = color;
        }

        private void JetMod_LBItem_Click(object sender, RoutedEventArgs e)
        {
            ResetButtonColors(200);
            ModType = "Jet";
            JetMod_LBItem.Background = Brushes.LimeGreen;
            JetMod_LBItem.BorderBrush = Brushes.Black;
        }

        private void SaveMod_LBItem_Click(object sender, RoutedEventArgs e)
        {
            ResetButtonColors(200);
            ModType = "Save";
            ProjPass_TextBox.Text = "";
            ProjPass_TextBox.IsEnabled = false;

            SaveMod_LBItem.Background = Brushes.LimeGreen;
            SaveMod_LBItem.BorderBrush = Brushes.Black;
        }

        private void NKHMod_LBItem_Click(object sender, RoutedEventArgs e)
        {
            ResetButtonColors(200);
            ModType = "NKH";
            NKHMod_LBItem.Background = Brushes.LimeGreen;
            NKHMod_LBItem.BorderBrush = Brushes.Black;
        }

        private void Back_Button_Click(object sender, RoutedEventArgs e)
        {
            Welcome_UC welcome = new Welcome_UC();
            welcome.Height = MainWindow.Instance.ContentPanel.ActualHeight;
            MainWindow.Instance.ContentPanel.Children.Add(welcome);
            MainWindow.Instance.ContentPanel.Children.Remove(this);
        }

        private void Create_Button_Click(object sender, RoutedEventArgs e)
        {
            if (Game == GameType.None)
            {
                MessageBox.Show("Please select which game you want to create a project for");
                return;
            }

            if (String.IsNullOrEmpty(ModType))
            {
                MessageBox.Show("Please select which type of mod you want to make");
                return;
            }

            if (ProjName_TextBox.Text.Length <= 0)
            {
                MessageBox.Show("You need to enter a project name before you can continue");
                return;
            }

            if (ProjPass_TextBox.Text.Length <= 0 && Game == GameType.BTDB)
            {
                MessageBox.Show("You need to enter a password for BTD Battles jet file before you can continue");
                return;
            }
        }

        private void ProjName_TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void BrowseLocation_Button_Click(object sender, RoutedEventArgs e)
        {
            /*CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;
            dialog.Title = "Select the project folder";
            dialog.Multiselect = false;
            dialog.InitialDirectory = Environment.CurrentDirectory;

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                ProjLocation_TextBox.Text = dialog.FileName;
                ProjLocation_TextBox.Select(ProjLocation_TextBox.Text.Length - 1, ProjLocation_TextBox.Text.Length - 1);
            }*/
        }
    }
}
