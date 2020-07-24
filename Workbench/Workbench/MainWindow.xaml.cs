using System;
using System.Windows;
using BTD_Backend;

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
