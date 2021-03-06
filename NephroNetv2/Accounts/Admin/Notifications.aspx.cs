﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NephroNet.Accounts.Admin
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
            countNewUsers();
            countNewTopics();
            countNewMessages();
            countNewComplains();
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
        protected void showAllFields()
        {
            btnNewUsers.Visible = true;
            btnNewTopics.Visible = true;
            btnNewMessages.Visible = true;
            btnNewComplains.Visible = true;
        }
        protected void countNewUsers()
        {
            connect.Open();
            SqlCommand cmd = connect.CreateCommand();
            cmd.CommandText = "select count(*) from Registrations";
            int count = Convert.ToInt32(cmd.ExecuteScalar());
            if(count == 0)
            {
                lblNewUsers.Text = "There are no new users to review.";
                btnNewUsers.Visible = false;
                //lblNewUsers.Visible = false;
            }
            else if(count == 1)
            {
                lblNewUsers.Text = "There is one new user to review.";
                btnNewUsers.Visible = true;
                lblNewUsers.Visible = true;
            }
            else
            {
                lblNewUsers.Text = "There are "+count+" new users to review.";
                btnNewUsers.Visible = true;
                lblNewUsers.Visible = true;
            }
            connect.Close();
        }
        
        protected void countNewTopics()
        {
            connect.Open();
            SqlCommand cmd = connect.CreateCommand();
            cmd.CommandText = "select count(*) from [Topics] where topic_isApproved = 0 and topic_isDenied = 0 and topic_isTerminated = 0";
            int count = Convert.ToInt32(cmd.ExecuteScalar());
            if (count == 0)
            {
                lblNewTopics.Text = "There are no new topics to review.";
                btnNewTopics.Visible = false;
                //lblNewTopics.Visible = false;
            }
            else if (count == 1)
            {
                lblNewTopics.Text = "There is one new topic to review.";
                btnNewTopics.Visible = true;
                lblNewTopics.Visible = true;
            }
            else
            {
                lblNewTopics.Text = "There are " + count + " new topics to review.";
                btnNewTopics.Visible = true;
                lblNewTopics.Visible = true;
            }
            connect.Close();
        }
        protected void countNewMessages()
        {
            connect.Open();
            SqlCommand cmd = connect.CreateCommand();
            //count messages that are not approved and have not been denied.
            cmd.CommandText = "select count(*) from [Entries] where entry_isApproved = 0 and entry_isDenied = 0 and entry_isDeleted = 0";
            int totalCount = Convert.ToInt32(cmd.ExecuteScalar());
            int count = 0;
            for (int i = 1; i <= totalCount; i++)
            {
                //Get the Message ID:
                cmd.CommandText = "select [entryId] from(SELECT rowNum = ROW_NUMBER() OVER(ORDER BY entryId ASC), * FROM [Entries] where entry_isApproved = 0 and entry_isDenied = 0 and entry_isDeleted = 0) as t where rowNum = '" + i + "'";
                string id = cmd.ExecuteScalar().ToString();
                //Get topic ID for the selected message:
                cmd.CommandText = "select [topicId] from [Entries] where entryId = '"+id+"' ";
                string topic = cmd.ExecuteScalar().ToString();
                cmd.CommandText = "select [topic_isDeleted] from [Topics] where topicId = '" + topic + "' ";
                int topic_isDeleted = Convert.ToInt32(cmd.ExecuteScalar());
                cmd.CommandText = "select [topic_isTerminated] from [Topics] where topicId = '" + topic + "' ";
                int topic_isTerminated = Convert.ToInt32(cmd.ExecuteScalar());
                if (topic_isDeleted == 0 && topic_isTerminated == 0)
                    count++;
            }
            if (count == 0)
            {
                lblNewMessages.Text = "There are no new messages to review.";
                btnNewMessages.Visible = false;
                //lblNewMessages.Visible = false;
            }
            else if (count == 1)
            {
                lblNewMessages.Text = "There is one new message to review.";
                btnNewMessages.Visible = true;
                lblNewMessages.Visible = true;
            }
            else
            {
                lblNewMessages.Text = "There are " + count + " new messages to review.";
                btnNewMessages.Visible = true;
                lblNewMessages.Visible = true;
            }
            connect.Close();
        }
        protected void countNewComplains()
        {
            connect.Open();
            SqlCommand cmd = connect.CreateCommand();
            cmd.CommandText = "select count(*) from [Complains] ";
            int count = Convert.ToInt32(cmd.ExecuteScalar());
            if (count == 0)
            {
                lblNewComplains.Text = "There are no new complains to review.";
                btnNewComplains.Visible = false;
                //lblNewTopics.Visible = false;
            }
            else if (count == 1)
            {
                lblNewComplains.Text = "There is one new complains to review.";
                btnNewComplains.Visible = true;
                lblNewComplains.Visible = true;
            }
            else
            {
                lblNewComplains.Text = "There are " + count + " new complains to review.";
                btnNewComplains.Visible = true;
                lblNewComplains.Visible = true;
            }
            connect.Close();
        }
        protected void btnNewUsers_Click(object sender, EventArgs e)
        {
            addSession();
            Response.Redirect("ApproveUsers");
        }
        protected void btnNewTopics_Click(object sender, EventArgs e)
        {
            addSession();
            Response.Redirect("ApproveTopics");
        }
        protected void btnNewMessages_Click(object sender, EventArgs e)
        {
            addSession();
            Response.Redirect("ApproveMessages");
        }
        protected void btnNewComplains_Click(object sender, EventArgs e)
        {
            addSession();
            Response.Redirect("ApproveComplains");
        }


    }
}