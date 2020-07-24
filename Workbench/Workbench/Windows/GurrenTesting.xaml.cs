using BTD_Backend;
using BTD_Backend.Game;
using BTD_Backend.IO;
using BTD_Backend.Persistence;
using BTD_Backend.NKHook5;
using BTD_Backend.Save_editing;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Reflection;

namespace Workbench
{
    /// <summary>
    /// Interaction logic for GurrenTesting.xaml
    /// </summary>
    public partial class GurrenTesting : Window
    {
        List<string> list = new List<string>();
        public GurrenTesting()
        {
            InitializeComponent();

            
            BTD_Backend.IO.Zip zip = new BTD_Backend.IO.Zip(Environment.CurrentDirectory + "\\BTD5.jet");
            foreach (var file in zip.Archive.Entries)
            {
                list.Add(file.FileName);
            }
        }

        private void DownloadButton_Click(object sender, RoutedEventArgs e)
        {
            NKHook5Manager.DownloadNKH();
        }

        private void OpenNkhDirButton_Click(object sender, RoutedEventArgs e)
        {
            NKHook5Manager.OpenNkhDir();
        }

        private void RunNKHButton_Click(object sender, RoutedEventArgs e)
        {
            NKHook5Manager.LaunchNKH();
        }

        private void UpdateNKhButton_Click(object sender, RoutedEventArgs e)
        {
            NKHook5Manager.HandleUpdates();
        }

        private void GetSaveDirButton_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void GetFilesInJetButton_Click(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
