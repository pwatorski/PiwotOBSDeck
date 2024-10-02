using PiwotOBSDeck;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PiwotOBSDeck
{

    public class SettingsCarrier
    {
        public string StoragePath { get; set; }

        public string MainPath { get; set; }
        public int MaxDonationTextWidth { get; set; } = 32;
        public string? DonationSceneName { get; set; }
        public string? DonationTextItemName { get; set; }
        public string? GoalTextItemName { get; set; }
        public string? GoalSceneName { get; set; }
        public string? GoalBarItemName { get; set; }
        public double? GoalTargetValue { get; set; }
        public double? GoalProgressValue { get; set; }
        public float? DVDSpeed { get; set; }

        public bool MultilangEnabled { get; set; }

        public SettingsCarrier()
        {
            MainPath = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "PiwotOBSDeck");
            StoragePath = Path.Join(MainPath, "storage");
        }

    }
    public class Settings
    {
        public static string SettingsPath
        {
            get => Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "PiwotOBSDeck", "settings.json");
        }
        public static int MaxDonationTextWidth
        {
            get => instance.MaxDonationTextWidth;
            set
            {
                instance.MaxDonationTextWidth = value;
            }
        }
        public static string StoragePath
        {
            get => instance.StoragePath;
            set
            {
                instance.StoragePath = value;
                UpdateStoragePaths();
            }
        }
        public static double GoalTargetValue
        {
            get => instance.GoalTargetValue??1;
            set
            {
                instance.GoalTargetValue = value;
            }
        }
        public static double GoalProgressValue
        {
            get => instance.GoalProgressValue?? 1;
            set
            {
                instance.GoalProgressValue = value;
            }
        }

        public static bool MultilangEnabled
        {
            get => instance.MultilangEnabled;
            set
            {
                instance.MultilangEnabled = value;
            }
        }

        public static string MainPath
        {
            get => instance.MainPath;
            set
            {
                instance.MainPath = value;
                UpdateStoragePaths();
            }
        }

        public static string? DonationSceneName
        {
            get => instance.DonationSceneName;
            set
            {
                instance.DonationSceneName = value;
            }
        }
        public static string? DonationTextItemName
        {
            get => instance.DonationTextItemName;
            set
            {
                instance.DonationTextItemName = value;
            }
        }

        public static string? GoalTextItemName
        {
            get => instance.GoalTextItemName;
            set
            {
                instance.GoalTextItemName = value;
            }
        }
        public static string? GoalSceneName
        {
            get => instance.GoalSceneName;
            set
            {
                instance.GoalSceneName = value;
            }
        }
        public static string? GoalBarItemName
        {
            get => instance.GoalBarItemName;
            set
            {
                instance.GoalBarItemName = value;
            }
        }

        public static string StoragePath_Avatars { get; private set; }
        public static string StoragePath_TTS { get; private set; }
        public static float DVDSpeed
        {
            get => instance.DVDSpeed??1;
            set
            {
                instance.DVDSpeed = value;
            }
        }

        private static SettingsCarrier instance;

        static Settings()
        {
            Load();
            UpdateStoragePaths();
        }

        static void UpdateStoragePaths()
        {
            StoragePath_Avatars = Path.Combine(StoragePath, "avatars");
            StoragePath_TTS = Path.Combine(StoragePath, "tts");
        }

        public static void Save()
        {
            if (!Directory.Exists(SettingsPath))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(SettingsPath)??"");
            }
            using (StreamWriter sw = new(SettingsPath, false, encoding: Encoding.UTF8))
            {
                sw.Write(ToJson());
            }
        }
        public static SettingsCarrier Load()
        {
            if (!Path.Exists(SettingsPath))
            {
                instance = new SettingsCarrier();
            }
            else
            {
                instance = FromJson(SettingsPath);
            }
            return instance;
        }

        public static string ToJson()
        {
            return JsonSerializer.Serialize(instance, Misc.JsonOptions);
        }

        public static SettingsCarrier FromJson(string settingsPath)
        {
            string json = "";
            using (StreamReader sr = new StreamReader(settingsPath, encoding: Encoding.UTF8))
            {
                json = sr.ReadToEnd();
            }
            return JsonSerializer.Deserialize<SettingsCarrier>(json) ?? new SettingsCarrier();
        }
    }
}
