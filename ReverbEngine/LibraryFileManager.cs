using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace ReverbEngine
{
    class LibraryFileManager
    {
        public static void LoadLibrary(string path)
        {
            // Do nothing if file does not exist
            if (!File.Exists(path)) return;
            var json = File.ReadAllText(path);
            SetCurrentStateJson(json);
        }

        public static void SaveLibrary()
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "MicroART Reverb Engine Library (*.relib)|*.relib";
            if (dialog.ShowDialog() == true)
            {
                var filePath = dialog.FileName;
                var json = GetCurrentStateJson();
                File.WriteAllText(filePath, json);
            }
        }

        private static string GetCurrentStateJson()
        {
            List<SaveableSoundRef> saveableSounds = new List<SaveableSoundRef>();
            if (MainWindow.Files.Count < 1) return null;
            // Get current state
            foreach (var t in MainWindow.Files)
            {
                SaveableSoundRef s = new SaveableSoundRef();
                s.FilePath = t.audioFile.FileName;
                s.IsLEDEnabled = t.IsLEDEnabled;
                if (t.LedColor == null)
                {
                    s.colorR = 140;
                    s.colorG = 140;
                    s.colorB = 140;
                }
                else
                {
                    s.colorR = ((SolidColorBrush)t.LedColor).Color.R;
                    s.colorG = ((SolidColorBrush)t.LedColor).Color.G;
                    s.colorB = ((SolidColorBrush)t.LedColor).Color.B;
                }
                saveableSounds.Add(s);
            }
            return Newtonsoft.Json.JsonConvert.SerializeObject(saveableSounds);
        }

        private static void SetCurrentStateJson(string json)
        {
            try
            {
                List<SaveableSoundRef> loadableSounds = Newtonsoft.Json.JsonConvert.DeserializeObject<List<SaveableSoundRef>>(json);
                foreach (var s in loadableSounds)
                {
                    if (File.Exists(s.FilePath))
                    {
                        var cd = MainWindow.AddSoundFile(s.FilePath);
                        if (s.IsLEDEnabled)
                        {
                            cd.LedColor = new SolidColorBrush(Color.FromRgb(s.colorR, s.colorG, s.colorB));
                            cd.EnableLED();
                        }
                    }
                }
            } catch (Exception) { }
        }
    }
    class SaveableSoundRef
    {
        public string FilePath;
        public byte colorR;
        public byte colorG;
        public byte colorB;
        public bool IsLEDEnabled;
    }
}
