using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Drawing;
using System.Globalization;

namespace NephroNet.Accounts.Patient
{
    public partial class MyProfile : System.Web.UI.Page
    {
        static string conn = "";
        SqlConnection connect = new SqlConnection(conn);
        string username, roleId, loginId, token;
        protected void Page_Load(object sender, EventArgs e)
        {
            initialPageAccess();
            if (!Page.IsPostBack)
                showInformation();
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
        protected void showInformation()
        {
            string nationalityHeadline = "Select your nationality";
            drpNationality.DataSource = getCountries();
            drpNationality.DataBind();
            drpNationality.Items.Insert(0, new ListItem(nationalityHeadline, nationalityHeadline));
            string countryHeadline = "Select your country";
            drpCountry.DataSource = getCountries();
            drpCountry.DataBind();
            drpCountry.Items.Insert(0, new ListItem(countryHeadline, countryHeadline));
            //Clear the drop downs to avoid duplicate dates:
            drpIssueStarted.Items.Clear();
            drpYearList.Items.Clear();
            for (int year = DateTime.Now.Year; year > 1880; year--)
            {
                string targetYear = String.Format("{0}", year);
                drpYearList.Items.Add(new ListItem(targetYear, year.ToString()));
                drpIssueStarted.Items.Add(new ListItem(targetYear, year.ToString()));
                //string targetYear = year.ToString();
                //drpYearList.Items.Add(new ListItem(targetYear));
            }
            getShortProfileInformation();
            getEditShortProfileInformation();
            getCompleteProfileInformation();
            getEditCompleteProfileInformation();
            viewProfiles();
        }
        protected void drpYearList_SelectedIndexChanged(object sender, EventArgs e)
        {
            calBirthdate.VisibleDate = DateTime.Now.AddYears(Convert.ToInt32(drpYearList.SelectedValue) - DateTime.Now.Year);
        }
        //public override void VerifyRenderingInServerForm(Control control) { }
        //Methods to show and hide controls
        protected void viewProfiles()
        {
            View.Visible = true;
            EditShortProfile.Visible = false;
            EditCompleteProfile.Visible = false;
            EditBlockedUsers.Visible = false;
            EditCurrentHealthConditions.Visible = false;
            EditCurrentTreatments.Visible = false;
            lblSaveShortProfileMessage.Visible = false;
            //Complete Profile Controls:
            EditMajorDiagnoses.Visible = false;
            EditPhoneNumbers.Visible = false;
            EditEmails.Visible = false;
            EditPastHealthConditions.Visible = false;
            EditAllergies.Visible = false;
            EditInsurances.Visible = false;
            EditPastPatientIDs.Visible = false;
            EditTreatmentsHistory.Visible = false;
            lblSaveCompleteProfileMessage.Visible = false;
        }
        protected void showEditShortProfile()
        {
            View.Visible = false;
            EditShortProfile.Visible = true;
            EditCompleteProfile.Visible = false;
            EditBlockedUsers.Visible = false;
            EditCurrentHealthConditions.Visible = false;
            EditCurrentTreatments.Visible = false;
            lblSaveShortProfileMessage.Visible = false;
            getEditShortProfileInformation();
            //Complete Profile Controls:
            EditMajorDiagnoses.Visible = false;
            EditPhoneNumbers.Visible = false;
            EditEmails.Visible = false;
            EditPastHealthConditions.Visible = false;
            EditAllergies.Visible = false;
            EditInsurances.Visible = false;
            EditPastPatientIDs.Visible = false;
            EditTreatmentsHistory.Visible = false;
        }
        protected void showEditCompleteProfile()
        {
            View.Visible = false;
            EditShortProfile.Visible = false;
            EditCompleteProfile.Visible = true;
            EditBlockedUsers.Visible = false;
            EditCurrentHealthConditions.Visible = false;
            EditCurrentTreatments.Visible = false;
            lblSaveShortProfileMessage.Visible = false;
            //Complete Profile Controls:
            EditMajorDiagnoses.Visible = false;
            EditPhoneNumbers.Visible = false;
            EditEmails.Visible = false;
            EditPastHealthConditions.Visible = false;
            EditAllergies.Visible = false;
            EditInsurances.Visible = false;
            EditPastPatientIDs.Visible = false;
            EditTreatmentsHistory.Visible = false;
        }
        protected void showEditBlockedUsers()
        {
            View.Visible = false;
            EditShortProfile.Visible = false;
            EditCompleteProfile.Visible = false;
            EditBlockedUsers.Visible = true;
            EditCurrentHealthConditions.Visible = false;
            EditCurrentTreatments.Visible = false;
            lblSaveShortProfileMessage.Visible = false;
            //Complete Profile Controls:
            EditMajorDiagnoses.Visible = false;
            EditPhoneNumbers.Visible = false;
            EditEmails.Visible = false;
            EditPastHealthConditions.Visible = false;
            EditAllergies.Visible = false;
            EditInsurances.Visible = false;
            EditPastPatientIDs.Visible = false;
            EditTreatmentsHistory.Visible = false;
        }
        protected void showEditCurrentHealthConditions()
        {
            View.Visible = false;
            EditShortProfile.Visible = false;
            EditCompleteProfile.Visible = false;
            EditBlockedUsers.Visible = false;
            EditCurrentHealthConditions.Visible = true;
            EditCurrentTreatments.Visible = false;
            lblSaveShortProfileMessage.Visible = false;
            //Complete Profile Controls:
            EditMajorDiagnoses.Visible = false;
            EditPhoneNumbers.Visible = false;
            EditEmails.Visible = false;
            EditPastHealthConditions.Visible = false;
            EditAllergies.Visible = false;
            EditInsurances.Visible = false;
            EditPastPatientIDs.Visible = false;
            EditTreatmentsHistory.Visible = false;
        }
        protected void showEditCurrentTreatments()
        {
            View.Visible = false;
            EditShortProfile.Visible = false;
            EditCompleteProfile.Visible = false;
            EditBlockedUsers.Visible = false;
            EditCurrentHealthConditions.Visible = false;
            EditCurrentTreatments.Visible = true;
            lblSaveShortProfileMessage.Visible = false;
            //Complete Profile Controls:
            EditMajorDiagnoses.Visible = false;
            EditPhoneNumbers.Visible = false;
            EditEmails.Visible = false;
            EditPastHealthConditions.Visible = false;
            EditAllergies.Visible = false;
            EditInsurances.Visible = false;
            EditPastPatientIDs.Visible = false;
            EditTreatmentsHistory.Visible = false;
        }
        protected void showEditMajorDiagnoses()
        {
            View.Visible = false;
            EditShortProfile.Visible = false;
            EditCompleteProfile.Visible = false;
            EditBlockedUsers.Visible = false;
            EditCurrentHealthConditions.Visible = false;
            EditCurrentTreatments.Visible = false;
            lblSaveShortProfileMessage.Visible = false;
            //Complete Profile Controls:
            EditMajorDiagnoses.Visible = true;
            EditPhoneNumbers.Visible = false;
            EditEmails.Visible = false;
            EditPastHealthConditions.Visible = false;
            EditAllergies.Visible = false;
            EditInsurances.Visible = false;
            EditPastPatientIDs.Visible = false;
            EditTreatmentsHistory.Visible = false;
        }
        protected void showEditPhoneNumbers()
        {
            View.Visible = false;
            EditShortProfile.Visible = false;
            EditCompleteProfile.Visible = false;
            EditBlockedUsers.Visible = false;
            EditCurrentHealthConditions.Visible = false;
            EditCurrentTreatments.Visible = false;
            lblSaveShortProfileMessage.Visible = false;
            //Complete Profile Controls:
            EditMajorDiagnoses.Visible = false;
            EditPhoneNumbers.Visible = true;
            EditEmails.Visible = false;
            EditPastHealthConditions.Visible = false;
            EditAllergies.Visible = false;
            EditInsurances.Visible = false;
            EditPastPatientIDs.Visible = false;
            EditTreatmentsHistory.Visible = false;
        }
        protected void showEditEmails()
        {
            View.Visible = false;
            EditShortProfile.Visible = false;
            EditCompleteProfile.Visible = false;
            EditBlockedUsers.Visible = false;
            EditCurrentHealthConditions.Visible = false;
            EditCurrentTreatments.Visible = false;
            lblSaveShortProfileMessage.Visible = false;
            //Complete Profile Controls:
            EditMajorDiagnoses.Visible = false;
            EditPhoneNumbers.Visible = false;
            EditEmails.Visible = true;
            EditPastHealthConditions.Visible = false;
            EditAllergies.Visible = false;
            EditInsurances.Visible = false;
            EditPastPatientIDs.Visible = false;
            EditTreatmentsHistory.Visible = false;
        }
        protected void showEditPastHealthConditions()
        {
            View.Visible = false;
            EditShortProfile.Visible = false;
            EditCompleteProfile.Visible = false;
            EditBlockedUsers.Visible = false;
            EditCurrentHealthConditions.Visible = false;
            EditCurrentTreatments.Visible = false;
            lblSaveShortProfileMessage.Visible = false;
            //Complete Profile Controls:
            EditMajorDiagnoses.Visible = false;
            EditPhoneNumbers.Visible = false;
            EditEmails.Visible = false;
            EditPastHealthConditions.Visible = true;
            EditAllergies.Visible = false;
            EditInsurances.Visible = false;
            EditPastPatientIDs.Visible = false;
            EditTreatmentsHistory.Visible = false;
        }
        protected void showEditAllergies()
        {
            View.Visible = false;
            EditShortProfile.Visible = false;
            EditCompleteProfile.Visible = false;
            EditBlockedUsers.Visible = false;
            EditCurrentHealthConditions.Visible = false;
            EditCurrentTreatments.Visible = false;
            lblSaveShortProfileMessage.Visible = false;
            //Complete Profile Controls:
            EditMajorDiagnoses.Visible = false;
            EditPhoneNumbers.Visible = false;
            EditEmails.Visible = false;
            EditPastHealthConditions.Visible = false;
            EditAllergies.Visible = true;
            EditInsurances.Visible = false;
            EditPastPatientIDs.Visible = false;
            EditTreatmentsHistory.Visible = false;
        }
        protected void showEditInsurances()
        {
            View.Visible = false;
            EditShortProfile.Visible = false;
            EditCompleteProfile.Visible = false;
            EditBlockedUsers.Visible = false;
            EditCurrentHealthConditions.Visible = false;
            EditCurrentTreatments.Visible = false;
            lblSaveShortProfileMessage.Visible = false;
            //Complete Profile Controls:
            EditMajorDiagnoses.Visible = false;
            EditPhoneNumbers.Visible = false;
            EditEmails.Visible = false;
            EditPastHealthConditions.Visible = false;
            EditAllergies.Visible = false;
            EditInsurances.Visible = true;
            EditPastPatientIDs.Visible = false;
            EditTreatmentsHistory.Visible = false;
        }
        protected void showEditPastPatientIDs()
        {
            View.Visible = false;
            EditShortProfile.Visible = false;
            EditCompleteProfile.Visible = false;
            EditBlockedUsers.Visible = false;
            EditCurrentHealthConditions.Visible = false;
            EditCurrentTreatments.Visible = false;
            lblSaveShortProfileMessage.Visible = false;
            //Complete Profile Controls:
            EditMajorDiagnoses.Visible = false;
            EditPhoneNumbers.Visible = false;
            EditEmails.Visible = false;
            EditPastHealthConditions.Visible = false;
            EditAllergies.Visible = false;
            EditInsurances.Visible = false;
            EditPastPatientIDs.Visible = true;
            EditTreatmentsHistory.Visible = false;
        }
        protected void showEditTreatmentsHistory()
        {
            View.Visible = false;
            EditShortProfile.Visible = false;
            EditCompleteProfile.Visible = false;
            EditBlockedUsers.Visible = false;
            EditCurrentHealthConditions.Visible = false;
            EditCurrentTreatments.Visible = false;
            lblSaveShortProfileMessage.Visible = false;
            //Complete Profile Controls:
            EditMajorDiagnoses.Visible = false;
            EditPhoneNumbers.Visible = false;
            EditEmails.Visible = false;
            EditPastHealthConditions.Visible = false;
            EditAllergies.Visible = false;
            EditInsurances.Visible = false;
            EditPastPatientIDs.Visible = false;
            EditTreatmentsHistory.Visible = true;
        }
        

        protected void btnShortProfile_Click(object sender, EventArgs e)
        {
            showEditShortProfile();
        }
        protected void btnCompleteProfile_Click(object sender, EventArgs e)
        {
            showEditCompleteProfile();
        }
        protected bool checkEditShortProfileInformationInput()
        {
            bool correct = true;
            //Hide everything first:
            lblFirstnameError.Visible = false;
            lblLastnameError.Visible = false;
            lblRaceError.Visible = false;
            lblGenderError.Visible = false;
            lblBirthdateError.Visible = false;
            lblNationalityError.Visible = false;
            lblIsPrivateError.Visible = false;
            CheckErrors check = new CheckErrors();
            if (!check.validFirstName(txtFirstname.Text, out string firstnameError))
            {
                correct = false;
                lblFirstnameError.Text = firstnameError;
                lblFirstnameError.Visible = true;
            }
            if (!check.validLastName(txtLastname.Text, out string lastnameError))
            {
                correct = false;
                lblLastnameError.Text = lastnameError;
                lblLastnameError.Visible = true;
            }
            if (drpRace.SelectedIndex == 0)
            {
                correct = false;
                lblRaceError.Text = "Invalid input: Please select a race.";
                lblRaceError.Visible = true;
            }
            if (drpGender.SelectedIndex == 0)
            {
                correct = false;
                lblGenderError.Text = "Invalid input: Please select a gender.";
                lblGenderError.Visible = true;
            }
            string str_minimum_date = "1880-01-01";
            DateTime minimum_date = DateTime.ParseExact(str_minimum_date, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            if (calBirthdate.SelectedDate == DateTime.MinValue)
            {
                correct = false;
                lblBirthdateError.Text = "Invalid input: Please select your birthdate.";
                lblBirthdateError.Visible = true;
            }
            else if (calBirthdate.SelectedDate < minimum_date || calBirthdate.SelectedDate >= DateTime.Now)
            {
                correct = false;
                lblBirthdateError.Text = "Invalid input: Please select a reasonable birthdate.";
                lblBirthdateError.Visible = true;
            }
            if (drpNationality.SelectedIndex == 0)
            {
                correct = false;
                lblNationalityError.Text = "Invalid input: Please select your nationality.";
                lblNationalityError.Visible = true;
            }
            if (drpIsPrivate.SelectedIndex == 0)
            {
                correct = false;
                lblIsPrivateError.Text = "Invalid input: Please select a permission.";
                lblIsPrivateError.Visible = true;
            }
            return correct;
        }
        protected void btnSaveEditShortProfile_Click(object sender, EventArgs e)
        {
            lblSaveShortProfileMessage.Visible = false;
            //check for input errors:
            if (checkEditShortProfileInformationInput())
            {
                //update the new information and store it in the DB:
                connect.Open();
                SqlCommand cmd = connect.CreateCommand();
                cmd.CommandText = "select userId from Users where loginId = '" + loginId + "' ";
                string userId = cmd.ExecuteScalar().ToString();
                ShortProfile shortProfile = new ShortProfile(userId, userId);
                string shortProfileId = shortProfile.Id;
                int isPrivate = 0;
                if (drpIsPrivate.SelectedIndex == 1)// 1: Public
                    isPrivate = 0;
                else if (drpIsPrivate.SelectedIndex == 2)// 2: Private
                    isPrivate = 1;
                cmd.CommandText = "update ShortProfiles set shortProfile_firstname = '" + txtFirstname.Text.Replace("'", "''") + "', shortProfile_lastname ='" + txtLastname.Text.Replace("'", "''") + "'," +
                    "shortProfile_race = '" + drpRace.SelectedValue + "', shortProfile_gender = '" + drpGender.SelectedValue + "', shortProfile_birthdate = '" + calBirthdate.SelectedDate + "', shortProfile_nationality = '" + drpNationality.SelectedValue + "'," +
                    "shortProfile_isPrivate = '" + isPrivate + "' where shortProfileId = '" + shortProfileId + "' ";
                cmd.ExecuteScalar();
                cmd.CommandText = "update Users set user_firstname = '" + txtFirstname.Text.Replace("'", "''") + "', " +
                    "user_lastname = '" + txtLastname.Text.Replace("'", "''") + "' where userId = '" + userId + "' ";
                cmd.ExecuteScalar();
                connect.Close();
                lblSaveShortProfileMessage.Visible = true;
            }
            getShortProfileInformation();
            getCompleteProfileInformation();
        }
        protected void btnCancelEditShortProfile_Click(object sender, EventArgs e)
        {
            viewProfiles();
            lblSaveShortProfileMessage.Visible = false;
            getShortProfileInformation();
            getCompleteProfileInformation();
        }
        protected void btnEditBlockedUsersView_Click(object sender, EventArgs e)
        {
            showEditBlockedUsers();
            getBlockedUsers();
        }
        protected void getBlockedUsers()
        {
            lblAddToBlockListError.Visible = false;
            lblRemoveFromBlockListError.Visible = false;
            lblSearchUsersToBlockError.Visible = false;
            lblSaveBlockedUsersMessage.Visible = false;
            drpBlockUsersOldResult.Items.Clear();
            connect.Open();
            SqlCommand cmd = connect.CreateCommand();
            cmd.CommandText = "select userId from Users where loginId = '" + loginId + "' ";
            string userId = cmd.ExecuteScalar().ToString();
            ShortProfile shortProfile = new ShortProfile(userId, userId);
            string shortProfileId = shortProfile.Id;
            ArrayList blockedUsers = shortProfile.BlockedUsers;
            //loop through blocked users:
            if (blockedUsers.Count > 0)
                for (int i = 0; i < blockedUsers.Count; i++)
                    drpBlockUsersOldResult.Items.Add(blockedUsers[i].ToString());
            connect.Close();
        }
        protected void btnCurrentHealthConditionsView_Click(object sender, EventArgs e)
        {
            showEditCurrentHealthConditions();
            getCurrentConditions();
        }
        protected void btnCurrentTreatmentsView_Click(object sender, EventArgs e)
        {
            showEditCurrentTreatments();
        }
        protected void getCurrentConditions()
        {
            lblAddTreatmentToListError.Visible = false;
            lblRemoveTreatmentError.Visible = false;
            lblCurrentConditionsMessage.Visible = false;
            drpConditionsToBeSaved.Items.Clear();
            connect.Open();
            SqlCommand cmd = connect.CreateCommand();
            cmd.CommandText = "select userId from Users where loginId = '" + loginId + "' ";
            string userId = cmd.ExecuteScalar().ToString();
            ShortProfile shortProfile = new ShortProfile(userId, userId);
            string shortProfileId = shortProfile.Id;
            ArrayList currentConditions = shortProfile.CurrentHealthConditions;
            //loop through blocked users:
            if (currentConditions.Count > 0)
                for (int i = 0; i < currentConditions.Count; i++)
                    drpConditionsToBeSaved.Items.Add(currentConditions[i].ToString());
            connect.Close();
        }
        public static List<string> getCountries()
        {
            List<string> countriesList = new List<string>();
            CultureInfo[] cultureInfo = CultureInfo.GetCultures(CultureTypes.SpecificCultures);
            foreach (CultureInfo culture in cultureInfo)
            {
                RegionInfo regionInfo = new RegionInfo(culture.LCID);
                if (!(countriesList.Contains(regionInfo.EnglishName)))
                    countriesList.Add(regionInfo.EnglishName);
            }
            countriesList.Sort();
            return countriesList;
        }
        protected void getEditShortProfileInformation()
        {
            connect.Open();
            SqlCommand cmd = connect.CreateCommand();
            cmd.CommandText = "select userId from Users where loginId = '" + loginId + "' ";
            string userId = cmd.ExecuteScalar().ToString();
            ShortProfile shortProfile = new ShortProfile(userId, userId);
            string shortProfileId = shortProfile.Id;
            txtFirstname.Text = shortProfile.FirstName;
            txtLastname.Text = shortProfile.LastName;
            string race = shortProfile.Race;
            if (!string.IsNullOrWhiteSpace(race))
                drpRace.SelectedValue = race;
            string gender = shortProfile.Gender;
            if (!string.IsNullOrWhiteSpace(gender))
                drpGender.SelectedValue = gender;
            string birthdate = shortProfile.Birthdate;
            if (!string.IsNullOrWhiteSpace(birthdate))
            {
                calBirthdate.SelectedDate = DateTime.Parse(birthdate);
                drpYearList.SelectedValue = calBirthdate.SelectedDate.Year.ToString();
                calBirthdate.VisibleDate = DateTime.Parse(birthdate);
            }
            string nationality = shortProfile.Nationality;
            if (!string.IsNullOrWhiteSpace(nationality))
                drpNationality.SelectedValue = nationality;
            int shortProfile_roleId = shortProfile.RoleId;
            lblRoleNameViewDisplay.Text = shortProfile.RoleName;
            int isPrivate = shortProfile.IsPrivate;
            if (isPrivate == 0)
                drpIsPrivate.SelectedIndex = 1;
            else if (isPrivate == 1)
                drpIsPrivate.SelectedIndex = 2;
            else
                drpIsPrivate.SelectedIndex = 0;
            ArrayList blockedUsers = shortProfile.BlockedUsers;
            lblBlockedUsersViewList.Text = "";
            lblCurrentHealthConditionsViewList.Text = "";
            lblCurrentTreatmentsViewList.Text = "";
            //loop through blocked users:
            if (blockedUsers.Count > 0)
            {
                for (int i = 0; i < blockedUsers.Count; i++)
                    lblBlockedUsersViewList.Text += blockedUsers[i].ToString() + "<br/>";
            }
            ArrayList currentHealthConditions = shortProfile.CurrentHealthConditions;
            //loop through blocked users:
            if (currentHealthConditions.Count > 0)
            {
                for (int i = 0; i < currentHealthConditions.Count; i++)
                    lblCurrentHealthConditionsViewList.Text += currentHealthConditions[i].ToString() + "<br/>";
            }
            ArrayList currentTreatments = shortProfile.CurrentTreatments;
            //loop through blocked users:
            if (currentTreatments.Count > 0)
            {
                for (int i = 0; i < currentTreatments.Count; i++)
                    lblCurrentTreatmentsViewList.Text += currentTreatments[i].ToString() + "<br/>";
            }
            string role_name = shortProfile.RoleName;
            connect.Close();
        }
        protected void btnSaveBlockedUsers_Click(object sender, EventArgs e)
        {
            lblSaveBlockedUsersMessage.Visible = false;
            connect.Open();
            SqlCommand cmd = connect.CreateCommand();
            cmd.CommandText = "select userId from Users where loginId = '" + loginId + "' ";
            string userId = cmd.ExecuteScalar().ToString();
            ShortProfile shortProfile = new ShortProfile(userId, userId);
            string shortProfileId = shortProfile.Id;
            //Delete everything from the list of blocked users:
            cmd.CommandText = "delete from BlockedUsers where shortProfileId = '" + shortProfileId + "' ";
            cmd.ExecuteScalar();
            foreach (ListItem u in drpBlockUsersOldResult.Items)
            {
                //Get the user ID of a user to be deleted from searching by name:
                cmd.CommandText = "select userId from users where (user_firstname + ' ' + user_lastname) like '%" + u.Value + "%' ";
                string temp_userId = cmd.ExecuteScalar().ToString();
                if (!string.IsNullOrWhiteSpace(temp_userId))//Double check if the search returned a value
                {
                    //Check that the selected user to be added in the block-list isn't already added:
                    cmd.CommandText = "select count(*) from BlockedUsers where shortProfileId = '" + shortProfileId + "' and userId = '" + temp_userId + "' ";
                    int exists = Convert.ToInt32(cmd.ExecuteScalar());
                    if (exists == 0)//0 =  the user doesn't already exist in the list of blocked users
                    {
                        cmd.CommandText = "insert into BlockedUsers (shortProfileId, userId) values ('" + shortProfileId + "', '" + temp_userId + "') ";
                        cmd.ExecuteScalar();
                    }
                }
            }
            lblSaveBlockedUsersMessage.Visible = true;
            lblSaveBlockedUsersMessage.Text = "You have successfully updated your list of blocked users!";
            connect.Close();
            //Update the information:
            getEditShortProfileInformation();
        }
        protected void btnSearchBlockUsers_Click(object sender, EventArgs e)
        {
            drpBlockUsersSearchResult.Items.Clear();
            if (string.IsNullOrWhiteSpace(txtSearchBlockedUsers.Text))
            {
                lblSearchUsersToBlockError.Visible = true;
                lblSearchUsersToBlockError.Text = "Please type a name to search for";
            }
            else
            {
                lblSearchUsersToBlockError.Visible = false;
                connect.Open();
                SqlCommand cmd = connect.CreateCommand();
                cmd.CommandText = "select count(*) from Users where (user_firstname +' ' +user_lastname) like '%" + txtSearchBlockedUsers.Text.Replace("'", "''") + "%'  ";
                int count = Convert.ToInt32(cmd.ExecuteScalar());
                for (int i = 1; i <= count; i++)
                {
                    cmd.CommandText = "select (user_firstname +' ' +user_lastname) from(SELECT rowNum = ROW_NUMBER() OVER(ORDER BY userId ASC), * " +
                        "FROM [Users] where (user_firstname +' ' +user_lastname) like '%" + txtSearchBlockedUsers.Text.Replace("'", "''") + "%') as t where rowNum = '" + i + "'";
                    string temp_name = cmd.ExecuteScalar().ToString();
                    //Check if this user is an adimn:
                    cmd.CommandText = "select loginId from(SELECT rowNum = ROW_NUMBER() OVER(ORDER BY userId ASC), * " +
                        "FROM [Users] where (user_firstname +' ' +user_lastname) like '%" + txtSearchBlockedUsers.Text.Replace("'", "''") + "%') as t where rowNum = '" + i + "'";
                    string temp_loginId = cmd.ExecuteScalar().ToString();
                    cmd.CommandText = "select roleId from logins where loginId = '" + temp_loginId + "' ";
                    int temp_roleId = Convert.ToInt32(cmd.ExecuteScalar());
                    //If not admin, add him/her to the search results:
                    if (temp_roleId != 1)//1: Admin
                        drpBlockUsersSearchResult.Items.Add(temp_name);
                }
                if (count == 0)
                {
                    lblSearchUsersToBlockError.Text = "There are no users with the entered keyword";
                    lblSearchUsersToBlockError.Visible = true;
                }
                else
                    lblSearchUsersToBlockError.Visible = false;
                cmd.CommandText = "select userId from users where loginId = '" + loginId + "' ";
                string userId = cmd.ExecuteScalar().ToString();
                cmd.CommandText = "select user_firstname + ' ' + user_lastname from users where userId = '" + userId + "' ";
                string myName = cmd.ExecuteScalar().ToString();
                drpBlockUsersSearchResult.Items.Remove(myName);
            }
            connect.Close();
        }
        protected void btnAddSelectedUsersToBlock_Click(object sender, EventArgs e)
        {
            lblAddToBlockListError.Visible = false;
            int i = 0;
            bool exists = false;
            foreach (ListItem u in drpBlockUsersSearchResult.Items)
                if (u.Selected)
                {
                    foreach (ListItem old in drpBlockUsersOldResult.Items)
                    {
                        //To avoid duplicate in the original list:
                        if (old.Equals(u))
                            exists = true;
                    }
                    if (!exists)
                        drpBlockUsersOldResult.Items.Add(u);
                    i++;
                }
            if (i == 0)
            {
                lblAddToBlockListError.Visible = true;
                lblAddToBlockListError.Text = "Please select users to add them";
            }
            else
                lblAddToBlockListError.Visible = false;

        }
        protected void btnRemoveSelectedUsersToBlock_Click(object sender, EventArgs e)
        {
            lblRemoveFromBlockListError.Visible = false;
            int size = drpBlockUsersOldResult.Items.Count;
            int i = 0;
            int a = 0;
            while (i < size)
            {
                ListItem item = drpBlockUsersOldResult.Items[i];
                if (item.Selected)
                {
                    drpBlockUsersOldResult.Items.RemoveAt(i);
                    size--;
                    a++;
                }
                i++;
            }
            if (a == 0)
            {
                lblRemoveFromBlockListError.Visible = true;
                lblRemoveFromBlockListError.Text = "Please select users to remove them";
            }
            else
                lblRemoveFromBlockListError.Visible = false;
        }
        protected void btnCancelBlockedUsers_Click(object sender, EventArgs e)
        {
            showEditShortProfile();
            lblSaveBlockedUsersMessage.Visible = false;
        }

        protected void btnAddConditionToList_Click(object sender, EventArgs e)
        {
            bool error = false;
            lblAddConditionToListError.Visible = false;
            string condition = txtCurrentConditionToList.Text;
            if (string.IsNullOrWhiteSpace(condition))
            {
                error = true;
                lblAddConditionToListError.Visible = true;
                lblAddConditionToListError.Text = "Please type a condition to be added";
            }

            bool exists = false;
            foreach (ListItem u in drpConditionsToBeSaved.Items)
            {
                //To avoid duplicate in the original list:
                if (condition.Equals(u.Value))
                {
                    exists = true;
                    error = true;
                }
            }
            if (exists)
            {
                error = true;
                lblAddConditionToListError.Visible = true;
                lblAddConditionToListError.Text = "Please type a condition that you did not add before";
            }
            if (!error)
            {
                drpConditionsToBeSaved.Items.Add(condition.Replace("'", "''"));
                txtCurrentConditionToList.Text = "";
            }

        }
        protected void btnConditionToBeRemoved_Click(object sender, EventArgs e)
        {
            lblRemoveConditionToListError.Visible = false;
            int size = drpConditionsToBeSaved.Items.Count;
            int i = 0;
            int a = 0;
            while (i < size)
            {
                ListItem item = drpConditionsToBeSaved.Items[i];
                if (item.Selected)
                {
                    drpConditionsToBeSaved.Items.RemoveAt(i);
                    size--;
                    a++;
                }
                i++;
            }
            if (a == 0)
            {
                lblRemoveConditionToListError.Visible = true;
                lblRemoveConditionToListError.Text = "Please select conditions to remove them";
            }
            else
                lblRemoveConditionToListError.Visible = false;

        }
        protected void btnSaveCurrentConditions_Click(object sender, EventArgs e)
        {
            lblCurrentConditionsMessage.Visible = false;
            connect.Open();
            SqlCommand cmd = connect.CreateCommand();
            cmd.CommandText = "select userId from Users where loginId = '" + loginId + "' ";
            string userId = cmd.ExecuteScalar().ToString();
            ShortProfile shortProfile = new ShortProfile(userId, userId);
            string shortProfileId = shortProfile.Id;
            //Delete everything from the list of blocked users:
            cmd.CommandText = "delete from CurrentHealthConditions where shortProfileId = '" + shortProfileId + "' ";
            cmd.ExecuteScalar();
            foreach (ListItem u in drpConditionsToBeSaved.Items)
            {
                cmd.CommandText = "insert into CurrentHealthConditions (shortProfileId, CurrentHealthCondition_name) values ('" + shortProfileId + "', '" + u.Value + "') ";
                cmd.ExecuteScalar();
            }
            lblCurrentConditionsMessage.Visible = true;
            lblCurrentConditionsMessage.Text = "You have successfully updated your list of current conditions!";
            connect.Close();
            //Update the information:
            getEditShortProfileInformation();
            txtCurrentConditionToList.Text = "";
        }
        protected void btnCancelCurrentConditions_Click(object sender, EventArgs e)
        {
            showEditShortProfile();
            lblCurrentConditionsMessage.Visible = false;
            txtCurrentConditionToList.Text = "";
        }

        protected void btnAddTreatmentToList_Click(object sender, EventArgs e)
        {
            bool error = false;
            lblAddTreatmentToListError.Visible = false;
            string treatment = txtTypeTreatment.Text;
            if (string.IsNullOrWhiteSpace(treatment))
            {
                error = true;
                lblAddTreatmentToListError.Visible = true;
                lblAddTreatmentToListError.Text = "Please type a treatment to be added";
            }

            bool exists = false;
            foreach (ListItem u in drpTreatmentsToBeSaved.Items)
            {
                //To avoid duplicate in the original list:
                if (treatment.Equals(u.Value))
                {
                    exists = true;
                    error = true;
                }
            }
            if (exists)
            {
                error = true;
                lblAddTreatmentToListError.Visible = true;
                lblAddTreatmentToListError.Text = "Please type a treatment that you did not add before";
            }
            if (!error)
            {
                drpTreatmentsToBeSaved.Items.Add(treatment.Replace("'", "''"));
                txtTypeTreatment.Text = "";
            }

        }
        protected void btnTreatmentsToBeRemoved_Click(object sender, EventArgs e)
        {
            lblRemoveTreatmentError.Visible = false;
            int size = drpTreatmentsToBeSaved.Items.Count;
            int i = 0;
            int a = 0;
            while (i < size)
            {
                ListItem item = drpTreatmentsToBeSaved.Items[i];
                if (item.Selected)
                {
                    drpTreatmentsToBeSaved.Items.RemoveAt(i);
                    size--;
                    a++;
                }
                i++;
            }
            if (a == 0)
            {
                lblRemoveTreatmentError.Visible = true;
                lblRemoveTreatmentError.Text = "Please select treatments to remove them";
            }
            else
                lblRemoveTreatmentError.Visible = false;

        }
        protected void btnSaveTreatment_Click(object sender, EventArgs e)
        {
            lblTreatmentsSavedSuccessfully.Visible = false;
            connect.Open();
            SqlCommand cmd = connect.CreateCommand();
            cmd.CommandText = "select userId from Users where loginId = '" + loginId + "' ";
            string userId = cmd.ExecuteScalar().ToString();
            ShortProfile shortProfile = new ShortProfile(userId, userId);
            string shortProfileId = shortProfile.Id;
            //Delete everything from the list of blocked users:
            cmd.CommandText = "delete from CurrentTreatments where shortProfileId = '" + shortProfileId + "' ";
            cmd.ExecuteScalar();
            foreach (ListItem u in drpTreatmentsToBeSaved.Items)
            {
                cmd.CommandText = "insert into CurrentTreatments (shortProfileId, CurrentTreatment_name) values ('" + shortProfileId + "', '" + u.Value + "') ";
                cmd.ExecuteScalar();
            }
            lblTreatmentsSavedSuccessfully.Visible = true;
            lblTreatmentsSavedSuccessfully.Text = "You have successfully updated your list of current treatments!";
            connect.Close();
            //Update the information:
            getEditShortProfileInformation();
            txtTypeTreatment.Text = "";
        }

        protected void getEditCompleteProfileInformation()
        {
            connect.Open();
            SqlCommand cmd = connect.CreateCommand();
            cmd.CommandText = "select userId from Users where loginId = '" + loginId + "' ";
            string userId = cmd.ExecuteScalar().ToString();
            CompleteProfile completeProfile = new CompleteProfile(userId, userId);
            string onDialysis = completeProfile.OnDialysis;
            string kidneyDisease = completeProfile.KidneyDisease;
            string issueStarted = completeProfile.IssueStartDate;
            string bloodType = completeProfile.BloodType;
            string country = completeProfile.Country;
            string city = completeProfile.City;
            string state = completeProfile.State;
            string address = completeProfile.Address;
            string zip = completeProfile.Zip;
            if (!string.IsNullOrWhiteSpace(onDialysis))
                drpOnDialysis.SelectedValue = onDialysis;
            if (!string.IsNullOrWhiteSpace(kidneyDisease))
                drpKidneyDisease.SelectedValue = kidneyDisease;
            if (!string.IsNullOrWhiteSpace(issueStarted))
            {
                calIssueStarted.SelectedDate = DateTime.Parse(issueStarted);
                drpIssueStarted.SelectedValue = calIssueStarted.SelectedDate.Year.ToString();
                calIssueStarted.VisibleDate = DateTime.Parse(issueStarted);
            }
            if (!string.IsNullOrWhiteSpace(bloodType))
                drpBloodType.SelectedValue = bloodType;
            if (!string.IsNullOrWhiteSpace(country))
                drpCountry.SelectedValue = country;
            if (!string.IsNullOrWhiteSpace(city))
                txtCity.Text = city;
            if (!string.IsNullOrWhiteSpace(state))
            {
                if (drpCountry.SelectedValue.Equals("United States"))
                {
                    drpState.SelectedValue = state;
                    txtState.Text = "";
                    drpState.Visible = true;
                    txtState.Visible = false;
                }
                else
                {
                    drpState.SelectedIndex = 0;
                    txtState.Text = state;
                    drpState.Visible = false;
                    txtState.Visible = true;
                }
            }
            if (!string.IsNullOrWhiteSpace(address))
                txtAddress.Text = address;
            if (!string.IsNullOrWhiteSpace(zip))
                txtZip.Text = zip;
            connect.Close();
        }
        protected void drpIssueStarted_SelectedIndexChanged(object sender, EventArgs e)
        {
            calIssueStarted.VisibleDate = DateTime.Now.AddYears(Convert.ToInt32(drpIssueStarted.SelectedValue) - DateTime.Now.Year);
        }

        protected void drpCountry_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!drpCountry.SelectedValue.Equals("United States"))
            {
                drpState.Visible = false;
                drpState.SelectedIndex = 0;
                txtState.Visible = true;
            }
            else
            {
                drpState.Visible = true;
                txtState.Visible = false;
                txtState.Text = "";
            }
        }

