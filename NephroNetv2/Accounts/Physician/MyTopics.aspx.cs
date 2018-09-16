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
    public partial class MyTopics : System.Web.UI.Page
    {

        static string conn = "";
        SqlConnection connect = new SqlConnection(conn);
        string username, roleId, loginId, token;
        protected void Page_Load(object sender, EventArgs e)
        {
            initialPageAccess();
            int countNewTopics = getTotalApprovedTopics();
            if (countNewTopics > 0)
            {
                lblMessage.Visible = false;
                createTable(countNewTopics);
            }
            else if (countNewTopics == 0)
            {
                lblMessage.Visible = true;
            }
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
        protected int getTotalApprovedTopics()
        {
            int count = 0;
            connect.Open();
            SqlCommand cmd = connect.CreateCommand();
            //Get the current user ID:
            cmd.CommandText = "select userId from users where loginId = '" + loginId + "' ";
            string userId = cmd.ExecuteScalar().ToString();
            int int_roleId = Convert.ToInt32(roleId);
            if(int_roleId == 2)//If the current user is a physician:
            {
                //count the consultation topics for the current user:
                cmd.CommandText = "select count(*) from Consultations where physician_userId = '" + userId + "' ";
                count = Convert.ToInt32(cmd.ExecuteScalar());
            }
            else if (int_roleId == 3)//If the current user is a patient:
            {
                //count the consultation topics for the current user:
                cmd.CommandText = "select count(*) from Consultations where patient_userId = '" + userId + "' ";
                count = Convert.ToInt32(cmd.ExecuteScalar());
            }
            connect.Close();
            return count;
        }
        protected void createTable(int count)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("ID", typeof(string));
            dt.Columns.Add("Title", typeof(string));
            dt.Columns.Add("Time Created", typeof(string));
            dt.Columns.Add("Current Participants", typeof(string));
            string id = "", title = "", type = "", time = "";
            int int_roleId = Convert.ToInt32(roleId);
            connect.Open();
            SqlCommand cmd = connect.CreateCommand();
            //Get the current user ID:
            cmd.CommandText = "select userId from users where loginId = '" + loginId + "' ";
            string userId = cmd.ExecuteScalar().ToString();
            for (int i = 1; i <= count; i++)
            {
                if (int_roleId == 2)//If the current user is a physician:
                {
                    //Get the topic ID:
                    cmd.CommandText = "select [topicId] from(SELECT rowNum = ROW_NUMBER() OVER(ORDER BY consultationId ASC), * FROM [Consultations] where physician_userId = '" + userId + "' ) as t where rowNum = '" + i + "'";
                    id = cmd.ExecuteScalar().ToString();
                }
                else if (int_roleId == 3)//If the current user is a patient:
                {
                    //Get the topic ID:
                    cmd.CommandText = "select [topicId] from(SELECT rowNum = ROW_NUMBER() OVER(ORDER BY consultationId ASC), * FROM [Consultations] where patient_userId = '" + userId + "' ) as t where rowNum = '" + i + "'";
                    id = cmd.ExecuteScalar().ToString();
                }
                cmd.CommandText = "select topic_isTerminated from topics where topicId = '" + id + "' ";
                int isTerminated = Convert.ToInt32(cmd.ExecuteScalar());
                cmd.CommandText = "select topic_isDeleted from topics where topicId = '" + id + "' ";
                int isDeleted = Convert.ToInt32(cmd.ExecuteScalar());
                if (isTerminated == 0 && isDeleted == 0)// 0 = false. Meaning that the topic is not terminated; therefore, show it in the list of my topics:
                {
                    //Get type:
                    cmd.CommandText = "select [topic_time] from topics where topicId = '" + id + "' ";
                    time = cmd.ExecuteScalar().ToString();
                    //Get title:
                    cmd.CommandText = "select [topic_title] from topics where topicId = '" + id + "' ";
                    title = cmd.ExecuteScalar().ToString();
                    //Get type:
                    cmd.CommandText = "select [topic_type] from topics where topicId = '" + id + "' ";
                    type = cmd.ExecuteScalar().ToString();
                    //Get creator's ID:
                    cmd.CommandText = "select [topic_createdBy] from topics where topicId = '" + id + "' ";
                    string creatorId = cmd.ExecuteScalar().ToString();
                    //dt.Rows.Add(id, title, Layouts.getTimeFormat(time), participantLink);
                    cmd.CommandText = "select patient_userId from [Consultations] where topicId = '" + id + "' ";
                    string patientId = cmd.ExecuteScalar().ToString();
                    cmd.CommandText = "select (user_firstname + ' ' + user_lastname) from Users where userId = '" + patientId + "' ";
                    string patientName = cmd.ExecuteScalar().ToString();
                    cmd.CommandText = "select physician_userId from [Consultations] where topicId = '" + id + "' ";
                    string physicianId = cmd.ExecuteScalar().ToString();
                    cmd.CommandText = "select (user_firstname + ' ' + user_lastname) from Users where userId = '" + physicianId + "' ";
                    string physicianName = cmd.ExecuteScalar().ToString();
                    dt.Rows.Add(id, title, Layouts.getTimeFormat(time), physicianName + ", " + patientName);
                }
            }

            grdTopics.DataSource = dt;
            grdTopics.DataBind();
            //Hide the header called "ID":
            grdTopics.HeaderRow.Cells[1].Visible = false;
            //Hide IDs column and content which are located in column index 1:
            for (int i = 0; i < grdTopics.Rows.Count; i++)
            {
                grdTopics.Rows[i].Cells[1].Visible = false;
            }
            //for (int row = 0; row < grdTopics.Rows.Count; row++)
            //{
            //    id = grdTopics.Rows[row].Cells[1].Text;
            //    //Get total approved participants for a topic:                
            //    cmd.CommandText = "select count(*) from Consultations where topicId = '" + id + "' ";
            //    int totalApprovedParticipants = Convert.ToInt32(cmd.ExecuteScalar());
            //    for (int j = 1; j <= totalApprovedParticipants; j++)
            //    {
            //        HyperLink patientLink = new HyperLink();
            //        cmd.CommandText = "select patient_userId from(SELECT rowNum = ROW_NUMBER() OVER(ORDER BY consultationId ASC), * FROM [Consultations] where topicId = '" + id + "') as t where rowNum = '" + j + "'";
            //        string patientId = cmd.ExecuteScalar().ToString();
            //        cmd.CommandText = "select (user_firstname + ' ' + user_lastname) from Users where userId = '" + patientId + "' ";
            //        string patient_name = cmd.ExecuteScalar().ToString();
            //        patientLink.Text = patient_name + " ";
            //        //participantLink.NavigateUrl = "Profile.aspx?id=" + patientId;
            //        grdTopics.Rows[row].Cells[4].Controls.Add(patientLink);
            //        if (totalApprovedParticipants > 1)
            //        {
            //            HyperLink temp = new HyperLink();
            //            temp.Text = "<br/>";
            //            grdTopics.Rows[row].Cells[4].Controls.Add(temp);
            //        }
            //        HyperLink physicianLink = new HyperLink();
            //        cmd.CommandText = "select physician_userId from(SELECT rowNum = ROW_NUMBER() OVER(ORDER BY consultationId ASC), * FROM [Consultations] where topicId = '" + id + "') as t where rowNum = '" + j + "'";
            //        string physicianId = cmd.ExecuteScalar().ToString();
            //        cmd.CommandText = "select (user_firstname + ' ' + user_lastname) from Users where userId = '" + physicianId + "' ";
            //        string physicianName = cmd.ExecuteScalar().ToString();
            //        physicianLink.Text = physicianName + " ";
            //        //participantLink.NavigateUrl = "Profile.aspx?id=" + patientId;
            //        grdTopics.Rows[row].Cells[4].Controls.Add(patientLink);
            //        if (totalApprovedParticipants > 1)
            //        {
            //            HyperLink temp = new HyperLink();
            //            temp.Text = "<br/>";
            //            grdTopics.Rows[row].Cells[4].Controls.Add(temp);
            //        }
            //    }
            //    if (totalApprovedParticipants == 0)
            //        grdTopics.Rows[row].Cells[4].Text = "There are no participants";

            //}
            connect.Close();
        }
        protected void grdTopics_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            grdTopics.PageIndex = e.NewPageIndex;
            grdTopics.DataBind();
            //Hide the header called "ID":
            grdTopics.HeaderRow.Cells[1].Visible = false;
            //Hide IDs column and content which are located in column index 1:
            for (int i = 0; i < grdTopics.Rows.Count; i++)
            {
                grdTopics.Rows[i].Cells[1].Visible = false;
            }
            //rebindValues();
        }
        protected void rebindValues()
        {
            connect.Open();
            SqlCommand cmd = connect.CreateCommand();
            string id = "";
            for (int row = 0; row < grdTopics.Rows.Count; row++)
            {
                id = grdTopics.Rows[row].Cells[1].Text;
                //Get total approved participants for a topic:                
                cmd.CommandText = "select count(*) from Consultations where topicId = '" + id + "' ";
                int totalApprovedParticipants = Convert.ToInt32(cmd.ExecuteScalar());
                for (int j = 1; j <= totalApprovedParticipants; j++)
                {
                    HyperLink participantLink = new HyperLink();
                    cmd.CommandText = "select patient_userId from(SELECT rowNum = ROW_NUMBER() OVER(ORDER BY consultationId ASC), * FROM [Consultations] where topicId = '" + id + "') as t where rowNum = '" + j + "'";
                    string patientId = cmd.ExecuteScalar().ToString();
                    cmd.CommandText = "select (user_firstname + ' ' + user_lastname) from Users where userId = '" + patientId + "' ";
                    string patient_name = cmd.ExecuteScalar().ToString();
                    participantLink.Text = patient_name + " ";
                    //participantLink.NavigateUrl = "Profile.aspx?id=" + patientId;
                    grdTopics.Rows[row].Cells[4].Controls.Add(participantLink);
                    if (totalApprovedParticipants > 1)
                    {
                        HyperLink temp = new HyperLink();
                        temp.Text = "<br/>";
                        grdTopics.Rows[row].Cells[4].Controls.Add(temp);
                    }
                    HyperLink physicianLink = new HyperLink();
                    cmd.CommandText = "select physician_userId from(SELECT rowNum = ROW_NUMBER() OVER(ORDER BY consultationId ASC), * FROM [Consultations] where topicId = '" + id + "') as t where rowNum = '" + j + "'";
                    string physicianId = cmd.ExecuteScalar().ToString();
                    cmd.CommandText = "select (user_firstname + ' ' + user_lastname) from Users where userId = '" + physicianId + "' ";
                    string physicianName = cmd.ExecuteScalar().ToString();
                    physicianLink.Text = patient_name + " ";
                    //participantLink.NavigateUrl = "Profile.aspx?id=" + patientId;
                    grdTopics.Rows[row].Cells[4].Controls.Add(physicianLink);
                    if (totalApprovedParticipants > 1)
                    {
                        HyperLink temp = new HyperLink();
                        temp.Text = "<br/>";
                        grdTopics.Rows[row].Cells[4].Controls.Add(temp);
                    }
                }
                if (totalApprovedParticipants == 0)
                    grdTopics.Rows[row].Cells[4].Text = "There are no participants";

            }
            connect.Close();
        }
    }
}