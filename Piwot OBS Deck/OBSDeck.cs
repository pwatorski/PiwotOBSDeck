using OBSWebsocketDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PiwotOBSDeck
{
    public static class OBSDeck
    {
        public static OBSWebsocket client;
        static readonly string uriPrefix = "ws://";
        static OBSDeck()
        {
            client = new OBSWebsocket();
            
        }

        public static bool ConnectToOBS(string ip, string port, string password)
        {
            Task.Run(() =>
            {
                try
                {
                    client.ConnectAsync($"{uriPrefix}{ip}:{port}", password);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            });
            return client.IsConnected;
        }

    }
}
