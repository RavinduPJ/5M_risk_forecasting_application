using System;
using System.Data;
using System.Data.SqlClient;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Web;

namespace _5MRF_MVC.Models
{
    public class DataHandle
    {
        public SqlConnection DB_CON = new SqlConnection(DataVariables.DBConString);
        DataVariables DataVariables = new DataVariables();
        private static Random random = new Random();

        public DataTable SqlSelectCMD(string strCMD)
        {
            DataTable ReturnDT = new DataTable();
            try
            {
                DB_CON.Open();
                SqlCommand SC = new SqlCommand(strCMD, DB_CON);
                SqlDataAdapter SDA = new SqlDataAdapter(SC);

                SDA.Fill(ReturnDT);

            }
            catch (SqlException EX)
            {
                HttpContext.Current.Session["MsgType"] = "Error";
                HttpContext.Current.Session["MsgDesc"] = EX.Message.ToString();
                ReturnDT = null;
            }
            finally
            {
                DB_CON.Close();
            }


            return ReturnDT;
        }

        public int Sql_CUD(string strCMD)
        {
            int CommonIntVal = 0;

            try
            {
                DB_CON.Open();
                SqlCommand SC = new SqlCommand(strCMD, DB_CON);
                SC.ExecuteNonQuery();

            }
            catch (SqlException EX)
            {
                HttpContext.Current.Session["MsgType"] = "Error";
                HttpContext.Current.Session["MsgDesc"] = EX.Message.ToString();
                CommonIntVal = 0;
            }
            finally
            {
                DB_CON.Close();
            }


            return CommonIntVal;
        }

        public string SqlNextId(string Table, string Column, int PadL)
        {
            string nextid = "1".PadLeft(PadL, '0');


            try
            {
                DB_CON.Open();
                DataTable DT = new DataTable();
                SqlCommand CMD = new SqlCommand("Select IsNull(max(" + Column + "), 0) + 1 as NextID FROM " + Table, DB_CON);
                SqlDataAdapter SDA = new SqlDataAdapter(CMD);
                SDA.Fill(DT);

                nextid = DT.Rows[0][0].ToString().PadLeft(PadL, '0');
            }
            catch (Exception)
            {
                nextid = "1".PadLeft(PadL, '0');
            }
            finally
            {
                DB_CON.Close();
            }

            return nextid;
        }

        public string Stringfy(string TextIn)
        {
            if (string.IsNullOrEmpty(TextIn) == false)
            {
                return TextIn.Replace("'", "''");
            }
            else
            {
                return TextIn;
            }
        }
        
        //// SPECIAL DATA FUNCTIONS ////

        public DataTable GetUncompletedPlans()
        {
            DataTable ReturnDT = new DataTable();

            try
            {
                DB_CON.Open();
                string qry = "";

                if (bool.Parse(HttpContext.Current.Session["Var_PerAdmin"].ToString()) == true)
                {
                    qry = "SELECT DISTINCT Plan_ID,Fact_ID,Fact_Name,Year,Month FROM VIEW_TMPTABLEUPLOAD";
                }
                else
                {
                    qry = "SELECT DISTINCT Plan_ID,Fact_ID,Fact_Name,Year,Month FROM VIEW_TMPTABLEUPLOAD WHERE Fact_ID ='" + HttpContext.Current.Session["Var_UserFactID"].ToString() + "'";
                }

                SqlCommand SC = new SqlCommand(qry, DB_CON);
                SqlDataAdapter SDA = new SqlDataAdapter(SC);

                SDA.Fill(ReturnDT);

            }
            catch (SqlException EX)
            {
                HttpContext.Current.Session["MsgType"] = "Error";
                HttpContext.Current.Session["MsgDesc"] = EX.Message.ToString();
                ReturnDT = null;
            }
            finally
            {
                DB_CON.Close();
            }


            return ReturnDT;
        }

        public bool CheckComplete(string User_ID, string Fact_ID, string Year, string Month)
        {
            bool ReturnBool = true;
            DataTable ReturnDT = new DataTable();

            try
            {
                DB_CON.Open();
                string qry = "Select * From UploadTemporySheet Where User_ID='" + User_ID + "' AND Fact_ID='" + Fact_ID + "' AND Year='" + Year + "' AND Month='" + Month + "' AND IsComplete='False'";

                SqlCommand SC = new SqlCommand(qry, DB_CON);
                SqlDataAdapter SDA = new SqlDataAdapter(SC);

                SDA.Fill(ReturnDT);

                if (ReturnDT.Rows.Count > 0)
                {
                    ReturnBool = false;
                }

            }
            catch (SqlException EX)
            {
                HttpContext.Current.Session["MsgType"] = "Error";
                HttpContext.Current.Session["MsgDesc"] = EX.Message.ToString();
                ReturnBool = true;
            }
            finally
            {
                DB_CON.Close();
            }


            return ReturnBool;
        }

