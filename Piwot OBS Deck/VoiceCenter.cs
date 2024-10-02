using PiwotOBS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiwotOBSDeck
{
    public static class VoiceCenter
    {
        private static VoiceMeter VoiceMeter;

        public static float CurrentVolume { get => VoiceMeter.CurVolume * VolumeMultiplier; }
        public static float AvgVolume { get => VoiceMeter.VolumeRecordAvg; }
        public static bool Enabled { get => VoiceMeter.Enabled; }
        public static List<float> VolumeHistory { get => VoiceMeter.VolumeHistory; }
        public static List<float> AvgVolumeHistory { get => VoiceMeter.AvgVolumeHistory; }
        public static event EventHandler<Tuple<float, float>>? VolumeUpdate;
        public static float VolumeMultiplier { get; set; } = 1.0f;

        static VoiceCenter()
        {
            VoiceMeter = new VoiceMeter();
            VoiceMeter.VolumeUpdate += (x, y) => { VolumeUpdate?.Invoke(x, y); };
        }

        public static void Enable()
        {
            VoiceMeter.Start();
        }

        public static void Disable()
        {
            VoiceMeter.Stop();
        }
    }
}
