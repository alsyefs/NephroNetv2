using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NephroNet
{
    public partial class Register : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {            
            //check input
            //if input = true, send it to database code to store it
            if(!IsPostBack)
            {
                drpCountries.DataSource = getCountries();
                drpCountries.DataBind();
                string headline = "Select your country";
                drpCountries.Items.Insert(0, new ListItem(headline, headline));
            }

        }
        public static List<string> getCountries()
        {
            List<string> countriesList = new List<string>();
            CultureInfo[] cultureInfo = CultureInfo.GetCultures(CultureTypes.SpecificCultures);
            foreach (CultureInfo culture in cultureInfo)
            {
                RegionInfo regionInfo = new RegionInfo(culture.LCID);
                if (!(countriesList.Contains(regionInfo.EnglishName)))
                {
                    countriesList.Add(regionInfo.EnglishName);
                }
            }
            countriesList.Sort();
            return countriesList;
        }
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            bool correctInput = checkInput();
            if (correctInput)
            {
                //send it all to database code:
                storeInput();

                //prompt a message that everything was successful:
                lblResult.ForeColor = System.Drawing.Color.Green;
                lblResult.Visible = true;
                lblResult.Text = "Your application has been successfully submitted!";
                clearInputs(Page);
            }
            else
                lblResult.Visible = false;
        }
        protected void clearInputs(Control p1)
        {
            foreach (Control ctrl in p1.Controls)
            {
                if (ctrl is TextBox)
                {
                    TextBox t = ctrl as TextBox;
                    if (t != null)
                        t.Text = String.Empty;
                }
                else
                {
                    if (ctrl.Controls.Count > 0)
                        clearInputs(ctrl);
                    else
                    {
                        drpStates.ClearSelection();
                        drpRole.ClearSelection();
                    }
                }
            }
        }
        
        protected void btnCancel_Click(object sender, EventArgs e)
        {
            //Go home:
            Response.Redirect("~/");
        }
        protected void hideAllErrorMessages()
        {
            lblFirstnameError.Visible = false;
            lblLastnameError.Visible = false;
            lblEmailError.Visible = false;
            lblCityError.Visible = false;
            lblStateError.Visible = false;
            lblZipError.Visible = false;
            lblAddressError.Visible = false;
            lblPhoneError.Visible = false;
            lblRoleError.Visible = false;
            lblCountryError.Visible = false;
        }
        protected bool checkInput()
        {
            bool correct = true;
            hideAllErrorMessages();
            //First, check if there is any special character within any field:
            CheckErrors error = new CheckErrors();
            //Check first name:
            string firstnameError = "";
            if (
                //error.ContainsSpecialChars(txtFirstname.Text, out firstnameError) || 
                !error.validFirstName(txtFirstname.Text, out firstnameError))
            {
                correct = false;
                lblFirstnameError.Visible = true;
                lblFirstnameError.Text = firstnameError;
                txtFirstname.Focus();
            }
            //Check last name:
            string lastnameError = "";
            if (
                //error.ContainsSpecialChars(txtLastname.Text, out lastnameError) || 
                !error.validLastName(txtLastname.Text, out lastnameError))
            {
                correct = false;
                lblLastnameError.Visible = true;
                lblLastnameError.Text = lastnameError;
                txtLastname.Focus();
            }            
            //Check email:
            string emailError = "";
            if (!error.validEmail(txtEmail.Text, out emailError))
            {
                correct = false;
                lblEmailError.Visible = true;
                lblEmailError.Text = emailError;
                txtEmail.Focus();
            }           
            //Check city:
            string cityError = "";
            if (
                //error.ContainsSpecialChars(txtCity.Text, out cityError) || 
                !error.validCity(txtCity.Text, out cityError))
            {
                correct = false;
                lblCityError.Visible = true;
                lblCityError.Text = cityError;
                txtCity.Focus();
            }
            //Check state:
            string stateError = "";
            if (drpCountries.SelectedValue.Equals("United States"))
            {
                if (!error.validState(drpStates.SelectedIndex, out stateError))
                {
                    correct = false;
                    lblStateError.Visible = true;
                    lblStateError.Text = stateError;
                    drpStates.Focus();
                }
            }
            else if (!drpCountries.SelectedValue.Equals("United States"))
            {
                if (string.IsNullOrWhiteSpace(txtState.Text))
                {
                    correct = false;
                    lblStateError.Visible = true;
                    lblStateError.Text = "Invalid input: Please type your state or region.";
                }
            }
            //Check zip:
            string zipError = "";
            if (drpCountries.SelectedValue.Equals("United States"))
            {
                if (error.ContainsSpecialChars(txtZip.Text, out zipError) || !error.validZip(txtZip.Text, out zipError))
                {
                    correct = false;
                    lblZipError.Visible = true;
                    lblZipError.Text = zipError;
                    txtZip.Focus();
                }
            }
            else if (!drpCountries.SelectedValue.Equals("United States"))
            {
                if (string.IsNullOrWhiteSpace(txtZip.Text))
                {
                    correct = false;
                    lblZipError.Visible = true;
                    lblZipError.Text = "Invalid input: Please type your zip code.";
                }
            }
            //Check address:
            string addressError = "";
            if (
                //error.ContainsSpecialChars(txtAddress.Text, out addressError) || 
                !error.validAddress(txtAddress.Text, out addressError))
            {
                correct = false;
                lblAddressError.Visible = true;
                lblAddressError.Text = addressError;
                txtAddress.Focus();
            }
            //Check phone:
            string phoneError = "";
            if (drpCountries.SelectedValue.Equals("United States"))
            {
                if (
                //error.ContainsSpecialChars(txtPhone.Text, out phoneError) || 
                !error.validPhone(txtPhone.Text, out phoneError))
                {
                    correct = false;
                    lblPhoneError.Visible = true;
                    lblPhoneError.Text = phoneError;
                    txtPhone.Focus();
                }
            }
            else if (drpCountries.SelectedValue.Equals("United States"))
            {
                if(!error.validInternationalPhone(txtPhone.Text, out phoneError))
                {
                    correct = false;
                    lblPhoneError.Visible = true;
                    lblPhoneError.Text = phoneError;
                    txtPhone.Focus();
                }
            }
            //Check if no country is selected:
            if(drpCountries.SelectedIndex == 0)
            {
                correct = false;
                lblCountryError.Visible = true;
                lblCountryError.Text = "Invalid input: Please select a country.";
                drpCountries.Focus();
            }
            //Check selected role:
            string roleError = "";
            if (!error.validRole(drpRole.SelectedIndex, out roleError))
            {
                correct = false;
                lblRoleError.Visible = true;
                lblRoleError.Text = roleError;
                drpRole.Focus();
            }
            if (txtPatientId.Visible == true)
            {
                string patientId = "";
                if (
                    //error.ContainsSpecialChars(txtLastname.Text, out lastnameError) || 
                    !error.validPatientId(txtPatientId.Text, out patientId))
                {
                    correct = false;
                    lblPatientIdError.Visible = true;
                    lblPatientIdError.Text = patientId;
                    txtPatientId.Focus();
                }
            }
            else
            {
                lblPatientIdError.Visible = false;
            }
            return correct;
        }

        //The below is the database code which can be separated in another class if needed:
        protected void storeInput()
        {
            //store role as an int to the temp table in the database: Admin = 1, Physician = 2, and Patient = 3.
            Configuration config = new Configuration();
            SqlConnection connect = new SqlConnection(config.getConnectionString());
            connect.Open();
            //If special characters are needed, then the below can be used for all strings
            //to replace a special character which can cause SQL errors.
            txtCity.Text = txtCity.Text.Replace("'", "''");
            txtAddress.Text = txtAddress.Text.Replace("'", "''");
            txtFirstname.Text = txtFirstname.Text.Replace("'", "''");
            txtLastname.Text = txtLastname.Text.Replace("'", "''");
            txtEmail.Text = txtEmail.Text.Replace("'", "''");
            txtPhone.Text = txtPhone.Text.Replace("'", "''");
            txtPatientId.Text = txtPatientId.Text.Replace("'", "''");
            txtZip.Text = txtZip.Text.Replace("'", "''");
            txtState.Text = txtState.Text.Replace("'", "''");
            txtPhone.Text = txtPhone.Text.Replace(" ", "");
            txtPhone.Text = txtPhone.Text.Replace("'", "''");
            string state = "";
            if (!drpCountries.SelectedValue.Equals("United States"))
            {
                state = txtState.Text.Replace("'", "''");
            }
            else
            {
                state = drpStates.SelectedItem.ToString();
            }
            SqlCommand cmd = connect.CreateCommand();
            cmd.CommandText = "insert into Registrations(register_firstname, register_lastname, register_email, register_city, register_state, register_zip, register_address, register_roleId, register_phone, register_patientId, register_country)" +
                " values ('" + txtFirstname.Text+"', '"+txtLastname.Text+"', '"+txtEmail.Text+"', '"+txtCity.Text+"', '"+ state + "'," +
                " '"+txtZip.Text+"', '"+txtAddress.Text+"', '"+drpRole.SelectedIndex+"','"+txtPhone.Text+"', '"+txtPatientId.Text+"', '"+drpCountries.SelectedItem.ToString()+"') ";
            cmd.ExecuteScalar();
            connect.Close();
        }

        protected void drpRole_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(drpRole.SelectedIndex == 3)
            {
                lblPatientId.Visible = true;
                txtPatientId.Visible = true;
            }
            else
            {
                lblPatientId.Visible = false;
                txtPatientId.Visible = false;
                lblPatientIdError.Visible = false;
            }
        }

        protected void drpCountries_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(!drpCountries.SelectedValue.Equals("United States"))
            {
                drpStates.Visible = false;
                txtState.Visible = true;
            }
            else
            {
                drpStates.Visible = true;
                txtState.Visible = false;
            }
        }
    }
}