        protected void btnMajorDiagnosesView_Click(object sender, EventArgs e)
        {
            showEditMajorDiagnoses();
        }

        protected void btnPhoneNumbersView_Click(object sender, EventArgs e)
        {
            showEditPhoneNumbers();
        }

        protected void btnEmailsView_Click(object sender, EventArgs e)
        {
            showEditEmails();
        }

        protected void btnPastHealthConditionsView_Click(object sender, EventArgs e)
        {
            showEditPastHealthConditions();
        }

        protected void btnAllergiesView_Click(object sender, EventArgs e)
        {
            showEditAllergies();
        }

        protected void btnInsurancesView_Click(object sender, EventArgs e)
        {
            showEditInsurances();
        }

        protected void btnPastPatientIDsView_Click(object sender, EventArgs e)
        {
            showEditPastPatientIDs();
        }

        protected void btnTreatmentsHistoryView_Click(object sender, EventArgs e)
        {
            showEditTreatmentsHistory();
        }
        protected bool checkEditCompleteProfileInformationInput()
        {
            bool correct = true;
            //Hide everything first:
            lblOnDialysisError.Visible = false;
            lblKidneyDiseaseError.Visible = false;
            lblIssueStartedError.Visible = false;
            lblBloodTypeError.Visible = false;
            lblCountryError.Visible = false;
            lblCityError.Visible = false;
            lblStateError.Visible = false;
            lblAddressError.Visible = false;
            if(drpOnDialysis.SelectedIndex == 0)
            {
                correct = false;
                lblOnDialysisError.Text = "Invalid input: Please select whether you are on dialysis or not. ";
                lblOnDialysisError.Visible = true;
            }
            if(drpKidneyDisease.SelectedIndex == 0)
            {
                correct = false;
                lblKidneyDiseaseError.Text = "Invalid input: Please select your kidney disease stage.";
                lblKidneyDiseaseError.Visible = true;
            }
            string str_minimum_date = "1880-01-01";
            DateTime minimum_date = DateTime.ParseExact(str_minimum_date, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            if (calIssueStarted.SelectedDate == DateTime.MinValue)
            {
                correct = false;
                lblIssueStartedError.Text = "Invalid input: Please select the date when your issue started.";
                lblIssueStartedError.Visible = true;
            }
            else if (calIssueStarted.SelectedDate < minimum_date || calIssueStarted.SelectedDate >= DateTime.Now)
            {
                correct = false;
                lblIssueStartedError.Text = "Invalid input: Please select a reasonable date.";
                lblIssueStartedError.Visible = true;
            }
            if (drpBloodType.SelectedIndex == 0)
            {
                correct = false;
                lblBloodTypeError.Text = "Invalid input: Please select your blood type.";
                lblBloodTypeError.Visible = true;
            }
            if (drpCountry.SelectedIndex == 0)
            {
                correct = false;
                lblCountryError.Text = "Invalid input: Please select a country.";
                lblCountryError.Visible = true;
            }
            if (string.IsNullOrWhiteSpace(txtCity.Text))
            {
                correct = false;
                lblCityError.Text = "Invalid input: Please type a city.";
                lblCityError.Visible = true;
            }
            if (drpCountry.SelectedValue.Equals("United States"))
            {
                if(drpState.SelectedIndex == 0)
                {
                    correct = false;
                    lblStateError.Text = "Invalid input: Please select a state.";
                    lblStateError.Visible = true;
                }
            }
            else
            {
                if (string.IsNullOrWhiteSpace(txtState.Text))
                {
                    correct = false;
                    lblStateError.Text = "Invalid input: Please type a state or region.";
                    lblStateError.Visible = true;
                }
            }
            if (string.IsNullOrWhiteSpace(txtAddress.Text))
            {
                correct = false;
                lblAddressError.Text = "Invalid input: Please type your address.";
                lblAddressError.Visible = true;
            }
            return correct;
        }
        protected void btnSaveEditCompleteProfile_Click(object sender, EventArgs e)
        {
            lblSaveCompleteProfileMessage.Visible = false;
            //check for input errors:
            if (checkEditCompleteProfileInformationInput())
            {
                //update the new information and store it in the DB:
                connect.Open();
                SqlCommand cmd = connect.CreateCommand();
                cmd.CommandText = "select userId from Users where loginId = '" + loginId + "' ";
                string userId = cmd.ExecuteScalar().ToString();
                CompleteProfile completeProfile = new CompleteProfile(userId, userId);
                string completeProfileId = completeProfile.Id;
                string state = "";
                if (drpCountry.SelectedValue.Equals("United States"))
                    state = drpState.SelectedValue;
                else
                    state = txtState.Text.Replace("'", "''");
                int onDialysis = 0;
                if (drpOnDialysis.SelectedValue.Equals("No"))
                    onDialysis = 0;
                else
                    onDialysis = 1;
                cmd.CommandText = "update CompleteProfiles set completeProfile_onDialysis = '" + onDialysis + "', completeProfile_kidneyDisease ='" + drpKidneyDisease.SelectedValue + "'," +
                    "completeProfile_issueStartDate = '" + calIssueStarted.SelectedDate + "', completeProfile_bloodType = '" + drpBloodType.SelectedValue + "', completeProfile_city = '" + txtCity.Text.Replace("'", "''") + "', completeProfile_state = '" + state + "'," +
                    "completeProfile_zip = '" + txtZip.Text.Replace("'", "''") + "', completeProfile_address = '"+txtAddress.Text.Replace("'", "''")+"', completeProfile_country = '"+drpCountry.SelectedValue+ "'  where completeProfileId = '" + completeProfileId + "' ";
                cmd.ExecuteScalar();
                cmd.CommandText = "update Users set user_city = '"+txtCity.Text.Replace("'", "''") + "', user_state = '"+state+"', user_zip = '"+txtZip.Text.Replace("'", "''") + "'," +
                    "user_address = '"+txtAddress.Text.Replace("'", "''") + "', user_country = '"+drpCountry.SelectedValue+"' where userId = '"+userId+"'  ";
                cmd.ExecuteScalar();
                connect.Close();
                lblSaveCompleteProfileMessage.Visible = true;
            }
            getShortProfileInformation();
            getCompleteProfileInformation();
        }

        protected void btnCancelEditCompleteProfile_Click(object sender, EventArgs e)
        {
            viewProfiles();
            lblSaveCompleteProfileMessage.Visible = false;
        }

        protected void btnCancelTreatment_Click(object sender, EventArgs e)
        {
            showEditShortProfile();
            lblTreatmentsSavedSuccessfully.Visible = false;
            txtTypeTreatment.Text = "";
        }

                        
                        
                        

        protected void getCompleteProfileInformation()
        {
            string newLine = "<br/>";
            string col_start = "<td>", col_end = "</td>", row_start = "<tr>", row_end = "</tr>";
            connect.Open();
            SqlCommand cmd = connect.CreateCommand();
            cmd.CommandText = "select userId from Users where loginId = '" + loginId + "' ";
            string userId = cmd.ExecuteScalar().ToString();
            CompleteProfile completeProfile = new CompleteProfile(userId, userId);
            string completeProfileId = completeProfile.Id;
            string onDialysis = completeProfile.OnDialysis;
            string kidneyDisease = completeProfile.KidneyDisease;
            string issueDate = completeProfile.IssueStartDate;
            string bloodType = completeProfile.BloodType;
            string address = completeProfile.Address + newLine + "  " + completeProfile.City + ", " + completeProfile.State + " " + completeProfile.Zip +
                "<br/>" + completeProfile.Country;
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
                row += row_start + col_start + "Insurance phone1 : " + col_end + col_start + Layouts.phoneFormat(ins.Phone1) + col_end + row_end;
                row += row_start + col_start + "Insurance phone2 : " + col_end + col_start + Layouts.phoneFormat(ins.Phone2) + col_end + row_end;
                row += row_start + col_start + "Insurance email: " + col_end + col_start + ins.Email + col_end + row_end;
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
                row += row_start + col_start + "" + col_end + col_start + ++counter + ". Phone number: " + Layouts.phoneFormat(e.PhoneNumber);
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
                row += row_start + col_start + "Phone 1: " + col_end + col_start + Layouts.phoneFormat(e.Phone1) + col_end + row_end;
                row += row_start + col_start + "Phone 2: " + col_end + col_start + Layouts.phoneFormat(e.Phone2) + col_end + row_end;
                row += row_start + col_start + "Phone 3: " + col_end + col_start + Layouts.phoneFormat(e.Phone3) + col_end + row_end;
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
            connect.Close();
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
            ShortProfile shortProfile = new ShortProfile(userId, userId);
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
            if (blockedUsers.Count > 0)
            {
                row += row_start + col_start + "Blocked users: " + col_end + col_start + "" + col_end + row_end;
                for (int i = 0; i < blockedUsers.Count; i++)
                    row += row_start + col_start + "" + col_end + col_start + (i + 1) + ". " + blockedUsers[i].ToString() + col_end + row_end;
            }
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
    }
}