        public bool CheckCompleteTemp(string Fact_ID, string Year, string Month)
        {
            bool ReturnBool = true;
            DataTable ReturnDT = new DataTable();

            try
            {
                DB_CON.Open();
                string qry = "Select * From UploadTemporySheet Where Fact_ID='" + Fact_ID + "' AND Year='" + Year + "' AND Month='" + Month + "' AND IsComplete='False'";

                SqlCommand SC = new SqlCommand(qry, DB_CON);
                SqlDataAdapter SDA = new SqlDataAdapter(SC);

                SDA.Fill(ReturnDT);

                if (ReturnDT.Rows.Count > 0)
                {
                    ReturnBool = false;
                }

            }
            catch (SqlException EX)
            {
                HttpContext.Current.Session["MsgType"] = "Error";
                HttpContext.Current.Session["MsgDesc"] = EX.Message.ToString();
                ReturnBool = true;
            }
            finally
            {
                DB_CON.Close();
            }


            return ReturnBool;
        }

        public bool CheckPlanComplete(string Plan_ID)
        {
            bool ReturnBool = true;
            DataTable ReturnDT = new DataTable();

            try
            {
                DB_CON.Open();
                string qry = "Select * From StyleMaster Where Plan_ID='" + Plan_ID + "' AND IsOpen='True'";

                SqlCommand SC = new SqlCommand(qry, DB_CON);
                SqlDataAdapter SDA = new SqlDataAdapter(SC);

                SDA.Fill(ReturnDT);

                if (ReturnDT.Rows.Count > 0)
                {
                    ReturnBool = false;
                }

            }
            catch (SqlException EX)
            {
                HttpContext.Current.Session["MsgType"] = "Error";
                HttpContext.Current.Session["MsgDesc"] = EX.Message.ToString();
                ReturnBool = true;
            }
            finally
            {
                DB_CON.Close();
            }


            return ReturnBool;
        }

        public bool CheckStyleIn(string Style, string factory, string year, string month, string Planid)
        {
            bool ReturnBool = false;
            DataTable ReturnDT1 = new DataTable();
            DataTable ReturnDT2 = new DataTable();

            try
            {
                DB_CON.Open();

                string qry1 = "Select * From StyleMaster Where Plan_ID='" + Planid + "' AND Style='" + Style + "'";
                SqlCommand SC1 = new SqlCommand(qry1, DB_CON);
                SqlDataAdapter SDA1 = new SqlDataAdapter(SC1);
                SDA1.Fill(ReturnDT1);

                string qry2 = "Select * From UploadTemporySheet Where Fact_ID='" + factory + "' AND Year='" + year + "' AND Month='" + month + "' AND Style='" + Style + "'";
                SqlCommand SC2 = new SqlCommand(qry2, DB_CON);
                SqlDataAdapter SDA2 = new SqlDataAdapter(SC2);
                SDA2.Fill(ReturnDT2);

                if (ReturnDT1.Rows.Count > 0 || ReturnDT2.Rows.Count > 0)
                {
                    ReturnBool = true;
                }

            }
            catch (SqlException EX)
            {
                HttpContext.Current.Session["MsgType"] = "Error";
                HttpContext.Current.Session["MsgDesc"] = EX.Message.ToString();
                ReturnBool = false;
            }
            finally
            {
                DB_CON.Close();
            }


            return ReturnBool;
        }

        public string CheckPlanDiscuss(string Fact_ID, string Year, string Month)
        {
            string ReturnBool = "";
            DataTable ReturnDT = new DataTable();

            try
            {
                DB_CON.Open();
                string qry = "Select * From PlanMaster Where Fact_ID='" + Fact_ID + "' AND Year='" + Year + "' AND Month='" + Month + "'";

                SqlCommand SC = new SqlCommand(qry, DB_CON);
                SqlDataAdapter SDA = new SqlDataAdapter(SC);

                SDA.Fill(ReturnDT);

                if (ReturnDT.Rows.Count == 1)
                {
                    ReturnBool = ReturnDT.Rows[0]["Plan_ID"].ToString();
                }

            }
            catch (SqlException EX)
            {
                HttpContext.Current.Session["MsgType"] = "Error";
                HttpContext.Current.Session["MsgDesc"] = EX.Message.ToString();
                ReturnBool = "";
            }
            finally
            {
                DB_CON.Close();
            }


            return ReturnBool;
        }

        public string ExtractDate(string value, string type)
        {
            string ReturnVAL = "";

            if (type == "Y")
            {
                ReturnVAL = value.Substring(0, value.IndexOf("-"));
            }
            else if (type == "M")
            {
                ReturnVAL = value.Remove(0, 5);
            }
            else
            {
                ReturnVAL = "";
            }

            return ReturnVAL;
        }

        public string GetMonth(string NumOfMonth)
        {
            switch (NumOfMonth)
            {
                case "01":
                    return "January";
                case "02":
                    return "February";
                case "03":
                    return "March";
                case "04":
                    return "April";
                case "05":
                    return "May";
                case "06":
                    return "June";
                case "07":
                    return "July";
                case "08":
                    return "August";
                case "09":
                    return "September";
                case "10":
                    return "October";
                case "11":
                    return "November";
                case "12":
                    return "December";
                default:
                    return "";

            }
        }

