using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using _5MRF_MVC.Models;
using System.IO;

namespace _5MRF_MVC.Controllers
{
    public class AccountController : Controller
    {
        DataHandle DataHandle = new DataHandle();
        DataVariables DataVariables = new DataVariables();
        MsgBox MsgBox = new MsgBox();
        SendMail SendMail = new SendMail();
        DataTable DT = new DataTable();

        // GET: Account
        public ActionResult Index()
        {
             return View();
        }


        public ActionResult Login()
        {
            Session["SessionKey"] = "";
            Session["SessionStatus"] = "";

            return View();
        }

        public ActionResult AccountSelect()
        {
            if(Session["Var_UserName"].ToString() == "")
            {
                return RedirectToAction("Signout", "Account");
            }

            return View();
        }

        public ActionResult ReLogin()
        {
            Session["SessionKey"] = "";
            Session["SessionStatus"] = "";

            return View();
        }

        public ActionResult Signout()
        {
            Session["Var_UserID"] = "";
            Session["Var_UserName"] = "";
            Session["Var_UserMail"] = "";
            Session["Var_UserPName"] = "";
            Session["Var_UserFactID"] = "";
            Session["Var_UserFactName"] = "";
            Session["Var_UserDepID"] = "";
            Session["Var_UserDepName"] = "";
            Session["Var_UserGrpID"] = "";
            Session["Var_UserGrpName"] = "";
            Session["Var_UserImg"] = "";

            Session["Var_PerUpload"] = false;
            Session["Var_PerDiscuission"] = false;
            Session["Var_PerFollow"] = false;
            Session["Var_PerFeedback"] = false;
            Session["Var_PerReports"] = false;
            Session["Var_PerAdmin"] = false;
            Session["Var_PerSuperAdmin"] = false;
            Session["Var_PerAllPlant"] = false;
            Session["Var_PerAllDepartment"] = false;
            Session["Var_PerOverall"] = false;
            Session["Var_PerMeetingClose"] = false;

            Session["SessionKey"] = "";
            Session["SessionStatus"] = "CLOSE";

            Session.RemoveAll();

            return RedirectToAction("Login", "Account");
        }

