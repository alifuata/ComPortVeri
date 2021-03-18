using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ComPortVeri
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
        }
        public bool portAcikMi { get; set; } = false;

        private SerialPort bizimPort;
        private void Form1_Load(object sender, EventArgs e)
        {
            btnOpen.Text = "Aç";
            //açık portları alıyoruz
            string[] ports = SerialPort.GetPortNames();
            foreach (var item in ports)
            {
                cbComPorts.Items.Add(item);
            }
            if (ports.Length > 0) cbComPorts.SelectedIndex = 0;
            //standart baudrateleri ekliyoruz
            cbBaudRates.Items.AddRange(new string[] { "75", "110", "300", "1200", "2400", "4800", "9600", "19200", "38400", "57600", "115200" });
            cbBaudRates.SelectedIndex = 6;

        }
        private void btnOpen_Click(object sender, EventArgs e)
        {
            if (!portAcikMi)
            {
                try
                {
                    //com portu açıyoruz
                    bizimPort = new SerialPort(cbComPorts.SelectedItem.ToString(), Convert.ToInt32(cbBaudRates.SelectedItem), Parity.None, 8, StopBits.One);
                    bizimPort.Open();
                    portAcikMi = true;
                    btnOpen.Text = "Kapat";
                    string okunan = "";
                    //arayüzün kilitlenmemesi için gelen veriyi okuduğumuz kısım thread içinde
                    Thread thread = new Thread(new ThreadStart(() =>
                    {
                        while (portAcikMi)
                        {
                            char deger = (char)bizimPort.ReadChar();
                            if (deger=='g')
                            {
                                okunan += deger;
                                lbGelenDeger.Items.Add(okunan);
                                okunan = "";
                            }
                            else if(deger!=' ')
                            {
                                okunan += deger;
                            }
                            
                        }

                    }));
                    thread.Start();

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Hata:" + ex.Message);
                }
            }
            else
            {
                portAcikMi = false;
                bizimPort.Close();
                btnOpen.Text = "Aç";
            }
        }
    }
}
