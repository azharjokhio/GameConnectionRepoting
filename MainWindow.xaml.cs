using OxyPlot;
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
    using System.Collections.Generic;

    using OxyPlot;
    using System.ComponentModel;
    using OxyPlot.Series;
    using System.Collections.ObjectModel;
    using OxyPlot.Axes;

    public partial class MainWindow : INotifyPropertyChanged
    {
        System.Windows.Forms.NotifyIcon ni = new System.Windows.Forms.NotifyIcon();
        System.Windows.Forms.ContextMenuStrip trayMenu = new System.Windows.Forms.ContextMenuStrip();
        System.Windows.Threading.DispatcherTimer performanceUpdateTimer = new System.Windows.Threading.DispatcherTimer();

        private static readonly Random random = new Random();
        private static readonly object syncLock = new object();

        bool IsGameConnected = true;

        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string property)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(property));
            }
        }

        private PlotModel plotModel;

        public PlotModel PlotModel
        {
            get
            {
                return this.plotModel;
            }
            set
            {
                this.plotModel = value;
                this.RaisePropertyChanged("PlotModel");
            }
        }

        private PlotModel CreatePlot()
        {
            var pm = new PlotModel();
            pm.TextColor = OxyColors.White;
            pm.DefaultFontSize = 9;
            pm.LegendPlacement = LegendPlacement.Outside;
            pm.LegendPosition = LegendPosition.TopLeft;
            pm.LegendOrientation = LegendOrientation.Vertical;
            LineSeries lineSeries1 = new LineSeries();
            lineSeries1.Title = "Internet Ping";
            lineSeries1.TextColor = OxyColors.White;
            lineSeries1.Color = OxyColors.SkyBlue;
            lineSeries1.Points.Add(new DataPoint(0, GenerateRandomValue(0, 20)));
            lineSeries1.Points.Add(new DataPoint(1, GenerateRandomValue(0, 20)));
            lineSeries1.Points.Add(new DataPoint(2, GenerateRandomValue(0, 20)));
            lineSeries1.Points.Add(new DataPoint(3, GenerateRandomValue(0, 20)));
            lineSeries1.Points.Add(new DataPoint(4, GenerateRandomValue(0, 20)));
            lineSeries1.Points.Add(new DataPoint(5, GenerateRandomValue(0, 20)));
            lineSeries1.Points.Add(new DataPoint(6, GenerateRandomValue(0, 20)));
            lineSeries1.Points.Add(new DataPoint(7, GenerateRandomValue(0, 20)));
            lineSeries1.Points.Add(new DataPoint(8, GenerateRandomValue(0, 20)));
            lineSeries1.Points.Add(new DataPoint(9, GenerateRandomValue(0, 20)));
            lineSeries1.Points.Add(new DataPoint(10, GenerateRandomValue(0, 20)));
            pm.Series.Add(lineSeries1);

            LineSeries lineSeries2 = new LineSeries();
            lineSeries2.Color = OxyColors.YellowGreen;
            lineSeries2.TextColor = OxyColors.SkyBlue;
            lineSeries2.Title = "Ping";
            lineSeries2.Points.Add(new DataPoint(0, GenerateRandomValue(0, 20)));
            lineSeries2.Points.Add(new DataPoint(1, GenerateRandomValue(0, 20)));
            lineSeries2.Points.Add(new DataPoint(2, GenerateRandomValue(0, 20)));
            lineSeries2.Points.Add(new DataPoint(3, GenerateRandomValue(0, 20)));
            lineSeries2.Points.Add(new DataPoint(4, GenerateRandomValue(0, 20)));
            lineSeries2.Points.Add(new DataPoint(5, GenerateRandomValue(0, 20)));
            lineSeries2.Points.Add(new DataPoint(6, GenerateRandomValue(0, 20)));
            lineSeries2.Points.Add(new DataPoint(7, GenerateRandomValue(0, 20)));
            lineSeries2.Points.Add(new DataPoint(8, GenerateRandomValue(0, 20)));
            lineSeries2.Points.Add(new DataPoint(9, GenerateRandomValue(0, 20)));
            lineSeries2.Points.Add(new DataPoint(10, GenerateRandomValue(0, 20)));

            pm.Series.Add(lineSeries2);

            return pm;

        }

        Gauge gauge1;

        public MainWindow()
        {
            InitializeComponent();

            CreateNotifyIcon();
            CreateNotifyStructure();

            //Set the current value of the gauges
            gauge1 = new Gauge(0);
            this.myGauge1.DataContext = gauge1;

            ConnectGameServer();
            SetPerformanceUpdateTimer();

            this.PlotModel = CreatePlot();
            this.DataContext = this;

          
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
                this.PlotModel = null;
            }
            else
            {
                ConnectGameServer();
                IsGameConnected = true;
                this.PlotModel = this.CreatePlot();
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

            double value1 = GenerateRandomValue(0, 100);
            lblConnectionImprovement.Content = value1 + "%";
         
            gauge1.Score = value1;

            GenerateRandomValue(lblPing, "", 0, 999);
            GenerateRandomValue(lblBytesReceived, "KB", 0, 999);
            GenerateRandomValue(lblBytesSent, "KB", 0, 999);

            this.PlotModel = this.CreatePlot();

            performanceUpdateTimer.Start();
    
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
            
            //Set the current value of the gauges
            gauge1 = new Gauge(0);
            this.myGauge1.DataContext = gauge1;
            gauge1.Score = 0;

            lblPing.Content = "";
            lblBytesReceived.Content = "";
            lblBytesSent.Content = "";

            performanceUpdateTimer.Stop();

        }

        public int GenerateRandomValue(int minNum, int maxNum)
        {
            lock (syncLock)
            {
                return random.Next(minNum, maxNum);
            }
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
            performanceUpdateTimer.Tick += new EventHandler(performanceUpdateTimer_Tick);
            performanceUpdateTimer.Interval = new TimeSpan(0, 0, 1);
            performanceUpdateTimer.Start();
        }
        #endregion

    }

    public class Gauge : INotifyPropertyChanged
    {
        private double score;

        public double Score
        {
            get { return score; }
            set
            {
                score = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Score"));
                }
            }
        }

        public Gauge(double scr)
        {
            this.Score = scr;
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }

}
