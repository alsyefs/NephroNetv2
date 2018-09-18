using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NephroNet
{
    public partial class AdminMaster : MasterPage
    {
        static string conn = "";
        SqlConnection connect = new SqlConnection(conn);
        string username, roleId, loginId, token;
        protected void Page_Load(object sender, EventArgs e)
        {
            initialAccess();
        }
        protected void initialAccess()
        {
            Configuration config = new Configuration();
            conn = config.getConnectionString();
            connect = new SqlConnection(conn);
            getSession();
            //Get from and to pages:
            string currentPage = "", previousPage = "";
            if (HttpContext.Current.Request.Url.AbsoluteUri != null) currentPage = HttpContext.Current.Request.Url.AbsoluteUri;
            if (Request.UrlReferrer != null) previousPage = Request.UrlReferrer.ToString();
            //Get current time:
            DateTime currentTime = DateTime.Now;
            //Get user's IP:
            string userIP = GetIPAddress();
            Accounts.Admin.CheckAdminSession session = new Accounts.Admin.CheckAdminSession();
            bool correctSession = session.sessionIsCorrect(username, roleId, token, currentPage, previousPage, currentTime, userIP);
            if (!correctSession)
                clearSession();
            lblAlerts.Text = "Alerts (" + countTotalAlerts() + ")";
        }
        protected string GetIPAddress()
        {
            System.Web.HttpContext context = System.Web.HttpContext.Current;
            string ipAddress = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (!string.IsNullOrEmpty(ipAddress))
            {
                string[] addresses = ipAddress.Split(',');
                if (addresses.Length != 0)
                {
                    return addresses[0];
                }
            }

            return context.Request.ServerVariables["REMOTE_ADDR"];
        }
        protected void clearSession()
        {

            Session.RemoveAll();
            Session.Clear();
            Session.Abandon();
            Response.Redirect("~/");
        }
        protected void addSession()
        {
            Session.Add("username", username);
            Session.Add("roleId", roleId);
            Session.Add("loginId", loginId);
            Session.Add("token", token);
        }
        protected void getSession()
        {
            username = (string)(Session["username"]);
            roleId = (string)(Session["roleId"]);
            loginId = (string)(Session["loginId"]);
            token = (string)(Session["token"]);
        }
        public int countTotalAlerts()
        {            
            connect.Open();
            SqlCommand cmd = connect.CreateCommand();
            //count users to be approved:
            cmd.CommandText = "select count(*) from registrations";
            int count = Convert.ToInt32(cmd.ExecuteScalar());
            //count topics to be approved:
            cmd.CommandText = "select count(*) from Topics where topic_isApproved = 0 and topic_isDenied = 0 and topic_isTerminated = 0";
            count = count + Convert.ToInt32(cmd.ExecuteScalar());
            //count messages to be approved by counting the messages that are not approved and have not been denied:
            cmd.CommandText = "select count(*) from [Entries] where entry_isApproved = 0 and entry_isDenied = 0 and entry_isDeleted = 0";
            int totalCount = Convert.ToInt32(cmd.ExecuteScalar());
            for (int i = 1; i <= totalCount; i++)
            {
                //Get the Message ID:
                cmd.CommandText = "select [entryId] from(SELECT rowNum = ROW_NUMBER() OVER(ORDER BY entryId ASC), * FROM [Entries] where entry_isApproved = 0 and entry_isDenied = 0 and entry_isDeleted = 0) as t where rowNum = '" + i + "'";
                string id = cmd.ExecuteScalar().ToString();
                //Get topic ID for the selected message:
                cmd.CommandText = "select [topicId] from [Entries] where entryId = '" + id + "' ";
                string topic = cmd.ExecuteScalar().ToString();
                cmd.CommandText = "select [topic_isDeleted] from [Topics] where topicId = '" + topic + "' ";
                int topic_isDeleted = Convert.ToInt32(cmd.ExecuteScalar());
                cmd.CommandText = "select [topic_isTerminated] from [Topics] where topicId = '" + topic + "' ";
                int topic_isTerminated = Convert.ToInt32(cmd.ExecuteScalar());
                if (topic_isDeleted == 0 && topic_isTerminated == 0)
                    count++;
            }
            //Count reports about messages:
            cmd.CommandText = "select count(*) from complains";
            count = count + Convert.ToInt32(cmd.ExecuteScalar());
            connect.Close();
            return count;
        }
    }
}