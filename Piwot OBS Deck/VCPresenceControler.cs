using PiwotOBS.Structure;
using PiwotOBS.Structure.Animations;
using PiwotOBSDeck.WebServices;
using PiwotOBSDeck.WebServices.WebRequests;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiwotOBSDeck
{
    public class VCPresencePortrait
    {
        public bool Enabled { get; protected set; }
        public bool Ready { get; protected set; }
        public Animator? Animator { get; protected set; }
        public ItemImage? ItemImage
        {
            get=>itemImage; protected set
            {
                itemImage = value;
                if (itemImage != null)
                    Ready = true;
            } }
        ItemImage? itemImage;
        public string? Path { get; protected set; }
        public string? ID { get; protected set; }
        public int Ordinal { get; protected set; }
        public ProceduralAnimation PortraitAnimation { get; protected set; }


        public VCPresencePortrait(ItemImage itemImage, int ordinal, string? id = null, string? path = null, Animator? animator=null)
        {
            Ordinal = ordinal;
            ID = id;
            Path = path;
            ItemImage = itemImage;
            Animator = animator;
            PortraitAnimation = new ProceduralAnimation(ItemImage, GetAnimationFrame);
        }

        AnimationTransform GetAnimationFrame(float time, SceneItem itemImage)
        {
            float doublePI = MathF.PI * 2;
            float offsetA = Ordinal/10.0f;
            float offsetB = offsetA;
            float period = 4;
            float A = MathF.Sin((time / period - offsetA) * doublePI);
            float B = MathF.Sin((time / period - offsetB) * doublePI);

            return new AnimationTransform(itemImage, position: new PiwotOBS.PMath.Float2(100 + 160 * Ordinal + 15 * A, 1080), size: new PiwotOBS.PMath.Float2(162), rotation: B*3.0f
                );
        }

        public void SetPresence(string id, string path)
        {
            if (!Ready)
                return;
            ID = id;
            Path = path;
            itemImage?.Enable();
            itemImage?.SetSource(path);
            Enabled = true;
            Animator?.RegisterAnimation(PortraitAnimation);
        }

        public void Disable()
        {
            Animator?.UnRegisterAnimation(PortraitAnimation);
            ID = null; Path = null;
            Enabled = false;
            itemImage?.Disable();
        }

    }
    public static class VCPresenceControler
    {
        static public Scene? Scene { get; private set; }
        static public List<VCPresencePortrait> PortraitItems { get; private set; } = new List<VCPresencePortrait>();
        public static bool Enabled { get; private set; }
        static Animator Animator;
        public static DateTime LastUpdate { get; private set; } = DateTime.MinValue;
        static VCPresenceControler()
        {
            Animator = new Animator(24);
            
        }



        public static void UpdateItems(Scene? scene, List<ItemImage> itemImages)
            
        {
            if (scene == null)
            {
                scene = null;
                PortraitItems = new List<VCPresencePortrait>();
                return;
            }
            Scene = scene;
            itemImages = itemImages.OrderBy((x) => int.Parse(x.Name.Split('_').Last())).ToList();
            PortraitItems = new List<VCPresencePortrait>();
            for (int i = 0; i < itemImages.Count; i++)
            {
                var image = itemImages[i];
                PortraitItems.Add(new VCPresencePortrait(image, i, animator:Animator));
            }
            
            
        }
        public static void UpdatePresence(VCPresenceUpdateRequest request)
        {
            if (request == null)
            {
                return;
            }
            UpdatePresence(request.Ids, request.StoragePath, request.Timestamp);
        }

        public static void UpdatePresence(string[] memberIds, string avatarStoragePath, double timestamp)
        {
            LastUpdate = DateTimeOffset.FromUnixTimeSeconds((long)timestamp)
                                              .UtcDateTime
                                              .AddSeconds(timestamp % 1);
            if (Scene ==  null) { return; }
            Array.Sort(memberIds);
            
            for (int i = 0; i < memberIds.Length && i < PortraitItems.Count; i++)
            {
                var memberId = memberIds[i];
                string avatar = Path.Combine(avatarStoragePath, $"{memberId}.png");
                PortraitItems[i].SetPresence(memberId, avatar);
            }

            for (int i = memberIds.Length; i < PortraitItems.Count; i++)
            {
                PortraitItems[i].Disable();
            }
            
        }
        internal static bool Toggle(bool status)
        {
            if (Scene == null)
            { return false; }
            if (Scene.CurEnabled == status)
                return status;

            if (status)
            {
                Scene.Enable();
                Animator.Run();                
            }
            else
            {
                Scene.Disable();
                Animator.Stop();
                Animator.Reset();
            }
            Enabled = status;
            return Scene.CurEnabled;
        }
        internal static bool Toggle()
        {
            return Toggle(!Enabled);
        }
    }
}
