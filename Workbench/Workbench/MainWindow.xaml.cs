using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using BTD_Backend;
using BTD_Backend.Persistence;
using Workbench.UserControls;

namespace Workbench
{
    /// <summary>
    /// Logica di interazione per MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Log.Instance.MessageLogged += MainWindow_MessageLogged;
            Log.Output("Welcome to BTD Workbench");

            foreach(FileInfo file in new DirectoryInfo(Environment.CurrentDirectory).GetFiles())
            {
                if(file.Extension == ".dll")
                {
                    Assembly.LoadFrom(file.FullName);
                }
            }

            UserData.MainProgramName = "BTD Workbench";
            UserData.MainProgramExePath = Environment.CurrentDirectory + "\\Workbench.exe";
            UserData.MainSettingsDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\BTD Workbench";

            UserData.LoadUserData();
            if (UserData.Instance.NewUser)
            {
                Welcome_UC welcome = new Welcome_UC();
                ContentPanel.Children.Add(welcome);
            }
            
#if DEBUG
            Console.WriteLine("DEBUG");
#else
            Console.WriteLine("RELEASE");
            Win32.ShowWindow(Win32.GetConsoleWindow(), (int)Win32.SW_HIDE);
#endif
        }

        private void MainWindow_MessageLogged(object sender, Log.LogEvents e)
        {
            ConsoleLog.Dispatcher.BeginInvoke((Action)(() =>
            {
                ConsoleLog.AppendText(e.Message);
                ConsoleLog.ScrollToEnd();

                Console.WriteLine(e.Message);
            }));
        }

        private void TestWindow_Button_Click(object sender, RoutedEventArgs e)
        {
            Log.Output("Opening test window"); ;
            TestingWindow testingWindow = new TestingWindow();
            testingWindow.Show();
        }

        private void MallisTesting_Button_Click(object sender, RoutedEventArgs e)
        {
            Log.Output("Opening Mallis test window");
            MallisTesting mallis = new MallisTesting();
            mallis.Show();
        }
        private void GurrenTest_Button_Click(object sender, RoutedEventArgs e)
        {
            Log.Output("Opening Gurren test window");
            GurrenTesting gurren = new GurrenTesting();
            gurren.Show();
        }

        private void OpenSettings_Button_Click(object sender, RoutedEventArgs e)
        {
            UserData.OpenSettingsDir();
        }
    }
}
