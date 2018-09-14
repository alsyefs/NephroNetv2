using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NephroNet.Accounts.Admin
{
    public partial class TerminateAccount : System.Web.UI.Page
    {
        static string conn = "";
        SqlConnection connect = new SqlConnection(conn);
        string username, roleId, loginId, token;
        string accountId = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            initialPageAccess();
            accountId = Request.QueryString["id"];
            bool accountIdExists = isAccountCorrect();
            if (!accountIdExists)
                closePage();
        }
        protected bool isAccountCorrect()
        {
            bool correct = true;
            CheckErrors errors = new CheckErrors();
            //check if id contains a special character:
            if (!errors.isDigit(accountId))
                correct = false;
            //check if id contains an id that does not exist in DB:
            else if (errors.ContainsSpecialChars(accountId))
                correct = false;
            if (correct)
            {
                connect.Open();
                SqlCommand cmd = connect.CreateCommand();
                //Count the existance of the user:
                cmd.CommandText = "select count(*) from Users where userId = '" + accountId + "' ";
                int count = Convert.ToInt32(cmd.ExecuteScalar());
                if (count > 0)//if count > 0, then the user ID exists in DB.
                {
                    //Get the current user's ID who is trying to access the profile:
                    cmd.CommandText = "select userId from Users where loginId = '" + loginId + "' ";
                    string current_userId = cmd.ExecuteScalar().ToString();
                    cmd.CommandText = "select loginId from users where userId = '" + accountId + "' ";
                    string account_loginId = cmd.ExecuteScalar().ToString();
                    cmd.CommandText = "select login_isActive from Logins where loginId = '"+ account_loginId + "' ";
                    int isActive = Convert.ToInt32(cmd.ExecuteScalar());
                    if (isActive == 0)
                        correct = false;
                    //Maybe later use the current user's ID to check if the current user has access to view the selected profile.
                    if (account_loginId == loginId)
                        correct = false;
                }
                else
                    correct = false; // means that the user ID does not exists in DB.
                connect.Close();
            }
            return correct;
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
        protected void btnTerminate_Click(object sender, EventArgs e)
        {
            bool accountIdExists = isAccountCorrect();
            if (accountIdExists)
            {
                connect.Open();
                SqlCommand cmd = connect.CreateCommand();
                cmd.CommandText = "select loginId from users where userId = '"+accountId+"' ";
                string account_loginId = cmd.ExecuteScalar().ToString();
                //update the DB and set isActive = false:
                cmd.CommandText = "update Logins set login_isActive = 0 where loginId = '" + account_loginId + "' ";
                cmd.ExecuteScalar();
                connect.Close();
                //Email the topic creator about the topic being deleted:
                sendEmailNotification();
            }
            closePage();
        }
        protected void sendEmailNotification()
        {
            connect.Open();
            SqlCommand cmd = connect.CreateCommand();
            cmd.CommandText = "select user_firstname from Users where userId = '" + accountId + "' ";
            string name = cmd.ExecuteScalar().ToString();
            cmd.CommandText = "select user_lastname from Users where userId = '" + accountId + "' ";
            name = name + " " + cmd.ExecuteScalar().ToString();
            cmd.CommandText = "select user_email from Users where userId = '" + accountId + "' ";
            string emailTo = cmd.ExecuteScalar().ToString();
            connect.Close();
            string emailBody = "Hello " + name + ",\n\n" +
                "This email is to inform you that your account has been terminated. If you think this happened by mistake, or you did not perform this action, plaese contact the support.\n\n" +
                "Best regards,\nNephroNet Support\nNephroNet2018@gmail.com";
            Email email = new Email();
            email.sendEmail(emailTo, emailBody);
        }
    }
}