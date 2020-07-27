using BTD_Backend.Persistence;
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
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
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
            MallisTesting mallisTesting = new MallisTesting();
            mallisTesting.WindowState = WindowState.Maximized;
            mallisTesting.Show();
            MainWindow.Instance.Close();
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

                /*System.Windows.MessageBox.Show(fileDialog.FileName);
                ProjectData projectData = new ProjectData(fileDialog.FileName);*/
                var proj = ProjectData.LoadProject(fileDialog.FileName);
                proj.WBP_Path = fileDialog.FileName;
                proj.SaveProject();
                //projectData.WBP_Path = fileDialog.FileName;




                MallisTesting mallis = new MallisTesting();
                mallis.Wbp_Path = fileDialog.FileName;

                mallis.WindowState = WindowState.Maximized;
                mallis.Show();
                MainWindow.Instance.Close();
            }
        }
    }
}
