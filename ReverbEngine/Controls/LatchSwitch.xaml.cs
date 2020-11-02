using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
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
    /// Interaction logic for LatchSwitch.xaml
    /// </summary>
    public partial class LatchSwitch : UserControl
    {
        public Storyboard LightOn;
        public Storyboard LightOff;
        public bool IsOn;

        public LatchSwitch()
        {
            InitializeComponent();
            LightOn = (Storyboard)FindResource("LightOn");
            LightOff = (Storyboard)FindResource("LightOff");
        }

        private void grid_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (IsOn)
            {
                TurnOff();
            }
            else
            {
                TurnOn();
            }
        }

        public void TurnOn()
        {
            LightOn.Begin();
            MainWindow.outputDevice.DeviceNumber = -1;
            IsOn = true;
            EnsurePlay();
            MainWindow.MiniHud.IsLive.Visibility = Visibility.Visible;
        }
        public void TurnOff()
        {
            LightOff.Begin();
            MainWindow.outputDevice.DeviceNumber = MainWindow.AudioDevice;
            IsOn = false;
            EnsurePlay();
            MainWindow.MiniHud.IsLive.Visibility = Visibility.Hidden;
        }
        public void EnsurePlay()
        {
            try
            {
                var isplaying = false;
                if (MainWindow.outputDevice.PlaybackState == NAudio.Wave.PlaybackState.Playing) isplaying = true;
                MainWindow.outputDevice.Stop();
                MainWindow.outputDevice.Init(MainWindow.currentlyPlayingAudioFile);
                if (isplaying) MainWindow.outputDevice.Play();
            } catch (Exception) { }
        }
    }
}
