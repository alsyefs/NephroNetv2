using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NephroNet.Accounts.Admin
{
    public partial class ReviewUser : System.Web.UI.Page
    {
        static string conn = "";
        SqlConnection connect = new SqlConnection(conn);
        string username, roleId, loginId, token;
        string registerId = "";
        //Globals for "Users" table:
        string g_firstName, g_lastName, g_email, g_city, g_state, g_zip, g_address, g_phone, g_patientOrPhysicianId, g_country;
        //Globals for "Logins" table:
        int g_roleId;
        static string previousPage = "";
        static string currentPage = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (HttpContext.Current.Request.Url.AbsoluteUri != null) currentPage = HttpContext.Current.Request.Url.AbsoluteUri;
                else currentPage = "Home.aspx";
                if (Request.UrlReferrer != null) previousPage = Request.UrlReferrer.ToString();
                else previousPage = "Home.aspx";
                if (currentPage.Equals(previousPage))
                    previousPage = "Home.aspx";
            }
            initialPageAccess();
            registerId = Request.QueryString["id"];
            showUserInformation();
        }
        protected void btnCancel_Click(object sender, EventArgs e)
        {
            goBack();
        }
        protected void goBack()
        {
            addSession();
            if (!string.IsNullOrWhiteSpace(previousPage)) Response.Redirect(previousPage);
            else Response.Redirect("Home.aspx");
        }
        protected void showUserInformation()
        {
            connect.Open();
            SqlCommand cmd = connect.CreateCommand();
            //Check if the ID exists in the database:
            cmd.CommandText = "select count(*) from registrations where registerId = '"+registerId.Replace("'", "''") + "' ";
            int countUser = Convert.ToInt32(cmd.ExecuteScalar());
            if(countUser > 0)//if ID exists, countUser = 1
            {
                //Get first name:
                cmd.CommandText = "select register_firstname from [Registrations] where [registerId] = '"+registerId+"' ";
                string firstName = cmd.ExecuteScalar().ToString();
                //Get last name and add it to the first name:
                cmd.CommandText = "select register_lastname from [Registrations] where [registerId] = '" + registerId + "' ";
                string lastName = cmd.ExecuteScalar().ToString();
                //Get email:
                cmd.CommandText = "select register_email from [Registrations] where [registerId] = '" + registerId + "' ";
                string email = cmd.ExecuteScalar().ToString();
                //Get city:
                cmd.CommandText = "select register_city from [Registrations] where [registerId] = '" + registerId + "' ";
                string city = cmd.ExecuteScalar().ToString();
                //Get state:
                cmd.CommandText = "select register_state from [Registrations] where [registerId] = '" + registerId + "' ";
                string state = cmd.ExecuteScalar().ToString();
                //Get zip code:
                cmd.CommandText = "select register_zip from [Registrations] where [registerId] = '" + registerId + "' ";
                string zip = cmd.ExecuteScalar().ToString();
                //Get address:
                cmd.CommandText = "select register_address from [Registrations] where [registerId] = '" + registerId + "' ";
                string address = cmd.ExecuteScalar().ToString();
                //Get role ID as int:
                cmd.CommandText = "select register_roleId from [Registrations] where [registerId] = '" + registerId + "' ";
                int int_roleId = Convert.ToInt32(cmd.ExecuteScalar());
                string patientOrPhysicianId = "";
                //Convert role ID to string:
                string role = "";
                if (int_roleId == 1)
                    role = "Admin";
                else if (int_roleId == 2)
                {
                    role = "Physician";
                    //Get Physician ID:
                    cmd.CommandText = "select register_physicianId from [Registrations] where [registerId] = '" + registerId + "' ";
                    patientOrPhysicianId = cmd.ExecuteScalar().ToString();
                }
                else if (int_roleId == 3)
                {
                    role = "Patient";
                    //Get patient ID:
                    cmd.CommandText = "select register_patientId from [Registrations] where [registerId] = '" + registerId + "' ";
                    patientOrPhysicianId = cmd.ExecuteScalar().ToString();
                }
                //Get phone:
                cmd.CommandText = "select register_phone from [Registrations] where [registerId] = '" + registerId + "' ";
                string phone = cmd.ExecuteScalar().ToString();
                //Get Country:
                cmd.CommandText = "select register_country from [Registrations] where [registerId] = '" + registerId + "' ";
                string country = cmd.ExecuteScalar().ToString();

                
                string phoneFormat = "";
                if (country.Equals("United States"))
                    phoneFormat = Layouts.phoneFormat(phone);
                else
                    phoneFormat = phone;
                //Create an informative message containing all information for the selected user:
                lblUserInformation.Text =
                    "<table>" +
                    "<tr><td>Name: </td><td>" + firstName + " " + lastName + "</td></tr>" +
                    "<tr><td>Email: </td><td>" + email + "</td></tr>" +
                    "<tr><td>Address: </td><td>" + address + "</td></tr>" +
                    "<tr><td>City: </td><td>" + city + "</td></tr>" +
                    "<tr><td>State: </td><td>" + state + "</td></tr>" +
                    "<tr><td>Zip code: </td><td>" + zip + "</td></tr>" +
                    "<tr><td>Phone#: </td><td>" + phoneFormat + "</td></tr>" +
                    "<tr><td>Role: </td><td>" + role + "</td></tr>";
                if (!string.IsNullOrWhiteSpace(patientOrPhysicianId))
                {
                    if (int_roleId == 3)
                        lblUserInformation.Text += "<tr><td>Patient ID: </td><td>" + patientOrPhysicianId + "</td></tr>";
                    else if (int_roleId == 2)
                        lblUserInformation.Text += "<tr><td>Physician ID: </td><td>" + patientOrPhysicianId + "</td></tr>";
                }
                lblUserInformation.Text += "</table>";
                lblUserInformation.Visible = true;
                //Copy values to globals:
                g_firstName = firstName; g_lastName = lastName; g_email = email; g_city = city; g_state = state;
                g_zip = zip; g_address = address; g_phone=phone;g_roleId = int_roleId; g_patientOrPhysicianId = patientOrPhysicianId; g_country = country;
            }
            else
            {
                goBack();
            }
            connect.Close();
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
        protected string createUsername()
        {
            string generatedUsername = "";
            //generatedUsername = g_firstName + g_lastName + g_roleId + registerId;
            generatedUsername = g_firstName + "." + g_lastName;// + g_roleId + registerId;
            generatedUsername = generatedUsername.Replace(" ", "");
            generatedUsername = generatedUsername.Replace("'", "");
            //Make sure the new username doesn't match another username in the system:
            connect.Open();
            SqlCommand cmd = connect.CreateCommand();
            cmd.CommandText = "select count(*) from Logins where login_username like '"+generatedUsername+"' ";
            int countDuplicateUsernames = Convert.ToInt32(cmd.ExecuteScalar());
            if(countDuplicateUsernames > 0)
            {
                //If the username exists, add the role ID at the end of it:
                generatedUsername = generatedUsername + g_roleId;
                cmd.CommandText = "select count(*) from Logins where login_username like '" + generatedUsername + "' ";
                int countDuplicateUsernames_2 = Convert.ToInt32(cmd.ExecuteScalar());
                if (countDuplicateUsernames_2 > 0)
                {
                    //If the username exists, add the register ID at the end of it:
                    generatedUsername = generatedUsername + registerId;
                    cmd.CommandText = "select count(*) from Logins where login_username like '" + generatedUsername + "' ";
                    int countDuplicateUsernames_3 = Convert.ToInt32(cmd.ExecuteScalar());
                    if (countDuplicateUsernames_3 > 0)
                    {
                        Random rnd = new Random();
                        int addUniqueness = rnd.Next(1, 999);
                        //If the username exists, add a random integer at the end of it:
                        generatedUsername = generatedUsername + addUniqueness;
                        cmd.CommandText = "select count(*) from Logins where login_username like '" + generatedUsername + "' ";
                        int countDuplicateUsernames_4 = Convert.ToInt32(cmd.ExecuteScalar());
                        if(countDuplicateUsernames_4 > 0)
                        {
                            //In an extreme case, if that generated username duplicates with another one, add the login ID + 1 from the last login ID:
                            cmd.CommandText = "select top 1 loginId from logins order by loginId desc";
                            int lastLoginId = Convert.ToInt32(cmd.ExecuteScalar());
                            lastLoginId++;
                            generatedUsername = generatedUsername + lastLoginId;
                        }
                    }
                }
            }
            connect.Close();
            return generatedUsername;
        }
        protected string createPassword()
        {
            //The below will generate a password of 8 characters having at least 4 non-alphanumeric characters:
            string generatedPassword = Membership.GeneratePassword(8, 4);
            return generatedPassword;
        }
        protected bool checkIfThereIsRecordInHospitalDB()
        {
            bool thereIs = true;
            SqlCommand cmd = connect.CreateCommand();
            connect.Open();
            int count = 0;
            int int_roleId = Convert.ToInt32(g_roleId);
            if (int_roleId == 2)//2: Physician
            {
                cmd.CommandText = "select count(*) from DB2_PhysicianShortProfiles where db2_physicianShortProfile_physicianId = '" + g_patientOrPhysicianId + "' ";
                count = Convert.ToInt32(cmd.ExecuteScalar());
            }
            else if (int_roleId == 3)//3: Patient
            {
                cmd.CommandText = "select count(*) from DB2_PatientShortProfiles where DB2_PatientShortProfile_patientId = '" + g_patientOrPhysicianId + "' ";
                count = Convert.ToInt32(cmd.ExecuteScalar());
            }
            if (count == 0)
                thereIs = false;
            connect.Close();
            return thereIs;
        }
        protected bool checkIfIdUsed()
        {
            bool used = false;
            SqlCommand cmd = connect.CreateCommand();
            connect.Open();
            int int_roleId = Convert.ToInt32(g_roleId);
            if (int_roleId == 2)//2: Physician
            {
                //Check if there is already a physician who used the same Physician ID:
                cmd.CommandText = "select count(*) from PhysicianShortProfiles where physicianShortProfile_physicianId like '" + g_patientOrPhysicianId + "' ";
                int physicianIdHasBeenUsed = Convert.ToInt32(cmd.ExecuteScalar());
                if (physicianIdHasBeenUsed > 0)
                {
                    //Get the user ID:
                    cmd.CommandText = "select userId from PhysicianShortProfiles where physicianShortProfile_physicianId like '" + g_patientOrPhysicianId + "'";
                    string physician_userId = cmd.ExecuteScalar().ToString();
                    //Get the login ID:
                    cmd.CommandText = "select loginId from Users where userId = '" + physician_userId + "'";
                    string physician_loginId = cmd.ExecuteScalar().ToString();
                    //Check if the account for that patient ID is active:
                    cmd.CommandText = "select login_isActive from Logins where loginId = '" + physician_loginId + "'";
                    int isActive = Convert.ToInt32(cmd.ExecuteScalar());
                    if (isActive == 1)
                        used = true;
                }
            }
            else if (int_roleId == 3)//3: Patient
            {
                //Check if there is already a patient who used the same Patient ID:
                cmd.CommandText = "select count(*) from PatientShortProfiles where patientShortProfile_patientId like '" + g_patientOrPhysicianId + "' ";
                int patientIdHasBeenUsed = Convert.ToInt32(cmd.ExecuteScalar());
                if (patientIdHasBeenUsed > 0)
                {
                    //Get the user ID:
                    cmd.CommandText = "select userId from PatientShortProfiles where patientShortProfile_patientId like '" + g_patientOrPhysicianId + "'";
                    string patient_userId = cmd.ExecuteScalar().ToString();
                    //Get the login ID:
                    cmd.CommandText = "select loginId from Users where userId = '" + patient_userId + "'";
                    string patient_loginId = cmd.ExecuteScalar().ToString();
                    //Check if the account for that patient ID is active:
                    cmd.CommandText = "select login_isActive from Logins where loginId = '" + patient_loginId + "'";
                    int isActive = Convert.ToInt32(cmd.ExecuteScalar());
                    if (isActive == 1)
                        used = true;
                }
            }
            connect.Close();
            return used;
        }
        protected void btnApprove_Click(object sender, EventArgs e)
        {
            //Hide the success message:
            lblMessage.Visible = false;
            lblMessage.ForeColor = System.Drawing.Color.Green;
            //Store the new patient ID or physician ID:
            int int_roleId = Convert.ToInt32(g_roleId);
            if (int_roleId == 2 || int_roleId == 3)
            {
                if (!checkIfThereIsRecordInHospitalDB())
                {
                    lblMessage.Visible = true;
                    lblMessage.ForeColor = System.Drawing.Color.Red;
                    if (int_roleId == 2)//2: Physician
                        lblMessage.Text = "There is no physician in a hospital with the entered Physician ID";
                    else if (int_roleId == 3)//3: Patient
                        lblMessage.Text = "There is no patient in a hospital with the entered Patient ID";
                }
                else if (checkIfIdUsed())
                {
                    lblMessage.Visible = true;
                    lblMessage.ForeColor = System.Drawing.Color.Red;
                    if (int_roleId == 2)//2: Physician
                        lblMessage.Text = "There is already an active account for a physician in the system with the entered Physician ID";
                    else if (int_roleId == 3)//3: Patient
                        lblMessage.Text = "There is already an active account for a patient in the system with the entered Patient ID";
                }
                else
                {
                    lblMessage.ForeColor = System.Drawing.Color.Green;
                    storeNewUser();
                }
            }
            else
            {
                lblMessage.ForeColor = System.Drawing.Color.Green;
                storeNewUser();
            }
            
        }
        protected void storeNewUser()
        {
            //Hide the success message:
            lblMessage.Visible = false;
            lblMessage.ForeColor = System.Drawing.Color.Green;
            //Store the new patient ID or physician ID:
            int int_roleId = Convert.ToInt32(g_roleId);
            //Create a new unique username:
            string newUsername = createUsername();
            //Create an initial password:
            string newPassword = createPassword();
            //Hash the password:
            string hashedPassword = Encryption.hash(newPassword);
            //Set login_attempts = 0, login_securityQuestionsAttempts = 0, login_initial = 1 and login_isActive = 1: (1 in bit = true)
            //Store the previous information into the table "Logins":
            g_phone = g_phone.Replace(" ", "");
            g_phone = g_phone.Replace("'", "''");
            connect.Open();
            SqlCommand cmd = connect.CreateCommand();
            cmd.CommandText = "insert into Logins (login_username, login_password, roleId, login_attempts, login_securityQuestionsAttempts, login_initial, login_isActive) values " +
                "('" + newUsername + "', '" + hashedPassword + "', '" + g_roleId + "', 0, 0, 1, 1)";
            cmd.ExecuteScalar();
            //Get the loginID of the user just created using the username:
            cmd.CommandText = "select loginId from Logins where login_username like '" + newUsername + "' ";
            string newLoginId = cmd.ExecuteScalar().ToString();
            //Store the user's information into the "Users" table:
            cmd.CommandText = "insert into Users (user_firstname, user_lastname, user_email, user_city, user_state, user_zip, user_address, user_phone, loginId, user_country) values " +
                "('" + g_firstName + "', '" + g_lastName + "', '" + g_email + "', '" + g_city + "', '" + g_state + "', '" + g_zip + "', '" + g_address + "', '" + g_phone + "', '" + newLoginId + "', '" + g_country + "') ";
            cmd.ExecuteScalar();
            //Get the user ID of the user who was just added:
            cmd.CommandText = "select userId from users where loginId = '" + newLoginId + "' ";
            string temp_userId = cmd.ExecuteScalar().ToString();
            if (int_roleId == 2)//2: Physician
            {
                //Insert into the complete profile:
                cmd.CommandText = "insert into PhysicianCompleteProfiles (physicianCompleteProfile_PhysicianId, userId) values " +
                    "('" + g_patientOrPhysicianId + "', '" + temp_userId + "')";
                cmd.ExecuteScalar();
                //Get the short profile information from the hospital's DB:
                cmd.CommandText = "select db2_physicianShortProfile_hospitalName from DB2_PhysicianShortProfiles where db2_physicianShortProfile_physicianId like '"+g_patientOrPhysicianId+"' ";
                string hospitalName = cmd.ExecuteScalar().ToString();
                cmd.CommandText = "select db2_physicianShortProfile_hospitalAddress from DB2_PhysicianShortProfiles where db2_physicianShortProfile_physicianId like '" + g_patientOrPhysicianId + "' ";
                string hospitalAddress = cmd.ExecuteScalar().ToString();
                cmd.CommandText = "select db2_physicianShortProfile_officePhone from DB2_PhysicianShortProfiles where db2_physicianShortProfile_physicianId like '" + g_patientOrPhysicianId + "' ";
                string hospitalPhone = cmd.ExecuteScalar().ToString();
                cmd.CommandText = "select db2_physicianShortProfile_officeEmail from DB2_PhysicianShortProfiles where db2_physicianShortProfile_physicianId like '" + g_patientOrPhysicianId + "' ";
                string hospitalEmail = cmd.ExecuteScalar().ToString();
                cmd.CommandText = "select db2_physicianShortProfile_speciality from DB2_PhysicianShortProfiles where db2_physicianShortProfile_physicianId like '" + g_patientOrPhysicianId + "' ";
                string hospitalSpeciality = cmd.ExecuteScalar().ToString();
                hospitalName = hospitalName.Replace("'", "''");
                hospitalAddress = hospitalAddress.Replace("'", "''");
                hospitalPhone = hospitalPhone.Replace("'", "''");
                hospitalEmail = hospitalEmail.Replace("'", "''");
                hospitalSpeciality = hospitalSpeciality.Replace("'", "''");
                //Insert into the short profile:
                cmd.CommandText = "insert into PhysicianShortProfiles (physicianShortProfile_hospitalName, physicianShortProfile_hospitalAddress, physicianShortProfile_officePhone," +
                    "physicianShortProfile_officeEmail, physicianShortProfile_speciality, userId, physicianShortProfile_physicianId) values" +
                    "('"+hospitalName+"', '"+hospitalAddress+"', '"+hospitalPhone+"', '"+hospitalEmail+"', '"+hospitalSpeciality+"', '"+temp_userId+"', '"+g_patientOrPhysicianId+"') ";
                cmd.ExecuteScalar();
            }
            else if (int_roleId == 3)//3: Patient
            {
                //Insert into the complete profile:
                cmd.CommandText = "insert into PatientCompleteProfiles (PatientCompleteProfile_PatientId, userId) values " +
                    "('" + g_patientOrPhysicianId + "', '" + temp_userId + "')";
                cmd.ExecuteScalar();
                //Get the short profile information from the hospital's DB:
                cmd.CommandText = "select db2_patientShortProfile_email from DB2_PatientShortProfiles where db2_patientShortProfile_patientId like '" + g_patientOrPhysicianId + "' ";
                string patientEmail = cmd.ExecuteScalar().ToString();
                cmd.CommandText = "select db2_patientShortProfile_phone from DB2_PatientShortProfiles where db2_patientShortProfile_patientId like '" + g_patientOrPhysicianId + "' ";
                string patientPhone = cmd.ExecuteScalar().ToString();
                cmd.CommandText = "select db2_patientShortProfile_gender from DB2_PatientShortProfiles where db2_patientShortProfile_patientId like '" + g_patientOrPhysicianId + "' ";
                string patientGender = cmd.ExecuteScalar().ToString();
                cmd.CommandText = "select db2_patientShortProfile_dateOfBirth from DB2_PatientShortProfiles where db2_patientShortProfile_patientId like '" + g_patientOrPhysicianId + "' ";
                string dob = cmd.ExecuteScalar().ToString();
                patientEmail = patientEmail.Replace("'", "''");
                patientPhone = patientPhone.Replace("'", "''");
                patientGender = patientGender.Replace("'", "''");
                dob = dob.Replace("'", "''");
                //Insert into the short profile:
                cmd.CommandText = "insert into PatientShortProfiles (patientShortProfile_email, patientShortProfile_phone, userId," +
                    "patientShortProfile_patientId, patientShortProfile_gender, patientShortProfile_dateOfBirth) values" +
                    "('" + patientEmail + "', '" + patientPhone + "', '" + temp_userId + "', '" + g_patientOrPhysicianId + "', " +
                    " '"+patientGender+"', '"+dob+"') ";
                cmd.ExecuteScalar();
            }
            connect.Close();
            //Create an email message to be sent:
            string emailMessage = "Hello " + g_firstName + " " + g_lastName + ",\n\n" +
                "This email is to inform you that your account has been approved for NephroNet. To access the site, you need the following information:\n" +
                "username: " + newUsername + "\n" +
                "password: " + newPassword + "\n" +
                "Remeber, your provided is a temporary password and you must change it once you login to the site.\n\n" +
                "Best regards,\nNephroNet Support\nNephroNet2018@gmail.com";
            //Send an email notification the user using the entered email:
            Email emailClass = new Email();
            emailClass.sendEmail(g_email, emailMessage);
            //Display a success message:
            lblMessage.Visible = true;
            lblMessage.Text = "The selected user has been successfully approved and the new information has been emailed to the user!";
            //Hide "Approve" and "Deny" buttons:
            hideApproveDeny();
            //Delete user information from "Registrations" table:
            connect.Open();
            cmd.CommandText = "delete from [Registrations] where registerId = '" + registerId + "' ";
            cmd.ExecuteScalar();
            connect.Close();
        }
        protected void hideApproveDeny()
        {
            btnApprove.Visible = false;
            btnDeny.Visible = false;
        }
        protected void btnDeny_Click(object sender, EventArgs e)
        {
            //Delete user information from "Registrations" table:
            connect.Open();
            SqlCommand cmd = connect.CreateCommand();
            cmd.CommandText = "delete from [Registrations] where registerId = '"+registerId+"' ";
            cmd.ExecuteScalar();
            connect.Close();
            //Create an email message to be sent:
            string emailMessage = "Hello " + g_firstName + " " + g_lastName + ",\n\n" +
                "This email is to inform you that your account has been denied for NephroNet. For more information, please contact the support.\n\n" +
                "Best regards,\nNephroNet Support\nNephroNet2018@gmail.com";
            //Send an email notification the user using the entered email:
            Email emailClass = new Email();
            emailClass.sendEmail(g_email, emailMessage);
            //Show in a message that the user was denied:
            lblMessage.Visible = true;
            lblMessage.Text = "The selected user has been successfully denied, emailed and removed from the list of applied users!";
            //Hide "Approve" and "Deny" buttons:
            hideApproveDeny();
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