using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Guntt.Models;

namespace Guntt.Controllers
{
    public class DeviceController : Controller
    {
        // GET: Device

        public ActionResult Index(string project)
        {
            ViewBag.Project = project;
            if (project != null)
            {
                var data= new DeviceBusiness().getDevices(project);
                return View(data);
            }
           return View("~/views/home/index.cshtml");
        }

        // GET: Device/Details/5
        public string Details(int id)
        {
            return "Test";
        }

        // GET: Device/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Device/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Device/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Device/Edit/5
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

        // GET: Device/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Device/Delete/5
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
