using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NephroNet.Accounts.Physician
{
    public partial class JoinTopic : System.Web.UI.Page
    {
        static string conn = "";
        SqlConnection connect = new SqlConnection(conn);
        string username, roleId, loginId, token;
        string topicId = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            initialPageAccess();
            topicId = Request.QueryString["id"];
            showTopicInformation();
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

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            addSession();
            Response.Redirect("Home");
        }

        protected void getSession()
        {
            username = (string)(Session["username"]);
            roleId = (string)(Session["roleId"]);
            loginId = (string)(Session["loginId"]);
            token = (string)(Session["token"]);
        }
        protected void showTopicInformation()
        {
            connect.Open();
            SqlCommand cmd = connect.CreateCommand();
            cmd.CommandText = "select userId from users where loginId = '"+loginId+"' ";
            string userId = cmd.ExecuteScalar().ToString();
            //Check if the ID exists in the database:
            cmd.CommandText = "select count(*) from topics where topicId = '" + topicId + "' ";
            int countTopic = Convert.ToInt32(cmd.ExecuteScalar());
            connect.Close();
            if (countTopic > 0)//if ID exists, countTopic = 1
            {
                //Check if a request has already been sent:
                connect.Open();
                cmd.CommandText = "select count(*) from UsersForTopics where userId = '"+userId+"' and topicId = '"+topicId+"' ";
                int userApplied = Convert.ToInt32(cmd.ExecuteScalar());
                connect.Close();
                if (userApplied > 0)
                {
                    btnRequest.Visible = false;
                    //Show a message:
                    lblError.Text = "You have already applied for this topic.";
                    lblError.Visible = true;
                    lblError.ForeColor = System.Drawing.Color.Red;
                }
                else
                {
                    btnRequest.Visible = true;
                }
                lblTopic.Text = getTopicInformation();
            }
            else
            {
                addSession();
                Response.Redirect("Home");
            }            
        }
        protected string getTopicInformation()
        {
            string info = "";
            connect.Open();
            SqlCommand cmd = connect.CreateCommand();
            cmd.CommandText = "select topic_title from Topics where topicId = '"+topicId+"' ";
            string title = cmd.ExecuteScalar().ToString();
            cmd.CommandText = "select topic_type from Topics where topicId = '" + topicId + "' ";
            string type = cmd.ExecuteScalar().ToString();
            cmd.CommandText = "select topic_time from Topics where topicId = '" + topicId + "' ";
            string time = cmd.ExecuteScalar().ToString();
            cmd.CommandText = "select topic_description from Topics where topicId = '" + topicId + "' ";
            string description = cmd.ExecuteScalar().ToString();
            cmd.CommandText = "select topic_createdBy from Topics where topicId = '" + topicId + "' ";
            string creatorId = cmd.ExecuteScalar().ToString();
            cmd.CommandText = "select user_firstname from users where userId = '"+creatorId+"' ";
            string creator = cmd.ExecuteScalar().ToString();
            cmd.CommandText = "select user_lastname from users where userId = '" + creatorId + "' ";
            creator = creator + " " + cmd.ExecuteScalar().ToString();
            //Now check if the topic has images:
            cmd.CommandText = "select topic_hasImage from Topics where topicId = '"+topicId+"' ";
            int hasImage = Convert.ToInt32(cmd.ExecuteScalar());
            string imagesHTML = "";
            if (hasImage == 1)
            {
                cmd.CommandText = "select count(*) from ImagesForTopics where topicId = '" + topicId + "' ";
                int totalImages = Convert.ToInt32(cmd.ExecuteScalar());
                for (int i = 1; i <= totalImages; i++)
                {
                    cmd.CommandText = "select [imageId] from(SELECT rowNum = ROW_NUMBER() OVER(ORDER BY imageId ASC), * FROM [ImagesForTopics] where topicId = '" + topicId + "') as t where rowNum = '" + i + "'";
                    string imageId = cmd.ExecuteScalar().ToString();
                    cmd.CommandText = "select image_name from Images where imageId = '" + imageId + "' ";
                    string image_name = cmd.ExecuteScalar().ToString();
                    imagesHTML = imagesHTML + "<img src='../../images/" + image_name + "'></img> <br /><br />";
                }
            }
            info = "Creator: " + creator + "<br />" +
                        "Type: " + type + "<br />" +
                        "Title: " + title + "<br />" +
                        "Time: " + Layouts.getTimeFormat(time) + "<br />" +
                        "Description: \"" + description + "\"<br />" +
                        imagesHTML;
            connect.Close();
            return info;
        }
        protected void btnRequest_Click(object sender, EventArgs e)
        {
            connect.Open();
            SqlCommand cmd = connect.CreateCommand();
            //Get the topic's creator:
            cmd.CommandText = "select topic_createdBy from Topics where topicId = '" + topicId + "' ";
            string creatorId = cmd.ExecuteScalar().ToString();
            cmd.CommandText = "select user_firstname from Users where userId = '" + creatorId + "' ";
            string creator_name = cmd.ExecuteScalar().ToString();
            cmd.CommandText = "select user_lastname from Users where userId = '" + creatorId + "' ";
            creator_name = creator_name + " " + cmd.ExecuteScalar().ToString();
            cmd.CommandText = "select user_email from Users where userId = '" + creatorId + "' ";
            string creator_email = cmd.ExecuteScalar().ToString();
            cmd.CommandText = "select topic_title from Topics where topicId = '" + topicId + "' ";
            string title = cmd.ExecuteScalar().ToString();
            //Get requester info:
            cmd.CommandText = "select userId from users where loginId = '"+loginId+"' ";
            string requesterId = cmd.ExecuteScalar().ToString();
            cmd.CommandText = "select user_firstname from Users where userId = '" + requesterId + "' ";
            string requester_name = cmd.ExecuteScalar().ToString();
            cmd.CommandText = "select user_lastname from Users where userId = '" + requesterId + "' ";
            requester_name = requester_name + " " + cmd.ExecuteScalar().ToString();
            cmd.CommandText = "select user_email from Users where userId = '" + requesterId + "' ";
            string requester_email = cmd.ExecuteScalar().ToString();
            Email emailClass = new Email();
            //email the creator:
            string emailCreator = "Hello " + creator_name + ",\n\n" +
                "This email is to inform you that the user ("+ requester_name + ") requested to join your topic (" + title + ") .\n\n" +
                "Best regards,\nNephroNet Support\nNephroNet2018@gmail.com";
            //Send an email notification to the user using the stored email:
            emailClass.sendEmail(creator_email, emailCreator);
            //Email the user requested to join:
            string emailRequester = "Hello " + requester_name + ",\n\n" +
                "This email is to inform you that your request to join the topic with the title (" + title + ") has been sent. Please, allow some time for review and you will be notified once the review is done.\n\n" +
                "Best regards,\nNephroNet Support\nNephroNet2018@gmail.com";
            //Send an email notification to the user using the stored email:
            emailClass.sendEmail(requester_email, emailRequester);
            //Add an alert to the creator:
            cmd.CommandText = "insert into UsersForTopics(userId, isApproved, topicId) values ('"+requesterId+"', '0', '"+topicId+"') ";
            cmd.ExecuteScalar();
            connect.Close();
            //Show a successfull message:
            lblError.Text = "You have successfully requested to join!";
            lblError.ForeColor = System.Drawing.Color.Green;
            lblError.Visible = true;
            //Hide the request button:
            btnRequest.Visible = false;
        }
    }
}