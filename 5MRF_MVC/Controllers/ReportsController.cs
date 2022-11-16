using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using _5MRF_MVC.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Web.Http;

namespace _5MRF_MVC.Controllers
{
    public class ReportsController : Controller
    {
        DataHandle DataHandle = new DataHandle();
        // GET: Reports
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult FollowUPExport(string id)
        {
            DataVariables DataVariables = new DataVariables
            {
                Var_Fu_PlanID = id,
                Var_Fu_UserID = Session["Var_UserID"].ToString(),
                Var_Fu_FactID = DataHandle.GetPlanDetails(id, "Fact_ID"),
                Var_Fu_Year = DataHandle.GetPlanDetails(id, "Year"),
                Var_Fu_Month = DataHandle.GetPlanDetails(id, "Month")
            };
            ViewBag.Message = DataVariables;
            ViewBag.Title = "Followup_"+ DataVariables.Var_Fu_Year+"_"+ DataVariables.Var_Fu_Month+"_"+ DataHandle.GetFactoryName(DataVariables.Var_Fu_FactID);
            return View();
        }

        public ActionResult FeedbackUPExport(string id)
        {
            DataVariables DataVariables = new DataVariables
            {
                Var_Fb_PlanID = id,
                Var_Fb_UserID = Session["Var_UserID"].ToString(),
                Var_Fb_FactID = DataHandle.GetPlanDetails(id, "Fact_ID"),
                Var_Fb_Year = DataHandle.GetPlanDetails(id, "Year"),
                Var_Fb_Month = DataHandle.GetPlanDetails(id, "Month")
            };
            ViewBag.Message = DataVariables;
            ViewBag.Title = "Feedback_" + DataVariables.Var_Fb_Year + "_" + DataVariables.Var_Fb_Month + "_" + DataHandle.GetFactoryName(DataVariables.Var_Fb_FactID);
            return View();
        }
    }
}
