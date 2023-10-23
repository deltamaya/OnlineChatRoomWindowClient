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
using System.Windows.Shapes;

namespace ChatRoom
{
    /// <summary>
    /// Interaction logic for Signup.xaml
    /// </summary>
    public partial class Signup : Window
    {
        public Signup()
        {
            InitializeComponent();
        }

        private void signup_Click(object sender, RoutedEventArgs e)
        {
            string username = uname_box.Text;
            string password = pwd_box.Text;
            Protocol.Request request = new Protocol.Request();
            request.uid= username;
            request.msg= password;
            request.service = Protocol.ServiceCode.signup;
            App.sock.Send(Encoding.UTF8.GetBytes(request.serialize()));
            var login_page=new MainWindow();
            login_page.Show();
            Hide();
        }
    }
}
