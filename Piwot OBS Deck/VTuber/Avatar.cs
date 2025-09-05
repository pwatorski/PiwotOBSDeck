using PiwotOBS.PMath;
using PiwotOBS.Structure;
using PiwotOBS.Structure.Animations;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Tab;

namespace PiwotOBSDeck.VTuber
{
    internal class Avatar
    {
        public string Name { get; set; } = "Unknown";
        [JsonIgnore]
        public SceneItem? CurBody { get; protected set; }
        public string CurBodyName { get; protected set; }
        [JsonIgnore]
        public SceneItem? CurFace { get; protected set; }
        public string CurFaceName { get; protected set; }
        [JsonIgnore]
        public Dictionary<string, SceneItem> Faces { get; protected set; }
        [JsonIgnore]
        public Dictionary<string, SceneItem> Bodies { get; protected set; }
        Animator animator = new Animator(60);
        ProceduralAnimation faceAnimation;
        public bool Enabled { get; protected set; }

        float curDirection;
        float maxRotation = 60;
        float rotationSetCut = 0.7f;
        float rotationResetCut = 0.4f;
        //TODO! v
        //public List<Expression> Expressions { get; protected set; }

        public Avatar(string name, string faceName, string bodyName)
        {
            Construct(name, new List<string>() { faceName }, new List<string>() { bodyName });
        }

        public Avatar(string name, List<string> faceNames, List<string> bodyNames)
        {
            Construct(name, faceNames, bodyNames);
        }

        protected void Construct(string avatarName, List<string> faceNames, List<string> bodyNames)
        {
            Name = avatarName;
            Faces = new Dictionary<string, SceneItem>();
            Bodies = new Dictionary<string, SceneItem>();
            foreach (string name in faceNames)
            {
                var obsObj = OBSStructure.RootScene?.FindItem(name);
                if (obsObj != null)
                {
                    Faces.Add(name, obsObj);
                }
            }

            foreach (string bodyName in bodyNames)
            {
                var obsObj = OBSStructure.RootScene?.FindItem(bodyName);
                if (obsObj != null)
                {
                    Bodies.Add(bodyName, obsObj);
                }
                
            }
            CurFaceName = Faces.Keys.First();
            CurBodyName = Bodies.Keys.First();
            CurFace = Faces[CurFaceName];
            CurBody = Bodies[CurBodyName];
            PrepareAnimator();
        }

        void PrepareAnimator()
        {
            animator.Stop();
            animator.DumpAnimations();
            if (CurFace != null)
            {
                faceAnimation = new ProceduralAnimation(CurFace, (float T, SceneItem x) =>
                {
                    return GetCurFaceTransform(T);
                });
                animator.RegisterAnimation(faceAnimation);
            }
        }

        protected AnimationTransform GetCurFaceTransform(float time)
        {

            if(time == 0)
            {
                return new AnimationTransform(CurFace);
            }
            var vol = VoiceCenter.CurrentVolume * 1.2f + 0.1f;
            var volSqrt = (float)(Math.Sqrt(VoiceCenter.CurrentVolume) * 1.2f + 0.1f);
            var scale = new Float2(
                    (float)Math.Abs(0.5 - volSqrt) + 0.5f,
                    (float)volSqrt) * CurFace.OBSScale;
            var rotation = GetTargetRotation(vol);
            return new AnimationTransform(
                CurFace,
                scale: Float2.Larp(CurFace.CurScale, scale, 0.5f),
                rotation: Arit.Larp(CurFace.CurRotation, rotation, 0.5f));
        }

        float DecideDirection(float val)
        {
            if (val > rotationSetCut && (curDirection == 0 || Rand.Int(10) == 0))
            {

                return Rand.Int(2) * 2 - 1;
            }
            if (val < rotationResetCut)
            {
                return 0;
            }
            return curDirection;
        }
        float GetTargetRotation(float val)
        {
            curDirection = DecideDirection(val);
            return maxRotation * curDirection * (1f - val);
        }

        public void Enable()
        {
            animator.Run();
            
            Enabled = true;
        }

        public void Disable()
        {
            animator.Reset();
            animator.Stop();
            Enabled = false;
        }

        public bool Toggle()
        {
            if (Enabled) Disable();
            else Enable();
            return Enabled;
        }

        public void CycleFace(bool next)
        {
            int nextIndex = Faces.Keys.ToList().IndexOf(CurFaceName);
            if (next)
            {
                nextIndex++;
            }
            else
            {
                nextIndex--;
            }
            SetFace(nextIndex);
        }

        public bool SetFace(int id)
        {
            int faceCount = Faces.Count;
            id %= faceCount;
            if (id < 0)
            {
                id += faceCount;
            }
            
            return SetFace(Faces.Keys.ToList()[id]);
        }

        public bool SetFace(string name)
        {
            if(!Faces.ContainsKey(name))
                return false;

            CurFace?.Disable();
            CurFace = Faces[name];
            CurFaceName = name;
            CurFace.Enable();
            var prevEnabled = Enabled;
            Reset();
            if (prevEnabled)
            {
                Enable();
            }
            return true;
        }

        public string ToJson()
        {
            return JsonSerializer.Serialize(this, Misc.JsonOptions);
        }

