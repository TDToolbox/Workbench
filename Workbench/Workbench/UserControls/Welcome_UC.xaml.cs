using System;
using System.Collections.Generic;
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

        }

        private void NewProject_Button_Click(object sender, RoutedEventArgs e)
        {
            NewProj_UC newProj = new NewProj_UC();
            newProj.Height = MainWindow.Instance.ContentPanel.ActualHeight;
            MainWindow.Instance.ContentPanel.Children.Add(newProj);
            MainWindow.Instance.ContentPanel.Children.Remove(this);
        }
    }
}
