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

namespace GameConnectionReporting
{
    public partial class MainWindow : Window
    {

        System.Windows.Forms.NotifyIcon ni = new System.Windows.Forms.NotifyIcon();
        System.Windows.Forms.ContextMenuStrip trayMenu = new System.Windows.Forms.ContextMenuStrip();

        private static readonly Random random = new Random();
        private static readonly object syncLock = new object();

        bool IsGameConnected = true;

        public MainWindow()
        {
            InitializeComponent();

            CreateNotifyIcon();
            CreateNotifyStructure();

            ConnectGameServer();
            SetPerformanceUpdateTimer();
        }

        #region Event Handlers

        protected override void OnStateChanged(EventArgs e)
        {
            if (WindowState == System.Windows.WindowState.Minimized)
            {
                this.Hide();
                ni.ShowBalloonTip(5000);
            }
            base.OnStateChanged(e);
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = System.Windows.WindowState.Minimized;
        }

        private void OpenApp_Click(object Sender, EventArgs e)
        {
            this.Show();
            this.WindowState = WindowState.Normal;
        }

        private void QuitApp_Click(object Sender, EventArgs e)
        {
            this.Close();
        }

        private void hlnkKillPong_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.killping.com");
        }

        private void performanceUpdateTimer_Tick(object sender, EventArgs e)
        {
            ConnectGameServer();
        }

        private void btnGameConnectivity_Click(object sender, RoutedEventArgs e)
        {
            if (IsGameConnected)
            {
                DisconnectGameServer();
                IsGameConnected = false;
                lblGameConnectivity.Content = "Connect Game";
                btnGameConnectivity.ToolTip = "Connect Game";
            }
            else
            {
                ConnectGameServer();
                IsGameConnected = true;
                lblGameConnectivity.Content = "Disconnect Game";
                btnGameConnectivity.ToolTip = "Disconnect Game";
            }
        }

        private void btnFeedback_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnGameConnectivity_MouseEnter(object sender, MouseEventArgs e)
        {
            brdGameConnectivity.BorderBrush = Brushes.LightBlue;
        }

        private void btnGameConnectivity_MouseLeave(object sender, MouseEventArgs e)
        {
            brdGameConnectivity.BorderBrush = Brushes.Red;
        }

        private void btnFeedback_MouseEnter(object sender, MouseEventArgs e)
        {
            brdFeedback.BorderBrush = Brushes.LightBlue;
        }

        private void btnFeedback_MouseLeave(object sender, MouseEventArgs e)
        {
            brdFeedback.BorderBrush = Brushes.YellowGreen;
        }

        private void btnMinimize_MouseEnter(object sender, MouseEventArgs e)
        {
            brdMinimize.Visibility = System.Windows.Visibility.Visible;
        }

        private void btnMinimize_MouseLeave(object sender, MouseEventArgs e)
        {
            brdMinimize.Visibility = System.Windows.Visibility.Hidden;
        }

        private void btnExit_MouseEnter(object sender, MouseEventArgs e)
        {
            brdExit.Visibility = System.Windows.Visibility.Visible;
        }

        private void btnExit_MouseLeave(object sender, MouseEventArgs e)
        {
            brdExit.Visibility = System.Windows.Visibility.Hidden;
        }

        #endregion

        #region Custom Functions

        private void CreateNotifyIcon()
        {
            ni.Icon = new System.Drawing.Icon("logo.ico");
            ni.Text = "Game Connection Reporting";
            ni.Visible = true;
            ni.DoubleClick +=
                delegate(object sender, EventArgs args)
                {
                    this.Show();
                    this.WindowState = WindowState.Normal;
                };

            ni.BalloonTipText = "Game Connection Reporting is minimized to system tray!";
            ni.BalloonTipTitle = "Game Connection Reporting";
        }

        private void CreateNotifyStructure()
        {
            trayMenu.Items.Add("Open Game Connection Reporting").Click += new EventHandler(OpenApp_Click);
            trayMenu.Items.Add("Quit").Click += new EventHandler(QuitApp_Click);
            ni.ContextMenuStrip = trayMenu;
        }

