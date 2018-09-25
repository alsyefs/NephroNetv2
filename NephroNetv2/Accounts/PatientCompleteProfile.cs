using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace NephroNet.Accounts
{
    public class PatientCompleteProfile
    {
        static string conn = "";
        SqlConnection connect = new SqlConnection(conn);
        public PatientCompleteProfile(string in_current_userId, string in_profileId)
        {
            int int_in_current_userId = Convert.ToInt32(in_current_userId);
            int int_in_profileId = Convert.ToInt32(in_profileId);
            Configuration config = new Configuration();
            conn = config.getConnectionString();
            connect = new SqlConnection(conn);
            connect.Open();
            SqlCommand cmd = connect.CreateCommand();
            cmd.CommandText = "select loginId from users where userId = '" + in_current_userId + "' ";
            int current_loginId = Convert.ToInt32(cmd.ExecuteScalar());
            cmd.CommandText = "select roleId from Logins where loginId = '" + current_loginId + "' ";
            int current_roleId = Convert.ToInt32(cmd.ExecuteScalar());
            connect.Close();
            if (current_roleId == 1) // 1 = Admin
                getCompleteProfile(int_in_current_userId, int_in_profileId);
            else if (int_in_current_userId == int_in_profileId)//if the user trying to view his/her own complete profile
                getCompleteProfile(int_in_current_userId, int_in_profileId);
        }
        protected void getCompleteProfile(int in_current_userId, int in_profileId)
        {
            connect.Open();
            SqlCommand cmd = connect.CreateCommand();
            try
            {
                cmd.CommandText = "select patientCompleteProfileId from PatientCompleteProfiles where userId = '" + in_profileId + "' ";
                ID = cmd.ExecuteScalar().ToString();
                cmd.CommandText = "select patientCompleteProfile_isPrivate from PatientCompleteProfiles where userId = '" + in_profileId + "' ";
                Private = Convert.ToInt32(cmd.ExecuteScalar());
                if (in_current_userId == in_profileId || Private == 0)//If the user trying access the profile is the owner
                {

                    cmd.CommandText = "select patientCompleteProfile_HighBloodPressure from PatientCompleteProfiles where userId = '" + in_profileId + "' ";
                    HighBloodPressure = cmd.ExecuteScalar().ToString();
                    cmd.CommandText = "select patientCompleteProfile_Diabetes from PatientCompleteProfiles where userId = '" + in_profileId + "' ";
                    Diabetes = cmd.ExecuteScalar().ToString();
                    cmd.CommandText = "select patientCompleteProfile_KidneyTransplant from PatientCompleteProfiles where userId = '" + in_profileId + "' ";
                    KidneyTransplant = cmd.ExecuteScalar().ToString();
                    cmd.CommandText = "select patientCompleteProfile_Dialysis from PatientCompleteProfiles where userId = '" + in_profileId + "' ";
                    Dialysis = cmd.ExecuteScalar().ToString();
                    cmd.CommandText = "select patientCompleteProfile_KidneyStone from PatientCompleteProfiles where userId = '" + in_profileId + "' ";
                    KidneyStone = cmd.ExecuteScalar().ToString();
                    cmd.CommandText = "select patientCompleteProfile_KidneyInfection from PatientCompleteProfiles where userId = '" + in_profileId + "' ";
                    KidneyInfection = cmd.ExecuteScalar().ToString();
                    cmd.CommandText = "select patientCompleteProfile_HeartFailure from PatientCompleteProfiles where userId = '" + in_profileId + "' ";
                    HeartFailure = cmd.ExecuteScalar().ToString();
                    cmd.CommandText = "select patientCompleteProfile_Cancer from PatientCompleteProfiles where userId = '" + in_profileId + "' ";
                    Cancer = cmd.ExecuteScalar().ToString();
                    cmd.CommandText = "select patientCompleteProfile_Cancer from PatientCompleteProfiles where userId = '" + in_profileId + "' ";
                    Cancer = cmd.ExecuteScalar().ToString();
                    cmd.CommandText = "select patientCompleteProfile_Comments from PatientCompleteProfiles where userId = '" + in_profileId + "' ";
                    Comments = cmd.ExecuteScalar().ToString();
                    cmd.CommandText = "select patientCompleteProfile_PatientID from PatientCompleteProfiles where userId = '" + in_profileId + "' ";
                    PatientID = cmd.ExecuteScalar().ToString();
                }
                else //if (Private == 1) if the account is set to private and the user accessing it is not the owner:
                {
                    //If the account is private or the user trying to access is not the owner, show nothing:
                    HighBloodPressure = "";
                    Diabetes = "";
                    KidneyTransplant = "";
                    Dialysis = "";
                    KidneyStone = "";
                    KidneyInfection = "";
                    HeartFailure = "";
                    Cancer = "";
                    Comments = "";
                    PatientID = "";
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occured while trying to retrieve the Patient Complete Profile ID (" + ID + "): ", e);
                //If there is an error, set everything to blank to avoid null values:
                HighBloodPressure = "";
                Diabetes = "";
                KidneyTransplant = "";
                Dialysis = "";
                KidneyStone = "";
                KidneyInfection = "";
                HeartFailure = "";
                Cancer = "";
                Comments = "";
                PatientID = "";
            }
            
            connect.Close();
        }
        public string ID { get; set; }
        public int Private { get; set; }
        public string HighBloodPressure { get; set; }
        public string Diabetes { get; set; }
        public string KidneyTransplant { get; set; }
        public string Dialysis { get; set; }
        public string KidneyStone { get; set; }
        public string KidneyInfection { get; set; }
        public string HeartFailure { get; set; }
        public string Cancer { get; set; }
        public string Comments { get; set; }
        public string PatientID { get; set; }

    }
}