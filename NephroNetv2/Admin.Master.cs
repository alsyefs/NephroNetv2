using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NephroNet
{
    public partial class AdminMaster : MasterPage
    {
        static string conn = "";
        SqlConnection connect = new SqlConnection(conn);
        string username, roleId, loginId, token;
        protected void Page_Load(object sender, EventArgs e)
        {
            initialAccess();
        }
        protected void initialAccess()
        {
            Configuration config = new Configuration();
            conn = config.getConnectionString();
            connect = new SqlConnection(conn);
            getSession();
            bool correctSession = sessionIsCorrect(username, roleId, token);
            if (!correctSession)
                clearSession();
            lblAlerts.Text = "Alerts (" + countTotalAlerts() + ")";
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
        public Boolean sessionIsCorrect(string temp_username, string temp_roleId, string temp_token)
        {
            username = temp_username;
            roleId = temp_roleId;
            token = temp_token;

            Boolean correctSession = true;
            Boolean isEmptySession = checkIfSessionIsEmpty();
            if (isEmptySession)
                correctSession = false;
            Boolean correctSessionValues = checkSeesionValues();
            if (correctSessionValues == false)
            {
                correctSession = false;
            }
            return correctSession;
        }
        public int countTotalAlerts()
        {            
            connect.Open();
            SqlCommand cmd = connect.CreateCommand();
            //count users to be approved:
            cmd.CommandText = "select count(*) from registrations";
            int count = Convert.ToInt32(cmd.ExecuteScalar());
            //count topics to be approved:
            cmd.CommandText = "select count(*) from Topics where topic_isApproved = 0 and topic_isDenied = 0 and topic_isTerminated = 0";
            count = count + Convert.ToInt32(cmd.ExecuteScalar());
            //count messages to be approved:
            //count messages that are not approved and have not been denied.
            cmd.CommandText = "select count(*) from [Entries] where entry_isApproved = 0 and entry_isDenied = 0 and entry_isDeleted = 0";
            int totalCount = Convert.ToInt32(cmd.ExecuteScalar());
            for (int i = 1; i <= totalCount; i++)
            {
                //Get the Message ID:
                cmd.CommandText = "select [entryId] from(SELECT rowNum = ROW_NUMBER() OVER(ORDER BY entryId ASC), * FROM [Entries] where entry_isApproved = 0 and entry_isDenied = 0 and entry_isDeleted = 0) as t where rowNum = '" + i + "'";
                string id = cmd.ExecuteScalar().ToString();
                //Get topic ID for the selected message:
                cmd.CommandText = "select [topicId] from [Entries] where entryId = '" + id + "' ";
                string topic = cmd.ExecuteScalar().ToString();
                cmd.CommandText = "select [topic_isDeleted] from [Topics] where topicId = '" + topic + "' ";
                int topic_isDeleted = Convert.ToInt32(cmd.ExecuteScalar());
                cmd.CommandText = "select [topic_isTerminated] from [Topics] where topicId = '" + topic + "' ";
                int topic_isTerminated = Convert.ToInt32(cmd.ExecuteScalar());
                if (topic_isDeleted == 0 && topic_isTerminated == 0)
                    count++;
            }
            connect.Close();
            return count;
        }
        protected Boolean checkSeesionValues()
        {
            Boolean correct = true;
            connect.Open();
            SqlCommand cmd = connect.CreateCommand();
            cmd.CommandText = "select count(*) from logins where login_username like '" + username + "' and roleId = '" + roleId + "' ";
            int countValues = Convert.ToInt32(cmd.ExecuteScalar());
            if (countValues < 1)//session has wrong values for any role.
                correct = false;
            cmd.CommandText = "select count(*) from logins where login_username like '" + username + "' and login_token like '" + token + "' and roleId = '" + roleId + "' ";
            int countTokenValues = Convert.ToInt32(cmd.ExecuteScalar());
            if (countTokenValues < 1)//session has wrong values for any role with the recieved token.
                correct = false;
            connect.Close();
            return correct;
        }
        protected Boolean checkIfSessionIsEmpty()
        {
            Boolean itIsEmpty = false;
            if (string.IsNullOrWhiteSpace(username) || (!roleId.Equals("1")))//if no session values for either username or roleId, set to false.
            {
                itIsEmpty = true;
            }
            return itIsEmpty;
        }
    }
}