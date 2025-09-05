using System;
using PiwotOBSDeck.WebServices.WebRequests;

namespace PiwotOBSDeck.WebServices.Events
{
    public class OnSteamAchievementEventArgs : EventArgs
    {
        public string ClientName { get; private set; }
        public WebRequests.SteamAchievementRequest SteamAchievementRequest { get; private set; }
        public OnSteamAchievementEventArgs(string clientName, WebRequests.SteamAchievementRequest steamAchievementRequest)
        {
            ClientName = clientName;
            SteamAchievementRequest = steamAchievementRequest;
        }
    }
}
