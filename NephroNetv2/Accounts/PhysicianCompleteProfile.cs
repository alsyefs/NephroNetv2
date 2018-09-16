using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace NephroNet.Accounts
{
    public class PhysicianCompleteProfile
    {
        static string conn = "";
        SqlConnection connect = new SqlConnection(conn);
        public PhysicianCompleteProfile(string in_current_userId, string in_profileId)
        {
            Configuration config = new Configuration();
            conn = config.getConnectionString();
            connect = new SqlConnection(conn);
            connect.Open();
            SqlCommand cmd = connect.CreateCommand();
            cmd.CommandText = "select loginId from users where userId = '" + in_current_userId + "' ";
            int current_roleId = Convert.ToInt32(cmd.ExecuteScalar());
            connect.Close();
            if (current_roleId == 1) // 1 = Admin
                setCompleteProfile(in_current_userId);
            else if (in_current_userId.Equals(in_profileId))//if the user trying to view his/her own complete profile
                setCompleteProfile(in_current_userId);
        }
        protected void setCompleteProfile(string in_current_userId)
        {

        }
    }
}