using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace League_Overlay_WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Timer t = new Timer(1000);
        DirectoryInfo logDirInf = new DirectoryInfo("C:\\Riot Games\\League of Legends\\Logs\\Game - R3d Logs");
        Process leagueProcess = null;
        StreamReader streamReader;
        string str;
        Stream stream;
        bool gameClosed = true;
        bool gameLoading = false;
        bool gameActive = false;
        bool alreadyDone = false;
        BitmapImage ui1;
        BitmapImage ui2;

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool GetWindowRect(IntPtr hWnd, ref RECT Rect);


        public MainWindow()
        {
            InitializeComponent();
        }

        private Process findLeagueWindow()
        {
            Process[] processlist = Process.GetProcesses();
            Process lProcess = null;
            foreach (Process process in processlist)
            {
                if ((process.MainWindowTitle).ToString().Equals("League of Legends (TM) Client"))
                {
                    // MessageBox.Show("Process: {0} ID: {1} Window title: {2}" + process.ProcessName + " " + process.Id + " "  + process.MainWindowTitle);//debugging
                    lProcess = process;
                }
            }
            return lProcess;
        }
       

        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            this.Topmost = true;
            this.Left = 0;
            this.Top = 0;
            this.Height = 1080;
            this.Width = 1920;
            overlay.Height = 1080;
            overlay.Width = 1920;

            ui1 = new BitmapImage();
            ui1.BeginInit();
            ui1.UriSource = new Uri("C:\\Users\\Bryan\\Documents\\GitHub\\LeagueOverlay\\League Overlay WPF\\League Overlay WPF\\diamond overlay.png");
            ui1.EndInit();
       
            ui2 = new BitmapImage();
            ui2.BeginInit();
            ui2.UriSource = new Uri("C:\\Users\\Bryan\\Documents\\GitHub\\LeagueOverlay\\League Overlay WPF\\League Overlay WPF\\load screen overlay.png");
            ui2.EndInit();

            //overlay.Source = ui1;

            leagueProcess = null;
            
           this.WindowState = WindowState.Minimized;
            //Loops until game opens
            while (gameClosed)
            {
                this.Topmost = false;
                leagueProcess = findLeagueWindow();
                if (leagueProcess != null)
                {
                    gameClosed = false;
                    gameLoading = true;
                    alreadyDone = false;
                }
                
            }

            //loops while game is on loading screen
            while (gameLoading)
            {
                var myFile = logDirInf.GetFiles().OrderByDescending(f => f.LastWriteTime).First();
                stream = File.Open(myFile.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

                streamReader = new StreamReader(stream);
                str = streamReader.ReadToEnd();

                if (!(str.Contains("GAMESTATE_GAMELOOP Begin")) && !alreadyDone)
                {
                    this.WindowState = WindowState.Maximized;
                    this.Topmost = true;
                    this.Left = 0;
                    this.Top = 0;
                    this.Height = 1080;
                    this.Width = 1920;
                    overlay.Height = 1080;
                    overlay.Width = 1920;
                    overlay.Source = ui2;
                    
                    
                    alreadyDone = true;
                }
                else if (str.Contains("GAMESTATE_GAMELOOP Begin"))
                {
                    gameLoading = false;
                    gameActive = true;
                    alreadyDone = false;
                }
                streamReader.Close();
                stream.Close();
                //System.Threading.Thread.Sleep(1000);
            }

            //loops while game is active
            while (gameActive)
            {
                var myFile = logDirInf.GetFiles().OrderByDescending(f => f.LastWriteTime).First();
                stream = File.Open(myFile.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

                streamReader = new StreamReader(stream);
                str = streamReader.ReadToEnd();


                if (str.Contains("GAMESTATE_GAMELOOP Begin") && !(str.Contains("Exiting WinMain")) && !alreadyDone)
                {
                    this.WindowState = WindowState.Maximized;
                    this.Topmost = true;
                    this.Left = 0;
                    this.Top = 0;
                    this.Height = 1080;
                    this.Width = 1920;
                    overlay.Height = 1080;
                    overlay.Width = 1920;
                    overlay.Source = ui1;
                    
                    alreadyDone = true;
                }
                else if (str.Contains("Exiting WinMain"))
                {
                    gameActive = false;
                    gameClosed = true;
                    alreadyDone = false;
                    this.WindowState = WindowState.Minimized;
                }
                streamReader.Close();
                stream.Close();

            }
            
            //this.Loaded += new RoutedEventHandler(Window_Loaded_1);
        }

        /// <summary>
        /// Timer tick event for checking if a league loading screen or game is open.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            
            
        }
    }
}
