using System.Net.Sockets;
using System.Net;
using System.Xml;
using System.Text;
namespace UdpClient
{
    public partial class Form1 : Form
    {
        Action<string> checkResponce;
        Client client;
        public Form1()
        {
            InitializeComponent();
            checkResponce = (string responce) =>
            {
                if (checkBox1.Checked == true && responce.Contains("/def"))
                    textBox1.Text += responce.Replace("/def", string.Empty) + Environment.NewLine;
                if (checkBox2.Checked == true && responce.Contains("/rare"))
                    textBox1.Text += responce.Replace("/rare", string.Empty) + Environment.NewLine;
                if(responce.Contains("/extra"))
                    textBox1.Text += responce.Replace("/extra", string.Empty) + Environment.NewLine;
            }; 
            client = new Client(checkResponce);
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            client.StartListeting();
        }
    }
    public class Client
    {
        private System.Net.Sockets.UdpClient client;
        private byte[] data = new byte[1024];
        private IPEndPoint ep;
        public Action<string> checkResponce;
        public Client(Action<string>checkResponce)
        {
            client = new System.Net.Sockets.UdpClient(8080);
            ep = new IPEndPoint(IPAddress.Loopback, 7777);
            this.checkResponce = checkResponce;
        }
        public async void StartListeting()
        {
            _=Task.Run(() =>
            {
                while (true)
                {
                    data = client.Receive(ref ep);
                    string textMessage = Encoding.UTF8.GetString(data);
                    checkResponce.Invoke(textMessage);
                }
            });
        }
    }
}
