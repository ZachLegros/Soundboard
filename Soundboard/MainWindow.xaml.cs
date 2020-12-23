using NAudio.CoreAudioApi;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace Soundboard
{
    class ViewModel
    {
        public ObservableCollection<PlaybackDevice> PlaybackDevices { get; set; }
        public ViewModel(string[] devices)
        {
            PlaybackDevices = new ObservableCollection<PlaybackDevice>();
            for (int i = 0; i < devices.Length; i++)
            {
                PlaybackDevice p = new PlaybackDevice
                {
                    DeviceId = i,
                    DeviceName = devices[i]
                };
                PlaybackDevices.Add(p);
            }
        }

        private int selectedPlaybackDeviceId;

        public int SelectedPlaybackDeviceId
        {
            get { return selectedPlaybackDeviceId; }
            set
            {
                selectedPlaybackDeviceId = value;
            }
        }


        private PlaybackDevice selectedPlaybackDevice;

        public PlaybackDevice SelectedPlaybackDevice
        {
            get { return selectedPlaybackDevice; }
            set
            {
                selectedPlaybackDevice = value;
            }
        }

    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MMDeviceEnumerator enumerator = new MMDeviceEnumerator();
            object[] devices = enumerator.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active).ToArray();
            string[] devicesNames = new string[devices.Length];

            for (int i=0; i< devicesNames.Length; i++)
            {
                devicesNames[i] = devices[i].ToString();
            }

            this.DataContext = new ViewModel(devicesNames);
    
        }
    }
}
