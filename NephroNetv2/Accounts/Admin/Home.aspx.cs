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
    public partial class Home : System.Web.UI.Page
    {
        static string conn = "";
        SqlConnection connect = new SqlConnection(conn);
        string username, roleId, loginId, token;
        protected void Page_Load(object sender, EventArgs e)
        {
            initialPageAccess();
            int countNewTopics = getTotalApprovedTopics();
            if (countNewTopics > 0)
            {
                lblMessage.Visible = false;
                createTable(countNewTopics);
            }
            else if (countNewTopics == 0)
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
            CheckAdminSession session = new CheckAdminSession();
            bool correctSession = session.sessionIsCorrect(username, roleId, token);
            if (!correctSession)
                clearSession();
            //lblAlerts.Text = "(" + session.countTotalAlerts() + ")";
            //alerts.inner ["class"] = "";
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
        protected void grdTopics_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            grdTopics.PageIndex = e.NewPageIndex;
            grdTopics.DataBind();
            //Hide the header called "ID":
            grdTopics.HeaderRow.Cells[1].Visible = false;
            //Hide IDs column and content which are located in column index 1:
            for (int i = 0; i < grdTopics.Rows.Count; i++)
            {
                grdTopics.Rows[i].Cells[1].Visible = false;
            }
            reBindValues();
        }
        protected void reBindValues()
        {
            connect.Open();
            SqlCommand cmd = connect.CreateCommand();
            string id = "", creator = "";
            for (int row = 0; row < grdTopics.Rows.Count; row++)
            {
                id = grdTopics.Rows[row].Cells[1].Text;
                //Get creator's ID:
                cmd.CommandText = "select [topic_createdBy] FROM [Topics] where topicId = " + id + " ";
                string creatorId = cmd.ExecuteScalar().ToString();
                //Get creator's name:
                cmd.CommandText = "select user_firstname from users where userId = '" + creatorId + "' ";
                creator = cmd.ExecuteScalar().ToString();
                cmd.CommandText = "select user_lastname from users where userId = '" + creatorId + "' ";
                creator = creator + " " + cmd.ExecuteScalar().ToString();
                HyperLink creatorLink = new HyperLink();
                creatorLink.Text = creator + " ";
                creatorLink.NavigateUrl = "Profile.aspx?id=" + creatorId;
                grdTopics.Rows[row].Cells[5].Controls.Add(creatorLink);
            }
            connect.Close();
        }
        protected void createTable(int count)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("ID", typeof(string));
            dt.Columns.Add("Time", typeof(string));
            dt.Columns.Add("Title", typeof(string));
            dt.Columns.Add("Type", typeof(string));
            dt.Columns.Add("Creator", typeof(string));
            string id = "", title = "", type = "", creator = "", time = "";
            connect.Open();
            SqlCommand cmd = connect.CreateCommand();
            for (int i = 1; i <= count; i++)
            {
                //Get the topic ID:
                cmd.CommandText = "select [topicId] from(SELECT rowNum = ROW_NUMBER() OVER(ORDER BY topicId ASC), * FROM [Topics] where topic_isApproved = 1 and topic_isDenied = 0 and topic_isDeleted = 0) as t where rowNum = '" + i + "'";
                id = cmd.ExecuteScalar().ToString();
                //Get type:
                cmd.CommandText = "select [topic_time] from(SELECT rowNum = ROW_NUMBER() OVER(ORDER BY topicId ASC), * FROM [Topics] where topic_isApproved = 1 and topic_isDenied = 0 and topic_isDeleted = 0) as t where rowNum = '" + i + "'";
                time = cmd.ExecuteScalar().ToString();
                //Get title:
                cmd.CommandText = "select [topic_title] from(SELECT rowNum = ROW_NUMBER() OVER(ORDER BY topicId ASC), * FROM [Topics] where topic_isApproved = 1 and topic_isDenied = 0 and topic_isDeleted = 0) as t where rowNum = '" + i + "'";
                title = cmd.ExecuteScalar().ToString();
                //Get type:
                cmd.CommandText = "select [topic_type] from(SELECT rowNum = ROW_NUMBER() OVER(ORDER BY topicId ASC), * FROM [Topics] where topic_isApproved = 1 and topic_isDenied = 0 and topic_isDeleted = 0) as t where rowNum = '" + i + "'";
                type = cmd.ExecuteScalar().ToString();
                //Get creator's ID:
                cmd.CommandText = "select [topic_createdBy] from(SELECT rowNum = ROW_NUMBER() OVER(ORDER BY topicId ASC), * FROM [Topics] where topic_isApproved = 1 and topic_isDenied = 0 and topic_isDeleted = 0) as t where rowNum = '" + i + "'";
                string creatorId = cmd.ExecuteScalar().ToString();
                //Get creator's name:
                cmd.CommandText = "select user_firstname from users where userId = '" + creatorId + "' ";
                creator = cmd.ExecuteScalar().ToString();
                cmd.CommandText = "select user_lastname from users where userId = '" + creatorId + "' ";
                creator = creator + " " + cmd.ExecuteScalar().ToString();
                dt.Rows.Add(id, Layouts.getTimeFormat(time), title, type, creator);
            }
            grdTopics.DataSource = dt;
            grdTopics.DataBind();
            //grdTopics.AutoGenerateColumns = true;
            //Hide the header called "ID":
            grdTopics.HeaderRow.Cells[1].Visible = false;
            //Hide IDs column and content which are located in column index 1:
            for (int i = 0; i < grdTopics.Rows.Count; i++)
            {
                grdTopics.Rows[i].Cells[1].Visible = false;
            }
            for (int row = 0; row < grdTopics.Rows.Count; row++)
            {
                id = grdTopics.Rows[row].Cells[1].Text;
                //Get creator's ID:
                cmd.CommandText = "select [topic_createdBy] FROM [Topics] where topicId = " + id + " ";
                string creatorId = cmd.ExecuteScalar().ToString();
                //Get creator's name:
                cmd.CommandText = "select user_firstname from users where userId = '" + creatorId + "' ";
                creator = cmd.ExecuteScalar().ToString();
                cmd.CommandText = "select user_lastname from users where userId = '" + creatorId + "' ";
                creator = creator + " " + cmd.ExecuteScalar().ToString();
                HyperLink creatorLink = new HyperLink();
                creatorLink.Text = creator + " ";
                creatorLink.NavigateUrl = "Profile.aspx?id=" + creatorId;
                grdTopics.Rows[row].Cells[5].Controls.Add(creatorLink);
            }
            connect.Close();
        }
        protected int getTotalApprovedTopics()
        {
            connect.Open();
            SqlCommand cmd = connect.CreateCommand();
            //count the approved topics:
            cmd.CommandText = "select count(*) from Topics where topic_isApproved = 1 and topic_isDenied = 0 and topic_isDeleted = 0";
            int count = Convert.ToInt32(cmd.ExecuteScalar());
            connect.Close();
            return count;
        }
    }
}