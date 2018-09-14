using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NephroNet.Accounts.Patient
{
    public partial class CreateTopic : System.Web.UI.Page
    {
        static string conn = "";
        SqlConnection connect = new SqlConnection(conn);
        string username, roleId, loginId, token;
        protected void Page_Load(object sender, EventArgs e)
        {
            Configuration config = new Configuration();
            conn = config.getConnectionString();
            connect = new SqlConnection(conn);
            getSession();
            //addSession();
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
            FileUpload1.Attributes.Clear();
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
            //Add the creator to UsersForTopics:
            cmd.CommandText = "insert into UsersForTopics (userId, topicId, joined_time) values ('" + userId + "', '" + topicId + "', '"+DateTime.Now+"')";
            cmd.ExecuteScalar();
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
            cmd.CommandText = "insert into Topics (topic_createdBy, topic_type, topic_title, topic_time, topic_description, topic_hasImage, topic_isDeleted, topic_isApproved, topic_isDenied, topic_isTerminated) values " +
                "('" + userId + "', '" + drpType.SelectedValue + "', '" + title + "', '" + entryTime + "', '" + description + "', '" + hasImage + "', '0', '0', '0', '0')";
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