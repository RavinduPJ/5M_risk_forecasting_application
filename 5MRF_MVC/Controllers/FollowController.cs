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
    public class FollowController : Controller
    {
        DataHandle DataHandle = new DataHandle();
        MsgBox MsgBox = new MsgBox();

        private class ControlVariables
        {
        
        }

        // GET: Follow
        public ActionResult Index()
        {
            return View();
        }
        
        // GET: Discussion/Meeting/5
        public ActionResult Followup(string id)
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
            
            return View();
        }

        public class VarFollowup
        {
            public string var_qaid { set; get; }
            public string var_status { set; get; }
            public string var_remark { set; get; }
        }

        [System.Web.Http.HttpPost]
        public JsonResult UpdateFollowup([FromBody]VarFollowup VarFollowup)
        {
            string QA_ID = VarFollowup.var_qaid.ToString();
            string QA_FU_REMARK = DataHandle.Stringfy(VarFollowup.var_remark.ToString());
            string QA_FU_STATUS = DataHandle.Stringfy(VarFollowup.var_status.ToString());

            int a = DataHandle.Sql_CUD("UPDATE PlanDiscussion SET QA_FO_DESC='"+ QA_FU_REMARK + "',QA_STATUS='"+ QA_FU_STATUS + "',QA_FO_USER='" + Session["Var_UserID"] + "',QA_FO_DATE='"+DateTime.Now+"' WHERE QA_ID='"+ QA_ID + "'");

            if (QA_FU_STATUS== "Complete")
            {
                a = DataHandle.Sql_CUD("UPDATE PlanDiscussion SET QA_IS_CLOSED='True',QA_STATUS='Completed' WHERE QA_ID='" + QA_ID + "'");
            }
            
            return Json("");

        }

        public class VarEscalate
        {
            public string var_qaid { set; get; }
        }

        [System.Web.Http.HttpPost]
        public JsonResult EscalateFollowup([FromBody]VarEscalate VarEscalate)
        {
            string QA_ID = VarEscalate.var_qaid.ToString();

            int a = DataHandle.Sql_CUD("UPDATE PlanDiscussion SET QA_IS_ESC='True' WHERE QA_ID='" + QA_ID + "'");

            return Json("");

        }

        public ActionResult _RowUpdateFU(string id)
        {
            DataVariables DataVariables = new DataVariables
            {
                Var_Fu_QAID = id
            };
            ViewBag.Message = DataVariables;
            
            return View();
        }

        public class OVarFollowup
        {
            public string var_qaid { set; get; }
            public string var_status { set; get; }
            public string var_remark { set; get; }
        }

        [System.Web.Http.HttpPost]
        public JsonResult OUpdateFollowup([FromBody]OVarFollowup OVarFollowup)
        {
            string QA_ID = OVarFollowup.var_qaid.ToString();
            string QA_FU_REMARK = DataHandle.Stringfy(OVarFollowup.var_remark.ToString());
            string QA_FU_STATUS = DataHandle.Stringfy(OVarFollowup.var_status.ToString());

            int a = DataHandle.Sql_CUD("UPDATE PlanOverall SET OQA_FO_DESC='" + QA_FU_REMARK + "',OQA_STATUS='" + QA_FU_STATUS + "',OQA_FO_USER='" + Session["Var_UserID"].ToString() + "',OQA_FO_DATE='" + DateTime.Now + "' WHERE OQA_ID='" + QA_ID + "'");

            if (QA_FU_STATUS == "Complete")
            {
                a = DataHandle.Sql_CUD("UPDATE PlanOverall SET OQA_IS_CLOSED='True',OQA_STATUS='Completed' WHERE OQA_ID='" + QA_ID + "'");
            }
            
            return Json("");
        }

        public class OVarEscalate
        {
            public string var_qaid { set; get; }
        }

        [System.Web.Http.HttpPost]
        public JsonResult OEscalateFollowup([FromBody]OVarEscalate OVarEscalate)
        {
            string QA_ID = OVarEscalate.var_qaid.ToString();

            int a = DataHandle.Sql_CUD("UPDATE PlanOverall SET OQA_IS_ESC='True' WHERE OQA_ID='" + QA_ID + "'");

            return Json("");

        }

        public ActionResult _ORowUpdateFU(string id)
        {
            DataVariables DataVariables = new DataVariables
            {
                Var_Fu_OQAID = id
            };
            ViewBag.Message = DataVariables;
            
            return View();
        }

    }
}
