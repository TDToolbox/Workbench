using BTD_Backend;
using BTD_Backend.Game;
using BTD_Backend.IO;
using BTD_Backend.Persistence;
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
    /// Interaction logic for Analysis_UC.xaml
    /// </summary>
    public partial class Analysis_UC : UserControl
    {
        List<CheckBox> searchCheckBoxes = new List<CheckBox>();
        Zip jet = new Zip(GameInfo.GetGame(ProjectData.Instance.TargetGame).JetPath, true);
        string selectedCbName = "";

        enum SearchType
        {
            Files,
            Fields,
            FileText
        }

        public Analysis_UC()
        {
            if (ProjectData.Instance.TargetGame == GameType.None)
            {
                Log.Output("Can't use Analyzer because no game was selected. Please open a project to continue", OutputType.Both);
                return;
            }
            InitializeComponent();
            
            searchCheckBoxes.Add(SearchForFields_CB);
            searchCheckBoxes.Add(SearchForFilePaths_CB);
            searchCheckBoxes.Add(SearchForFiles_CB);
            searchCheckBoxes.Add(SearchForText_CB);
            searchCheckBoxes.Add(SearchForSharedText_CB);
        }

        private void Checkbox_Checked(object sender, RoutedEventArgs e)
        {
            selectedCbName = "";
            foreach (var item in searchCheckBoxes)
            {
                if (item != sender)
                {
                    item.IsChecked = false;
                    continue;
                }

                selectedCbName = item.Name;
            }
        }

        bool caseSensitive = false;
        bool ignoreDup = true;
        private void Analyze_Button_Click(object sender, RoutedEventArgs e)
        {
            caseSensitive = CaseSensitive_CB.IsChecked.Value;
            ignoreDup = IgnoreDup_CB.IsChecked.Value;
            
            BgThread.AddToQueue(() => Analyze());
        }

        private void Analyze()
        {
            if (String.IsNullOrEmpty(selectedCbName))
            {
                Log.Output("You need to chose what to search for. Check one of the CheckBoxes to continue...", OutputType.Both);
                return;
            }

            var result = new List<string>();
            string fileName = "", filePath = "", fileText = "";
            
            Application.Current.Dispatcher.Invoke((Action)delegate {
                fileName = FileName_Contains_TB.Text;
                filePath = FilePath_Contains_TB.Text;
                fileText = FileText_Contains_TB.Text;
            });

            if (selectedCbName.ToLower().Replace(" ", "").Contains("forfile"))
                result = Search(SearchType.Files, "", fileName, filePath);
            else if (selectedCbName.ToLower().Replace(" ", "").Contains("fortext"))
                result = Search(SearchType.FileText, fileText, fileName, filePath);
            else if (selectedCbName.ToLower().Replace(" ", "").Contains("sharedtext"))
                result = SearchForSharedText(fileText, fileName, filePath);
            else
            {
                Log.Output("Something went wrong! Failed to find the checkbox with the name: \"" + selectedCbName + "\"." +
                    " This is an error, contact dev's if you are seeing this.", OutputType.Both);
                return;
            }


            if (result.Count == 0)
            {
                Log.Output("No results found", OutputType.Both);
                return;
            }

            Log.Output("Analyzer Result.Count = " + result.Count);

            var str = "";
            foreach (var element in result)
                str += element + "\n";

            Application.Current.Dispatcher.Invoke((Action)delegate {
                if (JetEditor.OpenedJetEditors.Count == 0)
                    new JetEditor();

                JetEditor.OpenedJetEditors[0].OpenFile(str, "Analyzer Results");
                JetEditor.OpenedJetEditors[0].Focus();
            });            
        }


        private List<string> SearchForSharedText(string searchText = "", string file = "", string path = "")
        {
            
            int count = 0;
            int skipAmount = 1;
            var result = new List<string>();
            var initialFiles = Search(SearchType.Files, searchText, file, path);

            if (initialFiles.Count == 0)
                return result;

            string[] initialLines = jet.ReadFileInZip(initialFiles[0].TrimStart('"').TrimEnd('"')).Split('\n');

            TotalProgress_PB.Dispatcher.BeginInvoke((Action)(() =>
            {
                TotalProgress_PB.Value = 0;
                TotalProgress_PB.Maximum = initialLines.Length;
            }));


            foreach (var l in initialLines)
            {
                count++;

                if (count >= skipAmount)
                {
                    UpdateProgressBar(skipAmount);
                    count = 0;
                }

                string tempLine = l;
                if (!tempLine.Contains(searchText))
                    continue;

                foreach (var fi in initialFiles)
                {
                    string fiText = jet.ReadFileInZip(fi.TrimStart('"').TrimEnd('"'));

                    if (!caseSensitive)
                    {
                        fiText = fiText.ToLower();
                        tempLine = tempLine.ToLower();
                    }

                    if (ignoreDup)
                    {
                        if (result.Contains(l))
                            continue;
                    }

                    result.Add(l);
                }
            }

            return result;
        }


        private List<string> Search(SearchType searchType, string searchText = "", string file = "", string path = "")
        {
            int count = 0;
            int skipAmount = 1;
            var result = new List<string>();

            TotalProgress_PB.Dispatcher.BeginInvoke((Action)(() =>
            {
                TotalProgress_PB.Value = 0;
                TotalProgress_PB.Maximum = jet.Archive.Entries.Count;
            }));

            foreach (var f in jet.Archive.Entries)
            {
                count++;
                if (count >= skipAmount)
                {
                    UpdateProgressBar(skipAmount);
                    count = 0;
                }


                string fName = f.FileName;
                if (!caseSensitive)
                {
                    fName = fName.ToLower();
                    searchText = searchText.ToLower();
                    file = file.ToLower();
                    path = path.ToLower();
                }

                if (!fName.Contains(path) || !fName.Contains(file))
                    continue;

                string fileText = jet.ReadFileInZip(f.FileName);

                if (!caseSensitive)
                    fileText = fileText.ToLower();


                if (!fileText.Contains(searchText))
                    continue;

                string temp = "";
                if (searchType == SearchType.FileText)
                {
                    foreach (var line in fileText.Split('\n'))
                    {
                        string tempLine = line.Trim();
                        if (!caseSensitive)
                            tempLine = tempLine.ToLower();

                        if (!tempLine.Contains(searchText))
                            continue;

                        temp = tempLine;
                        break;
                    }
                }
                else
                {
                    if (searchType == SearchType.Files)
                        temp = "\"" + f.FileName + "\"";
                }                

                if (ignoreDup)
                {
                    if (result.Contains(temp))
                        continue;
                }

                if (!String.IsNullOrEmpty(temp))
                    result.Add(temp);
            }

            return result;
        }

        private void UpdateProgressBar(int amount)
        {
            TotalProgress_PB.Dispatcher.BeginInvoke((Action)(() =>
            {
                //TotalProgress_PB.Value += amount;
                if (TotalProgress_PB.Value + amount > TotalProgress_PB.Maximum)
                {
                    TotalProgress_PB.Value = TotalProgress_PB.Maximum;
                    Log.Output("Attempted to raise Analysis progress bar above the maximum amount...");
                    return;
                }
                else
                    TotalProgress_PB.Value += amount;
            }));
        }

        bool scanRunning = false;
        private void TextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (scanRunning)
                    return;

                scanRunning = true;
                caseSensitive = CaseSensitive_CB.IsChecked.Value;
                ignoreDup = IgnoreDup_CB.IsChecked.Value;

                BgThread.AddToQueue(() =>
                {
                    Analyze();
                    scanRunning = false;
                });
            }
        }

        private void IgnoreDup_CB_Click(object sender, RoutedEventArgs e)
        {
            ignoreDup = IgnoreDup_CB.IsChecked.Value;
        }
    }
}
