using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace NephroNet.Accounts
{
    public class ShortProfile
    {
        static string conn = "";
        SqlConnection connect = new SqlConnection(conn);
        public ShortProfile(string in_profileId, string in_current_userId)
        {
            Configuration config = new Configuration();
            conn = config.getConnectionString();
            connect = new SqlConnection(conn);
            getShortProfile(in_profileId, in_current_userId);
        }
        protected void getShortProfile(string in_profileId, string in_current_userId)
        {
            connect.Open();
            SqlCommand cmd = connect.CreateCommand();
            cmd.CommandText = "select shortProfileId from [ShortProfiles] where userId = '" + in_profileId + "' ";
            string profile_Id = cmd.ExecuteScalar().ToString();
            cmd.CommandText = "select shortProfile_firstname from [ShortProfiles] where userId = '" + in_profileId + "' ";
            string profile_firstname = cmd.ExecuteScalar().ToString();
            cmd.CommandText = "select shortProfile_lastname from [ShortProfiles] where userId = '" + in_profileId + "' ";
            string profile_lastname = cmd.ExecuteScalar().ToString();
            cmd.CommandText = "select shortProfile_race from [ShortProfiles] where userId = '" + in_profileId + "' ";
            string profile_race = cmd.ExecuteScalar().ToString();
            cmd.CommandText = "select shortProfile_gender from [ShortProfiles] where userId = '" + in_profileId + "' ";
            string profile_gender = cmd.ExecuteScalar().ToString();
            cmd.CommandText = "select shortProfile_birthdate from [ShortProfiles] where userId = '" + in_profileId + "' ";
            string profile_birthdate = cmd.ExecuteScalar().ToString();
            cmd.CommandText = "select shortProfile_nationality from [ShortProfiles] where userId = '" + in_profileId + "' ";
            string profile_nationality = cmd.ExecuteScalar().ToString();
            cmd.CommandText = "select shortProfile_roleId from [ShortProfiles] where userId = '" + in_profileId + "' ";
            int profile_roleId = Convert.ToInt32(cmd.ExecuteScalar());
            cmd.CommandText = "select shortProfile_isPrivate from [ShortProfiles] where userId = '" + in_profileId + "' ";
            int profile_isPrivate = Convert.ToInt32(cmd.ExecuteScalar());
            //Count the number of blocked users:
            cmd.CommandText = "select count(*) from [BlockedUsers] where shortProfileId = '" + profile_Id + "' ";
            int totalBlockedUsers = Convert.ToInt32(cmd.ExecuteScalar());
            ArrayList blockedUsers = new ArrayList();
            for(int i = 1; i <= totalBlockedUsers; i++)
            {
                cmd.CommandText = "select [userId] from(SELECT rowNum = ROW_NUMBER() OVER(ORDER BY blockedUserId ASC), * FROM [BlockedUsers] where shortProfileId = '" + profile_Id + "' ) as t where rowNum = '" + i + "'";
                string blocked_userId = cmd.ExecuteScalar().ToString();
                if (!string.IsNullOrWhiteSpace(blocked_userId))
                {
                    cmd.CommandText = "select (user_firstname + ' ' + user_lastname) from Users where userId = '"+blocked_userId+"'  ";
                    string blocked_user = cmd.ExecuteScalar().ToString();
                    blockedUsers.Add(blocked_user);
                }
            }
            //Count the number of current health conditions:
            cmd.CommandText = "select count(*) from [CurrentHealthConditions] where shortProfileId = '" + profile_Id + "' ";
            int totalCurrentHealthConditions = Convert.ToInt32(cmd.ExecuteScalar());
            ArrayList currentHealthConditions = new ArrayList();
            for (int i = 1; i <= totalCurrentHealthConditions; i++)
            {
                cmd.CommandText = "select [currentHealthCondition_name] from(SELECT rowNum = ROW_NUMBER() OVER(ORDER BY currentHealthConditionId ASC), * FROM [CurrentHealthConditions] where shortProfileId = '" + profile_Id + "' ) as t where rowNum = '" + i + "'";
                string currentHealthCondition = cmd.ExecuteScalar().ToString();
                currentHealthConditions.Add(currentHealthCondition);
            }
            //Count the number of current treatments:
            cmd.CommandText = "select count(*) from [CurrentTreatments] where shortProfileId = '" + profile_Id + "' ";
            int totalCurrentTreatments = Convert.ToInt32(cmd.ExecuteScalar());
            ArrayList currentTreatments = new ArrayList();
            for (int i = 1; i <= totalCurrentTreatments; i++)
            {
                cmd.CommandText = "select [currentTreatment_name] from(SELECT rowNum = ROW_NUMBER() OVER(ORDER BY currentTreatmentId ASC), * FROM [CurrentTreatments] where shortProfileId = '" + profile_Id + "' ) as t where rowNum = '" + i + "'";
                string currentTreatment = cmd.ExecuteScalar().ToString();
                currentTreatments.Add(currentTreatment);
            }
            connect.Close();
            //Set the name according to the profile name:
            Id = profile_Id;
            FirstName = profile_firstname;
            LastName = profile_lastname;
            Race = profile_race;
            Gender = profile_gender;
            Birthdate = profile_birthdate;
            Nationality = profile_nationality;
            RoleId = profile_roleId;
            IsPrivate = profile_isPrivate;
            BlockedUsers = blockedUsers;
            CurrentHealthConditions = currentHealthConditions;
            CurrentTreatments = currentTreatments;
            if (profile_roleId == 1)
                RoleName = "Admin";
            else if (profile_roleId == 2)
                RoleName = "Physician";
            else
                RoleName = "Patient";
        }
        protected void setShortProfile(string shortProfileId, string firstname, string lastname, string race, string gender, string birthdate, string nationality,
            int roleId, int profile_isPrivate, string roleName, ArrayList blockedUsers, ArrayList currentHealthConditions, ArrayList currentTreatments)
        {
            connect.Open();
            SqlCommand cmd = connect.CreateCommand();
            //Update Short Profile table:
            cmd.CommandText = "update ShortProfiles set shortProfile_firstname = '"+firstname+"', shortProfile_lastname = '"+lastname+"', " +
                "shortProfile_race = '" + race+ "', shortProfile_gender = '"+gender+"', shortProfile_birthdate = '"+birthdate+"', " +
                "shortProfile_nationality = '"+nationality+"' , shortProfile_isPrivate = '"+profile_isPrivate+"' " +
                "where shortProfileId = '" + shortProfileId + "' ";
            cmd.ExecuteScalar();
            //Update the Blocked Users table:
            //To really update all the records, first we remove everything related to that shortProfile ID:
            cmd.CommandText = "delete from BlockedUsers where shortProfileId = '" + shortProfileId + "' ";
            cmd.ExecuteScalar();
            //Now, we insert the new records by looping through each element in the Array List:
            for (int i = 0; i < blockedUsers.Count; i++)
            {
                string temp_userId = blockedUsers[i].ToString();
                cmd.CommandText = "insert into BlockedUsers (shortProfileId, userId) values ('" + shortProfileId + "', '" + temp_userId + "')";
                cmd.ExecuteScalar();
            }
            //Update the Current Health Conditions table:
            //To really update all the records, first we remove everything related to that shortProfile ID:
            cmd.CommandText = "delete from CurrentHealthConditions where shortProfileId = '" + shortProfileId + "' ";
            cmd.ExecuteScalar();
            //Now, we insert the new records by looping through each element in the Array List:
            for (int i = 0; i < currentHealthConditions.Count; i++)
            {
                string temp_currentHealthCondition = currentHealthConditions[i].ToString();
                cmd.CommandText = "insert into CurrentHealthConditions (shortProfileId, currentHealthCondition_name) values ('" + shortProfileId + "', '" + temp_currentHealthCondition + "')";
                cmd.ExecuteScalar();
            }
            //Update the Current Treatments table:
            //To really update all the records, first we remove everything related to that shortProfile ID:
            cmd.CommandText = "delete from CurrentTreatments where shortProfileId = '" + shortProfileId + "' ";
            cmd.ExecuteScalar();
            //Now, we insert the new records by looping through each element in the Array List:
            for (int i = 0; i < currentTreatments.Count; i++)
            {
                string temp_currentTreatment = currentTreatments[i].ToString();
                cmd.CommandText = "insert into CurrentTreatments (shortProfileId, currentTreatment_name) values ('" + shortProfileId + "', '" + temp_currentTreatment + "')";
                cmd.ExecuteScalar();
            }
            connect.Close();
            Id = shortProfileId;
            FirstName = firstname;
            LastName = lastname;
            Race = race;
            Gender = gender;
            Birthdate = birthdate;
            Nationality = nationality;
            RoleId = roleId;
            IsPrivate = profile_isPrivate;
            RoleName = roleName;
            BlockedUsers = blockedUsers;
            CurrentHealthConditions = currentHealthConditions;
            CurrentTreatments = currentTreatments;
        }
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Race { get; set; }
        public string Gender { get; set; }
        public string Birthdate { get; set; }
        public string Nationality { get; set; }
        public int RoleId { get; set; }
        public int IsPrivate { get; set; }
        public string RoleName { get; set; }
        public ArrayList BlockedUsers { get; set; }
        public ArrayList CurrentHealthConditions { get; set; }
        public ArrayList CurrentTreatments { get; set; }
    }
}