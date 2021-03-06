﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NephroNet
{
    public partial class About : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //Delete the below after the first run:
            //createNewDummyDataInHospitalDB(); //10K records
            //createRandomGendersDobs(); //10K records
            //createRandomNames(); //10K records
        }
        protected void createRandomNames()
        {
            Configuration config = new Configuration();
            SqlConnection connect = new SqlConnection(config.getConnectionString());
            SqlCommand cmd = connect.CreateCommand();
            string name = "";
            connect.Open();
            try
            {
                string path = System.Web.HttpContext.Current.Request.PhysicalApplicationPath + @"\Content\random_names.txt";
                var lines = File.ReadAllLines(path);
                var r = new Random();
                var randomLineNumber = 0;
                var line="";
                List<string> names = new List<string>();
                string firstname = "";
                string lastname = "";
                for (int i = 0; i < 10000; i++)
                {
                    //Get a random name for physician:
                    randomLineNumber = r.Next(0, lines.Length - 1);
                    line = lines[randomLineNumber];
                    name = line.Replace("\t", " ").Replace("\n", "").Replace("'", "''");
                    names = name.Split(' ').ToList<string>();
                    firstname = names[0].ToString().TrimStart(' ').TrimEnd(' ');
                    lastname = names[1].ToString().TrimStart(' ').TrimEnd(' ');
                    //Insert into Physicians:
                    cmd.CommandText = "update DB2_PhysicianShortProfiles set db2_physicianShortProfile_firstname = '" + firstname + "', " +
                        "db2_physicianShortProfile_lastname = '" + lastname + "' where db2_physicianShortProfileId = '" + (i + 1) + "' ";
                    cmd.ExecuteScalar();
                    //Get a random name for patient:
                    randomLineNumber = r.Next(0, lines.Length - 1);
                    line = lines[randomLineNumber];
                    name = line.Replace("\t", " ").Replace("\n", "").Replace("'", "''");
                    names = name.Split(' ').ToList<string>();
                    firstname = names[0].ToString().TrimStart(' ').TrimEnd(' ');
                    lastname = names[1].ToString().TrimStart(' ').TrimEnd(' ');
                    //Insert into Patients:
                    cmd.CommandText = "update DB2_PatientShortProfiles set db2_patientShortProfile_firstname = '" + firstname + "', " +
                        "db2_patientShortProfile_lastname = '" + lastname + "' where db2_patientShortProfileId = '" + (i + 1) + "' " +
                        "and db2_patientShortProfile_firstname is null and db2_patientShortProfile_lastname is null";
                    cmd.ExecuteScalar();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }
            finally
            {
                Console.WriteLine("Executing finally block.");
            }
            connect.Close();
        }
        protected DateTime RandomDay(Random r_dob)
        {
            DateTime start = new DateTime(1900, 1, 1);
            int range = (DateTime.Today - start).Days;
            return start.AddDays(r_dob.Next(range));
        }
        protected void createRandomGendersDobs()
        {
            Configuration config = new Configuration();
            SqlConnection connect = new SqlConnection(config.getConnectionString());
            SqlCommand cmd = connect.CreateCommand();
            Random r_gender = new Random();
            Random r_dob = new Random();
            string[] genders = new string[] { "Male", "Female" };
            connect.Open();
            for (int i=0; i<10000; i++)
            {
                DateTime temp_date = RandomDay(r_dob);
                int temp_gender = r_gender.Next(0, 2);
                cmd.CommandText = "update DB2_PatientShortProfiles set db2_patientShortProfile_gender = '" + genders[temp_gender] + "', " +
                    "db2_patientShortProfile_dateOfBirth = '" + temp_date + "' where db2_patientShortProfileId = '" + (i + 1) + "' ";
                cmd.ExecuteScalar();
            }
            connect.Close();
        }
        protected void createNewDummyDataInHospitalDB()
        {
            Configuration config = new Configuration();
            SqlConnection connect = new SqlConnection(config.getConnectionString());
            SqlCommand cmd = connect.CreateCommand();
            string[] hospitalNames = new string[10];
            string[] hospitalAddresses = new string[10];
            string[] hospitalWebsites = new string[10];
            string[] hospitalPhones = new string[10];
            string[] hospitalSpecialities = new string[10];
            //Hospital 1:
            hospitalNames[0] = "MOUNDVIEW MEMORIAL HOSPITAL AND CLINICS";
            hospitalAddresses[0] = "402 W LAKE ST FRIENDSHIP, WI 53934";
            hospitalWebsites[0] = "moundview.org";
            hospitalPhones[0] = "6083393331";
            hospitalSpecialities[0] = "Pediatric Nephrology";
            //Hospital 2:
            hospitalNames[1] = "MEMORIAL MEDICAL CENTER";
            hospitalAddresses[1] = "1615 MAPLE LANE ASHLAND, WI 54806";
            hospitalWebsites[1] = "memorialmedical.com";
            hospitalPhones[1] = "7156855500";
            hospitalSpecialities[1] = "Dialysis, Intensive Care (Critical Care), Nephrology (Kidneys)";
            //Hospital 3:
            hospitalNames[2] = "UNIVERSITY OF WI HOSPITALS & CLINICS AUTHORITY";
            hospitalAddresses[2] = "600 HIGHLAND AVENUE MADISON, WI 53792";
            hospitalWebsites[2] = "uwhealth.org";
            hospitalPhones[2] = "6084175336";
            hospitalSpecialities[2] = "Dialysis (Kidneys)";
            //Hospital 4:
            hospitalNames[3] = "DOOR COUNTY MEDICAL CENTER";
            hospitalAddresses[3] = "323 SOUTH 18TH AVENUE STURGEON BAY, WI 54235";
            hospitalWebsites[3] = "dcmedical.org";
            hospitalPhones[3] = "9207435566";
            hospitalSpecialities[3] = "Nephrology (Kidneys)";
            //Hospital 5:
            hospitalNames[4] = "OAKLEAF SURGICAL HOSPITAL";
            hospitalAddresses[4] = "1000 OAKLEAF WAY ALTOONA, WI 54720";
            hospitalWebsites[4] = "oakleafsurgical.com";
            hospitalPhones[4] = "7158318130";
            hospitalSpecialities[4] = "Dialysis, Nephrology (Kidneys)";
            //Hospital 6:
            hospitalNames[5] = "SACRED HEART HOSPITAL";
            hospitalAddresses[5] = "900 W CLAIREMONT AVE EAU CLAIRE, WI 54701";
            hospitalWebsites[5] = "sacredhearteauclaire.org";
            hospitalPhones[5] = "7157174121";
            hospitalSpecialities[5] = "Nephrology (Kidneys)";
            //Hospital 7:
            hospitalNames[6] = "GUNDERSEN LUTHERAN MEDICAL CENTER";
            hospitalAddresses[6] = "1910 SOUTH AVE LA CROSSE, WI 54601";
            hospitalWebsites[6] = "gundersenhealth.org";
            hospitalPhones[6] = "6087827300";
            hospitalSpecialities[6] = "Nephrology (Kidneys)";
            //Hospital 8:
            hospitalNames[7] = "MAYO CLINIC HLTH SYSTEM FRANCISCAN MED CTR";
            hospitalAddresses[7] = "700 WEST AVENUE SOUTH LA CROSSE, WI 54601";
            hospitalWebsites[7] = "mayoclinichealthsystem.org";
            hospitalPhones[7] = "6087850940";
            hospitalSpecialities[7] = "Dialysis, Nephrology (Kidneys)";
            //Hospital 9:
            hospitalNames[8] = "BAY AREA MEDICAL CENTER";
            hospitalAddresses[8] = "3003 UNIVERSITY DR MARINETTE, WI 54143";
            hospitalWebsites[8] = "bamc.org";
            hospitalPhones[8] = "7157356621";
            hospitalSpecialities[8] = "Nephrology (Kidneys)";
            //Hospital 10:
            hospitalNames[9] = "AURORA MEDICAL CENTER";
            hospitalAddresses[9] = "975 PORT WASHINGTON ROAD GRAFTON, WI 53024";
            hospitalWebsites[9] = "auroramed.com";
            hospitalPhones[9] = "2623291000";
            hospitalSpecialities[9] = "Pediatric Nephrology";
            Random r_hospital = new Random();
            Random r_physicianId = new Random();
            Random r_patientId = new Random();
            Random r_phone = new Random();
            long minPhone = 1000000000;
            long maxPhone = 9999999999;
            Random r_gender = new Random();
            Random r_dob = new Random();
            string[] genders = new string[] { "Male", "Female" };
            connect.Open();
            for (int i = 0; i < 10000; i++)
            {
                DateTime temp_date = RandomDay(r_dob);
                int temp_gender = r_gender.Next(0, 2);
                int hospital = r_hospital.Next(0, 10);
                int physicianId = r_physicianId.Next(10000000, 99999999);
                int patientId = r_physicianId.Next(10000000, 99999999);
                long phone = LongRandom(minPhone, maxPhone, new Random());
                cmd.CommandText = "select count(*) from DB2_PhysicianShortProfiles where db2_physicianShortProfile_physicianId like '"+ physicianId + "' ";
                int physicianIdExists = Convert.ToInt32(cmd.ExecuteScalar());
                if (physicianIdExists == 0)
                {
                    //Insert new Physician information:
                    cmd.CommandText = "insert into DB2_PhysicianShortProfiles (db2_physicianShortProfile_hospitalName, db2_physicianShortProfile_hospitalAddress, " +
                      "db2_physicianShortProfile_officePhone, db2_physicianShortProfile_officeEmail, db2_physicianShortProfile_speciality, db2_physicianShortProfile_physicianId) values " +
                      "('" + hospitalNames[hospital] + "', '" + hospitalAddresses[hospital] + "', '" + hospitalPhones[hospital] + "', '" + "help@" + hospitalWebsites[hospital] + "'," +
                      " '" + hospitalSpecialities[hospital] + "', '" + physicianId + "')";
                    cmd.ExecuteScalar();
                }
                cmd.CommandText = "select count(*) from DB2_PatientShortProfiles where db2_patientShortProfile_patientId like '" + patientId + "' ";
                int patientIdExists = Convert.ToInt32(cmd.ExecuteScalar());
                if (patientIdExists == 0)
                {
                    //Insert new Patient information:
                    cmd.CommandText = "insert into DB2_PatientShortProfiles (db2_patientShortProfile_email, db2_patientShortProfile_phone, " +
                      "db2_patientShortProfile_patientId, db2_patientShortProfile_gender, db2_patientShortProfile_dateOfBirth) values " +
                      "('" + patientId + "@" + hospitalWebsites[hospital] + "', '" + phone + "', '" + patientId + "', '"+ genders[temp_gender] + "', '"+temp_date+"')";
                    cmd.ExecuteScalar();
                }
            }
            connect.Close();
        }
        long LongRandom(long min, long max, Random rand)
        {
            byte[] buf = new byte[8];
            rand.NextBytes(buf);
            long longRand = BitConverter.ToInt64(buf, 0);
            return (Math.Abs(longRand % (max - min)) + min);
        }
    }
}