using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace PiwotOBSDeck
{

    public static class TTSCenter
    {
        public static string VoiceScript = "from gtts import gTTS\r\nimport sys\r\n\r\ndef tts(text:str, save_path:str, lang:str='pl'):\r\n    try:\r\n        audio = gTTS(text=text, lang=lang, slow=False)\r\n        \r\n        audio.save(save_path)\r\n    except Exception as ex:\r\n        print(f\"{ex}\")\r\n    print(\"success\")\r\n\r\nif __name__ == '__main__':\r\n    tts(sys.argv[1], sys.argv[2], sys.argv[3])";
        static string VoiceScriptPath { get => Path.Join(Settings.MainPath, "voice_script.py"); }
        static string LanguageDictPath { get => Path.Join(Settings.MainPath, "tts_languages.json"); }
        public static Dictionary<string, string> Languages { get; private set; }
        static TTSCenter()
        {
            if (Path.Exists(VoiceScriptPath))
            {
                File.Delete(VoiceScriptPath);
            }
            using StreamWriter sw = new StreamWriter(VoiceScriptPath, false, encoding: Encoding.UTF8);
            sw.Write(VoiceScript);
            using StreamReader sr = new StreamReader(LanguageDictPath);
            JsonObject langDictRaw = JsonNode.Parse(sr.ReadToEnd()).AsObject();
            Languages = new Dictionary<string, string>();
            foreach (var lang in langDictRaw)
            {
                var val = lang.Value?.GetValue<string>();
                Languages.Add(lang.Key, val ?? "pl");
            }
        }

        public static bool DownloadTTS(string text, string target, string lang = "pl")
        {
            if(!Languages.ContainsKey(lang))
            {
                lang = "pl";
            }

            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = "python";
            start.Arguments = string.Format("{0} \"{1}\" \"{2}\" \"{3}\"", VoiceScriptPath, text, target, lang);
            start.UseShellExecute = false;
            start.RedirectStandardOutput = true;
            using Process process = Process.Start(start);
            using StreamReader reader = process.StandardOutput;
            string result = reader.ReadToEnd().Trim();
            if (result != "success")
            {
                throw new Exception($"Python exception:\n{result}");
            }
            return true;
        }
    }
}
