using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Web;

namespace NephroNet
{
	public class Layouts
	{
		public static string postHeader(string creator, string topic_type, string topic_title, string topic_time, 
            string topic_description, string imagesHTML, string roleId, string userId, string topicId, string topic_creatorId)
		{
            string terminateCommand = "";
            string deleteCommand = "";
            bool isDeleted = checkDeleted(topicId);
            bool isTerminated = checkTerminated(topicId);
            //Check if the user viewing the topic is the creator, or if the current user viewing is an admin:
            int int_roleId = Convert.ToInt32(roleId);
            if (topic_creatorId.Equals(userId) || int_roleId == 1)
            {
                //deleteCommand = "&nbsp;<button id='remove_button'type='button' onmousedown=\"OpenPopup('RemoveTopic.aspx?id=" + topicId + "')\">Remove Topic</button>";
                deleteCommand = "&nbsp;<button id='remove_button' type='button' onclick=\"removeTopic('" + topicId + "', '" + topic_creatorId + "')\">Remove Topic </button>";
            }
            if (int_roleId == 1)
            {
                //terminateCommand = "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<button id='terminate_button'type='button' onmousedown=\"OpenPopup('TerminateTopic.aspx?id=" + topicId + "')\">Terminate Topic</button>";
                terminateCommand = "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<button id='terminate_button' type='button' onclick=\"terminateTopic('" + topicId + "', '" + topic_creatorId + "')\">Terminate Topic </button>";
            }
            if (isTerminated)
                terminateCommand = "";
            if (isDeleted)
            {
                deleteCommand = "";
                terminateCommand = "";
            }
            string header = "<div id=\"header\">" +
            "<div id=\"messageHead\">" +
            "&nbsp;\"" + topic_title + "\" " +
            "Created by <a href=\"Profile.aspx?id="+ topic_creatorId + "\">" + creator + " </a>" +
            "as a " + topic_type.ToLower() + " topic on " + getTimeFormat(topic_time) + "</div>" +
            "<div id=\"messageDescription\"><br/>" + topic_description + "<br /><br/>" +
			imagesHTML + "</div>" +
            deleteCommand+
            terminateCommand+
            "</div>";
			return header;
		}
        static protected bool checkTerminated(string topicId)
        {
            bool terminated = false;
            Configuration config = new Configuration();
            SqlConnection connect = new SqlConnection(config.getConnectionString());
            connect.Open();
            SqlCommand cmd = connect.CreateCommand();
            cmd.CommandText = "select topic_isTerminated from topics where topicId = '"+topicId+"' ";
            int topic_isTerminated = Convert.ToInt32(cmd.ExecuteScalar());
            if (topic_isTerminated == 1)
                terminated = true;
            cmd.CommandText = "select topic_type from topics where topicId = '" + topicId + "' ";
            string topic_type = cmd.ExecuteScalar().ToString();
            if (topic_type.Equals("Dissemination"))
                terminated = true;
            connect.Close();
            return terminated;
        }
        static protected bool checkDeleted(string topicId)
        {
            bool deleted = false;
            Configuration config = new Configuration();
            SqlConnection connect = new SqlConnection(config.getConnectionString());
            connect.Open();
            SqlCommand cmd = connect.CreateCommand();
            cmd.CommandText = "select topic_isDeleted from topics where topicId = '" + topicId + "' ";
            int topic_isDeleted = Convert.ToInt32(cmd.ExecuteScalar());
            if (topic_isDeleted == 1)
                deleted = true;
            connect.Close();
            return deleted;
        }
        public static string postMessage(int i, string creator_name, string entry_time, string entry_text, string imagesHtml, 
			string entry_creatorId, string topic_creatorId, string userId, string entryId, string roleId, string topicId)
		{
			string deleteCommand = "";
            //Check if the user viewing the message is the creator, or if the current user viewing is an admin:
            int int_roleId = Convert.ToInt32(roleId);
            if (entry_creatorId.Equals(userId) || int_roleId == 1)
            //deleteCommand = "&nbsp;<asp:Button ID="btnRemove" runat="server" Text="Remove Entry " + i" OnClick="btnRemove_Click" onmousedown=\"OpenPopup('RemoveEntry.aspx?id=" + entryId + "') onchange='__doPostBack('<%=panelAttachments.UniqueID %>',''); return false;\"> + "</button><br/>";
            {
                deleteCommand = "&nbsp;<button id='remove_button' type='button' onmousedown=\"OpenPopup('RemoveEntry.aspx?id=" + entryId + "')\">Remove Entry " + i + "</button><br/>";
                deleteCommand = "&nbsp;<button id='remove_button' type='button' onclick=\"removeMessage('" + entryId + "', "+i+", '"+ entry_creatorId + "', '"+topicId+"')\">Remove Entry " + i + "</button><br/>";
            }

			string background_color = "";			
            if(i % 8 == 2)
                background_color = "style = \"background: rgb(255, 255, 255);background: rgba(203, 203, 152, 0.6);\""; //Custom color
            else if(i % 8 == 3)
                background_color = "style = \"background: rgb(255, 255, 255);background: rgba(118, 201, 201, 0.6);\""; //Light turquoise
            else if (i % 8 == 4)
                background_color = "style = \"background: rgb(255, 255, 255);background: rgba(118, 134, 201, 0.6);\""; //Light blue
            else if (i % 8 == 5)
                background_color = "style = \"background: rgb(255, 255, 255);background: rgba(170, 118, 201, 0.6);\""; //Light purple
            else if (i % 8 == 6)
                background_color = "style = \"background: rgb(255, 255, 255);background: rgba(201, 118, 118, 0.6);\""; //Light red
            else if (i % 8 == 7)
                background_color = "style = \"background: rgb(255, 255, 255);background: rgba(118, 201, 140, 0.6);\""; //Light green
            else if (i % 8 == 0)
                background_color = "style = \"background: rgb(255, 255, 255);background: rgba(209, 207, 131, 0.6);\""; //Light yellow
            string message = "<div id=\"message\" " + background_color + " ><div id=\"messageHead\">&nbsp;Message #" + i + " - added by " + 
                "<a href=\"Profile.aspx?id=" + entry_creatorId + "\"> "    +creator_name + "</a>"+ 
                " on " + getTimeFormat(entry_time) + "</div> " +
                    "<div id=\"messageDescription\"><p><br/>" + entry_text + "</p><br /> " +
						imagesHtml +
                        "</div> " +
					deleteCommand +
                    "</div><br />";
			return message;
		}
		public static string getTimeFormat(string originalTime)
		{
			string format = "";
            if (!string.IsNullOrWhiteSpace(originalTime))
            {
                DateTime dateTime = Convert.ToDateTime(originalTime);
                //DateTime dateTime = DateTime.ParseExact(originalTime, "mmddyyyyHH:mm:ss", CultureInfo.CurrentCulture);
                string month = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(dateTime.Month);
                string day = dateTime.Day.ToString("00");
                string year = dateTime.Year.ToString("0000");
                string hours = dateTime.Hour.ToString("00");
                //string minutes = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(dateTime.Minute);
                string minutes = dateTime.Minute.ToString("00");
                string seconds = dateTime.Second.ToString("00");
                //string milliseconds = dateTime.Millisecond.ToString();
                //Get AM/PM:
                string dayOrNight = "AM";
                int int_hours = Convert.ToInt32(hours);
                if (int_hours > 12)
                    dayOrNight = "PM";
                //Change the hour format to 12-hour-format:
                if (int_hours == 0)
                    hours = "12";
                else if (int_hours > 12)
                    hours = int_hours - 12 + "";
                format = month + " " + day + ", " + year + " " + hours + ":" + minutes + ":" + seconds + " " + dayOrNight;
            }
			return format;
		}
        public static string getBirthdateFormat(string originalTime)
        {
            string format = "";
            if (!string.IsNullOrWhiteSpace(originalTime))
            {
                DateTime dateTime = Convert.ToDateTime(originalTime);
                //DateTime dateTime = DateTime.ParseExact(originalTime, "mmddyyyyHH:mm:ss", CultureInfo.CurrentCulture);
                string month = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(dateTime.Month);
                string day = dateTime.Day.ToString("00");
                string year = dateTime.Year.ToString("0000");
                format = month + " " + day + ", " + year;
            }
            return format;
        }
        public static string phoneFormat(string phone_input)
        {
            //The input will arrive as XXXXXXXXXX and
            //the format output must be (XXX) XXX-XXXX with the space!
            string phone_output = "";
            string area_code = phone_input.Substring(0, 3);
            string three_digits = phone_input.Substring(3, 3);
            string four_digits = phone_input.Substring(6, 4);
            phone_output = "(" + area_code + ") " + three_digits + "-" + four_digits;
            return phone_output;
        }
	}
}