        public void ConnectGameServer()
        {
            // Grid labels
            GenerateRandomValue(lblRow1GVPN, "ms", 0, 999);
            GenerateRandomValue(lblRow1Internet, "ms", 0, 999);
            GenerateRandomValue(lblRow2GVPN, "ms", 0, 999);
            GenerateRandomValue(lblRow2Internet, "ms", 0, 999);
            GenerateRandomValue(lblRow3GVPN, "ms", 0, 999);
            GenerateRandomValue(lblRow3Internet, "ms", 0, 999);
            GenerateRandomValue(lblRow4GVPN, "ms", 0, 999);
            GenerateRandomValue(lblRow4Internet, "ms", 0, 999);
            GenerateRandomValue(lblRow5GVPN, "", 0, 999);
            GenerateRandomValue(lblRow5Internet, "", 0, 999);

            GenerateRandomValue(lblServer1ImpPrctge, "%", 0, 100);
            GenerateRandomValue(lblServer2ImpPrctge, "%", 0, 100);
            GenerateRandomValue(lblServer3ImpPrctge, "%", 0, 100);
            GenerateRandomValue(lblServer4ImpPrctge, "%", 0, 100);
            GenerateRandomValue(lblServer5ImpPrctge, "%", 0, 100);

            SetProgressBarValue(lblServer1ImpPrctge, PrgsBarServer1ImpPrctge);
            SetProgressBarValue(lblServer2ImpPrctge, PrgsBarServer2ImpPrctge);
            SetProgressBarValue(lblServer3ImpPrctge, PrgsBarServer3ImpPrctge);
            SetProgressBarValue(lblServer4ImpPrctge, PrgsBarServer4ImpPrctge);
            SetProgressBarValue(lblServer5ImpPrctge, PrgsBarServer5ImpPrctge);

            // Sumary labels
            GenerateRandomValue(lblActiveConnections, "", 0, 999);

            string[] ServerLocations = new string[] { "Asia", "Africa", "North America", "South America", "Europe", "Australia" };
            lblGVPNServerLocation.Content = ServerLocations[random.Next(5)];

            string[] ActiveServers = new string[] { "Ukrainian Server", "Indian Server", "American Server", "Australian Server", "Pakistani Server", "Canadian Server" };
            lblActiveServer.Content = ActiveServers[random.Next(5)];

            GenerateRandomValue(lblConnectionImprovement, "%", 0, 100);
            GenerateRandomValue(lblPing, "", 0, 999);
            GenerateRandomValue(lblBytesReceived, "KB", 0, 999);
            GenerateRandomValue(lblBytesSent, "KB", 0, 999);
        }

        public void DisconnectGameServer()
        {
            // Grid labels
            lblRow1GVPN.Content = "";
            lblRow1Internet.Content = "";
            lblRow2GVPN.Content = "";
            lblRow2Internet.Content = "";
            lblRow3GVPN.Content = "";
            lblRow3Internet.Content = "";
            lblRow4GVPN.Content = "";
            lblRow4Internet.Content = "";
            lblRow5GVPN.Content = "";
            lblRow5Internet.Content = "";

            lblServer1ImpPrctge.Content = "";
            lblServer2ImpPrctge.Content = "";
            lblServer3ImpPrctge.Content = "";
            lblServer4ImpPrctge.Content = "";
            lblServer5ImpPrctge.Content = "";

            PrgsBarServer1ImpPrctge.Value = 0;
            PrgsBarServer2ImpPrctge.Value = 0;
            PrgsBarServer3ImpPrctge.Value = 0;
            PrgsBarServer4ImpPrctge.Value = 0;
            PrgsBarServer5ImpPrctge.Value = 0;


            // Sumary labels
            lblActiveConnections.Content = "";
            lblGVPNServerLocation.Content = "";
            lblActiveServer.Content = "";

            lblConnectionImprovement.Content = "";
            lblPing.Content = "";
            lblBytesReceived.Content = "";
            lblBytesSent.Content = "";

        }

        public void GenerateRandomValue(Label lbl, string concatenationStr, int minNum, int maxNum)
        {
            lock (syncLock)
            {
                lbl.Content = random.Next(minNum, maxNum).ToString() + " " + concatenationStr;
            }
        }

        public void SetProgressBarValue(Label lbl, ProgressBar prgsBar)
        {
            prgsBar.Value = Convert.ToDouble(lbl.Content.ToString().Remove(lbl.Content.ToString().Length - 2));
        }

        private void SetPerformanceUpdateTimer()
        {
            System.Windows.Threading.DispatcherTimer performanceUpdateTimer = new System.Windows.Threading.DispatcherTimer();
            performanceUpdateTimer.Tick += new EventHandler(performanceUpdateTimer_Tick);
            performanceUpdateTimer.Interval = new TimeSpan(0, 0, 30);
            performanceUpdateTimer.Start();
        }
        #endregion

    }
}
