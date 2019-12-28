using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Guntt.Models
{
    public class ReportReqModel
    {
        [DisplayName("Date Commencing")]
        public DateTime date { get; set; }
 /*       [DisplayName("Link No 1-8")]
        [Range(1, 8, ErrorMessage = "Please enter valid Link 1-8")]
        public int linkNo { get; set; }
*/
        [DisplayName("1 Day = 1 7 Days = 2")]
        [Range(1, 2, ErrorMessage = "Please enter 1 or 2")]

        public int type { get; set; }
    }
}