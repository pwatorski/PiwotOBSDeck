using PiwotOBS.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace PiwotOBSDeck.UI
{
    /// <summary>
    /// Logika interakcji dla klasy SceneNodeView.xaml
    /// </summary>
    public partial class SceneNodeView : UserControl
    {
        public Thickness Indentation { get; }
        public SceneItem Item { get; private set; }
        public int Level { get; private set; }
        public bool IsExpanded { get; set; }
        public bool IsVisible { get; set; }
        public bool CanBeExpanded { get; private set; }
        public List<SceneNodeView> Children { get; } = new List<SceneNodeView>();
        public EventHandler OnToggled;
        public ListBoxItem listBoxItem { get; private set; }
        public SceneNodeView(PiwotOBS.Structure.SceneItem item, int level, ListBoxItem container)
        {
            InitializeComponent();
            listBoxItem = container;
            Item = item;
            Level = level;
            Indentation = new Thickness(level * 12, 0, 0, 0);
            Margin = Indentation;
            text_name.Text = item.Name;
            image_icon.Source = new BitmapImage(new Uri(GetIconForItem(item), UriKind.Absolute));
            CanBeExpanded = (item is PiwotOBS.Structure.Container);

            button_toggle.Visibility = !CanBeExpanded ? Visibility.Collapsed : Visibility.Visible;
            border.Visibility = CanBeExpanded ? Visibility.Collapsed : Visibility.Visible;
            IsExpanded = false;
        }

        private string GetIconForItem(PiwotOBS.Structure.SceneItem item)
        {
            return item switch
            {
                PiwotOBS.Structure.Scene { } => "pack://application:,,,/Resources/SCENE_ICON.png",
                PiwotOBS.Structure.Group { } => "pack://application:,,,/Resources/GROUP_ICON.png",
                PiwotOBS.Structure.ItemImage { } => "pack://application:,,,/Resources/IMAGE_ICON.png",
                PiwotOBS.Structure.ItemText { } => "pack://application:,,,/Resources/TEXT_ICON.png",
                PiwotOBS.Structure.ItemCaptureMonitor { } => "pack://application:,,,/Resources/CAPTUREMONITOR_ICON.png",
                PiwotOBS.Structure.ItemFfmpeg { } => "pack://application:,,,/Resources/FFMPG_ICON.png",
                _ => "pack://application:,,,/Resources/GROUP_ICON.png",
            };
        }

        private void ToggleButton_Click(object sender, RoutedEventArgs e)
        {
            ToggleExpand(!IsExpanded);
        }

        public void ToggleExpand(bool expand)
        {
            IsExpanded = expand;
            foreach (var child in Children)
            {
                child.ToggleHideRecursive(!IsExpanded);
            }
        }

        public void ToggleHideRecursive(bool hide)
        {
            listBoxItem.Visibility = hide ? Visibility.Collapsed : Visibility.Visible;

            foreach (var child in Children)
            {
                child.ToggleHideRecursive(!IsExpanded || hide);
            }
        }
    }
}
