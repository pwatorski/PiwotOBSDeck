using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PiwotOBSDeck
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            OBSDeck.client.Connected += new EventHandler(OnConnected);
            Console.WriteLine(Storage.GetFilenameInSettings("a"));
        }

        private void OnConnected(object? sender, EventArgs e)
        {
            Dispatcher.Invoke(new Action(() => {
                Console.WriteLine("Connected");
                try
                {
                    this.Icon = Imaging.CreateBitmapSourceFromHIcon(
                        Properties.Resources.OBSDeckIconOn.Handle,
                        Int32Rect.Empty,
                        BitmapSizeOptions.FromEmptyOptions());
                    Console.WriteLine((ImageSource)Resources["OBSDeckIconOn.ico"]);
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }));
        }

        private void ButtonConnect_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow loginWIndow = new LoginWindow();
            loginWIndow.Show();
        }


    }
}
