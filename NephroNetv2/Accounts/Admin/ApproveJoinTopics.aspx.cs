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
    public partial class ApproveJoinTopics : System.Web.UI.Page
    {
        static string conn = "";
        SqlConnection connect = new SqlConnection(conn);
        string username, roleId, loginId, token;
        protected void Page_Load(object sender, EventArgs e)
        {
            initialPageAccess();
            getTotalTopicsForUser();
            //int countTopicsForUser = getTotalTopicsForUser();
            //if (countTopicsForUser > 0)
            //{
            //    lblMessage.Visible = false;
            //    //createTable(countTopicsForUser);
            //    createTable();
            //}
            //else if (countTopicsForUser == 0)
            //{
            //    lblMessage.Visible = true;
            //}
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
        protected void grdTopics_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            grdTopics.PageIndex = e.NewPageIndex;
            grdTopics.DataBind();
            if (grdTopics.Rows.Count > 0)
            {
                //Hide the header called "ID":
                grdTopics.HeaderRow.Cells[1].Visible = false;
                //Hide IDs column and content which are located in column index 1:
                for (int i = 0; i < grdTopics.Rows.Count; i++)
                {
                    grdTopics.Rows[i].Cells[1].Visible = false;
                }
                rebindValues();
            }
        }
        protected void getTotalTopicsForUser()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("ID", typeof(string));
            dt.Columns.Add("Title", typeof(string));
            dt.Columns.Add("Requester", typeof(string));
            string id = "", title = "", requester = "";
            connect.Open();
            SqlCommand cmd = connect.CreateCommand();
            int count = 0;
            //Get this user's ID:
            cmd.CommandText = "select userId from Users where loginId = '" + loginId + "' ";
            string userId = cmd.ExecuteScalar().ToString();
            //Count the approved, not-deleted, not-denied, and not-terminated topics for this user:
            cmd.CommandText = "select count(*) from topics where topic_createdBy = '" + userId + "' and topic_isApproved = 1 and topic_isDeleted = 0 and topic_isDenied = 0 and topic_isTerminated = 0 ";
            int topicsForThisUser = Convert.ToInt32(cmd.ExecuteScalar());
            int requestsPerTopic = 0;
            if (topicsForThisUser > 0)
            {
                int topicsToReview = 0;
                //Get Topic IDs for this user:
                for (int i = 1; i <= topicsForThisUser; i++)
                {
                    //Get a topic ID for the user logged in:
                    cmd.CommandText = "select [topicId] from(SELECT rowNum = ROW_NUMBER() OVER(ORDER BY topicId ASC), * FROM [Topics] where topic_createdBy = '" + userId + "' and topic_isApproved = 1 and topic_isDenied = 0 and topic_isTerminated = 0 and topic_isDeleted = 0) as t where rowNum = '" + i + "'";
                    string topicId = cmd.ExecuteScalar().ToString();
                    //Count total requests to join that topic where users have not been approved yet:
                    cmd.CommandText = "select count(*) from [UsersForTopics] where topicId = '" + topicId + "' and isApproved = 0 ";
                    requestsPerTopic = Convert.ToInt32(cmd.ExecuteScalar());
                    //topicsToReview = topicsToReview + Convert.ToInt32(cmd.ExecuteScalar());
                    //Loop through the users requesting to join that specific topic:
                    for (int j = 1; j <= requestsPerTopic; j++)
                    {
                        //Get the requester ID:
                        cmd.CommandText = "select [usersForTopicsId] from(SELECT rowNum = ROW_NUMBER() OVER(ORDER BY usersForTopicsId ASC), * FROM [UsersForTopics] where isApproved = 0 and topicId = '" + topicId + "') as t where rowNum = '" + j + "'";
                        id = cmd.ExecuteScalar().ToString();
                        //Get title:
                        cmd.CommandText = "select [topic_title] from(SELECT rowNum = ROW_NUMBER() OVER(ORDER BY topicId ASC), * FROM [Topics] where topic_createdBy = '" + userId + "' and topic_isApproved = 1 and topic_isDenied = 0 and topic_isTerminated = 0 and topic_isDeleted = 0) as t where rowNum = '" + i + "'";
                        title = cmd.ExecuteScalar().ToString();
                        //Get a request ID for that topic:
                        cmd.CommandText = "select [UsersForTopicsId] from(SELECT rowNum = ROW_NUMBER() OVER(ORDER BY UsersForTopicsId ASC), * FROM [UsersForTopics] where topicId = '" + topicId + "' " +
                        "and isApproved = 0) as t where rowNum = '" + j + "'";
                        string requestId = cmd.ExecuteScalar().ToString();
                        //Get a requester ID, which is really just a user ID:
                        cmd.CommandText = "select [userId] from(SELECT rowNum = ROW_NUMBER() OVER(ORDER BY UsersForTopicsId ASC), * FROM [UsersForTopics] where topicId = '" + topicId + "' " +
                        "and isApproved = 0) as t where rowNum = '" + j + "'";
                        string requesterId = cmd.ExecuteScalar().ToString();
                        //Get the login ID for the requester:
                        cmd.CommandText = "select loginId from users where userId = '" + requesterId + "' ";
                        string requesterLoginId = cmd.ExecuteScalar().ToString();
                        //Get requester's name:
                        cmd.CommandText = "select user_firstname from users where userId = '" + requesterId + "' ";
                        requester = cmd.ExecuteScalar().ToString();
                        cmd.CommandText = "select user_lastname from users where userId = '" + requesterId + "' ";
                        requester = requester + " " + cmd.ExecuteScalar().ToString();

                        //Now, check if the requester's account is still active:
                        cmd.CommandText = "select login_isActive from Logins where loginId = '" + requesterLoginId + "' ";
                        int active = Convert.ToInt32(cmd.ExecuteScalar());
                        if (active == 1)
                        {
                            dt.Rows.Add(id, title, requester);
                            lblMessage.Visible = false;
                        }
                    }
                }
                count = topicsToReview;
            }
            else
                lblMessage.Visible = true;
            connect.Close();
            grdTopics.DataSource = dt;
            grdTopics.DataBind();
            if (requestsPerTopic > 0)
            {
                rebindValues();
                //Hide the header called "ID":
                grdTopics.HeaderRow.Cells[1].Visible = false;
                ////Hide IDs column and content which are located in column index 1:
                for (int i = 0; i < grdTopics.Rows.Count; i++)
                {
                    grdTopics.Rows[i].Cells[1].Visible = false;
                }
            }
        }
        protected void rebindValues()
        {
            connect.Open();
            SqlCommand cmd = connect.CreateCommand();
            string creator = "";
            for (int row = 0; row < grdTopics.Rows.Count; row++)
            {
                //Set the creator's link
                creator = grdTopics.Rows[row].Cells[3].Text;
                cmd.CommandText = "select userId from Users where (user_firstname +' '+ user_lastname) like '" + creator + "' ";
                string creatorId = cmd.ExecuteScalar().ToString();
                HyperLink creatorLink = new HyperLink();
                creatorLink.Text = creator + " ";
                creatorLink.NavigateUrl = "Profile.aspx?id=" + creatorId;
                grdTopics.Rows[row].Cells[3].Controls.Add(creatorLink);
            }
            connect.Close();
        }
        protected void createTable(int count)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("ID", typeof(string));
            dt.Columns.Add("Title", typeof(string));
            dt.Columns.Add("Requester", typeof(string));
            string id = "", title = "", requester = "";
            connect.Open();
            SqlCommand cmd = connect.CreateCommand();
            //Code start

            //Code end
            //Test start:
            //Get this user's ID:
            cmd.CommandText = "select userId from Users where loginId = '" + loginId + "' ";
            string userId = cmd.ExecuteScalar().ToString();
            int topicsToReview = 0;
            for (int i = 1; i <= count; i++)
            {                
                //Get a Topic ID for this user:
                cmd.CommandText = "select [topicId] from(SELECT rowNum = ROW_NUMBER() OVER(ORDER BY topicId ASC), * FROM [Topics] where topic_createdBy = '" + userId + "' and topic_isApproved = 1 and topic_isDenied = 0 and topic_isTerminated = 0 and topic_isDeleted = 0) as t where rowNum = '" + i + "'";
                string topicId = cmd.ExecuteScalar().ToString();                
                //Get title:
                cmd.CommandText = "select [topic_title] from(SELECT rowNum = ROW_NUMBER() OVER(ORDER BY topicId ASC), * FROM [Topics] where topic_createdBy = '" + userId + "' and topic_isApproved = 1 and topic_isDenied = 0 and topic_isTerminated = 0 and topic_isDeleted = 0) as t where rowNum = '" + i + "'";
                title = cmd.ExecuteScalar().ToString();
                //Get total requests for the selected topic:
                cmd.CommandText = "select count(*) from [UsersForTopics] where topicId = '" + topicId + "' and isApproved = 0 ";
                int requestsforTopic = Convert.ToInt32(cmd.ExecuteScalar());
                //add the total requests for the selected topic to all requests:
                topicsToReview = topicsToReview + requestsforTopic;
                //Loop through requests for the selected topic:
                for(int j = 1; j <= requestsforTopic; j++)
                {
                    //Get the requester ID:
                    cmd.CommandText = "select [usersForTopicsId] from(SELECT rowNum = ROW_NUMBER() OVER(ORDER BY usersForTopicsId ASC), * FROM [UsersForTopics] where isApproved = 0 and topicId = '" + topicId + "') as t where rowNum = '" + j + "'";
                    id = cmd.ExecuteScalar().ToString();
                    //Get the requester ID:
                    cmd.CommandText = "select [userId] from(SELECT rowNum = ROW_NUMBER() OVER(ORDER BY usersForTopicsId ASC), * FROM [UsersForTopics] where isApproved = 0 and topicId = '" + topicId + "') as t where rowNum = '" + j + "'";
                    string requesterId = cmd.ExecuteScalar().ToString();
                    //Get requester's name:
                    cmd.CommandText = "select user_firstname from users where userId = '" + requesterId + "' ";
                    requester = cmd.ExecuteScalar().ToString();
                    cmd.CommandText = "select user_lastname from users where userId = '" + requesterId + "' ";
                    requester = requester + " " + cmd.ExecuteScalar().ToString();
                    dt.Rows.Add(id, title, requester);
                }                
            }
            //Test end
            connect.Close();
            grdTopics.DataSource = dt;
            grdTopics.DataBind();
            if (topicsToReview > 0)
            {
                //Hide the header called "ID":
                grdTopics.HeaderRow.Cells[1].Visible = false;
                //Hide IDs column and content which are located in column index 1:
                for (int i = 0; i < grdTopics.Rows.Count; i++)
                {
                    grdTopics.Rows[i].Cells[1].Visible = false;
                }
            }
        }
        //protected int getTotalTopicsForUser()
        //{
        //    connect.Open();
        //    SqlCommand cmd = connect.CreateCommand();
        //    int count = 0;
        //    //Get this user's ID:
        //    cmd.CommandText = "select userId from Users where loginId = '" + loginId + "' ";
        //    string userId = cmd.ExecuteScalar().ToString();
        //    //Count the approved, not-deleted, not-denied, and not-terminated topics for this user:
        //    cmd.CommandText = "select count(*) from topics where topic_createdBy = '" + userId + "' and topic_isApproved = 1 and topic_isDeleted = 0 and topic_isDenied = 0 and topic_isTerminated = 0 ";
        //    int topicsForThisUser = Convert.ToInt32(cmd.ExecuteScalar());
        //    if (topicsForThisUser > 0)
        //    {
        //        int topicsToReview = 0;
        //        //Get Topic IDs for this user:
        //        for (int i = 1; i <= topicsForThisUser; i++)
        //        {
        //            //Get a topic ID for the user logged in:
        //            cmd.CommandText = "select [topicId] from(SELECT rowNum = ROW_NUMBER() OVER(ORDER BY topicId ASC), * FROM [Topics] where topic_createdBy = '" + userId + "' and topic_isApproved = 1 and topic_isDenied = 0 and topic_isTerminated = 0 and topic_isDeleted = 0) as t where rowNum = '" + i + "'";
        //            string topicId = cmd.ExecuteScalar().ToString();
        //            //Count total requests to join that topic where users have not been approved yet:
        //            cmd.CommandText = "select count(*) from [UsersForTopics] where topicId = '" + topicId + "' and isApproved = 0 ";
        //            int requestsPerTopic = Convert.ToInt32(cmd.ExecuteScalar());
        //            //topicsToReview = topicsToReview + Convert.ToInt32(cmd.ExecuteScalar());
        //            //Loop through the users requesting to join that specific topic:
        //            for (int j = 1; j <= requestsPerTopic; j++)
        //            {
        //                //Get a request ID for that topic:
        //                cmd.CommandText = "select [UsersForTopicsId] from(SELECT rowNum = ROW_NUMBER() OVER(ORDER BY UsersForTopicsId ASC), * FROM [UsersForTopics] where topicId = '" + topicId + "' " +
        //                "and isApproved = 0) as t where rowNum = '" + j + "'";
        //                string requestId = cmd.ExecuteScalar().ToString();
        //                //Get a requester ID, which is really just a user ID:
        //                cmd.CommandText = "select [userId] from(SELECT rowNum = ROW_NUMBER() OVER(ORDER BY UsersForTopicsId ASC), * FROM [UsersForTopics] where topicId = '" + topicId + "' " +
        //                "and isApproved = 0) as t where rowNum = '" + j + "'";
        //                string requesterId = cmd.ExecuteScalar().ToString();
        //                //Get the login ID for the requester:
        //                cmd.CommandText = "select loginId from users where userId = '" + requesterId + "' ";
        //                string requesterLoginId = cmd.ExecuteScalar().ToString();
        //                //Now, check if the requester's account is still active:
        //                cmd.CommandText = "select login_isActive from Logins where loginId = '" + requesterLoginId + "' ";
        //                int active = Convert.ToInt32(cmd.ExecuteScalar());
        //                if (active == 1)
        //                    topicsToReview++;
        //            }
        //        }
        //        count = topicsToReview;
        //    }
        //        connect.Close();
        //    return count;
        //}
    }
}