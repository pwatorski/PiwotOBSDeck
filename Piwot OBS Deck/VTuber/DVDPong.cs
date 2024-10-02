using PiwotOBS;
using PiwotOBS.PMath;
using PiwotOBS.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace PiwotOBSDeck.VTuber
{
    public class DVDPong
    {
        protected SceneItem? Item;
        protected Scene? scene;
        Float2 SceneSize;
        Float2 BoundingSize;
        Float2 ItemSize;
        public int FPS { get => fps; set { fps = value; msPerFrame = 1000 / fps; } }
        int fps;
        int msPerFrame;
        public Float2 Direction { 
            get
            {
                return direction;
            }
            set
            {
                direction = value / (float)Math.Sqrt(value.X * value.X + value.Y * value.Y);
            }
        }
        public float Speed { get; set; } = 20;

        Float2 direction = new Float2(2, 1);

        public Float2 MoveVector { get=>direction * Speed; 
            set
            {
                Speed = (float)Math.Sqrt(value.X * value.X + value.Y * value.Y);
                direction = value / Speed;
            }
        }

        Thread? thread;
        protected bool PauseThread;
        protected bool StopThread = true;
        public bool IsRunning { get => !StopThread; }
        public bool IsPaused { get => PauseThread; }
        protected int hueShift = 0;
        public DVDPong()
        {
            FPS = 30;
        }

        void InitItem()
        {
            Item = OBSStructure.RootScene?.FindItem("DVD_VIDEO");
            scene = (Scene?)OBSStructure.RootScene?.FindItem("DVD_SCENE");
            ItemSize = Item?.OBSSize??new Float2(10);
            SceneSize = new Float2(1920, 1080);
            BoundingSize = SceneSize - ItemSize;
        }
        public void Start()
        {
            if(Item==null)
            {
                InitItem();
            }
            scene?.Enable();
            scene?.MoveToTop();
            if (thread == null)
            {
                StopThread = false;
                thread = new Thread(Run);
                thread.Start();
            }
            else
            {
                PauseThread = false;
            }
        }
        public void Stop()
        {
            StopThread = true;
            if (thread != null)
            {
                thread.Join();
                thread = null;
            }
            scene?.Disable();
        }

        public void Pause()
        {
            PauseThread = true;
            scene?.Disable();
        }

        void Run()
        {
            while (!StopThread) 
            { 
                if(!PauseThread)
                    Step(); 
                Thread.Sleep(msPerFrame); 
            }
        }

        public void Step()
        {
            if (Item == null)
                return;
            Float2 newPos = Item.CurPosition + MoveVector;
            if (newPos.X > BoundingSize.X)
            {
                newPos.X -= newPos.X - BoundingSize.X;
                direction.X = -direction.X;
                hueShift += 30;
            }

            if (newPos.Y > BoundingSize.Y)
            {
                newPos.Y -= newPos.Y - BoundingSize.Y;
                direction.Y = -direction.Y;
                hueShift += 30;
            }

            if (newPos.X < 0)
            {
                newPos.X -= newPos.X;
                direction.X = -direction.X;
                hueShift += 30;
            }

            if (newPos.Y < 0)
            {
                newPos.Y -= newPos.Y;
                direction.Y = -direction.Y;
                hueShift += 30;
            }
            Item.TransformObject(newPos: newPos);
            if(hueShift > 180)
            {
                hueShift = -(hueShift - 180);
            }
            OBSDeck.OBS.SetSourceFilterSettings(Item.Name, "COLOR_CORRECTION", new JsonObject() { { "hue_shift", hueShift } });
        }
    }
}
