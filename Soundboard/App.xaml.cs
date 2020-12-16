using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using NAudio;
using NAudio.CoreAudioApi;

namespace Soundboard
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        { 
            MMDeviceEnumerator enumerator = new MMDeviceEnumerator();
            object[] devices = enumerator.EnumerateAudioEndPoints(DataFlow.All, DeviceState.Active).ToArray();

            foreach (object device in devices)
            {
                // need to check data bindings
                //MainWindow.comboPlaybackDevice.Items.Add(device);
            }
            Console.WriteLine("yop");
        }
    }
}