        public string GetFactoryName(string factid)
        {
            string ReturnFN = "";

            try
            {
                DB_CON.Open();
                string qry = "SELECT * FROM Factory WHERE Fact_ID='" + factid + "'";
                DataTable DT = new DataTable();
                SqlCommand SC = new SqlCommand(qry, DB_CON);
                SqlDataAdapter SDA = new SqlDataAdapter(SC);

                SDA.Fill(DT);
                if (DT.Rows.Count > 0)
                    ReturnFN = DT.Rows[0]["Fact_Name"].ToString();

            }
            catch (SqlException EX)
            {
                HttpContext.Current.Session["MsgType"] = "Error";
                HttpContext.Current.Session["MsgDesc"] = EX.Message.ToString();
                ReturnFN = "";
            }
            finally
            {
                DB_CON.Close();
            }


            return ReturnFN;
        }

        public string GetPlanID(string factid,string year,string month)
        {
            string ReturnFN = "";

            try
            {
                DB_CON.Open();
                string qry = "SELECT Plan_ID FROM PlanMaster WHERE Fact_ID='" + factid + "' AND Year='"+ year + "' AND Month='"+ month + "'";
                DataTable DT = new DataTable();
                SqlCommand SC = new SqlCommand(qry, DB_CON);
                SqlDataAdapter SDA = new SqlDataAdapter(SC);

                SDA.Fill(DT);
                if (DT.Rows.Count > 0)
                    ReturnFN = DT.Rows[0]["Plan_ID"].ToString();

            }
            catch (SqlException EX)
            {
                HttpContext.Current.Session["MsgType"] = "Error";
                HttpContext.Current.Session["MsgDesc"] = EX.Message.ToString();
                ReturnFN = "";
            }
            finally
            {
                DB_CON.Close();
            }


            return ReturnFN;
        }

        public string GetCustomerID(string Cusname)
        {
            string ReturnFN = "000";

            try
            {
                DB_CON.Open();
                string qry = "SELECT Cust_CatID FROM CustomerCategory WHERE Cust_Name='" + Cusname + "'";
                DataTable DT = new DataTable();
                SqlCommand SC = new SqlCommand(qry, DB_CON);
                SqlDataAdapter SDA = new SqlDataAdapter(SC);

                SDA.Fill(DT);
                if (DT.Rows.Count > 0)
                    ReturnFN = DT.Rows[0][0].ToString();

            }
            catch (SqlException EX)
            {
                HttpContext.Current.Session["MsgType"] = "Error";
                HttpContext.Current.Session["MsgDesc"] = EX.Message.ToString();
                ReturnFN = "000";
            }
            finally
            {
                DB_CON.Close();
            }


            return ReturnFN;
        }

        public string GetCustomerName(string CusID)
        {
            string ReturnFN = "";

            try
            {
                DB_CON.Open();
                string qry = "SELECT Cust_Name FROM CustomerCategory WHERE Cust_CatID='" + CusID + "'";
                DataTable DT = new DataTable();
                SqlCommand SC = new SqlCommand(qry, DB_CON);
                SqlDataAdapter SDA = new SqlDataAdapter(SC);

                SDA.Fill(DT);
                if (DT.Rows.Count > 0)
                    ReturnFN = DT.Rows[0][0].ToString();

            }
            catch (SqlException EX)
            {
                HttpContext.Current.Session["MsgType"] = "Error";
                HttpContext.Current.Session["MsgDesc"] = EX.Message.ToString();
                ReturnFN = "";
            }
            finally
            {
                DB_CON.Close();
            }


            return ReturnFN;
        }

        public string GetDeptName(string DeptID)
        {
            string ReturnFN = "";

            try
            {
                DB_CON.Open();
                string qry = "SELECT Dept_Name FROM Departments WHERE Dept_ID='" + DeptID + "'";
                DataTable DT = new DataTable();
                SqlCommand SC = new SqlCommand(qry, DB_CON);
                SqlDataAdapter SDA = new SqlDataAdapter(SC);

                SDA.Fill(DT);
                if (DT.Rows.Count > 0)
                    ReturnFN = DT.Rows[0][0].ToString();

            }
            catch (SqlException EX)
            {
                HttpContext.Current.Session["MsgType"] = "Error";
                HttpContext.Current.Session["MsgDesc"] = EX.Message.ToString();
                ReturnFN = "";
            }
            finally
            {
                DB_CON.Close();
            }


            return ReturnFN;
        }

