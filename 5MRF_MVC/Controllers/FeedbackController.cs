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
    public class FeedbackController : Controller
    {
        DataHandle DataHandle = new DataHandle();
        MsgBox MsgBox = new MsgBox();

        // GET: Feedback
        public ActionResult Index()
        {
            return View();
        }


        // GET: Discussion/Meeting/5
        public ActionResult Followup(string id)
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
            
            return View();
        }

        public class VarFeedback
        {
            public string var_qaid { set; get; }
            public string var_remark { set; get; }
        }

        [System.Web.Http.HttpPost]
        public JsonResult UpdateFeedback([FromBody]VarFeedback VarFeedback)
        {
            string QA_ID = VarFeedback.var_qaid.ToString();
            string QA_FU_REMARK = DataHandle.Stringfy(VarFeedback.var_remark.ToString());

            int a = DataHandle.Sql_CUD("UPDATE PlanDiscussion SET QA_FE_DESC='" + QA_FU_REMARK + "',QA_FE_USER='" + Session["Var_UserID"].ToString() + "',QA_FE_DATE='" + DateTime.Now + "' WHERE QA_ID='" + QA_ID + "'");


            return Json("");

        }

        public ActionResult _RowUpdateFB(string id)
        {
            DataVariables DataVariables = new DataVariables
            {
                Var_Fb_QAID = id
            };
            ViewBag.Message = DataVariables;
            
            return View();
        }

        public class OVarFeedback
        {
            public string var_qaid { set; get; }
            public string var_remark { set; get; }
        }

        [System.Web.Http.HttpPost]
        public JsonResult OUpdateFeedback([FromBody]OVarFeedback OVarFeedback)
        {
            string QA_ID = OVarFeedback.var_qaid.ToString();
            string QA_FU_REMARK = DataHandle.Stringfy(OVarFeedback.var_remark.ToString());

            int a = DataHandle.Sql_CUD("UPDATE PlanOverall SET OQA_FE_DESC='" + QA_FU_REMARK + "',OQA_FE_USER='" + Session["Var_UserID"].ToString() + "',OQA_FE_DATE='" + DateTime.Now + "' WHERE OQA_ID='" + QA_ID + "'");
            
            return Json("");

        }

        public ActionResult _ORowUpdateFB(string id)
        {
            DataVariables DataVariables = new DataVariables
            {
                Var_Fb_OQAID = id
            };
            ViewBag.Message = DataVariables;
            
            return View();
        }


    }
}
