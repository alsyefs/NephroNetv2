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
using System.IO;
using System.Text;

namespace NephroNet.Accounts.Physician
{
    public partial class MyProfile : System.Web.UI.Page
    {
        static string conn = "";
        SqlConnection connect = new SqlConnection(conn);
        string username, roleId, loginId, token;
        static bool requestedToSaveProfileInformation = false;
        static bool requestedToViewProfileInformation = false;
        static bool requestedToViewEditProfileInformation = false;
        static protected string confirmedPassword = "";
        static List<string[]> experiences = new List<string[]>();
        protected void Page_Load(object sender, EventArgs e)
        {
            initialPageAccess();
            if (!Page.IsPostBack)
            {
                experiences = new List<string[]>();
                confirmedPassword = "";
                showInformation();
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
        protected void showInformation()
        {
            viewProfiles();
            //getEditCompleteProfileInformation();
            getCompleteProfileInformation();
        }
        //public override void VerifyRenderingInServerForm(Control control) { }
        //Methods to show and hide controls
        protected void viewProfiles()
        {
            View.Visible = true;
            EditCompleteProfile.Visible = false;
            lblSaveCompleteProfileMessage.Visible = false;
            UserAgreement.Visible = false;
            SetNewPassword.Visible = false;
            UpdatePassword.Visible = false;
            TypePassword.Visible = false;
        }
        protected void showEditCompleteProfile()
        {
            View.Visible = false;
            EditCompleteProfile.Visible = true;
            UserAgreement.Visible = false;
            SetNewPassword.Visible = false;
            UpdatePassword.Visible = false;
            TypePassword.Visible = false;
        }
        protected void showUserAgreement()
        {
            View.Visible = false;
            EditCompleteProfile.Visible = false;
            lblSaveCompleteProfileMessage.Visible = false;
            UserAgreement.Visible = true;
            SetNewPassword.Visible = false;
            UpdatePassword.Visible = false;
            TypePassword.Visible = false;
            //Lode the user agreement:
            string userAgreementText =
                "This website follows HIPAA" +
                "- Your information will not be shared with a 3rd part.\n" +
                "- If you set your profile to private, no one will be able to view your information except you.\n" +
                "- If you do not set your profile to private, system admins will be able to view your complete profile.\n" +
                "  This helps them to give recommendations based on your profile information.\n" +
                "- For more information, please contact the support team by email NephroNet2018@gmail.com\n\n" +
                "-------------------------------------------------------------------------------------------------" +
                "----------------------------The below was copied from: https://www.hhs.gov ----------------------";
            txtUserAgreement.Text = userAgreementText;
            //The below method will more text to the user agreement:
            setAgreementText();
            lblAgree.Text = "I agree to the terms and conditions";
            chkAgree.Visible = true;
            chkAgree.Checked = false;
        }
        protected void showSetNewPassword()
        {
            View.Visible = false;
            EditCompleteProfile.Visible = false;
            UserAgreement.Visible = false;
            SetNewPassword.Visible = true;
            UpdatePassword.Visible = false;
            TypePassword.Visible = false;
        }
        protected void showUpdatePassword()
        {
            View.Visible = false;
            EditCompleteProfile.Visible = false;
            UserAgreement.Visible = false;
            SetNewPassword.Visible = false;
            UpdatePassword.Visible = true;
            TypePassword.Visible = false;
        }
        protected void showEnterPassword()
        {
            View.Visible = false;
            EditCompleteProfile.Visible = false;
            UserAgreement.Visible = false;
            SetNewPassword.Visible = false;
            UpdatePassword.Visible = false;
            TypePassword.Visible = true;
        }
        protected void btnCompleteProfile_Click(object sender, EventArgs e)
        {
            showEditCompleteProfile();
            getEditCompleteProfileInformation();
        }
        protected void getEditCompleteProfileInformation()
        {
            lblPrivateMessage.Visible = true;
            connect.Open();
            SqlCommand cmd = connect.CreateCommand();
            cmd.CommandText = "select userId from Users where loginId = '" + loginId + "' ";
            string userId = cmd.ExecuteScalar().ToString();
            cmd.CommandText = "select count(PhysicianCompleteProfile_password) from PhysicianCompleteProfiles where userId = '" + userId + "' ";
            int thereIsPassword = Convert.ToInt32(cmd.ExecuteScalar());
            connect.Close();
            if (thereIsPassword > 0)//1 is equivalent to true; 0 => false;
            {
                btnSetNewPassword.Visible = false;
                btnUpdatePassword.Visible = true;
            }
            else
            {
                btnSetNewPassword.Visible = true;
                btnUpdatePassword.Visible = false;
            }
            PhysicianCompleteProfile completeProfile = new PhysicianCompleteProfile(userId, userId);
            string completeProfileId = completeProfile.ID;
            int isPrivate = completeProfile.Private;
            string dialysis = completeProfile.Dialysis;
            string homeDialysis = completeProfile.HomeDialysis;
            string transplantation = completeProfile.Transplantation;
            string hypertension = completeProfile.Hypertension;
            string gN = completeProfile.GN;
            string physicianId = completeProfile.PhysicianID;
            int numberOfExperiences = 0;
            if (experiences != null && experiences.Count > 0)
                experiences.Clear();
            if (completeProfile.Experience != null && completeProfile.Experience.Count > 0)
            {
                experiences = completeProfile.Experience;
                numberOfExperiences = completeProfile.Experience.Count;
            }
            if (drpExperience.Items.Count > 0)
                drpExperience.Items.Clear();
            if (isPrivate == 1)
            {
                chkIsPrivate.Checked = true;
                //Check if the password has been entered to view the information:
                if (string.IsNullOrWhiteSpace(confirmedPassword))
                {
                    showEnterPassword();
                    requestedToViewEditProfileInformation = true;
                }
                else
                {
                    //Now, decrypt using the encryption key:
                    //string decryptionKey = txtPassword.Text;
                    string decryptionKey = confirmedPassword;
                    dialysis = Encryption.decrypt(dialysis, decryptionKey);
                    homeDialysis = Encryption.decrypt(homeDialysis, decryptionKey);
                    transplantation = Encryption.decrypt(transplantation, decryptionKey);
                    hypertension = Encryption.decrypt(hypertension, decryptionKey);
                    gN = Encryption.decrypt(gN, decryptionKey);
                    physicianId = Encryption.decrypt(physicianId, decryptionKey);
                    if (chkIsPrivate.Checked)
                        lblPrivateMessage.Text = "Your profile will become private";
                    else
                        lblPrivateMessage.Text = "Only Admins can view your profile";
                    if (!string.IsNullOrWhiteSpace(dialysis))
                        txtDialysis.Text = dialysis;
                    if (!string.IsNullOrWhiteSpace(homeDialysis))
                        txtHomeDialysis.Text = homeDialysis;
                    if (!string.IsNullOrWhiteSpace(transplantation))
                        txtTransplantation.Text = transplantation;
                    if (!string.IsNullOrWhiteSpace(hypertension))
                        txtHypertension.Text = hypertension;
                    if (!string.IsNullOrWhiteSpace(gN))
                        txtGN.Text = gN;
                    if (!string.IsNullOrWhiteSpace(physicianId))
                        txtPhysicianId.Text = physicianId;
                    if (experiences != null && experiences.Count > 0)
                    {
                        //Copy the list to a new temp one:
                        List<string[]> temp = new List<string[]>(experiences);
                        //Clear the encrypted data to refill it with decrypted data:
                        experiences.Clear();
                        for (int i = 0; i < numberOfExperiences; i++)
                        {
                            string[] result = new string[] {
                                Encryption.decrypt(temp[i][0], decryptionKey),
                                Encryption.decrypt(temp[i][1], decryptionKey),
                                Encryption.decrypt(temp[i][2], decryptionKey),
                                Encryption.decrypt(temp[i][3], decryptionKey) };
                            experiences.Add(result);
                            drpExperience.Items.Add(string.Join(" ", result));
                        }
                        //for (int i = 0; i < numberOfExperiences; i++)
                        //{
                        //    string[] result = new string[] { Encryption.decrypt(experiences[i][0], decryptionKey), Encryption.decrypt(experiences[i][1], decryptionKey),
                        //        Encryption.decrypt(experiences[i][2], decryptionKey), Encryption.decrypt(experiences[i][3], decryptionKey) };
                        //    //experiences.Add(result);
                        //    drpExperience.Items.Add(string.Join(" ", result));
                        //}
                    }
                    txtPassword.Text = "";
                    requestedToViewEditProfileInformation = false;
                }
            }
            else
            {
                chkIsPrivate.Checked = false;
                if (chkIsPrivate.Checked)
                    lblPrivateMessage.Text = "Your account will become private";
                else
                    lblPrivateMessage.Text = "Only Admins can view your account";
                if (!string.IsNullOrWhiteSpace(dialysis))
                    txtDialysis.Text = dialysis;
                if (!string.IsNullOrWhiteSpace(homeDialysis))
                    txtHomeDialysis.Text = homeDialysis;
                if (!string.IsNullOrWhiteSpace(transplantation))
                    txtTransplantation.Text = transplantation;
                if (!string.IsNullOrWhiteSpace(hypertension))
                    txtHypertension.Text = hypertension;
                if (!string.IsNullOrWhiteSpace(gN))
                    txtGN.Text = gN;
                if (!string.IsNullOrWhiteSpace(physicianId))
                    txtPhysicianId.Text = physicianId;
                if(experiences != null)
                {
                    ////REMOVE
                    //string decryptionKey = confirmedPassword;
                    //for (int i = 0; i < numberOfExperiences; i++)
                    //{
                    //    string[] result = new string[] { Encryption.decrypt(experiences[i][0], decryptionKey), Encryption.decrypt(experiences[i][1], decryptionKey),
                    //            Encryption.decrypt(experiences[i][2], decryptionKey), Encryption.decrypt(experiences[i][3], decryptionKey) };
                    //    experiences.Add(result);
                    //    drpExperience.Items.Add(string.Join(" ", result));
                    //}
                    ////REMOVE
                    for (int i=0; i < numberOfExperiences; i++)
                    {
                        string[] result = new string[] { experiences[i][0], experiences[i][1], experiences[i][2], experiences[i][3] };
                        //experiences.Add(result);
                        drpExperience.Items.Add(string.Join(" ", result));
                    }
                }
                txtPassword.Text = "";
                requestedToViewEditProfileInformation = false;
            }
        }
        protected bool checkEditCompleteProfileInformationInput()
        {
            bool correct = true;
            //Hide everything first:
            lblDialysisError.Visible = false;
            lblHomeDialysisError.Visible = false;
            lblTransplantationError.Visible = false;
            lblHypertensionError.Visible = false;
            lblGNError.Visible = false;
            lblPhysicianIdError.Visible = false;
            //There is nothing to validate for now!
            return correct;
        }
        protected void chkIsPrivate_CheckedChanged(object sender, EventArgs e)
        {
            lblPrivateMessage.Visible = true;
            if (chkIsPrivate.Checked)
                lblPrivateMessage.Text = "Your account will become private";
            else
                lblPrivateMessage.Text = "Only Admins can view your account";
        }
        protected void chkAgree_CheckedChanged(object sender, EventArgs e)
        {

        }
        protected void btnAgree_Click(object sender, EventArgs e)
        {
            //If the checkbox is not checked, show an error:
            if (!chkAgree.Checked)
            {
                lblAgreeError.Visible = true;
                lblAgreeError.Text = "Please, click on the check box that you agree on the terms and conditions";
            }
            else
            {
                //If the checkbock is checked, store the new information and hide the user agreement:
                //save:
                lblSaveCompleteProfileMessage.Visible = false;
                //check for input errors:
                if (checkEditCompleteProfileInformationInput())
                {
                    //update the new information and store it in the DB:
                    connect.Open();
                    SqlCommand cmd = connect.CreateCommand();
                    cmd.CommandText = "select userId from Users where loginId = '" + loginId + "' ";
                    string userId = cmd.ExecuteScalar().ToString();
                    PhysicianCompleteProfile completeProfile = new PhysicianCompleteProfile(userId, userId);
                    string completeProfileId = completeProfile.ID;
                    //replace the single quote with double quotes in all inputs.
                    string dialysis = txtDialysis.Text.Replace("'", "''");
                    string homeDialysis = txtHomeDialysis.Text.Replace("'", "''");
                    string transplantation = txtTransplantation.Text.Replace("'", "''");
                    string hypertension = txtHypertension.Text.Replace("'", "''");
                    string gN = txtGN.Text.Replace("'", "''");
                    string physicianId = txtPhysicianId.Text.Replace("'", "''");
                    int numberOfExperiences = 0;
                    if (drpExperience.Items != null && drpExperience.Items.Count > 0)
                        numberOfExperiences = drpExperience.Items.Count;
                    int isPrivate = 0;
                    if (chkIsPrivate.Checked)
                    {
                        isPrivate = 1;
                        //Check if the password has been entered to view the information:
                        if (string.IsNullOrWhiteSpace(confirmedPassword))
                        {
                            showEnterPassword();
                            requestedToSaveProfileInformation = true;
                        }
                        else
                        {
                            //Now, encrypt using the encryption key:
                            string encryptionKey = confirmedPassword;
                            dialysis = Encryption.encrypt(dialysis, encryptionKey);
                            homeDialysis = Encryption.encrypt(homeDialysis, encryptionKey);
                            transplantation = Encryption.encrypt(transplantation, encryptionKey);
                            hypertension = Encryption.encrypt(hypertension, encryptionKey);
                            gN = Encryption.encrypt(gN, encryptionKey);
                            physicianId = Encryption.encrypt(physicianId, encryptionKey);
                            //update the record in the database.
                            cmd.CommandText = "update [PhysicianCompleteProfiles] set [physicianCompleteProfile_Dialysis] = '" + dialysis + "', [physicianCompleteProfile_homeDialysis] = '" + homeDialysis + "', " +
                                "[physicianCompleteProfile_transplantation] = '" + transplantation + "', [physicianCompleteProfile_hypertension] = '" + hypertension + "', [physicianCompleteProfile_GN] = '" + gN + "', " +
                                "[physicianCompleteProfile_physicianId] = '" + physicianId + "', [physicianCompleteProfile_isPrivate] = '" + isPrivate + "'" +
                                " where [physicianCompleteProfileId] = '" + completeProfileId + "' ";
                            cmd.ExecuteScalar();
                            //if there is a related record in another table, update it.
                            //To update a list of records, delete all old records:
                            cmd.CommandText = "delete from PhysicianExperiences where [physicianCompleteProfileId] = '" + completeProfileId + "' ";
                            cmd.ExecuteScalar();
                            //Now, insert the new values:
                            for (int i = 0; i < experiences.Count; i++)
                            {
                                cmd.CommandText = "insert into PhysicianExperiences (physicianExperience_hospitalName, physicianExperience_hospitalAddress, " +
                                    "physicianExperience_fromYear, physicianExperience_toYear, physicianCompleteProfileId) values " +
                                    "('"+ Encryption.encrypt(experiences[i][0], encryptionKey).Replace("'", "''")+ "', '" + Encryption.encrypt(experiences[i][1], encryptionKey).Replace("'", "''") +
                                    "', '" + Encryption.encrypt(experiences[i][2], encryptionKey).Replace("'", "''") + "', '" + Encryption.encrypt(experiences[i][3], encryptionKey).Replace("'", "''") + "', '" + completeProfileId + "')";
                                cmd.ExecuteScalar();
                            }
                            lblSaveCompleteProfileMessage.Visible = true;
                            txtPassword.Text = "";
                            requestedToSaveProfileInformation = false;
                        }
                    }
                    else
                    {
                        isPrivate = 0;
                        //update the record in the database.
                        cmd.CommandText = "update [PhysicianCompleteProfiles] set [physicianCompleteProfile_Dialysis] = '" + dialysis + "', [physicianCompleteProfile_homeDialysis] = '" + homeDialysis + "', " +
                            "[physicianCompleteProfile_transplantation] = '" + transplantation + "', [physicianCompleteProfile_hypertension] = '" + hypertension + "', [physicianCompleteProfile_GN] = '" + gN + "', " +
                            "[physicianCompleteProfile_physicianId] = '" + physicianId + "', [physicianCompleteProfile_isPrivate] = '" + isPrivate + "'" +
                            " where [physicianCompleteProfileId] = '" + completeProfileId + "' ";
                        cmd.ExecuteScalar();
                        //if there is a related record in another table, update it.
                        //To update a list of records, delete all old records:
                        cmd.CommandText = "delete from PhysicianExperiences where [physicianCompleteProfileId] = '" + completeProfileId + "' ";
                        cmd.ExecuteScalar();
                        //Now, insert the new values:
                        for (int i = 0; i < experiences.Count; i++)
                        {
                            cmd.CommandText = "insert into PhysicianExperiences (physicianExperience_hospitalName, physicianExperience_hospitalAddress, " +
                                "physicianExperience_fromYear, physicianExperience_toYear, physicianCompleteProfileId) values " +
                                "('" + experiences[i][0].Replace("'", "''") + "', '" + experiences[i][1].Replace("'", "''") +
                                "', '" + experiences[i][2].Replace("'", "''") + "', '" + experiences[i][3].Replace("'", "''") + "', '" + completeProfileId + "')";
                            cmd.ExecuteScalar();
                        }
                        lblSaveCompleteProfileMessage.Visible = true;
                    }
                }
                connect.Close();
                //getCompleteProfileInformation();
                getEditCompleteProfileInformation();
                //Hide the user agreement:
                showEditCompleteProfile();
            }
        }
        protected void btnDisagree_Click(object sender, EventArgs e)
        {
            //Hide the user agreement and do nothing:
            showEditCompleteProfile();
            requestedToSaveProfileInformation = false;
        }
        protected void setAgreementText()
        {
            string line;
            try
            {
                string filePath = System.Web.HttpContext.Current.Request.PhysicalApplicationPath;
                StringBuilder text = new StringBuilder();
                //Pass the file path and file name to the StreamReader constructor
                using (StreamReader sr = new StreamReader(filePath + @"\Content\HIPPA_regulations.txt", Encoding.Default))
                {
                    while ((line = sr.ReadLine()) != null)
                    {
                        text.Append(line + "\n");
                    }
                }
                //Append the text from the text-file to the output:
                txtUserAgreement.Text += text.ToString();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }
            finally
            {
                Console.WriteLine("Executing finally block.");
            }
        }
        protected bool correctNewPassword()
        {
            bool correct = true;
            if (string.IsNullOrWhiteSpace(txtPassword1.Text))
            {
                lblPassword1Error.Text = "Invalid input: Please type a password.";
                lblPassword1Error.Visible = true;
                correct = false;
            }
            if (string.IsNullOrWhiteSpace(txtPassword2.Text))
            {
                lblPassword2Error.Text = "Invalid input: Please type a password.";
                lblPassword2Error.Visible = true;
                correct = false;
            }
            if (correct)
            {
                if (!txtPassword1.Text.Equals(txtPassword2.Text))
                {
                    lblPassword2Error.Text = "Invalid input: Your passwords do not match.";
                    lblPassword2Error.Visible = true;
                    correct = false;
                }
            }
            if (correct)
            {
                if (txtPassword1.Text.Length < 4)
                {
                    lblPassword1Error.Text = "Invalid input: Please type a password that is at least four characters.";
                    lblPassword1Error.Visible = true;
                    correct = false;
                }
                if (txtPassword2.Text.Length < 4)
                {
                    lblPassword2Error.Text = "Invalid input: Please type a password that is at least four characters.";
                    lblPassword2Error.Visible = true;
                    correct = false;
                }
            }
            if (correct)
            {
                if (txtPassword1.Text.Contains("'"))
                {
                    lblPassword1Error.Text = "Invalid input: Please type a password without the single quotation character.";
                    lblPassword1Error.Visible = true;
                    correct = false;
                }
                if (txtPassword2.Text.Contains("'"))
                {
                    lblPassword2Error.Text = "Invalid input: Please type a password without the single quotation character.";
                    lblPassword2Error.Visible = true;
                    correct = false;
                }
            }
            return correct;
        }
        protected void btnSaveNewPassword_Click(object sender, EventArgs e)
        {
            bool stored = false;
            //Check the inputs of the new password:
            if (correctNewPassword())
            {
                confirmedPassword = txtPassword1.Text;
                //Store the new password:
                connect.Open();
                SqlCommand cmd = connect.CreateCommand();
                cmd.CommandText = "select userId from Users where loginId = '" + loginId + "' ";
                string userId = cmd.ExecuteScalar().ToString();
                cmd.CommandText = "update PhysicianCompleteProfiles set PhysicianCompleteProfile_password = '" + Encryption.hash(confirmedPassword) + "' where userId = '" + userId + "'  ";
                cmd.ExecuteScalar();
                connect.Close();
                stored = true;
            }
            if (stored)
            {
                //Show the successful message, but don't go back! Let the user click on "Go Back".
                lblSaveNewPasswordMessage.Visible = true;
                lblPassword1Error.Visible = false;
                lblPassword2Error.Visible = false;
                txtPassword1.Text = "";
                txtPassword2.Text = "";
            }
            else
                lblSaveNewPasswordMessage.Visible = false;
        }
        protected void btnCancelNewPassword_Click(object sender, EventArgs e)
        {
            lblSaveNewPasswordMessage.Visible = false;
            showEditCompleteProfile();
            getEditCompleteProfileInformation();
        }
        protected void btnSetNewPassword_Click(object sender, EventArgs e)
        {
            lblSaveNewPasswordMessage.Visible = false;
            showSetNewPassword();
        }
        protected void btnUpdatePassword_Click(object sender, EventArgs e)
        {
            lblUpdatePasswordMessage.Visible = false;
            showUpdatePassword();
            getEditCompleteProfileInformation();
        }
        protected bool correctUpdatePassword()
        {
            bool correct = true;
            if (string.IsNullOrWhiteSpace(txtOldPassword.Text))
            {
                lblOldPasswordError.Text = "Invalid input: Please type your old password.";
                lblOldPasswordError.Visible = true;
                correct = false;
            }
            else
            {
                string hashed = Encryption.hash(txtOldPassword.Text);
                connect.Open();
                SqlCommand cmd = connect.CreateCommand();
                cmd.CommandText = "select userId from Users where loginId = '" + loginId + "' ";
                string userId = cmd.ExecuteScalar().ToString();
                cmd.CommandText = "select count(PhysicianCompleteProfile_password) from PhysicianCompleteProfiles where userId = '" + userId + "' ";
                int thereIsPassword = Convert.ToInt32(cmd.ExecuteScalar());
                if (thereIsPassword > 0)
                {
                    cmd.CommandText = "select PhysicianCompleteProfile_password from PhysicianCompleteProfiles where userId = '" + userId + "' ";
                    string hashedOld = cmd.ExecuteScalar().ToString();
                    if (!hashed.Equals(hashedOld))
                    {
                        lblOldPasswordError.Text = "Invalid input: Please type your correct old password.";
                        lblOldPasswordError.Visible = true;
                        correct = false;
                    }
                }
                connect.Close();
            }
            if (string.IsNullOrWhiteSpace(txtUpdatePassword1.Text))
            {
                lblUpdatePassword1Error.Text = "Invalid input: Please type a password.";
                lblUpdatePassword1Error.Visible = true;
                correct = false;
            }
            if (string.IsNullOrWhiteSpace(txtUpdatePassword2.Text))
            {
                lblUpdatePassword2Error.Text = "Invalid input: Please type a password.";
                lblUpdatePassword2Error.Visible = true;
                correct = false;
            }
            if (correct)
            {
                if (!txtUpdatePassword1.Text.Equals(txtUpdatePassword2.Text))
                {
                    lblUpdatePassword2Error.Text = "Invalid input: Your passwords do not match.";
                    lblUpdatePassword2Error.Visible = true;
                    correct = false;
                }
            }
            if (correct)
            {
                if (txtUpdatePassword1.Text.Length < 4)
                {
                    lblUpdatePassword1Error.Text = "Invalid input: Please type a password that is at least four characters.";
                    lblUpdatePassword1Error.Visible = true;
                    correct = false;
                }
                if (txtUpdatePassword2.Text.Length < 4)
                {
                    lblUpdatePassword2Error.Text = "Invalid input: Please type a password that is at least four characters.";
                    lblUpdatePassword2Error.Visible = true;
                    correct = false;
                }
            }
            if (correct)
            {
                if (txtUpdatePassword1.Text.Contains("'"))
                {
                    lblUpdatePassword1Error.Text = "Invalid input: Please type a password without the single quotation character.";
                    lblUpdatePassword1Error.Visible = true;
                    correct = false;
                }
                if (txtUpdatePassword2.Text.Contains("'"))
                {
                    lblUpdatePassword2Error.Text = "Invalid input: Please type a password without the single quotation character.";
                    lblUpdatePassword2Error.Visible = true;
                    correct = false;
                }
            }
            return correct;
        }
        protected void decryptThenEncrypt(string oldPassword, string newPassword)
        {
            if (!string.IsNullOrWhiteSpace(oldPassword) && !string.IsNullOrWhiteSpace(newPassword))
            {
                connect.Open();
                SqlCommand cmd = connect.CreateCommand();
                cmd.CommandText = "select userId from Users where loginId = '" + loginId + "' ";
                string userId = cmd.ExecuteScalar().ToString();
                connect.Close();
                PhysicianCompleteProfile completeProfile = new PhysicianCompleteProfile(userId, userId);
                string completeProfileId = completeProfile.ID;
                int isPrivate = completeProfile.Private;
                string dialysis = completeProfile.Dialysis;
                string homeDialysis = completeProfile.HomeDialysis;
                string transplantation = completeProfile.Transplantation;
                string hypertension = completeProfile.Hypertension;
                string gN = completeProfile.GN;
                string physicianId = completeProfile.PhysicianID;
                //Decrypt using the old encryption key:
                string decryptionKey = oldPassword;
                dialysis = Encryption.decrypt(dialysis, decryptionKey);
                homeDialysis = Encryption.decrypt(homeDialysis, decryptionKey);
                transplantation = Encryption.decrypt(transplantation, decryptionKey);
                hypertension = Encryption.decrypt(hypertension, decryptionKey);
                gN = Encryption.decrypt(gN, decryptionKey);
                physicianId = Encryption.decrypt(physicianId, decryptionKey);
                //Encrypt using the new encryption key:
                string encryptionKey = newPassword;
                dialysis = Encryption.encrypt(dialysis, encryptionKey);
                homeDialysis = Encryption.encrypt(homeDialysis, encryptionKey);
                transplantation = Encryption.encrypt(transplantation, encryptionKey);
                hypertension = Encryption.encrypt(hypertension, encryptionKey);
                gN = Encryption.encrypt(gN, encryptionKey);
                physicianId = Encryption.encrypt(physicianId, encryptionKey);
                connect.Open();
                //update the record in the database.
                cmd.CommandText = "update [PhysicianCompleteProfiles] set [physicianCompleteProfile_Dialysis] = '" + dialysis + "', [physicianCompleteProfile_homeDialysis] = '" + homeDialysis + "', " +
                    "[physicianCompleteProfile_transplantation] = '" + transplantation + "', [physicianCompleteProfile_hypertension] = '" + hypertension + "', [physicianCompleteProfile_GN] = '" + gN + "', " +
                    "[physicianCompleteProfile_physicianId] = '" + physicianId + "', [physicianCompleteProfile_isPrivate] = '" + isPrivate + "'" +
                    " where [physicianCompleteProfileId] = '" + completeProfileId + "' ";
                cmd.ExecuteScalar();
                connect.Close();
            }
        }
        protected void btnSaveUpdatePassword_Click(object sender, EventArgs e)
        {
            bool stored = false;
            //Check the inputs of the new password:
            if (correctUpdatePassword())
            {
                confirmedPassword = txtUpdatePassword1.Text;
                SqlCommand cmd = connect.CreateCommand();
                connect.Open();
                cmd.CommandText = "select userId from Users where loginId = '" + loginId + "' ";
                string userId = cmd.ExecuteScalar().ToString();
                connect.Close();
                PhysicianCompleteProfile completeProfile = new PhysicianCompleteProfile(userId, userId);
                string completeProfileId = completeProfile.ID;
                int isPrivate = completeProfile.Private;
                if (isPrivate == 1)
                    decryptThenEncrypt(txtOldPassword.Text, confirmedPassword);
                //Store the new password:
                connect.Open();
                cmd.CommandText = "update PhysicianCompleteProfiles set PhysicianCompleteProfile_password = '" + Encryption.hash(confirmedPassword) + "' where userId = '" + userId + "'  ";
                cmd.ExecuteScalar();
                connect.Close();
                stored = true;
            }
            if (stored)
            {
                //Show the successful message, but don't go back! Let the user click on "Go Back".
                lblUpdatePasswordMessage.Visible = true;
                lblOldPasswordError.Visible = false;
                lblUpdatePassword1Error.Visible = false;
                lblUpdatePassword2Error.Visible = false;
                txtOldPassword.Text = "";
                txtUpdatePassword1.Text = "";
                txtUpdatePassword2.Text = "";
            }
            else
                lblUpdatePasswordMessage.Visible = false;
        }
        protected void btnCancelUpdatePassword_Click(object sender, EventArgs e)
        {
            lblUpdatePasswordMessage.Visible = false;
            showEditCompleteProfile();
            getEditCompleteProfileInformation();
        }
        protected bool correctStoredPassword()
        {
            bool correct = true;
            if (string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                lblPasswordError.Text = "Invalid input: Please type your old password.";
                lblPasswordError.Visible = true;
                correct = false;
            }
            else
            {
                string hashed = Encryption.hash(txtPassword.Text);
                connect.Open();
                SqlCommand cmd = connect.CreateCommand();
                cmd.CommandText = "select userId from Users where loginId = '" + loginId + "' ";
                string userId = cmd.ExecuteScalar().ToString();
                cmd.CommandText = "select count(physicianCompleteProfile_password) from PhysicianCompleteProfiles where userId = '" + userId + "' ";
                int thereIsPassword = Convert.ToInt32(cmd.ExecuteScalar());
                if (thereIsPassword > 0)
                {
                    cmd.CommandText = "select physicianCompleteProfile_password from PhysicianCompleteProfiles where userId = '" + userId + "' ";
                    string hashedOld = cmd.ExecuteScalar().ToString();
                    if (!hashed.Equals(hashedOld))
                    {
                        lblPasswordError.Text = "Invalid input: Please type your correct profile password.";
                        lblPasswordError.Visible = true;
                        correct = false;
                    }
                }
                connect.Close();
            }
            return correct;
        }
        protected void btnSubmitPassword_Click(object sender, EventArgs e)
        {
            //check the password:
            if (correctStoredPassword())
            {
                confirmedPassword = txtPassword.Text;
                txtPassword.Text = "";
                if (requestedToSaveProfileInformation)
                    showUserAgreement();
                else if (requestedToViewProfileInformation)
                {
                    viewProfiles();
                    getCompleteProfileInformation();
                }
                else if (requestedToViewEditProfileInformation)
                {
                    showEditCompleteProfile();
                    getEditCompleteProfileInformation();
                }
                lblPasswordError.Visible = false;
            }
        }
        protected bool checkExperienceInput()
        {
            lblHospitalNameError.Visible = false;
            lblHospitalAddressError.Visible = false;
            lblYearsOfExperienceFrom.Visible = false;
            bool correct = true;
            if (string.IsNullOrWhiteSpace(txtHospitalName.Text))
            {
                correct = false;
                lblHospitalNameError.Visible = true;
                lblHospitalNameError.Text = "Invalid input: Please type the hospital name.";
            }
            if (string.IsNullOrWhiteSpace(txtHospitalAddress.Text))
            {
                correct = false;
                lblHospitalAddressError.Visible = true;
                lblHospitalAddressError.Text = "Invalid input: Please type the hospital address.";
            }
            if (string.IsNullOrWhiteSpace(txtYearsOfExperienceFrom.Text))
            {
                correct = false;
                lblYearsOfExperienceError.Visible = true;
                lblYearsOfExperienceError.Text = "Invalid input: Please type the year \"From\".";
            }
            else
            {
                if (Convert.ToInt32(txtYearsOfExperienceFrom.Text) < 1900 || Convert.ToInt32(txtYearsOfExperienceFrom.Text) > DateTime.Now.Year)
                {
                    correct = false;
                    lblYearsOfExperienceError.Visible = true;
                    lblYearsOfExperienceError.Text = "Invalid input: Please type a year between 1900 to the current year in \"From\".";
                }
            }
            if (string.IsNullOrWhiteSpace(txtYearsOfExperienceTo.Text))
            {
                correct = false;
                lblYearsOfExperienceError.Visible = true;
                lblYearsOfExperienceError.Text = "Invalid input: Please type the year \"To\".";
            }
            else
            {
                if (Convert.ToInt32(txtYearsOfExperienceTo.Text) < 1900 || Convert.ToInt32(txtYearsOfExperienceTo.Text) > DateTime.Now.Year)
                {
                    correct = false;
                    lblYearsOfExperienceError.Visible = true;
                    lblYearsOfExperienceError.Text = "Invalid input: Please type a year between 1900 to the current year in \"To\".";
                }
            }
            if (correct)
            {
                if (Convert.ToInt32(txtYearsOfExperienceTo.Text) < Convert.ToInt32(txtYearsOfExperienceFrom.Text))
                {
                    correct = false;
                    lblYearsOfExperienceError.Visible = true;
                    lblYearsOfExperienceError.Text = "Invalid input: Please type a year in \"From\" less than or equal to a year in \"To\".";
                }
            }
            return correct;
        }
        protected void btnAddExperience_Click(object sender, EventArgs e)
        {
            if(checkExperienceInput())
            {
                //string hospitalName = txtHospitalName.Text.Replace("'", "''");
                //string hospitalAddress = txtHospitalAddress.Text.Replace("'", "''");
                //string yearFrom = txtYearsOfExperienceFrom.Text.Replace("'", "''");
                //string yearTo = txtYearsOfExperienceTo.Text.Replace("'", "''");
                string hospitalName = txtHospitalName.Text;
                string hospitalAddress = txtHospitalAddress.Text;
                string yearFrom = txtYearsOfExperienceFrom.Text;
                string yearTo = txtYearsOfExperienceTo.Text;
                string[] result = new string[] { hospitalName, hospitalAddress, yearFrom, yearTo };
                try
                {
                    experiences.Add(result);
                    //string [] test = experiences[1];
                    //result = result.Except(new string[] { result[0] }).ToArray();
                    drpExperience.Items.Add(string.Join(" ", result));
                    txtHospitalName.Text = "";
                    txtHospitalAddress.Text = "";
                    txtYearsOfExperienceFrom.Text = "";
                    txtYearsOfExperienceTo.Text = "";
                }
                catch(Exception ex)
                {
                    Console.WriteLine("Error: "+ ex);
                }
                
            }

        }
        protected void btnRemoveExperience_Click(object sender, EventArgs e)
        {

            for (int i = drpExperience.Items.Count; i >=0 ; i--)
            {
                int indexToRemove = drpExperience.SelectedIndex;
                if (indexToRemove > -1)
                {
                    drpExperience.Items.RemoveAt(indexToRemove);
                    experiences.RemoveAt(indexToRemove);
                }
            }
            //for (int i = 0; i < drpExperience.Items.Count; i++)
            //{
            //    int indexToRemove = drpExperience.SelectedIndex;
            //    if (indexToRemove > -1)
            //    {
            //        drpExperience.Items.RemoveAt(indexToRemove);
            //        experiences.RemoveAt(indexToRemove);
            //    }
            //}
        }
        protected void drpExperience_SelectedIndexChanged(object sender, EventArgs e)
        {

        }


        protected void btnSaveEditCompleteProfile_Click(object sender, EventArgs e)
        {
            requestedToSaveProfileInformation = true;
            if (chkIsPrivate.Checked)
            {
                connect.Open();
                SqlCommand cmd = connect.CreateCommand();
                cmd.CommandText = "select userId from Users where loginId = '" + loginId + "' ";
                string userId = cmd.ExecuteScalar().ToString();
                cmd.CommandText = "select count(physicianCompleteProfile_password) from PhysicianCompleteProfiles where userId = '" + userId + "' ";
                int thereIsPassword = Convert.ToInt32(cmd.ExecuteScalar());
                connect.Close();
                if (thereIsPassword > 0)//1 is equivalent to true; 0 => false;
                {
                    showEnterPassword();
                }
                else
                {
                    showSetNewPassword();
                }
            }
            else
                showUserAgreement();
        }
        protected void btnCancelEditCompleteProfile_Click(object sender, EventArgs e)
        {
            viewProfiles();
            getCompleteProfileInformation();
            lblSaveCompleteProfileMessage.Visible = false;
        }
        protected void getCompleteProfileInformation()
        {
            lblRow.Text = "";
            string col_start = "<td>", col_end = "</td>", row_start = "<tr>", row_end = "</tr>";
            connect.Open();
            SqlCommand cmd = connect.CreateCommand();
            cmd.CommandText = "select userId from Users where loginId = '" + loginId + "' ";
            string userId = cmd.ExecuteScalar().ToString();
            connect.Close();
            PhysicianCompleteProfile completeProfile = new PhysicianCompleteProfile(userId, userId);
            string completeProfileId = completeProfile.ID;
            int isPrivate = completeProfile.Private;
            string dialysis = completeProfile.Dialysis;
            string homeDialysis = completeProfile.HomeDialysis;
            string transplantation = completeProfile.Transplantation;
            string hypertension = completeProfile.Hypertension;
            string gN = completeProfile.GN;
            string physicianId = completeProfile.PhysicianID;
            string str_isPrivate = "";
            if(completeProfile.Experience != null)
                experiences = completeProfile.Experience;
            if (drpExperience.Items.Count > 0)
                drpExperience.Items.Clear();
            if (isPrivate == 1)
                str_isPrivate = "Private";
            else
                str_isPrivate = "Viewable by Admins";
            if (isPrivate == 1)
            {
                //Check if the password has been entered to view the information:
                if (string.IsNullOrWhiteSpace(confirmedPassword))
                {
                    showEnterPassword();
                    requestedToViewProfileInformation = true;
                }
                else
                {
                    //Now, decrypt using the encryption key:
                    string decryptionKey = confirmedPassword;
                    dialysis = Encryption.decrypt(dialysis, decryptionKey);
                    homeDialysis = Encryption.decrypt(homeDialysis, decryptionKey);
                    transplantation = Encryption.decrypt(transplantation, decryptionKey);
                    hypertension = Encryption.decrypt(hypertension, decryptionKey);
                    gN = Encryption.decrypt(gN, decryptionKey);
                    physicianId = Encryption.decrypt(physicianId, decryptionKey);
                    string row = "";
                    row += row_start + col_start + "Physician Complete Profile Information: " + col_end + row_end;
                    row += row_start + col_start + "Account is: " + col_end + col_start + str_isPrivate + col_end + row_end;
                    if (!string.IsNullOrWhiteSpace(dialysis))
                        row += row_start + col_start + "Dialysis: " + col_end + col_start + dialysis + col_end + row_end;
                    if (!string.IsNullOrWhiteSpace(homeDialysis))
                        row += row_start + col_start + "Hemodialysis: " + col_end + col_start + homeDialysis + col_end + row_end;
                    if (!string.IsNullOrWhiteSpace(transplantation))
                        row += row_start + col_start + "Transplantation: " + col_end + col_start + transplantation + col_end + row_end;
                    if (!string.IsNullOrWhiteSpace(hypertension))
                        row += row_start + col_start + "Hypertension: " + col_end + col_start + hypertension + col_end + row_end;
                    if (!string.IsNullOrWhiteSpace(gN))
                        row += row_start + col_start + "GN: " + col_end + col_start + gN + col_end + row_end;
                    if (!string.IsNullOrWhiteSpace(physicianId))
                        row += row_start + col_start + "Physician ID: " + col_end + col_start + physicianId + col_end + row_end;
                    if (experiences != null && experiences.Count > 0)
                    {
                        row += "<tr><td><hr /></td><td><hr /></td></tr>";
                        row += row_start + col_start + "Physician Previous Experience: " + col_end + row_end;
                        for (int i = 0; i < experiences.Count; i++)
                        {
                            row += row_start + col_start + "Hospital Name: " + col_end + col_start + Encryption.decrypt(experiences[i][0], decryptionKey) + col_end + row_end;
                            row += row_start + col_start + "Hospital Address: " + col_end + col_start + Encryption.decrypt(experiences[i][1], decryptionKey) + col_end + row_end;
                            row += row_start + col_start + "Years of Experience: " + col_end;
                            row += col_start + "From : (" + Encryption.decrypt(experiences[i][2], decryptionKey)+") ";
                            row += " To: (" +  Encryption.decrypt(experiences[i][3], decryptionKey)+") " + col_end + row_end;
                        }
                    }
                    lblRow.Text += row;
                    txtPassword.Text = "";
                    requestedToViewProfileInformation = false;
                }
            }
            else
            {
                string row = "";
                row += row_start + col_start + "Physician Complete Profile Information: " + col_end + row_end;
                row += row_start + col_start + "Account is: " + col_end + col_start + str_isPrivate + col_end + row_end;
                if (!string.IsNullOrWhiteSpace(dialysis))
                    row += row_start + col_start + "Dialysis: " + col_end + col_start + dialysis + col_end + row_end;
                if (!string.IsNullOrWhiteSpace(homeDialysis))
                    row += row_start + col_start + "Hemodialysis: " + col_end + col_start + homeDialysis + col_end + row_end;
                if (!string.IsNullOrWhiteSpace(transplantation))
                    row += row_start + col_start + "Transplantation: " + col_end + col_start + transplantation + col_end + row_end;
                if (!string.IsNullOrWhiteSpace(hypertension))
                    row += row_start + col_start + "Hypertension: " + col_end + col_start + hypertension + col_end + row_end;
                if (!string.IsNullOrWhiteSpace(gN))
                    row += row_start + col_start + "GN: " + col_end + col_start + gN + col_end + row_end;
                if (!string.IsNullOrWhiteSpace(physicianId))
                    row += row_start + col_start + "Physician ID: " + col_end + col_start + physicianId + col_end + row_end;
                if (experiences != null && experiences.Count > 0)
                {
                    row += "<tr><td><hr /></td><td><hr /></td></tr>";
                    row += row_start + col_start + "Physician Previous Experience: " + col_end + row_end;
                    for (int i = 0; i < experiences.Count; i++)
                    {
                        row += row_start + col_start + "Hospital Name: " + col_end + col_start + experiences[i][0] + col_end + row_end;
                        row += row_start + col_start + "Hospital Address: " + col_end + col_start + experiences[i][1] + col_end + row_end;
                        row += row_start + col_start + "Years of Experience: " + col_end;
                        row += col_start + "From : (" + experiences[i][2] + ") ";
                        row += " To: (" + experiences[i][3] + ") " + col_end + row_end;
                    }
                }
                lblRow.Text += row;
                txtPassword.Text = "";
                requestedToViewProfileInformation = false;
            }
        }
    }
}