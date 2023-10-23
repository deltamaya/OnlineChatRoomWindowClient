using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Windows;

namespace ChatRoom
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static string uid;
        public static Socket sock;
        public static string inbuf;
        public static string username;
        public static Dictionary<int, string> uid_to_name;
        App()
        {
            uid_to_name = new Dictionary<int, string>();
            inbuf = new string("");
            username = "";
            IPAddress server_ip = IPAddress.Parse("127.0.0.1");
            IPEndPoint server = new IPEndPoint(server_ip, 55369);
            sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            sock.Connect(server);
            Console.WriteLine("connected");
            Console.WriteLine("tetst");
        }
    }
}
