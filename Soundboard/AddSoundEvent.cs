using Microsoft.Win32;
using System;
using System.Windows;

public class AddSoundEvent
{
	public static void AddSound(object sender, RoutedEventArgs e)
	{
		OpenFileDialog openFileDialog1 = new OpenFileDialog();
		openFileDialog1.Title = "Select sound file";
		openFileDialog1.DefaultExt = "mp3";
		openFileDialog1.Filter = "mp3 file (*.mp3)|*.mp3|wav file (*.wav)|*.wav";

		if (openFileDialog1.ShowDialog() == true)
		{
			string soundPath = openFileDialog1.FileName;
			string[] splittedPath = soundPath.Split('\\');
			string soundName = splittedPath[splittedPath.Length-1];

            System.IO.File.Copy(soundPath, Environment.CurrentDirectory + "\\Sounds\\" + soundName, true);

            MessageBox.Show(soundName + " was successfully added to the library.", "BibBap Soundboard", MessageBoxButton.OK, MessageBoxImage.Information);
		}
	}
}
