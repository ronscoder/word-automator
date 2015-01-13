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

namespace Automator
{
    /// <summary>
    /// Interaction logic for AddUser.xaml
    /// </summary>
    public partial class AddUser : UserControl
    {
        public AddUser()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            using (var db = new db_automator_2Entities())
            {
                db.SaveChanges();
            }

            //Username 10 char
            //password 15 char
            //Type 5
        }
        private void cbType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            //Cancel
            MainWindow.mainWind.screen0();
        }

        private void ClickCreateUser(object sender, RoutedEventArgs e)
        {
            if (txtNewUsername.Text.Trim() == string.Empty)
            {
                txtNewUsername.BorderBrush = Brushes.Red;
                MessageBox.Show("Enter a valid username");
                return;
            }
            txtNewUsername.BorderBrush = Brushes.Black;

            if(txtNewPassword.Text.Trim() == string.Empty)
            {
                txtNewPassword.BorderBrush = Brushes.Red;
                MessageBox.Show("Enter a valid password");
                return;
            }
            txtNewPassword.BorderBrush = Brushes.Black;
            this.Cursor = Cursors.Wait;
            using (var dbcontext = new db_automator_2Entities())
            {
                User newUser = new User();
                newUser.Username = txtNewUsername.Text;
                newUser.Password = txtNewPassword.Text;
                newUser.DisplayName = txtNewDisplayName.Text;
                dbcontext.Users.Add(newUser);
                dbcontext.SaveChanges();
            }
            this.Cursor = Cursors.Arrow;
            MessageBox.Show("Success. User created", "User creation", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ClickChangeUser(object sender, RoutedEventArgs e)
        {
            if (txtUsername.Text.Trim() == string.Empty)
            {
                txtUsername.BorderBrush = Brushes.Red;
                MessageBox.Show("Enter a valid username");
                return;
            }
            txtUsername.BorderBrush = Brushes.Black;

            if (txtChangePassword_old.Password.Trim() == string.Empty)
            {
                txtChangePassword_old.BorderBrush = Brushes.Red;
                MessageBox.Show("Enter a valid password");
                return;
            }
            txtChangePassword_old.BorderBrush = Brushes.Black;

            if (txtChangePassword1.Password.Trim() == string.Empty)
            {
                txtChangePassword1.BorderBrush = Brushes.Red;
                MessageBox.Show("Enter a valid password");
                return;
            }
            txtChangePassword1.BorderBrush = Brushes.Black;

            if (txtChangePassword2.Password.Trim() == string.Empty)
            {
                txtChangePassword2.BorderBrush = Brushes.Red;
                MessageBox.Show("Enter a valid password");
                return;
            }

            if (!txtChangePassword1.Password.Equals(txtChangePassword2.Password))
            {
                MessageBox.Show("Password not matched, re-enter!");
                return;
            }
            using (var dbcontext = new db_automator_2Entities())
            {
                var changeUser = (from user in dbcontext.Users
                                 where user.Username == txtUsername.Text.Trim()
                                 select user).FirstOrDefault<User>();
                if (changeUser == null)
                {
                    MessageBox.Show("Username does not exist in database. Create new or re-enter","No Username", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }
                if(!changeUser.Password.Equals(txtChangePassword_old.Password.Trim())){
                    txtUsername.BorderBrush = Brushes.Red;
                    MessageBox.Show("Wrong password for username:" + txtUsername.Text,"User Create", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }
                //Validated, update password
                changeUser.Password = txtChangePassword1.Password;
                dbcontext.SaveChanges();
            }

            MessageBox.Show("Success. User updated", "User Update", MessageBoxButton.OK, MessageBoxImage.Information);
            txtChangePassword2.BorderBrush =  txtUsername.BorderBrush = Brushes.Black;
        }

        private void event_users_expand(object sender, RoutedEventArgs e)
        {
            this.Cursor = Cursors.Wait;
            using (var db = new db_automator_2Entities())
            {
                var userss = from us in db.Users
                             select( new { us.Username, us.DisplayName });
                users.ItemsSource = userss.ToList(); 
                    //db.Users.Select(x => new { x.Username, x.DisplayName }).ToList();
            }
            this.Cursor = Cursors.Arrow;
        }

        private void txtNewUsername_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void users_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {

        }
    }
}
