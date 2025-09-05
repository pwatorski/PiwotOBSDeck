using NAudio.Wave;
using PiwotOBS.PMath;
using PiwotOBS.Structure;
using PiwotOBS.Structure.Animations;
using PiwotOBSDeck.Requests.Events;
using PiwotOBSDeck.WebServices.Events;
using PiwotOBSDeck.WebServices.WebRequests;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PiwotOBSDeck.Requests
{
    internal class SteamAchievementDispatcher
    {
        public static ConcurrentQueue<SteamAchievementRequest> SteamAchievementRequests = new ConcurrentQueue<SteamAchievementRequest>();
        public static ConcurrentQueue<SteamAchievementRequest> InboudQueue = new ConcurrentQueue<SteamAchievementRequest>();
        public static EventHandler? OnDonationAdded;
        static TimeSpan BetweenTimeSpan = new TimeSpan(0, 0, 2);
        static Thread? DequeuingThread;
        static Thread? InboudThread;
        public static Scene? AchievementsScene;
        public static ItemImage? SteamAchievementIconItem;
        public static ItemImage? SteamAchievementBackgroundItem;
        static bool doRun;
        public static EventHandler<OnSteamAchievementShownEventArgs>? OnSteamAchievementShown;
        public static bool Paused { get; set; }
        public static double GoalTarget { get; set; } = 1000;
        public static double GoalValue { get; set; } = 0;
        public static bool EnableMultiLang { get; set; }
        public static int EnqueuedDonations { get => SteamAchievementRequests.Count; }
        public static bool OBSSteamAchievementItemsSelected { get => SteamAchievementIconItem != null && SteamAchievementBackgroundItem != null; }
        public static SteamAchievementRequest? CurrentRequestPlaying;
        public static string SteamNotificationSoundPath { get; set; }
        static Animator Animator { get; set; }

        static SteamAchievementDispatcher()
        {
            SteamNotificationSoundPath = Settings.SteamNotificationSoundPath?? "G:\\OBS_DECK\\RESOURCES\\STEAM_ACH_SOUND.mp3";
            Animator = new Animator(60);
        }
        public static void AddAchievement(SteamAchievementRequest request)
        {
            InboudQueue.Enqueue(request);

        }

        static void PrepareAnimation()
        {
            Animator.DumpAnimations();
            var pa = new ProceduralAnimation(AchievementsScene, (float T, SceneItem x) =>
            {
                return GetCurentSceneTransform(T);
            });
            Animator.RegisterAnimation(pa);
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
                while (!InboudQueue.IsEmpty && InboudQueue.TryDequeue(out SteamAchievementRequest? request))
                {


                        SteamAchievementRequests.Enqueue(request);
                    

                }
                Thread.Sleep(100);
            }
        }

        static void DequeuingLoop()
        {
            while (doRun)
            {
                if (!Paused && OBSSteamAchievementItemsSelected && !SteamAchievementRequests.IsEmpty && SteamAchievementRequests.TryDequeue(out SteamAchievementRequest? request))
                {
                    CurrentRequestPlaying = request;
                    AchievementsScene?.Enable();
                    AchievementsScene?.TransformObject(newPos:new(573, 522));
                    SteamAchievementBackgroundItem?.Enable();
                    SteamAchievementIconItem?.Enable();
                    SteamAchievementBackgroundItem?.SetSource(CurrentRequestPlaying.BackgroundPath);
                    SteamAchievementIconItem?.SetSource(CurrentRequestPlaying.IconPath);
                    Thread thread = new Thread(SyncPlayNotification);
                    thread.Start();
                    Animator.Stop();
                    Animator.Reset();
                    PrepareAnimation();
                    Animator.Run();
                    Thread.Sleep(10000);
                    Animator.Stop();
                    AchievementsScene?.Disable();
                }
                else
                {
                    Thread.Sleep(100);
                }
            }
        }

        static AnimationTransform GetCurentSceneTransform(float time)
        {
            if (time == 0)
            {
                return new AnimationTransform(AchievementsScene);
            }

            var targetY = 0.0f;
            if (time < 1)
                targetY = 250 * (1 - time);
            else if (time < 9)
                targetY = 0;
            else if (time < 10)
                targetY = 250 * (time - 9);
            else
                targetY = 250;
            var at = new AnimationTransform(
                AchievementsScene,
                position: Float2.Larp(AchievementsScene.CurPosition, new Float2(0.0f, targetY), 0.5f),
                size:new Float2(1920, 1080),
                scale:Float2.One);
            return at;
        }


        static void SyncPlayNotification()
        {
            //SceneItem sceneItem = OBSStructure.RootScene.FindItem("DONATION_SCENE");
            //ItemText textItem = OBSStructure.RootScene.FindItem("DONATION_TEXT") as ItemText;
            //textItem.SetText(DisplayText);
            // sceneItem.Enable();
            var waveOutDevice = new WaveOut();
            var audioFileReader = new AudioFileReader(SteamNotificationSoundPath);
            waveOutDevice.Init(audioFileReader);
            waveOutDevice.Play();
            Thread.Sleep(2000);
            waveOutDevice?.Stop();
            audioFileReader?.Dispose();
            waveOutDevice?.Dispose();
            waveOutDevice = null;
            audioFileReader = null;
            //sceneItem.Disable();
        }
    }
}
