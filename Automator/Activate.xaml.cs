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

namespace Automator
{
    /// <summary>
    /// Interaction logic for Activate.xaml
    /// </summary>
    public partial class Activate : Window
    {
        public Activate()
        {
            InitializeComponent();
        }

        private void Activation(object sender, RoutedEventArgs e)
        {
            int day = DateTime.Now.Day;
            int year = DateTime.Now.Year;
            int theCode = year + day*day;
            string theCodeS = theCode.ToString();
            if (!theCodeS.Equals(Code.Text))
            {
                MessageBox.Show("Wrong Activate code");
                return;
            }
            Expiry ex = new Expiry();
            ex.isRegistered = true;
            ex.activationCode = theCodeS;
            Utility.serialize("License", ex);
            this.Close();
            
        }
    }
}
