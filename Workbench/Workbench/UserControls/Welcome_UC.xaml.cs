using BTD_Backend;
using BTD_Backend.IO;
using BTD_Backend.Persistence;
using System;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using UserControl = System.Windows.Controls.UserControl;

namespace Workbench.UserControls
{
    /// <summary>
    /// Interaction logic for Welcome_UC.xaml
    /// </summary>
    public partial class Welcome_UC : UserControl
    {
        
        public Welcome_UC()
        {
            InitializeComponent();
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow main = new MainWindow();
            main.Show();
        }

        private void ContinueWithoutCode_Button_MouseUp(object sender, MouseButtonEventArgs e)
        {
            JetEditor jetEditor = new JetEditor();
            jetEditor.WindowState = WindowState.Maximized;
            jetEditor.Show();
            MainWindow.Instance.Close();
            //MainWindow.Instance.Visibility = Visibility.Hidden;
        }

        private void NewProject_Button_Click(object sender, RoutedEventArgs e)
        {
            NewProj_UC newProj = new NewProj_UC();
            newProj.Height = MainWindow.Instance.ContentPanel.ActualHeight;
            
            MainWindow.Instance.ContentPanel.Children.Add(newProj);
            MainWindow.Instance.ContentPanel.Children.Remove(this);
        }

        private void OpenProject_Button_Click(object sender, RoutedEventArgs e)
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
            if (result == DialogResult.OK)
            {
                if (!UserData.Instance.PreviousProjects.Contains(fileDialog.FileName))
                {
                    UserData.Instance.PreviousProjects.Add(fileDialog.FileName);
                    UserData.SaveUserData();
                }

                Wbp proj = new Wbp(fileDialog.FileName);
                ProjectData.Instance = proj.getProjectData();
                ProjectData.Instance.WBP_Path = fileDialog.FileName;
                ProjectData.Instance.LastOpened = DateTime.Now;                
                proj.setProjectData(ProjectData.Instance); //update projData to have latest info (like date and path)

                bool safe;
                JetEditor jetEditor = new JetEditor(fileDialog.FileName, out safe);

                if (safe)
                {
                    jetEditor.Show();
                    MainWindow.Instance.Close();
                }
            }
        }
    }
}
