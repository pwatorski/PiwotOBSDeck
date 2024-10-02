using System;

namespace PiwotOBSDeck.Donations
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