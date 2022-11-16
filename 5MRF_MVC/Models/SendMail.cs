using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using _5MRF_MVC.Models;
using System.Net.Mail;
using System.Net;
using System.Configuration;

namespace _5MRF_MVC.Models
{
    public class SendMail
    {
        DataHandle DataHandle = new DataHandle();
        DataVariables DataVariables = new DataVariables();

        public void CommonSent(string To_EMAIL, string MAIL_HEAD, string MAIL_BODY)
        {
            try
            {
                SmtpClient sMail = new SmtpClient(ConfigurationManager.AppSettings["SMTP"].ToString());//exchange or smtp server goes here.
                sMail.DeliveryMethod = SmtpDeliveryMethod.Network;
                sMail.Credentials = new NetworkCredential(ConfigurationManager.AppSettings["FromEmail"].ToString(), ConfigurationManager.AppSettings["FromPassword"].ToString());
                MailMessage mailMessage = new MailMessage();

                mailMessage.From = new MailAddress(ConfigurationManager.AppSettings["FromEmail"].ToString());

                mailMessage.Subject = MAIL_HEAD;

                mailMessage.Body = MAIL_BODY;

                mailMessage.IsBodyHtml = true;

                mailMessage.To.Add(To_EMAIL);

                sMail.Send(mailMessage);
            }
            catch
            {
               
            }
            finally { }
        }
        //Mail Template
        public string MailTemp01(string UserName, string MailHead, string MailBody)
        {
            string returnmail = "";
            string Read_file_path = System.Web.HttpContext.Current.Server.MapPath("~/Mailtemplate/Temp01.html");
            //Start Build Mail
            returnmail = File.ReadAllText(Read_file_path);
            //--End Build Mail

            returnmail = returnmail.Replace("{{UserName}}", UserName);
            returnmail = returnmail.Replace("{{MailHead}}", MailHead);
            returnmail = returnmail.Replace("{{MailBody}}", MailBody);

            return returnmail;
        }
        //Month Wise Meeting Summary
        public string MonthlyMeetingsummery(string planid)
        {
            string emailserver = ConfigurationManager.AppSettings["SMTP"].ToString();
            string emailfrom = ConfigurationManager.AppSettings["FromEmail"].ToString();
            string emailpw = ConfigurationManager.AppSettings["FromPassword"].ToString();
            
            DataTable DT_MeetDetail = DataHandle.SqlSelectCMD("SELECT * FROM VIEW_PLANMASTER WHERE Plan_ID='"+ planid + "'");

            if (DT_MeetDetail.Rows.Count == 1)
            {
                string TB_head = "";
                string TB_body = "";
                MailMessage msg = new MailMessage();
                //msg.CC.Add(new MailAddress("devonp@brandix.com"));
                msg.CC.Add(new MailAddress("aroshaa@brandix.com"));

                foreach (DataRow DR_PCU in DataHandle.SqlSelectCMD("SELECT User_Mail FROM SysUser WHERE Dept_ID='009' AND Fact_ID='" + DT_MeetDetail.Rows[0]["Fact_ID"].ToString() + "' AND User_Active='True' AND User_Mail != '-'").Rows)
                {
                    msg.CC.Add(new MailAddress(DR_PCU["User_Mail"].ToString()));
                }

                TB_head = "<h2> 5M RISK FORECAST MEETING SUMMARY ///////////////</h2><hr>";
                TB_head = TB_head + "<p># Factory : " + DT_MeetDetail.Rows[0]["Fact_Name"].ToString() + "</p>";
                TB_head = TB_head + "<p># Year : " + DT_MeetDetail.Rows[0]["Year"].ToString() + "</p>";
                TB_head = TB_head + "<p># Month : " + DT_MeetDetail.Rows[0]["Month"].ToString() + "</p>";
                TB_head = TB_head + "<hr><b><p>Styles Count In Meeting</p></b></br>";

                DataTable DT_STYLE_COUNT = DataHandle.SqlSelectCMD("(SELECT COUNT(*) as w FROM StyleMaster WHERE Plan_ID = '" + 
                    planid + "') UNION ALL (SELECT COUNT(*) as h FROM StyleMaster WHERE Nor='New' AND Plan_ID = '" + 
                    planid + "') UNION ALL (SELECT COUNT(*) as k FROM StyleMaster WHERE Nor='Repeat' AND Plan_ID = '" + planid + "')");

                TB_head = TB_head + "<table>";
                TB_head = TB_head + "<tr><td>Total Styles :</td><td>"+ DT_STYLE_COUNT.Rows[0][0].ToString() + "</td></tr>";
                TB_head = TB_head + "<tr><td>Total New Styles :</td><td>" + DT_STYLE_COUNT.Rows[1][0].ToString() + "</td></tr>";
                TB_head = TB_head + "<tr><td>Total Repeat Styles :</td><td>" + DT_STYLE_COUNT.Rows[2][0].ToString() + "</td></tr>";
                TB_head = TB_head + "</table><br/><br/>";

                DataTable DT_USERSINPLANT = DataHandle.SqlSelectCMD("SELECT User_ID,User_Name,User_Mail FROM SysUser WHERE Fact_ID='" + DT_MeetDetail.Rows[0]["Fact_ID"].ToString() + "' AND User_Active='True' AND User_Mail != '-'");

                TB_body = "<table width=\"800\"><tr height=\"40\"><th width=\"200\" bgcolor=\"#537ec5\"><b>USER NAME</b></th><th width=\"200\" bgcolor=\"#f6c89f\"><b>TOTAL RISK</b></th><th width=\"200\" bgcolor=\"#71a95a\"><b>COMPLETE COUNT</b></th><th width=\"200\" bgcolor=\"#f66767\"><b>UNCOMPLETE COUNT</b></th></tr><tbody>";

                foreach (DataRow DR in DT_USERSINPLANT.Rows)
                {
                    DataTable DT_TOTAL = DataHandle.SqlSelectCMD("SELECT QA_ID AS co1 FROM PlanDiscussion WHERE QA_User_ID='"+ DR["User_ID"].ToString() + "' AND QA_OK='False' AND Style_ID IN (SELECT Style_ID FROM StyleMaster WHERE Plan_ID='"+ planid + "') UNION ALL SELECT OQA_ID FROM PlanOverall WHERE OQA_User_ID='" + DR["User_ID"].ToString() + "' AND Plan_ID='" + planid + "'");
                    DataTable DT_COMPLETE = DataHandle.SqlSelectCMD("SELECT QA_ID AS co1 FROM PlanDiscussion WHERE QA_User_ID='" + DR["User_ID"].ToString() + "' AND QA_OK='False' AND QA_IS_CLOSED='True' AND Style_ID IN (SELECT Style_ID FROM StyleMaster WHERE Plan_ID='" + planid + "') UNION ALL SELECT OQA_ID FROM PlanOverall WHERE OQA_User_ID='" + DR["User_ID"].ToString() + "' AND Plan_ID='" + planid + "' AND OQA_IS_CLOSED='True'");
                    DataTable DT_NOTCOMPLETE = DataHandle.SqlSelectCMD("SELECT QA_ID AS co1 FROM PlanDiscussion WHERE QA_User_ID='" + DR["User_ID"].ToString() + "' AND QA_OK='False' AND QA_IS_CLOSED='False' AND Style_ID IN (SELECT Style_ID FROM StyleMaster WHERE Plan_ID='" + planid + "') UNION ALL SELECT OQA_ID FROM PlanOverall WHERE OQA_User_ID='" + DR["User_ID"].ToString() + "' AND Plan_ID='" + planid + "' AND OQA_IS_CLOSED='False'");
                    msg.To.Add(new MailAddress(DR["User_Mail"].ToString()));

                    TB_body = TB_body + "<tr><td width=\"200\" bgcolor=\"#dedef0\">" + DR["User_Name"].ToString() + "</td><td width=\"200\" bgcolor=\"#ffe7d1\">" + DT_TOTAL.Rows.Count.ToString() + "</td><td width=\"200\" bgcolor=\"#eafbea\">" + DT_COMPLETE.Rows.Count.ToString() + "</td><td width=\"200\" bgcolor=\"#ffdfdf\">" + DT_NOTCOMPLETE.Rows.Count.ToString() + "</td></tr>";

                }
                
                TB_body = TB_body + "</tbody></table></br></br></br><a href=\"http://bci-elp-01:8080/Account/Login\">5M RISK FORECASTING TOOL LOGIN ( Click Here )</a></br>";
                TB_body = TB_body + "<p style=\"color: red;\">** This is a system generated email, do not reply to this email id <BFFSYSALERT@brandix.com>.</p>";

                msg.From = new MailAddress(emailfrom);
                msg.Subject = "5M Risk Forecast Meeting Summary";
                msg.Body = TB_head+ TB_body;
                msg.IsBodyHtml = true;

                SmtpClient client = new SmtpClient();
                 
                client.Credentials = new System.Net.NetworkCredential(emailfrom, emailpw);
                //client.UseDefaultCredentials = false;
                client.Port = 25; // You can use Port 25 if 587 is blocked (mine is!)
                client.Host = emailserver; //smtp.office365.com
                client.EnableSsl = false;
                client.
                //client.DeliveryMethod = SmtpDeliveryMethod.Network;
                try
                {
                    client.Send(msg);
                    return "E-mail Send Successfully";
                }
                catch (Exception ex)
                {
                    return ex.Message.ToString();
                }


            }

            

            return "Meeting Not Found.";
        }
    }
}