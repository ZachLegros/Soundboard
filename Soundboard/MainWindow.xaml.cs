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
using Microsoft.Win32;

namespace Soundboard
{

    // Dynamic playback devices combobox model
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
        private SerialPort port;
        private SoundProfile Profile;
        private SoundboardConfig Config;
        private ViewModel model;

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
            string[] deviceNames = GetPlaybackDevices();
            model = new ViewModel(deviceNames);
            comboPlaybackDevice.DataContext = model;

            // loading preferences
            LoadConfig();
            LoadProfile(Config.profile);

            if (Profile == null)
            {
                NewProfile(this, null);
                SetProfileAsDefault(Profile.GetName());
            }

            InitializeUI();

            // initialize serial communication with soundboard
            Thread serialCom = new Thread(new ThreadStart(SerialPortProgram));
            serialCom.Start();
        }

        private void LoadConfig()
        {
            Config = SoundboardConfig.Load();
        }

        private void LoadProfile(string profileName)
        {
            if (profileName != null && !profileName.Equals(""))
            {
                try
                {
                    Profile = SoundProfile.FromFile(profileName);
                }
                catch
                {

                }
            }
        }

        private void InitializeUI()
        {
            // set profile label to default profile
            labelCurrentProfile.Content = Profile.GetName();

            if (Config.serialPort != null)
            {
                string[] ports = SerialPort.GetPortNames();
                for (int i = 0; i < ports.Length; i++)
                {
                    if (Config.serialPort.Equals(ports[i]))
                    {
                        comboSerialPort.SelectedIndex = i;
                    }
                }
            }

            if (Config.playbackDevice != null)
            {
                string[] deviceNames = GetPlaybackDevices();
                for (int i = 0; i < deviceNames.Length; i++)
                {
                    if (Config.playbackDevice.Equals(deviceNames[i]))
                    {
                        model.SelectedPlaybackDevice = model.PlaybackDevices[i];
                        model.SelectedPlaybackDeviceId = i;
                    }
                }

            }
        }

        private void SerialPortProgram()
        {
            while (true)
            {
                if (port != null)
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
                    catch
                    {
                        if (port.IsOpen)
                        {
                            port.Close();
                        }
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

            if (matrix.Length == 16)
            {
                for (int i=0; i<matrix.Length; i++)
                {
                    string led = matrix[i];
                    if (int.TryParse(led, out parsedData))
                    {
                        int row = i / 4;
                        int col = i % 4;

                        string componentName = "R" + row + "C" + col;

                        Dispatcher.Invoke(() =>
                        {
                            Rectangle wantedNode = FindName(componentName) as Rectangle;
                            if (parsedData == 1)
                                wantedNode.Fill = new SolidColorBrush(Color.FromRgb(235, 64, 52));
                            else
                                wantedNode.Fill = null;
                        });
                    }
                }
            }
        }

        public string[] GetPlaybackDevices()
        {
            MMDeviceEnumerator enumerator = new MMDeviceEnumerator();
            object[] devices = enumerator.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active).ToArray();
            string[] devicesNames = new string[devices.Length];

            for (int i = 0; i < devicesNames.Length; i++)
            {
                devicesNames[i] = devices[i].ToString();
            }

            return devicesNames;
        }

        public void btnSetProfileAsDefault_Click(object sender, RoutedEventArgs e)
        {
            SetProfileAsDefault(Profile.GetName());
        }

        public void SetProfileAsDefault(string profileName)
        {
            if (Config.profile != profileName)
            {
                Config.profile = profileName;
                Config.Save();
            }
        }

        public void NewProfile(object sender, RoutedEventArgs e)
        {
            string inputRead = new InputBox("New sound Profile", "Enter the sound Profile name:").ShowDialogAndGetText();

            if (inputRead != null && !inputRead.Equals(""))
            {
                Profile = new SoundProfile(inputRead, null);
                Profile.Save();
            }
        }

        public void AddSound(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Title = "Select sound file";
            openFileDialog1.DefaultExt = "mp3";
            openFileDialog1.Filter = "mp3 file (*.mp3)|*.mp3|wav file (*.wav)|*.wav";

            if (openFileDialog1.ShowDialog() == true)
            {
                string soundPath = openFileDialog1.FileName;
                string[] splittedPath = soundPath.Split('\\');
                string soundName = splittedPath[splittedPath.Length - 1];

                System.IO.File.Copy(soundPath, Environment.CurrentDirectory + "\\Sounds\\" + soundName, true);

                MessageBox.Show(soundName + " was successfully added to the library.", "BibBap Soundboard", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cmb = sender as ComboBox;
            if (cmb.SelectedItem != null)
            {
                switch (cmb.Name)
                {
                    case "comboPlaybackDevice":
                        Config.playbackDevice = ((PlaybackDevice)cmb.SelectedItem).DeviceName;
                        break;
                    case "comboSerialPort":
                        string portName = cmb.SelectedItem.ToString();
                        Config.serialPort = portName;
                        if (port != null && port.IsOpen)
                            port.Close();
                        port = new SerialPort(portName, 9600, Parity.None, 8, StopBits.One);
                        break;
                    default:
                        break;
                }

                Config.Save();
            }
        }
    }
}

