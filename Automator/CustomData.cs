using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
namespace Automator
{
    [Serializable()]
    class CustomData
    {
        public string format_date_of_birth = "d";
        public string format_report_dates = "dd/MM/yyyy 'at' h:mm tt";
        public string format_doc_dates = "d MMMM, yyyy";

        public string format_dates_screen = "d";
        public string default_district = "Imphal East";
        public string default_tahsilBlock = "SDO, IE-I CD Block, Sawombung";
        public string default_localArea = "Imphal East, Sawombung";
        public static string fileName = "Custom_data";
        private static CustomData obj;
        private CustomData()
        {
            
            if(!File.Exists(fileName)){
                Utility.serialize(fileName, this);
            }
            else
            {

                obj = (CustomData)Utility.deserialize(fileName);
            }

        }
        public static CustomData getCustomData(){
            if (obj == null)
                return new CustomData();
            else
                return obj;
        }
        public void save()
        {
            Utility.serialize(fileName, getCustomData());
        }
    }
}
