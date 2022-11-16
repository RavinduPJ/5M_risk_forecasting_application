using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Drawing;
using System.Text;
using System.Xml;
using System.Configuration;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace _5MRF_MVC.Models
{

    public class DataVariables
    {
        [ThreadStatic]
        //System Variables
        public static string Var_SysKey = "";
        
        public static string Var_SysMode = "Open";
        
        public static string DBConString = ConfigurationManager.ConnectionStrings["CONSTR"].ConnectionString;

        
        //Common Variables For Users
        //public static string Var_UserID = "";
        
        //public static string Var_UserName = "";
       
        //public static string Var_UserMail = "";
        
        //public static string Var_UserPName = "";
        
        //public static string Var_UserFactID = "";
        
        //public static string Var_UserFactName = "";
        
        //public static string Var_UserDepID = "";
        
        //public static string Var_UserDepName = "";

        
        //User Group Permission
        //public static string Var_UserGrpID = "";
        
        //public static string Var_UserGrpName = "";
        
        //public static bool Var_PerUpload = false;
        
        //public static bool Var_PerDiscuission = false;
        
        //public static bool Var_PerFollow = false;
        
        //public static bool Var_PerFeedback = false;
        
        //public static bool Var_PerReports = false;
        
        //public static bool Var_PerAdmin = false;
        
        //public static bool Var_PerSuperAdmin = false;
        
        //public static bool Var_PerAllPlant = false;
        
        //public static bool Var_PerAllDepartment = false;
        
        //public static bool Var_PerOverall = false;
        
        //public static bool Var_PerMeetingClose = false;

        
        //UploadPlan
        public string Var_UP_FactID { get; set; }
        public string Var_UP_Year { get; set; }
        public string Var_UP_Month { get; set; }
        public string Var_UP_Plan { get; set; }


        //Select Styles For Discussion
        public string Var_Dis_FactID { get; set; }
        public string Var_Dis_Year { get; set; }
        public string Var_Dis_Month { get; set; }


        //Meeting
        public string Var_Met_FactID { get; set; }
        public string Var_Met_Year { get; set; }
        public string Var_Met_Month { get; set; }
        public string Var_Met_StyleID { get; set; }
        public string Var_Met_PlanID { get; set; }
        public string Var_Met_DeptID { get; set; }
        public string Var_Met_QAID { get; set; }


        //Overall Meeting
        public string Var_OM_FactID { get; set; }
        public string Var_OM_Year { get; set; }
        public string Var_OM_Month { get; set; }
        public string Var_OM_PlanID { get; set; }


        //Select Styles For Followup
        public string Var_Fo_PlanID { get; set; }
        
        //Followup
        public string Var_Fu_PlanID { get; set; }
        public string Var_Fu_QAID { get; set; }
        public string Var_Fu_OQAID { get; set; }
        public string Var_Fu_StyleID { get; set; }
        public string Var_Fu_UserID { get; set; }
        public string Var_Fu_FactID { get; set; }
        public string Var_Fu_Year { get; set; }
        public string Var_Fu_Month { get; set; }


        //Select Styles For Feedback
        public string Var_Fe_PlanID { get; set; }

        //Feedback
        public string Var_Fb_PlanID { get; set; }
        public string Var_Fb_QAID { get; set; }
        public string Var_Fb_OQAID { get; set; }
        public string Var_Fb_StyleID { get; set; }
        public string Var_Fb_UserID { get; set; }
        public string Var_Fb_FactID { get; set; }
        public string Var_Fb_Year { get; set; }
        public string Var_Fb_Month { get; set; }

    }

    public class MsgBox
    {
        //public string MsgType = "";
        
        //public string MsgDesc = "";
    }

    
}