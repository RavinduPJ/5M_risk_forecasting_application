using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Data.SqlClient;
using _5MRF_MVC.Models;
using OfficeOpenXml;

namespace _5MRF_MVC.Controllers
{
    public class UploadForcastController : Controller
    {
        string strPlanID;
        string strFactory;
        string strYear;
        string strMonth;
        string OriFileName;
        string SysFileName;
        string SaveFilePath;

        int Insert_i;
        string Qry = "";

        DataHandle DataHandle = new DataHandle();
        DataVariables DataVariables = new DataVariables();

        // GET: UploadForcast
        public ActionResult Index()
        {

            return View();
        }

        // GET: UploadForcast/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: UploadForcast/Create
        public ActionResult UploadNew()
        {
            return View();
        }
        public class ExcelFileUpload
        {
            public string cmbFactory { set; get; }

            public string cmbYmonth { set; get; }
        }

        // POST: UploadForcast/Upload New File
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult UploadNew_old(ExcelFileUpload ExcelFileUpload)
        {
            try
            {
                strFactory = ExcelFileUpload.cmbFactory;
                strYear = DataHandle.ExtractDate(ExcelFileUpload.cmbYmonth, "Y");
                strMonth = DataHandle.ExtractDate(ExcelFileUpload.cmbYmonth, "M");
                strPlanID = DataHandle.CheckPlanDiscuss(strFactory, strYear, strMonth);

                if (strPlanID != "")//Plan in the discuss
                {
                    #region Plan Is IN Plan Master
                    foreach (string file in Request.Files)
                    {
                        HttpPostedFileBase ExcelFile = Request.Files[file];

                        OriFileName = Path.GetFileName(ExcelFile.FileName);
                        SysFileName = DateTime.Now.ToString("yyyymmddhhmmss") + "_" + strFactory + "_" + strYear + "_" + strMonth + Path.GetExtension(OriFileName);
                        SaveFilePath = Server.MapPath("~/Upload/" + SysFileName);
                        ExcelFile.SaveAs(SaveFilePath);
                        Insert_i = DataHandle.Sql_CUD("Insert Into FileHistory (User_ID,Fact_ID,Year,Month,File_Namereal,File_Nameset,File_date) values( '" + Session["Var_UserID"].ToString() + "','" + strFactory + "', '" + strYear + "', '" + strMonth + "', '" + DataHandle.Stringfy(OriFileName) + "', '" + SysFileName + "', '" + DateTime.Now.ToString() + "')");

                        DataTable DT_EXCELSHEET = new DataTable();
                        OleDbConnection OLEDBCON = new OleDbConnection(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source='" + SaveFilePath + "';Extended Properties= 'Excel 8.0;HDR=Yes;IMEX=1'");
                        OleDbDataAdapter OLEDA = new OleDbDataAdapter("Select * from [Sheet1$] WHERE Style IS NOT NULL", OLEDBCON);
                        OLEDA.Fill(DT_EXCELSHEET);

                        int UploadCount = 0;

                        foreach (DataRow dtRow in DT_EXCELSHEET.Rows)
                        {
                            string Customer, Style, Line, Nor, Psd, Fdd = "";
                            int Oqty = 0;
                            bool IsComplete = false;

                            Customer = DataHandle.Stringfy(DataHandle.GetCustomerID(dtRow[0].ToString()));
                            Style = DataHandle.Stringfy(dtRow[1].ToString());
                            Oqty = int.Parse(dtRow[2].ToString());
                            Line = DataHandle.Stringfy(dtRow[3].ToString());
                            Psd = DataHandle.Stringfy(dtRow[4].ToString());
                            Nor = DataHandle.Stringfy(dtRow[5].ToString());
                            Fdd = DataHandle.Stringfy(dtRow[6].ToString());

                            if (DataHandle.CheckStyleIn(Style,strFactory,strYear,strMonth, strPlanID) == false)
                            {

                                if (Fdd.Length < 1 || Customer == "000")
                                {
                                    IsComplete = false;
                                    Qry = "INSERT INTO UploadTemporySheet (User_ID,Fact_ID,Year,Month,Cust_ID,Style,Oqty,Line,Psd,Nor,Fdd,IsComplete) " +
                                    "VALUES('" + Session["Var_UserID"] +
                                    "','" + strFactory +
                                    "','" + strYear +
                                    "','" + strMonth +
                                    "','" + Customer +
                                    "','" + Style +
                                    "','" + Oqty +
                                    "','" + Line +
                                    "','" + DateTime.Parse(Psd).ToShortDateString() +
                                    "','" + Nor +
                                    "',NULL,'" + IsComplete + "')";
                                }
                                else
                                {
                                    IsComplete = true;
                                    Qry = "INSERT INTO StyleMaster (Plan_ID,Cust_CatID,Style,Oqty,Line,Psd,Nor,Fdd,Status,IsOpen) " +
                                    "VALUES('" + strPlanID +
                                     "','" + Customer +
                                    "','" + Style +
                                    "','" + Oqty +
                                    "','" + Line +
                                    "','" + DateTime.Parse(Psd).ToShortDateString() +
                                    "','" + Nor +
                                    "','" + DateTime.Parse(Fdd).ToShortDateString() +
                                    "','Pending','" + IsComplete + "')";
                                }


                                UploadCount = UploadCount + 1;
                                Insert_i = DataHandle.Sql_CUD(Qry);
                            }

                        }

                        Session["MsgType"] = "Success";
                        Session["MsgDesc"] = UploadCount.ToString() + " Styles Uploaded Success.";
                    }
                    #endregion
                }
                else
                {
                    #region Plan Is Not In Plan Master

                    foreach (string file in Request.Files)
                    {
                        HttpPostedFileBase ExcelFile = Request.Files[file];

                        OriFileName = Path.GetFileName(ExcelFile.FileName);
                        SysFileName = DateTime.Now.ToString("yyyymmddhhmmss") + "_" + strFactory + "_" + strYear + "_" + strMonth + Path.GetExtension(OriFileName);
                        SaveFilePath = Server.MapPath("~/Upload/" + SysFileName);
                        ExcelFile.SaveAs(SaveFilePath);
                        Insert_i = DataHandle.Sql_CUD("Insert Into FileHistory (User_ID,Fact_ID,Year,Month,File_Namereal,File_Nameset,File_date) values( '" + Session["Var_UserID"].ToString() + "','" + strFactory + "', '" + strYear + "', '" + strMonth + "', '" + DataHandle.Stringfy(OriFileName) + "', '" + SysFileName + "', '" + DateTime.Now.ToString() + "')");

                        string Nextplanid = DataHandle.SqlNextId("PlanMaster", "Plan_ID", 8);

                        Insert_i = DataHandle.Sql_CUD("Insert Into PlanMaster (Plan_ID,Fact_ID,Year,Month,PushDate) values( '" + Nextplanid + "','" + strFactory + "', '" + strYear + "', '" + strMonth + "', '" + DateTime.Now.ToString() + "')");

                        strPlanID = Nextplanid;

                        DataTable DT_EXCELSHEET = new DataTable();
                        OleDbConnection OLEDBCON = new OleDbConnection(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source='" + SaveFilePath + "';Extended Properties= 'Excel 8.0;HDR=Yes;IMEX=1'");
                        OleDbDataAdapter OLEDA = new OleDbDataAdapter("Select * from [Sheet1$] WHERE Style IS NOT NULL", OLEDBCON);
                        OLEDA.Fill(DT_EXCELSHEET);

                        int UploadCount = 0;

                        foreach (DataRow dtRow in DT_EXCELSHEET.Rows)
                        {
                            string Customer, Style, Line, Nor, Psd, Fdd = "";
                            int Oqty = 0;
                            bool IsComplete = false;

                            Customer = DataHandle.Stringfy(DataHandle.GetCustomerID(dtRow[0].ToString()));
                            Style = DataHandle.Stringfy(dtRow[1].ToString());
                            Oqty = int.Parse(dtRow[2].ToString());
                            Line = DataHandle.Stringfy(dtRow[3].ToString());
                            Psd = DataHandle.Stringfy(dtRow[4].ToString());
                            Nor = DataHandle.Stringfy(dtRow[5].ToString());
                            Fdd = DataHandle.Stringfy(dtRow[6].ToString());

                            if (DataHandle.CheckStyleIn(Style, strFactory, strYear, strMonth, strPlanID) == false)
                            {

                                if (Fdd.Length < 1 || Customer == "000")
                                {
                                    IsComplete = false;
                                    Qry = "INSERT INTO UploadTemporySheet (User_ID,Fact_ID,Year,Month,Cust_ID,Style,Oqty,Line,Psd,Nor,Fdd,IsComplete) " +
                                    "VALUES('" + Session["Var_UserID"].ToString() +
                                    "','" + strFactory +
                                    "','" + strYear +
                                    "','" + strMonth +
                                    "','" + Customer +
                                    "','" + Style +
                                    "','" + Oqty +
                                    "','" + Line +
                                    "','" + DateTime.Parse(Psd).ToShortDateString() +
                                    "','" + Nor +
                                    "',NULL,'" + IsComplete + "')";
                                }
                                else
                                {
                                    IsComplete = true;
                                    Qry = "INSERT INTO StyleMaster (Plan_ID,Cust_CatID,Style,Oqty,Line,Psd,Nor,Fdd,Status,IsOpen) " +
                                    "VALUES('" + strPlanID +
                                     "','" + Customer +
                                    "','" + Style +
                                    "','" + Oqty +
                                    "','" + Line +
                                    "','" + DateTime.Parse(Psd).ToShortDateString() +
                                    "','" + Nor +
                                    "','" + DateTime.Parse(Fdd).ToShortDateString() +
                                    "','Pending','" + IsComplete + "')";
                                }


                                UploadCount = UploadCount + 1;
                                Insert_i = DataHandle.Sql_CUD(Qry);
                            }

                        }


                    }
                    #endregion
                    
                }

                return RedirectToAction("Index");
            }
            catch (Exception EX)
            {
                Session["MsgType"] = "Error";
                Session["MsgDesc"] = EX.Message.ToString();
                return View();
            }
            finally
            {

            }
        }

        // POST: UploadForcast/Upload New File
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult UploadNew(ExcelFileUpload ExcelFileUpload)
        {
            try
            {
                strFactory = ExcelFileUpload.cmbFactory;
                strYear = DataHandle.ExtractDate(ExcelFileUpload.cmbYmonth, "Y");
                strMonth = DataHandle.ExtractDate(ExcelFileUpload.cmbYmonth, "M");
                strPlanID = DataHandle.CheckPlanDiscuss(strFactory, strYear, strMonth);

                if (strPlanID != "")//Plan in the discuss
                {
                    #region Plan Is IN Plan Master
                    foreach (string file in Request.Files)
                    {
                        HttpPostedFileBase ExcelFile = Request.Files[file];

                        OriFileName = Path.GetFileName(ExcelFile.FileName);
                        SysFileName = DateTime.Now.ToString("yyyymmddhhmmss") + "_" + strFactory + "_" + strYear + "_" + strMonth + Path.GetExtension(OriFileName);
                        SaveFilePath = Server.MapPath("~/Upload/" + SysFileName);
                        ExcelFile.SaveAs(SaveFilePath);
                        Insert_i = DataHandle.Sql_CUD("Insert Into FileHistory (User_ID,Fact_ID,Year,Month,File_Namereal,File_Nameset,File_date) values( '" + Session["Var_UserID"].ToString() + "','" + strFactory + "', '" + strYear + "', '" + strMonth + "', '" + DataHandle.Stringfy(OriFileName) + "', '" + SysFileName + "', '" + DateTime.Now.ToString() + "')");

                        //Start To Read Excel File
                        FileInfo fileInfo = new FileInfo(SaveFilePath);
                        ExcelPackage package = new ExcelPackage(fileInfo);
                        ExcelWorksheet worksheet = package.Workbook.Worksheets.FirstOrDefault();

                        // get number of rows and columns in the sheet
                        int rows = worksheet.Dimension.Rows;

                        int UploadCount = 0;

                        for (int i = 2; i <= rows; i++)
                        {
                            string Customer, Style, Line, Nor, Psd, Fdd = "";
                            int Oqty = 0;
                            bool IsComplete = false;

                            Customer = DataHandle.Stringfy(DataHandle.GetCustomerID(worksheet.Cells[i, 1].Value.ToString()));
                            Style = DataHandle.Stringfy(worksheet.Cells[i, 2].Value.ToString());
                            Oqty = int.Parse(worksheet.Cells[i, 3].Value.ToString());
                            Line = DataHandle.Stringfy(worksheet.Cells[i, 4].Value.ToString());
                            Psd = DataHandle.Stringfy(worksheet.Cells[i, 5].Value.ToString());
                            Nor = DataHandle.Stringfy(worksheet.Cells[i, 6].Value.ToString());
                            Fdd = DataHandle.Stringfy(worksheet.Cells[i, 7].Value.ToString());

                            if (DataHandle.CheckStyleIn(Style, strFactory, strYear, strMonth, strPlanID) == false)
                            {

                                if (Fdd.Length < 1 || Customer == "000")
                                {
                                    IsComplete = false;
                                    Qry = "INSERT INTO UploadTemporySheet (User_ID,Fact_ID,Year,Month,Cust_ID,Style,Oqty,Line,Psd,Nor,Fdd,IsComplete) " +
                                    "VALUES('" + Session["Var_UserID"] +
                                    "','" + strFactory +
                                    "','" + strYear +
                                    "','" + strMonth +
                                    "','" + Customer +
                                    "','" + Style +
                                    "','" + Oqty +
                                    "','" + Line +
                                    "','" + DateTime.Parse(Psd).ToShortDateString() +
                                    "','" + Nor +
                                    "',NULL,'" + IsComplete + "')";
                                }
                                else
                                {
                                    IsComplete = true;
                                    Qry = "INSERT INTO StyleMaster (Plan_ID,Cust_CatID,Style,Oqty,Line,Psd,Nor,Fdd,Status,IsOpen) " +
                                    "VALUES('" + strPlanID +
                                     "','" + Customer +
                                    "','" + Style +
                                    "','" + Oqty +
                                    "','" + Line +
                                    "','" + DateTime.Parse(Psd).ToShortDateString() +
                                    "','" + Nor +
                                    "','" + DateTime.Parse(Fdd).ToShortDateString() +
                                    "','Pending','" + IsComplete + "')";
                                }


                                UploadCount = UploadCount + 1;
                                Insert_i = DataHandle.Sql_CUD(Qry);
                            }
                        }

                            

                        Session["MsgType"] = "Success";
                        Session["MsgDesc"] = UploadCount.ToString() + " Styles Uploaded Success.";
                    }
                    #endregion
                }
                else
                {
                    #region Plan Is Not In Plan Master

                    foreach (string file in Request.Files)
                    {
                        HttpPostedFileBase ExcelFile = Request.Files[file];

                        OriFileName = Path.GetFileName(ExcelFile.FileName);
                        SysFileName = DateTime.Now.ToString("yyyymmddhhmmss") + "_" + strFactory + "_" + strYear + "_" + strMonth + Path.GetExtension(OriFileName);
                        SaveFilePath = Server.MapPath("~/Upload/" + SysFileName);
                        ExcelFile.SaveAs(SaveFilePath);
                        Insert_i = DataHandle.Sql_CUD("Insert Into FileHistory (User_ID,Fact_ID,Year,Month,File_Namereal,File_Nameset,File_date) values( '" + Session["Var_UserID"].ToString() + "','" + strFactory + "', '" + strYear + "', '" + strMonth + "', '" + DataHandle.Stringfy(OriFileName) + "', '" + SysFileName + "', '" + DateTime.Now.ToString() + "')");

                        string Nextplanid = DataHandle.SqlNextId("PlanMaster", "Plan_ID", 8);

                        Insert_i = DataHandle.Sql_CUD("Insert Into PlanMaster (Plan_ID,Fact_ID,Year,Month,PushDate) values( '" + Nextplanid + "','" + strFactory + "', '" + strYear + "', '" + strMonth + "', '" + DateTime.Now.ToString() + "')");

                        strPlanID = Nextplanid;

                        //Start To Read Excel File
                        FileInfo fileInfo = new FileInfo(SaveFilePath);
                        ExcelPackage package = new ExcelPackage(fileInfo);
                        ExcelWorksheet worksheet = package.Workbook.Worksheets.FirstOrDefault();

                        // get number of rows and columns in the sheet
                        int rows = worksheet.Dimension.Rows;

                        int UploadCount = 0;

                        for (int i = 2; i <= rows; i++)
                        {
                            string Customer, Style, Line, Nor, Psd, Fdd = "";
                            int Oqty = 0;
                            bool IsComplete = false;

                            Customer = DataHandle.Stringfy(DataHandle.GetCustomerID(worksheet.Cells[i, 1].Value.ToString()));
                            Style = DataHandle.Stringfy(worksheet.Cells[i, 2].Value.ToString());
                            Oqty = int.Parse(worksheet.Cells[i, 3].Value.ToString());
                            Line = DataHandle.Stringfy(worksheet.Cells[i, 4].Value.ToString());
                            Psd = DataHandle.Stringfy(worksheet.Cells[i, 5].Value.ToString());
                            Nor = DataHandle.Stringfy(worksheet.Cells[i, 6].Value.ToString());
                            Fdd = DataHandle.Stringfy(worksheet.Cells[i, 7].Value.ToString());

                            if (DataHandle.CheckStyleIn(Style, strFactory, strYear, strMonth, strPlanID) == false)
                            {

                                if (Fdd.Length < 1 || Customer == "000")
                                {
                                    IsComplete = false;
                                    Qry = "INSERT INTO UploadTemporySheet (User_ID,Fact_ID,Year,Month,Cust_ID,Style,Oqty,Line,Psd,Nor,Fdd,IsComplete) " +
                                    "VALUES('" + Session["Var_UserID"].ToString() +
                                    "','" + strFactory +
                                    "','" + strYear +
                                    "','" + strMonth +
                                    "','" + Customer +
                                    "','" + Style +
                                    "','" + Oqty +
                                    "','" + Line +
                                    "','" + DateTime.Parse(Psd).ToShortDateString() +
                                    "','" + Nor +
                                    "',NULL,'" + IsComplete + "')";
                                }
                                else
                                {
                                    IsComplete = true;
                                    Qry = "INSERT INTO StyleMaster (Plan_ID,Cust_CatID,Style,Oqty,Line,Psd,Nor,Fdd,Status,IsOpen) " +
                                    "VALUES('" + strPlanID +
                                     "','" + Customer +
                                    "','" + Style +
                                    "','" + Oqty +
                                    "','" + Line +
                                    "','" + DateTime.Parse(Psd).ToShortDateString() +
                                    "','" + Nor +
                                    "','" + DateTime.Parse(Fdd).ToShortDateString() +
                                    "','Pending','" + IsComplete + "')";
                                }


                                UploadCount = UploadCount + 1;
                                Insert_i = DataHandle.Sql_CUD(Qry);
                            }
                        }

                    }
                    #endregion

                }

                return RedirectToAction("Index");
            }
            catch (Exception EX)
            {
                Session["MsgType"] = "Error";
                Session["MsgDesc"] = EX.Message.ToString();
                return View();
            }
            finally
            {

            }
        }

        // POST: UploadForcast/Create
        [HttpPost]
        public ActionResult UploadEdit(FormCollection FormData)
        {
            try
            {
                DataVariables DataVariables = new DataVariables
                {
                    Var_UP_FactID = FormData["txtFactory"].ToString(),
                    Var_UP_Year = FormData["txtYear"].ToString(),
                    Var_UP_Month = FormData["txtMonth"].ToString(),
                    Var_UP_Plan = FormData["txtPlan"].ToString()
                };

                ViewBag.Message = DataVariables;

                return View();
            }
            catch
            {
                return View();
            }
        }

        public ActionResult UploadEdit()
        {
            return View();
        }

        public ActionResult StyleUpdate(FormCollection FormData)
        {
            try
            {
                DataVariables DataVariables = new DataVariables
                {
                    Var_UP_FactID = FormData["txtFactory"].ToString(),
                    Var_UP_Year = FormData["txtYear"].ToString(),
                    Var_UP_Month = FormData["txtMonth"].ToString()
                };

                ViewBag.Message = DataVariables;
                
                string var_styleId = FormData["txtTemID"].ToString();
                string var_styleDate = FormData["m_datepicker_1"].ToString();
                string var_styleCustomer = FormData["cmbCustomer"].ToString();

                if (var_styleDate == "")
                {
                    Session["MsgType"] = "Error";
                    Session["MsgDesc"] = "Sorry ! Please Select FDD in Your Form";
                    return View("UploadEdit");
                }

                if (var_styleCustomer.Length != 3)
                {
                    Session["MsgType"] = "Error";
                    Session["MsgDesc"] = "Sorry ! Please Select Customer Category in Your Form";
                    return View("UploadEdit");
                }

                DataVariables.Var_UP_Plan = DataHandle.CheckPlanDiscuss(DataVariables.Var_UP_FactID, DataVariables.Var_UP_Year, DataVariables.Var_UP_Month);

                int i = DataHandle.Sql_CUD("UPDATE UploadTemporySheet SET Cust_ID='"+ var_styleCustomer + "',Fdd='" + var_styleDate + "',IsComplete='True' WHERE TempTrnId='" + var_styleId + "'");

                i = DataHandle.Sql_CUD("INSERT INTO StyleMaster(Plan_ID,Cust_CatID,Style,Oqty,Line,Psd,Nor,Fdd,Status,IsOpen) SELECT '" + DataVariables.Var_UP_Plan + "',Cust_ID,Style,Oqty,Line,Psd,Nor,Fdd,'Pending','True' FROM UploadTemporySheet WHERE TempTrnId='" + var_styleId + "'");

                i = DataHandle.Sql_CUD("DELETE FROM UploadTemporySheet WHERE TempTrnId='" + var_styleId + "'");

                return View("UploadEdit");

            }
            catch
            {
                return View("UploadEdit");
            }
        }


    }
}
