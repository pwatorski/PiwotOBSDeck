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
using System.Xml.Linq;

namespace PiwotOBSDeck.UI
{
    /// <summary>
    /// Logika interakcji dla klasy TreeView2.xaml
    /// </summary>
    /// 

    public partial class TreeDisplay : UserControl
    {
        public EventHandler<SelectionChangedEventArgs> SelectionChanged;
        public List<SceneNodeView> VisibleNodes = new List<SceneNodeView>();
        public TreeDisplay()
        {
            InitializeComponent();
        }

        public void Initialize(IEnumerable<SceneItem> roots)
        {
            MainListBox.Items.Clear();
            foreach (var root in roots)
            {
                var node =AddRecursive(root, 0);
            }
        }

        private SceneNodeView AddRecursive(SceneItem item, int level)
        {
            var lbi = new ListBoxItem();
            var node = new SceneNodeView(item, level, lbi);
            lbi.Content = node;


            MainListBox.Items.Add(lbi);
            VisibleNodes.Add(node);
            if (level == 0)
            {
                lbi.Visibility = Visibility.Visible;
            }
            else
            {
                lbi.Visibility = Visibility.Collapsed;
            }

            if (item is PiwotOBS.Structure.Container container)
            {
                foreach (var child in container.Items)
                {
                    var subcChild = AddRecursive(child, level + 1);
                    node.Children.Add(subcChild);
                }
            }
            return node;
        }

        private void MainListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectionChanged?.Invoke(this, e);
        }
    }
}
