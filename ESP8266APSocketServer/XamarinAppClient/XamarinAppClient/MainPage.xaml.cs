using SocketLite.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace XamarinAppClient
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            var tcpClient = new TcpSocketClient();
            await tcpClient.ConnectAsync("ws://192.168.1.69", 81);

            var helloWorld = "ledoff";

            var bytes = Encoding.UTF8.GetBytes(helloWorld);
            await tcpClient.WriteStream.WriteAsync(bytes, 0, bytes.Length);
            var stream = tcpClient.ReadStream;
            var response = Convert.ToString(stream);
            label.Text = response;
            tcpClient.Disconnect();
        }
    }
}
