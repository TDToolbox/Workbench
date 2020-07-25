using System;
using System.Collections.Generic;
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

        private void Back_Button_Click(object sender, RoutedEventArgs e)
        {
            Welcome_UC welcome = new Welcome_UC();
            welcome.Height = MainWindow.Instance.ContentPanel.ActualHeight;
            MainWindow.Instance.ContentPanel.Children.Add(welcome);
            MainWindow.Instance.ContentPanel.Children.Remove(this);
        }
    }
}
