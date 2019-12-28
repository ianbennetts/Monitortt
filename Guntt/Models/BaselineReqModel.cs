using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Guntt.Models
{
    public class BaselineReqModel
    {
        [DisplayName("Date Commencing")]
        public DateTime date { get; set; }
        /*       [DisplayName("Link No 1-8")]
               [Range(1, 8, ErrorMessage = "Please enter valid Link 1-8")]
               public int linkNo { get; set; }
       */
        [DisplayName("Number of Weeks")]
        [Range(1, 8, ErrorMessage = "Please enter between 1 or 8")]

        public int noOfWeeks { get; set; }
    }
}