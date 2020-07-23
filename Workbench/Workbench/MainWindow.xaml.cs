using System.Windows;

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
        }

        private void TestWindow_Button_Click(object sender, RoutedEventArgs e)
        {
            TestingWindow testingWindow = new TestingWindow();
            testingWindow.Show();
        }

        private void MallisTesting_Button_Click(object sender, RoutedEventArgs e)
        {
            MallisTesting mallis = new MallisTesting();
            mallis.Show();
        }
        private void GurrenTest_Button_Click(object sender, RoutedEventArgs e)
        {
            GurrenTesting gurren = new GurrenTesting();
            gurren.Show();
        }
    }
}
