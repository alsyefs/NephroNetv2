using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NephroNet.Accounts.Physician
{
    public partial class RemoveTopic : System.Web.UI.Page
    {
        static string conn = "";
        SqlConnection connect = new SqlConnection(conn);
        string username, roleId, loginId, token;
        string topicId = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            initialPageAccess();
            topicId = Request.QueryString["id"];
            bool topicIdExists = isTopicCorrect();
            if (!topicIdExists)
                closePage();
        }
        protected bool isTopicCorrect()
        {
            bool correct = true;
            CheckErrors errors = new CheckErrors();
            //check if id contains a special character:
            if (!errors.isDigit(topicId))
                correct = false;
            //check if id contains an id that does not exist in DB:
            else if (errors.ContainsSpecialChars(topicId))
                correct = false;
            connect.Open();
            SqlCommand cmd = connect.CreateCommand();
            //Count the existance of the topic:
            cmd.CommandText = "select count(*) from Topics where topicId = '" + topicId + "' ";
            int count = Convert.ToInt32(cmd.ExecuteScalar());
            if (count > 0)//if count > 0, then the topic ID exists in DB.
            {
                cmd.CommandText = "select topic_createdBy from Topics where topicId = '" + topicId + "' ";
                string creatorId = cmd.ExecuteScalar().ToString();
                cmd.CommandText = "select userId from Users where loginId = '" + loginId + "' ";
                string userId = cmd.ExecuteScalar().ToString();
                cmd.CommandText = "select topic_isDeleted from Topics where topicId = '" + topicId + "' ";
                int isDeleted = Convert.ToInt32(cmd.ExecuteScalar());
                
                //check if id belongs to a different user:
                if (!userId.Equals(creatorId))
                    correct = false;
                else if (isDeleted == 1)
                    correct = false;
            }
            else
                correct = false; // means that the topic ID does not exists in DB.
            connect.Close();
            return correct;
        }        
        protected void initialPageAccess()
        {
            Configuration config = new Configuration();
            conn = config.getConnectionString();
            connect = new SqlConnection(conn);
            getSession();
            CheckPhysicianSession session = new CheckPhysicianSession();
            bool correctSession = session.sessionIsCorrect(username, roleId, token);
            if (!correctSession)
                clearSession();
            //lblAlerts.Text = "(" + session.countTotalAlerts() + ")";
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
        protected void closePage()
        {
            Response.Write("<script>window.close();</script>");
        }
        protected void btnCancel_Click(object sender, EventArgs e)
        {
            closePage();
        }
        protected void btnRemove_Click(object sender, EventArgs e)
        {
            bool correct = isTopicCorrect();
            if (!correct)
            {
                closePage();
                lblMessageInformation.Text = "You are not authorized to remove the selected message!";
            }
            else
            {
                connect.Open();
                SqlCommand cmd = connect.CreateCommand();
                //update the DB and set isDeleted = true:
                cmd.CommandText = "update Topics set topic_isDeleted = 1 where topicId = '" + topicId + "' ";
                cmd.ExecuteScalar();
                connect.Close();
                //Email the topic creator about the topic being deleted:
                sendEmailNotification();
                closePage();
            }
        }
        protected void sendEmailNotification()
        {
            connect.Open();
            SqlCommand cmd = connect.CreateCommand();
            cmd.CommandText = "select topic_createdBy from Topics where topicId = '" + topicId + "' ";
            string creatorId = cmd.ExecuteScalar().ToString();
            cmd.CommandText = "select topic_title from Topics where topicId = '" + topicId + "' ";
            string topic_title = cmd.ExecuteScalar().ToString();
            cmd.CommandText = "select user_firstname from Users where userId = '" + creatorId + "' ";
            string name = cmd.ExecuteScalar().ToString();
            cmd.CommandText = "select user_lastname from Users where userId = '" + creatorId + "' ";
            name = name + " " + cmd.ExecuteScalar().ToString();
            cmd.CommandText = "select user_email from Users where userId = '" + creatorId + "' ";
            string emailTo = cmd.ExecuteScalar().ToString();
            connect.Close();
            string emailBody = "Hello " + name + ",\n\n" +
                "This email is to inform you that your topic with the title (" + topic_title + ") has been deleted. If you think this happened by mistake, or you did not perform this action, plaese contact the support.\n\n" +
                "Best regards,\nNephroNet Support\nNephroNet2018@gmail.com";
            Email email = new Email();
            email.sendEmail(emailTo, emailBody);
        }
    }
}