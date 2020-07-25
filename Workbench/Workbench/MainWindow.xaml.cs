using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using BTD_Backend;
using BTD_Backend.IO;
using BTD_Backend.Persistence;
using Workbench.UserControls;

namespace Workbench
{
    /// <summary>
    /// Logica di interazione per MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MainWindow Instance;
        
        public MainWindow()
        {
            InitializeComponent();
            Instance = this;
            //ContPanel = ContentPanel;
            Log.Instance.MessageLogged += MainWindow_MessageLogged;
            
            Log.Output("Welcome to BTD Workbench");
            


#if DEBUG
            Console.WriteLine("DEBUG");
#else
            Console.WriteLine("RELEASE");
            Win32.ShowWindow(Win32.GetConsoleWindow(), (int)Win32.SW_HIDE);
#endif
        }

        private void MainWindow_MessageLogged(object sender, Log.LogEvents e)
        {
            Console.WriteLine(e.Message);
            /*ConsoleLog.Dispatcher.BeginInvoke((Action)(() =>
            {
                ConsoleLog.AppendText(e.Message);
                ConsoleLog.ScrollToEnd();                
            }));*/
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            UserData.MainProgramName = "BTD Workbench";
            UserData.MainProgramExePath = Environment.CurrentDirectory + "\\Workbench.exe";
            UserData.MainSettingsDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\BTD Workbench";

            UserData.LoadUserData();

            Welcome_UC welcome = new Welcome_UC();
            welcome.Height = ContentPanel.ActualHeight;
            ContentPanel.Children.Add(welcome);
        }

        private void ContentPanel_SizeChanged(object sender, SizeChangedEventArgs e)
        {

        }
    }
}
