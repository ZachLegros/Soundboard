using System;
using System.IO;
using System.Text.Json;
using NAudio.Wave;

namespace Soundboard
{
    class SoundProfile
    {
        private string Name;
        private AudioFileReader[] AudioFiles;
        private string[] soundNames;
        public readonly static string SoundProfilePathRoot = Environment.CurrentDirectory + "\\SoundProfiles\\";

        private class Items
        {
            public string name { get; set; }
            public string[] soundNames { get; set; }
        }

        public static SoundProfile FromFile(string profileName)
        {
            string jsonString = File.ReadAllText(SoundProfilePathRoot + profileName + ".json");
            Items items = JsonSerializer.Deserialize<Items>(jsonString);

            SoundProfile soundProfile = new SoundProfile(items.name, items.soundNames);
            soundProfile.LoadSoundMatrix(items.soundNames);

            return soundProfile;
        }

        public SoundProfile(string profileName, string[] soundNames) 
        {
            Name = profileName;
            
            if (soundNames == null || soundNames.Length != 16)
            {
                this.soundNames = new string[16];
            }
            else
            {
                this.soundNames = soundNames;
            }

            AudioFiles = new AudioFileReader[16];
        }
            
        public void LoadSoundMatrix(string[] soundNames)
        {
            if (soundNames.Length == 16)
            {
                for (int i=0; i<16; i++)
                {
                    if (soundNames[i] != null && !soundNames[i].Equals(""))
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

        public string GetName()
        {
            return Name;
        }

        public void Save()
        {
            Items items = new Items();
            items.name = Name;
            items.soundNames = soundNames;

            string jsonString = JsonSerializer.Serialize(items);
            File.WriteAllText(SoundProfilePathRoot + items.name + ".json", jsonString);
        }        
    }
}
