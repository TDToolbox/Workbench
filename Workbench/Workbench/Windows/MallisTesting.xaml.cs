using BTD_Backend;
using BTD_Backend.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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
                    Instance.Dispatcher.Invoke((Action)delegate
                    {
                        /*double fileEditHeight = FileEditMenu.ActualHeight;
                        Point winLoc = Instance.PointToScreen(new Point(0, 0));
                        Point mainGridLoc = Instance.PointToScreen(MainGrid.po);
                        double titlebarSize = winLoc.Y - Instance.RenderSize.;
                        Log.Output(titlebarSize + "");
                        double availableSpace = MallisTestingWindow.ActualHeight - (fileEditHeight + titlebarSize);

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
                        }*/
                    });
                    
                    Thread.Sleep(100);
                }
            });
            refresher.Start();
            BgThread.AddToQueue(() =>
            {
                Zip zip = new Zip(Environment.CurrentDirectory + "\\BTD5.jet");
                var list = zip.GetFilesInZip();

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
            JetFiles.Height = MallisTestingWindow.ActualHeight - ModFiles.ActualHeight -88;
        }

        private void ModTreeViewItem_Expanded(object sender, RoutedEventArgs e)
        {
            JetFiles.Height = MallisTestingWindow.ActualHeight - ModFiles.ActualHeight - 88 - ModTreeViewItem.ActualHeight;
        }

        private void ModTreeViewItem_Collapsed(object sender, RoutedEventArgs e)
        {
            JetFiles.Height = MallisTestingWindow.ActualHeight - ModFiles.ActualHeight - ModTreeViewItem.ActualHeight;
	    }*/
		
        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            
        }

        private void TitleGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void TitleGrid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

        }

        private void TitleGrid_MouseMove(object sender, MouseEventArgs e)
        {

        }
    }
}
