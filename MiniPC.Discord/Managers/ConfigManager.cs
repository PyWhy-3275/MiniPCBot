using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Newtonsoft.Json;
using MiniPC.Discord.Helpers;

namespace MiniPC.Discord.Managers
{
    public static class ConfigManager
    {
        private static readonly string Folder = "Config";
        private static readonly string File = "config.json"; // Copy if newer
        private static readonly string Path = Folder + "/" + File;
        
        private static BotConfig Config { get; }
        
        public static string Token => Config.Token;
        public static string Prefix => Config.Prefix ?? "!";

        public static string Activity => Config.Activity;
        
        public static void Exists()
        {
            Log.Warning(!Directory.Exists(Folder) ? "Config Directory Doesnt exist!" : "Config Directory exists!");
        }
        
        private static void OpenConfigDir() // ToDo: Create Explorer helper?
        {
            var workingDirectory = Environment.CurrentDirectory;

            var directoryInfo = Directory.GetParent(workingDirectory)!.Parent;
            if (directoryInfo == null) return;
            var configDirectory = directoryInfo.Parent!.FullName + "\\Config";

            if (!Directory.Exists(configDirectory)) return;
            Log.Info("Opening Explorer with config!");
            var startInfo = new ProcessStartInfo
            {
                Arguments = configDirectory,
                FileName = "explorer.exe"
            };
            Process.Start(startInfo);
        }
        
        public static bool GetDms() => Config.Dms;
        
        static ConfigManager()
        {
            if (!Directory.Exists(Folder))
            {
                Log.Warning("Config Directory does not exist ... Creating new one!");
                Directory.CreateDirectory(Folder);
            }

            if (!System.IO.File.Exists(Path))
            {
                Log.Warning($"{Path} does not exist ... Creating new one!");

                Config = new BotConfig() {Token = "TOKEN HERE!!!"}; // Create default values here
                var json = JsonConvert.SerializeObject(Config, Formatting.Indented);
                System.IO.File.WriteAllText(Path, json);
            }
            else
            {
                var json = System.IO.File.ReadAllText(Path);

                Config = JsonConvert.DeserializeObject<BotConfig>(json);

                if (Config.Token is not "TOKEN HERE!!!") return;
                Log.Error("DATA IN CONFIG IS NOT SPECIFIED! ... ABORTING OPERATION!!!");

                Log.Warning($"\"Token\": \"TOKEN-HERE\" in {Path}");
                // Open config directory
                OpenConfigDir();
                // Turn off application
                Thread.Sleep(1000);
                Environment.Exit(0); // 0 to indicate that process completed successfully
            }
        }
    }
    
    [Serializable]
    public struct BotConfig
    {
        [JsonProperty("Token")]
        public string Token; // Token-Here
        [JsonProperty("Prefix")]
        public string Prefix;
        [JsonProperty("Dms")]
        public bool Dms;
        [JsonProperty("Activity")]
        public string Activity;
        [JsonProperty("ActivityType")]
        public string ActivityType;
    }
}