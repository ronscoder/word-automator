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
using System.Data.Entity;
using System.Threading;
using System.IO;

namespace Automator
{
    /// <summary>
    /// Interaction logic for newPerson.xaml
    /// </summary>
    public partial class newPerson : UserControl
    {
      //  CollectionViewSource personViewSource;
        person nuPers;
        public newPerson()
        {
            InitializeComponent();
            //Load your data here and assign the result to the CollectionViewSource.
//            personViewSource = (System.Windows.Data.CollectionViewSource)this.FindResource("personViewSource");
            newRecord();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //New
            newRecord();

        }
        void newRecord()
        {
            isSaved = false ;
          //  btnPrint.IsEnabled = true;
            btnSave.IsEnabled = true;
            msg_new.Visibility = Visibility.Hidden;
            clear_text(Grid1);
            nuPers = new person();
            setDefault();
            //btnSave.IsEnabled = true;
            isSaved = false;
        }
        Status regno;
        void setDefault()
        {
            Fa.IsChecked = true;
            wpHusband.Visibility = Visibility.Collapsed;
            wpRadio.Visibility = Visibility.Collapsed;
            cbItemBirth.IsSelected = true;
            SexSel.SelectedIndex = -1;
            date_of_issueDatePicker.SelectedDate = date_of_registrationDatePicker.SelectedDate = DateTime.Now;
            date_of_birthDatePicker.SelectedDate = null;
            CustomData custData = CustomData.getCustomData();
            districtTextBox.Text = custData.default_district;
            local_areaTextBox.Text = custData.default_localArea;
            tahsil_BlockTextBox.Text = custData.default_tahsilBlock;
            string yy = DateTime.Now.ToString("yy");
            
            using (db_automator_2Entities dbcon = new db_automator_2Entities())
            {
                regno = dbcon.Status.Where(b => b.OBJECT == "LAST_REG").FirstOrDefault();
                if (yy == regno.VALUE_CHAR.Trim())
                    regno.VALUE_INT ++;
                else
                    regno.VALUE_INT = 1;

                registration_numberTextBox.Text = regno.VALUE_INT.ToString() + '/' + yy;

            }
            /*
            if(File.Exists("appdata_current"))
            {
                dcurrent = (current) Utility.deserialize("appdata_current");
                if (yy == dcurrent.yy)
                    dcurrent.number++;
                else
                    dcurrent.number = 1;
                registration_numberTextBox.Text = dcurrent.number.ToString() + '/' + yy;
            }
            else{
                dcurrent.number = 1;
                dcurrent.yy = yy;
                registration_numberTextBox.Text = "1/" + dcurrent.yy;
                Utility.serialize("appdata_current", dcurrent);
            }
            */

        }
    [Serializable()]
        struct current
        {
            public int number;
            public string yy;
        }
        bool validate()
        {
            
            if (string.IsNullOrEmpty(registration_numberTextBox.Text))
            {
                MessageBox.Show("Enter registration number");
                return false;
            }
            using (db_automator_2Entities dbcon = new db_automator_2Entities())
            {
                var count = dbcon.persons.Count(o => o.Registration_number == registration_numberTextBox.Text);
                if(count > 0)
                {
                    MessageBox.Show("Registration number already used; Enter a new one!");
                    return false;
                }
            }
            if (string.IsNullOrEmpty(cbCateogry.Text))
            {
                MessageBox.Show("Select a category");
                return false;
            }

            if (string.IsNullOrEmpty(sexTextBox.Text))
            {
                MessageBox.Show("Select sex");
                return false;
            }
            if (date_of_birthDatePicker.SelectedDate == null || date_of_issueDatePicker.SelectedDate == null || date_of_registrationDatePicker.SelectedDate == null)
             {
                 MessageBox.Show("Date cannot be left blank!");
                 return false;
             }
            return true;
                
        }
        private void clear_text(DependencyObject obj)
        {
             for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                 if(obj is TextBox){
                     ((TextBox)obj).Text = null;
                 }
                 clear_text(VisualTreeHelper.GetChild(obj, i));
            }
        }

        //Save        
        bool isSaved;
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

