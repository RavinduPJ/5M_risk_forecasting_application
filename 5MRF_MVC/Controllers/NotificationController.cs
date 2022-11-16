using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using _5MRF_MVC.Models;


namespace _5MRF_MVC.Controllers
{
    public class NotificationController : Controller
    {
        SendMail SendMail = new SendMail();

        // GET: Notification
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult SendAlert(string id)
        {
            string plan_id = id.PadLeft(8, '0');

            string MsgSend = SendMail.MonthlyMeetingsummery(plan_id);

            ViewBag.Message = MsgSend;

            return View();
        }

    }
}