using Guntt.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Web.UI;
using ClosedXML.Excel;
using System.Configuration;
//using Microsoft.Office.Interop.Excel;

using System.Web.UI.WebControls;
using System.Data;
using System.Configuration;

namespace Guntt.Controllers
{
    public class BaseLineController : Controller
    {
        // GET: Reports
        public ActionResult Index()
        {

            return View();
        }

        // GET: Reports/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Reports/Create

        // POST: Reports/Create

        [HttpGet]
        [ActionName("Create")]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ActionName("Create")]
        public ActionResult Create_Post(string project)
        {
            BaselineReqModel req = new BaselineReqModel();
            XLWorkbook excel;
            TryUpdateModel(req);
            if (ModelState.IsValid)
            {
                 BaseLineMaker blm = new BaseLineMaker();
                excel =blm.createTTBase(req.date,req.noOfWeeks, project);

                DataTable dt = new DataTable();
                Response.Clear();
                Response.Buffer = true;
                Response.Charset = "";
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment;filename=" + "BaseLine " + project + ".xlsx");

                MemoryStream MyMemoryStream = new MemoryStream();
                excel.SaveAs(MyMemoryStream);
                MyMemoryStream.WriteTo(Response.OutputStream);
                Response.Flush();
                Response.End();
                return View();

            }
            return View();
        }

        [HttpGet]
        [ActionName("Volume")]
        public ActionResult Volume()
        {
            return View();
        }

        [HttpPost]
        [ActionName("Volume")]
        public ActionResult Volume_Post()
        {
            ReportReqModel req = new ReportReqModel();
            XLWorkbook excel;
            TryUpdateModel(req);
            if (ModelState.IsValid)
            {
                string[] s = new string[3];
                s[1] = " 1 Day";
                s[2] = " 7 Days";
                ReportMaker rm = new ReportMaker();
                if (req.type == 2)
                {
                    excel = rm.createWeekVol(req.date);
                }
                else
                {
                    excel = rm.createDayVol(req.date);
                }
                DataTable dt = new DataTable();
                Response.Clear();
                Response.Buffer = true;
                Response.Charset = "";
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                if (req.type == 1)
                {
                    Response.AddHeader("content-disposition", "attachment;filename=" + "BHUP Volume Daily Report " + req.date.ToString("yyyyMMdd ddd") + ".xlsx");
                }
                else
                {
                    Response.AddHeader("content-disposition", "attachment;filename=" + "BHUP Volume Weekly Report - Week Commencing " + req.date.ToString("yyyyMMdd") + ".xlsx");
                }
                MemoryStream MyMemoryStream = new MemoryStream();
                excel.SaveAs(MyMemoryStream);
                MyMemoryStream.WriteTo(Response.OutputStream);
                Response.Flush();
                Response.End();
                return View();

            }
            return View();
        }


        // GET: Reports/Edit/5

        [HttpGet]
        [ActionName("TTBaseLine")]
        public ActionResult TTBaseLine()
        {
            XLWorkbook excel;
            ReportMaker rm = new ReportMaker();
            excel = rm.createTTBase();
            Response.Clear();
            Response.Buffer = true;
            Response.Charset = "";
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment;filename=" + "BHUP Travel Time BaseLine Report " + ".xlsx");
            MemoryStream MyMemoryStream = new MemoryStream();
            excel.SaveAs(MyMemoryStream);
            MyMemoryStream.WriteTo(Response.OutputStream);
            Response.Flush();
            Response.End();
            return View();
        }
        [HttpGet]
        [ActionName("VolBaseLine")]
        public ActionResult VolBaseLine()
        {
            XLWorkbook excel;
            ReportMaker rm = new ReportMaker();
            excel = rm.createVolBase();
            Response.Clear();
            Response.Buffer = true;
            Response.Charset = "";
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment;filename=" + "BHUP Vol BaseLine Report " + ".xlsx");
            MemoryStream MyMemoryStream = new MemoryStream();
            excel.SaveAs(MyMemoryStream);
            MyMemoryStream.WriteTo(Response.OutputStream);
            Response.Flush();
            Response.End();
            return View();
        }

        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Reports/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Reports/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Reports/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
