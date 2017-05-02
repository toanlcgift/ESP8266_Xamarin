using Sockets.Plugin;
using System;
using Xamarin.Forms;

namespace XamarinAppClient
{
    public partial class MainPage : ContentPage
    {
        TcpSocketClient client;

        public MainPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            client = new TcpSocketClient();
            try
            {
                await client.ConnectAsync("ws://192.168.1.69:81", "tcp");
                client.WriteStream.WriteByte(Convert.ToByte("ledon"));
                await client.WriteStream.FlushAsync();
                await client.DisconnectAsync();
            }
            catch { }
        }
    }
}
