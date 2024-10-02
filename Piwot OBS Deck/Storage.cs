using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;



namespace PiwotOBSDeck
{
    public static class Storage
    {
        static readonly string AppdataDir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        static readonly string StorageDir = Path.Combine(AppdataDir, "PiwotOBSDeck");
        static readonly string SettingsDir = Path.Combine(StorageDir, "settings");

        static Storage() 
        {
            if (!Directory.Exists(StorageDir))
                Directory.CreateDirectory(StorageDir);

            if (!Directory.Exists(SettingsDir))
                Directory.CreateDirectory(StorageDir);
        }

        public static SettingsBatch LoadSettings(string name)
        {
            if (!name.EndsWith(".set"))
                name += ".set";
            return new SettingsBatch(GetFilenameInSettings(name));
        }

        public static void SaveSettings(SettingsBatch settings, string name, bool createDirs=true)
        {
            settings.Save(GetSettingsFilename(name), createDirs: createDirs);
        }

        public static string GetFilenameInStorage(string filename)
        {
            return Path.Combine(StorageDir, filename);
        }

        public static string GetFilenameInSettings(string filename)
        {
            return Path.Combine(SettingsDir, filename);
        }

        public static string GetSettingsFilename(string name)
        {
            if (!name.EndsWith(".set"))
                name += ".set";
            return GetFilenameInSettings(name);
        }


    }

    public class SettingsBatch
    {
        Dictionary<string, object> settings = new Dictionary<string, object>();
        static readonly char SepChar = (char)30;
        static readonly char NlnChar = (char)31;
        protected string? SourcePath;

        public SettingsBatch()
        {

        }
        public SettingsBatch(string filePath)
        {
            SourcePath = filePath;
            settings = new Dictionary<string, object>();
            if (File.Exists(SourcePath)) 
            {
                using var sr = new StreamReader(filePath);
                foreach (var line in sr.ReadToEnd().Split('\n'))
                {
                
                    var sline = line.Split(SepChar);
                    if (sline.Length == 3)
                    {
                        var key = sline[0];
                        var value = TryParse(sline[1], sline[2].Trim());
                        settings.Add(key, value);
                    }
                }
            }
        }

        public void Save(string? filePath=null, bool createDirs=true)
        {
            filePath ??= SourcePath;
            if(createDirs && !Directory.Exists(Path.GetDirectoryName(filePath)))
            {
                Directory.CreateDirectory($"{Path.GetDirectoryName(filePath)}");
            }
            if (filePath == null) throw new ArgumentNullException(nameof(filePath));
            using var sw = new StreamWriter(filePath);
            string value;
            foreach(var pair in settings) 
            {
                var valueObject = pair.Value;
                if (valueObject is string)
                {
                    value = ProcessStringOut((string)pair.Value);
                }
                else if (valueObject is DateTime)
                {
                    value = $"{((DateTime)valueObject).ToBinary()}";
                }
                else if (valueObject is Boolean)
                {
                    value = ((bool)valueObject)?"true":"false";
                }
                else
                {
                    value = $"{pair.Value}";
                }
                sw.WriteLine($"{pair.Key}{SepChar}{valueObject.GetType().Name}{SepChar}{value}");
            }

        }


        protected object TryParse(string typeName, string value)
        {
            switch (typeName)
            {
                case "Int32":
                    if (int.TryParse(value, out var intVal)) return intVal;
                    break;
                case "Single":
                    if (float.TryParse(value, out var floatVal)) return floatVal;
                    break;
                case "Double": 
                    if (double.TryParse(value, out var doubleVal)) return doubleVal;
                    break;
                case "DateTime":
                    if (long.TryParse(value.Trim(), out var longVal)) return DateTime.FromBinary(longVal);
                    break;
                case "Boolean":
                    if (value == "true") return true;
                    if (value == "false") return false;
                    break;
                default:
                    return ProcessStringIn(value);
            }
            return ProcessStringIn(value);
        }
        protected string ProcessStringOut(string str)
        {
            return str.Replace('\n', NlnChar).Replace('\r', NlnChar);
        }

        protected string ProcessStringIn(string str)
        {
            return str.Trim().Replace(NlnChar, '\n');
        }

        public T? GetVal<T>(string key, object? defVal=null)
        {
            if (settings.TryGetValue(key, out var val))
            {
                return (T)val;
            }
            return (T?)defVal;
        }

        public bool SetVal(string key, object val)
        {
            if(settings.ContainsKey(key))
                settings[key] = val;
            return false;
        }
        public bool RemoveVal(string key)
        {
            return settings.Remove(key);
        }

        public void AddOrSet(string key, object val)
        {
            if(settings.ContainsKey(key)) 
                settings[key] = val;
            else settings.Add(key, val);
        }

        public void Clear()
        {
            settings.Clear();
        }

        public override string ToString()
        {
            return string.Join("\n", settings.AsEnumerable().Select(x => $"{x.Key}: {x.Value}"));
        }

        public object? this[string key]
        {
            get
            {
                if(settings.ContainsKey(key))
                    return settings[key];
                return null;
            }

            set
            {
                if(value != null)
                    AddOrSet(key, value);
            }
        }
    }
}
