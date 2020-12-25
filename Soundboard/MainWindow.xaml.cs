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
using System.Threading;

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

            // initialize serial port combobox items
            string[] ports = SerialPort.GetPortNames();

            foreach (string port in ports)
            {
                comboSerialPort.Items.Add(port);
            }

            // initialize playback device combobox items
            MMDeviceEnumerator enumerator = new MMDeviceEnumerator();
            object[] devices = enumerator.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active).ToArray();
            string[] devicesNames = new string[devices.Length];

            for (int i = 0; i < devicesNames.Length; i++)
            {
                devicesNames[i] = devices[i].ToString();
            }
            comboPlaybackDevice.DataContext = new ViewModel(devicesNames);

            // initialize serial communication with soundboard
            Thread serialCom = new Thread(new ThreadStart(SerialPortProgram));
            serialCom.Start();
        }

        private void SerialPortProgram()
        {
            while (true)
            {
                try
                {
                    port.DataReceived += new SerialDataReceivedEventHandler(port_DataReceived);

                    if (!port.IsOpen)
                    {
                        port.Open();
                    }

                    // Enter an application loop to keep this thread alive 
                    Console.ReadLine();
                }
                catch (System.IO.IOException e)
                {
                    if (port.IsOpen)
                    {
                        port.Close();
                    }
                }
                Thread.Sleep(500);
            }
        }

        private void port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string data = port.ReadExisting();
            string[] matrix = data.Split(',');
            int parsedData;

            for (int i=0; i<matrix.Length; i++)
            {
                string led = matrix[i];
                if (int.TryParse(led, out parsedData))
                {
                    int row = i / 4;
                    int col = i % 4;

                    string componentName = "R" + row + "C" + col;

                    this.Dispatcher.Invoke(() =>
                    {
                        Rectangle wantedNode = (Rectangle)buttonMatrix.FindName(componentName);
                        if (parsedData == 1)
                            wantedNode.Fill = new SolidColorBrush(Color.FromRgb(235, 64, 52));
                        else
                            wantedNode.Fill = null;
                    });
                }
                }
            }
    }
}

