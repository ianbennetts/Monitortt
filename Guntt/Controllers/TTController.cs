using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Http;

using Guntt.Models;

namespace Guntt.Controllers
{
    public class TTController : Controller
    {
        // GET: TT
        public ActionResult Index(string project)
        {
           ViewBag.project = project;

            var data = new DataBusiness().getSqlData(project);
            ViewBag.Count = data.Count();
            return View(data);
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