        public string GetStyleDetails(string styleid, string FieldName)
        {
            string ReturnFN = "";

            try
            {
                DB_CON.Open();
                string qry = "SELECT * FROM VIEW_PLANSTYLECUSTOMER WHERE Style_ID='" + styleid + "'";
                DataTable DT = new DataTable();
                SqlCommand SC = new SqlCommand(qry, DB_CON);
                SqlDataAdapter SDA = new SqlDataAdapter(SC);

                SDA.Fill(DT);
                if (DT.Rows.Count > 0)
                {
                    ReturnFN = DT.Rows[0][FieldName].ToString();
                }

            }
            catch (SqlException EX)
            {
                HttpContext.Current.Session["MsgType"] = "Error";
                HttpContext.Current.Session["MsgDesc"] = EX.Message.ToString();
                ReturnFN = "";
            }
            finally
            {
                DB_CON.Close();
            }


            return ReturnFN;
        }

        public string GetPlanDetails(string planid, string FieldName)
        {
            string ReturnFN = "";

            try
            {
                DB_CON.Open();
                string qry = "SELECT * FROM PlanMaster WHERE Plan_ID='" + planid + "'";
                DataTable DT = new DataTable();
                SqlCommand SC = new SqlCommand(qry, DB_CON);
                SqlDataAdapter SDA = new SqlDataAdapter(SC);

                SDA.Fill(DT);
                if (DT.Rows.Count > 0)
                {
                    ReturnFN = DT.Rows[0][FieldName].ToString();
                }

            }
            catch (SqlException EX)
            {
                HttpContext.Current.Session["MsgType"] = "Error";
                HttpContext.Current.Session["MsgDesc"] = EX.Message.ToString();
                ReturnFN = "";
            }
            finally
            {
                DB_CON.Close();
            }


            return ReturnFN;
        }

        public string GetQADetails(string qaid, string FieldName)
        {
            string ReturnFN = "";

            try
            {
                DB_CON.Open();
                string qry = "SELECT * FROM PlanDiscussion WHERE QA_ID='" + qaid + "'";
                DataTable DT = new DataTable();
                SqlCommand SC = new SqlCommand(qry, DB_CON);
                SqlDataAdapter SDA = new SqlDataAdapter(SC);

                SDA.Fill(DT);
                if (DT.Rows.Count > 0)
                {
                    ReturnFN = DT.Rows[0][FieldName].ToString();
                }

            }
            catch (SqlException EX)
            {
                HttpContext.Current.Session["MsgType"] = "Error";
                HttpContext.Current.Session["MsgDesc"] = EX.Message.ToString();
                ReturnFN = "";
            }
            finally
            {
                DB_CON.Close();
            }


            return ReturnFN;
        }

        public string GetOQADetails(string oqaid, string FieldName)
        {
            string ReturnFN = "";

            try
            {
                DB_CON.Open();
                string qry = "SELECT * FROM PlanOverall WHERE OQA_ID='" + oqaid + "'";
                DataTable DT = new DataTable();
                SqlCommand SC = new SqlCommand(qry, DB_CON);
                SqlDataAdapter SDA = new SqlDataAdapter(SC);

                SDA.Fill(DT);
                if (DT.Rows.Count > 0)
                {
                    ReturnFN = DT.Rows[0][FieldName].ToString();
                }

            }
            catch (SqlException EX)
            {
                HttpContext.Current.Session["MsgType"] = "Error";
                HttpContext.Current.Session["MsgDesc"] = EX.Message.ToString();
                ReturnFN = "";
            }
            finally
            {
                DB_CON.Close();
            }


            return ReturnFN;
        }

        public DataTable GetNext12Months()
        {
            DateTime setDateTime = DateTime.Now;
            string WithOutName, WithName;
            DataTable DT = new DataTable();
            DT.Columns.Add("won", typeof(string));
            DT.Columns.Add("wn", typeof(string));

            //WithOutName = setDateTime.AddMonths(-1).ToString("yyyy-MM");
            //WithName = setDateTime.AddMonths(-1).ToString("yyyy-MMMM");
            //DT.Rows.Add(WithOutName, WithName);

            WithOutName = setDateTime.ToString("yyyy-MM");
            WithName = setDateTime.ToString("yyyy-MMMM");
            DT.Rows.Add(WithOutName, WithName);

            for (int i = 0; i < 13; i++)
            {
                setDateTime = setDateTime.AddMonths(1);
                WithOutName = setDateTime.ToString("yyyy-MM");
                WithName = setDateTime.ToString("yyyy-MMMM");
                DT.Rows.Add(WithOutName, WithName);
            }

            return DT;
        }

        public DataTable GetAssignFactoryList()
        {
            DataTable DT = new DataTable();

            try
            {
                DB_CON.Open();
                string qry = "";

                if (bool.Parse(HttpContext.Current.Session["Var_PerAdmin"].ToString()) == true)
                {
                    qry = "SELECT Fact_ID,Fact_Name FROM Factory";
                }
                else
                {
                    qry = "SELECT Fact_ID,Fact_Name FROM Factory WHERE Fact_ID='" + HttpContext.Current.Session["Var_UserFactID"].ToString() + "'";
                }

                SqlCommand SC = new SqlCommand(qry, DB_CON);
                SqlDataAdapter SDA = new SqlDataAdapter(SC);

                SDA.Fill(DT);

            }
            catch (SqlException EX)
            {
                HttpContext.Current.Session["MsgType"] = "Error";
                HttpContext.Current.Session["MsgDesc"] = EX.Message.ToString();
                DT = null;
            }
            finally
            {
                DB_CON.Close();
            }


            return DT;
        }

