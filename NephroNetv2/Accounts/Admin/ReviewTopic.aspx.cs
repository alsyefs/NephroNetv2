using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NephroNet.Accounts.Admin
{
    public partial class ReviewTopic : System.Web.UI.Page
    {
        static string conn = "";
        SqlConnection connect = new SqlConnection(conn);
        string username, roleId, loginId, token;
        string topicId = "";
        //Globals for "Topics" table:
        int g_topic_isApproved, g_topic_isDenied;
        string g_creator, g_topic_title, g_email;
        protected void Page_Load(object sender, EventArgs e)
        {
            initialPageAccess();
            topicId = Request.QueryString["id"];
            showTopicInformation();
        }
        protected void showTopicInformation()
        {
            connect.Open();
            SqlCommand cmd = connect.CreateCommand();
            //Check if the ID exists in the database:
            cmd.CommandText = "select count(*) from topics where topicId = '" + topicId + "' ";
            int countTopic = Convert.ToInt32(cmd.ExecuteScalar());
            if (countTopic > 0)//if ID exists, countTopic = 1
            {
                //Get topic_createdBy:
                cmd.CommandText = "select topic_createdBy from [Topics] where [topicId] = '" + topicId + "' ";
                string topic_createdBy = cmd.ExecuteScalar().ToString();
                //Get creator's email:
                cmd.CommandText = "select user_email from users where userId = '" + topic_createdBy + "' ";
                string email = cmd.ExecuteScalar().ToString();
                //Get creator's fullname:
                cmd.CommandText = "select user_firstname from users where userId = '"+topic_createdBy+"' ";
                string creator = cmd.ExecuteScalar().ToString();
                cmd.CommandText = "select user_lastname from users where userId = '" + topic_createdBy + "' ";
                creator = creator + " " + cmd.ExecuteScalar().ToString();
                //Get topic_type:
                cmd.CommandText = "select topic_type from [Topics] where [topicId] = '" + topicId + "' ";
                string topic_type = cmd.ExecuteScalar().ToString();
                //Get topic_title:
                cmd.CommandText = "select topic_title from [Topics] where [topicId] = '" + topicId + "' ";
                string topic_title = cmd.ExecuteScalar().ToString();
                //Get topic_time:
                cmd.CommandText = "select topic_time from [Topics] where [topicId] = '" + topicId + "' ";
                string topic_time = cmd.ExecuteScalar().ToString();
                //Get topic_description:
                cmd.CommandText = "select topic_description from [Topics] where [topicId] = '" + topicId + "' ";
                string topic_description = cmd.ExecuteScalar().ToString();
                //Get "Yes" or "No" for topic_hasImage:
                cmd.CommandText = "select topic_hasImage from [Topics] where [topicId] = '" + topicId + "' ";
                int topic_hasImage = Convert.ToInt32(cmd.ExecuteScalar());
                string str_topic_hasImage = "";
                if (topic_hasImage == 0)
                    str_topic_hasImage = "Topic does not have an image.";
                else
                    str_topic_hasImage = "Topic has an image.";
                //Get topic_isDeleted ?:
                cmd.CommandText = "select topic_isDeleted from [Topics] where [topicId] = '" + topicId + "' ";
                int int_topic_isDeleted = Convert.ToInt32(cmd.ExecuteScalar());
                string topic_isDeleted = "";
                if (int_topic_isDeleted == 0)
                    topic_isDeleted = "Topic has not been deleted.";
                else
                    topic_isDeleted = "Topic has been deleted.";
                //Get topic_isApproved ?:
                cmd.CommandText = "select topic_isApproved from [Topics] where [topicId] = '" + topicId + "' ";
                int int_topic_isApproved = Convert.ToInt32(cmd.ExecuteScalar());
                string topic_isApproved;
                if (int_topic_isApproved == 0)
                    topic_isApproved = "Topic has not been approved.";
                else
                    topic_isApproved = "Topic has been approved.";
                //Get topic_isDenied ?:
                cmd.CommandText = "select topic_isDenied from [Topics] where [topicId] = '" + topicId + "' ";
                int int_topic_isDenied = Convert.ToInt32(cmd.ExecuteScalar());
                string topic_isDenied;
                if (int_topic_isDenied == 0)
                    topic_isDenied = "Topic has not been denied.";
                else
                    topic_isDenied = "Topic has been denied.";
                //Get topic_isTerminated ?:
                cmd.CommandText = "select topic_isTerminated from [Topics] where [topicId] = '" + topicId + "' ";
                int int_topic_isTerminated = Convert.ToInt32(cmd.ExecuteScalar());
                string topic_isTerminated = "";
                if (int_topic_isTerminated == 0)
                    topic_isTerminated = "Topic has not been terminated.";
                else
                    topic_isTerminated = "Topic has been terminated.";
                //Get tags:
                string tagNames = "";
                cmd.CommandText = "select count(*) from TagsForTopics where topicId = '"+topicId+"' ";
                int totalTags = Convert.ToInt32(cmd.ExecuteScalar());
                if (totalTags == 0)
                    tagNames = "There are no tags for the selected topic";
                for (int i = 1; i <= totalTags; i++)
                {
                    cmd.CommandText = "select [tagId] from(SELECT rowNum = ROW_NUMBER() OVER(ORDER BY tagId ASC), * FROM [TagsForTopics] where topicId = '" + topicId + "') as t where rowNum = '" + i + "'";
                    string tagId = cmd.ExecuteScalar().ToString();
                    cmd.CommandText = "select tag_name from Tags where tagId = '"+tagId+"' ";
                    if (totalTags == 1)
                        tagNames = cmd.ExecuteScalar().ToString();
                    else if (totalTags > 1)
                    {
                        if(i==0)
                            tagNames = cmd.ExecuteScalar().ToString();
                        else
                            tagNames = tagNames + ", " + cmd.ExecuteScalar().ToString();
                    }
                }
                //Create an informative message containing all information for the selected user:
                string imagesHTML = "";
                if (topic_hasImage == 1)
                {
                    cmd.CommandText = "select count(*) from ImagesForTopics where topicId = '" + topicId + "' ";
                    int totalImages = Convert.ToInt32(cmd.ExecuteScalar());
                    for (int i = 1; i <= totalImages; i++)
                    {
                        cmd.CommandText = "select [imageId] from(SELECT rowNum = ROW_NUMBER() OVER(ORDER BY imageId ASC), * FROM [ImagesForTopics] where topicId = '" + topicId + "') as t where rowNum = '" + i + "'";
                        string imageId = cmd.ExecuteScalar().ToString();
                        cmd.CommandText = "select image_name from Images where imageId = '" + imageId + "' ";
                        string image_name = cmd.ExecuteScalar().ToString();
                        imagesHTML = imagesHTML + "<img src='../../images/" + image_name + "'></img> <br /> <br/>";
                    }
                }
                lblTopicInformation.Text = 
                    "<table>"+
                    "<tr><td>Creator: </td><td>" + creator + "</td></tr>" +
                    "<tr><td>Type: </td><td>" + topic_type + "</td></tr>" +
                    "<tr><td>Title: </td><td>" + topic_title + "</td></tr>" +
                    "<tr><td>Time: </td><td>" + Layouts.getTimeFormat(topic_time) + "</td></tr>" +
                    "<tr><td>Has image?: </td><td>" + str_topic_hasImage + "</td></tr>" +
                    "<tr><td>Deleted?: </td><td>" + topic_isDeleted + "</td></tr>" +
                    "<tr><td>Approved?: </td><td>" + topic_isApproved + "</td></tr>" +
                    "<tr><td>Denied?: </td><td>" + topic_isDenied + "</td></tr>" +
                    "<tr><td>Terminated?: </td><td>" + topic_isTerminated + "</td></tr>" +
                    "<tr><td>Tags: </td><td>" + tagNames + "</td></tr>" +
                    "<tr><td>Description: </td><td> <div style=\"background: #DCCDCA; padding-left:5px; padding-right:5px; \"> " + topic_description + 
                    imagesHTML+ "</div></td></tr>" +
                    "</table>";
                lblTopicInformation.Visible = true;
                //Copy values to globals:
                g_topic_isApproved = int_topic_isApproved; g_topic_isDenied = int_topic_isDenied;
                g_creator = creator; g_topic_title = topic_title;g_email = email;
            }
            else
            {
                addSession();
                Response.Redirect("ApproveTopics");
            }
            connect.Close();
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

        protected void btnDeny_Click(object sender, EventArgs e)
        {
            //Hide the success message:
            lblMessage.Visible = false;
            //Set topic_isApproved = 0, topic_isDenied = 1: (1 in bit = true)
            //Store the previous information into the table "Logins":
            connect.Open();
            SqlCommand cmd = connect.CreateCommand();
            cmd.CommandText = "update topics set topic_isApproved = 0, topic_isDenied = 1 where topicId = '" + topicId + "' ";
            cmd.ExecuteScalar();
            connect.Close();
            //Create an email message to be sent:
            string emailMessage = "Hello " + g_creator + ",\n\n" +
                "This email is to inform you that your topic (" + g_topic_title + ") has been denied for NephroNet.\n\n" +
                "Best regards,\nNephroNet Support\nNephroNet2018@gmail.com";
            //Send an email notification to the user using the stored email:
            Email emailClass = new Email();
            emailClass.sendEmail(g_email, emailMessage);
            //Display a success message:
            lblMessage.Visible = true;
            lblMessage.Text = "The selected topic has been successfully denied and a notification email has been sent to the user!";
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
            cmd.CommandText = "update topics set topic_isApproved = 1, topic_isDenied = 0 where topicId = '"+topicId+"' ";
            cmd.ExecuteScalar();
            cmd.CommandText = "select topic_createdBy from topics where topicId = '"+topicId+"' ";
            string creatorId = cmd.ExecuteScalar().ToString();
            cmd.CommandText = "update UsersForTopics set isApproved = 1 where topicId = '" + topicId + "' and userId = '"+ creatorId + "' ";
            cmd.ExecuteScalar();
            connect.Close();
            //Create an email message to be sent:
            string emailMessage = "Hello " + g_creator + ",\n\n" +
                "This email is to inform you that your topic ("+ g_topic_title + ") has been approved for NephroNet.\n\n" +
                "Best regards,\nNephroNet Support\nNephroNet2018@gmail.com";
            //Send an email notification to the user using the stored email:
            Email emailClass = new Email();
            emailClass.sendEmail(g_email, emailMessage);
            //Display a success message:
            lblMessage.Visible = true;
            lblMessage.Text = "The selected topic has been successfully approved and a notification email has been sent to the user!";
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
            Response.Redirect("ApproveTopics");
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