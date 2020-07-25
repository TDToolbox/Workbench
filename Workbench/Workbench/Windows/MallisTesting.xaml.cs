using BTD_Backend;
using BTD_Backend.IO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using Workbench.UserControls;

namespace Workbench
{
    /// <summary>
    /// Interaction logic for MallisTesting.xaml
    /// </summary>
    public partial class MallisTesting : Window
    {

        public MallisTesting()
        {
            InitializeComponent();
        }


        private void MallisTestingWindow_Loaded(object sender, RoutedEventArgs e)
        {
            /*Zip jet = new Zip(Environment.CurrentDirectory + "\\BTD5.jet");
            jet.Password = jet.TryGetPassword();
            //MessageBox.Show("Password: " + jet.Password);

            var entries = jet.GetEntries(Zip.EntryType.Files, "TowerDefinitions");
            //MessageBox.Show("Got Entries");

            var text = jet.ReadFileInZip(entries[0]);
            //MessageBox.Show(text);
            LinedTextBox_UC linedTextBox = new LinedTextBox_UC();
            linedTextBox.TextEditor.Text = text;
            //ContentPanel.Children.Add(linedTextBox);*/
        }

        private void TreeViewItem_Expanded(object sender, RoutedEventArgs e)
        {
            TreeViewItem source = (TreeViewItem)e.Source;
            TreeViewItem newItem = new TreeViewItem();
            newItem.Header = "Gameing";
            Label l = new Label();
            l.Content = "Gameing";
            newItem.Items.Add(l);
            source.Items.Add(newItem);
        }

        Brush TitleButtonDown = new SolidColorBrush(Color.FromArgb(0xFF, 0x44, 0x44, 0x44));
        Brush TitleButtonHover = new SolidColorBrush(Color.FromArgb(0xFF, 0x33, 0x33, 0x33));
        Brush TitleButtonDefault = new SolidColorBrush(Color.FromArgb(0xFF, 0x22, 0x22, 0x22));
        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (TitleButton.IsMouseOver)
            {
                if(Win32.GetAsyncKeyState(1))
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

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void MaximizeButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Normal;
            }
            else
            {
                this.WindowState = WindowState.Maximized;
            }
        }
        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Minimized)
            {
                this.WindowState = WindowState.Normal;
            }
            else
            {
                this.WindowState = WindowState.Minimized;
            }
        }

        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scv = (ScrollViewer)sender;
            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
            e.Handled = true;
        }

        private void TitleButton_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            TitleButton.Background = TitleButtonDown;
            IntPtr winHandle = new WindowInteropHelper(this).Handle;
            Win32.ReleaseCapture();
            Win32.SendMessage(winHandle, Win32.WM_NCLBUTTONDOWN, Win32.HTCAPTION, 0);
        }
    }
}
