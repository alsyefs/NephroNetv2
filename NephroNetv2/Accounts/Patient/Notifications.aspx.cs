using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NephroNet.Accounts.Patient
{
    public partial class Notifications : System.Web.UI.Page
    {
        static string conn = "";
        SqlConnection connect = new SqlConnection(conn);
        string username, roleId, loginId, token;
        protected void Page_Load(object sender, EventArgs e)
        {            
            initialPageAccess();
            showAllFields();
            countNewJoinTopicRequests();
        }
        protected void initialPageAccess()
        {
            Configuration config = new Configuration();
            conn = config.getConnectionString();
            connect = new SqlConnection(conn);
            getSession();
            CheckPatientSession session = new CheckPatientSession();
            bool correctSession = session.sessionIsCorrect(username, roleId, token);
            if (!correctSession)
                clearSession();
            
            if (session.countTotalAlerts() == 0)
            {
                lblError.Visible = true;
                lblError.Text = "There are no new alerts!";
            }
            else
            {
                lblError.Visible = false;
            }
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
        protected void showAllFields()
        {
            btnNewJoinTopicRequests.Visible = true;
        }
        protected void countNewJoinTopicRequests()
        {
            connect.Open();
            SqlCommand cmd = connect.CreateCommand();
            //Get this user's ID:
            cmd.CommandText = "select userId from Users where loginId = '"+loginId+"' ";
            string userId = cmd.ExecuteScalar().ToString();
            //Count the approved, not-deleted, not-denied, and not-terminated topics for this user:
            cmd.CommandText = "select count(*) from topics where topic_createdBy = '"+userId+"' and topic_isApproved = 1 and topic_isDeleted = 0 and topic_isDenied = 0 and topic_isTerminated = 0 ";
            int topicsForThisUser = Convert.ToInt32(cmd.ExecuteScalar());
            if (topicsForThisUser > 0)
            {
                int topicsToReview = 0;
                ////Get Topic IDs for this user:
                for (int i = 1; i <= topicsForThisUser; i++ )
                {
                    //Get a topic ID for the user logged in:
                    cmd.CommandText = "select [topicId] from(SELECT rowNum = ROW_NUMBER() OVER(ORDER BY topicId ASC), * FROM [Topics] where topic_createdBy = '" + userId + "' and topic_isApproved = 1 and topic_isDenied = 0 and topic_isTerminated = 0 and topic_isDeleted = 0) as t where rowNum = '" + i + "'";
                    string topicId = cmd.ExecuteScalar().ToString();
                    //Count total requests to join that topic where users have not been approved yet:
                    cmd.CommandText = "select count(*) from [UsersForTopics] where topicId = '" + topicId + "' and isApproved = 0 ";
                    int requestsPerTopic = Convert.ToInt32(cmd.ExecuteScalar());
                    //topicsToReview = topicsToReview + Convert.ToInt32(cmd.ExecuteScalar());
                    //Loop through the users requesting to join that specific topic:
                    for (int j = 1; j <= requestsPerTopic; j++)
                    {
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
                        //Now, check if the requester's account is still active:
                        cmd.CommandText = "select login_isActive from Logins where loginId = '" + requesterLoginId + "' ";
                        int active = Convert.ToInt32(cmd.ExecuteScalar());
                        if (active == 1)
                            topicsToReview++;
                    }
                }
                if (topicsToReview == 0)
                {
                    lblNewJoinTopicRequests.Text = "There are no new join-topic requests to review.";
                    btnNewJoinTopicRequests.Visible = false;
                    //lblNewJoinTopicRequests.Visible = false;
                }
                else if (topicsToReview == 1)
                {
                    lblNewJoinTopicRequests.Text = "There is one new join-topic request to review.";
                    btnNewJoinTopicRequests.Visible = true;
                    lblNewJoinTopicRequests.Visible = true;
                }
                else
                {
                    lblNewJoinTopicRequests.Text = "There are " + topicsToReview + " new join-topic requests to review.";
                    btnNewJoinTopicRequests.Visible = true;
                    lblNewJoinTopicRequests.Visible = true;
                }
            }
            else
            {
                lblNewJoinTopicRequests.Text = "There are no new join-topic requests to review.";
                btnNewJoinTopicRequests.Visible = false;
                lblNewJoinTopicRequests.Visible = false;
            }
            connect.Close();
        }
        protected void btnNewJoinTopicRequests_Click(object sender, EventArgs e)
        {
            addSession();
            Response.Redirect("ApproveJoinTopics");
        }
    }
}