        public DataTable GetAttendNotIn(string Factid, string Planid)
        {
            DataTable DT = new DataTable();

            try
            {
                DB_CON.Open();
                string qry = "";

                qry = "SELECT User_ID,User_Name,User_PName FROM SysUser WHERE User_Active='True' AND Fact_ID='" + Factid + "' AND User_ID NOT IN (SELECT User_ID FROM  PlanAttendance WHERE Plan_ID ='" + Planid + "')";

                SqlCommand SC = new SqlCommand(qry, DB_CON);
                SqlDataAdapter SDA = new SqlDataAdapter(SC);

                SDA.Fill(DT);

            }
            catch (SqlException EX)
            {
                HttpContext.Current.Session["MsgType"] = "Error";
                HttpContext.Current.Session["MsgDesc"] = EX.Message.ToString();
                DT = null;
            }
            finally
            {
                DB_CON.Close();
            }


            return DT;
        }

        public DataTable GetQuesINStyleDept(string StyleID, string DepID)
        {
            DataTable DT = new DataTable();

            try
            {
                DB_CON.Open();
                string qry = "";

                qry = "SELECT * FROM VIEW_MEETINGALLRISK WHERE Dept_ID='" + DepID + "' AND Style_ID='" + StyleID + "' ORDER BY QA_ID DESC";

                SqlCommand SC = new SqlCommand(qry, DB_CON);
                SqlDataAdapter SDA = new SqlDataAdapter(SC);

                SDA.Fill(DT);

            }
            catch (SqlException EX)
            {
                HttpContext.Current.Session["MsgType"] = "Error";
                HttpContext.Current.Session["MsgDesc"] = EX.Message.ToString();
                DT = null;
            }
            finally
            {
                DB_CON.Close();
            }


            return DT;
        }

        public DataTable GetQuesINStyleAll(string StyleID)
        {
            DataTable DT = new DataTable();

            try
            {
                DB_CON.Open();
                string qry = "";

                if (bool.Parse(HttpContext.Current.Session["Var_PerAllDepartment"].ToString()) == true)
                {
                    qry = "SELECT * FROM VIEW_MEETINGQUEALL WHERE Style_ID='" + StyleID + "' ORDER BY QA_ID DESC";
                }
                else
                {
                    qry = "SELECT * FROM VIEW_MEETINGQUEALL WHERE Style_ID='" + StyleID + "' AND Dept_ID='" + HttpContext.Current.Session["Var_UserDepID"].ToString() + "' ORDER BY QA_ID DESC";
                }
                
                SqlCommand SC = new SqlCommand(qry, DB_CON);
                SqlDataAdapter SDA = new SqlDataAdapter(SC);

                SDA.Fill(DT);

            }
            catch (SqlException EX)
            {
                HttpContext.Current.Session["MsgType"] = "Error";
                HttpContext.Current.Session["MsgDesc"] = EX.Message.ToString();
                DT = null;
            }
            finally
            {
                DB_CON.Close();
            }


            return DT;
        }

        public DataTable GetQuesINPlanAll(string PlanID)
        {
            DataTable DT = new DataTable();

            try
            {
                DB_CON.Open();
                string qry = "";

                qry = "SELECT * FROM VIEW_OVERALLQUEALL WHERE Plan_ID='" + PlanID + "' ORDER BY OQA_ID DESC";

                SqlCommand SC = new SqlCommand(qry, DB_CON);
                SqlDataAdapter SDA = new SqlDataAdapter(SC);

                SDA.Fill(DT);

            }
            catch (SqlException EX)
            {
                HttpContext.Current.Session["MsgType"] = "Error";
                HttpContext.Current.Session["MsgDesc"] = EX.Message.ToString();
                DT = null;
            }
            finally
            {
                DB_CON.Close();
            }


            return DT;
        }

        public DataTable GetQuesFollowUp(string StyleID, string UserID)
        {
            DataTable DT = new DataTable();

            try
            {
                DB_CON.Open();
                string qry = "";

                qry = "SELECT * FROM VIEW_MEETINGALLRISK WHERE QA_User_ID='" + UserID + "' AND Style_ID='" + StyleID + "' ORDER BY QA_ID DESC";

                SqlCommand SC = new SqlCommand(qry, DB_CON);
                SqlDataAdapter SDA = new SqlDataAdapter(SC);

                SDA.Fill(DT);

            }
            catch (SqlException EX)
            {
                HttpContext.Current.Session["MsgType"] = "Error";
                HttpContext.Current.Session["MsgDesc"] = EX.Message.ToString();
                DT = null;
            }
            finally
            {
                DB_CON.Close();
            }


            return DT;
        }

