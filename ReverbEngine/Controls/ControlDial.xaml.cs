using NAudio.Wave;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ReverbEngine.Controls
{
    /// <summary>
    /// Interaction logic for ControlDial.xaml
    /// </summary>
    public partial class ControlDial : UserControl
    {
        public Storyboard PressIn;
        public Storyboard PressOut;
        public Storyboard LedIn;
        public Storyboard LedOut;
        public Storyboard BlurIn;
        public Storyboard BlurOut;
        public bool IsPressed;
        public bool IsLEDEnabled;

        public AudioFileReader audioFile;

        public Brush LedColor
        {
            get => LED.Fill;
            set => LED.Fill = value;
        }

        public List<Brush> StandardLEDColors = new List<Brush>()
        {
            new SolidColorBrush(Color.FromRgb(255,0,0)),
            new SolidColorBrush(Color.FromRgb(242,132,15)),
            new SolidColorBrush(Color.FromRgb(255,201,15)),
            new SolidColorBrush(Color.FromRgb(174, 242, 15)),
            new SolidColorBrush(Color.FromRgb(53, 242, 15)),
            new SolidColorBrush(Color.FromRgb(15, 242, 223)),
            new SolidColorBrush(Color.FromRgb(15, 132, 242)),
            new SolidColorBrush(Color.FromRgb(144, 15, 242)),
            new SolidColorBrush(Color.FromRgb(242, 15, 189)),
            new SolidColorBrush(Color.FromRgb(242, 15, 98))
        };

        public ControlDial()
        {
            InitializeComponent();
            PressIn = (Storyboard)FindResource("PressIn");
            PressOut = (Storyboard)FindResource("PressOut");
            LedIn = (Storyboard)FindResource("LedIn");
            LedOut = (Storyboard)FindResource("LedOut");
            BlurIn = (Storyboard)FindResource("BlurIn");
            BlurOut = (Storyboard)FindResource("BlurOut");
        }

        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if ((Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)) && e.RightButton == MouseButtonState.Pressed)
            {
                if (IsPressed)
                {
                    PressOut.Begin();
                    IsPressed = false;
                }
                var d = new DoubleAnimation()
                {
                    To = 0,
                    Duration = new Duration(TimeSpan.FromMilliseconds(350)),
                    EasingFunction = new QuinticEase()
                };
                BlurOut.Completed += (x, y) =>
                {
                    d.Completed += (a, b) =>
                    {
                        Visibility = Visibility.Collapsed;
                        MainWindow.RemoveSoundFile(this);
                        audioFile.Dispose();
                    };
                    this.BeginAnimation(WidthProperty, d);
                };
                BlurOut.Begin();
            }
            else if (e.RightButton == MouseButtonState.Pressed)
            {
                if (LED.Fill != null)
                {
                    int i = StandardLEDColors.IndexOf(LED.Fill);
                    if (i < 0) i = 0;
                    if (StandardLEDColors.Count <= i + 1) i = 0;
                    else
                        i = i + 1;
                    LED.Fill = StandardLEDColors[i];
                    if (!IsLEDEnabled) _enableLED();
                    LedIn.Begin();
                }
            }
            else if (e.MiddleButton == MouseButtonState.Pressed)
            {
                _disableLED();
            }
            else
            {
                PressIn.Begin();
                IsPressed = true;
            }
        }

        private void Canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (IsPressed)
                {
                    if (MainWindow.MiniHud.TrackProgress.IsEnabled == false)
                    {
                        MainWindow.MiniHud.TrackProgress.IsEnabled = true;
                    }
                    audioFile.Position = 0;
                    MainWindow.outputDevice.Stop();
                    // if (MainWindow.currentlyPlayingAudioFile != null) MainWindow.currentlyPlayingAudioFile.Position = 0;
                    MainWindow.outputDevice.Init(audioFile);
                    MainWindow.outputDevice.Play();
                    MainWindow.MiniHud.Trackname = System.IO.Path.GetFileNameWithoutExtension(audioFile.FileName);
                    MainWindow.MiniHud.TrackStatus.Text = "Status: Playing";
                    MainWindow.PlayPauseDialx.VisualStatePlay();
                    var ts = audioFile.TotalTime;
                    //MainWindow.MiniHud.TrackTotalPos.Text = (audioFile.TotalTime.TotalHours >= 1 ? $"{audioFile.TotalTime.Hours}:" : $"") + $"{audioFile.TotalTime.Minutes}:{audioFile.TotalTime.Seconds}";
                    MainWindow.MiniHud.TrackTotalPos.Text = string.Format("{0:00}:{1:00}:{2:00}",
                       (int)(ts.TotalHours),
                       ts.Minutes,
                       ts.Seconds);
                    MainWindow.MiniHud.TrackCurrentPos.Text = "00:00:00";
                    MainWindow.IsPlaying = true;
                    MainWindow.currentlyPlayingAudioFile = audioFile;
                    MainWindow.MiniHud.TrackProgress.Maximum = audioFile.TotalTime.TotalSeconds;
                    var t = new DoubleAnimation()
                    {
                        To = audioFile.TotalTime.TotalSeconds,
                        Duration = new Duration(TimeSpan.FromSeconds(audioFile.TotalTime.TotalSeconds))
                    };
                    MainWindow.MiniHud.TrackProgress.BeginAnimation(ProgressBar.ValueProperty, null);
                    MainWindow.MiniHud.TrackProgress.Value = 0;
                    MainWindow.MiniHud.TrackProgress.BeginAnimation(ProgressBar.ValueProperty, t);
                    MainWindow.trackAnim = t;
                    PressOut.Begin();
                    IsPressed = false;
                }
            } catch (Exception)
            {
                if (IsPressed)
                {
                    PressOut.Begin();
                    IsPressed = false;
                }
                var d = new DoubleAnimation()
                {
                    To = 0,
                    Duration = new Duration(TimeSpan.FromMilliseconds(350)),
                    EasingFunction = new QuinticEase()
                };
                BlurOut.Completed += (x, y) =>
                {
                    d.Completed += (a, b) =>
                    {
                        Visibility = Visibility.Collapsed;
                        MainWindow.RemoveSoundFile(this);
                        audioFile.Dispose();
                    };
                    this.BeginAnimation(WidthProperty, d);
                };
                BlurOut.Begin();
            }
        }

        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            // Release Focus Because User Left Button Region
            if (IsPressed)
            {
                PressOut.Begin();
                IsPressed = false;
            }
        }

        public void EnableLED()
        {
            _enableLED();
        }

        public void DisableLED()
        {
            _disableLED();
        }

        private void _enableLED()
        {
            if (!IsLEDEnabled)
            {
                LedIn.Begin();
                IsLEDEnabled = true;
            }
        }
        private void _disableLED()
        {
            if (IsLEDEnabled)
            {
                LedOut.Begin();
                IsLEDEnabled = false;
            }
        }
    }
}
