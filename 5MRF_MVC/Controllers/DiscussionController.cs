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
    public class DiscussionController : Controller
    {
        DataHandle DataHandle = new DataHandle();
        MsgBox MsgBox = new MsgBox();

        // GET: Discussion
        public ActionResult Index()
        {
            DataVariables DataVariables = new DataVariables
            {
                Var_Dis_FactID = "",
                Var_Dis_Year = "",
                Var_Dis_Month = ""
            };
            ViewBag.Message = DataVariables;

            return View();
        }

        // POST: Discussion/Create
        [System.Web.Mvc.HttpPost]
        public ActionResult Index(FormCollection FormData)
        {
            try
            {

                DataVariables DataVariables = new DataVariables
                {
                    Var_Dis_FactID = FormData["cmbFactory"].ToString(),
                    Var_Dis_Year = DataHandle.ExtractDate(FormData["cmbYmonth"].ToString(), "Y"),
                    Var_Dis_Month = DataHandle.ExtractDate(FormData["cmbYmonth"].ToString(), "M")
                };
                ViewBag.Message = DataVariables;

                return View();
            }
            catch
            {
                DataVariables DataVariables = new DataVariables
                {
                    Var_Dis_FactID = "",
                    Var_Dis_Year = "",
                    Var_Dis_Month = ""
                };
                ViewBag.Message = DataVariables;

                return View();
            }
        }

        #region Style Meeting

        public ActionResult _AttendLoad(string id)
        {
            DataVariables DataVariables = new DataVariables
            {
                Var_OM_PlanID = id,
                Var_OM_FactID = DataHandle.GetPlanDetails(id, "Fact_ID")
            };
            ViewBag.Message = DataVariables;
            return View();
        }

        public ActionResult _CmbUserLoad()
        {
            return View();
        }

        public ActionResult _CmbUsersInDept(string factid, string depid)
        {
            DataVariables DataVariables = new DataVariables
            {
                Var_Met_DeptID = depid,
                Var_Met_FactID = factid
            };

            ViewBag.Message = DataVariables;

            return View();
        }

        public ActionResult _CmbQuestions(string id)
        {
            DataVariables DataVariables = new DataVariables
            {
                Var_Met_DeptID = id
            };

            ViewBag.Message = DataVariables;
            return View();
        }

        public ActionResult _DIVQAList(string id)
        {
            DataVariables DataVariables = new DataVariables
            {
                Var_Met_StyleID = id
            };
            ViewBag.Message = DataVariables;
            return View();
        }

        // GET: Discussion/Meeting/5
        public ActionResult Meeting(string id)
        {
            DataVariables DataVariables = new DataVariables
            {
                Var_Met_StyleID = id,
                Var_Met_FactID = DataHandle.GetStyleDetails(id, "Fact_ID"),
                Var_Met_Year = DataHandle.GetStyleDetails(id, "Year"),
                Var_Met_Month = DataHandle.GetStyleDetails(id, "Month"),
                Var_Met_PlanID = DataHandle.GetStyleDetails(id, "Plan_ID"),
                Var_Met_DeptID = ""
            };
            ViewBag.Message = DataVariables;

            return View();
        }

        // GET: Discussion/Meetingclose/5
        public ActionResult Meetingclose(string id)
        {
            DataVariables DataVariables = new DataVariables
            {
                Var_Met_StyleID = id,
                Var_Met_FactID = DataHandle.GetStyleDetails(id, "Fact_ID"),
                Var_Met_Year = DataHandle.GetStyleDetails(id, "Year"),
                Var_Met_Month = DataHandle.GetStyleDetails(id, "Month"),
                Var_Met_PlanID = DataHandle.GetStyleDetails(id, "Plan_ID"),
                Var_Met_DeptID = ""
            };

                int i = DataHandle.Sql_CUD("UPDATE StyleMaster SET Status='Follow' WHERE Style_ID='" + id + "'");
                int b = DataHandle.Sql_CUD("UPDATE PlanDiscussion SET QA_STATUS='Follow',QA_IS_CLOSED='False' WHERE Style_ID='" + id + "' AND QA_OK='False'");
                int c = DataHandle.UpdateMeetinQAFinished(id);
                Session["MsgType"] = "Success";
                Session["MsgDesc"] = "Congratulations !, 5M Forcasting Discussion Completed For (" + DataHandle.GetStyleDetails(id, "Style") + " - " + DataHandle.GetStyleDetails(id, "Cust_Name") + ")";

                ViewBag.Message = DataVariables;

                return RedirectToAction("Index", "Discussion");
            
        }

        public JsonResult MeetingCheck(string id)
        {
            int ChkOK = DataHandle.CheckMeetinQAFinished(id);

            if (ChkOK == 0)
            {
                var r = new { Content = "" };
                return Json(r, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var r = new { Content = "Sorry !, You must have to answer all mandatory questions in your 5M Meeting Discussion - (" + ChkOK.ToString() + " Questions Not In Your Discussion)" };
                return Json(r, JsonRequestBehavior.AllowGet);
            }
            
        }

        public class AttendUser
        {
            public string PlanID { set; get; }
            public string UserID { set; get; }
        }

        [System.Web.Http.HttpPost]
        public JsonResult AddUser([FromBody]AttendUser AttendUser)
        {
            string PLANID = AttendUser.PlanID.ToString().PadLeft(8, '0');
            string USERID = AttendUser.UserID.ToString();

            int a = DataHandle.Sql_CUD("INSERT INTO PlanAttendance(Plan_ID,User_ID,Date,IsAttend) VALUES('" + PLANID + "','" + USERID + "','" + DateTime.Now + "','True')");


            return Json("");

        }

        [System.Web.Http.HttpPost]
        public JsonResult AddUserMarkAbsent([FromBody]AttendUser AttendUser)
        {
            string PLANID = AttendUser.PlanID.ToString().PadLeft(8, '0');
            string USERID = AttendUser.UserID.ToString();

            int a = DataHandle.Sql_CUD("INSERT INTO PlanAttendance (Plan_ID,User_ID,Date,IsAttend) VALUES('" + PLANID + "','" + USERID + "','" + DateTime.Now + "','False')");


            return Json("");

        }

        public class AddRiskVariables
        {
            public string Var_get_txtStyle { set; get; }
            public string Var_get_cmbDepatment { set; get; }
            public string Var_get_cmbUserFor { set; get; }
            public string Var_get_cmbQues { set; get; }
            public string Var_get_txtRisk { set; get; }
            public string Var_get_txtCountermeasure { set; get; }
            public string Var_get_dtpWhen { set; get; }
            public string Var_get_cmbEscalate { set; get; }
        }

        [System.Web.Http.HttpPost]
        public JsonResult AddRisk([FromBody]AddRiskVariables AddRiskVariables)
        {
            string Var_get_txtStyle = DataHandle.Stringfy(AddRiskVariables.Var_get_txtStyle.ToString());
            string Var_get_cmbDepatment = AddRiskVariables.Var_get_cmbDepatment.ToString();
            string Var_get_cmbUserFor = AddRiskVariables.Var_get_cmbUserFor.ToString();
            string Var_get_cmbQues = DataHandle.Stringfy(AddRiskVariables.Var_get_cmbQues.ToString());
            string Var_get_txtRisk = DataHandle.Stringfy(AddRiskVariables.Var_get_txtRisk.ToString());
            string Var_get_txtCountermeasure = DataHandle.Stringfy(AddRiskVariables.Var_get_txtCountermeasure.ToString());
            string Var_get_dtpWhen = AddRiskVariables.Var_get_dtpWhen.ToString();
            string Var_get_cmbEscalate = AddRiskVariables.Var_get_cmbEscalate.ToString();


            int a = DataHandle.Sql_CUD("INSERT INTO PlanDiscussion (Style_ID,Ques_ID,Dept_ID,QA_User_ID,QA_OK,QA_RISK,QA_CM,QA_WHEN,QA_IS_ESC,QA_C_User,QA_C_Date,QA_STATUS,QA_IS_CLOSED) VALUES('" +
                Var_get_txtStyle + "','" +
                Var_get_cmbQues + "','" +
                Var_get_cmbDepatment + "','" +
                Var_get_cmbUserFor + "','False','" +
                Var_get_txtRisk + "','" +
                Var_get_txtCountermeasure + "','" +
                Var_get_dtpWhen + "','" +
                Var_get_cmbEscalate + "','" +
                Session["Var_UserID"].ToString() + "','" +
                DateTime.Now + "','Discuss','False')");


            return Json("");

        }

        public class NoRiskVariables
        {
            public string Var_get_txtStyle { set; get; }
            public string Var_get_cmbDepatment { set; get; }
            public string Var_get_cmbUserFor { set; get; }
            public string Var_get_cmbQues { set; get; }
        }

        [System.Web.Http.HttpPost]
        public JsonResult NoRisk([FromBody]NoRiskVariables NoRiskVariables)
        {
            string Var_get_txtStyle = NoRiskVariables.Var_get_txtStyle.ToString();
            string Var_get_cmbDepatment = NoRiskVariables.Var_get_cmbDepatment.ToString();
            string Var_get_cmbUserFor = NoRiskVariables.Var_get_cmbUserFor.ToString();
            string Var_get_cmbQues = DataHandle.Stringfy(NoRiskVariables.Var_get_cmbQues.ToString());

            int a = DataHandle.Sql_CUD("INSERT INTO PlanDiscussion (Style_ID,Ques_ID,Dept_ID,QA_User_ID,QA_OK,QA_RISK,QA_CM,QA_WHEN,QA_IS_ESC,QA_C_User,QA_C_Date,QA_STATUS,QA_IS_CLOSED) VALUES('" +
                Var_get_txtStyle + "','" +
                Var_get_cmbQues + "','" +
                Var_get_cmbDepatment + "','" +
                Var_get_cmbUserFor + "','True','','','','False','" +
                Session["Var_UserID"].ToString() + "','" +
                DateTime.Now + "','Complete','True')");


            return Json("");

        }

        public ActionResult DeleteRisk(string id)
        {

            string Var_Met_StyleID = DataHandle.GetQADetails(id, "Style_ID");

            int i = DataHandle.Sql_CUD("DELETE FROM PlanDiscussion WHERE QA_ID ='" + id + "'");

            Session["MsgType"] = "Success";
            Session["MsgDesc"] = "Risk Delete From List.";

            return Redirect(Url.Action("Meeting", "Discussion", new { id = Var_Met_StyleID }));
        }

        public class RiskUpdateVariables
        {
            public string var_Uqaid { set; get; }
            public string var_Urisk { set; get; }
            public string var_UCM { set; get; }
            public string var_Uwhen { set; get; }
            public string var_Uescalate { set; get; }
        }

        [System.Web.Http.HttpPost]
        public JsonResult UpdateRisk([FromBody]RiskUpdateVariables RiskUpdateVariables)
        {
            string Var_get_Uqaid = RiskUpdateVariables.var_Uqaid.ToString();
            string Var_get_Urisk = DataHandle.Stringfy(RiskUpdateVariables.var_Urisk.ToString());
            string Var_get_UCM = DataHandle.Stringfy(RiskUpdateVariables.var_UCM.ToString());
            string Var_get_Uwhen = RiskUpdateVariables.var_Uwhen.ToString();
            string Var_get_Uescalete = RiskUpdateVariables.var_Uescalate.ToString();

            int a = DataHandle.Sql_CUD("UPDATE PlanDiscussion SET QA_RISK='" + Var_get_Urisk + "',QA_CM='" + Var_get_UCM + "',QA_WHEN='" + Var_get_Uwhen + "',QA_IS_ESC='" + Var_get_Uescalete + "' WHERE QA_ID='" + Var_get_Uqaid + "'");


            return Json("");

        }

        public ActionResult _RowUpdate(string id)
        {
            DataVariables DataVariables = new DataVariables
            {
                Var_Met_QAID = id
            };

            ViewBag.Message = DataVariables;

            return View();
        }

        #endregion

        #region Overall Plant

        // GET: Discussion/Meeting/5
        public ActionResult Overall(string id)
        {
            DataVariables DataVariables = new DataVariables
            {
                Var_OM_PlanID = id,
                Var_OM_FactID = DataHandle.GetPlanDetails(id, "Fact_ID"),
                Var_OM_Year = DataHandle.GetPlanDetails(id, "Year"),
                Var_OM_Month = DataHandle.GetPlanDetails(id, "Month")
            };

            ViewBag.Message = DataVariables;

            return View();
        }

        public ActionResult _OverallDIVQAList(string id)
        {
            DataVariables DataVariables = new DataVariables
            {
                Var_OM_PlanID = id,
            };

            ViewBag.Message = DataVariables;

            return View();
        }

        public class OverallAddRiskVariables
        {
            public string Var_get_txtPlanID { set; get; }
            public string Var_get_cmbUserFor { set; get; }
            public string Var_get_cmbQues { set; get; }
            public string Var_get_txtRisk { set; get; }
            public string Var_get_txtCountermeasure { set; get; }
            public string Var_get_dtpWhen { set; get; }
            public string Var_get_cmbEscalate { set; get; }
        }

        [System.Web.Http.HttpPost]
        public JsonResult OverallAddRisk([FromBody]OverallAddRiskVariables OverallAddRiskVariables)
        {
            string Var_get_txtPlanID = OverallAddRiskVariables.Var_get_txtPlanID.ToString();
            string Var_get_cmbUserFor = OverallAddRiskVariables.Var_get_cmbUserFor.ToString();
            string Var_get_cmbQues = DataHandle.Stringfy(OverallAddRiskVariables.Var_get_cmbQues.ToString());
            string Var_get_txtRisk = DataHandle.Stringfy(OverallAddRiskVariables.Var_get_txtRisk.ToString());
            string Var_get_txtCountermeasure = DataHandle.Stringfy(OverallAddRiskVariables.Var_get_txtCountermeasure.ToString());
            string Var_get_dtpWhen = OverallAddRiskVariables.Var_get_dtpWhen.ToString();
            string Var_get_cmbEscalate = OverallAddRiskVariables.Var_get_cmbEscalate.ToString();


            int a = DataHandle.Sql_CUD("INSERT INTO PlanOverall (Plan_ID,OQues_ID,OQA_User_ID,OQA_OK,OQA_RISK,OQA_CM,OQA_WHEN,OQA_IS_ESC,OQA_C_User,OQA_C_Date,OQA_STATUS,OQA_IS_CLOSED) VALUES('" +
                Var_get_txtPlanID.PadLeft(8, '0') + "','" +
                Var_get_cmbQues + "','" +
                Var_get_cmbUserFor + "','False','" +
                Var_get_txtRisk + "','" +
                Var_get_txtCountermeasure + "','" +
                Var_get_dtpWhen + "','" +
                Var_get_cmbEscalate + "','" +
                Session["Var_UserID"].ToString() + "','" +
                DateTime.Now + "','Discuss','False')");


            return Json("");

        }

        public class OverallNoRiskVariables
        {
            public string Var_get_txtPlanID { set; get; }
            public string Var_get_cmbUserFor { set; get; }
            public string Var_get_cmbQues { set; get; }
        }

        [System.Web.Http.HttpPost]
        public JsonResult OverallNoRisk([FromBody]OverallNoRiskVariables OverallNoRiskVariables)
        {
            string Var_get_txtPlanID = OverallNoRiskVariables.Var_get_txtPlanID.ToString();
            string Var_get_cmbUserFor = OverallNoRiskVariables.Var_get_cmbUserFor.ToString();
            string Var_get_cmbQues = OverallNoRiskVariables.Var_get_cmbQues.ToString();

            int a = DataHandle.Sql_CUD("INSERT INTO PlanOverall (Plan_ID,OQues_ID,OQA_User_ID,OQA_OK,OQA_RISK,OQA_CM,OQA_WHEN,OQA_IS_ESC,OQA_C_User,OQA_C_Date,OQA_STATUS,OQA_IS_CLOSED) VALUES('" +
                Var_get_txtPlanID.PadLeft(8, '0') + "','" +
                Var_get_cmbQues + "','" +
                Var_get_cmbUserFor + "','True','','','','False','" +
                Session["Var_UserID"].ToString() + "','" +
                DateTime.Now + "','Complete','True')");


            return Json("");

        }

        public ActionResult OverallDeleteRisk(string id)
        { 
            string Var_OM_PlanID = DataHandle.GetOQADetails(id, "Plan_ID");

            int i = DataHandle.Sql_CUD("DELETE FROM PlanOverall WHERE OQA_ID ='" + id + "'");

            Session["MsgType"] = "Success";
            Session["MsgDesc"] = "Risk Delete From List.";

            return Redirect(Url.Action("Overall", "Discussion", new { id = Var_OM_PlanID }));
        }

        // GET: Discussion/Meetingclose/5
        public ActionResult OverallMeetingclose(string id)
        {
            string Var_OM_PlanID = id.PadLeft(8, '0');
            string Var_OM_FactID = DataHandle.GetPlanDetails(Var_OM_PlanID, "Fact_ID");

            //Check All Questions Finished
            int ChkOK = DataHandle.CheckMeetinOverallQAFinished(Var_OM_PlanID);

            int chkAtt = DataHandle.GetAttendNotIn(Var_OM_FactID, Var_OM_PlanID).Rows.Count;


            if (ChkOK == 0 && chkAtt == 0)
            {
                DataTable DTNotIn = DataHandle.SqlSelectCMD("Select * From OverallQuestionPool WHERE OQues_ID NOT IN (Select OQues_ID From PlanOverall Where Plan_ID='"+ Var_OM_PlanID + "') AND OQues_Active='True' AND OQues_Desc != 'Other'");

                foreach (DataRow DTR in DTNotIn.Rows)
                {
                    int a = DataHandle.Sql_CUD("INSERT INTO PlanOverall (Plan_ID,OQues_ID,OQA_User_ID,OQA_OK,OQA_RISK,OQA_CM,OQA_WHEN,OQA_IS_ESC,OQA_C_User,OQA_C_Date,OQA_STATUS,OQA_IS_CLOSED) VALUES('" +
                Var_OM_PlanID + "','" +
                DTR["OQues_ID"].ToString() + "','1','True','','','','False','" +
                Session["Var_UserID"].ToString() + "','" +
                DateTime.Now + "','Complete','True')");
                }

                int b = DataHandle.Sql_CUD("UPDATE PlanOverall SET OQA_STATUS='Follow',OQA_IS_CLOSED='False' WHERE Plan_ID='" + Var_OM_PlanID + "' AND OQA_OK='False'");
                Session["MsgType"] = "Success";
                Session["MsgDesc"] = "Congratulations !, 5M Forcasting Overall Discussion Completed For (" + DataHandle.GetPlanDetails(Var_OM_PlanID, "Year") + " - " + DataHandle.GetPlanDetails(Var_OM_PlanID, "Month") + ")";
                return RedirectToAction("Index", "Discussion");
            }
            else if (ChkOK != 0)
            {
                Session["MsgType"] = "Error";
                Session["MsgDesc"] = "Sorry !, You must have to answer all mandatory questions in your 5M Meeting Discussion - (" + ChkOK.ToString() + " Questions Not In Your Discussion)";
                return RedirectToAction("Overall", new { id = Var_OM_PlanID });

            }
            else if (chkAtt != 0)
            {
                Session["MsgType"] = "Error";
                Session["MsgDesc"] = "Sorry !, You must have to Mark all user participation in your 5M Meeting Discussion - (" + chkAtt.ToString() + " Users Not Mark In Your Discussion)";
                return RedirectToAction("Overall", new { id = Var_OM_PlanID });

            }
            else
            {
                Session["MsgType"] = "Error";
                Session["MsgDesc"] = "Sorry !, Try Again";
                return RedirectToAction("Overall", new { id = Var_OM_PlanID });
            }
        }

        public class ORiskUpdateVariables
        {
            public string var_Uqaid { set; get; }
            public string var_Urisk { set; get; }
            public string var_UCM { set; get; }
            public string var_Uwhen { set; get; }
            public string var_Uescalate { set; get; }
        }

        [System.Web.Http.HttpPost]
        public JsonResult OUpdateRisk([FromBody]RiskUpdateVariables ORiskUpdateVariables)
        {
            string Var_get_Uqaid = ORiskUpdateVariables.var_Uqaid.ToString();
            string Var_get_Urisk = DataHandle.Stringfy(ORiskUpdateVariables.var_Urisk.ToString());
            string Var_get_UCM = DataHandle.Stringfy(ORiskUpdateVariables.var_UCM.ToString());
            string Var_get_Uwhen = ORiskUpdateVariables.var_Uwhen.ToString();
            string Var_get_Uescalete = ORiskUpdateVariables.var_Uescalate.ToString();

            int a = DataHandle.Sql_CUD("UPDATE PlanOverall SET OQA_RISK='" + Var_get_Urisk + "',OQA_CM='" + Var_get_UCM + "',OQA_WHEN='" + Var_get_Uwhen + "',OQA_IS_ESC='" + Var_get_Uescalete + "' WHERE OQA_ID='" + Var_get_Uqaid + "'");

            return Json("");

        }

        public ActionResult _ORowUpdate(string id)
        {
            DataVariables DataVariables = new DataVariables
            {
                Var_Met_QAID = id
            };

            ViewBag.Message = DataVariables;

            return View();
        }

        #endregion

    }
}
