using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Http;

using Guntt.Models;

namespace Guntt.Controllers
{
    public class NthBoundController : Controller
    {
        // GET: TT
        public ActionResult Index()
        {
            return View(new DataBusiness().SqlDataNth);
        }
        public string Post([FromBody]string value)
        {
            if (value == "GetGraph")
            {
                return ("Worked");
            }
            return ("Did Not work");
        }
    }
}