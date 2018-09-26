using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Sockets;


namespace Simple_Messanging_Client
{
    public partial class mainForm : Form
    {
        public int dpPort = 7777;
        public static string dpServer = "localhost";
        public static int bufferSize = 512000;
        public int timeout = 5000;
        public static string userName = "noName";
        public static string password = "";
        public static mainForm mform;
        public static System.Net.EndPoint laddr;
        public static Socket listener;

        public static bool statusAktif = true;
        public static List<string> chatList = new List<string>();
        public static bool readyToClose = false;

        public static string TempText = "";

        public Socket sock;
        public mainForm()
        {
            InitializeComponent();
            
        }

        public static string[] dekode(string data)
        {
            return data.Split('|');
        }

        public static string enkode(string[] data)
        {
            return string.Join("|", data);
        }

        public static void kirim(string d)
        {
            //byte data = Convert.ToByte(d);
            //byte[] datas = {data, Convert.ToByte(userName), Convert.ToByte(password)};
            //mainForm.mform.sock.Send(datas);
            //Encoding data = Encoding.GetEncoding(d);

            List<byte> dataL = new List<byte>();
            string[] dd = {userName, password, d};
            //string dd = enkode({userName, password, d});
            string ddd = mainForm.enkode(dd);
            foreach (Char c in ddd)
            {
                dataL.Add(Convert.ToByte(c));
            }
            //mform.sock.Send(dataL.ToArray());
            //mainForm.mform.sock.Send(dataL.ToArray());

            mainForm.mform.sock.SendTo(dataL.ToArray(), mform.sock.RemoteEndPoint);
            
        }

        public static void getMessageAndAppliedIt()
        {
            byte[] data = new byte[mainForm.bufferSize];
            int recv = listener.Receive(data);
            string kalimat = "";
            kalimat = System.Text.Encoding.UTF8.GetString(data);
            string[] hasil = dekode(kalimat);
            chatList.Add(hasil[0] + " : " + hasil[1]);
            if (hasil[0] == "server" && hasil[1] == ">>>exit<<<")
            {
                mainForm.readyToClose = true;
                mainForm.mform.Close();
            }
            mainForm.TempText = kalimat;
           // mainForm.mform.ChatBox.Items.Add(kalimat);//musti beking cross multi thread dulu kata noh -_-
        }

        void thGetMSG()
        {

            while (statusAktif == true)
            {
                mainForm.getMessageAndAppliedIt();
            }
        }

        void OnUnLoad(Object sender, FormClosingEventArgs e)
        {
            statusAktif = false;
            mainForm.kirim(">>>RequestingTermination<<<");
            //int count = 0;
            //while (mainForm.readyToClose == false)
            //{
            //    //just keep looping
            //    statusAktif = false;
            //    if (count > 5000)
            //    {
            //        break;
            //    }
            //    count++;
            //}
            mainForm.listener.Shutdown(SocketShutdown.Receive);
            mainForm.listener.Close();
            sock.Close();
        }

        private void mainForm_Load(object sender, EventArgs e)
        {
            mainForm.mform = this;

            this.FormClosing += new FormClosingEventHandler(OnUnLoad);

            //inisialisasi socket
           //Console.WriteLine("hhmm");
           // sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IPv4);
            //sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);//kelihatannya dia terima kalo TCP noh
            sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            //Console.WriteLine("asd");
            sock.Connect(dpServer, dpPort);
            //sock.SendTimeout = timeout;
            mainForm.laddr = sock.LocalEndPoint;
            listener = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            listener.Bind(laddr);
           // listener.ReceiveTimeout = timeout;


            loginForm masukForm = new loginForm();
            masukForm.Show();
            this.Hide();

            //memulai thread yang baru
            System.Threading.Thread th = new System.Threading.Thread(thGetMSG);
            th.SetApartmentState(ApartmentState.STA);
            th.Start();
            
            

        }

        private void sendButton_Click(object sender, EventArgs e)
        {
            string isi = sendbox.Text;
            mainForm.kirim(isi);
            sendbox.Text = "";
            //Console.WriteLine("sending message");//causing freeze after recieving the message. Kemungkinan karna pa threading stw
            sendbox.Focus();
        }

        private void chatBoxTimer_Tick(object sender, EventArgs e)
        {

            //memulai thread yang baru
            //System.Threading.Thread th = new System.Threading.Thread(getMessageAndAppliedIt);
            //th.SetApartmentState(ApartmentState.STA);
            //th.Start();

            label1.Text = mainForm.TempText;
            msgCountLabel.Text = mainForm.chatList.Count.ToString();

            int lcdb = mainForm.chatList.Count;
            int lcb = ChatBox.Items.Count;

            if (lcdb > lcb)
            {
                int bsr = lcdb - lcb;
                for (int i = lcb; i < lcdb; i++)
                {
                    ChatBox.Items.Add(chatList[i]);
                }
            }
        }
    }
}