        public DataTable GetFollowUpList(string PlanID)
        {
            DataTable DT = new DataTable();
            
            try
            {
                DB_CON.Open();
                string qry = "";

                if (bool.Parse(HttpContext.Current.Session["Var_PerAllDepartment"].ToString()) == true)
                {
                    qry = "SELECT * FROM VIEW_ALLFUQUICKVIEW WHERE Plan_ID='" + PlanID + "' AND QA_STATUS != 'Discuss' AND QA_OK = 'False' ORDER BY QA_ID DESC";
                }
                else
                {
                    qry = "SELECT * FROM VIEW_ALLFUQUICKVIEW WHERE Dept_ID='" + HttpContext.Current.Session["Var_UserDepID"].ToString() + "' AND Plan_ID='" + PlanID + "' AND QA_STATUS != 'Discuss' AND QA_OK = 'False' ORDER BY QA_ID DESC";
                }
                
                SqlCommand SC = new SqlCommand(qry, DB_CON);
                SqlDataAdapter SDA = new SqlDataAdapter(SC);

                SDA.Fill(DT);

            }
            catch (SqlException EX)
            {
                HttpContext.Current.Session["MsgType"] = "Error";
                HttpContext.Current.Session["MsgDesc"] = EX.Message.ToString();
                DT = null;
            }
            finally
            {
                DB_CON.Close();
            }


            return DT;
        }

        public DataTable GetOverallFollowUpList(string PlanID)
        {
            DataTable DT = new DataTable();
            
            try
            {
                DB_CON.Open();
                string qry = "";

                
                if (bool.Parse(HttpContext.Current.Session["Var_PerAllDepartment"].ToString()) == true)
                {
                    qry = "SELECT * FROM VIEW_ALLFUORQUICKVIEW WHERE Plan_ID='" + PlanID + "' AND OQA_STATUS != 'Discuss' AND OQA_OK = 'False' ORDER BY OQA_ID DESC";
                }
                else
                {
                    qry = "SELECT * FROM VIEW_ALLFUORQUICKVIEW WHERE OQA_User_ID='"+ HttpContext.Current.Session["Var_UserID"].ToString() +"' AND Plan_ID='" + PlanID + "' AND OQA_STATUS != 'Discuss' AND OQA_OK = 'False' ORDER BY OQA_ID DESC";
                }

                SqlCommand SC = new SqlCommand(qry, DB_CON);
                SqlDataAdapter SDA = new SqlDataAdapter(SC);

                SDA.Fill(DT);

            }
            catch (SqlException EX)
            {
                HttpContext.Current.Session["MsgType"] = "Error";
                HttpContext.Current.Session["MsgDesc"] = EX.Message.ToString();
                DT = null;
            }
            finally
            {
                DB_CON.Close();
            }


            return DT;
        }

        public DataTable GetFeedbackList(string PlanID,string Cus_ID)
        {
            DataTable DT = new DataTable();

            try
            {
                DB_CON.Open();
                string qry = "";

                if (bool.Parse(HttpContext.Current.Session["Var_PerAllDepartment"].ToString()) == true)
                {
                    qry = "SELECT * FROM VIEW_ALLFUQUICKVIEW WHERE Plan_ID='" + PlanID + "' AND QA_STATUS != 'Discuss' AND QA_IS_ESC='True' AND Cust_CatID='" + Cus_ID + "' ORDER BY QA_ID DESC";
                }
                else
                {
                    qry = "SELECT * FROM VIEW_ALLFUQUICKVIEW WHERE Plan_ID='" + PlanID + "' AND QA_STATUS != 'Discuss' AND QA_IS_ESC='True' AND Cust_CatID='"+ Cus_ID + "' AND Ques_ID IN (SELECT Ques_ID FROM QuestionAssEsc WHERE Dept_ID='" + HttpContext.Current.Session["Var_UserDepID"] + "') ORDER BY QA_ID DESC";
                }

                SqlCommand SC = new SqlCommand(qry, DB_CON);
                SqlDataAdapter SDA = new SqlDataAdapter(SC);

                SDA.Fill(DT);

            }
            catch (SqlException EX)
            {
                HttpContext.Current.Session["MsgType"] = "Error";
                HttpContext.Current.Session["MsgDesc"] = EX.Message.ToString();
                DT = null;
            }
            finally
            {
                DB_CON.Close();
            }


            return DT;
        }

