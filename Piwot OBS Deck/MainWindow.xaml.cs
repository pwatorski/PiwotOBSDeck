
using PiwotOBS;
using PiwotOBS.Structure;
using PiwotOBS.Structure.Animations;
using PiwotOBSDeck.VTuber;
using System;
using System.IO;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using PiwotOBSDeck.Donations.Events;
using PiwotOBSDeck.Donations;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Newtonsoft.Json.Linq;

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
        FrameAnimation beerAnim;
        Animator Animator = new Animator(30);
        DVDPong DVDPong;
        DonationRecieverService DonationRecService;
        List<string> donationScenes;
        List<string> donationTextItems;
        List<string> goalScenes;
        List<string> goalTextItems;
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
            DonationRecService = new DonationRecieverService();
            DonationRecService.OnClientConnected += DonationRecService_OnClientConnected;
            DonationRecService.OnClientDisconnected += DonationRecService_OnClientDisconnected;
            DonationRecService.OnDonation += DonationRecService_OnDonation;
            DonationRecService.OnConnectionFail += DonationRecService_OnConnectionFail;
            //VoiceDisplay.AddElement(new SerieWatcher(() => new float[] { VoiceCenter.CurrentVolume, VoiceCenter.CurrentVolume }, 1, segments:1, lineThickness: 1, color: Colors.Yellow));
            
            DonationDispatcher.Start();
            DonationDispatcher.OnDonationAdded += DonationDispatcher_OnDonationAdded;
            DonationDispatcher.OnDonationShown += DonationDispatcher_OnDonationShown;
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

        private void DonationRecService_OnConnectionFail(object? sender, EventArgs e)
        {
            Dispatcher.InvokeAsync(new Action(() => {
                textBlock_DonationConnectionStatus.Text = "disconnected";
            }));
        }

        private void DonationRecService_OnClientDisconnected(object? sender, OnDonationClientDisconnectedEventArgs e)
        {
            Dispatcher.InvokeAsync(new Action(() => {
                textBlock_DonationConnectionStatus.Text = "disconnected";
                groupBox_connectionsDonations.BorderBrush = new SolidColorBrush(DisconnectedColor);
                groupBox_connectionsDonations.Background = new SolidColorBrush(DisconnectedBackColor);
            }));
        }

        private void DonationRecService_OnDonation(object? sender, OnDonationEventArgs e)
        {
            DonationDispatcher.AddDonation(DonationRequest.FromWebRequest(e.DonationRequest));
            /*Dispatcher.InvokeAsync(new Action(() => {
                DonationRequest donationRequest = DonationRequest.FromWebRequest(e.DonationRequest);
                donationRequest.PlaySoundAsync(downloadIfMissing: true);
            }));
            */
            
        }

        private void DonationRecService_OnClientConnected(object? sender, OnDonationClientConnectedEventArgs e)
        {
            Dispatcher.InvokeAsync(new Action(() => {
                textBlock_DonationConnectionStatus.Text = "connected";
                groupBox_connectionsDonations.BorderBrush = new SolidColorBrush(ConnectedColor);
                groupBox_connectionsDonations.Background = new SolidColorBrush(ConnectedBackColor);
            }));
        }

        private void OnConnected(object? sender, EventArgs e)
        {
            Dispatcher.InvokeAsync(new Action(() =>
            {
                Console.WriteLine("Connected");
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

                    goalScenes = OBSDeck.GetSceneList().Select(x => x.Name).ToList();
                    comboBox_goalnScenes.ItemsSource = goalScenes;
                    comboBox_goalnScenes.Items.Refresh();

                    groupBox_connectionsOBS.BorderBrush = new SolidColorBrush(ConnectedColor);
                    groupBox_connectionsOBS.Background = new SolidColorBrush(ConnectedBackColor);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }

                groupBox_Trash.IsEnabled = true;
                groupBox_VTuber.IsEnabled = true;

            }));

            OBSStructure.Init();
            /*Avatar = new Avatar("XD_FACE_AVATAR", "BOB_NO_FACE")
            {
                Name = "BASIC_XD_BOB"
            };*/
            Avatar = Avatar.FromSave(Path.Combine(Settings.StoragePath_Avatars, "BASIC_XD_BOB"));
            Avatar.Reset();
            JumpscareImage = OBSStructure.RootScene.FindItem("JUMPSCARE_IMAGE");
            JumpscareSound = OBSStructure.RootScene.FindItem("VINE_BOOM");
            JumpscareImage2 = OBSStructure.RootScene.FindItem("RALSEI2");
            beerAnim = FrameAnimation.FromJson(@"G:\OBS\ANIMS\BEER_ANIM.json", OBSStructure.RootScene);
            Animator.RegisterAnimation(beerAnim);
            Animator.Run();
            
            
            DVDPong.Speed = Settings.DVDSpeed;
            Dispatcher.InvokeAsync(new Action(() =>
            {
                textBlock_BeerLoopStatus.Text = beerAnim.Loop ? "on" : "off";
                if (Settings.DonationSceneName != null)
                    TryGettingDonationScene();
                if (DonationDispatcher.DonationScene != null && Settings.DonationTextItemName != null)
                {
                    TryGettingDonationTextItem();
                }

                if (Settings.GoalSceneName != null)
                    TryGettingGoalScene();
                if (DonationDispatcher.GoalScene != null)
                {
                    if(Settings.GoalTextItemName != null)
                        TryGettingGoalTextItem();
                    if(Settings.GoalBarItemName != null)
                        TryGettingGoalBarItem();
                }

                DonationDispatcher.RefreshGoal();

            }));
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

        private void ButtonJumpscare_Click(object sender, RoutedEventArgs e)
        {
            Task task = new Task(
                () =>
                {
                    string fName = @"G:\OBS\RESOURCES\VINE_BOOM.mp3";
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
                    string fName = @"G:\OBS\RESOURCES\VINE_BOOM.mp3";
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
            if (DonationRecService.IsRunning)
            {
                DonationRecService.Stop();
            }
            else
            {
                textBlock_DonationConnectionStatus.Text = "connecting...";
                groupBox_connectionsDonations.BorderBrush = new SolidColorBrush(ConnectingColor);
                groupBox_connectionsDonations.Background = new SolidColorBrush(ConnectingBackColor);
                DonationRecService.Start();
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
            if(Settings.DonationSceneName == sceneName && Settings.DonationTextItemName != null)
            {
                if(donationTextItems.Contains(Settings.DonationTextItemName))
                {
                    comboBox_donationTexts.SelectedItem = Settings.DonationTextItemName;
                }
            }
            DonationDispatcher.DonationScene = scene;
            Settings.DonationSceneName = sceneName;
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

        private void textBox_dvdSpeed_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
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
                    string fName = @"G:\OBS\RESOURCES\SMOKE_DETECTOR.mp3";
                    MediaPlayer? mediaPlayer = new MediaPlayer();
                    mediaPlayer.Open(new Uri(fName));
                    mediaPlayer.Volume = 0.7;
                    mediaPlayer.Play();
                    Thread.Sleep(2500);
                    mediaPlayer.Stop();
                    mediaPlayer.Close();
                    mediaPlayer = null;
                }
                );
            task.Start();
        }

    }
}
