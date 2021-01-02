using System;
using System.IO;
using System.Text.Json;

namespace Soundboard
{
    class SoundboardConfig
    {
        public string profile { set; get; }
        public string serialPort { set; get; }
        public string playbackDevice { set; get; }

        public static SoundboardConfig Load()
        {
            if (!File.Exists(Environment.CurrentDirectory + "\\SoundboardConfig.json"))
            {
                SoundboardConfig config = new SoundboardConfig();
                config.Save();
            }

            string jsonString = File.ReadAllText(Environment.CurrentDirectory + "\\SoundboardConfig.json");
            return JsonSerializer.Deserialize<SoundboardConfig>(jsonString);
        }


        public void Save()
        {
            string jsonString = JsonSerializer.Serialize(this);
            File.WriteAllText(Environment.CurrentDirectory + "\\SoundboardConfig.json", jsonString);
        }
    }
}
