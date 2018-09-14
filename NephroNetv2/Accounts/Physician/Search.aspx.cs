using System;
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
            CheckPhysicianSession session = new CheckPhysicianSession();
            bool correctSession = session.sessionIsCorrect(username, roleId, token);
            if (!correctSession)
                clearSession();
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
        protected int countResults()
        {
            int count = 0;
            string searchString = txtSearch.Text.Replace("'", "''");
            connect.Open();
            SqlCommand cmd = connect.CreateCommand();
            if (drpSearch.SelectedIndex == 1)//Searching topics by topic titles
            {
                cmd.CommandText = "select count(*) from topics where topic_title like '%" + searchString + "%' and topic_isDeleted = 0 and topic_isDenied = 0 and topic_isApproved = 1 ";
                count = Convert.ToInt32(cmd.ExecuteScalar());
            }
            else if (drpSearch.SelectedIndex == 2)//Searching topics by users' fullnames
            {
                cmd.CommandText = "select count(*) from users where (user_firstname+ ' ' +user_lastname) like '%" + searchString + "%' ";
                int totalUsers = Convert.ToInt32(cmd.ExecuteScalar());
                for (int i = 1; i <= totalUsers; i++)
                {
                    cmd.CommandText = "select [userId] from(SELECT rowNum = ROW_NUMBER() OVER(ORDER BY userId ASC), * FROM [Users] where (user_firstname+ ' ' +user_lastname) like '%" + searchString + "%' ) as t where rowNum = '" + i + "'";
                    string temp_userId = cmd.ExecuteScalar().ToString();
                    cmd.CommandText = "select count(*) from topics where topic_createdBy = '" + temp_userId + "' ";
                    int totalTopicsForTempUser = Convert.ToInt32(cmd.ExecuteScalar());
                    count += totalTopicsForTempUser;
                }
            }
            else if (drpSearch.SelectedIndex == 3)//Searching topics by messages
            {
                cmd.CommandText = "select * from entries where entry_text like '%" + searchString + "%' and entry_isDeleted = 0 and entry_isApproved = 1 and entry_isDenied = 0 ";
                count = Convert.ToInt32(cmd.ExecuteScalar());
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
                cmd.CommandText = "select count(*) from topics where topic_title like '%" + searchString + "%' and topic_isDeleted = 0 and topic_isDenied = 0 and topic_isApproved = 1 ";
                count = Convert.ToInt32(cmd.ExecuteScalar());
                cmd.CommandText = "select count(*) from users where (user_firstname+ ' ' +user_lastname) like '%" + searchString + "%' ";
                int totalUsers = Convert.ToInt32(cmd.ExecuteScalar());
                for (int i = 1; i <= totalUsers; i++)
                {
                    cmd.CommandText = "select [userId] from(SELECT rowNum = ROW_NUMBER() OVER(ORDER BY userId ASC), * FROM [Users] where (user_firstname+ ' ' +user_lastname) like '%" + searchString + "%' ) as t where rowNum = '" + i + "'";
                    string temp_userId = cmd.ExecuteScalar().ToString();
                    cmd.CommandText = "select count(*) from topics where topic_createdBy = '" + temp_userId + "' ";
                    int totalTopicsForTempUser = Convert.ToInt32(cmd.ExecuteScalar());
                    count += totalTopicsForTempUser;
                }
                cmd.CommandText = "select * from entries where entry_text like '%" + searchString + "%' ";
                count += Convert.ToInt32(cmd.ExecuteScalar());
            }
            connect.Close();
            return count;
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
        protected void rebindValues()
        {
            connect.Open();
            SqlCommand cmd = connect.CreateCommand();
            string id = "", title = "", creator = "";
            string searchString = txtSearch.Text.Replace("'", "''");
            for (int row = 0; row < grdResults.Rows.Count; row++)
            {
                //Set the creator's link
                creator = grdResults.Rows[row].Cells[4].Text;
                cmd.CommandText = "select userId from Users where (user_firstname +' '+ user_lastname) like '" + creator + "' ";
                string creatorId = cmd.ExecuteScalar().ToString();
                HyperLink creatorLink = new HyperLink();
                creatorLink.Text = creator + " ";
                creatorLink.NavigateUrl = "Profile.aspx?id=" + creatorId;
                grdResults.Rows[row].Cells[4].Controls.Add(creatorLink);
                //Set the title's link
                title = grdResults.Rows[row].Cells[0].Text;
                cmd.CommandText = "select topicId from topics where topic_title like '" + title + "' ";
                id = cmd.ExecuteScalar().ToString();
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
            string id = "", title = "", type = "", creator = "", time = "";
            string searchString = txtSearch.Text.Replace("'", "''");
            connect.Open();
            SqlCommand cmd = connect.CreateCommand();
            cmd.CommandText = "select count(*) from topics where topic_title like '%" + searchString + "%' and topic_isDeleted = 0 and topic_isDenied = 0 and topic_isApproved = 1 ";
            int count = Convert.ToInt32(cmd.ExecuteScalar());
            for (int i = 1; i <= count; i++)
            {
                //Get the topic ID:
                cmd.CommandText = "select [topicId] from(SELECT rowNum = ROW_NUMBER() OVER(ORDER BY topicId ASC), * FROM [Topics] where topic_title like '%" + searchString + "%' and topic_isApproved = 1 and topic_isDenied = 0 and topic_isDeleted = 0) as t where rowNum = '" + i + "'";
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
                dt.Rows.Add(title, "Title", Layouts.getTimeFormat(time), type, creator);
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
            string id = "", title = "", type = "", creator = "", time = "";
            string searchString = txtSearch.Text.Replace("'", "''");
            connect.Open();
            SqlCommand cmd = connect.CreateCommand();
            cmd.CommandText = "select count(*) from users where (user_firstname+ ' ' +user_lastname) like '%" + searchString + "%' ";
            int totalUsers = Convert.ToInt32(cmd.ExecuteScalar());
            for (int i = 1; i <= totalUsers; i++)
            {
                cmd.CommandText = "select [userId] from(SELECT rowNum = ROW_NUMBER() OVER(ORDER BY userId ASC), * FROM [Users] where (user_firstname+ ' ' +user_lastname) like '%" + searchString + "%' ) as t where rowNum = '" + i + "'";
                string temp_userId = cmd.ExecuteScalar().ToString();
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
                    dt.Rows.Add(title, "Creator name", Layouts.getTimeFormat(time), type, creator);
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
            string id = "", title = "", type = "", creator = "", time = "";
            string searchString = txtSearch.Text.Replace("'", "''");
            connect.Open();
            SqlCommand cmd = connect.CreateCommand();
            cmd.CommandText = "select count(*) from entries where entry_text like '%" + searchString + "%' and entry_isDeleted = 0 and entry_isApproved = 1 and entry_isDenied = 0 ";
            int count = Convert.ToInt32(cmd.ExecuteScalar());
            for (int i = 1; i <= count; i++)
            {
                //Get the topic ID:
                cmd.CommandText = "select [topicId] from(SELECT rowNum = ROW_NUMBER() OVER(ORDER BY topicId ASC), * FROM [Entries] where entry_text like '%" + searchString + "%' and entry_isDeleted = 0 and entry_isApproved = 1 and entry_isDenied = 0) as t where rowNum = '" + i + "'";
                string new_id = cmd.ExecuteScalar().ToString();
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
                        dt.Rows.Add(title, "Message text", Layouts.getTimeFormat(time), type, creator);
                    }
                }
            }
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
                dt.Rows.Add(title, "Time period", Layouts.getTimeFormat(time), type, creator);
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
            string id = "", title = "", type = "", creator = "", time = "";
            string searchString = txtSearch.Text.Replace("'", "''");
            connect.Open();
            SqlCommand cmd = connect.CreateCommand();
            //Search by title
            cmd.CommandText = "select count(*) from topics where topic_title like '%" + searchString + "%' and topic_isDeleted = 0 and topic_isDenied = 0 and topic_isApproved = 1 ";
            int count = Convert.ToInt32(cmd.ExecuteScalar());
            for (int i = 1; i <= count; i++)
            {
                //Get the topic ID:
                cmd.CommandText = "select [topicId] from(SELECT rowNum = ROW_NUMBER() OVER(ORDER BY topicId ASC), * FROM [Topics] where topic_title like '%" + searchString + "%' and topic_isApproved = 1 and topic_isDenied = 0 and topic_isDeleted = 0) as t where rowNum = '" + i + "'";
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
                dt.Rows.Add(title, "Title", Layouts.getTimeFormat(time), type, creator);
            }
            //Search by creator
            cmd.CommandText = "select count(*) from users where (user_firstname+ ' ' +user_lastname) like '%" + searchString + "%' ";
            int totalUsers = Convert.ToInt32(cmd.ExecuteScalar());
            for (int i = 1; i <= totalUsers; i++)
            {
                cmd.CommandText = "select [userId] from(SELECT rowNum = ROW_NUMBER() OVER(ORDER BY userId ASC), * FROM [Users] where (user_firstname+ ' ' +user_lastname) like '%" + searchString + "%' ) as t where rowNum = '" + i + "'";
                string temp_userId = cmd.ExecuteScalar().ToString();
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
                    dt.Rows.Add(title, "Creator name", Layouts.getTimeFormat(time), type, creator);
                }
            }
            //Search by message text
            cmd.CommandText = "select count(*) from entries where entry_text like '%" + searchString + "%' and entry_isDeleted = 0 and entry_isApproved = 1 and entry_isDenied = 0 ";
            count = Convert.ToInt32(cmd.ExecuteScalar());
            for (int i = 1; i <= count; i++)
            {
                //Get the topic ID:
                cmd.CommandText = "select [topicId] from(SELECT rowNum = ROW_NUMBER() OVER(ORDER BY topicId ASC), * FROM [Entries] where entry_text like '%" + searchString + "%' and entry_isDeleted = 0 and entry_isApproved = 1 and entry_isDenied = 0) as t where rowNum = '" + i + "'";
                string new_id = cmd.ExecuteScalar().ToString();
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
                        dt.Rows.Add(title, "Message text", Layouts.getTimeFormat(time), type, creator);
                    }
                }
            }
            connect.Close();
            grdResults.DataSource = dt;
            grdResults.DataBind();
            grdResults.Visible = true;
            rebindValues();
        }
    }
}