using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using Microsoft.Office.Interop.Word;
using System.Data;
using System.Reflection;
using System.Drawing.Printing;
namespace Automator
{
    class Processor
    {

        private string templeFile;
        string dateFormat;
        static Processor activeObj;
        Microsoft.Office.Interop.Word.Document doc;
        Microsoft.Office.Interop.Word.Application wApp;
        person pers;
        Authority auth;
        private Processor(person pers, Authority auth, string templatedoc)
        {


            this.pers = pers;
            this.auth = auth;
            this.templeFile = templatedoc;
            dateFormat = Automator.Properties.Settings.Default.dateFormat;
        }
        public static Processor getInstance(person pers, Authority auth)
        {
            string templatedoc;
            if (pers.Category == "Birth")
            {
                templatedoc = Automator.Properties.Settings.Default.TemplateFile_birth;
            }
            else
            {
                //If the person has husband, print husband
                if (!(string.IsNullOrEmpty(pers.Name_of_Husband) || string.IsNullOrEmpty(pers.Name_of_Husband)))
                    pers.Name_of_father = pers.Name_of_Husband;
                templatedoc = Automator.Properties.Settings.Default.TemplateFile_death;
            }
            activeObj = new Processor(pers, auth, templatedoc);
            return activeObj;
        }
        public void printPreview()
        {
            if (!checkParams())
            {
                return;
            }

            startFormatting();
        }

        public void print()
        {
            if (!checkParams())
            {
                return;
            }
            startFormatting();
        }

        private bool checkParams()
        {
            /*if (isExpired())
            {
                MessageBox.Show("This version has expired. Process will be cancelled", "Expiration", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }*/
            if (templeFile == null)
            {
                MessageBox.Show("Maintain template file in Birth Template");
                return false;
            }

            return true;
        }
        bool isExpired()
        {
            Expiry ex;
            if (File.Exists("license"))
            {
                ex = (Expiry)Utility.deserialize("license");
                if (ex.isRegistered)
                    return false;
                if (ex.noOperation > 15)
                    MessageBox.Show((30 - ex.noOperation).ToString() + ": " + "Product is not activated yet");
                if (ex.noOperation == 30)
                {
                    MessageBox.Show("You need to activate this product to continue using it");
                    return true;
                }
            }
            else
            {
                ex = new Expiry();
                ex.noOperation = 0;
                ex.isRegistered = false;
            }
            ex.noOperation += 1;
            Utility.serialize("license", ex);
            return false;
        }

        public static string getOrdinal(DateTime dt, string dateFormat, out List<string> subdates)
        {
            if(!dateFormat.Contains("d MMM"))
            {
                subdates = new List<string>();
                return dt.ToString(dateFormat);
            }
            subdates = new List<string>();
            string fmmmyyyy = dateFormat.Replace("d", "");
            subdates.Add(dt.Day.ToString());
            string dateb = "th";
            switch (dt.Day)
            {
                case 1:
                case 21:
                case 31:
                    dateb = "st";
                    break;
                case 2:
                case 22:
                    dateb = "nd";
                    break;
                case 3:
                case 23:
                    dateb = "rd";
                    break;
            }
            subdates.Add(dateb);
            subdates.Add(dt.ToString(fmmmyyyy));
            return subdates[0] + subdates[1] + subdates[2];
        }


