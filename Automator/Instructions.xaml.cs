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
    /// Interaction logic for Instructions.xaml
    /// </summary>
    public partial class Instructions : UserControl
    {
        public Instructions()
        {
            InitializeComponent();
            if (System.IO.File.Exists("license"))
                btnAct.Visibility = System.Windows.Visibility.Hidden;
        }

        private void Activate(object sender, RoutedEventArgs e)
        {
            (new Activate()).ShowDialog();
            
        }
    }
}
