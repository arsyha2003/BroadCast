using System.Net.Sockets;
using System.Net;
using System.Xml;
using HtmlAgilityPack;
using System.Text;
using System.Windows.Forms;
namespace BroadcastServer
{
    public partial class Form1 : Form
    {
        private GoBroadcast broadcast;
        public Form1()
        {
            broadcast = new GoBroadcast();
            InitializeComponent();
        }
        public async void StartSending(object sender, EventArgs e)
        {
           broadcast.StartSendingDefaultPhrases();
           broadcast.StartSendingExtraPhrases();
           broadcast.StartSendingRarePhrases();
        }
        public void StopSending(object sender,EventArgs e)
        {
            broadcast.cts.Cancel();
            broadcast.cts = new CancellationTokenSource(); 
        }
    }
    public class GoBroadcast
    {
        private UdpClient udpServer = new UdpClient(7777);
        private RandomPhrase phrase;
        private IPEndPoint remoteEndPoint;
        public CancellationTokenSource cts;
        public GoBroadcast()
        {
            udpServer = new UdpClient();
            udpServer.EnableBroadcast = true;
            remoteEndPoint = new IPEndPoint(IPAddress.Broadcast, 8080);
            phrase = new RandomPhrase();
            cts = new CancellationTokenSource();
        }
        public async void StartSendingDefaultPhrases()
        {
            await Task.Run(() =>
            {
                while (true)
                {
                    if(cts.IsCancellationRequested) return;
                    var pr = "/defќбычное сообщение: "+phrase.SendRequest();
                    var bytes = Encoding.UTF8.GetBytes(pr);
                    udpServer.Send(bytes, bytes.Length, remoteEndPoint);
                    Thread.Sleep(new Random().Next(1000));
                }
            });
        }
        public async void StartSendingExtraPhrases()
        {
            await Task.Run(() =>
            {
                while (true)
                {
                    if (cts.IsCancellationRequested) return;
                    var pr = "/extraЁкстра сообщение: "+phrase.SendRequest();
                    var bytes = Encoding.UTF8.GetBytes(pr);
                    udpServer.Send(bytes, bytes.Length, remoteEndPoint);
                    Thread.Sleep(new Random().Next(5000));
                }
            });
        }
        public async void StartSendingRarePhrases()//мнимые градации сообщений т.к в задании требуетс€ сделать уровни подписок на рассылку
        {
            await Task.Run(() =>
            {
                while (true)
                {
                    if (cts.IsCancellationRequested) return;
                    var pr = "/rare–едкое сообщение: "+phrase.SendRequest();
                    var bytes = Encoding.UTF8.GetBytes(pr);
                    udpServer.Send(bytes, bytes.Length, remoteEndPoint);
                    Thread.Sleep(new Random().Next(3000));
                }
            });
        }
    }
    public class RandomPhrase
    {
        public HttpClient client;
        public Action<string> fillData;
        public RandomPhrase()
        {
            client = new HttpClient();
        }
        public string SendRequest()
        {
            using HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, $"https://randstuff.ru/saying/id={1}/");
            request.Headers.Add("X-Requested-With", "XMLHttpRequest");
            using HttpResponseMessage response = client.Send(request);
            var message = response.Content.ReadAsStringAsync().Result;
            HtmlAgilityPack.HtmlDocument document = new HtmlAgilityPack.HtmlDocument();
            document.LoadHtml(message);
            HtmlNode phrase = document.DocumentNode.SelectSingleNode("//table[@class='text']");
            return (phrase.InnerText);
        }
    }
}