        private void startFormatting()
        {
            if (templeFile == "")
            {
                MessageBox.Show("Template file missing!");
                return;
            }
            Dictionary<String, String> Dic = new Dictionary<string, String>();
            List<string> dateFields = new List<string>();

                dateFields.Add("Date_of_birth_death");
                dateFields.Add("Date_of_registration");
                dateFields.Add("Date_of_issue");

            foreach (PropertyInfo pi in pers.GetType().GetProperties())
            {
                string val;
                object obj = pi.GetValue(pers, null);
                if (obj == null)
                {
                    Dic.Add(pi.Name, string.Empty);
                    continue;
                }
                if (obj.GetType() == typeof(DateTime) && !dateFields.Contains(pi.Name))
                {
                    DateTime dt = DateTime.Parse(obj.ToString());
                    if (!dateFields.Contains(pi.Name))
                    {
                        List<string> ls = new List<string>();
                        val = getOrdinal(dt, dateFormat, out ls);
                        Dic.Add(pi.Name, val);
                        continue;
                    }
                }
                Dic.Add(pi.Name, obj.ToString());
            }

            string fileName1 = Path.GetTempFileName();
            wApp = new Microsoft.Office.Interop.Word.Application();
            try
            {
                File.Copy(templeFile, fileName1, true);
                doc = wApp.Documents.Open(fileName1);
            }
            catch (SystemException ex)
            {
                MessageBox.Show(ex.Message);
                wApp.Quit();
            }
            finally
            {

            }

            if (doc == null)
                return;
            try
            {
                foreach (ContentControl cc in doc.ContentControls)
                {
                    if (Dic.ContainsKey(cc.Title))
                    {
                        if (string.IsNullOrEmpty(Dic[cc.Title]))
                            cc.Range.Text = " ";
                        else
                            cc.Range.Text = Dic[cc.Title].ToString();//Value of text
                    }

                    else
                        cc.Range.Text = "";
                }

                Microsoft.Office.Interop.Word.Shapes txtshapes = doc.Shapes;
                foreach (Microsoft.Office.Interop.Word.Shape txtb in txtshapes)
                {
                    foreach (ContentControl cc in txtb.TextFrame.TextRange.ContentControls)
                    {
                        if (cc.Title!= null && Dic.ContainsKey(cc.Title))
                        {
                            if (dateFields.Contains(cc.Title))
                            {
                                DateTime dt = DateTime.Parse(Dic[cc.Title]);
                                List<string> subdates = new List<string>();
                                if (!dateFormat.Contains("d MMM"))
                                {
                                    subdates.Add("");
                                    subdates.Add("");
                                    subdates.Add("");
                                    subdates.Add(dt.ToString(dateFormat));
                                }
                                else
                                {
                                    cc.Delete(true);
                                    getOrdinal(dt, dateFormat, out subdates);
                                }

                                int i = 0;

                                foreach (ContentControl subcc in txtb.TextFrame.TextRange.ContentControls)
                                {
                                    if (subcc.Title.Contains("super"))
                                    {
                                        subcc.Range.Font.Superscript = 1;
                                    }
                                    subcc.Range.Text = subdates[i];
                                    i++;
                                }
                            }
                            else
                                if (string.IsNullOrEmpty(Dic[cc.Title]))
                                    cc.Range.Text = " ";
                                else
                                    cc.Range.Text = Dic[cc.Title].ToString();//Value of text
                        }
                    }
                }
            }

            catch (SystemException ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {

            }
            doc.Save();
            //                doc.PrintPreview();

            PrintDialog pdig = new PrintDialog();
            if (!(pdig.ShowDialog() == DialogResult.OK))
                return;
            doc.Application.ActivePrinter = pdig.PrinterSettings.PrinterName;
            doc.PrintOut();
            doc.Close();
            wApp.Quit();
            //doc.PrintOut(Type.Missing, Type.Missing);
        }

        public const string dbFolder = @"C:\Automator";
        public const string dbFile_old = @"C:\Automator\DB_Automator.mdf";
        public const string dbFile = @"C:\Automator\db_automator_1.mdf";
        public static bool setup_initial()
        {
            if (!Directory.Exists(dbFolder))
            {
                try
                {
                    Directory.CreateDirectory(dbFolder);
                }
                catch
                {
                    MessageBox.Show("Failed to create director " + dbFolder + ". Please create it manually and re run this app");
                }
            }
            string templateDeath = Automator.Properties.Settings.Default.TemplateFile_death;
            string templateBirth = Automator.Properties.Settings.Default.TemplateFile_birth;
            if (File.Exists(dbFile))
                if (File.Exists(templateDeath))
                    if (File.Exists(templateBirth))
                        return true;
                    else
                    {
                        MessageBox.Show("Missing file: " + templateBirth);
                        return false;
                    }
                else
                {
                    MessageBox.Show("Missing file: " + templateDeath);
                    return false;
                }
            else
            {
                MessageBox.Show("Missing file: " + dbFile);
                return false;
            }

        }

        static bool check_file(string file1, string fileType)
        {
            string birthTempf; //Filename 
            string birthTemp; //Filename apth

            if (!File.Exists(file1))
            {
                birthTempf = Path.GetFileName(file1);
                birthTemp = Path.Combine("files", birthTempf);
                //File in installation folder?
                if (File.Exists(birthTemp))
                {
                    DialogResult dans = MessageBox.Show(fileType + " file is missing in C:\\Automator. "
                         + "Click YES to copy from insallation directory, No to select yourself?", "Fix missing file",
                         MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                    if (dans == DialogResult.Cancel)
                    {
                        return false;
                    }
                    if (dans == DialogResult.Yes)
                    {
                        File.Copy(birthTemp, file1, true);
                        return true;
                    }
                }
                OpenFileDialog op = new OpenFileDialog();
                op.FileName = birthTempf;
                if (op.ShowDialog() == DialogResult.Cancel)
                    return false;

                File.Copy(op.FileName, file1, true);

                return true;

            }
            return true;
        }

    }
    [Serializable()]
    class Expiry
    {
        public int noOperation { get; set; }
        public bool isRegistered { get; set; }
        public string activationCode { get; set; }

    }
}
