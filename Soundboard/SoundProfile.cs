using System;
using System.IO;
using NAudio.Wave;

namespace Soundboard
{
    class SoundProfile
    {
        private string Name;
        private AudioFileReader[] AudioFiles;

        private class Items
        {
            public string Name;
            public string[] SoundNames;
        }

        public static SoundProfile FromFile(string fileName)
        {
            string jsonString = File.ReadAllText(fileName);
            Items items = System.Text.Json.JsonSerializer.Deserialize<Items>(jsonString);

            SoundProfile soundProfile = new SoundProfile(items.Name);
            soundProfile.LoadSoundMatrix(items.SoundNames);

            return soundProfile;
        }

        public SoundProfile(string profileName) 
        {
            Name = profileName;
            AudioFiles = new AudioFileReader[16];
        }
            
        public void LoadSoundMatrix(string[] soundNames)
        {
            if (soundNames.Length != 16)
            {
                for (int i=0; i<16; i++)
                {
                    if (!soundNames[i].Equals("") && soundNames[i] != null)
                    {
                        try
                        {
                            AudioFiles[i] = new AudioFileReader(soundNames[i]);
                        }
                        catch
                        {
                        
                        }
                    }
                }
            }
            else
            {
                throw new Exception("Invalid array size. Array size must be 16.");
            }
        }

        public AudioFileReader GetSound(int index) 
        {
            return AudioFiles[index];
        }

        public void SetSound(string soundPath, int index)
        {
            try 
            {
                AudioFiles[index] = new AudioFileReader(soundPath);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void RemoveSound(int index)
        {
            try
            {
                AudioFiles[index] = null;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
