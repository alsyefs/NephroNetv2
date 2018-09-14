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
    public partial class ApproveMessages : System.Web.UI.Page
    {
        static string conn = "";
        SqlConnection connect = new SqlConnection(conn);
        string username, roleId, loginId, token;
        protected void Page_Load(object sender, EventArgs e)
        {
            initialPageAccess();
            int countNewMessages = getTotalNewMessages();
            if (countNewMessages > 0)
            {
                lblMessage.Visible = false;
                createTable(countNewMessages);
            }
            else if (countNewMessages == 0)
            {
                lblMessage.Visible = true;                
            }
        }
        protected void createTable(int count)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("ID", typeof(string));
            dt.Columns.Add("Topic ID", typeof(string));
            dt.Columns.Add("Creator", typeof(string));
            dt.Columns.Add("Topic Title", typeof(string));
            string id = "", creator = "", topic = "", title="";
            connect.Open();
            SqlCommand cmd = connect.CreateCommand();
            cmd.CommandType = CommandType.Text;
            for (int i = 1; i <= count; i++)
            {
                //Get the Message ID:
                cmd.CommandText = "select [entryId] from(SELECT rowNum = ROW_NUMBER() OVER(ORDER BY entryId ASC), * FROM [Entries] where entry_isApproved = 0 and entry_isDenied = 0 and entry_isDeleted = 0) as t where rowNum = '" + i + "'";
                id = cmd.ExecuteScalar().ToString();
                //Get topic ID for the selected message:
                cmd.CommandText = "select [topicId] from(SELECT rowNum = ROW_NUMBER() OVER(ORDER BY entryId ASC), * FROM [Entries] where entry_isApproved = 0 and entry_isDenied = 0 and entry_isDeleted = 0) as t where rowNum = '" + i + "'";
                topic = cmd.ExecuteScalar().ToString();
                //Get topic title:
                cmd.CommandText = "select [topic_title] from [Topics] where topicId = '"+topic+"' ";
                title = cmd.ExecuteScalar().ToString();
                cmd.CommandText = "select [topic_isDeleted] from [Topics] where topicId = '" + topic + "' ";
                int topic_isDeleted = Convert.ToInt32(cmd.ExecuteScalar());
                cmd.CommandText = "select [topic_isTerminated] from [Topics] where topicId = '" + topic + "' ";
                int topic_isTerminated = Convert.ToInt32(cmd.ExecuteScalar());
                //Get creator's ID:
                cmd.CommandText = "select [userId] from(SELECT rowNum = ROW_NUMBER() OVER(ORDER BY entryId ASC), * FROM [Entries] where entry_isApproved = 0 and entry_isDenied = 0 and entry_isDeleted = 0) as t where rowNum = '" + i + "'";
                string creatorId = cmd.ExecuteScalar().ToString();
                //Get creator's name:
                cmd.CommandText = "select user_firstname from users where userId = '" + creatorId + "' ";
                creator = cmd.ExecuteScalar().ToString();
                cmd.CommandText = "select user_lastname from users where userId = '" + creatorId + "' ";
                creator = creator + " " + cmd.ExecuteScalar().ToString();
                if(topic_isDeleted == 0 && topic_isTerminated == 0)
                    dt.Rows.Add(id, topic, creator, title);
            }
            connect.Close();
            grdMessages.DataSource = dt;
            grdMessages.DataBind();
            if (grdMessages.Rows.Count > 0)
            {
                //Hide the header called "ID":
                grdMessages.HeaderRow.Cells[1].Visible = false;
                grdMessages.HeaderRow.Cells[2].Visible = false;
                //Hide IDs column and content which are located in column index 1:
                for (int i = 0; i < grdMessages.Rows.Count; i++)
                {
                    grdMessages.Rows[i].Cells[1].Visible = false;
                    grdMessages.Rows[i].Cells[2].Visible = false;
                }
                rebindValues();
            }
        }
        protected int getTotalNewMessages()
        {
            connect.Open();
            SqlCommand cmd = connect.CreateCommand();
            //count the not-approved messages:
            cmd.CommandText = "select count(*) from Entries where entry_isApproved = 0 and entry_isDenied = 0 and entry_isDeleted = 0";
            int count = Convert.ToInt32(cmd.ExecuteScalar());
            connect.Close();
            return count;
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
        protected void rebindValues()
        {
            connect.Open();
            SqlCommand cmd = connect.CreateCommand();
            string creator = "";
            for (int row = 0; row < grdMessages.Rows.Count; row++)
            {
                //Set the creator's link
                creator = grdMessages.Rows[row].Cells[3].Text;
                cmd.CommandText = "select userId from Users where (user_firstname +' '+ user_lastname) like '" + creator + "' ";
                string creatorId = cmd.ExecuteScalar().ToString();
                HyperLink creatorLink = new HyperLink();
                creatorLink.Text = creator + " ";
                creatorLink.NavigateUrl = "Profile.aspx?id=" + creatorId;
                grdMessages.Rows[row].Cells[3].Controls.Add(creatorLink);
            }
            connect.Close();
        }
        protected void grdMessages_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            grdMessages.PageIndex = e.NewPageIndex;
            grdMessages.DataBind();
            if (grdMessages.Rows.Count > 0)
            {
                //Hide the header called "ID":
                grdMessages.HeaderRow.Cells[1].Visible = false;
                grdMessages.HeaderRow.Cells[2].Visible = false;
                //Hide IDs column and content which are located in column index 1:
                for (int i = 0; i < grdMessages.Rows.Count; i++)
                {
                    grdMessages.Rows[i].Cells[1].Visible = false;
                    grdMessages.Rows[i].Cells[2].Visible = false;
                }
                rebindValues();
            }
        }
    }
}