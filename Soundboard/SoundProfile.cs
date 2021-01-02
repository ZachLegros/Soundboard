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
        private string[] SoundNames;

        private class Items
        {
            public string name;
            public string[] soundNames;
        }

        public static SoundProfile FromFile(string fileName)
        {
            string jsonString = File.ReadAllText(fileName);
            Items items = System.Text.Json.JsonSerializer.Deserialize<Items>(jsonString);

            SoundProfile soundProfile = new SoundProfile(items.name, items.soundNames);
            soundProfile.LoadSoundMatrix(items.soundNames);

            return soundProfile;
        }

        public SoundProfile(string profileName, string[] SoundNames) 
        {
            Name = profileName;
            
            if (SoundNames == null || SoundNames.Length != 16)
            {
                this.SoundNames = new string[16];
            }
            else
            {
                this.SoundNames = SoundNames;
            }

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

        public void Save()
        {
            Items items = new Items();
            items.name = Name;
            items.soundNames = SoundNames;

            string jsonString = JsonSerializer.Serialize(items);
            File.WriteAllText(Environment.CurrentDirectory + "\\SoundProfiles\\" + items.name + ".json", jsonString);
        }        
    }
}