        public string ToCustomJson()
        {
            JsonArray faceNames = new();
            JsonArray bodyNames = new();
            foreach (var faceName in Faces.Keys)
            {
                faceNames.Add(faceName);
            }
            foreach (var bodyName in Bodies.Keys)
            {
                bodyNames.Add(bodyName);
            }
            JsonObject json = new()
            {
                { "name", Name },
                { "cur_face_name", CurFaceName },
                { "cur_body_name", CurBodyName },
                { "face_names", faceNames },
                { "body_names", bodyNames }
            };
            return json.ToJsonString(Misc.JsonOptions);
        }

        public static Avatar FromCustomJson(string jsonPath)
        {
            string jsonStr = "";
            using (StreamReader sr = new StreamReader(jsonPath, encoding: Encoding.UTF8))
            {
                jsonStr = sr.ReadToEnd();
            }
            JsonDocument jsonDoc = JsonDocument.Parse(jsonStr);
            JsonElement root = jsonDoc.RootElement;
            string name = root.GetProperty("name").GetString() ?? "Unknown";
            List<string> faceNames = root.GetProperty("face_names").EnumerateArray().Select(x=>x.GetString()).Where(x=>x!=null).ToList();
            List<string> bodyNames = root.GetProperty("body_names").EnumerateArray().Select(x => x.GetString()).Where(x => x != null).ToList();
            Avatar newAvatar = new Avatar(name, faceNames, bodyNames);

            return newAvatar ?? throw new Exception($"Avatar could not be loaded from file \"{jsonPath}\"");
        }

        public void Save(string directory)
        {
            string dirPath = Path.Combine(directory, $"{Name}");
            SaveDefinition( directory );
            foreach (var pair in Faces)
            {
                pair.Value.Save(dirPath, $"{pair.Key}.json");
            }
            foreach (var pair in Bodies)
            {
                pair.Value.Save(dirPath, $"{pair.Key}.json");
            }
        }

        public void SaveDefinition(string directory)
        {
            string dirPath = Path.Combine(directory, $"{Name}");
            if (!Directory.Exists(dirPath))
                Directory.CreateDirectory(dirPath);
            string filePath = Path.Combine(dirPath, "definition.json");
            using StreamWriter sw = new(filePath, append: false, encoding: Encoding.UTF8);
            sw.WriteLine(ToJson());

            filePath = Path.Combine(dirPath, "definition_custom.json");
            using StreamWriter sw2 = new(filePath, append: false, encoding: Encoding.UTF8);
            sw2.WriteLine(ToCustomJson());
        }

        public static Avatar FromJson(string jsonPath)
        {
            string jsonStr = "";
            using (StreamReader sr = new StreamReader(jsonPath, encoding: Encoding.UTF8))
            {
                jsonStr = sr.ReadToEnd();
            }
            JsonDocument jsonDoc = JsonDocument.Parse(jsonStr);
            JsonElement root = jsonDoc.RootElement;
            string name = root.GetProperty("Name").GetString() ?? "Unknown";
            string bodyName = root.GetProperty("CurBodyName").GetString() ?? "Unknown";
            string faceName = root.GetProperty("CurFaceName").GetString() ?? "Unknown";
            Avatar newAvatar = new Avatar(name, faceName, bodyName);
            return newAvatar ?? throw new Exception($"Avatar could not be loaded from file \"{jsonPath}\"");
        }

        public static Avatar FromSave(string directory)
        {
            string definitionPath = Path.Combine(directory, "definition_custom.json");
            Avatar avatar;
            try
            {
                avatar = Avatar.FromCustomJson(definitionPath);
            }
            catch
            {
                definitionPath = Path.Combine(directory, "definition.json");
                avatar = Avatar.FromJson(definitionPath);
            }
            avatar.UpdateFromSave(directory);
            return avatar;
        }

        private void UpdateFromSave(string directory)
        {
            CurFace?.OverrideFromSave(Path.Combine(directory, $"{CurFaceName}.json"));
            CurBody?.OverrideFromSave(Path.Combine(directory, $"{CurBodyName}.json"));
            foreach (var pair in Faces)
            {
                pair.Value.OverrideFromSave(Path.Combine(directory, $"{pair.Key}.json"));
            }
            foreach (var pair in Bodies)
            {
                pair.Value.OverrideFromSave(Path.Combine(directory, $"{pair.Key}.json"));
            }
        }

        internal void UpdateFromOBS()
        {
            CurFace?.UpdateFromOBS();   
            CurBody?.UpdateFromOBS();
            foreach (var pair in Faces)
            {
                pair.Value.UpdateFromOBS();
            }
            foreach (var pair in Bodies)
            {
                pair.Value.UpdateFromOBS();
            }
        }

        internal void Reset()
        {
            PrepareAnimator();
            foreach (var pair in Faces)
            {
                pair.Value.ResetOBSToCur();
                if(pair.Key != CurFaceName)
                {
                    pair.Value.Disable();
                }
                else
                {
                    pair.Value.Enable();
                }
            }
            foreach (var pair in Bodies)
            {
                pair.Value.ResetOBSToCur();
            }
        }

        public void SetFaces(List<string> list)
        {
            var prevEnabled = Enabled;
            Construct(Name, list, Bodies.Keys.ToList());
            if (prevEnabled)
            {
                Enable();
            }
        }
    }
}
