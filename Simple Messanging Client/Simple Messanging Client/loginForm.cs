using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Simple_Messanging_Client
{
    public partial class loginForm : Form
    {
        public loginForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            mainForm.dpServer = serverBox.Text;
            mainForm.userName = namebox.Text;
            mainForm.password = passbox.Text;
            mainForm.kirim(">>>masuk<<<");

            mainForm.mform.Show();
            mainForm.mform.chatBoxTimer.Enabled = true;
            this.Close();
        }

        private void loginForm_Load(object sender, EventArgs e)
        {
            //Console.WriteLine("focusing on theis form");
            //mainForm.mform.Close();
            mainForm.mform.Hide();
            this.Focus();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            mainForm.mform.Hide();
        }
    }
}
