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
                cmd.CommandText = "select physicianCompleteProfileId from PhysicianCompleteProfiles where userId = '" + in_profileId + "' ";
                ID = cmd.ExecuteScalar().ToString();
                cmd.CommandText = "select [physicianCompleteProfile_isPrivate] from PhysicianCompleteProfiles where userId = '" + in_profileId + "' ";
                Private = Convert.ToInt32(cmd.ExecuteScalar());
                if (in_current_userId == in_profileId || Private == 0)//If the user trying access the profile is the owner
                {

                    cmd.CommandText = "select [physicianCompleteProfile_Dialysis] from [PhysicianCompleteProfiles] where userId = '" + in_profileId + "' ";
                    Dialysis = cmd.ExecuteScalar().ToString();
                    cmd.CommandText = "select [physicianCompleteProfile_homeDialysis] from [PhysicianCompleteProfiles] where userId = '" + in_profileId + "' ";
                    HomeDialysis = cmd.ExecuteScalar().ToString();
                    cmd.CommandText = "select [physicianCompleteProfile_transplantation] from [PhysicianCompleteProfiles] where userId = '" + in_profileId + "' ";
                    Transplantation = cmd.ExecuteScalar().ToString();
                    cmd.CommandText = "select [physicianCompleteProfile_hypertension] from [PhysicianCompleteProfiles] where userId = '" + in_profileId + "' ";
                    Hypertension = cmd.ExecuteScalar().ToString();
                    cmd.CommandText = "select [physicianCompleteProfile_GN] from [PhysicianCompleteProfiles] where userId = '" + in_profileId + "' ";
                    GN = cmd.ExecuteScalar().ToString();
                    cmd.CommandText = "select [physicianCompleteProfile_physicianId] from [PhysicianCompleteProfiles] where userId = '" + in_profileId + "' ";
                    PhysicianID = cmd.ExecuteScalar().ToString();
                    cmd.CommandText = "select count(*) from PhysicianExperiences where physicianCompleteProfileId = '" + ID + "' ";
                    int experiencesCount = Convert.ToInt32(cmd.ExecuteScalar());
                    if(experiencesCount>0)
                        Experience = new List<string[]>();
                    for (int i=1; i<= experiencesCount; i++)
                    {
                        cmd.CommandText = "select physicianExperienceId from(SELECT rowNum = ROW_NUMBER() OVER(ORDER BY physicianExperienceId ASC), * FROM PhysicianExperiences where physicianCompleteProfileId = '" + ID + "') as t where rowNum = '" + i + "'";
                        string experienceId = cmd.ExecuteScalar().ToString();
                        cmd.CommandText = "select physicianExperience_hospitalName from PhysicianExperiences where physicianExperienceId = '"+ experienceId + "' ";
                        string hospitalName = cmd.ExecuteScalar().ToString();
                        cmd.CommandText = "select physicianExperience_hospitalAddress from PhysicianExperiences where physicianExperienceId = '" + experienceId + "' ";
                        string hospitalAddress = cmd.ExecuteScalar().ToString();
                        cmd.CommandText = "select physicianExperience_fromYear from PhysicianExperiences where physicianExperienceId = '" + experienceId + "' ";
                        string fromYear = cmd.ExecuteScalar().ToString();
                        cmd.CommandText = "select physicianExperience_toYear from PhysicianExperiences where physicianExperienceId = '" + experienceId + "' ";
                        string toYear = cmd.ExecuteScalar().ToString();
                        string[] result = new string[] { hospitalName, hospitalAddress, fromYear, toYear };
                        Experience.Add(result);
                    }

                }
                else //if (Private == 1) if the account is set to private and the user accessing it is not the owner:
                {
                    //If the account is private or the user trying to access is not the owner, show nothing:
                    Dialysis = "";
                    HomeDialysis = "";
                    Transplantation = "";
                    Hypertension = "";
                    GN = "";
                    PhysicianID = "";
                    if(Experience != null)
                        Experience.Clear();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occured while trying to retrieve the Physician Complete Profile ID (" + ID + "): ", e);
                //If there is an error, set everything to blank to avoid null values:
                Dialysis = "";
                HomeDialysis = "";
                Transplantation = "";
                Hypertension = "";
                GN = "";
                PhysicianID = "";
                if (Experience != null)
                    Experience.Clear();
            }

            connect.Close();
        }
        public string ID { get; set; }
        public int Private { get; set; }
        public string Dialysis { get; set; }
        public string HomeDialysis { get; set; }
        public string Transplantation { get; set; }
        public string Hypertension { get; set; }
        public string GN { get; set; }
        public string PhysicianID { get; set; }
        public List<string[]> Experience { get; set; }
    }
}