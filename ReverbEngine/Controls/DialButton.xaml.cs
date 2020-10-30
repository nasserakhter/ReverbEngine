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
    /// Interaction logic for DialButton.xaml
    /// </summary>
    public partial class DialButton : UserControl
    {
        public Storyboard PressIn;
        public Storyboard PressOut;
        public Storyboard PlayToPause;
        public Storyboard PauseToPlay;
        public bool IsPressed;
        public bool IsPaused;

        public bool PauseFunctionality = false;

        public DialButton()
        {
            InitializeComponent();
            PressIn = (Storyboard)FindResource("PressIn");
            PressOut = (Storyboard)FindResource("PressOut");
            PlayToPause = (Storyboard)FindResource("PlayToPause");
            PauseToPlay = (Storyboard)FindResource("PauseToPlay");
        }

        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            PressIn.Begin();
            IsPressed = true;
        }

        private void Canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (IsPressed)
            {
                PressOut.Begin();
                IsPressed = false;
                if (PauseFunctionality)
                {
                    if (IsPaused)
                    {
                        // Media was paused so being play
                        try
                        {
                            MainWindow.outputDevice.Play();
                            var clock = MainWindow.trackAnim.CreateClock();
                            MainWindow.MiniHud.TrackProgress.ApplyAnimationClock(ProgressBar.ValueProperty, clock);
                            clock.Controller.Begin();
                            clock.Controller.Resume();
                            MainWindow.MiniHud.TrackStatus.Text = "Status: Playing";
                            PlayToPause.Begin();
                            IsPaused = false;
                        }
                        catch (Exception) { }
                    }
                    else
                    {
                        // Media was playing so pause it
                        try
                        {
                            MainWindow.outputDevice.Pause();
                            var clock = MainWindow.trackAnim.CreateClock();
                            MainWindow.MiniHud.TrackProgress.ApplyAnimationClock(ProgressBar.ValueProperty, clock);
                            clock.Controller.Pause();
                            MainWindow.MiniHud.TrackStatus.Text = "Status: Paused";
                            PauseToPlay.Begin();
                            IsPaused = true;
                        }
                        catch (Exception) { }
                    }
                }
            }
        }

        public void VisualStatePause()
        {
            PauseToPlay.Begin();
            IsPaused = true;
        }
        public void VisualStatePlay()
        {
            PlayToPause.Begin();
            IsPaused = false;
        }
    }
}
