using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
    /// Logica di interazione per TabTextEditor_UC.xaml
    /// </summary>
    public partial class TabTextEditor_UC : UserControl
    {
        int TabIndex = 1;
        ObservableCollection<TabVM> Tabs = new ObservableCollection<TabVM>();

        public TabTextEditor_UC()
        {
            InitializeComponent();
            var tab1 = new TabVM()
            {
                Header = "EDITOR",
                Content = new ContentVM(new LinedTextBox_UC())
            };
            Tabs.Add(tab1);
            AddNewPlusButton();

            TabController.ItemsSource = Tabs;
            TabController.SelectionChanged += MyTabControl_SelectionChanged;
        }

        private void MyTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source is TabControl)
            {
                var pos = TabController.SelectedIndex;
                if (pos != 0 && pos == Tabs.Count - 1) //last tab
                {
                    var tab = Tabs.Last();
                    ConvertPlusToNewTab(tab);
                    AddNewPlusButton();
                }
            }
        }

        void ConvertPlusToNewTab(TabVM tab)
        {
            //Do things to make it a new tab.
            TabIndex++;
            tab.Header = $"Tab {TabIndex}";
            tab.IsPlaceholder = false;
            tab.Content = new ContentVM("Tab content", TabIndex);
        }

        void AddNewPlusButton()
        {
            var plusTab = new TabVM()
            {
                Header = "+",
                IsPlaceholder = true
            };
            Tabs.Add(plusTab);
        }

        class TabVM : INotifyPropertyChanged
        {
            string _Header;
            public string Header
            {
                get => _Header;
                set
                {
                    _Header = value;
                    OnPropertyChanged();
                }
            }

            bool _IsPlaceholder = false;
            public bool IsPlaceholder
            {
                get => _IsPlaceholder;
                set
                {
                    _IsPlaceholder = value;
                    OnPropertyChanged();
                }
            }

            ContentVM _Content = null;
            public ContentVM Content
            {
                get => _Content;
                set
                {
                    _Content = value;
                    OnPropertyChanged();
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;
            void OnPropertyChanged([CallerMemberName] string property = "")
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
            }
        }

        class ContentVM
        {
            public ContentVM(LinedTextBox_UC linedTB)
            {
                LinedTextBox = linedTB;
            }
            public ContentVM(string name, int index)
            {
                Name = name;
                Index = index;
            }

            public LinedTextBox_UC LinedTextBox { get; set; }
            public string Name { get; set; }
            public int Index { get; set; }
        }

        private void OnTabCloseClick(object sender, RoutedEventArgs e)
        {
            var tab = (sender as Button).DataContext as TabVM;
            if (Tabs.Count > 2)
            {
                var index = Tabs.IndexOf(tab);
                if (index == Tabs.Count - 2)//last tab before [+]
                {
                    TabController.SelectedIndex--;
                }
                Tabs.RemoveAt(index);
            }
        }

        public void AddFile(string file)
        {

        }

        private void TabItem_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
