using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace NephroNet.Accounts.Patient
{
    public class CheckPatientSession
    {
        string username = "", roleId = "", token = "";
        Configuration config = new Configuration();
        public Boolean sessionIsCorrect(string temp_username, string temp_roleId, string temp_token,
            string currentPage, string previousPage, DateTime currentTime, string userIP)
        {
            username = temp_username;
            roleId = temp_roleId;
            token = temp_token;

            Boolean correctSession = true;
            Boolean isEmptySession = checkIfSessionIsEmpty();
            if (isEmptySession)
                correctSession = false;
            Boolean correctSessionValues = checkSeesionValues();
            if (correctSessionValues == false)
            {
                correctSession = false;
            }
            Logs log = new Logs(temp_username, temp_roleId, temp_token, currentPage, previousPage, currentTime, userIP);
            return correctSession;
        }
        public int countTotalAlerts()
        {
            SqlConnection connect = new SqlConnection(config.getConnectionString());
            connect.Open();
            SqlCommand cmd = connect.CreateCommand();
            int count = 0;
            connect.Close();
            return count;
        }
        protected Boolean checkSeesionValues()
        {
            SqlConnection connect = new SqlConnection(config.getConnectionString());
            Boolean correct = true;
            connect.Open();
            SqlCommand cmd = connect.CreateCommand();
            cmd.CommandText = "select count(*) from logins where login_username like '" + username + "' and roleId = '" + roleId + "' ";
            int countValues = Convert.ToInt32(cmd.ExecuteScalar());
            if (countValues < 1)//session has wrong values for any role.
                correct = false;
            cmd.CommandText = "select count(*) from logins where login_token like '" + token + "' and roleId = '" + roleId + "' ";
            int countTokenValues = Convert.ToInt32(cmd.ExecuteScalar());
            if (countTokenValues < 1)//session has wrong values for any role with the recieved token.
                correct = false;
            connect.Close();
            return correct;
        }
        protected Boolean checkIfSessionIsEmpty()
        {
            Boolean itIsEmpty = false;
            if (string.IsNullOrWhiteSpace(username) || (!roleId.Equals("3")))//if no session values for either username or roleId, set to false.
            {
                itIsEmpty = true;
            }
            return itIsEmpty;
        }
    }
}