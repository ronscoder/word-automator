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
using System.IO;
namespace Automator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static string msg;
        //  private DB_AutomatorEntities dbcon;
        public static MainWindow mainWind;
        public User currentUser;
        public MainWindow()
        {
            InitializeComponent();
            UsConnection.templateFile = Automator.Properties.Settings.Default.TemplateFile_birth;
            mainWind = this;
            this.Height = System.Windows.SystemParameters.PrimaryScreenHeight;
            this.Width = System.Windows.SystemParameters.PrimaryScreenWidth;
        }

        private void TreeViewItem_MouseUp_1(object sender, MouseButtonEventArgs e)
        {
            ContentArea.Content = new organisation1();
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            ContentArea.Content = new Welcome();
        }
        public void screen0()
        {
            ContentArea.Content = new newPerson();
            MainMenu.Visibility = Visibility.Visible;
        }
        private void Default_click(object sender, RoutedEventArgs e)
        {
            ContentArea.Content = new UsConnection();
        }

        private void New_person(object sender, RoutedEventArgs e)
        {
            ContentArea.Content = new newPerson();
        }

        private void Reports(object sender, RoutedEventArgs e)
        {
            ContentArea.Content = new Persons();
        }

        private void Close(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Welcome_MouseUp(object sender, MouseButtonEventArgs e)
        {

        }

        private void Label_MouseDown(object sender, MouseButtonEventArgs e)
        {
            AboutBox ab = new AboutBox();
            ab.Show();
        }

        private void User_setting(object sender, RoutedEventArgs e)
        {
            ContentArea.Content = new AddUser();
        }
        public void LoginOk(User userLogin)
        {
            if(! File.Exists("Last_merge"))
            merge_records();
            currentUser = userLogin;
            screen0();
        }
        private void merge_records()
        {
            // 2 Current DB_AutomatorEntities
            // 1 automator DB_AutomatorEntities1
            // 0 DB_AutomatorEntities14
            using (DB_AutomatorEntities14 dbsource1 = new DB_AutomatorEntities14())
            {

                List<person> persons1 = dbsource1.persons.ToList<person>();
                DB_AutomatorEntities1 dbsource2 = new DB_AutomatorEntities1();

                string lastpers = dbsource2.persons.Max(p=>p.Registration_number);
                List<person> person2 = dbsource2.persons.ToList<person>();
                //persons1.AddRange(person2);
                List<person> persons = persons1.ToList();
                persons.AddRange(person2);
                if (persons1.Count == 0)
                    return;



                db_automator_2Entities dbtarget = new db_automator_2Entities();
                Status lastSerial1 = new Status();
                Status lastSerial2 = new Status();
                Status lastReg = new Status();

                string[] regs = lastpers.Split('/');
            if(regs.Length != 0)
            {
                lastReg.VALUE_INT = int.Parse(regs[0]);
                lastReg.VALUE_CHAR = regs[1];
            }


                int serial_index_birth = 1;
                int serial_index_death = 1;

                foreach (person p in persons)
                {
                    if (p.Category == "Birth")
                    {
                        p.Person_id = serial_index_birth;
                        serial_index_birth++;
                    }
                    else
                    {
                        p.Person_id = serial_index_death;
                        serial_index_death++;
                    }

                }
                lastSerial1.OBJECT = "LAST_SNR1";
                lastSerial1.VALUE_INT = serial_index_birth - 1;
                lastSerial2.OBJECT = "LAST_SNR2";
                lastSerial2.VALUE_INT = serial_index_birth - 1;
                lastReg.OBJECT = "LAST_REG";
                dbtarget.Entry(lastSerial1).State = System.Data.Entity.EntityState.Modified;
                dbtarget.Entry(lastSerial2).State = System.Data.Entity.EntityState.Modified;
                dbtarget.Entry(lastReg).State = System.Data.Entity.EntityState.Modified;
                dbtarget.persons.AddRange(persons);
                try{
                    dbtarget.SaveChanges();
                }
                catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
                {
                    Exception raise = dbEx;
                    foreach (var validationErrors in dbEx.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            string message = string.Format("{0}:{1}",
                                validationErrors.Entry.Entity.ToString(),
                                validationError.ErrorMessage);
                            // raise a new exception nesting  
                            // the current instance as InnerException  
                            raise = new InvalidOperationException(message, raise);
                        }
                    }
                    throw raise; 
                }
                FileStream fs = File.Create("Last_merge");
                fs.Close();
              }
        }
        private void About_click(object sender, RoutedEventArgs e)
        {
            (new AboutBox()).ShowDialog();
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
          MainScroll.Height = e.NewSize.Height - 80;
          MainScroll.Width = e.NewSize.Width - 50;

        }

        private void btn_start(object sender, RoutedEventArgs e)
        {

            if (!Processor.setup_initial())
                {
                MessageBox.Show("Set up Failed. Exiting application", "Set up failed", MessageBoxButton.OK, MessageBoxImage.Error);
                this.Close();
            }
            //Login check first
            Welcome.Visibility = Visibility.Collapsed;
            ContentArea.Visibility = Visibility.Visible;
            this.ContentArea.Content = new Login();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void Click_Instruction(object sender, RoutedEventArgs e)
        {
            this.ContentArea.Content = new Instructions();
        }

    }
}
