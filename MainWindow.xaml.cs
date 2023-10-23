using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Text.Json;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;
using System.Text.Encodings;
using System.Text.Json.Serialization;
using System.Threading;
using System.Net.Http.Headers;
using System.Windows.Interop;

namespace ChatRoom
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        Chat chat;
        Signup sign;
        public MainWindow()
        {
            chat=new Chat();
            sign = new Signup();

            //ThreadPool.QueueUserWorkItem(state =>
            //{
            //    reader();
            //});
            reader();
            InitializeComponent();

        }
        private void query_name(string uid)
        {
            Console.WriteLine("query name");
            Protocol.Request request = new Protocol.Request();
            request.service = Protocol.ServiceCode.query_uname;
            request.msg = uid; 
            App.sock.Send(Encoding.UTF8.GetBytes(request.serialize()));
            byte[] buffer = new byte[1024];
            var n = App.sock.Receive(buffer, SocketFlags.None);
            App.inbuf += Encoding.UTF8.GetString(buffer, 0, n);
            var r = Protocol.Request.parse_request(ref App.inbuf);
            if(r != null&&r.service==Protocol.ServiceCode.query_uname&&r.status==Protocol.StatusCode.ok)
            {
                App.uid_to_name[int.Parse(r.uid)] = r.msg;
            }

        }
        private async void reader()
        {


            while (true)
            {
                byte[] buffer = new byte[1024];
                var n = await App.sock.ReceiveAsync(buffer, SocketFlags.None);
                App.inbuf += Encoding.UTF8.GetString(buffer, 0, n);
                //Console.WriteLine("reader:");
                var r = Protocol.Request.parse_request(ref App.inbuf);
                if (r != null)
                {
                    Console.WriteLine("parse ok");
                    switch (r.service)
                    {
                        case Protocol.ServiceCode.postmsg:

                            await Console.Out.WriteLineAsync("post msg");
                            if (r.status != Protocol.StatusCode.error)
                            {
                                while (!App.uid_to_name.ContainsKey(int.Parse(r.uid)))
                                {
                                   query_name(r.uid);
                                    Thread.Sleep(1000);
                                }
                                chat.UpdateBg($"[{App.uid_to_name[int.Parse(r.uid)]}]# {r.msg}\n");
                                
                            }
                            //Console.WriteLine("postmsg");
                            break;

                        case Protocol.ServiceCode.query_uname:

                            if (r.status != Protocol.StatusCode.error)
                            {
                                App.uid_to_name[int.Parse(r.uid)] = r.msg;
                            }
                            break;


                        case Protocol.ServiceCode.login:
                            if (r.status != Protocol.StatusCode.error)
                            {
                                App.username = r.msg;
                                App.uid = r.uid;
                                chat.Show();
                                App.uid_to_name[int.Parse(r.uid)] = App.username;
                                Console.WriteLine($"welcome {App.username}");
                            }
                            else
                            {
                                MessageBox.Show("用户名或者密码错误！", "错误");
                            }
                            break;


                        case Protocol.ServiceCode.signup:

                            if (r.status != Protocol.StatusCode.error)
                            {
                                MessageBox.Show($"你的uid是：{r.uid}，请保存好！","注册成功");
                            }
                            else
                            {
                                MessageBox.Show("用户名或者密码格式不正确","错误");
                            }
                            break;


                    }
                }
                else
                {
                    Console.WriteLine("parse error");
                }
            }


        }
        private void signup_Click(object sender, RoutedEventArgs e)
        {
            
            //this.Hide();
            //signup_page.ShowDialog();
            sign.Show();
        }
        private void login_Click(object sender, RoutedEventArgs e)
        {

            string uid = uid_box.Text;
            string pwd = pwd_box.Text;
            Protocol.Request request = new Protocol.Request();
            request.uid = uid;
            request.msg = pwd;
            request.service = Protocol.ServiceCode.login;
            Console.WriteLine(request.serialize());

            App.sock.Send(Encoding.UTF8.GetBytes(request.serialize()));
            Console.WriteLine("sended msg");
        }
    }
    public class Protocol
    {
        public static string SEP = "##";
        public enum ServiceCode : byte
        {
            postmsg,
            signup,
            login,
            query_uname
        };
        public enum StatusCode : byte
        {
            ok,
            error
        };
        public class Request
        {
            public string uid { get; set; }
            public string msg { get; set; }
            public ServiceCode service { get; set; }
            public StatusCode status { get; set; }
            public string towhom { get; set; }
            public Request()
            {
                uid = new string("");
                msg = new string("");
                towhom = new string("");
                service = ServiceCode.postmsg; status = StatusCode.ok;
            }
            public Request(string jsonstr)
            {
                deserialize(jsonstr);
            }
            public static Request? parse_request(ref string str)
            {
                int idx = str.IndexOf(SEP);
                if (idx != -1)
                {
                    Request request = new Request(str.Substring(0, idx));
                    Console.WriteLine($"before parse: {str}");

                    str = str.Remove(0, idx + SEP.Length);
                    Console.WriteLine($"after parse: {str}");
                    return request;
                }
                return null;
            }
            public string serialize()
            {
                return JsonSerializer.Serialize(this)+SEP;
            }
            public void deserialize(string str)
            {
                Request temp = (JsonSerializer.Deserialize<Request>(str))!;

                this.uid = temp.uid;
                this.msg = temp.msg;
                this.service = temp.service;
                this.status = temp.status;
                this.towhom = temp.towhom;
            }
        }
    }


}