            if (validate()){
                save();

                MessageBox.Show("Record updated successfully. Proceed to print.", "Alert update", MessageBoxButton.OK, MessageBoxImage.Information);
            }
                else
                return;
        }

        DateTime get_date(DateTime inp){
            return new DateTime(inp.Year, inp.Month, inp.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
        }
        void save()
        {
            isSaved = true;
            //Utility.serialize("appdata_current", dcurrent);
            btnSave.IsEnabled = false;
           // btnPrint.IsEnabled = false;
            msg_new.Content = "Click New button to enter new record";
            msg_new.Visibility = Visibility.Visible;
            this.Cursor = Cursors.Wait;
            nuPers.Address_of_parents_birth__death = address_of_parents__birth_TextBox.Text;
            nuPers.Category = cbCateogry.Text;
            nuPers.Created_by = get_user();
            nuPers.Creation_date = get_date(DateTime.Now);
            nuPers.Date_of_birth_death = get_date(date_of_birthDatePicker.SelectedDate.Value);
            nuPers.Date_of_issue = get_date(date_of_issueDatePicker.SelectedDate.Value);
            nuPers.Date_of_registration = get_date(date_of_registrationDatePicker.SelectedDate.Value);
            nuPers.District = districtTextBox.Text;
            nuPers.First_name = first_nameTextBox.Text;
            nuPers.Issuing_authority = getIssueAuth();
            nuPers.Last_name = last_nameTextBox.Text;
            nuPers.Local_area = local_areaTextBox.Text;
            nuPers.Middle_name = middle_nameTextBox.Text;
            nuPers.Name_of_father = name_of_fatherTextBox.Text;
            nuPers.Name_of_Husband = txtHusband.Text;
            nuPers.Name_of_mother = name_of_motherTextBox.Text;
            nuPers.Permanent_address_of_parents_deceased = permanent_address_of_parentsTextBox.Text;
            nuPers.Place_of_birth_death = place_of_birthTextBox.Text.Trim();
            nuPers.Registration_number = registration_numberTextBox.Text;
            nuPers.Remarks = remarksTextBlock.Text;
            nuPers.Sex = sexTextBox.Text;
            nuPers.Tahsil_Block = tahsil_BlockTextBox.Text;
           // if(isSaved == false)
            using (db_automator_2Entities db = new db_automator_2Entities())
            {
                Status last_serial1 = db.Status.Where(ser => ser.OBJECT == "LAST_SNR1").FirstOrDefault();
                Status last_serial2 = db.Status.Where(ser => ser.OBJECT == "LAST_SNR2").FirstOrDefault();
                int int_last_serial1 = int.Parse(last_serial1.VALUE_INT.ToString());
                int int_last_serial2 = int.Parse(last_serial2.VALUE_INT.ToString());
                if(nuPers.Person_id == 0) //is new
                {
                    if (nuPers.Category == "Birth")
                    {
                        last_serial1.VALUE_INT = nuPers.Person_id = int_last_serial1 + 1;
                        db.Entry(last_serial1).State = EntityState.Modified;
                    }
                    else
                    {
                        last_serial2.VALUE_INT = nuPers.Person_id = int_last_serial2 + 1;
                        db.Entry(last_serial2).State = EntityState.Modified;

                    }

                    db.Entry(nuPers).State = EntityState.Added;

                    db.Entry(regno).State = EntityState.Modified;
                }
                else
                   db.Entry(nuPers).State = EntityState.Modified; 

                if (db.persons.Count() == 0)
                {
                }
                try {
                    db.SaveChanges();
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
 
                
            }
          //  isSaved = true;
            this.Cursor = Cursors.Arrow;
        }
        string getIssueAuth()
        {
            return "Default";
        }
        string get_user()
        {
            return MainWindow.mainWind.currentUser.Username;
        }

        //Print
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            this.Cursor = Cursors.Wait;
            if (!isSaved)
            {
                MessageBox.Show("Save the record first.");
                this.Cursor = Cursors.Arrow;
                return;
            }

            //Save and print
            /* if (validate())
                save();
            else
            {
                this.Cursor = Cursors.Arrow;
                return;
            } 
                
            */
            if (MessageBox.Show("Do you want to print the certificate?", "Confirm print", MessageBoxButton.YesNo,MessageBoxImage.Question) != MessageBoxResult.Yes)
            {
                this.Cursor = Cursors.Arrow;
                return;
            }
                
         Processor proc = Processor.getInstance(nuPers, new Authority());
         proc.print();
         this.Cursor = Cursors.Arrow;
         
        }

        private void SexSel_DropDownClosed(object sender, EventArgs e)
        {
            sexTextBox.Text = SexSel.Text;
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            permanent_address_of_parentsTextBox.Text = address_of_parents__birth_TextBox.Text;
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (!this.IsLoaded)
                return;
            setRadio();
                
        }
        void setRadio()
        {
            wpFather.Visibility = Visibility.Collapsed;
            wpHusband.Visibility = Visibility.Collapsed;
            if (Fa.IsChecked == true)
            {
                wpFather.Visibility = Visibility.Visible;
                txtHusband.Clear();
            }
            else
            {
                wpHusband.Visibility = Visibility.Visible;
                name_of_fatherTextBox.Clear();
            }
        }
        //Death
        Object add;
        Object Date;
        Object Perm;
        Object place;
        private void ComboBoxItem_Selected(object sender, RoutedEventArgs e)
        {
            if (!this.IsLoaded)
                return;
            setDeath();
        
        }
        void setDeath()
        {
            wpRadio.Visibility = Visibility.Visible;
            add = lbAdd.Content;
            Date = lbDateBirth.Content;
            Perm = lbPermAdd.Content;
            place = lbPlaceBirth.Content;
            //Address of the deceased at the time of Death:
            lbAdd.Content = "Address of the deceased at the time of Death:";
            lbDateBirth.Content = "Date of death:";
            lbPermAdd.Content = "Permanent address of the deceased:";
            lbPlaceBirth.Content = "Place of death:";
        }
        //Birth
        private void ComboBoxItem_Selected_1(object sender, RoutedEventArgs e)
        {

            if (!this.IsLoaded)
                return;
            if (add == null)
                return;
            wpRadio.Visibility = Visibility.Collapsed;
            Fa.IsChecked = true;
            lbAdd.Content = add;
            lbDateBirth.Content = Date;
            lbPermAdd.Content = Perm;
            lbPlaceBirth.Content = place;
            txtHusband.Clear();
        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
           // Scroller.Height = e.NewSize.Height;
        }
        private void OpenCalender(object sender, RoutedEventArgs e)
        {
            ((DatePicker)sender).IsDropDownOpen = true;
        }

        private void CloseCalender(object sender, MouseEventArgs e)
        {
            ((DatePicker)sender).IsDropDownOpen = false;
        }

        private void CloseCalen(object sender, RoutedEventArgs e)
        {
            ((DatePicker)sender).IsDropDownOpen = false;
        }

        private void OpenCalender(object sender, MouseEventArgs e)
        {
            ((DatePicker)sender).IsDropDownOpen = true;
        }

        public bool Edit(person nuPers)
        {
            cbItemDeath.IsSelected = true;
            if (nuPers.Category == "Death")
            {
             setDeath();
                if (nuPers.Name_of_father.Trim() != "")
                {
                Fa.IsChecked = true;
                setRadio();
                }
                else if(nuPers.Name_of_Husband.Trim() != "")
                {
                    Hu.IsChecked = true;
                    setRadio();
                }
            }

            this.nuPers = nuPers;
            address_of_parents__birth_TextBox.Text = nuPers.Address_of_parents_birth__death;
            cbCateogry.Text = nuPers.Category.Trim();
//            get_user() = nuPers.Created_by;
//            nuPers.Creation_date                    = get_date(DateTime.Now);
            date_of_birthDatePicker.SelectedDate = nuPers.Date_of_birth_death;
            date_of_issueDatePicker.SelectedDate = nuPers.Date_of_issue;
            date_of_registrationDatePicker.SelectedDate = nuPers.Date_of_registration;
            districtTextBox.Text = nuPers.District;
            first_nameTextBox.Text = nuPers.First_name;
           // nuPers.Issuing_authority = getIssueAuth();
            last_nameTextBox.Text = nuPers.Last_name;
            local_areaTextBox.Text = nuPers.Local_area;
            middle_nameTextBox.Text = nuPers.Middle_name;
            name_of_fatherTextBox.Text = nuPers.Name_of_father;
            txtHusband.Text = nuPers.Name_of_Husband;
            name_of_motherTextBox.Text = nuPers.Name_of_mother;
            permanent_address_of_parentsTextBox.Text = nuPers.Permanent_address_of_parents_deceased;
            place_of_birthTextBox.Text = nuPers.Place_of_birth_death;
            registration_numberTextBox.Text = nuPers.Registration_number;
            remarksTextBlock.Text = nuPers.Remarks;
            SexSel.Text = sexTextBox.Text = nuPers.Sex.Trim();
            tahsil_BlockTextBox.Text = nuPers.Tahsil_Block;
            return true;
        }

        private void registration_numberTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            string[] regs = registration_numberTextBox.Text.Split('/');
            if(regs.Length == 0)
            {
                MessageBox.Show("Registration number has invalid characters");
                registration_numberTextBox.Clear();
                return;
            }
            regno.VALUE_INT = int.Parse(regs[0]);
            regno.VALUE_CHAR = regs[1];
          //  dcurrent.number = int.Parse(regs[0]);
          //  dcurrent.yy = regs[1];
        }
          
    }
}
