﻿using System;
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
            //Get from and to pages:
            string current_page = "", previous_page = "";
            if (HttpContext.Current.Request.Url.AbsoluteUri != null) current_page = HttpContext.Current.Request.Url.AbsoluteUri;
            if (Request.UrlReferrer != null) previous_page = Request.UrlReferrer.ToString();
            //Get current time:
            DateTime currentTime = DateTime.Now;
            //Get user's IP:
            string userIP = GetIPAddress();
            CheckPatientSession session = new CheckPatientSession();
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
            getCompleteProfileInformation();
            getEditCompleteProfileInformation();
            viewProfiles();
        }
        //public override void VerifyRenderingInServerForm(Control control) { }
        //Methods to show and hide controls
        protected void viewProfiles()
        {
            View.Visible = true;
            EditCompleteProfile.Visible = false;
            lblSaveCompleteProfileMessage.Visible = false;
            UserAgreement.Visible = false;
        }
        protected void showEditCompleteProfile()
        {
            View.Visible = false;
            EditCompleteProfile.Visible = true;
            UserAgreement.Visible = false;
        }
        protected void showUserAgreement()
        {
            View.Visible = false;
            EditCompleteProfile.Visible = false;
            lblSaveCompleteProfileMessage.Visible = false;
            UserAgreement.Visible = true;
            //Lode the user agreement:
            string userAgreementText =
                "This website follows HIPAA" +
                "- Your information will not be shared with a 3rd part.\n" +
                "- If you set your profile to private, no one will be able to view your information except you.\n" +
                "- If you do not set your profile to private, system admins will be able to view your complete profile.\n" +
                "  This helps them to give recommendations based on your profile information.\n" +
                "- For more information, please contact the support team by email NephroNet2018@gmail.com\n\n"+
                "-------------------------------------------------------------------------------------------------"+
                "----------------------------The below was copied from: https://www.hhs.gov ----------------------";
            txtUserAgreement.Text = userAgreementText;
            //The below method will more text to the user agreement:
            setAgreementText();
            lblAgree.Text = "I agree to the terms and conditions";
            chkAgree.Visible = true;
            chkAgree.Checked = false;
        }
        protected void btnCompleteProfile_Click(object sender, EventArgs e)
        {
            showEditCompleteProfile();
        }
       
        protected void getEditCompleteProfileInformation()
        {
            lblPrivateMessage.Visible = true;
            connect.Open();
            SqlCommand cmd = connect.CreateCommand();
            cmd.CommandText = "select userId from Users where loginId = '" + loginId + "' ";
            string userId = cmd.ExecuteScalar().ToString();
            connect.Close();
            PatientCompleteProfile completeProfile = new PatientCompleteProfile(userId, userId);
            int isPrivate = completeProfile.Private;
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
            if (isPrivate == 1)
                chkIsPrivate.Checked = true;
            else
                chkIsPrivate.Checked = false;
            if (chkIsPrivate.Checked)
                lblPrivateMessage.Text = "Your account will become private";
            else
                lblPrivateMessage.Text = "Only Admins can view your account";
            if (!string.IsNullOrWhiteSpace(highBloodPressure))
                txtHighBloodPressure.Text = highBloodPressure;
            if (!string.IsNullOrWhiteSpace(diabetes))
                txtDiabetes.Text = diabetes;
            if (!string.IsNullOrWhiteSpace(kidneyTransplant))
                txtKidneyTransplant.Text = kidneyTransplant;
            if (!string.IsNullOrWhiteSpace(dialysis))
                txtDialysis.Text = dialysis;
            if (!string.IsNullOrWhiteSpace(kidneyStone))
                txtKidneyStone.Text = kidneyStone;
            if (!string.IsNullOrWhiteSpace(kidneyInfection))
                txtKidneyInfection.Text = kidneyInfection;
            if (!string.IsNullOrWhiteSpace(heartFailure))
                txtHeartFailure.Text = heartFailure;
            if (!string.IsNullOrWhiteSpace(cancer))
                txtCancer.Text = cancer;
            if (!string.IsNullOrWhiteSpace(comments))
                txtComments.Text = comments;
            if (!string.IsNullOrWhiteSpace(patientId))
                txtPatientId.Text = patientId;   
        }
        protected bool checkEditCompleteProfileInformationInput()
        {
            bool correct = true;
            //Hide everything first:
            lblHighBloodPressureError.Visible = false;
            lblDiabetesError.Visible = false;
            lblKidneyTransplantError.Visible = false;
            lblDialysisError.Visible = false;
            lblKidneyStoneError.Visible = false;
            lblKidneyInfectionError.Visible = false;
            lblHeartFailureError.Visible = false;
            lblCancerError.Visible = false;
            lblCommentsError.Visible = false;
            lblPatientIdError.Visible = false;
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
                    PatientCompleteProfile completeProfile = new PatientCompleteProfile(userId, userId);
                    string completeProfileId = completeProfile.ID;
                    //replace the single quote with double quotes in all inputs.
                    string highBloodPressure = txtHighBloodPressure.Text.Replace("'", "''");
                    string diabetes = txtDiabetes.Text.Replace("'", "''");
                    string kidneyTransplant = txtKidneyTransplant.Text.Replace("'", "''");
                    string dialysis = txtDialysis.Text.Replace("'", "''");
                    string kidneyStone = txtKidneyStone.Text.Replace("'", "''");
                    string kidneyInfection = txtKidneyInfection.Text.Replace("'", "''");
                    string heartFailure = txtHeartFailure.Text.Replace("'", "''");
                    string cancer = txtCancer.Text.Replace("'", "''");
                    string comments = txtComments.Text.Replace("'", "''");
                    string patientId = txtPatientId.Text.Replace("'", "''");
                    int isPrivate = 0;
                    if (chkIsPrivate.Checked)
                        isPrivate = 1;
                    else
                        isPrivate = 0;
                    //update the record in the database.
                    cmd.CommandText = "update PatientCompleteProfiles set patientCompleteProfile_highBloodPressure = '" + highBloodPressure + "', patientCompleteProfile_Diabetes = '" + diabetes + "', " +
                        "patientCompleteProfile_kidneyTransplant = '" + kidneyTransplant + "', patientCompleteProfile_Dialysis = '" + dialysis + "', patientCompleteProfile_kidneyStone = '" + kidneyStone + "', " +
                        "patientCompleteProfile_kidneyInfection = '" + kidneyInfection + "', patientCompleteProfile_heartFailure = '" + heartFailure + "', patientCompleteProfile_cancer = '" + cancer + "',  " +
                        "patientCompleteProfile_comments = '" + comments + "', patientCompleteProfile_patientId = '" + patientId + "', patientCompleteProfile_isPrivate = '" + isPrivate + "'" +
                        " where patientCompleteProfileId = '" + completeProfileId + "' ";
                    cmd.ExecuteScalar();
                    //if there is a related record in another table, update it.

                    connect.Close();
                    lblSaveCompleteProfileMessage.Visible = true;
                }
                getCompleteProfileInformation();
                //Hide the user agreement:
                showEditCompleteProfile();
            }
        }

        protected void btnDisagree_Click(object sender, EventArgs e)
        {
            //Hide the user agreement and do nothing:
            showEditCompleteProfile();
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
        protected void btnSaveEditCompleteProfile_Click(object sender, EventArgs e)
        {
            showUserAgreement();
            
        }
        protected void btnCancelEditCompleteProfile_Click(object sender, EventArgs e)
        {
            getCompleteProfileInformation();
            viewProfiles();
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
            PatientCompleteProfile completeProfile = new PatientCompleteProfile(userId, userId);
            string completeProfileId = completeProfile.ID;
            int isPrivate = completeProfile.Private;
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
            string row = "";
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
}