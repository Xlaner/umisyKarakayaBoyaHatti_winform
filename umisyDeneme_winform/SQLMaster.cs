
using S7.Net;
using umisyDeneme_winform;
using S7.Net.Types;
using System.Data.SqlClient;
using System;
using Microsoft.Data.SqlClient;
using System.Net.NetworkInformation;

namespace umisyDeneme_winform


{
    public partial class SQLMaster : Form
    {
        

        public static string connStrtoUmisy = "Server=10.10.41.96;Database=XPODA;User Id=izmit.umisy;Password=Umisy.ýzmt18%+!;TrustServerCertificate=True;";

        public static string connstr_burosso = "Server=10.10.41.96;Database=master;User Id=izmit.umisy;Password=Umisy.ýzmt18%+!;TrustServerCertificate=True;";
        public SQLMaster()
        {
            InitializeComponent();
            this.Text = "Umisy Makine Ve Otomasyon Urun Takip Sistemi";
            this.FormClosed += SQLMaster_FormClosed;
        }

        private void SQLMaster_FormClosed (object sender, FormClosedEventArgs e)
        {
          //  Application.Restart();
        }



        static Plc plc = new Plc(CpuType.S71500, "188.119.38.1", 0, 1);
        int pingnum = 0;

        public void FormAc(Form frm)
        {
            panel1.Controls.Clear();

            frm.TopLevel = false;
            frm.FormBorderStyle = FormBorderStyle.None;
            frm.Dock = DockStyle.Fill;

            panel1.Controls.Add(frm);
            frm.Show();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            FormAc(ClassOfMenus.anaMenu);
            
            timer1.Start();
            timer1.Interval = 1000;

        }



        public bool kvy9UrunVar = false;
        public List<Urun> urunListesi = new List<Urun>();



        private void timer1_Tick(object sender, EventArgs e)
        {

            umisyDeneme_winform.LýstOfFunctions.burassoVeri_yaz(connstr_burosso, connStrtoUmisy);
            
            
            Ping ping = new Ping();

            if (ping.Send("188.119.38.1").Status == IPStatus.Success)
            {
               
                umisyDeneme_winform.LýstOfFunctions.boyasizVeri_yaz(plc, connStrtoUmisy);
                umisyDeneme_winform.LýstOfFunctions.boyaliVeri_Yaz(plc, connStrtoUmisy);
                umisyDeneme_winform.LýstOfFunctions.BurassoToPLC(connstr_burosso, plc);
            }
            else

            {
                pingnum++;

                if (pingnum > 60) 
                {
                    Application.Restart();
                }

            }




        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
