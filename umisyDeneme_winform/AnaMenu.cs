using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace umisyDeneme_winform
{
    public partial class AnaMenu : Form
    {
        public AnaMenu()
        {
            InitializeComponent();
            timer1.Interval = 60000; // 1 saniye

            this.Text = "Umisy Makine Ve Otomasyon Urun Takip Sistemi";

            dataGridView1.BorderStyle = BorderStyle.None;
            dataGridView1.BackgroundColor = Color.White;
            dataGridView1.EnableHeadersVisualStyles = false;

            dataGridView1.RowHeadersVisible = false;

            dataGridView1.DefaultCellStyle.BackColor = Color.White;
            dataGridView1.DefaultCellStyle.ForeColor = Color.Black;
            dataGridView1.DefaultCellStyle.Font = new Font("Segoe UI", 10);

            dataGridView1.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245);

            dataGridView1.DefaultCellStyle.SelectionBackColor = Color.FromArgb(0, 120, 215);
            dataGridView1.DefaultCellStyle.SelectionForeColor = Color.White;

            dataGridView1.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dataGridView1.GridColor = Color.LightGray;

            dataGridView1.RowTemplate.Height = 30;

            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.MultiSelect = false;

            dataGridView1.EnableHeadersVisualStyles = false;

            Color headerBack = Color.FromArgb(200, 0, 0);

            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = headerBack;
            dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;

            dataGridView1.ColumnHeadersDefaultCellStyle.SelectionBackColor = headerBack;
            dataGridView1.ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.Black;



            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.Red;
            dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;

            dataGridView1.DefaultCellStyle.SelectionBackColor = Color.LightGray;
            dataGridView1.DefaultCellStyle.SelectionForeColor = Color.Black;

            Panel panelContainer = new Panel();
            panelContainer.BackColor = Color.Gray; // çerçeve rengi (isteğe bağlı)

            // İstediğiniz boşluklar
            panelContainer.Left = 10;
            panelContainer.Top = 100;
            panelContainer.Width = this.ClientSize.Width - 20; // sağ + sol 10 px
            panelContainer.Height = this.ClientSize.Height - 110; // üst 200 + alt 10 px

            dataGridView1.Parent = panelContainer;
            dataGridView1.Dock = DockStyle.Fill;

            // Form resize olunca da uyumlu olsun
            panelContainer.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

            this.Controls.Add(panelContainer);


        }
        public static DataTable VeriGetir()
        {
            string connectionString = "Server=10.10.41.96;Database=XPODA;User Id=izmit.umisy;Password=Umisy.ızmt18%+!;TrustServerCertificate=True;";

            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string query = "SELECT * FROM UmisyMakineOtomasyonVeriTablosu";

                using (SqlDataAdapter da = new SqlDataAdapter(query, conn))
                {
                    da.Fill(dt);
                }
            }

            return dt;
        }


        private void AnaMenu_Load(object sender, EventArgs e)
        {
            
            dataGridView1.DataSource = VeriGetir();
            dataGridView1.Columns["BARKOD"].Frozen = true;
            this.AutoScaleMode = AutoScaleMode.Font;
            this.WindowState = FormWindowState.Maximized; // açılışta tam ekran
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dataGridView1.Dock = DockStyle.Fill;

            timer1.Start();

        }


        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
        private void Filtrele()
        {
            DataTable dt = (DataTable)dataGridView1.DataSource;

            string barkod = textBox_BARKOD.Text.Trim();
            DateTime tarih = dateTimePicker2.Value.Date;

            string filter =
                $"Kayit_Tarihi >= #{tarih:MM/dd/yyyy} 00:00:00# AND " +
                $"Kayit_Tarihi < #{tarih.AddDays(1):MM/dd/yyyy}#";

            if (!string.IsNullOrWhiteSpace(barkod))
            {
                filter += $" AND Barkod LIKE '%{barkod.Replace("'", "''")}%'";
            }

            dt.DefaultView.RowFilter = filter;
        }
        private void textBox_BARKOD_TextChanged(object sender, EventArgs e)
        {
            Filtrele();
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {
            Filtrele();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            
            dataGridView1.DataSource = VeriGetir();
            this.AutoScaleMode = AutoScaleMode.Font;
            this.WindowState = FormWindowState.Maximized; // açılışta tam ekran
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dataGridView1.Dock = DockStyle.Fill;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            
            dataGridView1.DataSource = VeriGetir();
            dataGridView1.Columns["BARKOD"].Frozen = true;
            this.AutoScaleMode = AutoScaleMode.Font;
            this.WindowState = FormWindowState.Maximized; // açılışta tam ekran
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dataGridView1.Dock = DockStyle.Fill;
        }
    }
}
