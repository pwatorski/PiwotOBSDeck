using System;

namespace PiwotOBSDeck.WebServices.Events
{
    public class OnRequestClientConnectedEventArgs : EventArgs
    {
        public string ClientName { get; private set; }
        public OnRequestClientConnectedEventArgs(string clientName)
        {
            ClientName = clientName;
        }
    }
}
