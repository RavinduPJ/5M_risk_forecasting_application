using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace _5MRF_MVC.Controllers
{
    public class AdminController : Controller
    {
        #region SYSTEM USERS

        public ActionResult SYSUSER()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SYSUSERCREATE(FormCollection collection)
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

        // POST: Admin/Create
        [HttpPost]
        public ActionResult SYSUSERUPDATE(FormCollection collection)
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

        public ActionResult _AddUserView()
        {
            return View();
        }

        #endregion

        #region DATA MASTER
        public ActionResult DEPARTMENTS()
        {
            return View();
        }

        public ActionResult FACTORIES()
        {
            return View();
        }

        public ActionResult QAPOOL()
        {
            return View();
        }

        public ActionResult CUSTOMERS()
        {
            return View();
        }

        public ActionResult PERMISSIONS()
        {
            return View();
        }

        #endregion

        // GET: Admin/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }
        
    }
}
