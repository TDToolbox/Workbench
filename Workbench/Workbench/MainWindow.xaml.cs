using System;
using System.Collections.Generic;
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
        private static bool firstLoad = true;
        public static MainWindow Instance;
        public static bool debugMode = false;
        private string windowTitle;

        public string WindowTitle
        {
            get { return windowTitle; }
            set { 
                windowTitle = value;
                Title = value.Trim();

                if (TitleBar != null)
                    TitleBar.WindowTitle = value;
            }
        }


        public MainWindow()
        {
            InitializeComponent();
            Instance = this;
            //ContPanel = ContentPanel;
            
            if (firstLoad)
            {
                Log.MessageLogged += MainWindow_MessageLogged;
                Log.Output("Welcome to BTD Workbench");
                WindowTitle = "               Welcome";
            }



#if DEBUG
            debugMode = true;
            if (firstLoad)
                Console.WriteLine("DEBUG");
#else
            Console.WriteLine("RELEASE");
            Win32.ShowWindow(Win32.GetConsoleWindow(), (int)Win32.SW_HIDE);
#endif
        }

        private void MainWindow_MessageLogged(object sender, Log.LogEvents e)
        {
            Console.WriteLine(e.Message);

            if (e.Output == OutputType.Both || e.Output == OutputType.MsgBox)
                MessageBox.Show(e.Message.Replace(">> ", ""));

            if (e.Output == OutputType.Both || e.Output == OutputType.Console)
            {
                /*ConsoleLog.Dispatcher.BeginInvoke((Action)(() =>
                {
                    ConsoleLog.AppendText(e.Message);
                    ConsoleLog.ScrollToEnd();
                }));*/
            }
        }

        private void TestWindow_Button_Click(object sender, RoutedEventArgs e)
        {
            Log.Output("Opening test window"); ;
            TestingWindow testingWindow = new TestingWindow();
            testingWindow.Show();
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

            var existingProjects = new List<string>();
            foreach (var item in UserData.Instance.PreviousProjects)
            {
                if (!File.Exists(item))
                {
                    Log.Output(item + " no longer exists... Removing from previous projects list.");
                    continue;
                }

                existingProjects.Add(item);
            }

            if (existingProjects.Count != UserData.Instance.PreviousProjects.Count)
            {
                UserData.Instance.PreviousProjects = existingProjects;
                UserData.SaveUserData();
            }

            if (firstLoad)
            {
                Welcome_UC welcome = new Welcome_UC();
                welcome.Height = ContentPanel.ActualHeight;
                ContentPanel.Children.Add(welcome);
            }

            firstLoad = false;
        }

        private void ContentPanel_SizeChanged(object sender, SizeChangedEventArgs e)
        {

        }
    }
}
