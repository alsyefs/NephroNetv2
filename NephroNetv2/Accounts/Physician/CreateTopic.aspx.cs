using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NephroNet.Accounts.Physician
{
    public partial class CreateTopic : System.Web.UI.Page
    {
        static string conn = "";
        SqlConnection connect = new SqlConnection(conn);
        string username, roleId, loginId, token;
        static ArrayList consultationUsers = new ArrayList();
        protected void Page_Load(object sender, EventArgs e)
        {
            initialAccess();
        }
        protected void initialAccess()
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
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            hideErrorLabels();
            Boolean correct = checkInput();
            if (correct)
            {
                addNewEntry();
                clearInputs();
                sendEmail();
            }
        }
        protected void clearInputs()
        {
            txtTitle.Text = "";
            txtTags.Text = "";
            txtDescription.Text = "";
            //FileUpload1.Attributes.Clear();
        }
        protected void sendEmail()
        {
            connect.Open();
            SqlCommand cmd = connect.CreateCommand();
            cmd.CommandText = "select userId from Users where loginId = '" + loginId + "' ";
            string userId = cmd.ExecuteScalar().ToString();
            cmd.CommandText = "select user_email from Users where userId like '" + userId + "' ";
            string emailTo = cmd.ExecuteScalar().ToString();
            cmd.CommandText = "select user_firstname from Users where userId like '" + userId + "' ";
            string name = cmd.ExecuteScalar().ToString();
            cmd.CommandText = "select user_lastname from Users where userId like '" + userId + "' ";
            name = name + " " + cmd.ExecuteScalar().ToString();
            connect.Close();
            string messageBody = "Hello " + name + ",\nThis email is to notify you that your topic (" + txtTitle.Text + ") has been successfully submitted and will be reviewed.\n" +
                "You will be notified by email once the review is complete. The below is the description you typed: \n\n\"" + txtDescription.Text + "\"\n\nBest regards,\nNephroNet Support\nNephroNet2018@gmail.com";
            Email email = new Email();
            email.sendEmail(emailTo, messageBody);
        }
        protected void addNewEntry()
        {
            //string imageName="";
            int hasImage = 0;
            ArrayList files = new ArrayList();
            if (FileUpload1.HasFile)
            {
                //Count number of files:
                int fileCount = FileUpload1.PostedFiles.Count;
                for (int i = 0; i < fileCount; i++)
                {
                    //Store the file names in an array list:
                    files.Add(FileUpload1.PostedFiles[i].FileName);
                }
                storeImagesInServer();
                hasImage = 1;
            }
            //Store new topic as neither approved nor denied and return its ID:
            string topicId = storeTopic(hasImage);
            //Allow the creator of topic to access it when it's approved and add the new tags to the topic:
            allowUserAccessTopicAndStoreTags(topicId);
            storeImagesInDB(topicId, hasImage, files);
            lblError.Visible = true;
            lblError.ForeColor = System.Drawing.Color.Green;
            lblError.Text = "The topic has been successfully submitted and an email notification has been sent to you. <br/>" +
                "Your topic will be reviewed and you will be notified by email once the review is complete.";
        }
        protected void storeImagesInDB(string topicId, int hasImage, ArrayList files)
        {
            connect.Open();
            SqlCommand cmd = connect.CreateCommand();
            //Check if there is an image:
            if (hasImage == 1)
            {
                for (int i = 0; i < files.Count; i++)
                {
                    string imageName = files[i].ToString().Replace("'", "''");
                    //Add to Images:
                    cmd.CommandText = "insert into Images (image_name) values ('" + imageName + "')";
                    cmd.ExecuteScalar();
                    //Get the image ID:
                    cmd.CommandText = "select imageId from Images where image_name like '" + imageName + "' ";
                    string imageId = cmd.ExecuteScalar().ToString();
                    //Add ImagesForTopics:
                    cmd.CommandText = "insert into ImagesForTopics (imageId, topicId) values ('" + imageId + "', '" + topicId + "')";
                    cmd.ExecuteScalar();
                }
            }
            connect.Close();
        }
        protected void allowUserAccessTopicAndStoreTags(string topicId)
        {
            connect.Open();
            SqlCommand cmd = connect.CreateCommand();
            //Get the current user's ID:
            cmd.CommandText = "select userId from Users where loginId = '" + loginId + "' ";
            string userId = cmd.ExecuteScalar().ToString();
            //Check if there is a tag entered:
            if (!string.IsNullOrWhiteSpace(txtTags.Text))
            {
                //Add to Tags:
                cmd.CommandText = "insert into tags (tag_name) values ('" + txtTags.Text.Replace("'", "''") + "')";
                cmd.ExecuteScalar();
                //Get the tag ID:
                cmd.CommandText = "select tagId from tags where tag_name like '" + txtTags.Text.Replace("'", "''") + "' ";
                string tagId = cmd.ExecuteScalar().ToString();
                //Store values into TagsForTopics:
                cmd.CommandText = "insert into TagsForTopics (topicId, tagId) values ('" + topicId + "', '" + tagId + "')";
                cmd.ExecuteScalar();
            }
            if(drpType.SelectedIndex == 3)//If the consultation topic type is selected:
            {

                int userIndex = drpFindUser.SelectedIndex;
                string selectedUser = drpFindUser.SelectedValue;
                var digits = new[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', ' ' };
                string result = selectedUser.TrimStart(digits);
                int int_roleId = Convert.ToInt32(roleId);
                if(int_roleId == 2)//If the current user trying to add another user is a physician:
                {
                    string temp_userId = consultationUsers[userIndex].ToString();
                    cmd.CommandText = "insert into Consultations (patient_userId, physician_userId, topicId) values" +
                    "('" + temp_userId + "', '" + userId + "', '" + topicId + "') ";
                    cmd.ExecuteScalar();
                }
                else if(int_roleId == 3)//If the current user trying to add another user is a patient
                {
                    string temp_userId = consultationUsers[userIndex].ToString();
                    cmd.CommandText = "insert into Consultations (patient_userId, physician_userId, topicId) values" +
                    "('"+userId+"', '"+ temp_userId + "', '"+topicId+"') ";
                    cmd.ExecuteScalar();
                }
            }
            connect.Close();
        }
        protected void storeImagesInServer()
        {
            //Loop through images and store each one of them:
            for (int i = 0; i < FileUpload1.PostedFiles.Count; i++)
            {
                string path = Server.MapPath("~/images/" + FileUpload1.PostedFiles[i].FileName);
                System.Drawing.Bitmap image = new System.Drawing.Bitmap(FileUpload1.PostedFiles[i].InputStream);
                System.Drawing.Bitmap image_copy = new System.Drawing.Bitmap(image);
                System.Drawing.Image img = RezizeImage(System.Drawing.Image.FromStream(FileUpload1.PostedFiles[i].InputStream), 500, 500);
                img.Save(path, ImageFormat.Jpeg);
            }
        }
        private MemoryStream BytearrayToStream(byte[] arr)
        {
            return new MemoryStream(arr, 0, arr.Length);
        }
        private System.Drawing.Image RezizeImage(System.Drawing.Image img, int maxWidth, int maxHeight)
        {
            if (img.Height < maxHeight && img.Width < maxWidth) return img;
            using (img)
            {
                Double xRatio = (double)img.Width / maxWidth;
                Double yRatio = (double)img.Height / maxHeight;
                Double ratio = Math.Max(xRatio, yRatio);
                int nnx = (int)Math.Floor(img.Width / ratio);
                int nny = (int)Math.Floor(img.Height / ratio);
                Bitmap cpy = new Bitmap(nnx, nny, PixelFormat.Format32bppArgb);
                using (Graphics gr = Graphics.FromImage(cpy))
                {
                    gr.Clear(Color.Transparent);

                    // This is said to give best quality when resizing images
                    gr.InterpolationMode = InterpolationMode.HighQualityBicubic;

                    gr.DrawImage(img,
                        new Rectangle(0, 0, nnx, nny),
                        new Rectangle(0, 0, img.Width, img.Height),
                        GraphicsUnit.Pixel);
                }
                return cpy;
            }

        }
        protected string storeTopic(int hasImage)
        {
            string topicId = "";
            DateTime entryTime = DateTime.Now;
            connect.Open();
            SqlCommand cmd = connect.CreateCommand();
            string description = txtDescription.Text.Replace("'", "''");
            description = description.Replace("\n", "<br />");
            description = description.Replace("\r", "&nbsp;&nbsp;&nbsp;&nbsp;");
            string title = txtTitle.Text.Replace("'", "''");
            title = title.Replace("'", "''");
            title = title.Replace("\n", "");
            title = title.Replace("\r", "&nbsp;&nbsp;&nbsp;&nbsp;");
            //Get the current user's ID:
            cmd.CommandText = "select userId from Users where loginId = '" + loginId + "' ";
            string userId = cmd.ExecuteScalar().ToString();
            DateTime currentTime = DateTime.Now;
            cmd.CommandText = "insert into Topics (topic_createdBy, topic_type, topic_title, topic_time, topic_description, topic_hasImage, topic_isDeleted, topic_isApproved, topic_isDenied, topic_isTerminated, topic_createdDate) values " +
                "('" + userId + "', '" + drpType.SelectedValue + "', '" + title + "', '" + entryTime + "', '" + description + "', '" + hasImage + "', '0', '0', '0', '0', '"+ currentTime + "')";
            cmd.ExecuteScalar();
            cmd.CommandText = "select [topicId] from(SELECT rowNum = ROW_NUMBER() OVER(ORDER BY topicId ASC), * FROM [topics] " +
                "where topic_createdBy = '" + userId + "' and topic_type like '" + drpType.SelectedValue + "' and topic_title like '" + title + "' "
                //+"and topic_description like '" + description 
                + " and topic_hasImage = '" + hasImage +
                "' and topic_isDeleted = '0' and topic_isApproved = '0' and topic_isDenied = '0' and topic_isTerminated = '0' " +
                " ) as t where rowNum = '1'";
            topicId = cmd.ExecuteScalar().ToString();

            connect.Close();
            return topicId;
        }
        protected void drpType_SelectedIndexChanged(object sender, EventArgs e)
        {
            drpFindUser.Items.Clear();
            txtFindUser.Text = "";
            lblFindUserResult.Text = "";
            consultationUsers.Clear();
            if (drpType.SelectedIndex == 3)//3 = Consultation
            {
                lblFindUser.Visible = true;
                txtFindUser.Visible = true;
                drpFindUser.Visible = true;
                lblSelectUser.Visible = true;
                lblFindUserResult.Visible = true;
                int int_roleId = Convert.ToInt32(roleId);
                if (int_roleId == 2)
                {
                    lblFindUser.Text = "Find patient";
                    lblSelectUser.Text = "Select patient";
                }
                else if (int_roleId == 3)
                {
                    lblFindUser.Text = "Find physician";
                    lblSelectUser.Text = "Select physician";
                }
            }
            else
            {
                lblFindUser.Visible = false;
                txtFindUser.Visible = false;
                lblFindUserResult.Visible = false;
                drpFindUser.Visible = false;
                lblSelectUser.Visible = false;
            }
        }
        protected void drpFindUser_SelectedIndexChanged(object sender, EventArgs e)
        {
            int userIndex = drpFindUser.SelectedIndex;
            string selectedUser = drpFindUser.SelectedValue;
            var digits = new[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', ' ' };
            string result = selectedUser.TrimStart(digits);
            lblFindUserResult.Text = "Selected user: " + (userIndex+1) + " " + result;
            lblFindUserResult.Visible = true;
        }
        protected void txtFindUser_TextChanged(object sender, EventArgs e)
        {
            drpFindUser.Items.Clear();
            consultationUsers.Clear();
            int counter = 0;
            connect.Open();
            SqlCommand cmd = connect.CreateCommand();
            cmd.CommandText = "select count(user_firstname + ' ' + user_lastname) from Users where (user_firstname + ' ' + user_lastname) like '%"+txtFindUser.Text.Replace("'", "''")+"%'  ";
            int count = Convert.ToInt32(cmd.ExecuteScalar());
            for (int i =1; i <= count; i++)
            {
                cmd.CommandText = "select userId from(SELECT rowNum = ROW_NUMBER() OVER(ORDER BY userId ASC), * FROM [Users] where (user_firstname + ' ' + user_lastname) like '%" + txtFindUser.Text.Replace("'", "''") + "%' ) as t where rowNum = '" + i + "'";
                int temp_userId = Convert.ToInt32(cmd.ExecuteScalar());
                cmd.CommandText = "select (user_firstname + ' ' + user_lastname) from users where userId = '"+temp_userId+"' ";
                string temp_user = cmd.ExecuteScalar().ToString();
                cmd.CommandText = "select loginId from Users where userId = '"+temp_userId+"' ";
                int temp_loginId = Convert.ToInt32(cmd.ExecuteScalar());
                cmd.CommandText = "select login_isActive from Logins where loginId = '"+temp_loginId+"' ";
                int temp_isActive = Convert.ToInt32(cmd.ExecuteScalar());
                cmd.CommandText = "select roleId from Logins where loginId = '" + temp_loginId + "' ";
                int temp_roleId = Convert.ToInt32(cmd.ExecuteScalar());
                int int_loginId = Convert.ToInt32(loginId);
                int int_roleId = Convert.ToInt32(roleId);
                if (int_roleId == 2)//If the current user is a physician
                {
                    //add the searched user if his/her account is active, he/she is a patient, and not me
                    if (temp_isActive == 1 && temp_roleId == 3 && temp_loginId != int_loginId)
                    {
                        consultationUsers.Add(temp_userId);
                        drpFindUser.Items.Add(++counter + " " + temp_user);
                    }
                }
                else if (int_roleId == 3)//If the current user is a patient
                {
                    //add the searched user if his/her account is active, he/she is a physician, and not me
                    if (temp_isActive == 1 && temp_roleId == 2 && temp_loginId != int_loginId)
                    {
                        consultationUsers.Add(temp_userId);
                        drpFindUser.Items.Add(++counter + " " + temp_user);
                    }
                }
            }
            connect.Close();
        }
        protected Boolean checkInput()
        {
            Boolean correct = true;

            if (FileUpload1.HasFile)
            {
                string fileExtension = System.IO.Path.GetExtension(FileUpload1.FileName);
                int filesize = FileUpload1.PostedFile.ContentLength;
                string filename = FileUpload1.FileName;
                if (fileExtension.ToLower() != ".jpg" && fileExtension.ToLower() != ".tiff" && fileExtension.ToLower() != ".jpeg" &&
                    fileExtension.ToLower() != ".png" && fileExtension.ToLower() != ".gif" && fileExtension.ToLower() != ".bmp" &&
                    fileExtension.ToLower() != ".tif")
                {
                    correct = false;
                    lblImageError.Visible = true;
                    lblImageError.Text = "File Error: The supported formats for files are: jpg, jpeg, tif, tiff, png, gif, and bmp.";
                }

                if (filesize > 5242880)
                {
                    correct = false;
                    lblImageError.Visible = true;
                    lblImageError.Text = "File Error: The size of any uploaded file needs to be less than 5MB.";
                }
                if (string.IsNullOrWhiteSpace(filename))
                {
                    correct = false;
                    lblImageError.Visible = true;
                    lblImageError.Text = "File Error: The file you are trying to upload must have a name.";
                }
            }
            //Check for blank title:
            if (string.IsNullOrWhiteSpace(txtTitle.Text))
            {
                correct = false;
                lblTitleError.Visible = true;
                lblTitleError.Text = "Input Error: Please type something for the title.";
            }
            //Check for blank description:
            if (string.IsNullOrWhiteSpace(txtDescription.Text))
            {
                correct = false;
                lblDescriptionError.Visible = true;
                lblDescriptionError.Text = "Input Error: Please type something for the description.";
            }
            //Check for type:
            if (drpType.SelectedIndex == 0)
            {
                correct = false;
                lblTypeError.Visible = true;
                lblTypeError.Text = "Input Error: Please select a type.";
            }
            return correct;
        }
        protected void hideErrorLabels()
        {
            lblTitleError.Visible = false;
            lblTypeError.Visible = false;
            lblDescriptionError.Visible = false;
            lblImageError.Visible = false;
            lblError.Visible = false;
        }
        protected void btnCancel_Click(object sender, EventArgs e)
        {
            addSession();
            Response.Redirect("Home");
        }
    }
}