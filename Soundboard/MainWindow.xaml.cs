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
using System.IO.Ports;

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
        private SerialPort port = new SerialPort("COM3", 9600, Parity.None, 8, StopBits.One);

        public MainWindow()
        {
            InitializeComponent();

            string[] ports = SerialPort.GetPortNames();

            foreach (string port in ports)
            {
                comboSerialPort.Items.Add(port);
            }

            MMDeviceEnumerator enumerator = new MMDeviceEnumerator();
            object[] devices = enumerator.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active).ToArray();
            string[] devicesNames = new string[devices.Length];

            for (int i = 0; i < devicesNames.Length; i++)
            {
                devicesNames[i] = devices[i].ToString();
            }

            comboPlaybackDevice.DataContext = new ViewModel(devicesNames);
            SerialPortProgram();
        }

        private void HandleButtonPress(string componentName)
        {
            Rectangle wantedNode = (Rectangle)buttonMatrix.FindName(componentName);
            wantedNode.Fill = new SolidColorBrush(Color.FromRgb(235, 64, 52));
        }

        [STAThread]
        private void SerialPortProgram()
        {
            port.DataReceived += new SerialDataReceivedEventHandler(port_DataReceived);
            port.Open();

            // Enter an application loop to keep this thread alive 
            Console.ReadLine();
        }

        private void port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            //Console.WriteLine(port.ReadExisting());
            int data = int.Parse(port.ReadExisting());
            int row = data / 4;
            int col = data % 4;

            string componentName = "R" + row + "C" + col;

            this.Dispatcher.Invoke(() =>
            {
                HandleButtonPress(componentName);
            });
        }
    }
}

