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

namespace NephroNet.Accounts.Physician
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
            getCompleteProfileInformation();
            viewProfiles();
        }
        
        //public override void VerifyRenderingInServerForm(Control control) { }
        protected void viewProfiles()
        {
            View.Visible = true;
            EditCompleteProfile.Visible = false;
        }
        
        protected void showEditCompleteProfile()
        {
            View.Visible = false;
            EditCompleteProfile.Visible = true;
        }
        protected void showEditBlockedUsers()
        {
            View.Visible = false;
            EditCompleteProfile.Visible = false;
        }
        protected void showEditCurrentHealthConditions()
        {
            View.Visible = false;
            EditCompleteProfile.Visible = false;
        }
        protected void showEditCurrentTreatments()
        {
            View.Visible = false;
            EditCompleteProfile.Visible = false;
        }
        protected void btnCompleteProfile_Click(object sender, EventArgs e)
        {

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
            string address = completeProfile.Address + newLine + "  " + completeProfile.City + ", " + completeProfile.State + " " + completeProfile.Zip+
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
    }
}