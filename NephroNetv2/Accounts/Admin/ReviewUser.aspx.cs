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
        string g_firstName, g_lastName, g_email, g_city, g_state, g_zip, g_address, g_phone, g_patientId, g_country;
        //Globals for "Logins" table:
        int g_roleId;
        protected void Page_Load(object sender, EventArgs e)
        {
            initialPageAccess();
            registerId = Request.QueryString["id"];
            showUserInformation();
        }
        protected void btnCancel_Click(object sender, EventArgs e)
        {
            addSession();
            Response.Redirect("ApproveUsers");
        }
        protected void showUserInformation()
        {
            connect.Open();
            SqlCommand cmd = connect.CreateCommand();
            //Check if the ID exists in the database:
            cmd.CommandText = "select count(*) from registrations where registerId = '"+registerId+"' ";
            int countUser = Convert.ToInt32(cmd.ExecuteScalar());
            if(countUser > 0)//if ID exists, countUser = 1
            {
                //Get first name:
                cmd.CommandText = "select register_firstname from [Registrations] where [registerId] = '"+registerId+"' ";
                string firstName = cmd.ExecuteScalar().ToString();
                //Get last name and add it to the first name:
                cmd.CommandText = "select register_lastname from [Registrations] where [registerId] = '" + registerId + "' ";
                string lastName = " " + cmd.ExecuteScalar().ToString();
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
                //Convert role ID to string:
                string role = "";
                if (int_roleId == 1)
                    role = "Admin";
                else if (int_roleId == 2)
                    role = "Physician";
                else
                    role = "Patient";
                //Get phone:
                cmd.CommandText = "select register_phone from [Registrations] where [registerId] = '" + registerId + "' ";
                string phone = cmd.ExecuteScalar().ToString();
                //Get Country:
                cmd.CommandText = "select register_country from [Registrations] where [registerId] = '" + registerId + "' ";
                string country = cmd.ExecuteScalar().ToString();
                //Get patient ID:
                cmd.CommandText = "select register_patientId from [Registrations] where [registerId] = '" + registerId + "' ";
                string patientId = cmd.ExecuteScalar().ToString();
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
                if (!string.IsNullOrWhiteSpace(patientId))
                {
                    lblUserInformation.Text += "<tr><td>Patient ID: </td><td>" + patientId + "</td></tr>";
                }
                lblUserInformation.Text += "</table>";
                lblUserInformation.Visible = true;
                //Copy values to globals:
                g_firstName = firstName; g_lastName = lastName; g_email = email; g_city = city; g_state = state;
                g_zip = zip; g_address = address; g_phone=phone;g_roleId = int_roleId; g_patientId = patientId; g_country = country;
            }
            else
            {
                addSession();
                Response.Redirect("ApproveUsers");
            }
            connect.Close();
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
        protected void btnApprove_Click(object sender, EventArgs e)
        {
            //Hide the success message:
            lblMessage.Visible = false;
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
                "('"+newUsername+"', '"+ hashedPassword + "', '"+g_roleId+"', 0, 0, 1, 1)";
            cmd.ExecuteScalar();
            //Get the loginID of the user just created using the username:
            cmd.CommandText = "select loginId from Logins where login_username like '"+newUsername+"' ";
            string newLoginId = cmd.ExecuteScalar().ToString();
            //Store the user's information into the "Users" table:
            cmd.CommandText = "insert into Users (user_firstname, user_lastname, user_email, user_city, user_state, user_zip, user_address, user_phone, loginId, user_patientId, user_country) values " +
                "('"+g_firstName+"', '"+g_lastName+"', '"+g_email+"', '"+g_city+"', '"+g_state+"', '"+g_zip+"', '"+g_address+"', '"+g_phone+"', '"+newLoginId+"', '"+g_patientId+"', '"+g_country+"') ";
            cmd.ExecuteScalar();
            //Get the user ID of the user who was just added:
            cmd.CommandText = "select userId from users where loginId = '"+newLoginId+"' ";
            string temp_userId = cmd.ExecuteScalar().ToString();
            //Store the user's information into the "ShortProfiles" table:
            cmd.CommandText = "insert into ShortProfiles (userId,shortProfile_firstname, shortProfile_lastname, shortProfile_roleId) values " +
                "('" + temp_userId + "', '" + g_firstName + "', '" + g_lastName + "', '" + g_roleId + "') ";
            cmd.ExecuteScalar();
            //Store the user's ID into the "CompleteProfiles" table:
            cmd.CommandText = "insert into CompleteProfiles (userId, completeProfile_city, completeProfile_state, completeProfile_zip, completeProfile_address, completeProfile_country) values " +
                "('" + temp_userId + "', '" + g_city + "', '" + g_state + "', '" + g_zip + "', '"+ g_address + "', '"+g_country+"') ";
            cmd.ExecuteScalar();
            //Get the complete profile ID:
            cmd.CommandText = "select completeProfileId from CompleteProfiles where userId = '"+temp_userId+"' ";
            string completeProfileId = cmd.ExecuteScalar().ToString();
            //Store the email in Emails table:
            cmd.CommandText = "insert into Emails (completeProfileId, email_emailAddress, email_isDefault) values ('" + completeProfileId+"', '"+g_email+"', 1) ";
            cmd.ExecuteScalar();
            //Store the phone number in PhoneNumbers table:
            cmd.CommandText = "insert into PhoneNumbers (completeProfileId, phonenumber_phone, phonenumber_isDefault) values ('" + completeProfileId + "', '" + g_phone + "', 1) ";
            cmd.ExecuteScalar();
            connect.Close();
            //Create an email message to be sent:
            string emailMessage = "Hello "+g_firstName + " " + g_lastName + ",\n\n"+
                "This email is to inform you that your account has been approved for NephroNet. To access the site, you need the following information:\n"+
                "username: " + newUsername + "\n"+
                "password: " + newPassword + "\n"+
                "Remeber, your provided is a temporary password and you must change it once you login to the site.\n\n"+
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