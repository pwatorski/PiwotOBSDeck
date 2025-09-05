using System;

namespace PiwotOBSDeck.Requests.Events
{
    public class OnDonationShownEventArgs:EventArgs
    {
        public DonationRequest Request { get; set; }
        public OnDonationShownEventArgs(DonationRequest request)
        {
            Request = request;
        }
    }
}