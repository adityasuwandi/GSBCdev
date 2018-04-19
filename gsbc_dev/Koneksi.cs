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
using System.Windows.Shapes;
using System.IO.Ports;
using System.Threading;

namespace gsbc_dev
{
    class Koneksi
    {
        public class SerialDataEventArgs : EventArgs
        {
            public SerialDataEventArgs(string dataInByteArray)
            {
                Data = dataInByteArray;
            }

            /// <summary>
            /// Byte array containing data from serial port
            /// </summary>
            public string Data;
        }

        public Koneksi()
        {

            komSerial.DataReceived += new SerialDataReceivedEventHandler(komSerial_DataReceived); 
            //serialLaunch.DataReceived += new SerialDataReceivedEventHandler(serialLaunch_DataReceived);
        }

        public EventHandler<SerialDataEventArgs> NewSerialDataReceived;

        public SerialPort komSerial = new SerialPort();
        public SerialPort komSerial2 = new SerialPort();
        //public SerialPort serialLaunch = new SerialPort();
        //private Tampil tampil = new Tampil();

        private void komInit()
        {
            komSerial.PortName = Komunikasi.Default.NamaPort;
            komSerial.BaudRate = Komunikasi.Default.BaudRate;
            komSerial.Parity = Komunikasi.Default.Parity;
            komSerial.DataBits = Komunikasi.Default.DataBits;
            komSerial.StopBits = Komunikasi.Default.StopBits;
            komSerial.Handshake = Komunikasi.Default.Handshake;
            //komSerial.ReadTimeout = 100;
            //komSerial.WriteTimeout = 8;
            komSerial2.PortName = "COM7";
            komSerial2.BaudRate = 115200;

        }

        //private void launchInit()
        //{
        //    serialLaunch.PortName = Komunikasi.Default.Launcher;
        //}

        public bool statusKoneksi()
        {
            //if (komSerial.IsOpen)
            //    return true;
            //else
            //    return false;
            return komSerial.IsOpen && komSerial2.IsOpen; //&& serialLaunch.IsOpen;
        }

        public void buka()
        {
            if (!komSerial.IsOpen || !komSerial2.IsOpen)
            {
                try
                {
                    komInit();
                    komSerial.DtrEnable = true;
                    komSerial.Open();
                    komSerial2.Open();
                    //serialLaunch.Open();
                    Komunikasi.Default.terkoneksi = true;

                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("empty"))
                    {
                        MessageBox.Show("Mohon dipastikan semua pengaturan telah diisi.", "Kesalahan");
                    }
                    else if (ex.Message.Contains("used"))
                    {
                        MessageBox.Show("Port " + Komunikasi.Default.NamaPort + " sedang dipakai aplikasi yang lain, mohon dicek kembali.", "Kesalahan");
                    }
                    else if (ex.Message.Contains("use"))
                    {
                        MessageBox.Show("Resource " + Komunikasi.Default.NamaPort + " sedang dipakai, mohon dicek kembali.", "Kesalahan");
                    }
                    else
                    {
                        MessageBox.Show(ex.Message + "\r\r(Mohon dicek kembali dan diulangi.)");
                    }
                    Komunikasi.Default.terkoneksi = false;
                }
            }
            else
            {
                Komunikasi.Default.terkoneksi = false;
                komSerial.Close();
                //serialLaunch.Close();
            }
        }

        //public void tulisLauncher(string textLaunch)
        //{
        //    try
        //    {
        //        serialLaunch.Write(textLaunch);
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message, "Error");
        //    }
        //}

        public void tulis(string text)
        {
           
            try
            {
                
                komSerial2.Write(text);
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        void komSerial_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                string hasilSerial = komSerial.ReadLine();
                lemparKeEvent(hasilSerial);
            }
            catch (Exception)
            {
                //MessageBox.Show(ex.Message.ToString());
            }

        }

     

        //private void serialLaunch_DataReceived(object sender, SerialDataReceivedEventArgs e)
        //{
        //    try
        //    {
        //        string launchSerial = serialLaunch.ReadLine();
        //        lemparKeEvent(launchSerial);
        //    }
        //    catch (Exception)
        //    {
        //        //MessageBox.Show(ex.Message.ToString());
        //    }
        //}

        void lemparKeEvent(string lempar)
        {
            if (NewSerialDataReceived != null)
            {
                NewSerialDataReceived(this, new SerialDataEventArgs(lempar));
            }
        }

        public void tutup()
        {
            if (NewSerialDataReceived != null)
            {
                try {
                    komSerial.Close();
                    komSerial2.Close();
                }
                catch { }
                
            }
        }

    }
}
