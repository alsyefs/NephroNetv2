using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NephroNet.Accounts.Admin
{
    public partial class ApproveComplains : System.Web.UI.Page
    {
        static string conn = "";
        SqlConnection connect = new SqlConnection(conn);
        string username, roleId, loginId, token;
        protected void Page_Load(object sender, EventArgs e)
        {
            initialPageAccess();
            int countNewComplains = getTotalNewComplains();
            if (countNewComplains > 0)
            {
                lblMessage.Visible = false;
                createTable(countNewComplains);
            }
            else if (countNewComplains == 0)
            {
                lblMessage.Visible = true;
            }
        }
        protected void initialPageAccess()
        {
            Configuration config = new Configuration();
            conn = config.getConnectionString();
            connect = new SqlConnection(conn);
            getSession();
            //Get from and to pages:
            string current_page = "", previous_page = "";
            if (HttpContext.Current.Request.Url.AbsoluteUri != null) current_page = HttpContext.Current.Request.Url.AbsoluteUri;
            if (Request.UrlReferrer != null) previous_page = Request.UrlReferrer.ToString();
            //Get current time:
            DateTime currentTime = DateTime.Now;
            //Get user's IP:
            string userIP = GetIPAddress();
            CheckAdminSession session = new CheckAdminSession();
            bool correctSession = session.sessionIsCorrect(username, roleId, token, current_page, previous_page, currentTime, userIP);
            if (!correctSession)
                clearSession();
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
        protected void grdComplains_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            grdComplains.PageIndex = e.NewPageIndex;
            grdComplains.DataBind();
            if (grdComplains.Rows.Count > 0)
            {
                //Hide the header called "ID":
                grdComplains.HeaderRow.Cells[1].Visible = false;
                //Hide IDs column and content which are located in column index 1:
                for (int i = 0; i < grdComplains.Rows.Count; i++)
                {
                    grdComplains.Rows[i].Cells[1].Visible = false;
                }
                rebindValues();
            }
        }
        protected void rebindValues()
        {
            connect.Open();
            SqlCommand cmd = connect.CreateCommand();
            string creator = "";
            for (int row = 0; row < grdComplains.Rows.Count; row++)
            {
                //Set the creator's link
                creator = grdComplains.Rows[row].Cells[3].Text;
                cmd.CommandText = "select userId from Users where (user_firstname +' '+ user_lastname) like '" + creator + "' ";
                string creatorId = cmd.ExecuteScalar().ToString();
                HyperLink creatorLink = new HyperLink();
                creatorLink.Text = creator + " ";
                creatorLink.NavigateUrl = "Profile.aspx?id=" + creatorId;
                grdComplains.Rows[row].Cells[3].Controls.Add(creatorLink);
            }
            connect.Close();
        }
        protected void createTable(int count)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("ID", typeof(string));
            dt.Columns.Add("Complain time", typeof(string));
            dt.Columns.Add("From user", typeof(string));
            string id = "", time = "", from_user = "";
            connect.Open();
            SqlCommand cmd = connect.CreateCommand();
            cmd.CommandType = CommandType.Text;
            for (int i = 1; i <= count; i++)
            {
                //Get the complain's ID:
                cmd.CommandText = "select [complainId] from(SELECT rowNum = ROW_NUMBER() OVER(ORDER BY complainId ASC), * FROM [Complains] ) as t where rowNum = '" + i + "'";
                id = cmd.ExecuteScalar().ToString();
                cmd.CommandText = "select complain_time from Complains where complainId = '"+id+"' ";
                time = cmd.ExecuteScalar().ToString();
                cmd.CommandText = "select complain_fromUser from Complains where complainId = '" + id + "' ";
                string reporter_userId = cmd.ExecuteScalar().ToString();
                //Get reporter's name:
                cmd.CommandText = "select (user_firstname + ' ' + user_lastname) from users where userId = '" + reporter_userId + "' ";
                from_user = cmd.ExecuteScalar().ToString();
                dt.Rows.Add(id, Layouts.getTimeFormat(time), from_user);
            }
            connect.Close();
            grdComplains.DataSource = dt;
            grdComplains.DataBind();
            if (count > 0)
            {
                //Hide the header called "ID":
                grdComplains.HeaderRow.Cells[1].Visible = false;
                //Hide IDs column and content which are located in column index 1:
                for (int i = 0; i < grdComplains.Rows.Count; i++)
                {
                    grdComplains.Rows[i].Cells[1].Visible = false;
                }
                rebindValues();
            }
        }
        protected int getTotalNewComplains()
        {
            connect.Open();
            SqlCommand cmd = connect.CreateCommand();
            //count the not-approved topics:
            cmd.CommandText = "select count(*) from Complains";
            int count = Convert.ToInt32(cmd.ExecuteScalar());
            connect.Close();
            return count;
        }
    }
}