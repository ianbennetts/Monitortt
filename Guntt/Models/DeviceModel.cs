using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Guntt.Models
{
    public class DeviceModel
    {
        public int ID { get; set; }
        public int SiteNo { get; set; }
        public int Modem { get; set; }
        public int Location { get; set; }
        public int BtDevice { get; set; }


        public int NoOfSightings { get; set; }
        
        [System.ComponentModel.DataAnnotations.DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm:ss}", ApplyFormatInEditMode = true)]
        public DateTime? When { get; set; }
       
    }
}