using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Globalization;
using System.Text.Json.Nodes;
using System.IO;
using System.Windows.Interop;
using PiwotOBSDeck.Donations.Events;
using PiwotOBSDeck.Donations.WebRequests;

namespace PiwotOBSDeck.Donations
{

    public class DonationRecieverService
    {
        protected Thread recievingThread;
        protected Thread connectingThread;
        protected TcpClient? client;
        protected NetworkStream? stream;
        protected TcpListener? server;
        protected bool StopRecievingThreadFlag;
        protected bool StopConnectingThreadFlag;
        public event EventHandler<OnDonationClientConnectedEventArgs> OnClientConnected;
        public event EventHandler<EventArgs> OnConnectionFail;
        public event EventHandler<OnDonationClientDisconnectedEventArgs> OnClientDisconnected;
        public event EventHandler<OnDonationEventArgs> OnDonation;
        public bool DebugMode { get; set; } = false;
        public double MaxPingInterval { get; set; } = 5;
        public DateTime? LastPing;
        private int recivTickLength = 100;
        private int RecivTimeout=5000;

        public bool IsDonationSourceConnected { get => LastPing != null && (DateTime.Now - LastPing).Value.TotalSeconds < MaxPingInterval; }
        public bool IsRunning { get; private set; }
        public int ConnectionTimeout { get; private set; } = 10000;
        public string? CurrentClientName { get; private set; }
        public bool ClientDidConnect { get; private set; }
        public DonationRecieverService()
        {
            
            StopRecievingThreadFlag = false;
            StopConnectingThreadFlag = false;
            IsRunning = false;
            LastPing = null;
            ClientDidConnect = false;
        }

        public void Start()
        {
            if (IsRunning)
                return;
            IsRunning = true;
            connectingThread = new Thread(ConnectingLoop);
            connectingThread.Start();
        }

        public void AbortConnecting()
        {
            StopConnectingThreadFlag = true;
            connectingThread.Join();
        }

        public void Stop()
        {
            StopRecievingThreadFlag = true;
            StopConnectingThreadFlag = true;
            connectingThread?.Join();
            recievingThread?.Join();
            EndRecieving();
        }

        //TODO move all requests to one loop
        protected void ConnectingLoop()
        {
            server = new TcpListener(IPAddress.Parse("127.0.0.1"), 8765);
            server.Start();
            TcpClient? connectingClient = null;
            int maxTries = ConnectionTimeout / 100;
            while (!StopConnectingThreadFlag && connectingClient == null && maxTries > 0)
            {
                Thread.Sleep(100);
                if (server.Pending())
                    connectingClient = server.AcceptTcpClient();
                
                maxTries--;
            }
            if (StopConnectingThreadFlag || connectingClient == null)
            {
                OnConnectionFail?.Invoke(this, new EventArgs());
                return;
            }
            client = connectingClient;
            stream = client.GetStream();
            recievingThread = new Thread(RecivingLoop);
            recievingThread.Start();
        }

        protected string RecieveMessageRaw()
        {
            if (stream == null || client == null)
                return string.Empty;
            byte[] bytes = new byte[client.Available];

            stream.Read(bytes, 0, bytes.Length);
            var msg = Encoding.UTF8.GetString(bytes);
            if(DebugMode) Console.WriteLine($"REC:\n{msg}");
            return msg;
        }

        protected JsonObject? RecieveMessageJson()
        {
            string message = RecieveMessageRaw();
            if (message.Length == 0)
                return null;
            return JsonNode.Parse(message)?.AsObject();
        }

        protected List<RequestBase> RecieveRequests()
        {
            string message = RecieveMessageRaw();
            if (message.Length == 0)
                return new List<RequestBase>();
            return RequestBase.Parse(message);
        }

        protected void EndRecieving()
        {

            stream?.Close();
            stream?.Dispose();
            client?.Close();
            client?.Dispose();
            server?.Stop();
            StopRecievingThreadFlag = false;
            StopConnectingThreadFlag = false;
            CurrentClientName = null;
            LastPing = null;
            IsRunning = false;
        }

        protected void RecivingLoop()
        {
            while (!StopRecievingThreadFlag)
            {
                int recivTimePassed = 0;
                while (!StopRecievingThreadFlag && stream != null && !stream.DataAvailable && recivTimePassed < RecivTimeout) {
                    Thread.Sleep(recivTickLength);
                    recivTimePassed += recivTickLength;
                }

                if (StopRecievingThreadFlag || recivTimePassed > RecivTimeout || !(client?.Connected ?? false) || !stream.DataAvailable)
                {
                    Console.WriteLine("Client disconnected. Closing connection.");
                    EndRecieving();
                    OnClientDisconnected?.Invoke(this, new OnDonationClientDisconnectedEventArgs(CurrentClientName??""));
                    return;
                }

                
                List<RequestBase> requests;
                try
                {
                     requests = RecieveRequests();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    requests = new List<RequestBase>();
                }

                if (requests.Count == 0)
                    continue;
                foreach (RequestBase request in requests)
                {
                    switch (request.Type)
                    {
                        case RequestBase.RequestType.Empty: 
                            break;
                        case RequestBase.RequestType.Greeting:
                            GreetingRequest? greetingRequest = (GreetingRequest)request;
                            Console.WriteLine($"Greeting from: {greetingRequest.Name} ({greetingRequest.PeerType})");
                            new GreetingRequest("PiwotOBSDeck", "donationReciever").Send(stream);
                            if(!IsDonationSourceConnected)
                            {
                                CurrentClientName = greetingRequest.Author;
                            }
                            break;
                        case RequestBase.RequestType.Status: 
                            StatusRequest? statusRequest = (StatusRequest)request;
                            Console.WriteLine($"Status from [{statusRequest.Author}]: {statusRequest.Status}");
                            
                            if (CurrentClientName == statusRequest.Author)
                            {
                                if (statusRequest.Status == "ready")
                                {
                                    new StatusRequest("ready").Send(stream);
                                    LastPing = DateTime.Now;
                                    OnClientConnected?.Invoke(this, new OnDonationClientConnectedEventArgs(CurrentClientName));
                                }
                                else
                                {
                                    new StatusRequest("closing").Send(stream);
                                    EndRecieving();
                                    OnConnectionFail?.Invoke(this, new EventArgs());
                                    return;
                                }
                            }
                            else
                            {
                                new StatusRequest("busy").Send(stream);
                            }
                            break;
                        case RequestBase.RequestType.Ping:
                            PingRequest? pingRequest = (PingRequest) request;
                            if(pingRequest.Author != CurrentClientName)
                            {
                                break;
                            }
                            LastPing = DateTime.Now;
                            try
                            {
                                new PongRequest().Send(stream);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.ToString());
                                LastPing = null;
                            }
                            break;
                        case RequestBase.RequestType.Pong:
                            break;
                        case RequestBase.RequestType.Donation:
                            WebRequests.DonationRequest dr = (WebRequests.DonationRequest)request;

                            Console.WriteLine($"{dr.AuthorName}:\n{dr.Text}");
                            OnDonation?.Invoke(this, new OnDonationEventArgs(CurrentClientName??"", dr));
                            break;

                        default: break;
                    }
                }
            }
        }
    }
}
