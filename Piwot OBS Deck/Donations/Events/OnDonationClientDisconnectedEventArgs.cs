using System;

namespace PiwotOBSDeck.Donations.Events
{
    public class OnDonationClientDisconnectedEventArgs : EventArgs
    {
        public string ClientName { get; private set; }
        public OnDonationClientDisconnectedEventArgs(string clientName)
        {
            ClientName = clientName;
        }
    }
}
