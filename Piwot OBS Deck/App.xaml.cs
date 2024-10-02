﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;

namespace PiwotOBSDeck
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    

    public partial class App : Application
    {
        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        const int SW_HIDE = 0;
        const int SW_SHOW = 5;



        public App()
        {
            var handle = GetConsoleWindow();
            //ShowWindow(handle, SW_HIDE);
        }


    }
}
