using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NephroNet.Accounts.Admin
{
    public partial class ViewTopic : System.Web.UI.Page
    {
        static string conn = "";
        SqlConnection connect = new SqlConnection(conn);
        string username, roleId, loginId, token;
        string topicId = ""; int g_entries = 0;
        static string previousPage = "";
        static string currentPage = "";
        static bool requestedRemoveTopic = false;
        static bool requestedRemoveMessage = false;
        protected void Page_Load(object sender, EventArgs e)
        {
            Configuration config = new Configuration();
            conn = config.getConnectionString();
            connect = new SqlConnection(conn);
            getSession();
            CheckAdminSession session = new CheckAdminSession();
            bool correctSession = session.sessionIsCorrect(username, roleId, token);
            if (!correctSession)
                clearSession();
            topicId = Request.QueryString["id"];
            CheckErrors check = new CheckErrors();
            if (!check.isDigit(topicId))
                goBack();
            int pageNum = Convert.ToInt32(Request.QueryString["page"]);
            bool topicApproved = isTopicApproved();
            if (!topicApproved)
                topicNotApproved();
            bool authorized = isUserAuthorizedToView();
            if (!authorized)
                unauthorized();
            showInformation(pageNum);
            checkIfTerminated();
            checkIfDeleted();
            if (!IsPostBack)
            {
                if (!requestedRemoveTopic && !requestedRemoveMessage)
                {
                    if (HttpContext.Current.Request.Url.AbsoluteUri != null) currentPage = HttpContext.Current.Request.Url.AbsoluteUri;
                    else currentPage = "Home.aspx";
                    if (Request.UrlReferrer != null) previousPage = Request.UrlReferrer.ToString();
                    else previousPage = "Home.aspx";
                    if (currentPage.Equals(previousPage))
                        previousPage = "Home.aspx";
                }
            }
        }
        protected bool isTopicApproved()
        {
            bool approved = true;
            connect.Open();
            SqlCommand cmd = connect.CreateCommand();
            cmd.CommandText = "select count(*) from topics where topicId = '" + topicId + "' and topic_isApproved = 1 ";
            int totalTopics = Convert.ToInt32(cmd.ExecuteScalar());
            connect.Close();
            if (totalTopics == 0)
                approved = false;
            return approved;
        }
        protected void topicNotApproved()
        {
            hideErrorLabels();
            hideLabels();
            lblError.Visible = true;
            lblError.Text = "The topic you are trying to access has not been aproved yet!";
        }
        protected void hideLabels()
        {
            btnSubmit.Visible = false;
            FileUpload1.Visible = false;
            txtEntry.Visible = false;
            lblContents.Visible = false;
            lblEntry.Visible = false;
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
        protected bool isUserAuthorizedToView()
        {
            bool authorized = true;
            connect.Open();
            SqlCommand cmd = connect.CreateCommand();
            //Get the userId:
            cmd.CommandText = "select userId from Users where loginId = '" + loginId + "' ";
            string userId = cmd.ExecuteScalar().ToString();
            //Check if the topic exist by counting its ID:
            cmd.CommandText = "select count(*) from Topics where topicId = '" + topicId + "' ";
            int topicCount = Convert.ToInt32(cmd.ExecuteScalar());
            if (topicCount == 0)
                goBack();
            //check if topic is for dissemination. If type = Dissemination, then ignore authorization:
            cmd.CommandText = "select topic_type from Topics where topicId = '" + topicId + "' ";
            string type = cmd.ExecuteScalar().ToString();
            if (type.Equals("Discussion"))
            {
                //Count to check if the user has requested join:
                //cmd.CommandText = "select count(*) from UsersForTopics where topicId = '" + topicId + "' and userId = '" + userId + "'  ";
                //int countUserForTopic = Convert.ToInt32(cmd.ExecuteScalar());
                //if (countUserForTopic > 0)
                //{
                //    //Check if the user is approved:
                //    cmd.CommandText = "select isApproved from UsersForTopics where topicId = '" + topicId + "' and userId = '" + userId + "'  ";
                //    int isApproved = Convert.ToInt32(cmd.ExecuteScalar());
                //    if (isApproved == 0)
                //        authorized = false;
                //}
                //else
                //    authorized = false;
            }
            else if (type.Equals("Dissemination"))
            {
                hideLabels();
                hideErrorLabels();
                lblError.Visible = true;
                lblError.Text = "This is a dissemination topic and no participations are allowed";
            }
            else
            {
                goBack();
            }
            connect.Close();
            return authorized;
        }
        protected void unauthorized()
        {
            addSession();
            Response.Redirect("JoinTopic.aspx?id=" + topicId);
        }
        protected string getHeader()
        {
            string header = "";
            connect.Open();
            SqlCommand cmd = connect.CreateCommand();
            //Check if the ID exists in the database:
            cmd.CommandText = "select count(*) from topics where topicId = '" + topicId + "' ";
            int countTopic = Convert.ToInt32(cmd.ExecuteScalar());
            if (countTopic > 0)//if ID exists, countTopic = 1
            {
                //Get topic_createdBy:
                cmd.CommandText = "select topic_createdBy from [Topics] where [topicId] = '" + topicId + "' ";
                string topic_createdBy = cmd.ExecuteScalar().ToString();
                //Get creator's email:
                cmd.CommandText = "select user_email from users where userId = '" + topic_createdBy + "' ";
                string email = cmd.ExecuteScalar().ToString();
                //Get creator's fullname:
                cmd.CommandText = "select user_firstname from users where userId = '" + topic_createdBy + "' ";
                string creator = cmd.ExecuteScalar().ToString();
                cmd.CommandText = "select user_lastname from users where userId = '" + topic_createdBy + "' ";
                creator = creator + " " + cmd.ExecuteScalar().ToString();
                //Get topic_type:
                cmd.CommandText = "select topic_type from [Topics] where [topicId] = '" + topicId + "' ";
                string topic_type = cmd.ExecuteScalar().ToString();
                //Get topic_title:
                cmd.CommandText = "select topic_title from [Topics] where [topicId] = '" + topicId + "' ";
                string topic_title = cmd.ExecuteScalar().ToString();
                //Get topic_time:
                cmd.CommandText = "select topic_time from [Topics] where [topicId] = '" + topicId + "' ";
                string topic_time = cmd.ExecuteScalar().ToString();
                //Get topic_description:
                cmd.CommandText = "select topic_description from [Topics] where [topicId] = '" + topicId + "' ";
                string topic_description = cmd.ExecuteScalar().ToString();
                //Get "Yes" or "No" for topic_hasImage:
                cmd.CommandText = "select topic_hasImage from [Topics] where [topicId] = '" + topicId + "' ";
                int topic_hasImage = Convert.ToInt32(cmd.ExecuteScalar());

                //Get topic_isDeleted ?:
                cmd.CommandText = "select topic_isDeleted from [Topics] where [topicId] = '" + topicId + "' ";
                int int_topic_isDeleted = Convert.ToInt32(cmd.ExecuteScalar());

                //Get topic_isApproved ?:
                cmd.CommandText = "select topic_isApproved from [Topics] where [topicId] = '" + topicId + "' ";
                int int_topic_isApproved = Convert.ToInt32(cmd.ExecuteScalar());

                //Get topic_isDenied ?:
                cmd.CommandText = "select topic_isDenied from [Topics] where [topicId] = '" + topicId + "' ";
                int int_topic_isDenied = Convert.ToInt32(cmd.ExecuteScalar());

                //Get topic_isTerminated ?:
                cmd.CommandText = "select topic_isTerminated from [Topics] where [topicId] = '" + topicId + "' ";
                int int_topic_isTerminated = Convert.ToInt32(cmd.ExecuteScalar());

                //Get tags:
                string tagNames = "";
                cmd.CommandText = "select count(*) from TagsForTopics where topicId = '" + topicId + "' ";
                int totalTags = Convert.ToInt32(cmd.ExecuteScalar());
                if (totalTags == 0)
                    tagNames = "There are no tags for the selected topic";
                for (int i = 1; i <= totalTags; i++)
                {
                    cmd.CommandText = "select [tagId] from(SELECT rowNum = ROW_NUMBER() OVER(ORDER BY tagId ASC), * FROM [TagsForTopics] where topicId = '" + topicId + "') as t where rowNum = '" + i + "'";
                    string tagId = cmd.ExecuteScalar().ToString();
                    cmd.CommandText = "select tag_name from Tags where tagId = '" + tagId + "' ";
                    if (totalTags == 1)
                        tagNames = cmd.ExecuteScalar().ToString();
                    else if (totalTags > 1)
                    {
                        if (i == 0)
                            tagNames = cmd.ExecuteScalar().ToString();
                        else
                            tagNames = tagNames + ", " + cmd.ExecuteScalar().ToString();
                    }
                }
                //Create an informative message containing all information for the selected user:
                string imagesHTML = "";
                if (topic_hasImage == 1)
                {
                    cmd.CommandText = "select count(*) from ImagesForTopics where topicId = '" + topicId + "' ";
                    int totalImages = Convert.ToInt32(cmd.ExecuteScalar());
                    for (int i = 1; i <= totalImages; i++)
                    {
                        cmd.CommandText = "select [imageId] from(SELECT rowNum = ROW_NUMBER() OVER(ORDER BY imageId ASC), * FROM [ImagesForTopics] where topicId = '" + topicId + "') as t where rowNum = '" + i + "'";
                        string imageId = cmd.ExecuteScalar().ToString();
                        cmd.CommandText = "select image_name from Images where imageId = '" + imageId + "' ";
                        string image_name = cmd.ExecuteScalar().ToString();
                        imagesHTML = imagesHTML + "<img src='../../images/" + image_name + "'></img> <br />";
                    }
                }
                //Get userId of current user viewing:
                cmd.CommandText = "select userId from Users where loginId = '" + loginId + "' ";
                string userId = cmd.ExecuteScalar().ToString();
                header = Layouts.postHeader(creator, topic_type, topic_title, topic_time, topic_description, imagesHTML, roleId, userId, topicId, topic_createdBy);
            }
            else
            {
                addSession();
                Response.Redirect("Home");
            }
            connect.Close();
            return header;
        }
        protected string getContents(int pageNum)
        {
            string content = "";
            connect.Open();
            SqlCommand cmd = connect.CreateCommand();
            //Count entries for this topic that are approved, not denied, and not deleted:
            cmd.CommandText = "select count(*) from Entries where topicId = '" + topicId + "' and entry_isApproved = 1 and entry_isDenied = 0 and entry_isDeleted = 0 ";
            int totalEntries = Convert.ToInt32(cmd.ExecuteScalar());
            g_entries = totalEntries;
            for (int i = 1; i <= totalEntries; i++)
            {
                //Get entry ID:
                cmd.CommandText = "select [entryId] from (SELECT rowNum = ROW_NUMBER() OVER(ORDER BY entryId ASC), * FROM [Entries] where topicId = '" + topicId + "' and entry_isApproved = 1 and entry_isDenied = 0 and entry_isDeleted = 0) as t where rowNum = '" + i + "'";
                string entryId = cmd.ExecuteScalar().ToString();
                //Get entry text:
                cmd.CommandText = "select [entry_text] from (SELECT rowNum = ROW_NUMBER() OVER(ORDER BY entryId ASC), * FROM [Entries] where topicId = '" + topicId + "' and entry_isApproved = 1 and entry_isDenied = 0 and entry_isDeleted = 0) as t where rowNum = '" + i + "'";
                string entry_text = cmd.ExecuteScalar().ToString();
                //Get entry time:
                cmd.CommandText = "select [entry_time] from (SELECT rowNum = ROW_NUMBER() OVER(ORDER BY entryId ASC), * FROM [Entries] where topicId = '" + topicId + "' and entry_isApproved = 1 and entry_isDenied = 0 and entry_isDeleted = 0) as t where rowNum = '" + i + "'";
                string entry_time = cmd.ExecuteScalar().ToString();
                //Get entry's creator:
                cmd.CommandText = "select [userId] from (SELECT rowNum = ROW_NUMBER() OVER(ORDER BY entryId ASC), * FROM [Entries] where topicId = '" + topicId + "' and entry_isApproved = 1 and entry_isDenied = 0 and entry_isDeleted = 0) as t where rowNum = '" + i + "'";
                string entry_creatorId = cmd.ExecuteScalar().ToString();
                cmd.CommandText = "select user_firstname from users where userId = '" + entry_creatorId + "' ";
                string creator_name = cmd.ExecuteScalar().ToString();
                cmd.CommandText = "select user_lastname from users where userId = '" + entry_creatorId + "' ";
                creator_name = creator_name + " " + cmd.ExecuteScalar().ToString();
                //Check if entry has images:
                string imagesHtml = "";
                cmd.CommandText = "select [entry_hasImage] from (SELECT rowNum = ROW_NUMBER() OVER(ORDER BY entryId ASC), * FROM [Entries] where topicId = '" + topicId + "' and entry_isApproved = 1 and entry_isDenied = 0 and entry_isDeleted = 0) as t where rowNum = '" + i + "'";
                int hasImage = Convert.ToInt32(cmd.ExecuteScalar());
                if (hasImage == 1)
                {
                    //Count total images for this entry:
                    cmd.CommandText = "select count(*) from ImagesForEntries where entryId = '" + entryId + "' ";
                    int totalImages = Convert.ToInt32(cmd.ExecuteScalar());
                    //Loop through images and store their names:
                    for (int j = 1; j <= totalImages; j++)
                    {
                        //Get the entry's images:
                        cmd.CommandText = "select [imageId] from (SELECT rowNum = ROW_NUMBER() OVER(ORDER BY imagesForEntriesId ASC), * FROM [imagesForEntries] where entryId = '" + entryId + "' ) as t where rowNum = '" + j + "'";
                        string imageId = cmd.ExecuteScalar().ToString();
                        //Get the image name:
                        cmd.CommandText = "select image_name from images where imageId = '" + imageId + "' ";
                        string image_name = cmd.ExecuteScalar().ToString();
                        imagesHtml = imagesHtml + "<img src='../../images/" + image_name + "'></img> <br /> <br/>";
                    }
                }
                //Get userId of current user viewing:
                cmd.CommandText = "select userId from Users where loginId = '" + loginId + "' ";
                string userId = cmd.ExecuteScalar().ToString();
                //Get topic creator ID of current user viewing:
                cmd.CommandText = "select topic_createdBy from Topics where topicId = '" + topicId + "' ";
                string topic_creatorId = cmd.ExecuteScalar().ToString();
                content = content + Layouts.postMessage(i, creator_name, entry_time, entry_text, imagesHtml, entry_creatorId, topic_creatorId, userId, entryId, roleId, topicId);
            }
            connect.Close();
            return content;
        }
        protected void showInformation(int pageNum)
        {
            //Show header:
            lblHeader.Text = getHeader();
            //Display info:
            lblContents.Text = getContents(pageNum);
            //Maybe create new labels and place them for each entry:
            //<div id="div1" runat = "sever" ></div> //<-- Add this in the web page as a placeholder for the label.
            //Label lblNew = new Label();
            //div1.Controls.Add(lblNew);
            //lblNew.Visible = true;
        }
        protected void checkIfTerminated()
        {
            // hide 3 things if terminated.
            connect.Open();
            SqlCommand cmd = connect.CreateCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "select topic_isTerminated from Topics where topicId = '" + topicId + "' ";
            int terminated = Convert.ToInt32(cmd.ExecuteScalar());
            if (terminated == 1)
            {
                lblEntry.Visible = false;
                txtEntry.Visible = false;
                btnSubmit.Visible = false;
                FileUpload1.Visible = false;
                lblError.Visible = true;
                lblError.Text = "This discussion has been terminated and no more messages can be added.";
            }
            connect.Close();
        }
        protected void checkIfDeleted()
        {
            connect.Open();
            SqlCommand cmd = connect.CreateCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "select topic_isDeleted from Topics where topicId = '" + topicId + "' ";
            int deleted = Convert.ToInt32(cmd.ExecuteScalar());
            connect.Close();
            if (deleted == 1)
            {
                if (Convert.ToInt32(roleId) == 1)//if the one trying to access is an admin:
                {
                    lblEntry.Visible = false;
                    txtEntry.Visible = false;
                    btnSubmit.Visible = false;
                    FileUpload1.Visible = false;
                    lblError.Visible = true;
                    lblError.Text = "This discussion has been deleted and no more messages can be added.";
                }
                else
                    goBack();
            }
        }
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            if (!requestedRemoveTopic && !requestedRemoveMessage)
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
            if (requestedRemoveTopic) requestedRemoveTopic = false;
            if (requestedRemoveMessage) requestedRemoveMessage = false;
            clearInputs();
        }
        protected void clearInputs()
        {
            txtEntry.Text = "";
            txtEntry.Text = "";
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
            //Get topic title:
            cmd.CommandText = "select topic_title from Topics where topicId = '" + topicId + "' ";
            string topic_title = cmd.ExecuteScalar().ToString();
            connect.Close();
            string messageBody = "Hello " + name + ",\nThis email is to notify you that your message for the topic titled (" + topic_title + ") has been successfully submitted and will be reviewed.\n" +
                "You will be notified by email once the review is complete. The below is the message text you typed: \n\n\"" + txtEntry.Text + "\"\n\nBest regards,\nNephroNet Support\nNephroNet2018@gmail.com";
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
            string entryId = storeEntry(hasImage);
            storeImagesInDB(entryId, hasImage, files);
            lblError.Visible = true;
            lblError.ForeColor = System.Drawing.Color.Green;
            lblError.Text = "The message has been successfully submitted and an email notification has been sent to you. <br/>" +
                "Your message will be reviewed and you will be notified by email once the review is complete.";
        }
        protected void storeImagesInDB(string entryId, int hasImage, ArrayList files)
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
                    cmd.CommandText = "insert into ImagesForEntries (imageId, entryId) values ('" + imageId + "', '" + entryId + "')";
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
        protected string storeEntry(int hasImage)
        {
            string entryId = "";
            DateTime entry_time = DateTime.Now;
            connect.Open();
            SqlCommand cmd = connect.CreateCommand();
            string description = txtEntry.Text.Replace("'", "''");
            description = description.Replace("\n", "<br />");
            description = description.Replace("\r", "&nbsp;&nbsp;&nbsp;&nbsp;");
            //Get the current user's ID:
            cmd.CommandText = "select userId from Users where loginId = '" + loginId + "' ";
            string userId = cmd.ExecuteScalar().ToString();
            cmd.CommandText = "insert into Entries (topicId, userId, entry_time, entry_text, entry_isDeleted, entry_isApproved, entry_isDenied, entry_hasImage) values " +
                "('" + topicId + "', '" + userId + "', '" + entry_time + "', '" + description + "', ' 0 ', '0', '0', '" + hasImage + "')";
            cmd.ExecuteScalar();
            cmd.CommandText = "select [entryId] from(SELECT rowNum = ROW_NUMBER() OVER(ORDER BY entryId ASC), * FROM [Entries] " +
                "where topicId = '" + topicId + "' and userId = '" + userId + "' and entry_isDeleted = '0' and entry_hasImage = '" + hasImage +
                "' and entry_isApproved = '0' and entry_isDenied = '0' " +
                " ) as t where rowNum = '1'";
            entryId = cmd.ExecuteScalar().ToString();
            connect.Close();
            return entryId;
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
            if (string.IsNullOrWhiteSpace(txtEntry.Text))
            {
                correct = false;
                lblEntryError.Visible = true;
                lblEntryError.Text = "Input Error: Please type something for the title.";
            }
            return correct;
        }

        [WebMethod]
        [ScriptMethod()]
        public static void terminateTopic_Click(string topicId, string entry_creatorId)
        {

            Configuration config = new Configuration();
            SqlConnection connect = new SqlConnection(config.getConnectionString());
            bool topicIdExists = isTopicCorrect(topicId, entry_creatorId);
            
            if (topicIdExists)
            {
                connect.Open();
                SqlCommand cmd = connect.CreateCommand();
                //update the DB and set topic_isTerminated = true:
                cmd.CommandText = "update Topics set topic_isTerminated = 1 where topicId = '" + topicId + "' ";
                cmd.ExecuteScalar();
                //Email the topic creator about the topic being deleted:
                cmd.CommandText = "select topic_createdBy from Topics where topicId = '" + topicId + "' ";
                string creatorId = cmd.ExecuteScalar().ToString();
                cmd.CommandText = "select topic_title from Topics where topicId = '" + topicId + "' ";
                string topic_title = cmd.ExecuteScalar().ToString();
                cmd.CommandText = "select user_firstname from Users where userId = '" + creatorId + "' ";
                string name = cmd.ExecuteScalar().ToString();
                cmd.CommandText = "select user_lastname from Users where userId = '" + creatorId + "' ";
                name = name + " " + cmd.ExecuteScalar().ToString();
                cmd.CommandText = "select user_email from Users where userId = '" + creatorId + "' ";
                string emailTo = cmd.ExecuteScalar().ToString();
                connect.Close();
                string emailBody = "Hello " + name + ",\n\n" +
                    "This email is to inform you that your topic with the title (" + topic_title + ") has been terminated. If you think this happened by mistake, or you did not perform this action, plaese contact the support.\n\n" +
                    "Best regards,\nNephroNet Support\nNephroNet2018@gmail.com";
                Email email = new Email();
                email.sendEmail(emailTo, emailBody);
            }
        }
        [WebMethod]
        [ScriptMethod()]
        public static void removeTopic_Click(string topicId, string entry_creatorId)
        {
            requestedRemoveTopic = true;
            Configuration config = new Configuration();
            SqlConnection connect = new SqlConnection(config.getConnectionString());
            bool topicIdExists = isTopicCorrect(topicId, entry_creatorId);
            if (topicIdExists)
            {
                connect.Open();
                SqlCommand cmd = connect.CreateCommand();
                //update the DB and set isDeleted = true:
                cmd.CommandText = "update Topics set topic_isDeleted = 1 where topicId = '" + topicId + "' ";
                cmd.ExecuteScalar();
                //Email the topic creator about the topic being deleted:
                cmd.CommandText = "select topic_createdBy from Topics where topicId = '" + topicId + "' ";
                string creatorId = cmd.ExecuteScalar().ToString();
                cmd.CommandText = "select topic_title from Topics where topicId = '" + topicId + "' ";
                string topic_title = cmd.ExecuteScalar().ToString();
                cmd.CommandText = "select user_firstname from Users where userId = '" + creatorId + "' ";
                string name = cmd.ExecuteScalar().ToString();
                cmd.CommandText = "select user_lastname from Users where userId = '" + creatorId + "' ";
                name = name + " " + cmd.ExecuteScalar().ToString();
                cmd.CommandText = "select user_email from Users where userId = '" + creatorId + "' ";
                string emailTo = cmd.ExecuteScalar().ToString();
                connect.Close();
                string emailBody = "Hello " + name + ",\n\n" +
                    "This email is to inform you that your topic with the title (" + topic_title + ") has been deleted. If you think this happened by mistake, or you did not perform this action, plaese contact the support.\n\n" +
                    "Best regards,\nNephroNet Support\nNephroNet2018@gmail.com";
                Email email = new Email();
                email.sendEmail(emailTo, emailBody);
            }
        }
        protected static bool isTopicCorrect(string topicId, string creatorId)
        {
            bool correct = true;
            CheckErrors errors = new CheckErrors();
            //check if id contains a special character:
            if (!errors.isDigit(topicId))
                correct = false;
            //check if id contains an id that does not exist in DB:
            else if (errors.ContainsSpecialChars(topicId))
                correct = false;
            if (correct)
            {
                Configuration config = new Configuration();
                SqlConnection connect = new SqlConnection(config.getConnectionString());
                connect.Open();
                SqlCommand cmd = connect.CreateCommand();
                //Count the existance of the topic:
                cmd.CommandText = "select count(*) from Topics where topicId = '" + topicId + "' ";
                int count = Convert.ToInt32(cmd.ExecuteScalar());
                if (count > 0)//if count > 0, then the topic ID exists in DB.
                {
                    cmd.CommandText = "select topic_createdBy from Topics where topicId = '" + topicId + "' ";
                    string actual_creatorId = cmd.ExecuteScalar().ToString();
                    //cmd.CommandText = "select userId from Users where loginId = '" + loginId + "' ";
                    //string userId = cmd.ExecuteScalar().ToString();
                    cmd.CommandText = "select topic_isDeleted from Topics where topicId = '" + topicId + "' ";
                    int isDeleted = Convert.ToInt32(cmd.ExecuteScalar());

                    //check if id belongs to a different user:
                    //if (!userId.Equals(creatorId))
                    //    correct = false;
                    //else 
                    if (isDeleted == 1)
                        correct = false;
                }
                else
                    correct = false; // means that the topic ID does not exists in DB.
                connect.Close();
            }
            return correct;
        }
        [WebMethod]
        [ScriptMethod()]
        public static void removeMessage_Click(string entryId, string entry_creatorId)
        {
            requestedRemoveMessage = true;
            bool messageIdExists = isMessageCorrect(entryId, entry_creatorId);
            if (messageIdExists)
            {
                Configuration config = new Configuration();
                SqlConnection connect = new SqlConnection(config.getConnectionString());
                connect.Open();
                SqlCommand cmd = connect.CreateCommand();
                //update the DB and set isDeleted = true:
                cmd.CommandText = "update Entries set entry_isDeleted = 1 where entryId = '" + entryId + "' ";
                cmd.ExecuteScalar();
                //connect.Close();
                //Email the topic creator about the topic being deleted:
                cmd.CommandText = "select userId from Entries where entryId = '" + entryId + "' ";
                string creatorId = cmd.ExecuteScalar().ToString();
                cmd.CommandText = "select entry_text from Entries where entryId = '" + entryId + "' ";
                string entry_text = cmd.ExecuteScalar().ToString();
                cmd.CommandText = "select user_firstname from Users where userId = '" + creatorId + "' ";
                string name = cmd.ExecuteScalar().ToString();
                cmd.CommandText = "select user_lastname from Users where userId = '" + creatorId + "' ";
                name = name + " " + cmd.ExecuteScalar().ToString();
                cmd.CommandText = "select user_email from Users where userId = '" + creatorId + "' ";
                string emailTo = cmd.ExecuteScalar().ToString();
                cmd.CommandText = "select topicId from Entries where entryId = '" + entryId + "' ";
                string topicId = cmd.ExecuteScalar().ToString();
                cmd.CommandText = "select topic_title from Topics where topicId = '" + topicId + "' ";
                string topic_title = cmd.ExecuteScalar().ToString();
                connect.Close();
                string emailBody = "Hello " + name + ",\n\n" +
                    "This email is to inform you that your message in the topic with the title (" + topic_title + ") has been deleted. The message was:\n" +
                    "\"" + entry_text + "\" \nIf you think this happened by mistake, or you did not perform this action, plaese contact the support.\n\n" +
                    "Best regards,\nNephroNet Support\nNephroNet2018@gmail.com";
                Email email = new Email();
                email.sendEmail(emailTo, emailBody);
            }
        }
        protected static bool isMessageCorrect(string messageId, string creatorId)
        {
            Configuration config = new Configuration();
            SqlConnection connect = new SqlConnection(config.getConnectionString());
            bool correct = true;
            CheckErrors errors = new CheckErrors();
            //check if id contains a special character:
            if (!errors.isDigit(messageId))
                correct = false;
            //check if id contains an id that does not exist in DB:
            else if (errors.ContainsSpecialChars(messageId))
                correct = false;
            if (correct)
            {
                connect.Open();
                SqlCommand cmd = connect.CreateCommand();
                //Count the existance of the message:
                cmd.CommandText = "select count(*) from Entries where entryId = '" + messageId + "' ";
                int count = Convert.ToInt32(cmd.ExecuteScalar());
                if (count > 0)//if count > 0, then the message ID exists in DB.
                {
                    //Get creator ID:
                    cmd.CommandText = "select userId from Entries where entryId = '" + messageId + "' ";
                    string actual_creatorId = cmd.ExecuteScalar().ToString();
                    //Get the current user's ID who is trying to access the message:
                    //cmd.CommandText = "select userId from Users where loginId = '" + loginId + "' ";
                    //string userId = cmd.ExecuteScalar().ToString();
                    //Get the deletion's status:
                    cmd.CommandText = "select entry_isDeleted from Entries where entryId = '" + messageId + "' ";
                    int isDeleted = Convert.ToInt32(cmd.ExecuteScalar());

                    //check if id belongs to a different user:
                    //Admins can delete anything!
                    //if (!userId.Equals(creatorId))
                    //    correct = false;
                    //else
                    if (isDeleted == 1)
                        correct = false;
                }
                else
                    correct = false; // means that the topic ID does not exists in DB.
                connect.Close();
            }
            return correct;
        }

        protected void hideErrorLabels()
        {
            lblImageError.Visible = false;
            lblError.Visible = false;
        }
        protected void btnCancel_Click(object sender, EventArgs e)
        {
            goBack();
        }
        protected void goBack()
        {
            addSession();
            if (!string.IsNullOrWhiteSpace(previousPage))
                Response.Redirect(previousPage);
            else
                Response.Redirect("Home.aspx");
        }
    }
}