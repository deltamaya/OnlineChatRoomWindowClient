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
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ChatRoom
{
    /// <summary>
    /// Interaction logic for Chat.xaml
    /// </summary>
    public partial class Chat : Window
    {
        
        public static string[] history { get; set; }
        public Chat()
        {
            history = new string[] { };
            InitializeComponent();
        }

         public void UpdateBg(string str)
        {
            msg.Height += 30;
            Chat.history.Append(str);
            background.AppendText(str);
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (msg.Text.IndexOf("##") != -1)
            {
                MessageBox.Show("不能发送带有‘##’的文字");
                return;
            }
            Protocol.Request request = new Protocol.Request();
            request.uid = App.uid;
            request.service = Protocol.ServiceCode.postmsg;
            request.msg = msg.Text;
            msg.Text= string.Empty;
            App.sock.Send(Encoding.UTF8.GetBytes(request.serialize()));

        }
    }
}
