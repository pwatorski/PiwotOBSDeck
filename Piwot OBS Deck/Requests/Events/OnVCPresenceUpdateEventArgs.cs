using PiwotOBSDeck.WebServices.WebRequests;
using System;

namespace PiwotOBSDeck.Requests.Events
{
    public class OnVCPresenceUpdateEventArgs:EventArgs
    {
        public VCPresenceUpdateRequest Request { get; set; }
        public OnVCPresenceUpdateEventArgs(VCPresenceUpdateRequest request)
        {
            Request = request;
        }
    }
}