        public DataTable GetOverallFeedbackList(string PlanID)
        {
            DataTable DT = new DataTable();

            try
            {
                DB_CON.Open();
                string qry = "";


                if (bool.Parse(HttpContext.Current.Session["Var_PerAllDepartment"].ToString()) == true)
                {
                    qry = "SELECT * FROM VIEW_ALLFUORQUICKVIEW WHERE Plan_ID='" + PlanID + "' AND OQA_STATUS != 'Discuss' AND OQA_IS_ESC='True' ORDER BY OQA_ID DESC";
                }
                else
                {
                    qry = "SELECT * FROM VIEW_ALLFUORQUICKVIEW WHERE Plan_ID='" + PlanID + "' AND OQA_STATUS != 'Discuss' AND OQA_IS_ESC='True' AND OQues_ID IN (SELECT OQues_ID FROM OverallQuestionAssEsc WHERE Dept_ID='" + HttpContext.Current.Session["Var_UserDepID"].ToString() + "') ORDER BY OQA_ID DESC";
                }

                SqlCommand SC = new SqlCommand(qry, DB_CON);
                SqlDataAdapter SDA = new SqlDataAdapter(SC);

                SDA.Fill(DT);

            }
            catch (SqlException EX)
            {
                HttpContext.Current.Session["MsgType"] = "Error";
                HttpContext.Current.Session["MsgDesc"] = EX.Message.ToString();
                DT = null;
            }
            finally
            {
                DB_CON.Close();
            }


            return DT;
        }

        public DataTable GetPlanList()
        {
            DataTable DT = new DataTable();

            try
            {
                DB_CON.Open();
                string qry = "";

                if (bool.Parse(HttpContext.Current.Session["Var_PerAdmin"].ToString()) == true)
                {
                    qry = "SELECT * FROM PlanMaster ORDER BY Plan_ID DESC";
                }
                else
                {
                    qry = "SELECT * FROM PlanMaster WHERE Fact_ID='" + HttpContext.Current.Session["Var_UserFactID"].ToString() + "' ORDER BY Plan_ID DESC";
                }


                SqlCommand SC = new SqlCommand(qry, DB_CON);
                SqlDataAdapter SDA = new SqlDataAdapter(SC);

                SDA.Fill(DT);

            }
            catch (SqlException EX)
            {
                HttpContext.Current.Session["MsgType"] = "Error";
                HttpContext.Current.Session["MsgDesc"] = EX.Message.ToString();
                DT = null;
            }
            finally
            {
                DB_CON.Close();
            }


            return DT;
        }

        public DataTable GetDepartmentForDiscuss()
        {
            DataTable DT = new DataTable();

            try
            {
                DB_CON.Open();
                string qry = "";

                if (bool.Parse(HttpContext.Current.Session["Var_PerAllDepartment"].ToString()) == true)
                {
                    qry = "SELECT Dept_ID,Dept_Name FROM Departments WHERE Dept_Active= 'True' AND Dept_ID IN (Select distinct Dept_ID FROM QuestionPool WHERE Ques_Active='True') Order BY Dept_Desc";
                }
                else
                {
                    qry = "SELECT Dept_ID,Dept_Name FROM Departments WHERE Dept_Active= 'True' AND Dept_ID='"+ HttpContext.Current.Session["Var_UserDepID"] + "'";
                }


                SqlCommand SC = new SqlCommand(qry, DB_CON);
                SqlDataAdapter SDA = new SqlDataAdapter(SC);

                SDA.Fill(DT);

            }
            catch (SqlException EX)
            {
                HttpContext.Current.Session["MsgType"] = "Error";
                HttpContext.Current.Session["MsgDesc"] = EX.Message.ToString();
                DT = null;
            }
            finally
            {
                DB_CON.Close();
            }


            return DT;
        }

        public void CopyPlanToPlan(string Fact_ID, string Year, string Month)
        {

            try
            {
                string MaxID = SqlNextId("PlanMaster", "Plan_ID", 6);

                int i_1 = Sql_CUD("INSERT INTO PlanMaster(Plan_ID,Fact_ID,Year,Month,PushDate) VALUES('" + MaxID + "','" + Fact_ID + "','" + Year + "','" + Month + "','" + DateTime.Now + "')");

                int i_2 = Sql_CUD("INSERT INTO StyleMaster(Plan_ID,Style,Oqty,Line,Psd,Nor,Fdd) SELECT '" + MaxID + "' AS Plan_ID,Style,Oqty,Line,Psd,Nor,Fdd FROM UploadTemporySheet WHERE Fact_ID='" + Fact_ID + "' AND Year='" + Year + "' AND Month='" + Month + "'");

                HttpContext.Current.Session["MsgType"] = "Success";
                HttpContext.Current.Session["MsgDesc"] = "All Styles Completed and Ready For Discussion.";

            }
            catch (SqlException EX)
            {
                HttpContext.Current.Session["MsgType"] = "Error";
                HttpContext.Current.Session["MsgDesc"] = EX.Message.ToString();
            }
            finally
            {

            }

        }

