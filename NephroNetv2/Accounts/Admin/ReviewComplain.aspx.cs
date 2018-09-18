using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NephroNet.Accounts.Admin
{
    public partial class ReviewComplain : System.Web.UI.Page
    {
        static string conn = "";
        SqlConnection connect = new SqlConnection(conn);
        string username, roleId, loginId, token;
        string complainId = "";
        string g_topic_title, g_email, g_complain_entryId, g_complain_reason;
        string g_complain_fromUser, g_complain_time, g_reporter_name, g_topicId, g_entry_text;
        string g_creatorId, g_creator_name;
        protected void Page_Load(object sender, EventArgs e)
        {
            initialPageAccess();
            complainId = Request.QueryString["id"];
            showTopicInformation();
        }
        protected void showTopicInformation()
        {
            connect.Open();
            SqlCommand cmd = connect.CreateCommand();
            //Check if the ID exists in the database:
            cmd.CommandText = "select count(*) from Complains where complainId = '" + complainId + "' ";
            int countComplain = Convert.ToInt32(cmd.ExecuteScalar());
            if (countComplain > 0)//if ID exists, countComplain = 1
            {
                cmd.CommandText = "select entryId from Complains where complainId = '" + complainId + "' ";
                string complain_entryId = cmd.ExecuteScalar().ToString();
                cmd.CommandText = "select complain_reason from Complains where complainId = '" + complainId + "' ";
                string complain_reason = cmd.ExecuteScalar().ToString();
                cmd.CommandText = "select complain_FromUser from Complains where complainId = '" + complainId + "' ";
                string complain_fromUser = cmd.ExecuteScalar().ToString();
                cmd.CommandText = "select complain_time from Complains where complainId = '" + complainId + "' ";
                string complain_time = cmd.ExecuteScalar().ToString();
                //Get reporter's fullname:
                cmd.CommandText = "select (user_firstname + ' ' + user_lastname) from users where userId = '" + complain_fromUser + "' ";
                string reporter_name = cmd.ExecuteScalar().ToString();
                //Get topics title as a link:
                cmd.CommandText = "select topicId from Entries where entryId = '"+complain_entryId+"' ";
                string topicId = cmd.ExecuteScalar().ToString();
                cmd.CommandText = "select topic_title from Topics where topicId = '" + topicId + "' ";
                string temp_topic_title = cmd.ExecuteScalar().ToString();
                string topic_title = "<a href=\"ViewTopic.aspx?id="+topicId+"\">"+ temp_topic_title + "</a>";
                //Get the original message text:
                cmd.CommandText = "select entry_text from Entries where entryId = '" + complain_entryId + "' ";
                string entry_text = cmd.ExecuteScalar().ToString();
                //Get the creator of the message:
                cmd.CommandText = "select userId from Entries where entryId = '"+complain_entryId+"' ";
                string creatorId = cmd.ExecuteScalar().ToString();
                //Get the creator's name:
                cmd.CommandText = "select (user_firstname + ' ' + user_lastname) from Users where userId = '" + creatorId + "' ";
                string creator_name = cmd.ExecuteScalar().ToString();
                //Get the creator's name:
                cmd.CommandText = "select user_email from Users where userId = '" + creatorId + "' ";
                string creator_email = cmd.ExecuteScalar().ToString();
                lblComplainInformation.Text =
                    "<table>" +
                    "<tr><td>Reporter: </td><td>" + reporter_name + "</td></tr>" +
                    "<tr><td>About message: </td><td>" + entry_text + "</td></tr>" +
                    "<tr><td>In the topic: </td><td>" + topic_title + "</td></tr>" +
                    "<tr><td>Complain time: </td><td>" + Layouts.getTimeFormat(complain_time) + "</td></tr>" +
                    "<tr><td>Reason: </td><td> <div style=\"background: #DCCDCA; padding-left:5px; padding-right:5px; \"> " + complain_reason +
                    "</table>";
                lblComplainInformation.Visible = true;
                //Copy values to globals:
                g_complain_entryId = complain_entryId;
                g_complain_reason = complain_reason;
                g_complain_fromUser = complain_fromUser;
                g_complain_time = complain_time;
                g_reporter_name = reporter_name;
                g_topicId = topicId;
                g_topic_title = temp_topic_title;
                g_entry_text = entry_text;
                
                g_creatorId = creatorId;
                g_creator_name = creator_name;
                g_email = creator_email;
            }
            else
            {
                addSession();
                Response.Redirect("ApproveComplains");
            }
            connect.Close();
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
        protected void btnDeny_Click(object sender, EventArgs e)
        {
            //Hide the success message:
            lblMessage.Visible = false;
            connect.Open();
            SqlCommand cmd = connect.CreateCommand();
            cmd.CommandText = "delete from Complains where complainId = '"+ complainId + "' ";
            cmd.ExecuteScalar();
            connect.Close();
            //Display a success message:
            lblMessage.Visible = true;
            lblMessage.Text = "You have successfully disagreed with the selected complain!";
            //Hide "Approve" and "Deny" buttons:
            hideApproveDeny();
        }

        protected void btnApprove_Click(object sender, EventArgs e)
        {
            //Hide the success message:
            lblMessage.Visible = false;
            //Set topic_isApproved = 1, topic_isDenied = 0: (1 in bit = true)
            connect.Open();
            SqlCommand cmd = connect.CreateCommand();
            cmd.CommandText = "update Entries set entry_isDeleted = 1 where entryId = '" + g_complain_entryId + "' ";
            cmd.ExecuteScalar();
            cmd.CommandText = "delete from Complains where complainId = '"+complainId+"' ";
            cmd.ExecuteScalar();
            connect.Close();
            //Create an email message to be sent:
            string emailMessage = "Hello " + g_creator_name + ",\n\n" +
                "This email is to inform you that your message in the topic with the title (" + g_topic_title + ") has been removed from NephroNet." +
                "Your message text was:\n ("+g_entry_text+") \n\n" +
                "Best regards,\nNephroNet Support\nNephroNet2018@gmail.com";
            //Send an email notification to the user using the stored email:
            Email emailClass = new Email();
            emailClass.sendEmail(g_email, emailMessage);
            //Display a success message:
            lblMessage.Visible = true;
            lblMessage.Text = "The selected message has been successfully removed and a notification email has been sent to the creator!";
            //Hide "Approve" and "Deny" buttons:
            hideApproveDeny();
        }
        protected void hideApproveDeny()
        {
            btnApprove.Visible = false;
            btnDeny.Visible = false;
        }
        protected void btnCancel_Click(object sender, EventArgs e)
        {
            addSession();
            Response.Redirect("ApproveComplains");
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
    }
}