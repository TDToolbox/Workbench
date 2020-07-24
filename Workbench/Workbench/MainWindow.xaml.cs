using System;
using System.Runtime.InteropServices;
using System.Windows;
using BTD_Backend;

namespace Workbench
{
    /// <summary>
    /// Logica di interazione per MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        const int SW_HIDE = 0;
        const int SW_SHOW = 5;


        public MainWindow()
        {
            InitializeComponent();
            Log.Instance.MessageLogged += MainWindow_MessageLogged;
            Log.Output("Welcome to BTD Workbench");

#if DEBUG
            Console.WriteLine("DEBUG");
#else
            Console.WriteLine("RELEASE");
            ShowWindow(GetConsoleWindow(), SW_HIDE);
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
    }
}
