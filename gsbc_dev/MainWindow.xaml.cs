using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Text;
using System.ComponentModel;
using System.IO;
using System.IO.Ports;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Data;
using System.Device.Location;
using Geolocation;


namespace gsbc_dev
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private GeoCoordinateWatcher Watcher = null;
        string pesan;

        public string logName;
        Koneksi konekin = new Koneksi();
        System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
        DispatcherTimer dispatcherTimer = new DispatcherTimer();
        int interval;
        string[] data;
        double latit, longit;

        Stopwatch sw = new Stopwatch();
        FileStream fs = null;
        public MainWindow()
        {
            InitializeComponent();
            Watcher = new GeoCoordinateWatcher();
            // Catch the StatusChanged event.

            Dispatcher.Invoke((Action)(() => Watcher.StatusChanged += Watcher_StatusChanged));
            // Start the watcher.

            Watcher.Start();
            initBaudList();
            initComPortList();
            initDatabitsList();
            initStopbitsList();
            initParityList();
            initHandshakeList();
            initDelay();
            string logPath = @"D:\Project\gsbc_dev\Log ";
            if (!Directory.Exists(logPath))
            {
                Directory.CreateDirectory(logPath);
            }
            logName = @"D:\Project\gsbc_dev\log\LOG " + DateTime.Now.Day.ToString() + "-" + DateTime.Now.Month.ToString() + "-" + DateTime.Now.Year.ToString() + " " + DateTime.Now.Hour.ToString() + "." + DateTime.Now.Minute.ToString() + "." + DateTime.Now.Second.ToString() + ".txt";
            if (!File.Exists(logName))
            {
                using (StreamWriter writer = new StreamWriter(logName)) { }
            }
        }

        private void Watcher_StatusChanged(object sender, GeoPositionStatusChangedEventArgs e)
        {
            if (e.Status == GeoPositionStatus.Ready)
            {
                // Display the latitude and longitude.
                if (Watcher.Position.Location.IsUnknown)
                {
                    bearings.Content = "Cannot find location data";
                }
                else
                {
                    latit = Watcher.Position.Location.Latitude;
                    longit = Watcher.Position.Location.Longitude;
                }
            }
        }
        private void initBaudList()
        {
            string[] bauds = { "110", "300", "600", "1200", "2400", "4800", "9600", "14400", "19200", "38400", "56000", "57600", "115200" };
            foreach (string baud in bauds)
            {
                baudCombo.Items.Add(baud);
            }
            baudCombo.SelectedIndex = 11;
        }
        private void initPortLaunch()
        {
            foreach (String s in SerialPort.GetPortNames())
            {
                if (s != "")
                {
                    portLauncher.Items.Add("COM" + s.Substring(3));
                    portLauncher.SelectedIndex = 0;
                }
                else
                {
                    portLauncher.Items.Add("Unknown");
                    portLauncher.SelectedIndex = 0;
                }
            }
            if (Komunikasi.Default.NamaPort != "")
            {
                portCombo.SelectedIndex = 0;
            }
        }
        private void initComPortList()
        {
            foreach (String s in SerialPort.GetPortNames())
            {
                if (s != "")
                {
                    portCombo.Items.Add("COM" + s.Substring(3));
                    portCombo.SelectedIndex = 0;
                }
                else
                {
                    portCombo.Items.Add("Unknown");
                    portCombo.SelectedIndex = 0;
                }
            }
            if (Komunikasi.Default.NamaPort != "")
            {
                portCombo.SelectedIndex = 0;
            }
        }
        private void initDatabitsList()
        {
            for (int i = 5; i <= 9; i++)
            {
                databitCombo.Items.Add(i);
            }
            databitCombo.SelectedIndex = 3;
        }
        private void initStopbitsList()
        {
            foreach (string s in Enum.GetNames(typeof(StopBits)))
            {
                if (s != "None")
                {
                    stopbitCombo.Items.Add(s);
                }
            }
            stopbitCombo.SelectedIndex = 0;
        }
        private void initParityList()
        {
            foreach (String s in Enum.GetNames(typeof(Parity)))
            {
                parityCombo.Items.Add(s);
            }
            parityCombo.SelectedIndex = 0;
        }
        private void initHandshakeList()
        {
            foreach (String s in Enum.GetNames(typeof(Handshake)))
            {
                handshakeCombo.Items.Add(s);
            }
            handshakeCombo.SelectedIndex = 0;
        }
        private void initDelay()
        {
            delayCombo.Items.Add("100  ms");
            delayCombo.Items.Add("200  ms");
            delayCombo.Items.Add("500  ms");
            delayCombo.Items.Add("1000 ms");
            delayCombo.SelectedIndex = 0;
        }

        private void okeSetButton_Click(object sender, RoutedEventArgs e)
        {
            Komunikasi.Default.BaudRate = Convert.ToInt32(baudCombo.SelectedItem);
            Komunikasi.Default.NamaPort = Convert.ToString( portCombo.SelectedItem);
            Komunikasi.Default.DataBits = Convert.ToUInt16( databitCombo.SelectedItem);
            Komunikasi.Default.StopBits = (StopBits)Enum.Parse(typeof(StopBits),  stopbitCombo.SelectedItem.ToString());
            Komunikasi.Default.Handshake = (Handshake)Enum.Parse(typeof(Handshake),  handshakeCombo.SelectedItem.ToString());
            Komunikasi.Default.Parity = (Parity)Enum.Parse(typeof(Parity),  parityCombo.SelectedItem.ToString());
            Komunikasi.Default.Launcher = Convert.ToString( portLauncher.SelectedItem);
            interval = Convert.ToInt32(Convert.ToString( delayCombo.SelectedItem).Remove(5));
            try
            {
                konekin.buka();
                konekin.NewSerialDataReceived += konekin_NewSerialDataReceived;
                if (Komunikasi.Default.terkoneksi)
                {
                    //btnPutus.Content = "Terhubung ke " + Komunikasi.Default.NamaPort;              
                    stopwatch.Reset();
                    //ClearLog();
                }
                else
                {

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void manualInput_Click(object sender, RoutedEventArgs e)
        {
            while (true)
            {
                konekin.tulis(inpulat.Text + " " + inputver.Text);
                //konekin.tulis("80 90");
                Thread.Sleep(3000);
            }
            
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }
        void konekin_NewSerialDataReceived(object sender, Koneksi.SerialDataEventArgs e)
        {
            Thread.Sleep(1000);
            try
            {
                pesan = e.Data;
            }
            catch { }

            {
                //pesan = pesan.Replace('.', ',');
                data = pesan.Split(' ');
                Dispatcher.Invoke((Action)(() => SerialMonitor.Text += pesan + " " + data.Length.ToString() + "\n"));


                try
                {
                    // 0        1   2   3       4       5           6          7
                    //GPSTIME Lat Long  Suhu Tekananan Kelembaban Ketinggian CO2
                    if (data.Length == 8)
                    {
                        
                        try
                        {
                            double distance = GeoCalculator.GetDistance(Convert.ToDouble(data[1]), Convert.ToDouble(data[2]), -7.76395814, 110.3767115,4);
                            double bearing = GeoCalculator.GetBearing(-7.76395814, 110.3767115, Convert.ToDouble(data[1]), Convert.ToDouble(data[2]));
                            double vertikal = itung(distance*1609.34, Convert.ToDouble(data[6]));
                            this.Dispatcher.BeginInvoke(DispatcherPriority.Send, new Action(delegate ()
                            {
                        

                                #region terminal text
                                try
                                {
                                   
                                    //SerialMonitor.AppendText("MAHESWARA_KOMBAT2016 " + " " + data[4] + " " + data[6] + " " + data[5] + " " + data[3] +  " " + data[2] + " " + data[1] + "\n");
                                    using (StreamWriter writer = File.AppendText(logName))
                                    {
                                    //konekin.tulis(data[9] + " " + data[10]);
                                    writer.WriteLine("MAHESWARAUGM" + " " + data[0] + " " + data[1] + " " + data[2] + " " + data[3] + " " + data[4] + " " + data[5] + " " + data[6] + " " + data[7]  + " " + bearing + " " + vertikal + " " + distance* 1609.34);
                                    }
                                }
                                catch { }
                                inpulat.Text = bearing.ToString("F0");
                                inputver.Text = vertikal.ToString("F0");
                                SerialMonitor.ScrollToEnd();
                                Dispatcher.Invoke((Action)(() => konekin.tulis(bearing.ToString("F0") + " " + inputver.Text)));
                                #endregion
                            }));
                        }
                        catch (Exception) { }
                    }
                }
                catch (Exception) { }
            }
        }

        double itung(double jarak, double tinggi)
        {
           double radians = Math.Atan(tinggi/jarak);
           double angle = radians * (180 / Math.PI);
           return angle;
        }

        private void disconnectButton_Click(object sender, RoutedEventArgs e)
        {
            konekin.tutup();
        }

    }
}
