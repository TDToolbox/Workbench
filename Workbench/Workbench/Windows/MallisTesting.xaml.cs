using BTD_Backend;
using BTD_Backend.IO;
using System;
using System.Collections.Generic;
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

namespace Workbench
{
    /// <summary>
    /// Interaction logic for MallisTesting.xaml
    /// </summary>
    public partial class MallisTesting : Window
    {
        public static MallisTesting Instance;
        public MallisTesting()
        {
            InitializeComponent();
            Instance = this;
            addTreeDel = new AddTreeItemDel(AddTreeItem);
            //AddTreeItemDel.CreateDelegate(typeof(MallisTesting), Instance, "AddTreeItem");

            Log.Output("how");

            
        }

        private void MallisTestingWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Thread refresher = new Thread(()=>
            {
                while (true)
                {
                    try
                    {

                        Instance.Dispatcher.Invoke((Action)delegate
                        {
                            double fileEditHeight = FileEditMenu.ActualHeight;
                            //Point winLoc = Instance.PointToScreen(new Point(0, 0));
                            double availableSpace = MallisTestingWindow.ActualHeight - (fileEditHeight + TitleGrid.ActualHeight + 5);

                            if (availableSpace > 0)
                            {
                                FileViewGrid.MaxHeight = availableSpace;
                                GroupBox modView = (GroupBox)FileViewGrid.Children[1];
                                GroupBox jetView = (GroupBox)FileViewGrid.Children[2];
                                double jvHeight = availableSpace - modView.ActualHeight;
                                if (jvHeight > 0)
                                {
                                    jetView.Height = jvHeight;
                                }
                            }
                            if (dragging)
                            {
                                Win32.POINT mousePoint;
                                Win32.GetCursorPos(out mousePoint);
                                IntPtr winHandle = new WindowInteropHelper(this).Handle;
                                int newX = mousePoint.X - dx;
                                int newY = mousePoint.Y - dy;
                                Win32.SetWindowPos(winHandle, IntPtr.Zero, newX, newY, (int)Instance.Width, (int)Instance.Height, Win32.SetWindowPosFlags.ShowWindow);
                            }
                        });
                    }
                    catch (Exception)
                    {
                        break;
                    }
                    
                    Thread.Sleep(1);
                }
            });
            refresher.Start();
            BgThread.AddToQueue(() =>
            {
                Zip zip = new Zip(Environment.CurrentDirectory + "\\BTD5.jet");
                var list = zip.GetEntries(Zip.EntryType.Directories, "JSON");

                int i = 0;
                foreach (var item in list)
                {
                    TreeViewItem treeItem = new TreeViewItem();
                    treeItem.Header = item;
                    addTreeDel.Invoke(treeItem);
                    /*Instance.JetTreeView.Items.Dispatcher.Invoke((Action)(() =>
                    {
                        Instance.JetTreeView.Items.Add(treeItem);
                    }));*/
                }
            }, System.Threading.ApartmentState.STA);
        }


        public delegate void AddTreeItemDel(TreeViewItem item);
        public static AddTreeItemDel addTreeDel;
        public void AddTreeItem(TreeViewItem item)
        {
            //Instance.JetTreeView.Items.Add(item);
            /*Instance.JetTreeView.Dispatcher.BeginInvoke((Action)(() =>
             {
                 
             }));*/
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

        /*private void MallisTestingWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //Subtracting 175 from ActualHeight to get the Height minus the UI elements above Jetfiles
            //JetFiles.Height = MallisTestingWindow.ActualHeight - 175;
            //JetFiles.Height = MallisTestingWindow.ActualHeight - ModFiles.ActualHeight -88;
        }

        private void ModTreeViewItem_Expanded(object sender, RoutedEventArgs e)
        {
            //JetFiles.Height = MallisTestingWindow.ActualHeight - ModFiles.ActualHeight - 88 - ModTreeViewItem.ActualHeight;
        }

        private void ModTreeViewItem_Collapsed(object sender, RoutedEventArgs e)
        {
            JetFiles.Height = MallisTestingWindow.ActualHeight - ModFiles.ActualHeight - ModTreeViewItem.ActualHeight;
	    }*/

        Brush titleGridDown = new SolidColorBrush(Color.FromArgb(0xFF, 0x44, 0x44, 0x44));
        Brush titleGridHover = new SolidColorBrush(Color.FromArgb(0xFF, 0x33, 0x33, 0x33));
        Brush titleGrid = new SolidColorBrush(Color.FromArgb(0xFF, 0x22, 0x22, 0x22));
        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (TitleGrid.IsMouseOver)
            {
                if(Win32.GetAsyncKeyState(1))
                {
                    TitleGrid.Background = titleGridDown;
                    Log.Output("Mouse over and clicked");
                }
                else
                {
                    TitleGrid.Background = titleGridHover;
                    Log.Output("Mouse over");
                }
            }
            else
            {
                TitleGrid.Background = titleGrid;
                Log.Output("Mouse not over");
            }
        }

        private void TitleGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            TitleGrid.Background = titleGridDown;
            Point winLoc = Instance.PointToScreen(new Point(0, 0));
            Win32.POINT mousePoint;
            Win32.GetCursorPos(out mousePoint);
            dx = mousePoint.X - (int)winLoc.X;
            dy = mousePoint.Y - (int)winLoc.Y;
            dragging = true;
        }

        private void TitleGrid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            TitleGrid.Background = titleGrid;
            dragging = false;
        }

        bool dragging;
        int dx;
        int dy;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Instance.Close();
        }

        /*private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            double fileEditHeight = FileEditMenu.ActualHeight;
            double availableSpace = MallisTestingWindow.ActualHeight - fileEditHeight;
            FileViewGrid.MaxHeight = availableSpace;
            GroupBox modView = (GroupBox)FileViewGrid.Children[0];
            GroupBox jetView = (GroupBox)FileViewGrid.Children[1];
            Log.Output(FileViewGrid.MaxHeight+"");
        }*/
    }
}
