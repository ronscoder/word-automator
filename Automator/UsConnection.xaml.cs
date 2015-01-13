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
using System.Windows.Forms;
namespace Automator
{
    /// <summary>
    /// Interaction logic for UsConnection.xaml
    /// </summary>
    public partial class UsConnection :System.Windows.Controls.UserControl
    {
        public static string templateFile;
        public static string dateformat;
        CustomData cust;
        public UsConnection()
        {
            InitializeComponent();
            tboxTemplate_birth.Text = Automator.Properties.Settings.Default.TemplateFile_birth ;
            tboxTemplate_death.Text = Automator.Properties.Settings.Default.TemplateFile_death;
            tboxDateFormat.Text = Automator.Properties.Settings.Default.dateFormat;
            if (tboxDateFormat.Text != string.Empty)
            {
                List<string> ls;
                lbDate.Content = "Current Display Format: " + Processor.getOrdinal(DateTime.Now, tboxDateFormat.Text, out ls);
            }
            cust = CustomData.getCustomData();
           // this.DataContext = cust;
            txtDateFormatBirth.Text = cust.format_date_of_birth;
            txtDateFormatRep.Text = cust.format_report_dates;
            txtDateFormatSrn.Text = cust.format_dates_screen;
            txtDistrict.Text = cust.default_district;
            txtLocal.Text = cust.default_localArea;
            txtTahsil.Text = cust.default_tahsilBlock;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog op = new OpenFileDialog();
            if (op.ShowDialog() == DialogResult.OK)
            {
              //tboxTemplate.Text = templateFile = Automator.Properties.Settings.Default.TemplateFile_birth = op.FileName;
              Automator.Properties.Settings.Default.Save();
            }
        }
        //Save
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
           Automator.Properties.Settings.Default.dateFormat = tboxDateFormat.Text;
           Automator.Properties.Settings.Default.Save();
           if (tboxDateFormat.Text != string.Empty)
           {
               List<string> ls;
               lbDate.Content = "Current Display Format: " + Processor.getOrdinal(DateTime.Now, tboxDateFormat.Text, out ls);
           }
           cust.format_date_of_birth = txtDateFormatBirth.Text;
           cust.format_report_dates = txtDateFormatRep.Text;
           cust.format_dates_screen = txtDateFormatSrn.Text;
           cust.default_district = txtDistrict.Text;
           cust.default_localArea = txtLocal.Text;
           cust.default_tahsilBlock = txtTahsil.Text;
           cust.save();
        }
    }
}
