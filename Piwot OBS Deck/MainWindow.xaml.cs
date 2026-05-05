
using Newtonsoft.Json.Linq;
using PiwotOBS;
using PiwotOBS.PMath;
using PiwotOBS.Structure;
using PiwotOBS.Structure.Animations;
using PiwotOBSDeck.Requests;
using PiwotOBSDeck.Requests.Events;
using PiwotOBSDeck.VTuber;
using PiwotOBSDeck.WebServices;
using PiwotOBSDeck.WebServices.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace PiwotOBSDeck
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Avatar Avatar;
        SceneItem JumpscareImage;
        SceneItem JumpscareImage2;
        SceneItem JumpscareSound;
        SceneItem PoczekaShrek;
        FrameAnimation beerAnim;
        Animator Animator = new Animator(30);
        DVDPong DVDPong;
        RequestRecieverService RequestRecService;
        List<string> vcPresenceScenes;
        List<string> donationScenes;
        List<string> donationTextItems;
        List<string> achievementScenes;
        List<string> achievementBackgroundImageItems;
        List<string> achievementIconImageItems;
        List<string> goalScenes;
        List<string> goalTextItems;
        List<string> avatarScenes;
        List<string> avatarSceneItems;
        List<string> goalBarItems;
        public readonly Color ConnectedColor = Colors.LightGreen;
        public readonly Color ConnectingColor = Colors.Cyan;
        public readonly Color DisconnectedColor = Colors.Gray;

        public readonly Color ConnectedBackColor = Color.FromArgb(0xFF, 0xCA, 0xFF, 0xCA);
        public readonly Color ConnectingBackColor = Colors.LightCyan;
        public readonly Color DisconnectedBackColor = Colors.LightGray;
        public MainWindow()
        {
            InitializeComponent();
            DVDPong = new DVDPong();
            if (Settings.DonationSceneName != null)
            {
                donationScenes = new List<string>()
                {
                    Settings.DonationSceneName
                };
                comboBox_donationScenes.ItemsSource = donationScenes;
                comboBox_donationScenes.SelectedIndex = 0;
            }

            if (Settings.DonationTextItemName != null)
            {
                donationTextItems = new List<string>()
                {
                    Settings.DonationTextItemName
                };
                comboBox_donationTexts.ItemsSource = donationTextItems;
                comboBox_donationTexts.SelectedIndex = 0;
            }

            if (Settings.AchievementSceneName != null)
            {
                achievementScenes = new List<string>()
                {
                    Settings.AchievementSceneName
                };
                comboBox_achievementScenes1.ItemsSource = achievementScenes;
                comboBox_achievementScenes1.SelectedIndex = 0;
            }


            if (Settings.AchievementBackgroundItemName != null)
            {
                achievementBackgroundImageItems = new List<string>()
                {
                    Settings.AchievementBackgroundItemName
                };
                comboBox_achBackground1.ItemsSource = achievementBackgroundImageItems;
                comboBox_achBackground1.SelectedIndex = 0;
            }

            if (Settings.AchievementIconItemName != null)
            {
                achievementIconImageItems = new List<string>()
                {
                    Settings.AchievementIconItemName
                };
                comboBox_achIcon1.ItemsSource = achievementIconImageItems;
                comboBox_achIcon1.SelectedIndex = 0;
            }


            if (Settings.GoalSceneName != null)
            {
                goalScenes = new List<string>()
                {
                    Settings.GoalSceneName
                };
                comboBox_goalnScenes.ItemsSource = goalScenes;
                comboBox_goalnScenes.SelectedIndex = 0;
            }

            if (Settings.GoalTextItemName != null)
            {
                goalTextItems = new List<string>()
                {
                    Settings.GoalTextItemName
                };
                comboBox_goalTexts.ItemsSource = goalTextItems;
                comboBox_goalTexts.SelectedIndex = 0;
            }

            if (Settings.GoalBarItemName != null)
            {
                goalBarItems = new List<string>()
                {
                    Settings.GoalBarItemName
                };
                comboBox_goalBar.ItemsSource = goalBarItems;
                comboBox_goalBar.SelectedIndex = 0;
            }

            if (Settings.AvatarSceneName != null)
            {
                avatarScenes = new List<string>()
                {
                    Settings.AvatarSceneName
                };
                comboBox_avatarScenes.ItemsSource = avatarScenes;
                comboBox_avatarScenes.SelectedIndex = 0;
            }

            if (Settings.VCPresenceSceneName != null)
            {
                vcPresenceScenes = new List<string>()
                {
                    Settings.VCPresenceSceneName
                };
                comboBox_vcPresenceScenes.ItemsSource = vcPresenceScenes;
                comboBox_vcPresenceScenes.SelectedIndex = 0;
            }



            if (Settings.PortraitSize != null)
            {
                VCPresenceControler.PortraitSize = Settings.PortraitSize;
                textBox_VCP_PortraitSizeX.Text = Settings.PortraitSize.X.ToString();
                textBox_VCP_PortraitSizeY.Text = Settings.PortraitSize.Y.ToString();
            }
            if (Settings.PortraitScreenAnchorPosition != null)
            {
                VCPresenceControler.PortraitScreenAnchorPosition = Settings.PortraitScreenAnchorPosition;
                textBox_VCP_PortraitScreenAnchorX.Text = Settings.PortraitScreenAnchorPosition.X.ToString();
                textBox_VCP_PortraitScreenAnchorY.Text = Settings.PortraitScreenAnchorPosition.Y.ToString();
            }
            if (Settings.PortraitRelativeOrdinalOffset != null)
            {
                VCPresenceControler.PortraitRelativeOrdinalOffset = Settings.PortraitRelativeOrdinalOffset;
                textBox_VCP_PortraitOrdinalOffsetX.Text = Settings.PortraitRelativeOrdinalOffset.X.ToString();
                textBox_VCP_PortraitOrdinalOffsetY.Text = Settings.PortraitRelativeOrdinalOffset.Y.ToString();
            }
            if (Settings.PortraitMovementMagnitude != null)
            {
                VCPresenceControler.PortraitMovementMagnitude = Settings.PortraitMovementMagnitude;
                textBox_VCP_PortraitMovementMagnitudeX.Text = Settings.PortraitMovementMagnitude.X.ToString();
                textBox_VCP_PortraitMovementMagnitudeY.Text = Settings.PortraitMovementMagnitude.Y.ToString();
            }
            VCPresenceControler.PortraitRotationMagnitude = Settings.PortraitRotationMagnitude;
            textBox_VCP_PortraitRotationMagnitude.Text = Settings.PortraitRotationMagnitude.ToString();
            VCPresenceControler.PortraitMovementTimePeriod = Settings.PortraitMovementTimePeriod;
            textBox_VCP_PortraitMovementPeriod.Text = Settings.PortraitMovementTimePeriod.ToString();
            VCPresenceControler.PortraitRotationTimePeriod = Settings.PortraitRotationTimePeriod;
            textBox_VCP_PortraitRotationPeriod.Text = Settings.PortraitRotationTimePeriod.ToString();
            VCPresenceControler.PortraitMovementTimeOffset = Settings.PortraitMovementTimeOffset;
            textBox_VCP_PortraitMovementOffset.Text = Settings.PortraitMovementTimeOffset.ToString();
            VCPresenceControler.PortraitRotationTimeOffset = Settings.PortraitRotationTimeOffset;
            textBox_VCP_PortraitRotationOffset.Text = Settings.PortraitRotationTimeOffset.ToString();
            VCPresenceControler.PortraitMovementOrdinalTimeOffsetMultiplier = Settings.PortraitMovementOrdinalTimeOffsetMultiplier;
            textBox_VCP_PortraitOrdinalMoveTimeOffsetMultiplier.Text = Settings.PortraitMovementOrdinalTimeOffsetMultiplier.ToString();
            VCPresenceControler.PortraitRotationOrdinalTimeOffsetMultiplier = Settings.PortraitRotationOrdinalTimeOffsetMultiplier;
            textBox_VCP_PortraitOrdinalRotTimeOffsetMultiplier.Text = Settings.PortraitRotationOrdinalTimeOffsetMultiplier.ToString();



        textBox_dvdSpeed.Text = $"{Settings.DVDSpeed}";
            checkBox_enableMultilang.IsChecked = Settings.MultilangEnabled;

            OBSDeck.Connected += new EventHandler(OnConnected);
            Console.WriteLine(Storage.GetFilenameInSettings("a"));
            VoiceDisplay.AddElement(new SerieHLine(0.95, VoiceDisplay.Height * 0.1, Color.FromArgb(128, 255, 0, 0)));
            VoiceDisplay.AddElement(new SerieWatcher(() => VoiceCenter.AvgVolumeHistory.ToArray(), 1, lineThickness: 2, color: Colors.DarkGray));
            VoiceDisplay.AddElement(new SerieWatcher(() => VoiceCenter.VolumeHistory.ToArray(), 1, lineThickness: 2, color: Colors.Black));
            var hLine = new SerieHLine(0, 1, Colors.Yellow);
            VoiceDisplay.AddElement(hLine);
            VoiceCenter.VolumeUpdate += (x, volumes) => { hLine.X = volumes.Item1; };
            RequestRecService = new RequestRecieverService();
            RequestRecService.OnClientConnected += RequestRecService_OnClientConnected;
            RequestRecService.OnClientDisconnected += RequestRecService_OnClientDisconnected;
            RequestRecService.OnDonation += RequestRecService_OnDonation;
            RequestRecService.OnSteamAchievement += RequestRecService_OnSteamAchievement;
            RequestRecService.OnConnectionFail += RequestRecService_OnConnectionFail;
            RequestRecService.OnVCPresenceUpdate += RequestRecService_OnVCPresenceUpdate;
            //VoiceDisplay.AddElement(new SerieWatcher(() => new float[] { VoiceCenter.CurrentVolume, VoiceCenter.CurrentVolume }, 1, segments:1, lineThickness: 1, color: Colors.Yellow));
            
            DonationDispatcher.Start();
            DonationDispatcher.OnDonationAdded += DonationDispatcher_OnDonationAdded;
            DonationDispatcher.OnDonationShown += DonationDispatcher_OnDonationShown;
            SteamAchievementDispatcher.Start();
           
        }

        private void RequestRecService_OnVCPresenceUpdate(object? sender, OnVCPresenceUpdateEventArgs e)
        {
            VCPresenceControler.UpdatePresence(e.Request);
            var enablesStatus = VCPresenceControler.Enabled ? "Enabled" : "Disabled";
            var curViewersText = $"{e.Request.Ids.Length}";
            var lastUpdate = VCPresenceControler.LastUpdate.ToLocalTime().ToString("dd.MM HH:mm:ss.ff");
            Dispatcher.InvokeAsync(new Action(() =>
            {
                textBlock_VCPresenceCurViewers.Text = curViewersText;
                textBlock_VCPresenceStatus.Text = enablesStatus;
                textBlock_VCPresenceLastUpdate.Text = lastUpdate;
            }));
        }

        private void RequestRecService_OnSteamAchievement(object? sender, OnSteamAchievementEventArgs e)
        {
            SteamAchievementDispatcher.AddAchievement(SteamAchievementRequest.FromWebRequest(e.SteamAchievementRequest));
        }

        private void DonationDispatcher_OnDonationShown(object? sender, OnDonationShownEventArgs e)
        {
            Dispatcher.InvokeAsync(new Action(() =>
            {
                textBox_goalTargetValue.Text = $"{DonationDispatcher.GoalTarget}";
                textBox_goalProgressValue.Text = $"{DonationDispatcher.GoalValue}";
                progressBar_goal.Value = DonationDispatcher.GoalValue / DonationDispatcher.GoalTarget * progressBar_goal.Maximum;
                textBlock_donationQueueValue.Text = $"{DonationDispatcher.DonationRequests.Count}";
            }));
        }

        private void DonationDispatcher_OnDonationAdded(object? sender, EventArgs e)
        {
            Dispatcher.InvokeAsync(new Action(() =>
            {
                textBlock_donationQueueValue.Text = $"{DonationDispatcher.DonationRequests.Count}";
            }));
        }

        private void RequestRecService_OnConnectionFail(object? sender, EventArgs e)
        {
            Dispatcher.InvokeAsync(new Action(() => {
                textBlock_DonationConnectionStatus.Text = "disconnected";
            }));
        }

        private void RequestRecService_OnClientDisconnected(object? sender, OnRequestClientDisconnectedEventArgs e)
        {
            Dispatcher.InvokeAsync(new Action(() => {
                textBlock_DonationConnectionStatus.Text = "disconnected";
                groupBox_connectionsDonations.BorderBrush = new SolidColorBrush(DisconnectedColor);
                groupBox_connectionsDonations.Background = new SolidColorBrush(DisconnectedBackColor);
            }));
        }

        private void RequestRecService_OnDonation(object? sender, OnDonationEventArgs e)
        {
            DonationDispatcher.AddDonation(DonationRequest.FromWebRequest(e.DonationRequest));
            /*Dispatcher.InvokeAsync(new Action(() => {
                DonationRequest donationRequest = DonationRequest.FromWebRequest(e.DonationRequest);
                donationRequest.PlaySoundAsync(downloadIfMissing: true);
            }));
            */
            
        }

        private void RequestRecService_OnClientConnected(object? sender, OnRequestClientConnectedEventArgs e)
        {
            Dispatcher.InvokeAsync(new Action(() => {
                textBlock_DonationConnectionStatus.Text = "connected";
                groupBox_connectionsDonations.BorderBrush = new SolidColorBrush(ConnectedColor);
                groupBox_connectionsDonations.Background = new SolidColorBrush(ConnectedBackColor);
            }));
        }

        private void OnConnected(object? sender, EventArgs e)
        {
            Console.WriteLine("Connected");
            DispatcherOperation? dispatcherOperation = Dispatcher.InvokeAsync(new Action(() =>
            {
                Console.WriteLine("Dispatcher 1 started.");
                try
                {
                    this.Icon = Imaging.CreateBitmapSourceFromHIcon(
                        Properties.Resources.OBSDeckIconOn.Handle,
                        Int32Rect.Empty,
                        BitmapSizeOptions.FromEmptyOptions());
                    //this.Title = "PiwotOBSDeck (connected)";
                    textBlock_OBSConnectionStatus.Text = "connected";
                    Console.WriteLine((ImageSource)Resources["OBSDeckIconOn.ico"]);

                    donationScenes = OBSDeck.GetSceneList().Select(x => x.Name).ToList();
                    comboBox_donationScenes.ItemsSource = donationScenes;
                    comboBox_donationScenes.Items.Refresh();
                    Console.WriteLine("Donations refreshed.");

                    goalScenes = OBSDeck.GetSceneList().Select(x => x.Name).ToList();
                    comboBox_goalnScenes.ItemsSource = goalScenes;
                    comboBox_goalnScenes.Items.Refresh();
                    Console.WriteLine("Goal refreshed.");

                    avatarScenes = OBSDeck.GetSceneList().Select(x => x.Name).ToList();
                    comboBox_avatarScenes.ItemsSource = avatarScenes;
                    comboBox_avatarScenes.Items.Refresh();
                    Console.WriteLine("Avatar refreshed.");

                    achievementScenes = OBSDeck.GetSceneList().Select(x => x.Name).ToList();
                    Console.WriteLine(string.Join(", ", achievementScenes));
                    comboBox_achievementScenes1.ItemsSource = achievementScenes;
                    comboBox_achievementScenes1.Items.Refresh();
                    Console.WriteLine("Achievements refreshed.");

                    vcPresenceScenes = OBSDeck.GetSceneList().Select(x => x.Name).ToList();
                    Console.WriteLine(string.Join(", ", vcPresenceScenes));
                    comboBox_vcPresenceScenes.ItemsSource = vcPresenceScenes;
                    comboBox_vcPresenceScenes.Items.Refresh();
                    Console.WriteLine("VC presence refreshed.");


                    groupBox_connectionsOBS.BorderBrush = new SolidColorBrush(ConnectedColor);
                    groupBox_connectionsOBS.Background = new SolidColorBrush(ConnectedBackColor);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }

                groupBox_Trash.IsEnabled = true;
                groupBox_VTuber.IsEnabled = true;
                groupBox_achievementPopupMain.IsEnabled = true;
                groupBox_DonationsMain.IsEnabled = true;
                groupBox_VCPresenceMain.IsEnabled = true;

                Console.WriteLine("Loaded basic objects.");

            }));
            dispatcherOperation.Wait();
            OBSStructure.Init();
            Console.WriteLine("Init done.");
            /*Avatar = new Avatar("XD_FACE_AVATAR", "BOB_NO_FACE")
            {
                Name = "BASIC_XD_BOB"
            };*/
            Avatar = Avatar.FromSave(Path.Combine(Settings.StoragePath_Avatars, "BASIC_XD_BOB"));
            Avatar.Reset();
            Console.WriteLine("Avatar reset.");
            dispatcherOperation = Dispatcher.InvokeAsync(new Action(() =>
            {
                Console.WriteLine("Connected");
                try
                {
                    textBlock_currentFace.Text = Avatar.CurFaceName;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }

                groupBox_Trash.IsEnabled = true;
                groupBox_VTuber.IsEnabled = true;
                Console.WriteLine("Updated UI");

            }));
            dispatcherOperation.Wait();
            JumpscareImage = OBSStructure.RootScene.FindItem("JUMPSCARE_IMAGE");
            JumpscareSound = OBSStructure.RootScene.FindItem("VINE_BOOM");
            JumpscareImage2 = OBSStructure.RootScene.FindItem("RALSEI2");
            PoczekaShrek = OBSStructure.RootScene.FindItem("NO_TO_SE_JESZCZE_POCZEKA");
            beerAnim = FrameAnimation.FromJson(@"G:\OBS_DECK\ANIMS\BEER_ANIM.json", OBSStructure.RootScene);
            Console.WriteLine("Trash scenes loaded");
            Animator.RegisterAnimation(beerAnim);
            Animator.Run();
            Console.WriteLine("Animator enabled");

            DVDPong.Speed = Settings.DVDSpeed;
            dispatcherOperation = Dispatcher.InvokeAsync(new Action(() =>
            {
                textBlock_BeerLoopStatus.Text = beerAnim.Loop ? "on" : "off";
                if (Settings.DonationSceneName != null)
                    TryGettingDonationScene();
                if (DonationDispatcher.DonationScene != null && Settings.DonationTextItemName != null)
                {
                    TryGettingDonationTextItem();
                }

                if (Settings.AchievementSceneName != null)
                    TryGettingAchievementsScene();
                if (SteamAchievementDispatcher.AchievementsScene != null && Settings.AchievementIconItemName != null)
                {
                    TryGettingAchivementIconItem();
                }
                if (SteamAchievementDispatcher.AchievementsScene != null && Settings.AchievementBackgroundItemName != null)
                {
                    TryGettingAchivementBackgroundItem();
                }
                if (Settings.GoalSceneName != null)
                    TryGettingGoalScene();
                if (Settings.AvatarSceneName != null)
                    TryGettingAvatarScene();
                if (DonationDispatcher.GoalScene != null)
                {
                    if (Settings.GoalTextItemName != null)
                        TryGettingGoalTextItem();
                    if (Settings.GoalBarItemName != null)
                        TryGettingGoalBarItem();
                }

                if(Settings.VCPresenceSceneName  != null)
                {
                    TryGettingVCPresenceScene();
                }

                DonationDispatcher.RefreshGoal();
                Console.WriteLine("Dvd refreshed.");

            }));
            dispatcherOperation.Wait();

            
            Console.WriteLine("All OnConnect actions triggered.");
        }

        private void ButtonConnect_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow loginWIndow = new LoginWindow();
            loginWIndow.Show();
        }

        private void ButtonConnect_Copy_Click(object sender, RoutedEventArgs e)
        {
            var scene = OBSDeck.GetScene("SAFARI");
            var json = scene.ToJson();
            Console.WriteLine(json.ToJsonString());
        }

        private void ButtonVoiceToggle_Click(object sender, RoutedEventArgs e)
        {
            if(VoiceCenter.Enabled)
            {
                VoiceCenter.Disable();
                VoiceDisplay.Pause();
            }    
            else
            {
                VoiceCenter.Enable();
                VoiceDisplay.Start();
            }
            UpdateVoiceMeterBackground();
        }

        private void ButtonAutoConnect_Click(object sender, RoutedEventArgs e)
        {
            var SavedCredentials = Storage.LoadSettings("StoredCredentials");
            if (SavedCredentials != null)
            {
                OBSDeck.Connect(SavedCredentials.GetVal<string>("IP") ?? "", SavedCredentials.GetVal<int>("port").ToString(), SavedCredentials.GetVal<string>("password") ?? "");
            }
        }

        private void ButtonToggleAvatar_Click(object sender, RoutedEventArgs e)
        {
            Avatar.Toggle();
            UpdateAvatarMainBackground();
        }

        void UpdateAvatarMainBackground()
        {
            if (Avatar?.Enabled??false)
            {
                groupBox_vtuberAvatarMain.BorderBrush = new SolidColorBrush(ConnectedColor);
                groupBox_vtuberAvatarMain.Background = new SolidColorBrush(ConnectedBackColor);
            }
            else
            {
                groupBox_vtuberAvatarMain.BorderBrush = new SolidColorBrush(DisconnectedColor);
                groupBox_vtuberAvatarMain.Background = new SolidColorBrush(DisconnectedBackColor);
            }
        }

        void UpdateVoiceMeterBackground()
        {
            if (VoiceCenter.Enabled)
            {
                groupBox_voiceMeter.BorderBrush = new SolidColorBrush(ConnectedColor);
                groupBox_voiceMeter.Background = new SolidColorBrush(ConnectedBackColor);
            }
            else
            {
                groupBox_voiceMeter.BorderBrush = new SolidColorBrush(DisconnectedColor);
                groupBox_voiceMeter.Background = new SolidColorBrush(DisconnectedBackColor);
            }
        }

        private void ButtonJumpscare_Click(object sender, RoutedEventArgs e)
        {
            Task task = new Task(
                () =>
                {
                    string fName = @"G:\OBS_DECK\RESOURCES\VINE_BOOM.mp3";
                    MediaPlayer mediaPlayer = new MediaPlayer();
                    mediaPlayer.Open(new Uri(fName));
                    mediaPlayer.Volume = 0.7;
                    mediaPlayer.Play();
                    Thread.Sleep(100);
                    JumpscareImage.Enable();
                    Thread.Sleep(300);
                    JumpscareImage.Disable();
                    Thread.Sleep(2500);
                    mediaPlayer.Stop();
                    mediaPlayer.Close();
                    mediaPlayer = null;
                }
                );
            task.Start();
        }

        private void ButtonJumpscare2_Click(object sender, RoutedEventArgs e)
        {
            Task task = new Task(
                () =>
                {
                    string fName = @"G:\OBS_DECK\RESOURCES\VINE_BOOM.mp3";
                    MediaPlayer mediaPlayer = new MediaPlayer();
                    mediaPlayer.Open(new Uri(fName));
                    mediaPlayer.Volume = 0.95;
                    mediaPlayer.Play();
                    Thread.Sleep(300);
                    JumpscareImage2.Enable();
                    Thread.Sleep(500);
                    JumpscareImage2.Disable();
                    Thread.Sleep(1900);
                    mediaPlayer.Stop();
                    mediaPlayer.Close();
                    mediaPlayer = null;
                }
                );
            task.Start();
        }

        private void ButtonBeer_Loop_Click(object sender, RoutedEventArgs e)
        {
            beerAnim.Loop = !beerAnim.Loop;
            textBlock_BeerLoopStatus.Text = beerAnim.Loop ? "on":"off";
        }

        private void ButtonBeer_Click(object sender, RoutedEventArgs e)
        {
            Animator.Stop();
            Animator.Run();

        }

        private void ButtonDVD_Click(object sender, RoutedEventArgs e)
        {
            if (!DVDPong.IsRunning || DVDPong.IsPaused)
            {
                DVDPong.Start();
            }
            else
            {
                DVDPong.Pause();
            }
        }

        private void button_loadAvatar_Click(object sender, RoutedEventArgs e)
        {
            Avatar?.Disable();

            Avatar = Avatar.FromSave(Path.Combine(Settings.StoragePath_Avatars, "BASIC_XD_BOB"));
            Avatar.Reset();
            textBlock_currentFace.Text = Avatar.CurFaceName;
            UpdateAvatarMainBackground();
        }

        private void button_saveAvatar_Click(object sender, RoutedEventArgs e)
        {
            Avatar?.UpdateFromOBS();
            Avatar?.Save(Settings.StoragePath_Avatars);
        }

        private void ButtonDumpScenesToFile_Click(object sender, RoutedEventArgs e)
        {

            foreach(var kind in OBSDeck.OBS.GetInputKindList())
            {
                Console.WriteLine(kind);
            }

            var scenes = OBSDeck.GetSceneList();
            JsonArray ja = new JsonArray();
            foreach(var scene in scenes)
            {
                ja.Add(scene.ToJson());
            }
            using(StreamWriter sw = new StreamWriter(Path.Join(Settings.StoragePath, "scenes_dump.json")))
            {
                sw.Write(ja.ToJsonString(Misc.JsonOptions));
            }

            var x = OBSDeck.GetScenesJson();

            using (StreamWriter sw = new StreamWriter(Path.Join(Settings.StoragePath, "scenes_dump_raw.json")))
            {
                sw.Write(x.ToJsonString(Misc.JsonOptions));
            }
        }

        private void button_text_Click(object sender, RoutedEventArgs e)
        {

        }

        private void button_toggleDonations_Click(object sender, RoutedEventArgs e)
        {
            if (RequestRecService.IsRunning)
            {
                RequestRecService.Stop();
            }
            else
            {
                textBlock_DonationConnectionStatus.Text = "connecting...";
                groupBox_connectionsDonations.BorderBrush = new SolidColorBrush(ConnectingColor);
                groupBox_connectionsDonations.Background = new SolidColorBrush(ConnectingBackColor);
                RequestRecService.Start();
            }
        }

        private void checkBox_enableMultilang_Checked(object sender, RoutedEventArgs e)
        {
            DonationDispatcher.EnableMultiLang = checkBox_enableMultilang.IsChecked??false;
            Settings.MultilangEnabled = DonationDispatcher.EnableMultiLang;
            Settings.Save();
        }
        void TryGettingDonationScene()
        {
            string sceneName = (string)comboBox_donationScenes.SelectedItem;
            Scene? scene = (Scene?)(OBSStructure.RootScene?.FindItem(sceneName));
            var textItems = scene?.GetChildrenOfType<ItemText>(true) ?? new List<ItemText>();
            donationTextItems = textItems.Select(x => x.Name).ToList();
            comboBox_donationTexts.ItemsSource = donationTextItems;
            comboBox_donationTexts.Items.Refresh();
            if (Settings.DonationSceneName == sceneName && Settings.DonationTextItemName != null)
            {
                if (donationTextItems.Contains(Settings.DonationTextItemName))
                {
                    comboBox_donationTexts.SelectedItem = Settings.DonationTextItemName;
                }
            }
            DonationDispatcher.DonationScene = scene;
            Settings.DonationSceneName = sceneName;
            Settings.Save();
        }

        void TryGettingAchievementsScene()
        {
            string sceneName = (string)comboBox_achievementScenes1.SelectedItem;
            Scene? scene = (Scene?)(OBSStructure.RootScene?.FindItem(sceneName));
            var imageItems = scene?.GetChildrenOfType<ItemImage>(true) ?? new List<ItemImage>();
            achievementBackgroundImageItems = imageItems.Select(x => x.Name).ToList();
            achievementIconImageItems = imageItems.Select(x => x.Name).ToList();
            comboBox_achBackground1.ItemsSource = achievementBackgroundImageItems;
            comboBox_achIcon1.ItemsSource = achievementIconImageItems;
            comboBox_achBackground1.Items.Refresh();
            comboBox_achIcon1.Items.Refresh();
            if (Settings.AchievementSceneName == sceneName)
            {
                if (Settings.AchievementBackgroundItemName != null && achievementBackgroundImageItems.Contains(Settings.AchievementBackgroundItemName))
                {
                    comboBox_achBackground1.SelectedItem = Settings.AchievementBackgroundItemName;
                }
                if (Settings.AchievementIconItemName != null && achievementIconImageItems.Contains(Settings.AchievementIconItemName))
                {
                    comboBox_achIcon1.SelectedItem = Settings.AchievementIconItemName;
                }
            }



            SteamAchievementDispatcher.AchievementsScene = scene;
            Settings.AchievementSceneName = sceneName;
            Settings.Save();
        }

        void TryGettingVCPresenceScene()
        {
            string sceneName = (string)comboBox_vcPresenceScenes.SelectedItem;
            Scene? scene = (Scene?)(OBSStructure.RootScene?.FindItem(sceneName));
            var imageItems = scene?.GetChildrenOfType<ItemImage>(true).Where((x)=>x.Name.StartsWith(textBox_VCPresenceNameInput.Text)).ToList() ?? new List<ItemImage>();
            


            if (Settings.VCPresenceSceneName == sceneName)
            {
                // nothing for now i guess
            }


            var dispatcherOperation = Dispatcher.InvokeAsync(new Action(() =>
            {
                try
                {
                    textBlock_VCPresenceFoundPortraits.Text = imageItems.Count.ToString();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }

            }));
            dispatcherOperation.Wait();
            VCPresenceControler.UpdateItems(scene, imageItems);
            Settings.VCPresenceSceneName = sceneName;
            Settings.Save();
        }

        void TryGettingDonationTextItem()
        {
            string textItemName = (string)comboBox_donationTexts.SelectedItem;
            var textItem = DonationDispatcher.DonationScene?.FindItem(textItemName);
            if (textItem != null)
            {
                DonationDispatcher.DonationTextItem = (ItemText?)textItem;
            }
            Settings.DonationTextItemName = textItemName;
            Settings.Save();
        }

        void TryGettingAchivementIconItem()
        {
            string imageName = (string)comboBox_achIcon1.SelectedItem;
            var imageItem = SteamAchievementDispatcher.AchievementsScene?.FindItem(imageName);
            if (imageItem != null)
            {
                SteamAchievementDispatcher.SteamAchievementIconItem = (ItemImage?)imageItem;
            }
            Settings.AchievementIconItemName = imageName;
            Settings.Save();
        }

        void TryGettingAchivementBackgroundItem()
        {
            string imageName = (string)comboBox_achBackground1.SelectedItem;
            var imageItem = SteamAchievementDispatcher.AchievementsScene?.FindItem(imageName);
            if (imageItem != null)
            {
                SteamAchievementDispatcher.SteamAchievementBackgroundItem = (ItemImage?)imageItem;
            }
            Settings.AchievementBackgroundItemName = imageName;
            Settings.Save();
        }

        void TryGettingGoalScene()
        {
            string sceneName = (string)comboBox_goalnScenes.SelectedItem;
            Scene? scene = (Scene?)(OBSStructure.RootScene?.FindItem(sceneName));
            var textItems = scene?.GetChildrenOfType<ItemText>(true) ?? new List<ItemText>();
            goalTextItems = textItems.Select(x => x.Name).ToList();
            comboBox_goalTexts.ItemsSource = goalTextItems;
            comboBox_goalTexts.Items.Refresh();

            var goalItems = scene?.GetChildrenOfType<SceneItem>(true) ?? new List<SceneItem>();
            goalBarItems = goalItems.Select(x => x.Name).ToList();
            comboBox_goalBar.ItemsSource = goalBarItems;
            comboBox_goalBar.Items.Refresh();

            if (Settings.GoalSceneName == sceneName)
            {
                if (Settings.GoalTextItemName != null && goalTextItems.Contains(Settings.GoalTextItemName))
                {
                    comboBox_goalTexts.SelectedItem = Settings.GoalTextItemName;
                }
                if (Settings.GoalBarItemName != null && goalBarItems.Contains(Settings.GoalBarItemName))
                {
                    comboBox_goalBar.SelectedItem = Settings.GoalBarItemName;
                }
            }

            DonationDispatcher.GoalScene = scene;
            Settings.GoalSceneName = sceneName;
            Settings.Save();
        }

        void TryGettingAvatarScene()
        {
            string sceneName = (string)comboBox_avatarScenes.SelectedItem;
            Scene? scene = (Scene?)(OBSStructure.RootScene?.FindItem(sceneName));
            var children = scene?.GetChildren(true) ?? new List<SceneItem>();
            avatarSceneItems = children.Select(x => x.Name).ToList();
            listBox_faceSelection.ItemsSource = avatarSceneItems;
            listBox_faceSelection.Items.Refresh();


            if (Settings.AvatarSceneName == sceneName)
            {
                foreach(var faceName in Avatar.Faces.Keys)
                {
                    if(avatarSceneItems.Contains(faceName))
                    {
                        listBox_faceSelection.SelectedItems.Add(faceName);
                    }
                }
            }

            Settings.AvatarSceneName = sceneName;
            Settings.Save();
        }

        void TryGettingGoalTextItem()
        {
            string textItemName = (string)comboBox_goalTexts.SelectedItem;
            var textItem = DonationDispatcher.GoalScene?.FindItem(textItemName);
            if (textItem != null)
            {
                DonationDispatcher.GoalTextItem = (ItemText?)textItem;
            }
            Settings.GoalTextItemName = textItemName;
            Settings.Save();
        }

        void TryGettingGoalBarItem()
        {
            string barItemName = (string)comboBox_goalBar.SelectedItem;
            var barItem = DonationDispatcher.GoalScene?.FindItem(barItemName);
            if (barItem != null)
            {
                DonationDispatcher.GoalBarItem = (SceneItem?)barItem;
            }
            Settings.GoalBarItemName = barItemName;
            Settings.Save();
        }

        private void comboBox_donationScenes_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if(OBSDeck.IsConnected)
                Dispatcher.InvokeAsync(TryGettingDonationScene);
        }

        private void comboBox_donationTexts_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (OBSDeck.IsConnected)
                Dispatcher.InvokeAsync(TryGettingDonationTextItem);
        }

        private void comboBox_goalnScenes_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (OBSDeck.IsConnected)
                Dispatcher.InvokeAsync(TryGettingGoalScene);
        }

        private void comboBox_goalTexts_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (OBSDeck.IsConnected)
                Dispatcher.InvokeAsync(TryGettingGoalTextItem);
        }

        private void comboBox_goalBar_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (OBSDeck.IsConnected)
                Dispatcher.InvokeAsync(TryGettingGoalBarItem);
        }

        private void button_skipDonate_Click(object sender, RoutedEventArgs e)
        {
            DonationDispatcher.SkipDonation();
        }


        private void textBox_goalValue_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if(e.Key == System.Windows.Input.Key.Enter)
            {
                Keyboard.ClearFocus();
            }
        }

        private void textBox_goalProgress_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                Keyboard.ClearFocus();
            }
        }

        private void textBox_goalTargetValue_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            var textValue = textBox_goalTargetValue.Text;
            if (double.TryParse(textValue, out double value) && value > 0)
            {
                textBox_goalTargetValue.Text = $"{value}";
                if (DonationDispatcher.GoalTarget != value)
                {
                    DonationDispatcher.SetGoalTarget(value);
                }
            }
            else
            {
                textBox_goalTargetValue.Text = $"{DonationDispatcher.GoalTarget}";
            }
        }

        private void textBox_goalProgressValue_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            var textValue = textBox_goalProgressValue.Text;
            if (double.TryParse(textValue, out double value) && value >= 0)
            {
                textBox_goalProgressValue.Text = $"{value}";
                if (DonationDispatcher.GoalValue != value)
                {
                    DonationDispatcher.SetGoalValue(value);
                }
            }
            else
            {
                textBox_goalProgressValue.Text = $"{DonationDispatcher.GoalValue}";
            }
        }

        private void textBox_dvdSpeed_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (DVDPong == null) { return; }
            var textValue = textBox_dvdSpeed.Text;
            
            if (float.TryParse(textValue, out float value) && value > 0)
            {
                textBox_dvdSpeed.Text = $"{value}";
                if (DVDPong.Speed != value)
                {
                    DVDPong.Speed = value;
                    Settings.DVDSpeed = value;
                    Settings.Save();
                }
            }
            else
            {
                textBox_goalProgressValue.Text = $"{DonationDispatcher.GoalValue:0.00}";
            }
        }

        private void textBox_KeyUp_ClearFocus(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Keyboard.ClearFocus();
            }
        }

        private void progressBar_goal_MouseMove(object sender, MouseEventArgs e)
        {
            if (Mouse.LeftButton != MouseButtonState.Pressed)
                return;
            UpdateGoalValueByProgressMouse();
        }

        private void progressBar_goal_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            UpdateGoalValueByProgressMouse();
        }

        void UpdateGoalValueByProgressMouse()
        {
            var p = Mouse.GetPosition(progressBar_goal);
            progressBar_goal.Value = p.X * progressBar_goal.Maximum / progressBar_goal.ActualWidth;
            var value = DonationDispatcher.GoalTarget * progressBar_goal.Value / progressBar_goal.Maximum;
          
            textBox_goalProgressValue.Text = $"{value:0.00}";
        }

        private void button_pauseDonations_Click(object sender, RoutedEventArgs e)
        {
            DonationDispatcher.Paused = !DonationDispatcher.Paused;
            if(DonationDispatcher.Paused)
            {
                button_pauseDonations.Content = "Resume queue";
            }
            else
            {
                button_pauseDonations.Content = "Pause queue";
            }
        }

        private void slider_voiceVolume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var val = slider_voiceVolume.Value;
            val = Math.Pow(val / 500, 3);
            if(textBlock_voiceVolume != null)
                textBlock_voiceVolume.Text = $"{val*100:0.00}%";
            VoiceCenter.VolumeMultiplier = (float) val;
        }

        private void ButtonChirp_Click(object sender, RoutedEventArgs e)
        {
            Task task = new Task(
                () =>
                {
                    string fName = @"G:\OBS_DECK\RESOURCES\SMOKE_DETECTOR.mp3";
                    MediaPlayer? mediaPlayer = new MediaPlayer();
                    mediaPlayer.Open(new Uri(fName));
                    mediaPlayer.Volume = 0.5;
                    mediaPlayer.Play();
                    Thread.Sleep(2500);
                    mediaPlayer.Stop();
                    mediaPlayer.Close();
                    mediaPlayer = null;
                }
                );
            task.Start();
        }

        private void comboBox_avatarScenes_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (OBSDeck.IsConnected)
                Dispatcher.InvokeAsync(TryGettingAvatarScene);
        }

        private void button_faceSelection_Click(object sender, RoutedEventArgs e)
        {
            if(stackPanel_faceSelection.Visibility == Visibility.Visible)
            {
                stackPanel_faceSelection.Visibility = Visibility.Collapsed;
                button_faceSelection.Content = "Face selection";
                Avatar.SetFaces(listBox_faceSelection.SelectedItems.Cast<string>().ToList());
                //Avatar.UpdateFromOBS();
                Avatar.SaveDefinition(Settings.StoragePath_Avatars);
            }
            else
            {
                stackPanel_faceSelection.Visibility = Visibility.Visible;
                button_faceSelection.Content = "Confirm selection";
            }
        }

        private void button_faceRight_Click(object sender, RoutedEventArgs e)
        {
            Avatar.CycleFace(true);
            textBlock_currentFace.Text = Avatar.CurFaceName;
        }

        private void button_faceLeft_Click(object sender, RoutedEventArgs e)
        {
            Avatar.CycleFace(false);
            textBlock_currentFace.Text = Avatar.CurFaceName;
        }

        private void ButtonPoczeka_Shrek_Click(object sender, RoutedEventArgs e)
        {
            Task task = new Task(
                () =>
                {
                    PoczekaShrek.Enable();
                    string fName = @"G:\OBS_DECK\RESOURCES\POCZEKA_SHREK.mp3";
                    MediaPlayer? mediaPlayer = new MediaPlayer();
                    mediaPlayer.Open(new Uri(fName));
                    mediaPlayer.Volume = 0.7;
                    mediaPlayer.Play();
                    Thread.Sleep(3000);
                    mediaPlayer.Stop();
                    mediaPlayer.Close();
                    mediaPlayer = null;
                    PoczekaShrek.Disable();
                }
                );
            task.Start();
            
            //PoczekaShrek.Enable();

        }

        private void button_pauseAchievements1_Click(object sender, RoutedEventArgs e)
        {
            SteamAchievementDispatcher.Paused = !SteamAchievementDispatcher.Paused;
            if (SteamAchievementDispatcher.Paused)
            {
                button_pauseAchievements1.Content = "Resume queue";
            }
            else
            {
                button_pauseAchievements1.Content = "Pause queue";
            }
        }

        private void button_skipAchievement1_Click(object sender, RoutedEventArgs e)
        {

        }

        private void comboBox_achievementScenes1_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (OBSDeck.IsConnected)
                Dispatcher.InvokeAsync(TryGettingAchievementsScene);
        }

        private void comboBox_achBackground1_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (OBSDeck.IsConnected)
                Dispatcher.InvokeAsync(TryGettingAchivementBackgroundItem);
        }

        private void comboBox_achIcon1_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (OBSDeck.IsConnected)
                Dispatcher.InvokeAsync(TryGettingAchivementIconItem);
        }

        private void comboBox_vcPresenceScenes_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (OBSDeck.IsConnected)
                Dispatcher.InvokeAsync(TryGettingVCPresenceScene);
        }

        

        private void button_toggleVCPresence_Click(object sender, RoutedEventArgs e)
        {
            if (OBSDeck.IsConnected)
            {
                var enabled = VCPresenceControler.Toggle();
                button_toggleVCPresence.Content = !enabled ? "Enable" : "Disable";
                textBlock_VCPresenceStatus.Text = enabled ? "Enabled" : "Disabled";
            }
        }

        private void button_toggleVCPresenceSettings_Click(object sender, RoutedEventArgs e)
        {
            groupBox_VCPresenceSetings.Visibility = groupBox_VCPresenceSetings.Visibility == Visibility.Collapsed ? Visibility.Visible : Visibility.Collapsed;
        }

        private void textBox_VCPresenceNameInput_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            Settings.VCPresencePortraitNameTemplate = textBox_VCPresenceNameInput.Text;
            if (OBSDeck.IsConnected)
                Dispatcher.InvokeAsync(TryGettingVCPresenceScene);
        }

        private void textBox_VCP_PortraitSize_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (!OBSDeck.IsConnected)
            {
                return;
            }
            var textValueX = textBox_VCP_PortraitSizeX.Text;
            var textValueY = textBox_VCP_PortraitSizeY.Text;
            
            if (float.TryParse(textValueX, out float valueX) && valueX > 0 
                && float.TryParse(textValueY, out float valueY) && valueY > 0)
            {
                textBox_VCP_PortraitSizeX.Text = $"{valueX}";
                textBox_VCP_PortraitSizeY.Text = $"{valueY}";
                VCPresenceControler.PortraitSize = new PiwotOBS.PMath.Float2(valueX, valueY);
                Settings.PortraitSize = VCPresenceControler.PortraitSize;
                Settings.Save();
            }
            else
            {
                if (textBox_VCP_PortraitSizeX.Text.Length > 0)
                {
                    textBox_VCP_PortraitSizeX.Text = $"{VCPresenceControler.PortraitSize.X}";
                }
                if (textBox_VCP_PortraitSizeX.Text.Length > 0)
                {
                    textBox_VCP_PortraitSizeY.Text = $"{VCPresenceControler.PortraitSize.Y}";
                }
            }
        }

        private void textBox_VCP_PortraitScreenAnchor_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (!OBSDeck.IsConnected)
            {
                return;
            }
            var textValueX = textBox_VCP_PortraitScreenAnchorX.Text;
            var textValueY = textBox_VCP_PortraitScreenAnchorY.Text;
            if (float.TryParse(textValueX, out float valueX) 
                && float.TryParse(textValueY, out float valueY))
            {
                textBox_VCP_PortraitScreenAnchorX.Text = $"{valueX}";
                textBox_VCP_PortraitScreenAnchorY.Text = $"{valueY}";
                VCPresenceControler.PortraitScreenAnchorPosition = new PiwotOBS.PMath.Float2(valueX, valueY);
                Settings.PortraitScreenAnchorPosition = VCPresenceControler.PortraitScreenAnchorPosition;
                Settings.Save();
            }
            else
            {
                if (textValueX.Length > 0)
                {
                    textBox_VCP_PortraitScreenAnchorX.Text = $"{VCPresenceControler.PortraitScreenAnchorPosition.X}";
                }
                if (textValueY.Length > 0)
                {
                    textBox_VCP_PortraitScreenAnchorY.Text = $"{VCPresenceControler.PortraitScreenAnchorPosition.Y}";
                }
            }
        }

        private void textBox_VCP_PortraitOrdinalOffset_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (!OBSDeck.IsConnected)
            {
                return;
            }
            var textValueX = textBox_VCP_PortraitOrdinalOffsetX.Text;
            var textValueY = textBox_VCP_PortraitOrdinalOffsetY.Text;
            if (float.TryParse(textValueX, out float valueX)
                && float.TryParse(textValueY, out float valueY))
            {
                textBox_VCP_PortraitOrdinalOffsetX.Text = $"{valueX}";
                textBox_VCP_PortraitOrdinalOffsetY.Text = $"{valueY}";
                VCPresenceControler.PortraitRelativeOrdinalOffset = new PiwotOBS.PMath.Float2(valueX, valueY);
                Settings.PortraitRelativeOrdinalOffset = VCPresenceControler.PortraitRelativeOrdinalOffset;
                Settings.Save();
            }
            else
            {
                if (textValueX.Length > 0)
                {
                    textBox_VCP_PortraitOrdinalOffsetX.Text = $"{VCPresenceControler.PortraitRelativeOrdinalOffset.X}";
                }
                if (textValueY.Length > 0)
                {
                    textBox_VCP_PortraitOrdinalOffsetY.Text = $"{VCPresenceControler.PortraitRelativeOrdinalOffset.Y}";
                }
            }
        }

        private void textBox_VCP_PortraitMovementMagnitude_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (!OBSDeck.IsConnected)
            {
                return;
            }
            var textValueX = textBox_VCP_PortraitMovementMagnitudeX.Text;
            var textValueY = textBox_VCP_PortraitMovementMagnitudeY.Text;
            if (float.TryParse(textValueX, out float valueX)
                && float.TryParse(textValueY, out float valueY))
            {
                textBox_VCP_PortraitMovementMagnitudeX.Text = $"{valueX}";
                textBox_VCP_PortraitMovementMagnitudeY.Text = $"{valueY}";
                VCPresenceControler.PortraitMovementMagnitude = new PiwotOBS.PMath.Float2(valueX, valueY);
                Settings.PortraitMovementMagnitude = VCPresenceControler.PortraitMovementMagnitude;
                Settings.Save();
            }
            else
            {
                if (textValueX.Length > 0)
                {
                    textBox_VCP_PortraitMovementMagnitudeX.Text = $"{VCPresenceControler.PortraitMovementMagnitude.X}";
                }
                if (textValueY.Length > 0)
                {
                    textBox_VCP_PortraitMovementMagnitudeY.Text = $"{VCPresenceControler.PortraitMovementMagnitude.Y}";
                }
            }
        }

        private void textBox_VCP_PortraitRotationMagnitude_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {

            if (!OBSDeck.IsConnected)
            {
                return;
            }
            var textValue = textBox_VCP_PortraitRotationMagnitude.Text.Replace('.', ',');
            if (textValue.EndsWith(','))
                return;
            if (float.TryParse(textValue, out float value))
            {
                textBox_VCP_PortraitRotationMagnitude.Text = $"{value}";
                VCPresenceControler.PortraitRotationMagnitude = value;
                Settings.PortraitRotationMagnitude = VCPresenceControler.PortraitRotationMagnitude;
                Settings.Save();
            }
            else
            {
                if (textValue.Length > 0)
                {
                    textBox_VCP_PortraitRotationMagnitude.Text = $"{VCPresenceControler.PortraitRotationMagnitude}";
                }
            }
        }

        private void textBox_VCP_PortraitMovementPeriod_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (!OBSDeck.IsConnected)
            {
                return;
            }
            var textValue = textBox_VCP_PortraitMovementPeriod.Text.Replace('.', ',');
            if (textValue.EndsWith(','))
                return;
            if (float.TryParse(textValue, out float value))
            {
                // textBox_VCP_PortraitMovementPeriod.Text = $"{value}";
                VCPresenceControler.PortraitMovementTimePeriod = value;
                Settings.PortraitMovementTimePeriod = VCPresenceControler.PortraitMovementTimePeriod;
                Settings.Save();
            }
            else
            {
                if (textValue.Length > 0)
                {
                    textBox_VCP_PortraitMovementPeriod.Text = $"{VCPresenceControler.PortraitMovementTimePeriod}";
                }
            }
        }

        private void textBox_VCP_PortraitRotationPeriod_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (!OBSDeck.IsConnected)
            {
                return;
            }
            var textValue = textBox_VCP_PortraitRotationPeriod.Text.Replace('.', ',');
            if (textValue.EndsWith(','))
                return;
            if (float.TryParse(textValue, out float value))
            {
                textBox_VCP_PortraitRotationPeriod.Text = $"{value}";
                VCPresenceControler.PortraitRotationTimePeriod = value;
                Settings.PortraitRotationTimePeriod = VCPresenceControler.PortraitRotationTimePeriod;
                Settings.Save();
            }
            else
            {
                if (textValue.Length > 0)
                {
                    textBox_VCP_PortraitRotationPeriod.Text = $"{VCPresenceControler.PortraitRotationTimePeriod}";
                }
            }
        }


        private void textBox_VCP_PortraitMovementOffset_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (!OBSDeck.IsConnected)
            {
                return;
            }
            var textValue = textBox_VCP_PortraitMovementOffset.Text.Replace('.', ',');
            if (textValue.EndsWith(','))
                return;
            if (float.TryParse(textValue, out float value))
            {
                textBox_VCP_PortraitMovementOffset.Text = $"{value}";
                VCPresenceControler.PortraitMovementTimeOffset = value;
                Settings.PortraitMovementTimeOffset = VCPresenceControler.PortraitMovementTimeOffset;
                Settings.Save();
            }
            else
            {
                if (textValue.Length > 0)
                {
                    textBox_VCP_PortraitMovementOffset.Text = $"{VCPresenceControler.PortraitMovementTimeOffset}";
                }
            }
        }

        private void textBox_VCP_PortraitRotationOffset_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (!OBSDeck.IsConnected)
            {
                return;
            }
            var textValue = textBox_VCP_PortraitRotationOffset.Text.Replace('.', ',');
            if (textValue.EndsWith(','))
                return;
            if (float.TryParse(textValue, out float value))
            {
                textBox_VCP_PortraitRotationOffset.Text = $"{value}";
                VCPresenceControler.PortraitRotationTimeOffset = value;
                Settings.PortraitRotationTimeOffset = VCPresenceControler.PortraitRotationTimeOffset;
                Settings.Save();
            }
            else
            {
                if (textValue.Length > 0)
                {
                    textBox_VCP_PortraitRotationOffset.Text = $"{VCPresenceControler.PortraitRotationTimeOffset}";
                }
            }

        }

        private void textBox_VCP_PortraitOrdinalMoveTimeOffsetMultiplier_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (!OBSDeck.IsConnected)
            {
                return;
            }
            var textValue = textBox_VCP_PortraitOrdinalMoveTimeOffsetMultiplier.Text.Replace('.', ',');
            if (textValue.EndsWith(','))
                return;
            if (float.TryParse(textValue, out float value))
            {
                textBox_VCP_PortraitOrdinalMoveTimeOffsetMultiplier.Text = $"{value}";
                VCPresenceControler.PortraitMovementOrdinalTimeOffsetMultiplier = value;
                Settings.PortraitMovementOrdinalTimeOffsetMultiplier = VCPresenceControler.PortraitMovementOrdinalTimeOffsetMultiplier;
                Settings.Save();
            }
            else
            {
                if (textValue.Length > 0)
                {
                    textBox_VCP_PortraitOrdinalMoveTimeOffsetMultiplier.Text = $"{VCPresenceControler.PortraitMovementOrdinalTimeOffsetMultiplier}";
                }
            }
        }

        private void textBox_VCP_PortraitOrdinalRotTimeOffsetMultiplier_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (!OBSDeck.IsConnected)
            {
                return;
            }
            var textValue = textBox_VCP_PortraitOrdinalRotTimeOffsetMultiplier.Text.Replace('.', ',');
            if (textValue.EndsWith(','))
                return;
            if (float.TryParse(textValue, out float value))
            {
                textBox_VCP_PortraitOrdinalRotTimeOffsetMultiplier.Text = $"{value}";
                VCPresenceControler.PortraitRotationOrdinalTimeOffsetMultiplier = value;
                Settings.PortraitRotationOrdinalTimeOffsetMultiplier = VCPresenceControler.PortraitRotationOrdinalTimeOffsetMultiplier;
                Settings.Save();
            }
            else
            {
                if (textValue.Length > 0)
                {
                    textBox_VCP_PortraitOrdinalRotTimeOffsetMultiplier.Text = $"{VCPresenceControler.PortraitRotationOrdinalTimeOffsetMultiplier}";
                }
            }
        }
    }
}
