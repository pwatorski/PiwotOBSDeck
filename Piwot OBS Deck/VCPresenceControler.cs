using PiwotOBS.PMath;
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
            PortraitAnimation = new ProceduralAnimation(ItemImage, (t, i)=>VCPresenceControler.GetAnimationFrame(t, i, Ordinal));
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

        public static Float2 PortraitSize { get; set; } = new Float2(150);
        public static Float2 PortraitScreenAnchorPosition { get; set; } = new Float2(100, 1080);
        public static Float2 PortraitRelativeOrdinalOffset { get; set; } = new Float2(160, 0);
        public static Float2 PortraitMovementMagnitude { get; set; } = new Float2(15, 0);
        public static float PortraitRotationMagnitude { get; set; } = 4;
        public static float PortraitMovementTimePeriod { get=> portraitMovementTimePeriod; set { if (value != 0) portraitMovementTimePeriod = value; else portraitMovementTimePeriod = 0.0001f; } }
        static float portraitMovementTimePeriod = 1;
        public static float PortraitRotationTimePeriod { get => portraitRotationTimePeriod; set { if (value != 0) portraitRotationTimePeriod = value; else portraitRotationTimePeriod = 0.0001f; } }
        static float portraitRotationTimePeriod = 1;

        public static float PortraitMovementTimeOffset { get; set; } = 1;
        public static float PortraitRotationTimeOffset { get; set; } = 1;

        public static float PortraitMovementOrdinalTimeOffsetMultiplier { get; set; } = 0.1f;
        public static float PortraitRotationOrdinalTimeOffsetMultiplier { get; set; } = 0.1f;

        public static AnimationTransform GetAnimationFrame(float time, SceneItem itemImage, int ordinal)
        {
            float doublePI = MathF.PI * 2;
            float timeOffsetMovement = PortraitMovementTimeOffset + ordinal * PortraitMovementOrdinalTimeOffsetMultiplier;
            float timeOffsetRotation = PortraitRotationTimeOffset + ordinal * PortraitRotationOrdinalTimeOffsetMultiplier;

            float movementMultiplier = MathF.Sin((time / PortraitMovementTimePeriod - timeOffsetMovement) * doublePI);
            float rotationMultiplier = MathF.Sin((time / PortraitRotationTimePeriod - timeOffsetRotation) * doublePI);

            var position = PortraitScreenAnchorPosition + PortraitRelativeOrdinalOffset * (float)ordinal + PortraitMovementMagnitude * movementMultiplier;
            return new AnimationTransform(itemImage, position: position, rotation: rotationMultiplier * PortraitRotationMagnitude, scale: PortraitSize / 160f
                );
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
