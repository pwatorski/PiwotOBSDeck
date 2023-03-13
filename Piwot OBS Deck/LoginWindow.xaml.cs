using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PiwotOBSDeck
{
    /// <summary>
    /// Logika interakcji dla klasy LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private const int GWL_STYLE = -16;
        private const int WS_SYSMENU = 0x80000;
        [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
        protected SettingsBatch SavedCredentials = new SettingsBatch();
        public LoginWindow()
        {
            Loaded += ToolWindow_Loaded;
            InitializeComponent();
            OBSDeck.client.Connected += new EventHandler(OnConnected);
            SavedCredentials = Storage.LoadSettings("StoredCredentials");
            if(SavedCredentials != null )
            {
                TextBox_IP.Text = SavedCredentials.GetVal("IP", "0.0.0.0");
                TextBox_Port.Text = SavedCredentials.GetVal("port", 4455).ToString();
                TextBox_Password.Password = SavedCredentials.GetVal("password", "");
                CheckBox_Remember.IsChecked = SavedCredentials.GetVal("remember", false);
            }
        }

        void ToolWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Code to remove close box from window
            var hwnd = new System.Windows.Interop.WindowInteropHelper(this).Handle;
            SetWindowLong(hwnd, GWL_STYLE, GetWindowLong(hwnd, GWL_STYLE) & ~WS_SYSMENU);
        }

        private void OnConnected(object? sender, EventArgs e)
        {
            //try
            //{
                
            //}catch (Exception ex) { Console.WriteLine(ex.ToString()); } 
            Dispatcher.Invoke(new Action(() => {
                try
                {
                    SavedCredentials = new SettingsBatch();
                    SavedCredentials["remember"] = CheckBox_Remember.IsChecked ?? false;
                    if (CheckBox_Remember.IsChecked ?? false)
                    {
                        SavedCredentials["IP"] = TextBox_IP.Text;
                        SavedCredentials["port"] = int.Parse(TextBox_Port.Text);
                        SavedCredentials["password"] = TextBox_Password.Password;
                    }
                    SavedCredentials.Save(Storage.GetSettingsFilename("StoredCredentials"));
                    Close();
                }
                catch (Exception ex) { Console.WriteLine(ex.ToString()); }
                return;
            }));
            
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ButtonLogin_Click(object sender, RoutedEventArgs e)
        {
            OBSDeck.ConnectToOBS(TextBox_IP.Text, TextBox_Port.Text, TextBox_Password.Password);
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void TextBox_Password_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                OBSDeck.ConnectToOBS(TextBox_IP.Text, TextBox_Port.Text, TextBox_Password.Password);
            }
        }
    }
}
