using System;
using PiwotOBSDeck.Donations.WebRequests;

namespace PiwotOBSDeck.Donations.Events
{
    public class OnDonationEventArgs : EventArgs
    {
        public string ClientName { get; private set; }
        public WebRequests.DonationRequest DonationRequest { get; private set; }
        public OnDonationEventArgs(string clientName, WebRequests.DonationRequest donationRequest)
        {
            ClientName = clientName;
            DonationRequest = donationRequest;
        }
    }
}
