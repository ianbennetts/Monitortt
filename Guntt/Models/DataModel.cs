using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Guntt.Models
{
    public class DataModel
    {
        public int ID { get; set; }
        public int SiteNo { get; set; }
        public string LinkName { get; set; }

        public int baseTT { get; set; }
        public int baseSpeed { get; set; }
        public int delay { get
            {
                return CurrentTT-baseTT;
            }
        }
        public int CurrentTT { get; set; }
        public int CurrentSpeed { get; set; }
        public int LinkLength { get; set; }
        public string Last90MinBase { get; set; }
        public string Last90MinTT { get; set; }
    }
}