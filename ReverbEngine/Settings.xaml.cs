using Microsoft.Win32;
using NAudio.CoreAudioApi;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Shapes;

namespace ReverbEngine
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        public Settings()
        {
            InitializeComponent();
            List<WaveOutCapabilities> devices = MainWindow.GetOutputDevices();
            List<string> devicesNames = new List<string>();
            foreach (var d in devices)
            {
                devicesNames.Add(d.ProductName);
            }
            AudioDevices.ItemsSource = devicesNames;
        }

        private void AudioDevices_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MainWindow.overrideSwitch.TurnOff();
            MainWindow.outputDevice.DeviceNumber = AudioDevices.SelectedIndex;
            MainWindow.AudioDevice = AudioDevices.SelectedIndex;
            MainWindow.outputDevice.Stop();
            MainWindow.outputDevice.Init(MainWindow.currentlyPlayingAudioFile);
            MainWindow.outputDevice.Play();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Import
            OpenFileDialog o = new OpenFileDialog();
            o.Filter = "MicroART Reverb Engine Library (*.relib)|*.relib";
            if (o.ShowDialog() == true)
            {
                foreach (var f in o.FileNames)
                {
                    LibraryFileManager.LoadLibrary(f);
                }
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            // Export
            LibraryFileManager.SaveLibrary();
        }
    }
}
