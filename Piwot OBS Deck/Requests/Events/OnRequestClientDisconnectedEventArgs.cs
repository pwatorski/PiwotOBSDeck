using System;

namespace PiwotOBSDeck.WebServices.Events
{
    public class OnRequestClientDisconnectedEventArgs : EventArgs
    {
        public string ClientName { get; private set; }
        public OnRequestClientDisconnectedEventArgs(string clientName)
        {
            ClientName = clientName;
        }
    }
}
