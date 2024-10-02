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
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Tab;

namespace PiwotOBSDeck.VTuber
{
    internal class Avatar
    {
        public string Name { get; set; } = "Unknown";
        [JsonIgnore]
        public SceneItem? Body { get; protected set; }
        public string BodyName { get; protected set; }
        [JsonIgnore]
        public SceneItem? Face { get; protected set; }
        public string FaceName { get; protected set; }
        Animator animator = new Animator(60);

        public bool Enabled { get; protected set; }

        float curDirection;
        float maxRotation = 60;
        float rotationSetCut = 0.7f;
        float rotationResetCut = 0.4f;
        //TODO! v
        //public List<Expression> Expressions { get; protected set; }

        public Avatar(string faceName, string bodyName)
        {
            FaceName = faceName;
            BodyName = bodyName;
            Face = OBSStructure.RootScene?.FindItem(FaceName);
            Body = OBSStructure.RootScene?.FindItem(BodyName);
            PrepareAnimator();
        }

        void PrepareAnimator()
        {
            animator.Stop();
            animator.DumpAnimations();
            if (Face != null)
            {
                ProceduralAnimation proceduralAnimation = new ProceduralAnimation(Face, (float T, SceneItem x) =>
                {
                    return GetCurFaceTransform(T);
                });
                animator.RegisterAnimation(proceduralAnimation);
            }
        }

        protected AnimationTransform GetCurFaceTransform(float time)
        {
            if(time == 0)
            {
                return new AnimationTransform(Face);
            }
            var vol = VoiceCenter.CurrentVolume * 1.2f + 0.1f;
            var volSqrt = (float)(Math.Sqrt(VoiceCenter.CurrentVolume) * 1.2f + 0.1f);
            var scale = new Float2(
                    (float)Math.Abs(0.5 - volSqrt) + 0.5f,
                    (float)volSqrt) * Face.OBSScale;
            var rotation = GetTargetRotation(vol);
            return new AnimationTransform(
                Face,
                scale: Float2.Larp(Face.CurScale, scale, 0.5f),
                rotation: Arit.Larp(Face.CurRotation, rotation, 0.5f));
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

        public string ToJson()
        {
            return JsonSerializer.Serialize(this, Misc.JsonOptions);
        }

        public void Save(string directory)
        {
            string dirPath = Path.Combine(directory, $"{Name}");
            if(!Directory.Exists(dirPath))
                Directory.CreateDirectory(dirPath);
            string filePath = Path.Combine(dirPath, "definition.json");
            using StreamWriter sw = new(filePath, append: false, encoding: Encoding.UTF8);
            sw.WriteLine(ToJson());
            Face.Save(dirPath, $"{FaceName}.json");
            Body.Save(dirPath, $"{BodyName}.json");
        }

        public static Avatar FromJson(string jsonPath)
        {
            string json = "";
            using (StreamReader sr = new StreamReader(jsonPath, encoding: Encoding.UTF8))
            {
                json = sr.ReadToEnd();
            }
            Avatar? newAvatar = JsonSerializer.Deserialize<Avatar>(json, Misc.JsonOptions);

            return newAvatar ?? throw new Exception($"Avatar could not be loaded from file \"{jsonPath}\"");
        }

        public static Avatar FromSave(string directory)
        {
            string definitionPath = Path.Combine(directory, "definition.json");
            Avatar avatar = Avatar.FromJson(definitionPath);
            avatar.UpdateFromSave(directory);
            return avatar;
        }

        private void UpdateFromSave(string directory)
        {
            Face.OverrideFromSave(Path.Combine(directory, $"{FaceName}.json"));
            Body.OverrideFromSave(Path.Combine(directory, $"{BodyName}.json"));
        }

        internal void UpdateFromOBS()
        {
            Face?.UpdateFromOBS();   
            Body?.UpdateFromOBS();
        }

        internal void Reset()
        {
            PrepareAnimator();

            Face?.ResetOBSToCur();
            Body?.ResetOBSToCur();
        }
    }
}
