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
using System.Collections.ObjectModel;
using System.Data.Entity;


namespace Automator
{
    /// <summary>
    /// Interaction logic for Persons.xaml
    /// </summary>
    public partial class Persons : UserControl
    {
        private db_automator_2Entities dbcon;
       // private DB_AutomatorEntities14 dbcon14;
        List<person> lsGrid1 ;
        List<person> lsGrid2 ;
        public Persons ()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            //Do not load your data at design time.
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                this.personDataGrid.MaxWidth = System.Windows.SystemParameters.PrimaryScreenWidth - 50;
                this.personDataGrid2.MaxWidth = System.Windows.SystemParameters.PrimaryScreenWidth - 50;
            }
           
        }


        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {

        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {

        }

        private void personDataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            /*
            BindingExpression be = e.EditingElement.GetBindingExpression(TextBox.TextProperty);
            if(be != null)
            be.UpdateSource();*/
        }



        public string Person_id { get; set; }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            //Search
            search();
        }
        private void search()
        {
            personDataGrid.ItemsSource = null;
            personDataGrid2.ItemsSource = null; 
            this.Cursor = Cursors.Wait;
            int fromPers;
            DateTime? fromDate, toDate;
            String fromReg;
            int.TryParse(tbFromPers.Text, out fromPers);
            fromDate = tbFromDate.SelectedDate;
            toDate = tbToDate.SelectedDate;
            fromReg = tbFromReg.Text;
            //if( cbFromArchive.IsChecked == true )
            //{
            //    using (dbcon14 = new DB_AutomatorEntities14())
            //    {
            //        try
            //        {
            //            if (string.IsNullOrEmpty(tbFromPers.Text) &&
            //                string.IsNullOrEmpty(tbFromDate.Text) &&
            //                string.IsNullOrEmpty(tbToDate.Text) &&
            //                string.IsNullOrEmpty(tbFromReg.Text))
            //            {
            //                var selPers = from p in dbcon14.persons
            //                              select p;
            //                // personViewSource.Source = selPers.ToList();
            //                if (rbBirth.IsChecked == true || rbBoth.IsChecked == true)
            //                    personDataGrid.ItemsSource = lsGrid1 = selPers.Where<person>(p => p.Category == "Birth").ToList();
            //                if (rbDeath.IsChecked == true || rbBoth.IsChecked == true)
            //                    personDataGrid2.ItemsSource = lsGrid2 = selPers.Where<person>(p => p.Category == "Death").ToList();
            //            }
            //            else
            //            {
            //                var selPers = from p in dbcon14.persons
            //                              where (
            //                              (fromPers == 0 || p.Person_id == fromPers)
            //                                  //&& (toPers == 0 || p.Person_id <= toPers)
            //                              && (fromDate == null || p.Date_of_issue >= fromDate)
            //                              && (toDate == null || p.Date_of_issue <= toDate)
            //                              && (string.IsNullOrEmpty(fromReg) || p.Registration_number == fromReg))
            //                              select p;

            //                if (rbBirth.IsChecked == true || rbBoth.IsChecked == true)
            //                    personDataGrid.ItemsSource = lsGrid1 = selPers.Where<person>(p => p.Category == "Birth").ToList<person>();
            //                if (rbDeath.IsChecked == true || rbBoth.IsChecked == true)
            //                    personDataGrid2.ItemsSource = lsGrid2 = selPers.Where<person>(p => p.Category == "Death").ToList<person>();
            //            }

            //        }
            //        catch (Exception ex)
            //        {
            //            MessageBox.Show("Error in query buidling" + ex.Message);
            //        }
            //    }
            //}
            //else
            //{

                using (dbcon = new db_automator_2Entities())
                {
                    try
                    {
                        if (string.IsNullOrEmpty(tbFromPers.Text) &&
                            string.IsNullOrEmpty(tbFromDate.Text) &&
                            string.IsNullOrEmpty(tbToDate.Text) &&
                            string.IsNullOrEmpty(tbFromReg.Text))
                        {
                            var selPers = from p in dbcon.persons
                                          select p;
                            // personViewSource.Source = selPers.ToList();
                            if (rbBirth.IsChecked == true || rbBoth.IsChecked == true)
                                personDataGrid.ItemsSource = lsGrid1 = selPers.Where<person>(p => p.Category == "Birth").ToList();
                            if (rbDeath.IsChecked == true || rbBoth.IsChecked == true)
                                personDataGrid2.ItemsSource = lsGrid2 = selPers.Where<person>(p => p.Category == "Death").ToList();
                        }
                        else
                        {
                            var selPers = from p in dbcon.persons
                                          where (
                                          (fromPers == 0 || p.Person_id == fromPers)
                                              //&& (toPers == 0 || p.Person_id <= toPers)
                                          && (fromDate == null || p.Date_of_issue >= fromDate)
                                          && (toDate == null || p.Date_of_issue <= toDate)
                                          && (string.IsNullOrEmpty(fromReg) || p.Registration_number == fromReg))
                                          select p;

                            if (rbBirth.IsChecked == true || rbBoth.IsChecked == true)
                                personDataGrid.ItemsSource = lsGrid1 = selPers.Where<person>(p => p.Category == "Birth").ToList<person>();
                            if (rbDeath.IsChecked == true || rbBoth.IsChecked == true)
                                personDataGrid2.ItemsSource = lsGrid2 = selPers.Where<person>(p => p.Category == "Death").ToList<person>();
                        }

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error in query buidling" + ex.Message);
                    }
                }
            //}

            if (rbBirth.IsChecked == true || rbBoth.IsChecked == true)
            {
                summary_birth.Content = "Total records displayed " + personDataGrid.Items.Count.ToString();
                Expander2.IsExpanded = true;
            }
                if (rbDeath.IsChecked == true || rbBoth.IsChecked == true) {     
            summary_death.Content = "Total records displayed " + personDataGrid2.Items.Count.ToString();
                Expander3.IsExpanded = true;
                }
        
            this.Cursor = Cursors.Arrow;
        }
        private void personDataGrid_Loaded(object sender, RoutedEventArgs e)
        {
          //  tools2.Visibility = System.Windows.Visibility.Visible;
        }

        private void personDataGrid_SourceUpdated(object sender, DataTransferEventArgs e)
        {

        }

        private void personDataGrid_Initialized(object sender, EventArgs e)
        {

        }

        private void Click_Clear(object sender, RoutedEventArgs e)
        {
            clear_text(grid_filter);
            personDataGrid.ItemsSource = null;
            personDataGrid2.ItemsSource = null; 
        }

        private void clear_text(DependencyObject obj)
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                if (obj is TextBox)
                {
                    ((TextBox)obj).Text = null;
                }
                clear_text(VisualTreeHelper.GetChild(obj, i));
            }
        }

        private void Click_SearchAll(object sender, RoutedEventArgs e)
        {
            clear_text(grid_filter);
            search();
        }

        private void btnPrint1(object sender, RoutedEventArgs e)
        {

            if (personDataGrid.SelectedIndex == -1)
            {
                MessageBox.Show("Select a record first");
                return;
            }
            this.Cursor = Cursors.Wait;
            Processor proc = Processor.getInstance((person)personDataGrid.SelectedItem, new Authority());
            proc.print();
            this.Cursor = Cursors.Arrow;
        }

        private void btnPrint2(object sender, RoutedEventArgs e)
        {
            if (personDataGrid2.SelectedIndex == -1)
            {
                MessageBox.Show("Select a record first");
                return;
            }
            this.Cursor = Cursors.Wait;
            Processor proc = Processor.getInstance((person)personDataGrid2.SelectedItem, new Authority());
            proc.print();
            this.Cursor = Cursors.Arrow;
        }

        private void btnExport2(object sender, RoutedEventArgs e)
        {
            this.Cursor = Cursors.Wait;
            export(personDataGrid2);
            this.Cursor = Cursors.Arrow;
        }

        private void btnExport1(object sender, RoutedEventArgs e)
        {
            this.Cursor = Cursors.Wait;
            export(personDataGrid);
            this.Cursor = Cursors.Arrow;
        }
        void export(DataGrid dgDisplay)
        {
            if (dgDisplay.Items.Count == 0)
            {
                MessageBox.Show("Nothing to export. Load first");
                return;
            }
            System.Windows.Forms.SaveFileDialog sav = new System.Windows.Forms.SaveFileDialog();
            sav.Title = "Save data to... ";
            sav.Filter = "Excel File(*.xls)|*.xls";
            if (sav.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;
            
            dgDisplay.SelectionMode = DataGridSelectionMode.Extended;
            dgDisplay.SelectAllCells();
            dgDisplay.ClipboardCopyMode = DataGridClipboardCopyMode.IncludeHeader;
            ApplicationCommands.Copy.Execute(null, dgDisplay);
            String resultat = (string)Clipboard.GetData(DataFormats.CommaSeparatedValue);
            String result = (string)Clipboard.GetData(DataFormats.Text);
            dgDisplay.UnselectAllCells();
            System.IO.StreamWriter file = new System.IO.StreamWriter(sav.FileName);
            file.WriteLine(result.Replace(',', ' '));
            file.Close();
            MessageBox.Show("Success. Data exported");
            dgDisplay.SelectionMode = DataGridSelectionMode.Single;
        }

        private void personDataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            person p = new person();
            switch(e.PropertyName){
                case "Issuing_authority":
                //case "Created_by":
                    e.Column.Visibility = Visibility.Hidden;
//                    e.Cancel = true;
                    break;

            }
            CustomData cust = CustomData.getCustomData();
            string pname = e.PropertyName;
            if (pname == "Person_id")
                e.Column.Header = "Sl No.";

            if (pname == "Date_of_birth_death")
            {
                (e.Column as DataGridTextColumn).Binding.StringFormat = cust.format_date_of_birth;
            }
            else if(pname.ToLower().Contains("date")){
                (e.Column as DataGridTextColumn).Binding.StringFormat =  cust.format_report_dates;
            }
            if(((DataGrid)sender).Name == "personDataGrid")
            {
                if (pname.ToLower().Contains("husband"))
                    e.Cancel = true;
                e.Column.Header = e.Column.Header.ToString().Replace("death", "");
                e.Column.Header = e.Column.Header.ToString().Replace("deceased", "");
            }
            else
            {
                e.Column.Header = e.Column.Header.ToString().Replace("birth", "");
                e.Column.Header = e.Column.Header.ToString().Replace("parents", "");
            }
            e.Column.Header = e.Column.Header.ToString().Replace("_", " ").Replace("  "," ");

        }

        private void DeleteRecord(object sender, RoutedEventArgs e)
        {
            DataGrid dg;
            List<person> pDel;
            if (((Button)sender).Name == "btnDelete1")
            {
                dg = personDataGrid;
                pDel = lsGrid1;
            }
            else
            {
                dg = personDataGrid2;
                pDel = lsGrid2;
            }

            if (dg.SelectedIndex == -1)
            {
                MessageBox.Show("Select a record", "Select record", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }
            this.Cursor = Cursors.Wait;
            using (var db = new db_automator_2Entities())
            {
                int pid = pDel[dg.SelectedIndex].Person_id;
                var q = db.persons.SingleOrDefault(x => x.Person_id == pid);
                if (q != null) {
                    db.persons.Remove(q);
                    db.SaveChanges();
                    pDel.RemoveAt(dg.SelectedIndex);
                    dg.ItemsSource = pDel;
                    dg.Items.Refresh();
                }

            }
            this.Cursor = Cursors.Arrow;
        }

        private void Edit2(object sender, RoutedEventArgs e)
        {

            Edit(personDataGrid2, lsGrid2);
        }

        private void Edit1(object sender, RoutedEventArgs e)
        {
            Edit(personDataGrid, lsGrid1);
        }

        void Edit(DataGrid db, List<person> ls)
        {
            if (db.SelectedIndex == -1)
            {
                MessageBox.Show("Select a record to edit", "Select record", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }
            newPerson nper = new newPerson();
            if(nper.Edit(ls[db.SelectedIndex])){
            this.Content = nper;
            }

        }
    }
}