        public string GetBoolText(string Val1, string DataType)
        {
            string ReturnFN = "";

            if (bool.Parse(Val1) == true)
            {
                if (DataType == "YN")
                    ReturnFN = "YES";

                if (DataType == "OK")
                    ReturnFN = "OK";

                if (DataType == "TF")
                    ReturnFN = "True";

                if (DataType == "OZ")
                    ReturnFN = "1";

                if (DataType == "MO")
                    ReturnFN = "Mandatory";

                if (DataType == "RISK")
                    ReturnFN = "NO";
            }
            else if (bool.Parse(Val1) == false)
            {
                if (DataType == "YN")
                    ReturnFN = "NO";

                if (DataType == "OK")
                    ReturnFN = "NOT OK";

                if (DataType == "TF")
                    ReturnFN = "False";

                if (DataType == "OZ")
                    ReturnFN = "0";

                if (DataType == "MO")
                    ReturnFN = "Optional";

                if (DataType == "RISK")
                    ReturnFN = "YES";
            }

            return ReturnFN;

        }

        public int CheckMeetinQAFinished(string Style_ID)
        {
            int ReturnBool = 0;
            DataTable ReturnDT = new DataTable();

            try
            {
                DB_CON.Open();
                string qry = "Select * FROM QuestionPool WHERE Ques_Must ='True' AND Ques_Active='True' AND Ques_ID NOT IN (SELECT Ques_ID FROM PlanDiscussion WHERE Style_ID='" + Style_ID + "')";

                SqlCommand SC = new SqlCommand(qry, DB_CON);
                SqlDataAdapter SDA = new SqlDataAdapter(SC);

                SDA.Fill(ReturnDT);

                ReturnBool = ReturnDT.Rows.Count;

            }
            catch (SqlException EX)
            {
                HttpContext.Current.Session["MsgType"] = "Error";
                HttpContext.Current.Session["MsgDesc"] = EX.Message.ToString();
                ReturnBool = 0;
            }
            finally
            {
                DB_CON.Close();
            }


            return ReturnBool;
        }

        public int UpdateMeetinQAFinished(string Style_ID)
        {
            int returnval = 0;
            DataTable ReturnDT = new DataTable();

            try
            {
                DB_CON.Open();
                string qry = "Select * FROM QuestionPool WHERE Ques_Must ='True' AND Ques_Active='True' AND Ques_ID NOT IN (SELECT Ques_ID FROM PlanDiscussion WHERE Style_ID='" + Style_ID + "')";

                SqlCommand SC = new SqlCommand(qry, DB_CON);
                SqlDataAdapter SDA = new SqlDataAdapter(SC);

                SDA.Fill(ReturnDT);

                foreach (DataRow dtRow in ReturnDT.Rows)
                { 
                    string cmd = "INSERT INTO PlanDiscussion (Style_ID,Ques_ID,Dept_ID,QA_User_ID,QA_OK,QA_RISK,QA_CM,QA_WHEN,QA_IS_ESC,QA_C_User,QA_C_Date,QA_STATUS,QA_IS_CLOSED) VALUES('" +
                    Style_ID + "','" +
                    dtRow["Ques_ID"].ToString() + "','" +
                    dtRow["Dept_ID"].ToString() + "','1','True','','','','False','" +
                    HttpContext.Current.Session["Var_UserID"].ToString() + "','" +
                    DateTime.Now + "','Completed','True')";

                    SqlCommand InsertCom = new SqlCommand(cmd, DB_CON);
                    InsertCom.ExecuteNonQuery();

                    returnval = returnval + 1;
                }

            }
            catch (SqlException EX)
            {
                HttpContext.Current.Session["MsgType"] = "Error";
                HttpContext.Current.Session["MsgDesc"] = EX.Message.ToString();
                returnval = 0;
            }
            finally
            {
                DB_CON.Close();
            }

            return returnval;


        }

        public int CheckMeetinOverallQAFinished(string Plan_ID)
        {
            int Returnval = 0;
            DataTable ReturnDT = new DataTable();

            try
            {
                DB_CON.Open();
                string qry = "Select * FROM OverallQuestionPool WHERE OQues_Must ='True' AND OQues_Active='True' AND OQues_ID NOT IN (SELECT OQues_ID FROM PlanOverall WHERE Plan_ID='" + Plan_ID + "')";

                SqlCommand SC = new SqlCommand(qry, DB_CON);
                SqlDataAdapter SDA = new SqlDataAdapter(SC);

                SDA.Fill(ReturnDT);

                Returnval = ReturnDT.Rows.Count;

            }
            catch (SqlException EX)
            {
                HttpContext.Current.Session["MsgType"] = "Error";
                HttpContext.Current.Session["MsgDesc"] = EX.Message.ToString();
                Returnval = 0;
            }
            finally
            {
                DB_CON.Close();
            }


            return Returnval;
        }

        public void UpdateAllUsingQAID(string QA_ID)
        {

        }

        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public bool DoesUserExist(string userName)
        {
            using (var domainContext = new PrincipalContext(ContextType.Domain, "BRANDIXLK"))
            {
                using (var foundUser = UserPrincipal.FindByIdentity(domainContext, IdentityType.SamAccountName, userName))
                {
                    return foundUser != null;
                }
            }
        }

    }
}