using System;

namespace PiwotOBSDeck.Requests.Events
{
    public class OnSteamAchievementShownEventArgs : EventArgs
    {
        public SteamAchievementRequest Request { get; set; }
        public OnSteamAchievementShownEventArgs(SteamAchievementRequest request)
        {
            Request = request;
        }
    }
}