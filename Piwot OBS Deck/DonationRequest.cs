using NAudio.Wave;
using PiwotOBS;
using PiwotOBS.Structure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace PiwotOBSDeck
{
    public  class DonationRequest
    {
        public string AuthorName { get; }
        public string Text { get; private set; }
        public double Value { get; }

        public long ID { get; }
        public long AuthorID { get; }
        public TimeSpan? Duration { get; set; }
        public string TargetPath { get; }
        public string DonateVerb { get => FemaleAuthor ? "PRZEKAZAŁA" : "PRZEKAZAŁ"; }
        public string HeaderText { get => $"{AuthorName} {DonateVerb} [{Value}zł]"; }
        public string DisplayText { get => $"{HeaderText}\n{ParseTextToLineWidth()}"; }
        public string ReadText { get => $"{HeaderText}\n{Text.Replace("TROLLFACE'A", "trolfejsa")}"; }
        public string Lang { get; set; } = "auto";
        public JsonNode? Meta { get; set; }
        public bool FemaleAuthor { get; set; } = false;
        public bool HasMeta { get => Meta != null; }
        public bool IsVivi { get; set; } = false;

        IWavePlayer? waveOutDevice;
        AudioFileReader? audioFileReader;
        public DonationRequest(string authorName, string text, double value, long id, long authorID, JsonNode? meta = null)
        {
            AuthorName = authorName;
            Text = text;
            Value = value;
            ID = id;
            TargetPath = Path.Join(Directory.GetCurrentDirectory(), $"{ID}.mp3");
            AuthorID = authorID;
            Meta = meta;
            if(Meta != null )
            {
                IsVivi = Meta["isVivi"]?.GetValue<bool>() ?? false;
                FemaleAuthor = (Meta["authorSex"]?.GetValue<string>() ?? "") == "F";
                FemaleAuthor = FemaleAuthor || IsVivi;
            }
            ParseTextForLang();
        }

        protected void ParseTextForLang()
        {
            var words = Text.Split(' ').SkipWhile(x => x.Length == 0);
            foreach (var word in words)
            {
                var tempWord = word.Trim();
                if (tempWord.StartsWith("{lang:") && tempWord.EndsWith("}"))
                {
                    tempWord = tempWord.Substring(6, tempWord.Length - 7);
                    if(TTSCenter.Languages.ContainsKey(tempWord))
                        Lang = tempWord;
                    Text = Text.Replace(word, "");
                }
            }
        }

        private string ParseTextToLineWidth()
        {
            var words = Text.Split(' ').SkipWhile(x=>x.Length == 0);
            string finalText = "";
            string line = "";
            foreach(var word in words)
            {
                if (line.Length + word.Length <= Settings.MaxDonationTextWidth)
                {
                    line += $" {word}";
                }
                else
                {
                    finalText += $"{line}\n";
                    line = word;
                }
            }
            finalText += line;
            finalText = finalText.Trim();
            return finalText;
        }

        public bool Download(string lang="pl")
        {
            
            try
            {
                TTSCenter.DownloadTTS(ReadText, TargetPath, lang);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            
            IWavePlayer waveOutDevice = new WaveOut();
            AudioFileReader audioFileReader = new AudioFileReader(TargetPath);
            Duration = audioFileReader.TotalTime;
            audioFileReader.Dispose();
            waveOutDevice.Dispose();
            return true;
            
        }
        
        public void PlaySound()
        {
            if(Duration == null)
            {
                throw new Exception("Cannot play. TTS is not downloaded!");
            }

            SyncPlay();
        }

        public TimeSpan PlaySoundAsync(bool downloadIfMissing=false)
        {
            if (Duration == null)
            {
                if(downloadIfMissing)
                {
                    Download();
                }
                else
                {
                    throw new Exception("Cannot play. TTS is not downloaded!");
                }
            }
            Thread thread = new Thread(SyncPlay);
            thread.Start();
            return (TimeSpan)Duration;
            
        }
        protected void SyncPlay()
        {
            //SceneItem sceneItem = OBSStructure.RootScene.FindItem("DONATION_SCENE");
            //ItemText textItem = OBSStructure.RootScene.FindItem("DONATION_TEXT") as ItemText;
            //textItem.SetText(DisplayText);
            // sceneItem.Enable();
            waveOutDevice = new WaveOut();
            audioFileReader = new AudioFileReader(TargetPath);
            waveOutDevice.Init(audioFileReader);
            waveOutDevice.Play();
            Thread.Sleep((TimeSpan)Duration);
            waveOutDevice?.Stop();
            audioFileReader?.Dispose();
            waveOutDevice?.Dispose();
            waveOutDevice = null;
            audioFileReader = null;
            //sceneItem.Disable();
        }



        public static DonationRequest FromWebRequest(Donations.WebRequests.DonationRequest source) 
        {
            return new DonationRequest(source.AuthorName, source.Text, source.Value, source.ID, source.AuthorID, source.Meta);
        }

        internal void Stop()
        {
            if (waveOutDevice == null || audioFileReader == null)
                return;
            waveOutDevice?.Stop();
            audioFileReader?.Dispose();
            waveOutDevice?.Dispose();
            waveOutDevice = null;
            audioFileReader = null;
        }
    }
}
