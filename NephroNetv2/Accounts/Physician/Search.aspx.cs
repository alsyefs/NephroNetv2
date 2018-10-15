using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NephroNet.Accounts.Physician
{
    public partial class Search : System.Web.UI.Page
    {
        static string conn = "";
        SqlConnection connect = new SqlConnection(conn);
        string username, roleId, loginId, token;
        protected void Page_Load(object sender, EventArgs e)
        {
            initialAccess();
            createSomeTable();
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
        protected void initialAccess()
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
            CheckPhysicianSession session = new CheckPhysicianSession();
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
        protected void hideEverything()
        {
            lblResultsMessage.Visible = false;
            lblErrorMessage.Visible = false;
        }
        protected void createSomeTable()
        {
            hideEverything();
            bool correct = checkInput();
            if (correct)
            {
                //Count the results:
                int count = countResults();
                //if no results, show a message:
                if (count == 0)
                {
                    lblResultsMessage.Visible = true;
                    grdResults.Visible = false;
                }
                else
                {
                    grdResults.Visible = true;
                    lblResultsMessage.Visible = false;
                    //call a method to create a table for the selected criteria:
                    if (drpSearch.SelectedIndex == 1)//Searching for topic titles
                        createTopicsTable();
                    else if (drpSearch.SelectedIndex == 2)//Searching for users' names
                        createUsersTable();
                    else if (drpSearch.SelectedIndex == 3)//Searching for messages
                        createMessagesTable();
                    else if (drpSearch.SelectedIndex == 4)//Searching for topics within a time period
                        createTimePeriodTable();
                    else if (drpSearch.SelectedIndex == 5)//Searching for everything; topics, users, and messages
                        createEverythingTable();
                }
            }
            hideEverything();
        }
        protected void showCalendars()
        {
            calFrom.Visible = true;
            calTo.Visible = true;
            lblFrom.Visible = true;
            lblTo.Visible = true;
            txtSearch.Visible = false;
            txtSearch.Text = "";
        }
        protected void hideCalendars()
        {
            calFrom.Visible = false;
            calTo.Visible = false;
            lblFrom.Visible = false;
            lblTo.Visible = false;
            txtSearch.Visible = true;
            txtSearch.Text = "";
        }
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            hideEverything();
            bool correct = checkInput();
            if (correct)
            {
                //Count the results:
                int count = countResults();
                //if no results, show a message:
                if (count == 0)
                {
                    lblResultsMessage.Visible = true;
                    grdResults.Visible = false;
                }
                else
                {
                    grdResults.Visible = true;
                    lblResultsMessage.Visible = false;
                    //call a method to create a table for the selected criteria:
                    if (drpSearch.SelectedIndex == 1)//Searching for topic titles
                        createTopicsTable();
                    else if (drpSearch.SelectedIndex == 2)//Searching for users' names
                        createUsersTable();
                    else if (drpSearch.SelectedIndex == 3)//Searching for messages
                        createMessagesTable();
                    else if (drpSearch.SelectedIndex == 4)//Searching for topics within a time period
                        createTimePeriodTable();
                    else if (drpSearch.SelectedIndex == 5)//Searching for everything; topics, users, and messages
                        createEverythingTable();
                }
            }
        }
        protected bool checkInput()
        {
            bool correct = true;
            lblErrorMessage.Text = "";
            if (drpSearch.SelectedIndex != 4)// 4= search using dates From and To
            {
                //check if search text is blank:
                if (string.IsNullOrWhiteSpace(txtSearch.Text))
                {
                    correct = false;
                    lblErrorMessage.Text += "Please type something in the search text field.<br/>";
                }
            }
            //if input has one quotation, replace it with a double quotaion to avoid SQL errors:
            txtSearch.Text = txtSearch.Text.Replace("'", "''");
            //check if no criteria was selected:
            if (drpSearch.SelectedIndex == 0)
            {
                correct = false;
                lblErrorMessage.Text += "Please select a search criteria.<br/>";
            }
            if (!correct)
                lblErrorMessage.Visible = true;
            else
                lblErrorMessage.Visible = false;
            return correct;
        }
        protected void txtSearch_TextChanged(object sender, EventArgs e)
        {

        }
        protected void drpSearch_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtSearch.Text = "";
            grdResults.Dispose();
            grdResults.DataSource = null;
            grdResults.Visible = false;
            if (drpSearch.SelectedIndex == 4)//searching with a time period
                showCalendars();
            else
                hideCalendars();
        }
        protected void grdResults_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            grdResults.PageIndex = e.NewPageIndex;
            grdResults.DataBind();
            rebindValues();
        }
        protected int countResults()
        {
            int count = 0;
            string searchString = txtSearch.Text.Replace("'", "''");
            string[] words = searchString.Split(' ');
            connect.Open();
            SqlCommand cmd = connect.CreateCommand();
            if (drpSearch.SelectedIndex == 1)//Searching topics by topic titles
            {
                SortedSet<string> set_results = new SortedSet<string>();
                foreach (string word in words)
                {
                    if (!string.IsNullOrWhiteSpace(word))
                    {
                        cmd.CommandText = "select count(*) from topics where topic_title like '%" + word + "%' and topic_isDeleted = 0 and topic_isDenied = 0 and topic_isApproved = 1 ";
                        int temp_count = Convert.ToInt32(cmd.ExecuteScalar());
                        for (int i = 1; i <= temp_count; i++)
                        {
                            cmd.CommandText = "select [topicId] from(SELECT rowNum = ROW_NUMBER() OVER(ORDER BY topicId ASC), * FROM [topics] where topic_title like '%" + word + "%' and topic_isDeleted = 0 and topic_isDenied = 0 and topic_isApproved = 1) as t where rowNum = '" + i + "'";
                            string temp_Id = cmd.ExecuteScalar().ToString();
                            set_results.Add(temp_Id);
                        }
                    }
                }
                count = set_results.Count;
            }
            else if (drpSearch.SelectedIndex == 2)//Searching topics by users' fullnames
            {
                SortedSet<string> set_results = new SortedSet<string>();
                foreach (string word in words)
                {
                    if (!string.IsNullOrWhiteSpace(word))
                    {
                        cmd.CommandText = "select count(*) from users where (user_firstname+ ' ' +user_lastname) like '%" + word + "%' ";
                        int countUsers = Convert.ToInt32(cmd.ExecuteScalar());
                        for (int i = 1; i <= countUsers; i++)
                        {
                            cmd.CommandText = "select [userId] from(SELECT rowNum = ROW_NUMBER() OVER(ORDER BY userId ASC), * FROM [Users] where (user_firstname+ ' ' +user_lastname) like '%" + word + "%') as t where rowNum = '" + i + "'";
                            string temp_Id = cmd.ExecuteScalar().ToString();
                            set_results.Add(temp_Id);
                        }
                    }
                }
                for (int i = 0; i < set_results.Count; i++)
                {
                    string temp_userId = set_results.ElementAt(i);
                    cmd.CommandText = "select count(*) from topics where topic_createdBy = '" + temp_userId + "' ";
                    int totalTopicsForTempUser = Convert.ToInt32(cmd.ExecuteScalar());
                    count += totalTopicsForTempUser;
                }
            }
            else if (drpSearch.SelectedIndex == 3)//Searching topics by messages
            {
                SortedSet<string> set_results = new SortedSet<string>();
                foreach (string word in words)
                {
                    if (!string.IsNullOrWhiteSpace(word))
                    {
                        cmd.CommandText = "select count(*) from entries where entry_text like '%" + word + "%' and entry_isDeleted = 0 and entry_isApproved = 1 and entry_isDenied = 0 ";
                        int countEntries = Convert.ToInt32(cmd.ExecuteScalar());
                        for (int i = 1; i <= countEntries; i++)
                        {
                            cmd.CommandText = "select [entryId] from(SELECT rowNum = ROW_NUMBER() OVER(ORDER BY entryId ASC), * FROM [entries] where entry_text like '%" + word + "%' and entry_isDeleted = 0 and entry_isApproved = 1 and entry_isDenied = 0) as t where rowNum = '" + i + "'";
                            string temp_Id = cmd.ExecuteScalar().ToString();
                            set_results.Add(temp_Id);
                        }
                    }
                }
                count = set_results.Count;
            }
            else if (drpSearch.SelectedIndex == 4)//Search within a time period
            {
                DateTime start_time = calFrom.SelectedDate, end_time = calTo.SelectedDate;
                if (!start_time.ToString().Equals("1/1/0001 12:00:00 AM") && !end_time.ToString().Equals("1/1/0001 12:00:00 AM"))
                {
                    cmd.CommandText = "select count(*) from topics where topic_time >= '" + start_time + "' and topic_time <= '" + end_time + "' and topic_isDeleted = 0 and topic_isDenied = 0 and topic_isApproved = 1 ";
                    count = Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            else if (drpSearch.SelectedIndex == 5)//Searching topics by everything; topics, users, and messages
            {
                SortedSet<string> topicIds = new SortedSet<string>();
                SortedSet<string> userIds = new SortedSet<string>();
                SortedSet<string> entryIds = new SortedSet<string>();
                foreach (string word in words)
                {
                    if (!string.IsNullOrWhiteSpace(word))
                    {
                        cmd.CommandText = "select count(*) from topics where topic_title like '%" + word + "%' and topic_isDeleted = 0 and topic_isDenied = 0 and topic_isApproved = 1 ";
                        int temp_count = Convert.ToInt32(cmd.ExecuteScalar());
                        for (int i = 1; i <= temp_count; i++)
                        {
                            cmd.CommandText = "select [topicId] from(SELECT rowNum = ROW_NUMBER() OVER(ORDER BY topicId ASC), * FROM [topics] where topic_title like '%" + word + "%' and topic_isDeleted = 0 and topic_isDenied = 0 and topic_isApproved = 1) as t where rowNum = '" + i + "'";
                            string temp_Id = cmd.ExecuteScalar().ToString();
                            topicIds.Add(temp_Id);
                        }
                    }
                }
                count = topicIds.Count;
                foreach (string word in words)
                {
                    if (!string.IsNullOrWhiteSpace(word))
                    {
                        cmd.CommandText = "select count(*) from users where (user_firstname+ ' ' +user_lastname) like '%" + word + "%' ";
                        int countUsers = Convert.ToInt32(cmd.ExecuteScalar());
                        for (int i = 1; i <= countUsers; i++)
                        {
                            cmd.CommandText = "select [userId] from(SELECT rowNum = ROW_NUMBER() OVER(ORDER BY userId ASC), * FROM [Users] where (user_firstname+ ' ' +user_lastname) like '%" + word + "%') as t where rowNum = '" + i + "'";
                            string temp_Id = cmd.ExecuteScalar().ToString();
                            userIds.Add(temp_Id);
                        }
                    }
                }
                for (int i = 0; i < userIds.Count; i++)
                {
                    string temp_userId = userIds.ElementAt(i);
                    cmd.CommandText = "select count(*) from topics where topic_createdBy = '" + temp_userId + "' ";
                    int totalTopicsForTempUser = Convert.ToInt32(cmd.ExecuteScalar());
                    count += totalTopicsForTempUser;
                }
                foreach (string word in words)
                {
                    if (!string.IsNullOrWhiteSpace(word))
                    {
                        cmd.CommandText = "select count(*) from entries where entry_text like '%" + word + "%' and entry_isDeleted = 0 and entry_isApproved = 1 and entry_isDenied = 0 ";
                        int countEntries = Convert.ToInt32(cmd.ExecuteScalar());
                        for (int i = 1; i <= countEntries; i++)
                        {
                            cmd.CommandText = "select [entryId] from(SELECT rowNum = ROW_NUMBER() OVER(ORDER BY entryId ASC), * FROM [entries] where entry_text like '%" + word + "%' and entry_isDeleted = 0 and entry_isApproved = 1 and entry_isDenied = 0) as t where rowNum = '" + i + "'";
                            string temp_Id = cmd.ExecuteScalar().ToString();
                            entryIds.Add(temp_Id);
                        }
                    }
                }
                count += entryIds.Count;
            }
            connect.Close();
            return count;
        }
        protected void rebindValues()
        {
            connect.Open();
            SqlCommand cmd = connect.CreateCommand();
            string id = "", title = "", creator = "";
            string searchString = txtSearch.Text.Replace("'", "''");
            if (grdResults.Rows.Count > 0)
            {
                //Hide the header called "Topic ID" and "User ID":
                grdResults.HeaderRow.Cells[5].Visible = false;
                grdResults.HeaderRow.Cells[6].Visible = false;
                //Hide IDs columns and content which are located in columns index 5 and 6:
                for (int i = 0; i < grdResults.Rows.Count; i++)
                {
                    grdResults.Rows[i].Cells[5].Visible = false;
                    grdResults.Rows[i].Cells[6].Visible = false;
                }
            }
            for (int row = 0; row < grdResults.Rows.Count; row++)
            {
                //Set the creator's link
                creator = grdResults.Rows[row].Cells[4].Text;
                string creatorId = grdResults.Rows[row].Cells[6].Text;
                HyperLink creatorLink = new HyperLink();
                creatorLink.Text = creator + " ";
                //creatorLink.NavigateUrl = "Profile.aspx?id=" + creatorId;
                grdResults.Rows[row].Cells[4].Controls.Add(creatorLink);
                //Set the title's link
                title = grdResults.Rows[row].Cells[0].Text;
                id = grdResults.Rows[row].Cells[5].Text;
                HyperLink topicLink = new HyperLink();
                topicLink.Text = title + " ";
                topicLink.NavigateUrl = "ViewTopic.aspx?id=" + id;
                grdResults.Rows[row].Cells[0].Controls.Add(topicLink);
            }
            connect.Close();
        }
        protected void createTopicsTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Title", typeof(string));
            dt.Columns.Add("Found in", typeof(string));
            dt.Columns.Add("Time", typeof(string));
            dt.Columns.Add("Type", typeof(string));
            dt.Columns.Add("Creator", typeof(string));
            dt.Columns.Add("Topic ID", typeof(string));
            dt.Columns.Add("User ID", typeof(string));
            string id = "", title = "", type = "", creator = "", time = "";
            string searchString = txtSearch.Text.Replace("'", "''");
            string[] words = searchString.Split(' ');
            SortedSet<string> set_results = new SortedSet<string>();
            connect.Open();
            SqlCommand cmd = connect.CreateCommand();
            foreach (string word in words)
            {
                if (!string.IsNullOrWhiteSpace(word))
                {
                    cmd.CommandText = "select count(*) from topics where topic_title like '%" + word + "%' and topic_isDeleted = 0 and topic_isDenied = 0 and topic_isApproved = 1 ";
                    int countTopics = Convert.ToInt32(cmd.ExecuteScalar());
                    for (int i = 1; i <= countTopics; i++)
                    {
                        cmd.CommandText = "select [topicId] from(SELECT rowNum = ROW_NUMBER() OVER(ORDER BY topicId ASC), * FROM [topics] where topic_title like '%" + word + "%' and topic_isDeleted = 0 and topic_isDenied = 0 and topic_isApproved = 1) as t where rowNum = '" + i + "'";
                        string temp_topicId = cmd.ExecuteScalar().ToString();
                        set_results.Add(temp_topicId);
                    }
                }
            }
            int count = set_results.Count;
            for (int i = 0; i < count; i++)
            {
                //Get the topic ID:
                id = set_results.ElementAt(i);
                //Get type:
                cmd.CommandText = "select [topic_time] from Topics where topicId = '" + id + "' ";
                time = cmd.ExecuteScalar().ToString();
                //Get title:
                cmd.CommandText = "select [topic_title] from Topics where topicId = '" + id + "' ";
                title = cmd.ExecuteScalar().ToString();
                //Get type:
                cmd.CommandText = "select [topic_type] from Topics where topicId = '" + id + "' ";
                type = cmd.ExecuteScalar().ToString();
                //Get creator's ID:
                cmd.CommandText = "select [topic_createdBy] from Topics where topicId = '" + id + "' ";
                string creatorId = cmd.ExecuteScalar().ToString();
                //Get creator's name:
                cmd.CommandText = "select user_firstname from users where userId = '" + creatorId + "' ";
                creator = cmd.ExecuteScalar().ToString();
                cmd.CommandText = "select user_lastname from users where userId = '" + creatorId + "' ";
                creator = creator + " " + cmd.ExecuteScalar().ToString();
                if (type.Equals("Consultation"))
                {
                    cmd.CommandText = "select userId from Users where loginId = '" + loginId + "' ";
                    string userId = cmd.ExecuteScalar().ToString();
                    int int_roleId = Convert.ToInt32(roleId);
                    if (int_roleId == 2)//2 = Physician
                    {
                        cmd.CommandText = "select count(*) from Consultations where topicId = '" + id + "' and physician_userId = '" + userId + "' ";
                        int exists = Convert.ToInt32(cmd.ExecuteScalar());
                        if (exists > 0)
                        {
                            dt.Rows.Add(title, "Title", Layouts.getTimeFormat(time), type, creator, id, creatorId);
                        }
                    }
                    else if (int_roleId == 3)//3 = Patient
                    {
                        cmd.CommandText = "select count(*) from Consultations where topicId = '" + id + "' and patient_userId = '" + userId + "' ";
                        int exists = Convert.ToInt32(cmd.ExecuteScalar());
                        if (exists > 0)
                        {
                            dt.Rows.Add(title, "Title", Layouts.getTimeFormat(time), type, creator, id, creatorId);
                        }
                    }
                    //Else will be the admin. If admin, just don't show anything about the consultation topics.
                }
                else
                    dt.Rows.Add(title, "Title", Layouts.getTimeFormat(time), type, creator, id, creatorId);
            }
            connect.Close();
            grdResults.DataSource = dt;
            grdResults.DataBind();
            grdResults.Visible = true;
            rebindValues();
        }
        protected void createUsersTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Title", typeof(string));
            dt.Columns.Add("Found in", typeof(string));
            dt.Columns.Add("Time", typeof(string));
            dt.Columns.Add("Type", typeof(string));
            dt.Columns.Add("Creator", typeof(string));
            dt.Columns.Add("Topic ID", typeof(string));
            dt.Columns.Add("User ID", typeof(string));
            string id = "", title = "", type = "", creator = "", time = "";
            string searchString = txtSearch.Text.Replace("'", "''");
            string[] words = searchString.Split(' ');
            SortedSet<string> set_results = new SortedSet<string>();
            connect.Open();
            SqlCommand cmd = connect.CreateCommand();
            foreach (string word in words)
            {
                if (!string.IsNullOrWhiteSpace(word))
                {
                    cmd.CommandText = "select count(*) from users where (user_firstname+ ' ' +user_lastname) like '%" + word + "%' ";
                    int countTopics = Convert.ToInt32(cmd.ExecuteScalar());
                    for (int i = 1; i <= countTopics; i++)
                    {
                        cmd.CommandText = "select [userId] from(SELECT rowNum = ROW_NUMBER() OVER(ORDER BY userId ASC), * FROM [Users] where (user_firstname+ ' ' +user_lastname) like '%" + word + "%' ) as t where rowNum = '" + i + "'";
                        string temp_userId = cmd.ExecuteScalar().ToString();
                        set_results.Add(temp_userId);
                    }
                }
            }
            int totalUsers = set_results.Count;
            for (int i = 0; i < totalUsers; i++)
            {
                string temp_userId = set_results.ElementAt(i);
                cmd.CommandText = "select count(*) from topics where topic_createdBy = '" + temp_userId + "' and topic_isApproved = 1 and topic_isDenied = 0 and topic_isDeleted = 0";
                int totalTopicsForTempUser = Convert.ToInt32(cmd.ExecuteScalar());
                for (int j = 1; j <= totalTopicsForTempUser; j++)
                {
                    //Get the topic ID:
                    cmd.CommandText = "select [topicId] from(SELECT rowNum = ROW_NUMBER() OVER(ORDER BY topicId ASC), * FROM [Topics] where topic_createdBy = '" + temp_userId + "' and topic_isApproved = 1 and topic_isDenied = 0 and topic_isDeleted = 0) as t where rowNum = '" + j + "'";
                    id = cmd.ExecuteScalar().ToString();
                    //Get type:
                    cmd.CommandText = "select [topic_time] from Topics where topicId = '" + id + "' ";
                    time = cmd.ExecuteScalar().ToString();
                    //Get title:
                    cmd.CommandText = "select [topic_title] from Topics where topicId = '" + id + "' ";
                    title = cmd.ExecuteScalar().ToString();
                    //Get type:
                    cmd.CommandText = "select [topic_type] from Topics where topicId = '" + id + "' ";
                    type = cmd.ExecuteScalar().ToString();
                    //Get creator's ID:
                    cmd.CommandText = "select [topic_createdBy] from Topics where topicId = '" + id + "' ";
                    string creatorId = cmd.ExecuteScalar().ToString();
                    //Get creator's name:
                    cmd.CommandText = "select user_firstname from users where userId = '" + creatorId + "' ";
                    creator = cmd.ExecuteScalar().ToString();
                    cmd.CommandText = "select user_lastname from users where userId = '" + creatorId + "' ";
                    creator = creator + " " + cmd.ExecuteScalar().ToString();
                    if (type.Equals("Consultation"))
                    {
                        cmd.CommandText = "select userId from Users where loginId = '" + loginId + "' ";
                        string userId = cmd.ExecuteScalar().ToString();
                        int int_roleId = Convert.ToInt32(roleId);
                        if (int_roleId == 2)//2 = Physician
                        {
                            cmd.CommandText = "select count(*) from Consultations where topicId = '" + id + "' and physician_userId = '" + userId + "' ";
                            int exists = Convert.ToInt32(cmd.ExecuteScalar());
                            if (exists > 0)
                            {
                                dt.Rows.Add(title, "Creator Name", Layouts.getTimeFormat(time), type, creator, id, creatorId);
                            }
                        }
                        else if (int_roleId == 3)//3 = Patient
                        {
                            cmd.CommandText = "select count(*) from Consultations where topicId = '" + id + "' and patient_userId = '" + userId + "' ";
                            int exists = Convert.ToInt32(cmd.ExecuteScalar());
                            if (exists > 0)
                            {
                                dt.Rows.Add(title, "Creator Name", Layouts.getTimeFormat(time), type, creator, id, creatorId);
                            }
                        }
                        //Else will be the admin. If admin, just don't show anything about the consultation topics.
                    }
                    else
                        dt.Rows.Add(title, "Creator Name", Layouts.getTimeFormat(time), type, creator, id, creatorId);
                }
            }
            connect.Close();
            grdResults.DataSource = dt;
            grdResults.DataBind();
            grdResults.Visible = true;
            rebindValues();
        }
        protected void createMessagesTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Title", typeof(string));
            dt.Columns.Add("Found in", typeof(string));
            dt.Columns.Add("Time", typeof(string));
            dt.Columns.Add("Type", typeof(string));
            dt.Columns.Add("Creator", typeof(string));
            dt.Columns.Add("Topic ID", typeof(string));
            dt.Columns.Add("User ID", typeof(string));
            string id = "", title = "", type = "", creator = "", time = "";
            string searchString = txtSearch.Text.Replace("'", "''");
            string[] words = searchString.Split(' ');
            SortedSet<string> set_results = new SortedSet<string>();
            SqlCommand cmd = connect.CreateCommand();
            connect.Open();
            foreach (string word in words)
            {
                if (!string.IsNullOrWhiteSpace(word))
                {
                    cmd.CommandText = "select count(*) from entries where entry_text like '%" + word + "%' and entry_isDeleted = 0 and entry_isApproved = 1 and entry_isDenied = 0 ";
                    int count_temp = Convert.ToInt32(cmd.ExecuteScalar());
                    for (int i = 1; i <= count_temp; i++)
                    {
                        cmd.CommandText = "select [topicId] from(SELECT rowNum = ROW_NUMBER() OVER(ORDER BY topicId ASC), * FROM [Entries] where entry_text like '%" + word + "%' and entry_isDeleted = 0 and entry_isApproved = 1 and entry_isDenied = 0) as t where rowNum = '" + i + "'";
                        string temp_userId = cmd.ExecuteScalar().ToString();
                        set_results.Add(temp_userId);
                    }
                }
            }
            int count = set_results.Count;
            for (int i = 0; i < count; i++)
            {
                //Get the topic ID:
                string new_id = set_results.ElementAt(i);
                if (!new_id.Equals(id))
                {
                    id = new_id;
                    //Check if the topic of the selected message is deleted or not:
                    cmd.CommandText = "select topic_isDeleted from Topics where topicId = '" + id + "' ";
                    int isDeleted = Convert.ToInt32(cmd.ExecuteScalar());
                    if (isDeleted == 0)//0: False, meaning that the topic is not deleted
                    {
                        //Get type:
                        cmd.CommandText = "select [topic_time] from Topics where topicId = '" + id + "' ";
                        time = cmd.ExecuteScalar().ToString();
                        //Get title:
                        cmd.CommandText = "select [topic_title] from Topics where topicId = '" + id + "' ";
                        title = cmd.ExecuteScalar().ToString();
                        //Get type:
                        cmd.CommandText = "select [topic_type] from Topics where topicId = '" + id + "' ";
                        type = cmd.ExecuteScalar().ToString();
                        //Get creator's ID:
                        cmd.CommandText = "select [topic_createdBy] from Topics where topicId = '" + id + "' ";
                        string creatorId = cmd.ExecuteScalar().ToString();
                        //Get creator's name:
                        cmd.CommandText = "select user_firstname from users where userId = '" + creatorId + "' ";
                        creator = cmd.ExecuteScalar().ToString();
                        cmd.CommandText = "select user_lastname from users where userId = '" + creatorId + "' ";
                        creator = creator + " " + cmd.ExecuteScalar().ToString();
                        if (type.Equals("Consultation"))
                        {
                            cmd.CommandText = "select userId from Users where loginId = '" + loginId + "' ";
                            string userId = cmd.ExecuteScalar().ToString();
                            int int_roleId = Convert.ToInt32(roleId);
                            if (int_roleId == 2)//2 = Physician
                            {
                                cmd.CommandText = "select count(*) from Consultations where topicId = '" + id + "' and physician_userId = '" + userId + "' ";
                                int exists = Convert.ToInt32(cmd.ExecuteScalar());
                                if (exists > 0)
                                {
                                    dt.Rows.Add(title, "Message Text", Layouts.getTimeFormat(time), type, creator, id, creatorId);
                                }
                            }
                            else if (int_roleId == 3)//3 = Patient
                            {
                                cmd.CommandText = "select count(*) from Consultations where topicId = '" + id + "' and patient_userId = '" + userId + "' ";
                                int exists = Convert.ToInt32(cmd.ExecuteScalar());
                                if (exists > 0)
                                {
                                    dt.Rows.Add(title, "Message Text", Layouts.getTimeFormat(time), type, creator, id, creatorId);
                                }
                            }
                            //Else will be the admin. If admin, just don't show anything about the consultation topics.
                        }
                        else
                            dt.Rows.Add(title, "Message Text", Layouts.getTimeFormat(time), type, creator, id, creatorId);
                    }
                }
            }
            connect.Close();
            grdResults.DataSource = dt;
            grdResults.DataBind();
            grdResults.Visible = true;
            rebindValues();
        }
        protected void createEverythingTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Title", typeof(string));
            dt.Columns.Add("Found in", typeof(string));
            dt.Columns.Add("Time", typeof(string));
            dt.Columns.Add("Type", typeof(string));
            dt.Columns.Add("Creator", typeof(string));
            dt.Columns.Add("Topic ID", typeof(string));
            dt.Columns.Add("User ID", typeof(string));
            string id = "", title = "", type = "", creator = "", time = "";
            string searchString = txtSearch.Text.Replace("'", "''");
            string[] words = searchString.Split(' ');
            connect.Open();
            SqlCommand cmd = connect.CreateCommand();
            //Search by title
            SortedSet<string> topics = new SortedSet<string>();
            foreach (string word in words)
            {
                if (!string.IsNullOrWhiteSpace(word))
                {
                    cmd.CommandText = "select count(*) from topics where topic_title like '%" + word + "%' and topic_isDeleted = 0 and topic_isDenied = 0 and topic_isApproved = 1 ";
                    int countTopics = Convert.ToInt32(cmd.ExecuteScalar());
                    for (int i = 1; i <= countTopics; i++)
                    {
                        cmd.CommandText = "select [topicId] from(SELECT rowNum = ROW_NUMBER() OVER(ORDER BY topicId ASC), * FROM [topics] where topic_title like '%" + word + "%' and topic_isDeleted = 0 and topic_isDenied = 0 and topic_isApproved = 1) as t where rowNum = '" + i + "'";
                        string temp_Id = cmd.ExecuteScalar().ToString();
                        topics.Add(temp_Id);
                    }
                }
            }
            int count = topics.Count;
            for (int i = 0; i < count; i++)
            {
                //Get the topic ID:
                id = topics.ElementAt(i);
                //Get type:
                cmd.CommandText = "select [topic_time] from Topics where topicId = '" + id + "' ";
                time = cmd.ExecuteScalar().ToString();
                //Get title:
                cmd.CommandText = "select [topic_title] from Topics where topicId = '" + id + "' ";
                title = cmd.ExecuteScalar().ToString();
                //Get type:
                cmd.CommandText = "select [topic_type] from Topics where topicId = '" + id + "' ";
                type = cmd.ExecuteScalar().ToString();
                //Get creator's ID:
                cmd.CommandText = "select [topic_createdBy] from Topics where topicId = '" + id + "' ";
                string creatorId = cmd.ExecuteScalar().ToString();
                //Get creator's name:
                cmd.CommandText = "select user_firstname from users where userId = '" + creatorId + "' ";
                creator = cmd.ExecuteScalar().ToString();
                cmd.CommandText = "select user_lastname from users where userId = '" + creatorId + "' ";
                creator = creator + " " + cmd.ExecuteScalar().ToString();
                if (type.Equals("Consultation"))
                {
                    cmd.CommandText = "select userId from Users where loginId = '" + loginId + "' ";
                    string userId = cmd.ExecuteScalar().ToString();
                    int int_roleId = Convert.ToInt32(roleId);
                    if (int_roleId == 2)//2 = Physician
                    {
                        cmd.CommandText = "select count(*) from Consultations where topicId = '" + id + "' and physician_userId = '" + userId + "' ";
                        int exists = Convert.ToInt32(cmd.ExecuteScalar());
                        if (exists > 0)
                        {
                            dt.Rows.Add(title, "Title", Layouts.getTimeFormat(time), type, creator, id, creatorId);
                        }
                    }
                    else if (int_roleId == 3)//3 = Patient
                    {
                        cmd.CommandText = "select count(*) from Consultations where topicId = '" + id + "' and patient_userId = '" + userId + "' ";
                        int exists = Convert.ToInt32(cmd.ExecuteScalar());
                        if (exists > 0)
                        {
                            dt.Rows.Add(title, "Title", Layouts.getTimeFormat(time), type, creator, id, creatorId);
                        }
                    }
                    //Else will be the admin. If admin, just don't show anything about the consultation topics.
                }
                else
                    dt.Rows.Add(title, "Title", Layouts.getTimeFormat(time), type, creator, id, creatorId);
            }
            //Search by creator
            SortedSet<string> users = new SortedSet<string>();
            foreach (string word in words)
            {
                if (!string.IsNullOrWhiteSpace(word))
                {
                    cmd.CommandText = "select count(*) from users where (user_firstname+ ' ' +user_lastname) like '%" + word + "%' ";
                    int countUsers = Convert.ToInt32(cmd.ExecuteScalar());
                    for (int i = 1; i <= countUsers; i++)
                    {
                        cmd.CommandText = "select [userId] from(SELECT rowNum = ROW_NUMBER() OVER(ORDER BY userId ASC), * FROM [Users] where (user_firstname+ ' ' +user_lastname) like '%" + word + "%' ) as t where rowNum = '" + i + "'";
                        string temp_Id = cmd.ExecuteScalar().ToString();
                        users.Add(temp_Id);
                    }
                }
            }
            int totalUsers = users.Count;
            for (int i = 0; i < totalUsers; i++)
            {
                string temp_userId = users.ElementAt(i);
                cmd.CommandText = "select count(*) from topics where topic_createdBy = '" + temp_userId + "' and topic_isApproved = 1 and topic_isDenied = 0 and topic_isDeleted = 0";
                int totalTopicsForTempUser = Convert.ToInt32(cmd.ExecuteScalar());
                for (int j = 1; j <= totalTopicsForTempUser; j++)
                {
                    //Get the topic ID:
                    cmd.CommandText = "select [topicId] from(SELECT rowNum = ROW_NUMBER() OVER(ORDER BY topicId ASC), * FROM [Topics] where topic_createdBy = '" + temp_userId + "' and topic_isApproved = 1 and topic_isDenied = 0 and topic_isDeleted = 0) as t where rowNum = '" + j + "'";
                    id = cmd.ExecuteScalar().ToString();
                    //Get type:
                    cmd.CommandText = "select [topic_time] from Topics where topicId = '" + id + "' ";
                    time = cmd.ExecuteScalar().ToString();
                    //Get title:
                    cmd.CommandText = "select [topic_title] from Topics where topicId = '" + id + "' ";
                    title = cmd.ExecuteScalar().ToString();
                    //Get type:
                    cmd.CommandText = "select [topic_type] from Topics where topicId = '" + id + "' ";
                    type = cmd.ExecuteScalar().ToString();
                    //Get creator's ID:
                    cmd.CommandText = "select [topic_createdBy] from Topics where topicId = '" + id + "' ";
                    string creatorId = cmd.ExecuteScalar().ToString();
                    //Get creator's name:
                    cmd.CommandText = "select user_firstname from users where userId = '" + creatorId + "' ";
                    creator = cmd.ExecuteScalar().ToString();
                    cmd.CommandText = "select user_lastname from users where userId = '" + creatorId + "' ";
                    creator = creator + " " + cmd.ExecuteScalar().ToString();
                    if (type.Equals("Consultation"))
                    {
                        cmd.CommandText = "select userId from Users where loginId = '" + loginId + "' ";
                        string userId = cmd.ExecuteScalar().ToString();
                        int int_roleId = Convert.ToInt32(roleId);
                        if (int_roleId == 2)//2 = Physician
                        {
                            cmd.CommandText = "select count(*) from Consultations where topicId = '" + id + "' and physician_userId = '" + userId + "' ";
                            int exists = Convert.ToInt32(cmd.ExecuteScalar());
                            if (exists > 0)
                            {
                                dt.Rows.Add(title, "Creator Name", Layouts.getTimeFormat(time), type, creator, id, creatorId);
                            }
                        }
                        else if (int_roleId == 3)//3 = Patient
                        {
                            cmd.CommandText = "select count(*) from Consultations where topicId = '" + id + "' and patient_userId = '" + userId + "' ";
                            int exists = Convert.ToInt32(cmd.ExecuteScalar());
                            if (exists > 0)
                            {
                                dt.Rows.Add(title, "Creator Name", Layouts.getTimeFormat(time), type, creator, id, creatorId);
                            }
                        }
                        //Else will be the admin. If admin, just don't show anything about the consultation topics.
                    }
                    else
                        dt.Rows.Add(title, "Creator Name", Layouts.getTimeFormat(time), type, creator, id, creatorId);
                }
            }
            //Search by message text
            SortedSet<string> messages = new SortedSet<string>();
            foreach (string word in words)
            {
                if (!string.IsNullOrWhiteSpace(word))
                {
                    cmd.CommandText = "select count(*) from entries where entry_text like '%" + word + "%' and entry_isDeleted = 0 and entry_isApproved = 1 and entry_isDenied = 0 ";
                    int countMessages = Convert.ToInt32(cmd.ExecuteScalar());
                    for (int i = 1; i <= countMessages; i++)
                    {
                        cmd.CommandText = "select [topicId] from(SELECT rowNum = ROW_NUMBER() OVER(ORDER BY topicId ASC), * FROM [Entries] where entry_text like '%" + word + "%' and entry_isDeleted = 0 and entry_isApproved = 1 and entry_isDenied = 0) as t where rowNum = '" + i + "'";
                        string temp_Id = cmd.ExecuteScalar().ToString();
                        messages.Add(temp_Id);
                    }
                }
            }
            int totalMessages = messages.Count;
            for (int i = 0; i < totalMessages; i++)
            {
                //Get the topic ID:
                string new_id = messages.ElementAt(i);
                if (!new_id.Equals(id))
                {
                    id = new_id;
                    //Check if the topic of the selected message is deleted or not:
                    cmd.CommandText = "select topic_isDeleted from Topics where topicId = '" + id + "' ";
                    int isDeleted = Convert.ToInt32(cmd.ExecuteScalar());
                    if (isDeleted == 0)//0: False, meaning that the topic is not deleted
                    {
                        //Get type:
                        cmd.CommandText = "select [topic_time] from Topics where topicId = '" + id + "' ";
                        time = cmd.ExecuteScalar().ToString();
                        //Get title:
                        cmd.CommandText = "select [topic_title] from Topics where topicId = '" + id + "' ";
                        title = cmd.ExecuteScalar().ToString();
                        //Get type:
                        cmd.CommandText = "select [topic_type] from Topics where topicId = '" + id + "' ";
                        type = cmd.ExecuteScalar().ToString();
                        //Get creator's ID:
                        cmd.CommandText = "select [topic_createdBy] from Topics where topicId = '" + id + "' ";
                        string creatorId = cmd.ExecuteScalar().ToString();
                        //Get creator's name:
                        cmd.CommandText = "select user_firstname from users where userId = '" + creatorId + "' ";
                        creator = cmd.ExecuteScalar().ToString();
                        cmd.CommandText = "select user_lastname from users where userId = '" + creatorId + "' ";
                        creator = creator + " " + cmd.ExecuteScalar().ToString();
                        if (type.Equals("Consultation"))
                        {
                            cmd.CommandText = "select userId from Users where loginId = '" + loginId + "' ";
                            string userId = cmd.ExecuteScalar().ToString();
                            int int_roleId = Convert.ToInt32(roleId);
                            if (int_roleId == 2)//2 = Physician
                            {
                                cmd.CommandText = "select count(*) from Consultations where topicId = '" + id + "' and physician_userId = '" + userId + "' ";
                                int exists = Convert.ToInt32(cmd.ExecuteScalar());
                                if (exists > 0)
                                {
                                    dt.Rows.Add(title, "Message Text", Layouts.getTimeFormat(time), type, creator, id, creatorId);
                                }
                            }
                            else if (int_roleId == 3)//3 = Patient
                            {
                                cmd.CommandText = "select count(*) from Consultations where topicId = '" + id + "' and patient_userId = '" + userId + "' ";
                                int exists = Convert.ToInt32(cmd.ExecuteScalar());
                                if (exists > 0)
                                {
                                    dt.Rows.Add(title, "Message Text", Layouts.getTimeFormat(time), type, creator, id, creatorId);
                                }
                            }
                            //Else will be the admin. If admin, just don't show anything about the consultation topics.
                        }
                        else
                            dt.Rows.Add(title, "Message Text", Layouts.getTimeFormat(time), type, creator, id, creatorId);
                    }
                }
            }
            dt = removeDuplicateRows(dt, "Time");
            connect.Close();
            grdResults.DataSource = dt;
            grdResults.DataBind();
            grdResults.Visible = true;
            rebindValues();
        }
        protected void createTimePeriodTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Title", typeof(string));
            dt.Columns.Add("Found in", typeof(string));
            dt.Columns.Add("Time", typeof(string));
            dt.Columns.Add("Type", typeof(string));
            dt.Columns.Add("Creator", typeof(string));
            dt.Columns.Add("Topic ID", typeof(string));
            dt.Columns.Add("User ID", typeof(string));
            string id = "", title = "", type = "", creator = "", time = "";
            string searchString = txtSearch.Text.Replace("'", "''");
            int count = 0;
            connect.Open();
            SqlCommand cmd = connect.CreateCommand();
            DateTime start_time = calFrom.SelectedDate, end_time = calTo.SelectedDate;
            if (!start_time.ToString().Equals("1/1/0001 12:00:00 AM") && !end_time.ToString().Equals("1/1/0001 12:00:00 AM"))
            {
                cmd.CommandText = "select count(*) from topics where topic_time >= '" + start_time + "' and topic_time <= '" + end_time + "' and topic_isDeleted = 0 and topic_isDenied = 0 and topic_isApproved = 1 ";
                count = Convert.ToInt32(cmd.ExecuteScalar());
            }
            for (int i = 1; i <= count; i++)
            {
                //Get the topic ID:
                cmd.CommandText = "select [topicId] from(SELECT rowNum = ROW_NUMBER() OVER(ORDER BY topicId ASC), * FROM [Topics] where topic_time >= '" + calFrom.SelectedDate + "' and topic_time <= '" + calTo.SelectedDate + "' and topic_isApproved = 1 and topic_isDenied = 0 and topic_isDeleted = 0) as t where rowNum = '" + i + "'";
                id = cmd.ExecuteScalar().ToString();
                //Get type:
                cmd.CommandText = "select [topic_time] from Topics where topicId = '" + id + "' ";
                time = cmd.ExecuteScalar().ToString();
                //Get title:
                cmd.CommandText = "select [topic_title] from Topics where topicId = '" + id + "' ";
                title = cmd.ExecuteScalar().ToString();
                //Get type:
                cmd.CommandText = "select [topic_type] from Topics where topicId = '" + id + "' ";
                type = cmd.ExecuteScalar().ToString();
                //Get creator's ID:
                cmd.CommandText = "select [topic_createdBy] from Topics where topicId = '" + id + "' ";
                string creatorId = cmd.ExecuteScalar().ToString();
                //Get creator's name:
                cmd.CommandText = "select user_firstname from users where userId = '" + creatorId + "' ";
                creator = cmd.ExecuteScalar().ToString();
                cmd.CommandText = "select user_lastname from users where userId = '" + creatorId + "' ";
                creator = creator + " " + cmd.ExecuteScalar().ToString();
                if (type.Equals("Consultation"))
                {
                    cmd.CommandText = "select userId from Users where loginId = '" + loginId + "' ";
                    string userId = cmd.ExecuteScalar().ToString();
                    int int_roleId = Convert.ToInt32(roleId);
                    if (int_roleId == 2)//2 = Physician
                    {
                        cmd.CommandText = "select count(*) from Consultations where topicId = '" + id + "' and physician_userId = '" + userId + "' ";
                        int exists = Convert.ToInt32(cmd.ExecuteScalar());
                        if (exists > 0)
                        {
                            dt.Rows.Add(title, "Time Period", Layouts.getTimeFormat(time), type, creator, id, creatorId);
                        }
                    }
                    else if (int_roleId == 3)//3 = Patient
                    {
                        cmd.CommandText = "select count(*) from Consultations where topicId = '" + id + "' and patient_userId = '" + userId + "' ";
                        int exists = Convert.ToInt32(cmd.ExecuteScalar());
                        if (exists > 0)
                        {
                            dt.Rows.Add(title, "Time Period", Layouts.getTimeFormat(time), type, creator, id, creatorId);
                        }
                    }
                    //Else will be the admin. If admin, just don't show anything about the consultation topics.
                }
                else
                    dt.Rows.Add(title, "Time Period", Layouts.getTimeFormat(time), type, creator, id, creatorId);
            }
            connect.Close();
            grdResults.DataSource = dt;
            grdResults.DataBind();
            grdResults.Visible = true;
            rebindValues();
        }
        public DataTable removeDuplicateRows(DataTable dTable, string colName)
        {
            Hashtable hTable = new Hashtable();
            ArrayList duplicateList = new ArrayList();
            try
            {
                for (int rowIndex = 0; rowIndex < dTable.Rows.Count; rowIndex++)
                {
                    string temp_title = dTable.Rows[rowIndex][0].ToString();
                    string temp_time = dTable.Rows[rowIndex][2].ToString();
                    string txt_foundIn = dTable.Rows[rowIndex][1].ToString();
                    string temp_foundIn = "";
                    string title_toBeRemoved = "";
                    string time_toBeRemoved = "";
                    for (int j = 0; j < dTable.Rows.Count; j++)
                    {
                        string new_title = dTable.Rows[j][0].ToString();
                        string new_time = dTable.Rows[j][2].ToString();
                        if (new_title.Equals(temp_title) && new_time.Equals(temp_time))
                        {
                            string current_tempFoundIn = "";
                            if (string.IsNullOrWhiteSpace(temp_foundIn))
                            {
                                temp_foundIn = dTable.Rows[j][1].ToString();
                                current_tempFoundIn = temp_foundIn;
                            }
                            if (!string.IsNullOrWhiteSpace(temp_foundIn) && !current_tempFoundIn.Equals(temp_foundIn))
                            {
                                temp_foundIn = temp_foundIn + ", " + dTable.Rows[j][1].ToString();
                            }
                            title_toBeRemoved = new_title;
                            time_toBeRemoved = new_time;
                        }
                    }
                    if (!string.IsNullOrWhiteSpace(temp_foundIn))
                        txt_foundIn = temp_foundIn;
                    dTable.Rows[rowIndex][1] = txt_foundIn;
                }
                //Add list of all the unique item value to hashtable, which stores combination of key, value pair.
                //And add duplicate item value in arraylist.
                foreach (DataRow drow in dTable.Rows)
                {
                    if (hTable.Contains(drow[colName]))
                        duplicateList.Add(drow);
                    else
                    {
                        hTable.Add(drow[colName], string.Empty);
                    }
                }
                //Removing a list of duplicate items from datatable.
                foreach (DataRow dRow in duplicateList)
                    dTable.Rows.Remove(dRow);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                //lblErrorMessage.Text = e.ToString();
                //lblErrorMessage.Visible = true;
            }
            //Datatable which contains unique records will be return as output.
            return dTable;
        }
    }
}