        // POST: Account/Create
        [HttpPost]
        public ActionResult LoginAD(FormCollection FormData)
        {
            try
            {
                string username = FormData["txtUsername"].ToString();
                string password = FormData["txtPassword"].ToString();
                bool valid = false;

                using (PrincipalContext context = new PrincipalContext(ContextType.Domain, "BRANDIXLK"))
                {
                    valid = context.ValidateCredentials(username, password);
                }

                if (valid == true)
                {
                    DT = DataHandle.SqlSelectCMD("SELECT * FROM VIEW_FORUSERLOGIN WHERE User_Name='" + username + "' AND User_Active='True' AND Grp_Active='True'");

                    if (DT.Rows.Count == 1)
                    {
                        Session["Var_UserID"] = DT.Rows[0]["User_ID"].ToString();
                        Session["Var_UserName"] = DT.Rows[0]["User_Name"].ToString();
                        Session["Var_UserMail"] = DT.Rows[0]["User_Mail"].ToString();
                        Session["Var_UserPName"] = DT.Rows[0]["User_PName"].ToString();
                        Session["Var_UserFactID"] = DT.Rows[0]["Fact_ID"].ToString();
                        Session["Var_UserFactName"] = DT.Rows[0]["Fact_Name"].ToString();
                        Session["Var_UserDepID"] = DT.Rows[0]["Dept_ID"].ToString();
                        Session["Var_UserDepName"] = DT.Rows[0]["Dept_Name"].ToString();
                        Session["Var_UserGrpID"] = DT.Rows[0]["Grp_ID"].ToString();
                        Session["Var_UserGrpName"] = DT.Rows[0]["Grp_Name"].ToString();

                        
                        //Image
                        try
                        {
                            var directoryEntry = new DirectoryEntry("LDAP://BRANDIXLK");
                            var directorySearcher = new DirectorySearcher(directoryEntry);
                            directorySearcher.Filter = string.Format("(&(SAMAccountName={0}))", Session["Var_UserName"]);
                            var user = directorySearcher.FindOne();
                            byte[] imageArray = user.Properties["thumbnailPhoto"][0] as byte[];

                            string base64ImageRepresentation = Convert.ToBase64String(imageArray);
                            Session["Var_UserImg"] = base64ImageRepresentation;

                        }
                        catch
                        {
                            Session["Var_UserImg"] = "iVBORw0KGgoAAAANSUhEUgAAAgAAAAIAAgMAAACJFjxpAAAADFBMVEXFxcX////p6enW1tbAmiBwAAAFiElEQVR4AezAgQAAAACAoP2pF6kAAAAAAAAAAAAAAIDbu2MkvY0jiuMWWQoUmI50BB+BgRTpCAz4G6C8CJDrC3AEXGKPoMTlYA/gAJfwETawI8cuBs5Nk2KtvfiLW+gLfK9m+r3X82G653+JP/zjF8afP1S//y+An4/i51//AsB4aH+/QPD6EQAY/zwZwN8BAP50bh786KP4+VT+3fs4/noigEc+jnHeJrzxX+NWMDDh4g8+EXcnLcC9T8U5S/CdT8bcUeBEIrwBOiI8ki7Ba5+NrePgWUy89/nYyxQ8Iw3f+pWY4h1gb3eAW7sDTPEOsLc7wK1TIeDuDB+I/OA1QOUHv/dFsZQkhKkh4QlEfOULYz2nGj2/Nn1LmwR/86VxlCoAW6kCsHRGANx1RgCMo5Qh2EsZgrXNQZZShp5Liv7Il8eIc5C91EHY2hxk6bwYmNscZIReDBwtCdhbErC1JGBpScBcOgFMLQsZMQs5Whayd+UQsLYsZGlZyNyykKllISNmIUfAwifw8NXvTojAjGFrdYi11SGWVoeYWx1i6lmQCiEjFkKOVgjZ+xxIhZCtFULWHkCqxCw9gNQKmP9vNHzipdEPrRcxtVbAeDkAvve0iM2QozVD9hfjhp4YP/UrkJYDbD2AtBxgfSkAvvHEeNcDSAsilgtAWxIy91J8AXgZAJ5e33+4tuACcAG4AFwALgBXRXQB6AFcB5MXAuA6nl9/0Vx/011/1V5/1/dfTPJvRtdnu/zL6beeFO/7r+fXBYbrEkt/j+i6ytXfpuvvE/ZXOnsA/a3a/l5xf7O6v1t+Xe/vOyz6HpO8yyboM8o7rfJes77bru83THk48p7TvOs27zvOO6/73vO++z7l4cgnMPQzKPopHC0N9noSSz6LJp/Gk88jyicy5TOp6qlc+VyyfDJbPpuuns6XzyfMJzTmMyrrKZ35nNJ8Ums+q7af1tvPK+4nNodEnPKp3fnc8npyez67/qVP7+/fL8hfcMjfsOhf8cjfMclfcnn9+BkOnLECP8Q58OYeyJ40eoyF6Ee/En/JHlP6mIlRVXprF4BxtAvArV0AxtEuALd2ARhHuwDc2gVgHPX/hFv9fMBddjIGeKg/WCxlCsI46u+Ga5mCcJd+sIG9UkGAW32ZbApFAHhod4Bb3eo04h3god0BbiUHYApVCNjbHeBW+QDAXT4a7qg7r7e214057vg0QhkEHkoSwq0kIdydXw4/Q3H8hjYJ3vL0WConBJhCHQaOToeBrU0BljYFmEoVgHGUKgAPnREAt84IgLuqFgAYSUEOAHszDwuAtSkHAZhLGYIpdCLgKGUIHtocZG1zkLmUIRhxDnJU1RDA1uYga5uDzKUOwhTnIEfnxcDe5iBrcyQAYGlzkKkUYhhxDrKXQgxbSwLWUohhbknA1JKAEZOAvSUBW0sC1pYEzC0JmFoSMMJyCDhaFrK3JGDtyiFgaVnI3LKQqWUhI2YhR8tC9paFrC0LWVoWMrcsZGpZyIhZyNGykL2rSIGtlQHWVgZYWhlgbmWAqZUBRiwDHK0MsLcywNbKAGsOoNUhllaHmFsdYmp1iBHrEEerQ+w5gFYI2VodYm11iKXVIeYcQCuETK0QMmIh5MgBtELI3gohWyuErDmAVolZWiFkzgG0SszUKjGjfj6gVmKOVonZcwCtFbB9HQC+ozWDbz1bvGu9iKW1AuYcQOtFTLEX1GbIaFegN0OOHEBrhuw5gNYM2XIArRuz5gDacoB3bTnAEktxXQ4wfw0AvveM8b4tiJjSJOwLIsbXsAKeNeKCiOO3D+AVbUl0AfjGs8ZPbUnIdgFoa1LWC0BblfMuB9AeC1j6gqQE0J9LmC8AOYD2ZMb7i4bt2ZTpWoHfPoB7Tj2fXzT8N1X41vkq/QHOAAAAAElFTkSuQmCC";
                        }

                        //User Group Permission

                        Session["Var_PerUpload"] = bool.Parse(DT.Rows[0]["Grp_Upload_Plan"].ToString());
                        Session["Var_PerDiscuission"] = bool.Parse(DT.Rows[0]["Grp_Discuission"].ToString());
                        Session["Var_PerFollow"] = bool.Parse(DT.Rows[0]["Grp_Follow"].ToString());
                        Session["Var_PerFeedback"] = bool.Parse(DT.Rows[0]["Grp_Feedback"].ToString());
                        Session["Var_PerReports"] = bool.Parse(DT.Rows[0]["Grp_Reports"].ToString());
                        Session["Var_PerAdmin"] = bool.Parse(DT.Rows[0]["Grp_Admin"].ToString());
                        Session["Var_PerSuperAdmin"] = bool.Parse(DT.Rows[0]["Grp_SuperAdmin"].ToString());
                        Session["Var_PerAllPlant"] = bool.Parse(DT.Rows[0]["Grp_AllPlant"].ToString());
                        Session["Var_PerAllDepartment"] = bool.Parse(DT.Rows[0]["Grp_AllDepartment"].ToString());
                        Session["Var_PerOverall"] = bool.Parse(DT.Rows[0]["Grp_Overall"].ToString());
                        Session["Var_PerMeetingClose"] = bool.Parse(DT.Rows[0]["Grp_MeetingClose"].ToString());

                        Session["SessionKey"] = "Kw34RT778UHjj7BBB000d";
                        Session["SessionStatus"] = "OPEN";

                        Session["MsgType"] = "";
                        Session["MsgDesc"] = "";

                        int i = DataHandle.Sql_CUD("UPDATE SysUser SET User_Lastlog='" + DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss") + "' WHERE User_Name='" + DT.Rows[0]["User_Name"].ToString() + "'");

                        return RedirectToAction("Index", "Account");
                    }
                    else if (DT.Rows.Count > 1)
                    {
                        Session["Var_UserName"] = DT.Rows[0]["User_Name"].ToString();
                        Session["Var_UserMail"] = DT.Rows[0]["User_Mail"].ToString();
                        Session["Var_UserPName"] = DT.Rows[0]["User_PName"].ToString();

                        //Image
                        try
                        {
                            var directoryEntry = new DirectoryEntry("LDAP://BRANDIXLK");
                            var directorySearcher = new DirectorySearcher(directoryEntry);
                            directorySearcher.Filter = string.Format("(&(SAMAccountName={0}))", Session["Var_UserName"]);
                            var user = directorySearcher.FindOne();
                            byte[] imageArray = user.Properties["thumbnailPhoto"][0] as byte[];

                            string base64ImageRepresentation = Convert.ToBase64String(imageArray);
                            Session["Var_UserImg"] = base64ImageRepresentation;

                        }
                        catch
                        {
                            Session["Var_UserImg"] = "iVBORw0KGgoAAAANSUhEUgAAAgAAAAIAAgMAAACJFjxpAAAADFBMVEXFxcX////p6enW1tbAmiBwAAAFiElEQVR4AezAgQAAAACAoP2pF6kAAAAAAAAAAAAAAIDbu2MkvY0jiuMWWQoUmI50BB+BgRTpCAz4G6C8CJDrC3AEXGKPoMTlYA/gAJfwETawI8cuBs5Nk2KtvfiLW+gLfK9m+r3X82G653+JP/zjF8afP1S//y+An4/i51//AsB4aH+/QPD6EQAY/zwZwN8BAP50bh786KP4+VT+3fs4/noigEc+jnHeJrzxX+NWMDDh4g8+EXcnLcC9T8U5S/CdT8bcUeBEIrwBOiI8ki7Ba5+NrePgWUy89/nYyxQ8Iw3f+pWY4h1gb3eAW7sDTPEOsLc7wK1TIeDuDB+I/OA1QOUHv/dFsZQkhKkh4QlEfOULYz2nGj2/Nn1LmwR/86VxlCoAW6kCsHRGANx1RgCMo5Qh2EsZgrXNQZZShp5Liv7Il8eIc5C91EHY2hxk6bwYmNscZIReDBwtCdhbErC1JGBpScBcOgFMLQsZMQs5Whayd+UQsLYsZGlZyNyykKllISNmIUfAwifw8NXvTojAjGFrdYi11SGWVoeYWx1i6lmQCiEjFkKOVgjZ+xxIhZCtFULWHkCqxCw9gNQKmP9vNHzipdEPrRcxtVbAeDkAvve0iM2QozVD9hfjhp4YP/UrkJYDbD2AtBxgfSkAvvHEeNcDSAsilgtAWxIy91J8AXgZAJ5e33+4tuACcAG4AFwALgBXRXQB6AFcB5MXAuA6nl9/0Vx/011/1V5/1/dfTPJvRtdnu/zL6beeFO/7r+fXBYbrEkt/j+i6ytXfpuvvE/ZXOnsA/a3a/l5xf7O6v1t+Xe/vOyz6HpO8yyboM8o7rfJes77bru83THk48p7TvOs27zvOO6/73vO++z7l4cgnMPQzKPopHC0N9noSSz6LJp/Gk88jyicy5TOp6qlc+VyyfDJbPpuuns6XzyfMJzTmMyrrKZ35nNJ8Ums+q7af1tvPK+4nNodEnPKp3fnc8npyez67/qVP7+/fL8hfcMjfsOhf8cjfMclfcnn9+BkOnLECP8Q58OYeyJ40eoyF6Ee/En/JHlP6mIlRVXprF4BxtAvArV0AxtEuALd2ARhHuwDc2gVgHPX/hFv9fMBddjIGeKg/WCxlCsI46u+Ga5mCcJd+sIG9UkGAW32ZbApFAHhod4Bb3eo04h3god0BbiUHYApVCNjbHeBW+QDAXT4a7qg7r7e214057vg0QhkEHkoSwq0kIdydXw4/Q3H8hjYJ3vL0WConBJhCHQaOToeBrU0BljYFmEoVgHGUKgAPnREAt84IgLuqFgAYSUEOAHszDwuAtSkHAZhLGYIpdCLgKGUIHtocZG1zkLmUIRhxDnJU1RDA1uYga5uDzKUOwhTnIEfnxcDe5iBrcyQAYGlzkKkUYhhxDrKXQgxbSwLWUohhbknA1JKAEZOAvSUBW0sC1pYEzC0JmFoSMMJyCDhaFrK3JGDtyiFgaVnI3LKQqWUhI2YhR8tC9paFrC0LWVoWMrcsZGpZyIhZyNGykL2rSIGtlQHWVgZYWhlgbmWAqZUBRiwDHK0MsLcywNbKAGsOoNUhllaHmFsdYmp1iBHrEEerQ+w5gFYI2VodYm11iKXVIeYcQCuETK0QMmIh5MgBtELI3gohWyuErDmAVolZWiFkzgG0SszUKjGjfj6gVmKOVonZcwCtFbB9HQC+ozWDbz1bvGu9iKW1AuYcQOtFTLEX1GbIaFegN0OOHEBrhuw5gNYM2XIArRuz5gDacoB3bTnAEktxXQ4wfw0AvveM8b4tiJjSJOwLIsbXsAKeNeKCiOO3D+AVbUl0AfjGs8ZPbUnIdgFoa1LWC0BblfMuB9AeC1j6gqQE0J9LmC8AOYD2ZMb7i4bt2ZTpWoHfPoB7Tj2fXzT8N1X41vkq/QHOAAAAAElFTkSuQmCC";
                        }

                        return RedirectToAction("AccountSelect", "Account");
                    }
                    else
                    {
                        Session["Var_UserID"] = "";
                        Session["Var_UserName"] = "";
                        Session["Var_UserMail"] = "";
                        Session["Var_UserPName"] = "";
                        Session["Var_UserFactID"] = "";
                        Session["Var_UserFactName"] = "";
                        Session["Var_UserDepID"] = "";
                        Session["Var_UserDepName"] = "";
                        Session["Var_UserGrpID"] = "";
                        Session["Var_UserGrpName"] = "";

                        Session["Var_UserImg"] = "";
                        //User Group Permission

                        Session["Var_PerUpload"] = false;
                        Session["Var_PerDiscuission"] = false;
                        Session["Var_PerFollow"] = false;
                        Session["Var_PerFeedback"] = false;
                        Session["Var_PerReports"] = false;
                        Session["Var_PerAdmin"] = false;
                        Session["Var_PerSuperAdmin"] = false;
                        Session["Var_PerAllPlant"] = false;
                        Session["Var_PerAllDepartment"] = false;
                        Session["Var_PerOverall"] = false;
                        Session["Var_PerMeetingClose"] = false;
                        Session.RemoveAll();

                        Session["SessionKey"] = "";
                        Session["SessionStatus"] = "CLOSE";

                        Session["MsgType"] = "Error";
                        Session["MsgDesc"] = "Oooh !, Please Try Again.";

                        return RedirectToAction("ReLogin", "Account");
                    }
                }

                return RedirectToAction("ReLogin", "Account");
            }
            catch
            {
                return RedirectToAction("ReLogin", "Account");
            }
        }

        // POST: Account/Create
        [HttpPost]
        public ActionResult Login(FormCollection FormData)
        {
            try
            {
                string Uname = FormData["txtUsername"].ToString();
                string Pwd = FormData["txtPassword"].ToString();

                DT = DataHandle.SqlSelectCMD("SELECT * FROM VIEW_FORUSERLOGIN WHERE User_Name='" + Uname + "' AND User_PW='" + Pwd + "' AND User_Active='True' AND Grp_Active='True'");

                if (DT.Rows.Count == 1)
                {
                    Session["Var_UserID"] = DT.Rows[0]["User_ID"].ToString();
                    Session["Var_UserName"] = DT.Rows[0]["User_Name"].ToString();
                    Session["Var_UserMail"] = DT.Rows[0]["User_Mail"].ToString();
                    Session["Var_UserPName"] = DT.Rows[0]["User_PName"].ToString();
                    Session["Var_UserFactID"] = DT.Rows[0]["Fact_ID"].ToString();
                    Session["Var_UserFactName"] = DT.Rows[0]["Fact_Name"].ToString();
                    Session["Var_UserDepID"] = DT.Rows[0]["Dept_ID"].ToString();
                    Session["Var_UserDepName"] = DT.Rows[0]["Dept_Name"].ToString();
                    Session["Var_UserGrpID"] = DT.Rows[0]["Grp_ID"].ToString();
                    Session["Var_UserGrpName"] = DT.Rows[0]["Grp_Name"].ToString();
                    //User Group Permission

                    Session["Var_PerUpload"] = bool.Parse(DT.Rows[0]["Grp_Upload_Plan"].ToString());
                    Session["Var_PerDiscuission"] = bool.Parse(DT.Rows[0]["Grp_Discuission"].ToString());
                    Session["Var_PerFollow"] = bool.Parse(DT.Rows[0]["Grp_Follow"].ToString());
                    Session["Var_PerFeedback"] = bool.Parse(DT.Rows[0]["Grp_Feedback"].ToString());
                    Session["Var_PerReports"] = bool.Parse(DT.Rows[0]["Grp_Reports"].ToString());
                    Session["Var_PerAdmin"] = bool.Parse(DT.Rows[0]["Grp_Admin"].ToString());
                    Session["Var_PerSuperAdmin"] = bool.Parse(DT.Rows[0]["Grp_SuperAdmin"].ToString());
                    Session["Var_PerAllPlant"] = bool.Parse(DT.Rows[0]["Grp_AllPlant"].ToString());
                    Session["Var_PerAllDepartment"] = bool.Parse(DT.Rows[0]["Grp_AllDepartment"].ToString());
                    Session["Var_PerOverall"] = bool.Parse(DT.Rows[0]["Grp_Overall"].ToString());
                    Session["Var_PerMeetingClose"] = bool.Parse(DT.Rows[0]["Grp_MeetingClose"].ToString());

                    Session["SessionKey"] = "Kw34RT778UHjj7BBB000d";
                    Session["SessionStatus"] = "OPEN";

                    Session["MsgType"] = "";
                    Session["MsgDesc"] = "";

                    int i = DataHandle.Sql_CUD("UPDATE SysUser SET User_Lastlog='"+ DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss") +"' WHERE User_Name='"+ DT.Rows[0]["User_Name"].ToString() + "'");

                    return RedirectToAction("Index", "Account");
                }
                else
                {
                    Session["Var_UserID"] = "";
                    Session["Var_UserName"] = "";
                    Session["Var_UserMail"] = "";
                    Session["Var_UserPName"] = "";
                    Session["Var_UserFactID"] = "";
                    Session["Var_UserFactName"] = "";
                    Session["Var_UserDepID"] = "";
                    Session["Var_UserDepName"] = "";
                    Session["Var_UserGrpID"] = "";
                    Session["Var_UserGrpName"] = "";
                    //User Group Permission

                    Session["Var_PerUpload"] = false;
                    Session["Var_PerDiscuission"] = false;
                    Session["Var_PerFollow"] = false;
                    Session["Var_PerFeedback"] = false;
                    Session["Var_PerReports"] = false;
                    Session["Var_PerAdmin"] = false;
                    Session["Var_PerSuperAdmin"] = false;
                    Session["Var_PerAllPlant"] = false;
                    Session["Var_PerAllDepartment"] = false;
                    Session["Var_PerOverall"] = false;
                    Session["Var_PerMeetingClose"] = false;
                    Session.RemoveAll();

                    Session["SessionKey"] = "";
                    Session["SessionStatus"] = "CLOSE";

                    Session["MsgType"] = "Error";
                    Session["MsgDesc"] = "Oooh !, Please Try Again.";

                    return RedirectToAction("Login", "Account");
                }


            }
            catch (Exception EX)
            {
                Session["MsgType"] = "Error";
                Session["MsgDesc"] = EX.Message.ToString();
                
                Session["SessionKey"] = "";
                Session["SessionStatus"] = "CLOSE";

                return RedirectToAction("Login", "Account");
            }
        }
         
        // GET: Account/Create
        public ActionResult RequestPassword()
        {
            return View();
        }

        public ActionResult GenerateSend(FormCollection FormData)
        {
            //try
            //{
            //    string ReqUname = FormData["txtUsername"].ToString();

            //    DT = DataHandle.SqlSelectCMD("SELECT * FROM VIEW_FORUSERLOGIN WHERE User_Name='" + ReqUname + "' AND User_Active='True' AND Grp_Active='True'");

            //    if (DT.Rows.Count == 1)
            //    {
            //        string newpass = DataHandle.RandomString(7);
            //        int up_int = DataHandle.Sql_CUD("UPDATE SysUser SET  User_PW='"+ newpass + "' WHERE User_Name='"+ ReqUname + "'");

            //        string HTMLMAIL = SendMail.MailTemp01(ReqUname, "New System Password", "Your New Password : "+ newpass);
            //        SendMail.CommonSent(ReqUname + "@brandix.com", "Password Reset", HTMLMAIL);
            //        Session["MsgType"] = "Success";
            //        Session["MsgDesc"] = "Hi !, We Send New Password To Your Email";
            //    }
            //    else
            //    {
            //        Session["MsgType"] = "Error";
            //        Session["MsgDesc"] = "Sorry !, You aren't active user in this system. please contact systemadmin";
            //    }

            //}
            //catch (Exception EX)
            //{
            //    Session["MsgType"] = "Error";
            //    Session["MsgDesc"] = EX.Message.ToString();

            //    Session["SessionKey"] = "";
            //    Session["SessionStatus"] = "CLOSE";

            //    return RedirectToAction("RequestPassword", "Account");
            //}

            //string Uname = FormData["txtUsername"].ToString();

            return RedirectToAction("RequestPassword", "Account");
        }

        public ActionResult PasswordReset()
        { 
            return View();
        }

        // POST: Account/Create
        [HttpPost]
        public ActionResult PasswordReset(FormCollection FormData)
        {
            //try
            //{
            //    string textOPass = FormData["txtOldPass"].ToString();
            //    string textNPass = FormData["txtNewPass"].ToString();

            //    DT = DataHandle.SqlSelectCMD("SELECT * FROM VIEW_FORUSERLOGIN WHERE User_Name='" + Session["Var_UserName"] + "' AND User_PW='" + textOPass + "' AND User_Active='True' AND Grp_Active='True'");

            //    if (DT.Rows.Count == 1)
            //    {
            //        int up_int = DataHandle.Sql_CUD("UPDATE SysUser SET  User_PW='" + textNPass + "' WHERE User_Name='" + Session["Var_UserName"] + "'");
            //        Session["MsgType"] = "Success";
            //        Session["MsgDesc"] = "Hi !, Please Login Again With Your New Password";
            //    }
            //    else
            //    {
            //        Session["MsgType"] = "Error";
            //        Session["MsgDesc"] = "Sorry !, You aren't active user in this system. please contact systemadmin";
            //    }


            //    return RedirectToAction("Signout", "Account");
            //}
            //catch
            //{
            //    Session["MsgType"] = "Error";
            //    Session["MsgDesc"] = "Sorry !, We Can't Complete Your Operation";
            //    return RedirectToAction("PasswordReset", "Account");
            //}

            return RedirectToAction("PasswordReset", "Account");
        }

        [HttpGet]
        public ActionResult Plant(string id)
        {
            DT = DataHandle.SqlSelectCMD("SELECT * FROM VIEW_FORUSERLOGIN WHERE User_ID='" + id + "' AND User_Active='True' AND Grp_Active='True'");

            if (DT.Rows.Count == 1)
            {
                Session["Var_UserID"] = DT.Rows[0]["User_ID"].ToString();
                Session["Var_UserName"] = DT.Rows[0]["User_Name"].ToString();
                Session["Var_UserMail"] = DT.Rows[0]["User_Mail"].ToString();
                Session["Var_UserPName"] = DT.Rows[0]["User_PName"].ToString();
                Session["Var_UserFactID"] = DT.Rows[0]["Fact_ID"].ToString();
                Session["Var_UserFactName"] = DT.Rows[0]["Fact_Name"].ToString();
                Session["Var_UserDepID"] = DT.Rows[0]["Dept_ID"].ToString();
                Session["Var_UserDepName"] = DT.Rows[0]["Dept_Name"].ToString();
                Session["Var_UserGrpID"] = DT.Rows[0]["Grp_ID"].ToString();
                Session["Var_UserGrpName"] = DT.Rows[0]["Grp_Name"].ToString();


                //Image
                try
                {
                    var directoryEntry = new DirectoryEntry("LDAP://BRANDIXLK");
                    var directorySearcher = new DirectorySearcher(directoryEntry);
                    directorySearcher.Filter = string.Format("(&(SAMAccountName={0}))", Session["Var_UserName"]);
                    var user = directorySearcher.FindOne();
                    byte[] imageArray = user.Properties["thumbnailPhoto"][0] as byte[];

                    string base64ImageRepresentation = Convert.ToBase64String(imageArray);
                    Session["Var_UserImg"] = base64ImageRepresentation;

                }
                catch
                {
                    Session["Var_UserImg"] = "iVBORw0KGgoAAAANSUhEUgAAAgAAAAIAAgMAAACJFjxpAAAADFBMVEXFxcX////p6enW1tbAmiBwAAAFiElEQVR4AezAgQAAAACAoP2pF6kAAAAAAAAAAAAAAIDbu2MkvY0jiuMWWQoUmI50BB+BgRTpCAz4G6C8CJDrC3AEXGKPoMTlYA/gAJfwETawI8cuBs5Nk2KtvfiLW+gLfK9m+r3X82G653+JP/zjF8afP1S//y+An4/i51//AsB4aH+/QPD6EQAY/zwZwN8BAP50bh786KP4+VT+3fs4/noigEc+jnHeJrzxX+NWMDDh4g8+EXcnLcC9T8U5S/CdT8bcUeBEIrwBOiI8ki7Ba5+NrePgWUy89/nYyxQ8Iw3f+pWY4h1gb3eAW7sDTPEOsLc7wK1TIeDuDB+I/OA1QOUHv/dFsZQkhKkh4QlEfOULYz2nGj2/Nn1LmwR/86VxlCoAW6kCsHRGANx1RgCMo5Qh2EsZgrXNQZZShp5Liv7Il8eIc5C91EHY2hxk6bwYmNscZIReDBwtCdhbErC1JGBpScBcOgFMLQsZMQs5Whayd+UQsLYsZGlZyNyykKllISNmIUfAwifw8NXvTojAjGFrdYi11SGWVoeYWx1i6lmQCiEjFkKOVgjZ+xxIhZCtFULWHkCqxCw9gNQKmP9vNHzipdEPrRcxtVbAeDkAvve0iM2QozVD9hfjhp4YP/UrkJYDbD2AtBxgfSkAvvHEeNcDSAsilgtAWxIy91J8AXgZAJ5e33+4tuACcAG4AFwALgBXRXQB6AFcB5MXAuA6nl9/0Vx/011/1V5/1/dfTPJvRtdnu/zL6beeFO/7r+fXBYbrEkt/j+i6ytXfpuvvE/ZXOnsA/a3a/l5xf7O6v1t+Xe/vOyz6HpO8yyboM8o7rfJes77bru83THk48p7TvOs27zvOO6/73vO++z7l4cgnMPQzKPopHC0N9noSSz6LJp/Gk88jyicy5TOp6qlc+VyyfDJbPpuuns6XzyfMJzTmMyrrKZ35nNJ8Ums+q7af1tvPK+4nNodEnPKp3fnc8npyez67/qVP7+/fL8hfcMjfsOhf8cjfMclfcnn9+BkOnLECP8Q58OYeyJ40eoyF6Ee/En/JHlP6mIlRVXprF4BxtAvArV0AxtEuALd2ARhHuwDc2gVgHPX/hFv9fMBddjIGeKg/WCxlCsI46u+Ga5mCcJd+sIG9UkGAW32ZbApFAHhod4Bb3eo04h3god0BbiUHYApVCNjbHeBW+QDAXT4a7qg7r7e214057vg0QhkEHkoSwq0kIdydXw4/Q3H8hjYJ3vL0WConBJhCHQaOToeBrU0BljYFmEoVgHGUKgAPnREAt84IgLuqFgAYSUEOAHszDwuAtSkHAZhLGYIpdCLgKGUIHtocZG1zkLmUIRhxDnJU1RDA1uYga5uDzKUOwhTnIEfnxcDe5iBrcyQAYGlzkKkUYhhxDrKXQgxbSwLWUohhbknA1JKAEZOAvSUBW0sC1pYEzC0JmFoSMMJyCDhaFrK3JGDtyiFgaVnI3LKQqWUhI2YhR8tC9paFrC0LWVoWMrcsZGpZyIhZyNGykL2rSIGtlQHWVgZYWhlgbmWAqZUBRiwDHK0MsLcywNbKAGsOoNUhllaHmFsdYmp1iBHrEEerQ+w5gFYI2VodYm11iKXVIeYcQCuETK0QMmIh5MgBtELI3gohWyuErDmAVolZWiFkzgG0SszUKjGjfj6gVmKOVonZcwCtFbB9HQC+ozWDbz1bvGu9iKW1AuYcQOtFTLEX1GbIaFegN0OOHEBrhuw5gNYM2XIArRuz5gDacoB3bTnAEktxXQ4wfw0AvveM8b4tiJjSJOwLIsbXsAKeNeKCiOO3D+AVbUl0AfjGs8ZPbUnIdgFoa1LWC0BblfMuB9AeC1j6gqQE0J9LmC8AOYD2ZMb7i4bt2ZTpWoHfPoB7Tj2fXzT8N1X41vkq/QHOAAAAAElFTkSuQmCC";
                }

                //User Group Permission

                Session["Var_PerUpload"] = bool.Parse(DT.Rows[0]["Grp_Upload_Plan"].ToString());
                Session["Var_PerDiscuission"] = bool.Parse(DT.Rows[0]["Grp_Discuission"].ToString());
                Session["Var_PerFollow"] = bool.Parse(DT.Rows[0]["Grp_Follow"].ToString());
                Session["Var_PerFeedback"] = bool.Parse(DT.Rows[0]["Grp_Feedback"].ToString());
                Session["Var_PerReports"] = bool.Parse(DT.Rows[0]["Grp_Reports"].ToString());
                Session["Var_PerAdmin"] = bool.Parse(DT.Rows[0]["Grp_Admin"].ToString());
                Session["Var_PerSuperAdmin"] = bool.Parse(DT.Rows[0]["Grp_SuperAdmin"].ToString());
                Session["Var_PerAllPlant"] = bool.Parse(DT.Rows[0]["Grp_AllPlant"].ToString());
                Session["Var_PerAllDepartment"] = bool.Parse(DT.Rows[0]["Grp_AllDepartment"].ToString());
                Session["Var_PerOverall"] = bool.Parse(DT.Rows[0]["Grp_Overall"].ToString());
                Session["Var_PerMeetingClose"] = bool.Parse(DT.Rows[0]["Grp_MeetingClose"].ToString());

                Session["SessionKey"] = "Kw34RT778UHjj7BBB000d";
                Session["SessionStatus"] = "OPEN";

                Session["MsgType"] = "";
                Session["MsgDesc"] = "";

                int i = DataHandle.Sql_CUD("UPDATE SysUser SET User_Lastlog='" + DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss") + "' WHERE User_Name='" + DT.Rows[0]["User_Name"].ToString() + "'");

                return RedirectToAction("Index", "Account");
            }
            else
            {
                return RedirectToAction("Signout", "Account");
            }
        }




    }
}
