namespace umisyDeneme_winform
{
    partial class AnaMenu
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AnaMenu));
            dataGridView1 = new DataGridView();
            textBox_BARKOD = new TextBox();
            BARKOD_TEXT = new Label();
            dateTimePicker2 = new DateTimePicker();
            pictureBox1 = new PictureBox();
            panel1 = new Panel();
            button1 = new Button();
            timer1 = new System.Windows.Forms.Timer(components);
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // dataGridView1
            // 
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Location = new Point(-4, 110);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.ReadOnly = true;
            dataGridView1.Size = new Size(1280, 873);
            dataGridView1.TabIndex = 0;
            dataGridView1.CellContentClick += dataGridView1_CellContentClick;
            // 
            // textBox_BARKOD
            // 
            textBox_BARKOD.Location = new Point(17, 31);
            textBox_BARKOD.Name = "textBox_BARKOD";
            textBox_BARKOD.Size = new Size(200, 23);
            textBox_BARKOD.TabIndex = 1;
            textBox_BARKOD.TextChanged += textBox_BARKOD_TextChanged;
            // 
            // BARKOD_TEXT
            // 
            BARKOD_TEXT.AutoSize = true;
            BARKOD_TEXT.Font = new Font("Elephant", 17.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            BARKOD_TEXT.Location = new Point(17, -1);
            BARKOD_TEXT.Name = "BARKOD_TEXT";
            BARKOD_TEXT.Size = new Size(135, 30);
            BARKOD_TEXT.TabIndex = 2;
            BARKOD_TEXT.Text = "BARKOD";
            BARKOD_TEXT.Click += label1_Click;
            // 
            // dateTimePicker2
            // 
            dateTimePicker2.Location = new Point(258, 28);
            dateTimePicker2.Name = "dateTimePicker2";
            dateTimePicker2.Size = new Size(200, 23);
            dateTimePicker2.TabIndex = 4;
            dateTimePicker2.ValueChanged += dateTimePicker2_ValueChanged;
            // 
            // pictureBox1
            // 
            pictureBox1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            pictureBox1.BackColor = SystemColors.Window;
            pictureBox1.Image = Properties.Resources.logo;
            pictureBox1.Location = new Point(852, 12);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(400, 75);
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox1.TabIndex = 5;
            pictureBox1.TabStop = false;
            pictureBox1.Click += pictureBox1_Click;
            // 
            // panel1
            // 
            panel1.BorderStyle = BorderStyle.FixedSingle;
            panel1.Controls.Add(textBox_BARKOD);
            panel1.Controls.Add(BARKOD_TEXT);
            panel1.Controls.Add(dateTimePicker2);
            panel1.Location = new Point(167, 19);
            panel1.Name = "panel1";
            panel1.Size = new Size(472, 68);
            panel1.TabIndex = 6;
            // 
            // button1
            // 
            button1.Location = new Point(658, 19);
            button1.Name = "button1";
            button1.Size = new Size(98, 68);
            button1.TabIndex = 7;
            button1.Text = "button1";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click_1;
            // 
            // timer1
            // 
            timer1.Tick += timer1_Tick;
            // 
            // AnaMenu
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.Window;
            ClientSize = new Size(1264, 985);
            Controls.Add(button1);
            Controls.Add(panel1);
            Controls.Add(pictureBox1);
            Controls.Add(dataGridView1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "AnaMenu";
            Text = "AnaMenu";
            Load += AnaMenu_Load;
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private DataGridView dataGridView1;
        private TextBox textBox_BARKOD;
        private Label BARKOD_TEXT;
        private DateTimePicker dateTimePicker2;
        private PictureBox pictureBox1;
        private Panel panel1;
        private PictureBox pictureBox2;
        private Button button1;
        private System.Windows.Forms.Timer timer1;
    }
}