using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
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
    /// Interaction logic for MiniHud.xaml
    /// </summary>
    public partial class MiniHud : UserControl
    {

        public MiniHud()
        {
            InitializeComponent();
            TrackProgress.IsEnabled = false;
            TrackProgress.PreviewMouseUp += (x, y) =>
            {
                if (MainWindow.currentlyPlayingAudioFile == null) return;
                var d = new DoubleAnimation()
                {
                    To = MainWindow.currentlyPlayingAudioFile.TotalTime.TotalSeconds,
                    From = TrackProgress.Value,
                    Duration = new Duration(TimeSpan.FromSeconds(MainWindow.currentlyPlayingAudioFile.TotalTime.TotalSeconds - TrackProgress.Value))
                };
                TrackProgress.BeginAnimation(ProgressBar.ValueProperty, d);
                MainWindow.PlayPauseDialx.VisualStatePlay();
                MainWindow.MiniHud.TrackStatus.Text = "Status: Playing";
                MainWindow.trackAnim = d;
                try
                {
                    MainWindow.outputDevice.Play();
                } catch (Exception) { }
            };

            TrackProgress.PreviewMouseMove += (sender, args) =>
            {
                if (MainWindow.currentlyPlayingAudioFile == null) return;
                if (args.LeftButton == MouseButtonState.Pressed)
                {
                    TrackProgress.RaiseEvent(new MouseButtonEventArgs(args.MouseDevice, args.Timestamp, MouseButton.Left)
                    {
                        RoutedEvent = UIElement.PreviewMouseLeftButtonDownEvent,
                        Source = args.Source
                    });
                    TrackProgress.BeginAnimation(ProgressBar.ValueProperty, null);
                    var ts = TimeSpan.FromSeconds(TrackProgress.Value);
                    MainWindow.currentlyPlayingAudioFile.CurrentTime = ts;
                    MainWindow.MiniHud.TrackCurrentPos.Text = string.Format("{0:00}:{1:00}:{2:00}",
                   (int)(ts.TotalHours),
                   ts.Minutes,
                   ts.Seconds);
                    try
                    {
                        MainWindow.outputDevice.Play();
                    } catch (Exception) { }
                }
            };
        }

        public string Trackname
        {
            get => TrackName.Text;
            set => TrackName.Text = value;
        }

        private void UserControl_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Settings s = new Settings();
            s.ShowDialog();
        }

        private void TrackPositionSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
        }

        private void TrackProgress_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
        }
    }
}
