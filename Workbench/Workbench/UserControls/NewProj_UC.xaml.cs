using BTD_Backend.Game;
using BTD_Backend.Game.Jet_Files;
using BTD_Backend.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

namespace Workbench.UserControls
{
    /// <summary>
    /// Interaction logic for NewProj_UC.xaml
    /// </summary>
    public partial class NewProj_UC : UserControl
    {
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
            var selected = GameTypes_ListB.SelectedItem;
            if (selected == BTD5_LBItem)
            {
                JetMod_LBItem.IsEnabled = true;
                SaveMod_LBItem.IsEnabled = true;
                NKHMod_LBItem.IsEnabled = true;
                ProjPass_TextBox.IsEnabled = false;
                ProjPass_TextBox.Text = "Q%_{6#Px]]";
            }
            if (selected == BTDB_LBItem)
            {
                NKHMod_LBItem.IsEnabled = false;
                SaveMod_LBItem.IsEnabled = true;
                ProjPass_TextBox.IsEnabled = true;
                ProjPass_TextBox.Text = "";
                BTDBHandling();
            }
            if (selected == BMC_LBItem)
            {
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

        private void JetMod_LBItem_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void SaveMod_LBItem_Click(object sender, RoutedEventArgs e)
        {
            ProjPass_TextBox.Text = "";
            ProjPass_TextBox.IsEnabled = false;
        }

        private void NKHMod_LBItem_Click(object sender, RoutedEventArgs e)
        {
            
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
            if (ProjName_TextBox.Text.Length <= 0)
                MessageBox.Show("You need to enter a project name before you can continue");
        }

        private void ProjName_TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
