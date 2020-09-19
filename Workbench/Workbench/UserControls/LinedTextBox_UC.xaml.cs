using BTD_Backend;
using BTD_Backend.IO;
using BTD_Backend.Persistence;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;

namespace Workbench.UserControls
{
    /// <summary>
    /// Interaction logic for LinedTextBox_UC.xaml
    /// </summary>
    public partial class LinedTextBox_UC : UserControl
    {
        bool fileJustOpened = false;
        bool autosaveTimerRunning = false;
        bool manualSaveInProgress = false;
        bool cancelAutoSave = false;
        string lastSavedText = "";

        #region Constructors

        public LinedTextBox_UC()
        {
            InitializeComponent();

            AutosaveTimerStart += LinedTextBox_UC_AutosaveTimerStart;
            AutosaveTimerFinished += LinedTextBox_UC_AutosaveTimerFinished;

            if (!IsAnalyzerResult)
            {
                SaveFile += LinedTextBox_UC_SaveFile;
                SaveAllOpenedFiles += LinedTextBox_UC_SaveAllOpenedFiles;
            }            

            if (OpenedFiles == null)
                OpenedFiles = new List<LinedTextBox_UC>();

            OpenedFiles.Add(this);

            string json = Properties.Resources.BJson;
            using (Stream s = GenerateStreamFromString(json))
            {
                using (XmlTextReader reader = new XmlTextReader(s))
                {
                    TextEditor.SyntaxHighlighting = HighlightingLoader.Load(reader, HighlightingManager.Instance);
                }
            }
        }



        #endregion


        #region Properties

        public static List<LinedTextBox_UC> OpenedFiles;
        public string Text { get; set; }
        public string FilePath { get; set; }
        public string TabName { get; set; }
        public TabItem Tab_Owner { get; set; }
        public bool IsFromJet { get; set; } = false;
        public bool IsAnalyzerResult { get; set; } = false;
        #endregion



        #region UI Events
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(Text))
                fileJustOpened = true;

            TextEditor.Text = Text;
            lastSavedText = Text;
        }

        private void TextEditor_TextChanged(object sender, EventArgs e)
        {
            Text = TextEditor.Text;

            if (autosaveTimerRunning || IsAnalyzerResult) //Always keep autosave section at end of method
                return;

            if (fileJustOpened)
            {
                fileJustOpened = false;
                return;
            }

            autosaveTimerRunning = true;
            var args = new LinedTBEventArgs();
            args.TextBeforeSave = TextEditor.Text;
            OnAutosaveTimerStart(args);
        }

        #endregion
        private void LinedTextBox_UC_AutosaveTimerStart(object sender, LinedTBEventArgs e)
        {
            if (!autosaveTimerRunning)
                autosaveTimerRunning = true;

            var now = DateTime.Now;

            new Thread(() =>
            {
                for (int i = 0; i < Settings.Instance.AutosaveTime/1000; i++)
                {
                    if (cancelAutoSave)
                    {
                        cancelAutoSave = false;
                        autosaveTimerRunning = false;
                        return;
                    }
                    
                    Thread.Sleep(1000);
                }
                
                if (Text == lastSavedText)
                {
                    autosaveTimerRunning = false;
                    cancelAutoSave = false;
                    return;
                }

                var args = new LinedTBEventArgs();
                OnAutosaveTimerFinished(args);
            }).Start();
        }

        private void LinedTextBox_UC_AutosaveTimerFinished(object sender, LinedTBEventArgs e)
        {
            autosaveTimerRunning = false;
            cancelAutoSave = false;
            OnSaveFile(new LinedTBEventArgs());
        }

        
        private void LinedTextBox_UC_SaveFile(object sender, LinedTBEventArgs e)
        {
            if (manualSaveInProgress)
                return;

            if (autosaveTimerRunning)
                cancelAutoSave = true;

            if (Text == lastSavedText)
            {
                cancelAutoSave = false;
                autosaveTimerRunning = false;
                return;
            }

            manualSaveInProgress = true;

            if (IsFromJet)
            {
                string projPath = ProjectData.Instance.WBP_Path;
                var proj = new Zip(projPath);

                string pathInZip = "/Jet_Mod/" + FilePath;

                if (proj.Archive.ContainsEntry(pathInZip))
                    proj.Archive.RemoveEntry(pathInZip);

                proj.Archive.AddEntry(pathInZip, Text);
                proj.Archive.Save();
            }

            lastSavedText = Text;
            manualSaveInProgress = false;
        }

        private void LinedTextBox_UC_SaveAllOpenedFiles(object sender, LinedTBEventArgs e)
        {
            OnSaveFile(new LinedTBEventArgs());
        }



        public static Stream GenerateStreamFromString(string s)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        



        #region Events

        public event EventHandler<LinedTBEventArgs> AutosaveTimerStart;
        public event EventHandler<LinedTBEventArgs> AutosaveTimerFinished;
        public event EventHandler<LinedTBEventArgs> SaveFile;
        public static event EventHandler<LinedTBEventArgs> SaveAllOpenedFiles;

        public class LinedTBEventArgs : EventArgs
        {
            public string TextBeforeSave { get; set; }
            //public string TextToSave { get; set; }
        }

        public void OnAutosaveTimerStart(LinedTBEventArgs e)
        {
            EventHandler<LinedTBEventArgs> handler = AutosaveTimerStart;
            if (handler != null)
                handler(this, e);
        }

        public void OnAutosaveTimerFinished(LinedTBEventArgs e)
        {
            EventHandler<LinedTBEventArgs> handler = AutosaveTimerFinished;
            if (handler != null)
                handler(this, e);
        }

        public void OnSaveFile(LinedTBEventArgs e)
        {
            EventHandler<LinedTBEventArgs> handler = SaveFile;
            if (handler != null)
                handler(this, e);
        }

        public void OnSaveAllOpenedFiles(LinedTBEventArgs e)
        {
            EventHandler<LinedTBEventArgs> handler = SaveAllOpenedFiles;
            if (handler != null)
                handler(this, e);
        }

        #endregion
    }
}
