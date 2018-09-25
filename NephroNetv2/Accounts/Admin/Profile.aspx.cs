using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NephroNet.Accounts.Admin
{
    public partial class Profile : System.Web.UI.Page
    {
        static string conn = "";
        SqlConnection connect = new SqlConnection(conn);
        static string previousPage = "";
        static string currentPage = "";
        static bool requestedTerminateOrUnlockAccount = false;
        string username, roleId, loginId, token;
        static string g_loginId = "";
        string profileId = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (!requestedTerminateOrUnlockAccount)
                {
                    if (HttpContext.Current.Request.Url.AbsoluteUri != null) currentPage = HttpContext.Current.Request.Url.AbsoluteUri;
                    else currentPage = "Home.aspx";
                    if (Request.UrlReferrer != null) previousPage = Request.UrlReferrer.ToString();
                    else previousPage = "Home.aspx";
                    if (currentPage.Equals(previousPage))
                        previousPage = "Home.aspx";
                }
            }
            initialPageAccess();
            profileId = Request.QueryString["id"];
            g_loginId = loginId;
            bool userIdExists = isUserCorrect();
            if (!userIdExists)
                goHome();
            showInformation();
        }
        protected void showInformation()
        {
            connect.Open();
            SqlCommand cmd = connect.CreateCommand();
            //Get the current user's ID who is trying to access the profile:
            cmd.CommandText = "select userId from Users where loginId = '" + loginId + "' ";
            string current_userId = cmd.ExecuteScalar().ToString();
            cmd.CommandText = "select loginId from users where userId = '" + profileId + "' ";
            string account_loginId = cmd.ExecuteScalar().ToString();
            cmd.CommandText = "select login_isActive from Logins where loginId = '" + account_loginId + "' ";
            int isActive = Convert.ToInt32(cmd.ExecuteScalar());
            cmd.CommandText = "select roleId from logins where loginId = '" + account_loginId + "' ";
            int account_roleId = Convert.ToInt32(cmd.ExecuteScalar());
            connect.Close();
            //if physician, check the PhysicianCompleteProfiles table to see if the profile is private:
            if (account_roleId == 2)//2 = Physician
            {
                connect.Open();
                cmd.CommandText = "select PhysicianCompleteProfile_isPrivate from PhysicianCompleteProfiles where userId = '" + profileId + "' ";
                int physician_isPrivate = Convert.ToInt32(cmd.ExecuteScalar());
                connect.Close();
                if (physician_isPrivate == 0)
                {
                    //fetch information...
                    getPhysicianCompleteProfileInformation(profileId);
                }
                else
                    lblRow.Text = "The account you are trying to access is private.";
            }
            //if patient, check the PatientCompleteProfiles table to see if the profile is private:
            else if (account_roleId == 3)//3 = Patient
            {
                connect.Open();
                cmd.CommandText = "select patientCompleteProfile_isPrivate from PatientCompleteProfiles where userId = '" + profileId + "' ";
                int patient_isPrivate = Convert.ToInt32(cmd.ExecuteScalar());
                connect.Close();
                if (patient_isPrivate == 0)
                {
                    //fetch information...
                    //Display the information:
                    getPatientCompleteProfileInformation(profileId);
                }
                else
                    lblRow.Text = "The account you are trying to access is private.";
            }
            else //This account you are trying to access belongs to an admin
            {
                if (account_loginId == loginId)
                {
                    lblRow.Text = "This account belongs to you as an admin in the system";
                }
                else //Another admin
                    lblRow.Text = "This account belongs to admin in the system";
            }
            string terminateCommand = "<button id='terminate_button'type='button' onclick=\"terminateAccount('" + profileId + "')\">Terminate Account</button>";
            string unlockCommand = "<button id='unlock_button'type='button' onclick=\"unlockAccount('" + profileId + "')\">Unlock Account</button>";
            if (isActive == 1 && account_loginId != loginId)
                lblAdminCommands.Text += terminateCommand;
            else if (isActive == 0 && account_loginId != loginId)
                lblAdminCommands.Text += unlockCommand;
        }
        protected void getPhysicianCompleteProfileInformation(string id)
        {
            string row = "";
            lblRow.Text = "";
            string col_start = "<td>", col_end = "</td>", row_start = "<tr>", row_end = "</tr>";
            connect.Open();
            SqlCommand cmd = connect.CreateCommand();
            cmd.CommandText = "select userId from Users where loginId = '" + loginId + "' ";
            string userId = cmd.ExecuteScalar().ToString();
            connect.Close();
            PhysicianCompleteProfile completeProfile = new PhysicianCompleteProfile(userId, id);
            string completeProfileId = completeProfile.ID;
            int isPrivate = completeProfile.Private;
            if (isPrivate == 1)
            {
                row += row_start + col_start + "This physician complete profile information is private " + col_end + row_end;
                lblRow.Text += row;
                lblRow.ForeColor = System.Drawing.Color.Red;
            }
            else
            {
                lblRow.ForeColor = System.Drawing.Color.Black;
                string dialysis = completeProfile.Dialysis;
                string homeDialysis = completeProfile.HomeDialysis;
                string transplantation = completeProfile.Transplantation;
                string hypertension = completeProfile.Hypertension;
                string gN = completeProfile.GN;
                string physicianId = completeProfile.PhysicianID;
                string str_isPrivate = "";
                if (isPrivate == 1)
                    str_isPrivate = "Private";
                else
                    str_isPrivate = "Viewable by Admins";
                row += row_start + col_start + "Physician Complete Profile Information: " + col_end + row_end;
                row += row_start + col_start + "Account is: " + col_end + col_start + str_isPrivate + col_end + row_end;
                if (!string.IsNullOrWhiteSpace(dialysis))
                    row += row_start + col_start + "Dialysis: " + col_end + col_start + dialysis + col_end + row_end;
                if (!string.IsNullOrWhiteSpace(homeDialysis))
                    row += row_start + col_start + "Home Dialysis: " + col_end + col_start + homeDialysis + col_end + row_end;
                if (!string.IsNullOrWhiteSpace(transplantation))
                    row += row_start + col_start + "Transplantation: " + col_end + col_start + transplantation + col_end + row_end;
                if (!string.IsNullOrWhiteSpace(hypertension))
                    row += row_start + col_start + "Hypertension: " + col_end + col_start + hypertension + col_end + row_end;
                if (!string.IsNullOrWhiteSpace(gN))
                    row += row_start + col_start + "GN: " + col_end + col_start + gN + col_end + row_end;
                if (!string.IsNullOrWhiteSpace(physicianId))
                    row += row_start + col_start + "Physician ID: " + col_end + col_start + physicianId + col_end + row_end;
                lblRow.Text += row;
            }
        }
        protected void getPatientCompleteProfileInformation(string id)
        {
            string row = "";
            lblRow.Text = "";
            string col_start = "<td>", col_end = "</td>", row_start = "<tr>", row_end = "</tr>";
            connect.Open();
            SqlCommand cmd = connect.CreateCommand();
            cmd.CommandText = "select userId from Users where loginId = '" + loginId + "' ";
            string userId = cmd.ExecuteScalar().ToString();
            connect.Close();
            PatientCompleteProfile completeProfile = new PatientCompleteProfile(userId, id);
            string completeProfileId = completeProfile.ID;
            int isPrivate = completeProfile.Private;
            if (isPrivate == 1)
            {
                row += row_start + col_start + "This patient complete profile information is private " + col_end + row_end;
                lblRow.Text += row;
                lblRow.ForeColor = System.Drawing.Color.Red;
            }
            else
            {
                lblRow.ForeColor = System.Drawing.Color.Black;
                string highBloodPressure = completeProfile.HighBloodPressure;
                string diabetes = completeProfile.Diabetes;
                string kidneyTransplant = completeProfile.KidneyTransplant;
                string dialysis = completeProfile.Dialysis;
                string kidneyStone = completeProfile.KidneyStone;
                string kidneyInfection = completeProfile.KidneyInfection;
                string heartFailure = completeProfile.HeartFailure;
                string cancer = completeProfile.Cancer;
                string comments = completeProfile.Comments;
                string patientId = completeProfile.PatientID;
                string str_isPrivate = "";
                if (isPrivate == 1)
                    str_isPrivate = "Private";
                else
                    str_isPrivate = "Viewable by Admins";
                row += row_start + col_start + "Patient Complete Profile Information: " + col_end + row_end;
                row += row_start + col_start + "Account is: " + col_end + col_start + str_isPrivate + col_end + row_end;
                if (!string.IsNullOrWhiteSpace(highBloodPressure))
                    row += row_start + col_start + "High Blood Pressure: " + col_end + col_start + highBloodPressure + col_end + row_end;
                if (!string.IsNullOrWhiteSpace(diabetes))
                    row += row_start + col_start + "Diabetes: " + col_end + col_start + diabetes + col_end + row_end;
                if (!string.IsNullOrWhiteSpace(kidneyTransplant))
                    row += row_start + col_start + "Kidney Transplant: " + col_end + col_start + kidneyTransplant + col_end + row_end;
                if (!string.IsNullOrWhiteSpace(dialysis))
                    row += row_start + col_start + "Dialysis: " + col_end + col_start + dialysis + col_end + row_end;
                if (!string.IsNullOrWhiteSpace(kidneyStone))
                    row += row_start + col_start + "Kidney Stone: " + col_end + col_start + kidneyStone + col_end + row_end;
                if (!string.IsNullOrWhiteSpace(kidneyInfection))
                    row += row_start + col_start + "Kidney Infection: " + col_end + col_start + kidneyInfection + col_end + row_end;
                if (!string.IsNullOrWhiteSpace(heartFailure))
                    row += row_start + col_start + "Heart Failure: " + col_end + col_start + heartFailure + col_end + row_end;
                if (!string.IsNullOrWhiteSpace(cancer))
                    row += row_start + col_start + "Cancer: " + col_end + col_start + cancer + col_end + row_end;
                if (!string.IsNullOrWhiteSpace(comments))
                    row += row_start + col_start + "Comments: " + col_end + col_start + comments + col_end + row_end;
                if (!string.IsNullOrWhiteSpace(patientId))
                    row += row_start + col_start + "Patient ID: " + col_end + col_start + patientId + col_end + row_end;
                lblRow.Text += row;
            }
        }
        protected bool isUserCorrect()
        {
            bool correct = true;
            CheckErrors errors = new CheckErrors();
            //check if id contains a special character:
            if (!errors.isDigit(profileId))
                correct = false;
            //check if id contains an id that does not exist in DB:
            else if (errors.ContainsSpecialChars(profileId))
                correct = false;
            if (correct)
            {
                connect.Open();
                SqlCommand cmd = connect.CreateCommand();
                //Count the existance of the user:
                cmd.CommandText = "select count(*) from Users where userId = '" + profileId + "' ";
                int count = Convert.ToInt32(cmd.ExecuteScalar());
                if (count > 0)//if count > 0, then the user ID exists in DB.
                {
                    //Get the current user's ID who is trying to access the profile:
                    cmd.CommandText = "select userId from Users where loginId = '" + loginId + "' ";
                    string current_userId = cmd.ExecuteScalar().ToString();
                    //Maybe later use the current user's ID to check if the current user has access to view the selected profile.
                }
                else
                    correct = false; // means that the user ID does not exists in DB.
                connect.Close();
            }
            return correct;
        }
        protected void goHome()
        {
            Response.Redirect("Home.aspx");
        }
        protected void goBack()
        {
            addSession();
            requestedTerminateOrUnlockAccount = false;
            if (!string.IsNullOrWhiteSpace(previousPage)) Response.Redirect(previousPage);
            else Response.Redirect("Home.aspx");
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
            goBack();
        }

        protected void getSession()
        {
            username = (string)(Session["username"]);
            roleId = (string)(Session["roleId"]);
            loginId = (string)(Session["loginId"]);
            token = (string)(Session["token"]);
        }
        [WebMethod]
        [ScriptMethod()]
        public static void terminateOrUnlockAccount(string in_profileId, int terminateOrUnlock)
        {
            requestedTerminateOrUnlockAccount = true;
            Configuration config = new Configuration();
            SqlConnection connect = new SqlConnection(config.getConnectionString());
            bool accountIdExists = isAccountCorrect(in_profileId, terminateOrUnlock);
            if (accountIdExists)
            {
                connect.Open();
                SqlCommand cmd = connect.CreateCommand();
                cmd.CommandText = "select loginId from users where userId = '" + in_profileId + "' ";
                string account_loginId = cmd.ExecuteScalar().ToString();
                connect.Close();
                if (terminateOrUnlock == 1)//1=terminate
                {
                    connect.Open();
                    //update the DB and set isActive = false:
                    cmd.CommandText = "update Logins set login_isActive = 0 where loginId = '" + account_loginId + "' ";
                    cmd.ExecuteScalar();
                    //Email the topic creator about the topic being deleted:
                    cmd.CommandText = "select user_firstname from Users where userId = '" + in_profileId + "' ";
                    string name = cmd.ExecuteScalar().ToString();
                    cmd.CommandText = "select user_lastname from Users where userId = '" + in_profileId + "' ";
                    name = name + " " + cmd.ExecuteScalar().ToString();
                    cmd.CommandText = "select user_email from Users where userId = '" + in_profileId + "' ";
                    string emailTo = cmd.ExecuteScalar().ToString();
                    connect.Close();
                    string emailBody = "Hello " + name + ",\n\n" +
                        "This email is to inform you that your account has been terminated. If you think this happened by mistake, or you did not perform this action, plaese contact the support.\n\n" +
                        "Best regards,\nNephroNet Support\nNephroNet2018@gmail.com";
                    Email email = new Email();
                    email.sendEmail(emailTo, emailBody);
                }
                else if (terminateOrUnlock == 2)//2 = Unlock
                {
                    connect.Open();
                    //update the DB and set isActive = true:
                    cmd.CommandText = "update Logins set login_isActive = 1 where loginId = '" + account_loginId + "' ";
                    cmd.ExecuteScalar();
                    //Email the topic creator about the topic being deleted:
                    cmd.CommandText = "select user_firstname from Users where userId = '" + in_profileId + "' ";
                    string name = cmd.ExecuteScalar().ToString();
                    cmd.CommandText = "select user_lastname from Users where userId = '" + in_profileId + "' ";
                    name = name + " " + cmd.ExecuteScalar().ToString();
                    cmd.CommandText = "select user_email from Users where userId = '" + in_profileId + "' ";
                    string emailTo = cmd.ExecuteScalar().ToString();
                    connect.Close();
                    string emailBody = "Hello " + name + ",\n\n" +
                    "This email is to inform you that your account has been unlocked and can now be used in the system. You can now use your username and password to login. If you have any questions, plaese contact the support.\n\n" +
                    "Best regards,\nNephroNet Support\nNephroNet2018@gmail.com";
                    Email email = new Email();
                    email.sendEmail(emailTo, emailBody);
                }
            }
        }
        protected static bool isAccountCorrect(string in_profileId, int terminateOrUnlock)
        {
            Configuration config = new Configuration();
            SqlConnection connect = new SqlConnection(config.getConnectionString());
            bool correct = true;
            CheckErrors errors = new CheckErrors();
            //check if id contains a special character:
            if (!errors.isDigit(in_profileId))
                correct = false;
            //check if id contains an id that does not exist in DB:
            else if (errors.ContainsSpecialChars(in_profileId))
                correct = false;
            if (correct)
            {
                connect.Open();
                SqlCommand cmd = connect.CreateCommand();
                //Count the existance of the user:
                cmd.CommandText = "select count(*) from Users where userId = '" + in_profileId + "' ";
                int count = Convert.ToInt32(cmd.ExecuteScalar());
                if (count > 0)//if count > 0, then the user ID exists in DB.
                {
                    //Get the current user's ID who is trying to access the profile:
                    cmd.CommandText = "select userId from Users where loginId = '" + g_loginId + "' ";
                    string current_userId = cmd.ExecuteScalar().ToString();
                    cmd.CommandText = "select loginId from users where userId = '" + in_profileId + "' ";
                    string account_loginId = cmd.ExecuteScalar().ToString();
                    cmd.CommandText = "select login_isActive from Logins where loginId = '" + account_loginId + "' ";
                    int isActive = Convert.ToInt32(cmd.ExecuteScalar());
                    if (terminateOrUnlock == 1)// if the command was to terminate:
                        if (isActive == 0)
                            correct = false;
                        else if (terminateOrUnlock == 2)// if the command was to unlock:
                            if (isActive == 1)
                                correct = false;
                    //Maybe later use the current user's ID to check if the current user has access to view the selected profile.
                    if (account_loginId == g_loginId)
                        correct = false;
                }
                else
                    correct = false; // means that the user ID does not exists in DB.
                connect.Close();
            }
            return correct;
        }
    }
}