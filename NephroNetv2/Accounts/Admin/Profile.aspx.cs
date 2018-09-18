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
            string terminateCommand = "<button id='terminate_button'type='button' onclick=\"terminateAccount('" + profileId + "')\">Terminate Account</button>";
            string unlockCommand = "<button id='unlock_button'type='button' onclick=\"unlockAccount('" + profileId + "')\">Unlock Account</button>";
            if (isActive == 1 && account_loginId != loginId)
                lblAdminCommands.Text += terminateCommand;
            else if (isActive == 0 && account_loginId != loginId)
                lblAdminCommands.Text += unlockCommand;
        }
        protected void getPhysicianCompleteProfileInformation(string id)
        {

        }
        protected void getPatientCompleteProfileInformation(string id)
        {
            string newLine = "<br/>";
            string col_start = "<td>", col_end = "</td>", row_start = "<tr>", row_end = "</tr>";
            connect.Open();
            SqlCommand cmd = connect.CreateCommand();
            cmd.CommandText = "select userId from Users where loginId = '" + loginId + "' ";
            string userId = cmd.ExecuteScalar().ToString();
            CompleteProfile completeProfile = new CompleteProfile(userId, profileId);
            string completeProfileId = completeProfile.Id;
            if (!string.IsNullOrWhiteSpace(completeProfileId))
            {
                string onDialysis = completeProfile.OnDialysis;
                string kidneyDisease = completeProfile.KidneyDisease;
                string issueDate = completeProfile.IssueStartDate;
                string bloodType = completeProfile.BloodType;
                string address = completeProfile.Address + newLine + "  " + completeProfile.City + ", " + completeProfile.State + " " + completeProfile.Zip;
                int counter = 0;
                string row = "";
                row += row_start + col_start + col_end + col_start + col_end + row_end;
                row += row_start + col_start + col_end + col_start + col_end + row_end;
                row += row_start + col_start + "Complete Profile Information: " + col_end + row_end;
                if (!string.IsNullOrWhiteSpace(onDialysis))
                    row += row_start + col_start + "On dialysis: " + col_end + col_start + onDialysis + col_end + row_end;
                if (!string.IsNullOrWhiteSpace(kidneyDisease))
                    row += row_start + col_start + "Kidney disease stage: " + col_end + col_start + kidneyDisease + col_end + row_end;
                if (!string.IsNullOrWhiteSpace(issueDate))
                    row += row_start + col_start + "Health issue started on: " + col_end + col_start + issueDate + col_end + row_end;
                if (!string.IsNullOrWhiteSpace(bloodType))
                    row += row_start + col_start + "Blood type: " + col_end + col_start + bloodType + col_end + row_end;
                row += row_start + col_start + "Address: " + col_end + col_start + address + col_end + row_end;
                List<Insurance> insurances = completeProfile.Insurances;
                if (insurances.Count > 0)
                    row += row_start + col_start + "Insurances: " + col_end + col_start + col_end + row_end;
                foreach (Insurance ins in insurances)
                {
                    row += row_start + col_start + "Insurance #:" + col_end + col_start + ++counter + col_end + row_end;
                    row += row_start + col_start + "Member ID:" + col_end + col_start + ins.MemberId + col_end + row_end;
                    row += row_start + col_start + "Group ID:" + col_end + col_start + ins.GroupId + col_end + row_end;
                    row += row_start + col_start + "Insurance name: " + col_end + col_start + ins.CompanyName + col_end + row_end;
                    row += row_start + col_start + "Insurance phone1 : " + col_end + col_start + ins.Phone1 + col_end + row_end;
                    row += row_start + col_start + "Insurance phone2 : " + col_end + col_start + ins.Phone2 + col_end + row_end;
                    row += row_start + col_start + "Insurance email: " + col_end + col_start + ins.Phone2 + col_end + row_end;
                    row += row_start + col_start + "Insurance address: " + col_end + col_start +
                        ins.Address + newLine + ins.City + ", " + ins.State + " " + ins.Zip + newLine + ins.Country +
                        col_end + row_end;
                }
                ArrayList allergies = completeProfile.Allergies;
                if (allergies.Count > 0)
                    row += row_start + col_start + "Allergies: " + col_end + col_start + col_end + row_end;
                counter = 0;
                foreach (var a in allergies)
                    row += row_start + col_start + col_end + col_start + ++counter + ". " + a.ToString() + col_end + row_end;
                ArrayList majorDiagnoses = completeProfile.MajorDiagnoses;
                if (majorDiagnoses.Count > 0)
                    row += row_start + col_start + "Major diagnoses: " + col_end + col_start + col_end + row_end;
                counter = 0;
                foreach (var a in majorDiagnoses)
                    row += row_start + col_start + col_end + col_start + ++counter + ". " + a.ToString() + col_end + row_end;
                ArrayList pastHealthConditions = completeProfile.PastHealthConditions;
                if (pastHealthConditions.Count > 0)
                    row += row_start + col_start + "Past health conditions: " + col_end + col_start + col_end + row_end;
                counter = 0;
                foreach (var a in pastHealthConditions)
                    row += row_start + col_start + "" + col_end + col_start + ++counter + ". " + a.ToString() + col_end + row_end;
                List<EmailObject> emails = completeProfile.Emails;
                if (emails.Count > 0)
                    row += row_start + col_start + "Emails: " + col_end + col_start + col_end + row_end;
                counter = 0;
                foreach (EmailObject e in emails)
                {
                    row += row_start + col_start + "" + col_end + col_start + ++counter + ". Email Address: " + e.EmailAddress;
                    if (e.IsDefault == 1)
                        row += " (default)" + col_end + row_end;
                    else
                        row += col_end + row_end;
                }
                List<Phone> phones = completeProfile.Phones;
                if (phones.Count > 0)
                    row += row_start + col_start + "Phone numbers: " + col_end + col_start + col_end + row_end;
                counter = 0;
                foreach (Phone e in phones)
                {
                    row += row_start + col_start + "" + col_end + col_start + ++counter + ". Phone number: " + e.PhoneNumber;
                    if (e.IsDefault == 1)
                        row += " (default)" + col_end + row_end;
                    else
                        row += col_end + row_end;
                }
                List<EmergencyContact> emergnecyContacts = completeProfile.EmergencyContacts;
                if (emergnecyContacts.Count > 0)
                    row += row_start + col_start + "Emergency contacts: " + col_end + col_start + col_end + row_end;
                counter = 0;
                foreach (EmergencyContact e in emergnecyContacts)
                {
                    row += row_start + col_start + "Contact #:" + col_end + col_start + ++counter + col_end + row_end;
                    row += row_start + col_start + "Name: " + col_end + col_start + e.Firstname + " " + e.Lastname + col_end + row_end;
                    row += row_start + col_start + "Phone 1: " + col_end + col_start + e.Phone1 + col_end + row_end;
                    row += row_start + col_start + "Phone 2: " + col_end + col_start + e.Phone2 + col_end + row_end;
                    row += row_start + col_start + "Phone 3: " + col_end + col_start + e.Phone3 + col_end + row_end;
                    row += row_start + col_start + "Email: " + col_end + col_start + e.Email + col_end + row_end;
                    row += row_start + col_start + "Address: " + col_end + col_start +
                        e.Address + newLine + e.City + ", " + e.State + " " + e.Zip + newLine + e.Country +
                        col_end + row_end;
                }
                List<PastPatientID> pastPatientIds = completeProfile.PastPatientIds;
                if (pastPatientIds.Count > 0)
                    row += row_start + col_start + "Past patient IDs: " + col_end + col_start + col_end + row_end;
                counter = 0;
                int treatment_count = 0;
                foreach (PastPatientID p in pastPatientIds)
                {
                    //row += row_start + col_start + "" + col_end + col_start + "" + col_end + row_end;
                    row += row_start + col_start + "Medical Record Number: " + col_end + col_start + p.MRN + col_end + row_end;
                    List<Treatment> treatments = completeProfile.Treatments;
                    string str_treatments = "";
                    if (treatments.Count > 0)
                    {
                        str_treatments = "Treatments: " + newLine;
                    }
                    foreach (Treatment t in treatments)
                    {
                        if (t.PastPatientId.Equals(p.ID))
                        {
                            row += row_start + col_start + "Treatment #: " + col_end + col_start + ++treatment_count + col_end + row_end;
                            row += row_start + col_start + "Physician name: " + col_end + col_start + t.PhysicianFirstName + " " + t.PhysicianLastName + col_end + row_end;
                            row += row_start + col_start + "Treatment started on: " + col_end + col_start + t.StartDate + col_end + row_end;
                            row += row_start + col_start + "Hospital name: " + col_end + col_start + t.HospitalName + col_end + row_end;
                            row += row_start + col_start + "Hospital address: " + col_end + col_start +
                                t.HospitalAddress + newLine +
                                t.HospitalCity + ", " + t.HospitalState + " " + t.HospitalZip + newLine +
                                t.HospitalCountry +
                                col_end + row_end;
                        }
                    }
                }
                lblRow.Text += row;
            }
            connect.Close();
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