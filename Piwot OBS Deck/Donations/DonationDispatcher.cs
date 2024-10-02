using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using PiwotOBS.Structure;

namespace PiwotOBSDeck.Donations
{
    public static class DonationDispatcher
    {
        public static ConcurrentQueue<DonationRequest> DonationRequests = new ConcurrentQueue<DonationRequest>();
        public static ConcurrentQueue<DonationRequest> InboudQueue = new ConcurrentQueue<DonationRequest>();
        public static EventHandler? OnDonationAdded;
        public static EventHandler<OnDonationShownEventArgs>? OnDonationShown;
        static TimeSpan BetweenTimeSpan = new TimeSpan(0, 0, 2);
        static Thread? DequeuingThread;
        static Thread? InboudThread;
        public static Scene? DonationScene;
        public static ItemText? DonationTextItem;
        public static Scene? GoalScene;
        public static ItemText? GoalTextItem;
        public static SceneItem? GoalBarItem;
        static bool doRun;

        public static bool Paused { get; set; }
        public static double GoalTarget { get; set; } = 1000;
        public static double GoalValue { get; set; } = 0;
        public static bool EnableMultiLang { get; set; }
        public static int EnqueuedDonations { get => DonationRequests.Count; }
        public static bool OBSDonationItemsSelected { get => DonationScene != null && DonationTextItem != null; }
        public static DonationRequest? CurrentRequestPlaying;

        static DonationDispatcher()
        {
            GoalValue = Settings.GoalProgressValue;
            GoalTarget = Settings.GoalTargetValue;
        }
        public static void AddDonation(DonationRequest request)
        {
            InboudQueue.Enqueue(request);

        }

        public static void Start()
        {
            if (doRun)
            {
                return;
            }
            InboudThread = new Thread(InboudLoop);
            DequeuingThread = new Thread(DequeuingLoop);

            doRun = true;

            InboudThread.Start();
            DequeuingThread.Start();
        }

        public static void Stop()
        {
            doRun = false;
            InboudThread?.Join();
            DequeuingThread?.Join();
        }

        static void InboudLoop()
        {
            while (doRun)
            {
                while (!InboudQueue.IsEmpty && InboudQueue.TryDequeue(out DonationRequest? request))
                {

                    if (request.Duration == null)
                    {
                        if (EnableMultiLang)
                        {
                            if (request.Lang == "auto")
                            {
                                switch (request.AuthorID)
                                {
                                    case 689528640655851616: //ramil
                                        request.Download("cs");
                                        break;
                                    default:
                                        request.Download("pl");
                                        break;
                                }
                            }
                            else
                            {
                                request.Download(request.Lang);
                            }
                        }
                        else
                        {
                            request.Download();
                        }

                        DonationRequests.Enqueue(request);
                        OnDonationAdded?.Invoke(null, new EventArgs());
                    }

                }
                Thread.Sleep(100);
            }
        }

        public static void RefreshGoal()
        {
            GoalTextItem?.SetText($"Goal: {GoalValue:0.00}/{GoalTarget:0.00} zł");
            GoalBarItem?.TransformObject(newScale: new PiwotOBS.PMath.Float2((float)(GoalValue / GoalTarget), 1));
            Settings.GoalProgressValue = GoalValue;
            Settings.GoalTargetValue = GoalTarget;
            Settings.Save();
        }

        static void DequeuingLoop()
        {
            while (doRun)
            {
                if (!Paused && OBSDonationItemsSelected && !DonationRequests.IsEmpty && DonationRequests.TryDequeue(out DonationRequest? request))
                {
                    CurrentRequestPlaying = request;
                    GoalValue += request.Value;

                    RefreshGoal();
                    SceneItem? vivi = null;
                    bool isVivi = false;
                    OnDonationShown?.Invoke(null, new OnDonationShownEventArgs(request));
                    DonationScene?.Enable();
                    DonationTextItem?.Enable();
                    
                    if(request.HasMeta)
                    {
                        isVivi = request.Meta?["isVivi"]?.GetValue<bool>()??false;
                    }
                    if(isVivi)
                    {
                        vivi = DonationScene?.FindItem("VIVI_DONATE");
                        vivi?.Enable();
                    }

                    DonationTextItem?.SetText(request.DisplayText);
                    var requestTime = request.PlaySoundAsync();
                    DateTime startTime = DateTime.Now;
                    while (DateTime.Now - startTime < requestTime && CurrentRequestPlaying != null)
                    {
                        Thread.Sleep(100);
                    }
                    Thread.Sleep(500);
                    DonationScene?.Disable();
                    vivi?.Disable();
                    Thread.Sleep(BetweenTimeSpan);
                    CurrentRequestPlaying = null;
                }
                else
                {
                    Thread.Sleep(100);
                }
            }
        }

        internal static void SkipDonation()
        {
            CurrentRequestPlaying?.Stop();
            DonationScene?.Disable();
            CurrentRequestPlaying = null;
        }

        internal static void SetGoalTarget(double value)
        {
            GoalTarget = value;
            RefreshGoal();
        }

        internal static void SetGoalValue(double value)
        {
            GoalValue = value;
            RefreshGoal();
        }


    }
}
