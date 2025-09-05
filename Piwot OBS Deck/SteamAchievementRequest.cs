using NAudio.Wave;
using PiwotOBS;
using PiwotOBS.Structure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace PiwotOBSDeck
{
    public  class SteamAchievementRequest
    {
        public string StoragePath { get; private set; }
        public string BackgroundPath { get; private set; }
        public string IconPath { get; private set; }

        public long ID { get; }
        public JsonNode? Meta { get; set; }
        public bool HasMeta { get => Meta != null; }
        public bool IsHandled { get; set; } = false;

        public SteamAchievementRequest(string storagePath, string backgroundPath, string iconPath, long id, JsonNode? meta = null)
        {
            StoragePath = storagePath;
            BackgroundPath = backgroundPath;
            IconPath = iconPath;
            ID = id;
            Meta = meta;

        }


        public static SteamAchievementRequest FromWebRequest(WebServices.WebRequests.SteamAchievementRequest source) 
        {
            return new SteamAchievementRequest(source.StoragePath, source.BackgroundPath, source.IconPath, source.ID);
        }


    }
}
