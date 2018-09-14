using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NephroNet.Accounts.Patient
{
    public partial class Profile : System.Web.UI.Page
    {
        static string conn = "";
        SqlConnection connect = new SqlConnection(conn);
        string username, roleId, loginId, token;
        string profileId = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            initialPageAccess();
            profileId = Request.QueryString["id"];
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
            cmd.CommandText = "select shortProfile_isPrivate from ShortProfiles where userId = '" + profileId + "' ";
            int isPrivate = Convert.ToInt32(cmd.ExecuteScalar());
            cmd.CommandText = "select shortProfileId from ShortProfiles where userId = '" + profileId + "' ";
            string shortProfileId = cmd.ExecuteScalar().ToString();
            cmd.CommandText = "select count(*) from BlockedUsers where shortProfileId = '" + shortProfileId + "' ";
            int countBlocked = Convert.ToInt32(cmd.ExecuteScalar());
            bool current_user_isBlocked = false;
            for (int i = 1; i <= countBlocked; i++)
            {
                cmd.CommandText = "select [userId] from(SELECT rowNum = ROW_NUMBER() OVER(ORDER BY BlockedUserId ASC), * " +
                    "FROM [BlockedUsers] where shortProfileId = '" + shortProfileId + "' ) as t where rowNum = '" + i + "'";
                string blocked_userId = cmd.ExecuteScalar().ToString();
                if (blocked_userId.Equals(current_userId))
                    current_user_isBlocked = true;
            }
            connect.Close();
            int int_roleId = Convert.ToInt32(roleId);
            lblAdminCommands.Text = "";
            if (int_roleId == 1)
            {
                //Display the information:
                getShortProfileInformation();
                getCompleteProfileInformation();
                string terminateCommand = "<br/><button id='terminate_button'type='button' onmousedown=\"OpenPopup('TerminateAccount.aspx?id=" + profileId + "')\">Terminate Account</button>";
                string unlockCommand = "<br/><button id='unlock_button'type='button' onmousedown=\"OpenPopup('UnlockAccount.aspx?id=" + profileId + "')\">Unlock Account</button>";
                if (isActive == 1 && account_loginId != loginId)
                    lblAdminCommands.Text += terminateCommand;
                else if (isActive == 0 && account_loginId != loginId)
                    lblAdminCommands.Text += unlockCommand;
            }
            if (int_roleId != 1)
            {
                if (isPrivate == 0 && !current_user_isBlocked)//if current user not admin and profile is public and current user not blocked, show info
                    getShortProfileInformation(); //Display the information:
                else
                {
                    lblRow.Text = "This account is private.";
                }
            }
        }
        protected void getShortProfileInformation()
        {
            lblRow.Text = "";
            string row = "";
            string col_start = "<td>", col_end = "</td>", row_start = "<tr>", row_end = "</tr>";
            connect.Open();
            SqlCommand cmd = connect.CreateCommand();
            cmd.CommandText = "select userId from Users where loginId = '" + loginId + "' ";
            string userId = cmd.ExecuteScalar().ToString();
            ShortProfile shortProfile = new ShortProfile(profileId, userId);
            string shortProfileId = shortProfile.Id;
            string name = shortProfile.FirstName + " " + shortProfile.LastName;
            string race = shortProfile.Race;
            string gender = shortProfile.Gender;
            string birthdate = shortProfile.Birthdate;
            string nationality = shortProfile.Nationality;
            int shortProfile_roleId = shortProfile.RoleId;
            ArrayList blockedUsers = shortProfile.BlockedUsers;
            ArrayList currentHealthConditions = shortProfile.CurrentHealthConditions;
            ArrayList currentTreatments = shortProfile.CurrentTreatments;
            string role_name = shortProfile.RoleName;
            row += row_start + col_start + "Short Profile Information: " + col_end + row_end;
            row += row_start + col_start + "Name: " + col_end + col_start + name + col_end + row_end;
            if (!string.IsNullOrWhiteSpace(race))
                row += row_start + col_start + "Race: " + col_end + col_start + race + col_end + row_end;
            if (!string.IsNullOrWhiteSpace(gender))
                row += row_start + col_start + "Gender: " + col_end + col_start + gender + col_end + row_end;
            if (!string.IsNullOrWhiteSpace(birthdate))
                row += row_start + col_start + "Birthdate: " + col_end + col_start + Layouts.getBirthdateFormat(birthdate) + col_end + row_end;
            if (!string.IsNullOrWhiteSpace(nationality))
                row += row_start + col_start + "Nationality: " + col_end + col_start + nationality + col_end + row_end;
            row += row_start + col_start + "Role: " + col_end + col_start + role_name + col_end + row_end;
            //loop through blocked users:
            //if (blockedUsers.Count > 0)
            //{
            //    row += row_start + col_start + "Blocked users: " + col_end + col_start + "" + col_end + row_end;
            //    for (int i = 0; i < blockedUsers.Count; i++)
            //        row += row_start + col_start + "" + col_end + col_start + (i + 1) + ". " + blockedUsers[i].ToString() + col_end + row_end;
            //}
            //loop through current health conditions:
            if (currentHealthConditions.Count > 0)
            {
                row += row_start + col_start + "Current health conditions: " + col_end + col_start + "" + col_end + row_end;
                for (int i = 0; i < currentHealthConditions.Count; i++)
                    row += row_start + col_start + "" + col_end + col_start + (i + 1) + ". " + currentHealthConditions[i].ToString() + col_end + row_end;
            }
            //loop through current Treatments:
            if (currentTreatments.Count > 0)
            {
                row += row_start + col_start + "Current Treatments: " + col_end + col_start + "" + col_end + row_end;
                for (int i = 0; i < currentTreatments.Count; i++)
                    row += row_start + col_start + "" + col_end + col_start + (i + 1) + ". " + currentTreatments[i].ToString() + col_end + row_end;
            }
            lblRow.Text += row;
            connect.Close();
        }
        protected void getCompleteProfileInformation()
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
        protected void initialPageAccess()
        {
            Configuration config = new Configuration();
            conn = config.getConnectionString();
            connect = new SqlConnection(conn);
            getSession();
            CheckPatientSession session = new CheckPatientSession();
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
    }
}