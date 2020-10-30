using NAudio.Wave;
using ReverbEngine.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace ReverbEngine
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static WaveOutEvent outputDevice;
        public static AudioFileReader currentlyPlayingAudioFile;
        public static bool IsPlaying;
        public static DispatcherTimer timerVideoTime;
        public static int AudioDevice;

        public static List<ControlDial> Files = new List<ControlDial>();
        public static MiniHud MiniHud;
        public static WrapPanel SoundsListEx;
        public static bool StopSource_Unloaded = false;
        public static DialButton PlayPauseDialx;
        public static DoubleAnimation trackAnim;

        public MainWindow()
        {
            InitializeComponent();
            Title += " - v" + Configs.Version;
            outputDevice = new WaveOutEvent();
            MiniHud = HUD;
            SoundsListEx = SoundsList;
            PlayPauseDial.PlaySymbol.Opacity = 100;
            PlayPauseDial.PauseFunctionality = true;
            PlayPauseDialx = PlayPauseDial;
            string[] args = Environment.GetCommandLineArgs();
            if (args.Length > 1)
            {
                if (!string.IsNullOrWhiteSpace(args[1]))
                {
                    LibraryFileManager.LoadLibrary(args[1]);
                }
            }
            outputDevice.PlaybackStopped += (x, y) =>
            {
                if (StopSource_Unloaded)
                {
                    MiniHud.TrackStatus.Text = "Status: Unloaded";
                    StopSource_Unloaded = false;
                }
                else if (currentlyPlayingAudioFile != null)
                {
                    if (currentlyPlayingAudioFile.CurrentTime == currentlyPlayingAudioFile.TotalTime)
                    {
                        HUD.TrackStatus.Text = "Status: Stopped";
                        MainWindow.MiniHud.TrackProgress.BeginAnimation(ProgressBar.ValueProperty, null);
                        IsPlaying = false;
                        PlayPauseDial.VisualStatePause();
                    }
                }
                else
                {
                    HUD.TrackStatus.Text = "Status: Stopped";
                    MainWindow.MiniHud.TrackProgress.BeginAnimation(ProgressBar.ValueProperty, null);
                    IsPlaying = false;
                    PlayPauseDial.VisualStatePause();
                }
            };
            timerVideoTime = new DispatcherTimer();
            timerVideoTime.Interval = TimeSpan.FromMilliseconds(1000);
            timerVideoTime.Tick += (x,y) =>
            {
                if (IsPlaying) 
                {
                    var ts = currentlyPlayingAudioFile.CurrentTime;
                    //HUD.TrackCurrentPos.Text = (ty.TotalHours >= 1 ? $"{ty.Hours}:" : $"") + $"{ty.Minutes}:{ty.Seconds}";
                    HUD.TrackCurrentPos.Text = string.Format("{0:00}:{1:00}:{2:00}",
                   (int)(ts.TotalHours),
                   ts.Minutes,
                   ts.Seconds);
                }
            };
            timerVideoTime.Start();
        }

        public static List<WaveOutCapabilities> GetOutputDevices()
        {
            List<WaveOutCapabilities> outputDevices = new List<WaveOutCapabilities>();
            int numberOfOutputs = WaveOut.DeviceCount;
            for (int i = 0; i < numberOfOutputs; i++)
            {
                outputDevices.Add(WaveOut.GetCapabilities(i));
            }
            return outputDevices;

        }

        private void Window_Drop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach (var s in files)
            {
                if (System.IO.Path.GetExtension(s).ToLower() == ".relib")
                {
                    LibraryFileManager.LoadLibrary(s);
                }
                else
                {
                    try
                    {
                        AddSoundFile(s);
                    }
                    catch (Exception) { }
                }
            }
        }

        public static ControlDial AddSoundFile(string s)
        {
            var ad = new AudioFileReader(s);
            var cd = new ControlDial();
            cd.audioFile = ad;
            cd.Width = 690;
            cd.Height = 755;
            cd.LayoutTransform = new ScaleTransform(0.15, 0.15);
            Files.Add(cd);
            SoundsListEx.Children.Add(cd);
            return cd;
        }

        public static void RemoveSoundFile(ControlDial file)
        {
            Files.Remove(file);
            SoundsListEx.Children.Remove(file);
            if (currentlyPlayingAudioFile == file.audioFile)
            {
                outputDevice.Stop();
                currentlyPlayingAudioFile = null;
                IsPlaying = false;
                MiniHud.TrackCurrentPos.Text = "00:00";
                MiniHud.TrackProgress.BeginAnimation(ProgressBar.ValueProperty, null);
                MiniHud.TrackProgress.Value = 0;
                MainWindow.StopSource_Unloaded = true;
            }
        }

        private void DialButton_MouseUp(object sender, MouseButtonEventArgs e)
        {
            outputDevice.Stop();
            HUD.TrackStatus.Text = "Status: Stopped";
            HUD.TrackProgress.BeginAnimation(ProgressBar.ValueProperty, null);
            PlayPauseDial.VisualStatePause();
        }
    }
}
