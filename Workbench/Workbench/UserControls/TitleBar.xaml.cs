using BTD_Backend;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Workbench.UserControls
{
    /// <summary>
    /// Logica di interazione per TitleBar.xaml
    /// </summary>
    public partial class TitleBar : UserControl
    {
        public TitleBar()
        {
            InitializeComponent();
        }

        public string WindowTitle
        {
            get {
                return (string)TitleBarLabel.Content;
            }
            set {
                TitleBarLabel.Content = value;
            }
        }





        Brush TitleButtonDown = new SolidColorBrush(Color.FromArgb(0xFF, 0x44, 0x44, 0x44));
        Brush TitleButtonHover = new SolidColorBrush(Color.FromArgb(0xFF, 0x33, 0x33, 0x33));
        Brush TitleButtonDefault = new SolidColorBrush(Color.FromArgb(0xFF, 0x22, 0x22, 0x22));
        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (TitleButton.IsMouseOver)
            {
                if (Win32.GetAsyncKeyState(1))
                {
                    TitleButton.Background = TitleButtonDown;
                    //Log.Output("Mouse over and clicked");
                }
                else
                {
                    TitleButton.Background = TitleButtonHover;
                    //Log.Output("Mouse over");
                }
            }
            else
            {
                TitleButton.Background = TitleButtonDefault;
                //Log.Output("Mouse not over");
            }
        }

        private void TitleButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            TitleButton.Background = TitleButtonDefault;
        }

        private void MaximizeButton_Click(object sender, RoutedEventArgs e)
        {
            Window parent = (Window)((Grid)this.Parent).Parent;
            if (parent.WindowState == WindowState.Maximized)
            {
                parent.WindowState = WindowState.Normal;
            }
            else
            {
                parent.WindowState = WindowState.Maximized;
            }
        }
        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            Window parent = (Window)((Grid)this.Parent).Parent;
            if (parent.WindowState == WindowState.Minimized)
            {
                parent.WindowState = WindowState.Normal;
            }
            else
            {
                parent.WindowState = WindowState.Minimized;
            }
        }
        private void TitleButton_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Window parent = (Window)((Grid)this.Parent).Parent;
            TitleButton.Background = TitleButtonDown;
            IntPtr winHandle = new WindowInteropHelper(parent).Handle;
            Win32.ReleaseCapture();
            Win32.SendMessage(winHandle, Win32.WM_NCLBUTTONDOWN, Win32.HTCAPTION, 0);
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Window parent = (Window)((Grid)this.Parent).Parent;
            parent.Close();
        }
    }
}
