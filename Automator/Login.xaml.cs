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
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
namespace Automator
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : UserControl
    {
        public User currentUser;
        public bool access;
        public Login()
        {
            InitializeComponent();
        }

        private void Login_click(object sender, RoutedEventArgs e)
        {
            this.Cursor = Cursors.Wait;
            if (txtUsername.Text == "")
            {
                MessageBox.Show("Enter a username");
                return;
            }
            if (txtPassword.Password == "")
            {
                MessageBox.Show("Enter password");
            }

            using (var dbcontext = new db_automator_2Entities())
            {
                bool needquery = true ;
                log_user user0 = new log_user();
                if (File.Exists("appusr.log"))
                {
                    user0 = (log_user)Utility.deserialize("appusr.log");

                    
                    if(user0.username.Trim().ToLower().Equals(txtUsername.Text.ToLower()))
                    {
                        if (user0.password.Trim().Equals(txtPassword.Password))
                        {//OK. Proceed safe
                            needquery = false;
                            currentUser = new User();
                            currentUser.Username = user0.username;
                            currentUser.Password = user0.password;
                        }
                    }
                }
                if (needquery) { 
                    var query = (from us in dbcontext.Users
                                 where us.Username == txtUsername.Text
                                 select us).SingleOrDefault<User>();
                    if (query == null)
                    {
                        MessageBox.Show("Invalid username");
                        this.Cursor = Cursors.Arrow;
                        return;
                    }
                    if (!query.Password.Equals(txtPassword.Password))
                    {
                        MessageBox.Show("Wrong passowrd");
                        txtPassword.Clear();
                        this.Cursor = Cursors.Arrow;
                        return;
                    }
                    if (query.Active == false)
                    {
                        MessageBox.Show("Username is not active. Contact admin.");
                        txtPassword.Clear();
                        this.Cursor = Cursors.Arrow;
                        return;
                    }
                    currentUser = query as User;
                }

                if (needquery) { 
                log_user last_user = new log_user();

                last_user.username = currentUser.Username;
                last_user.password = currentUser.Password;
                Utility.serialize("appusr.log", last_user);
                }
                access = true;
                MainWindow.mainWind.LoginOk(currentUser);
            }
            this.Cursor = Cursors.Arrow;
        }

        private void txtUsername_Loaded(object sender, RoutedEventArgs e)
        {
            txtUsername.Focus();
        }
    }
    [Serializable()]
    class log_user
    {
        public string username { get; set; }
        public string password { get; set; }
    }


    class Utility
    {
        public static void serialize(string filename, object obj)
        {
            try
            {
                Stream file_user = File.Create(filename);
                BinaryFormatter ser = new BinaryFormatter();
                ser.Serialize(file_user, obj);
                file_user.Close();
            }
            catch
            {

            }
        }
        public static object deserialize(string fileName){
                    Stream st = File.OpenRead(fileName);
                    BinaryFormatter deser = new BinaryFormatter();
                    object obj= deser.Deserialize(st);
                    st.Close();
                    return obj;
        }
        static public void wait(int wait)
        {
            Progressing prog = new Progressing();
            prog.ShowDialog();
            Thread.Sleep(wait);
            prog.Close();
        }
    }
}
