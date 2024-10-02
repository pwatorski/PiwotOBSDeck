using System;

namespace PiwotOBSDeck.Donations.Events
{
    public class OnDonationClientConnectedEventArgs : EventArgs
    {
        public string ClientName { get; private set; }
        public OnDonationClientConnectedEventArgs(string clientName)
        {
            ClientName = clientName;
        }
    }
}
