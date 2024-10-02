using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Windows.Forms.LinkLabel;

namespace PiwotOBSDeck
{

    public class SerieElement
    {
        protected Line[] Lines { get; set; }
        public SerieElement(Line[] lines) 
        {
            Lines = lines;
        }
        public virtual void UpdateLines(double width, double height)
        {
            throw new NotImplementedException();
        }
        public void Register(UIElementCollection targetCollection) 
        {
            if (Lines == null)
                return;
            foreach(var l in Lines)
            {
                targetCollection.Add(l);
            }
        }
    }
    public class SerieLine: SerieElement
    {
        protected Line line;
        protected double position;
        protected double lastWidth;
        protected double lastHeight;
        protected double lastPosition;
        public SerieLine(double position, double thickness, Color color): base(new Line[1])
        {
            line = new Line() 
            {
                Stroke = new SolidColorBrush(color),
                StrokeThickness = thickness
            };
            Lines[0] = line;
            this.position = position;
        }
        protected bool CheckIfSthChanged(double width, double height)
        {
            return !(
                position == lastPosition
                && width == lastWidth
                && height == lastHeight
                );
        }

        protected void UpdateLastVals(double width, double height)
        {
            lastPosition = position;
            lastWidth = width;
            lastHeight = height;
        }
    }
    public class SerieHLine : SerieLine
    {
        public double X { get => position; set => position = value; }
        public SerieHLine(double position, double thickness, Color color) : base(position, thickness, color)
        {
        }
        public override void UpdateLines(double width, double height)
        {
            if (CheckIfSthChanged(width, height))
            {
                line.X1 = 0;
                line.X2 = width;
                line.Y1 = (1 - position) * height;
                line.Y2 = (1 - position) * height;
                UpdateLastVals(width, height);
            }
        }
    }
    public class SerieVLine : SerieLine
    {
        public double Y { get => position; set => position = value; }
        public SerieVLine(double position, double thickness, Color color) : base(position, thickness, color)
        {
        }
        public override void UpdateLines(double width, double height)
        {
            if (CheckIfSthChanged(width, height))
            {
                line.Y1 = 0;
                line.Y2 = height;
                line.X1 = (1 - position) * width;
                line.X2 = (1 - position) * width;
                UpdateLastVals(width, height);
            }
        }
    }
    public class SerieWatcher: SerieElement
    {
        protected Func<float[]> GetValsFunc;
        protected float scale;
        
        protected int segments;
        protected bool verticalPositionsSet = false;
        public SolidColorBrush BaseColor { get; set; }
        public SolidColorBrush? HighColor { get; set; }

        public SerieWatcher(Func<float[]> func, float scale, int segments=100, double lineThickness=1, Color color=default): base(new Line[segments])
        {
            if(color == default)
            {
                color = Colors.Black;
            }
            GetValsFunc = func;
            this.scale = scale;
            this.segments = segments;
            Lines = new Line[segments];
            BaseColor = new SolidColorBrush(color);
            for (int i = 0; i < segments; i++)
            {
                var line = new Line()
                {
                    StrokeThickness = lineThickness,
                    Stroke = BaseColor

                };
                Lines[i] = line;
            }
        }
        public override void UpdateLines(double width, double height)
        {
            if (Lines == null)
                return;
            var values = GetValsFunc.Invoke();
            var valBorder = Math.Max(0, Math.Min(values.Length - 1, segments));
            var startY = height;
            Line line;
            
            for (int i = 0; i < valBorder; i++)
            {
                line = Lines[i];
                var starVal = values[i] / scale;
                line.Y1 = startY * (1 - starVal);
                line.Y2 = startY * (1 - values[i + 1] / scale);
                line.X1 = width * ((double)i / segments);
                line.X2 = width * ((double)(i + 1) / segments);
                if(starVal > 0.9)
                {
                    line.Stroke = HighColor??BaseColor;
                }
                else
                {
                    line.Stroke = BaseColor;
                }

            }
            for (int i = valBorder; i < segments; i++)
            {
                line = Lines[i];
                line.Y1 = startY;
                line.Y2 = startY;
                line.X1 = width * (i / segments);
                line.X2 = width * ((i + 1) / segments);
            }

        }
        public float[] GetVals()
        {
            return GetValsFunc.Invoke();
        }
    }
    /// <summary>
    /// Logika interakcji dla klasy SerieDisplay.xaml
    /// </summary>
    /// 
    public partial class SerieDisplay : UserControl
    {
        
        public int Segments { get; set; } = 100;
        public float Scale { get; set; } = 1;

        public int UpdateInterval { get; set; } = 100;
        protected Thread? UpdatingThread;
        protected bool doUpdate = true;
        protected bool pause = true;
        protected List<SerieElement> SerieElements = new List<SerieElement>();
        
        public SerieDisplay()
        {
            InitializeComponent();
        }

        public void AddElement(SerieElement watcher)
        {
            SerieElements.Add(watcher);
            watcher.Register(CanvasSerie.Children);
        }

        public void Start()
        {
            if(UpdatingThread != null && doUpdate) 
            { 
                if (pause) pause = false;
                return;   
            }
            UpdatingThread?.Join();
            doUpdate = true;
            pause = false;
            UpdatingThread = new Thread(UpdatingWork);
            UpdatingThread.Start();

        }
        public void Pause()
        {
            pause = true;
        }
        public void Stop()
        {
            doUpdate = false;
            UpdatingThread?.Join();
        }

        protected void UpdatingWork()
        {
            while (doUpdate) 
            {
                if(!pause)
                {
                    Dispatcher.Invoke(() => {
                        foreach (var w in SerieElements)
                            w.UpdateLines(CanvasSerie.ActualWidth, CanvasSerie.ActualHeight);
                    });
                }
                try
                {
                    Thread.Sleep(UpdateInterval);
                }
                catch (Exception ex)
                {

                }
            }
            
        }


        //Dispatcher.Invoke(new Action(() =>
        //    {
        //    try
        //    {
        //        SavedCredentials = new SettingsBatch();
        //        SavedCredentials["remember"] = CheckBox_Remember.IsChecked ?? false;
        //        if (CheckBox_Remember.IsChecked ?? false)
        //        {
        //            SavedCredentials["IP"] = TextBox_IP.Text;
        //            SavedCredentials["port"] = int.Parse(TextBox_Port.Text);
        //            SavedCredentials["password"] = TextBox_Password.Password;
        //        }
        //        SavedCredentials.Save(Storage.GetSettingsFilename("StoredCredentials"));
        //        Close();
        //    }
        //    catch (Exception ex) { Console.WriteLine(ex.ToString()); }
        //    return;
        //}));
    }
}
