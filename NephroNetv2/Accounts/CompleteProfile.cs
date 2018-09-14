using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace NephroNet.Accounts
{
    public class CompleteProfile
    {
        static string conn = "";
        SqlConnection connect = new SqlConnection(conn);
        public CompleteProfile(string in_current_userId, string in_profileId)
        {
            Configuration config = new Configuration();
            conn = config.getConnectionString();
            connect = new SqlConnection(conn);
            connect.Open();
            SqlCommand cmd = connect.CreateCommand();
            cmd.CommandText = "select loginId from users where userId = '"+in_current_userId+"' ";
            int current_roleId = Convert.ToInt32(cmd.ExecuteScalar());
            connect.Close();
            if(current_roleId == 1) // 1 = Admin
                getCompleteProfile(in_current_userId);
            else if(in_current_userId.Equals(in_profileId))//if the user trying to view his/her own complete profile
                getCompleteProfile(in_current_userId);
        }
        protected void getCompleteProfile(string in_current_userId)
        {
            connect.Open();
            SqlCommand cmd = connect.CreateCommand();
            cmd.CommandText = "select completeProfileId from [CompleteProfiles] where userId = '" + in_current_userId + "' ";
            string profile_Id = cmd.ExecuteScalar().ToString();
            cmd.CommandText = "select completeProfile_onDialysis from [CompleteProfiles] where userId = '" + in_current_userId + "' ";
            string completeProfile_onDialysis = cmd.ExecuteScalar().ToString();
            cmd.CommandText = "select completeProfile_kidneyDisease from [CompleteProfiles] where userId = '" + in_current_userId + "' ";
            string completeProfile_kidneyDisease = cmd.ExecuteScalar().ToString();
            cmd.CommandText = "select completeProfile_issueStartDate from [CompleteProfiles] where userId = '" + in_current_userId + "' ";
            string completeProfile_issueStartDate = cmd.ExecuteScalar().ToString();
            cmd.CommandText = "select completeProfile_bloodType from [CompleteProfiles] where userId = '" + in_current_userId + "' ";
            string completeProfile_bloodType = cmd.ExecuteScalar().ToString();
            cmd.CommandText = "select completeProfile_city from [CompleteProfiles] where userId = '" + in_current_userId + "' ";
            string completeProfile_city = cmd.ExecuteScalar().ToString();
            cmd.CommandText = "select completeProfile_state from [CompleteProfiles] where userId = '" + in_current_userId + "' ";
            string completeProfile_state = cmd.ExecuteScalar().ToString();
            cmd.CommandText = "select completeProfile_zip from [CompleteProfiles] where userId = '" + in_current_userId + "' ";
            string completeProfile_zip = cmd.ExecuteScalar().ToString();
            cmd.CommandText = "select completeProfile_address from [CompleteProfiles] where userId = '" + in_current_userId + "' ";
            string completeProfile_address = cmd.ExecuteScalar().ToString();
            cmd.CommandText = "select completeProfile_country from [CompleteProfiles] where userId = '" + in_current_userId + "' ";
            string completeProfile_country = cmd.ExecuteScalar().ToString();
            //Count Emails:
            cmd.CommandText = "select count(*) from [Emails] where completeProfileId = '" + profile_Id + "' ";
            int totalEmails = Convert.ToInt32(cmd.ExecuteScalar());
            //Count Phones:
            cmd.CommandText = "select count(*) from [PhoneNumbers] where completeProfileId = '" + profile_Id + "' ";
            int totalPhones = Convert.ToInt32(cmd.ExecuteScalar());
            //Count Allergies:
            cmd.CommandText = "select count(*) from [Allergies] where completeProfileId = '" + profile_Id + "' ";
            int totalAllergies = Convert.ToInt32(cmd.ExecuteScalar());
            //Count Major Diagnoses:
            cmd.CommandText = "select count(*) from [MajorDiagnoses] where completeProfileId = '" + profile_Id + "' ";
            int totalMajorDiagnoses = Convert.ToInt32(cmd.ExecuteScalar());
            //Count Past Health Conditions:
            cmd.CommandText = "select count(*) from [PastHealthConditions] where completeProfileId = '" + profile_Id + "' ";
            int totalPastHealthConditions = Convert.ToInt32(cmd.ExecuteScalar());            
            //Count Insurance information:
            cmd.CommandText = "select count(*) from [InsuranceInformation] where completeProfileId = '" + profile_Id + "' ";
            int totalInsurances = Convert.ToInt32(cmd.ExecuteScalar());
            //Count Past Patient IDs:
            cmd.CommandText = "select count(*) from [PastPatientIDs] where completeProfileId = '" + profile_Id + "' ";
            int totalPastPatientIds = Convert.ToInt32(cmd.ExecuteScalar());            
            //Count Emergency Contacts:
            cmd.CommandText = "select count(*) from [EmergencyContacts] where completeProfileId = '" + profile_Id + "' ";
            int totalEmergencyContacts = Convert.ToInt32(cmd.ExecuteScalar());
            List<EmailObject> emails = new List<EmailObject>();
            List<Phone> phones = new List<Phone>();
            ArrayList allergies = new ArrayList();
            ArrayList majorDiagnoses = new ArrayList();
            ArrayList pastHealthConditions = new ArrayList();
            List<Treatment> treatments = new List<Treatment>();
            List<Insurance> insurances = new List<Insurance>();
            List<PastPatientID> pastPatientIds = new List<PastPatientID>();
            List<EmergencyContact> emergencyContacts = new List<EmergencyContact>();
            //get emails:
            for (int i = 1; i <= totalEmails; i++)
            {
                cmd.CommandText = "select [emailId] from(SELECT rowNum = ROW_NUMBER() OVER(ORDER BY emailId ASC), * " +
                    "FROM [Emails] where completeProfileId = '" + profile_Id + "' ) as t where rowNum = '" + i + "'";
                string emailId = cmd.ExecuteScalar().ToString();
                cmd.CommandText = "select [email_emailAddress] from Emails where emailId = '"+emailId+"'  ";
                string emailAddress = cmd.ExecuteScalar().ToString();
                cmd.CommandText = "select [email_isDefault] from Emails where emailId = '" + emailId + "'  ";
                int emailIsDefault = Convert.ToInt32(cmd.ExecuteScalar());
                //string str_emailIsDefault = cmd.ExecuteScalar().ToString();
                //int emailIsDefault = 0;
                //if (!string.IsNullOrWhiteSpace(str_emailIsDefault))
                //    emailIsDefault = Convert.ToInt32(str_emailIsDefault);
                EmailObject email = new EmailObject(emailId, emailAddress, emailIsDefault);
                emails.Add(email);
            }
            //get phones:
            for (int i = 1; i <= totalPhones; i++)
            {
                cmd.CommandText = "select [phonenumberId] from(SELECT rowNum = ROW_NUMBER() OVER(ORDER BY phonenumberId ASC), * " +
                    "FROM [PhoneNumbers] where completeProfileId = '" + profile_Id + "' ) as t where rowNum = '" + i + "'";
                string phoneId = cmd.ExecuteScalar().ToString();
                cmd.CommandText = "select phoneNumber_phone from PhoneNumbers where phoneNumberId = '"+phoneId+"' ";
                string phoneNumber = cmd.ExecuteScalar().ToString();
                cmd.CommandText = "select phoneNumber_isDefault from PhoneNumbers where phoneNumberId = '" + phoneId + "' ";
                int phoneIsDefault = Convert.ToInt32(cmd.ExecuteScalar());
                //string str_phoneIsDefault = cmd.ExecuteScalar().ToString();
                //int phoneIsDefault = 0;
                //if (!string.IsNullOrWhiteSpace(str_phoneIsDefault))
                //    phoneIsDefault = Convert.ToInt32(str_phoneIsDefault);
                Phone phone = new Phone(phoneId, phoneNumber, phoneIsDefault);
                phones.Add(phone);
            }
            //get allergies:
            for (int i = 1; i <= totalAllergies; i++)
            {
                cmd.CommandText = "select [allergy_name] from(SELECT rowNum = ROW_NUMBER() OVER(ORDER BY allergyId ASC), * " +
                    "FROM [Allergies] where completeProfileId = '" + profile_Id + "' ) as t where rowNum = '" + i + "'";
                string allergy = cmd.ExecuteScalar().ToString();
                allergies.Add(allergy);
            }
            //get majorDiagnoses:
            for (int i = 1; i <= totalMajorDiagnoses; i++)
            {
                cmd.CommandText = "select [majorDiagnoses_name] from(SELECT rowNum = ROW_NUMBER() OVER(ORDER BY majorDiagnosesId ASC), * " +
                    "FROM [MajorDiagnoses] where completeProfileId = '" + profile_Id + "' ) as t where rowNum = '" + i + "'";
                string majorDiagnose = cmd.ExecuteScalar().ToString();
                majorDiagnoses.Add(majorDiagnose);
            }
            //get pastHealthConditions:
            for (int i = 1; i <= totalPastHealthConditions; i++)
            {
                cmd.CommandText = "select [pastHealthCondition_name] from(SELECT rowNum = ROW_NUMBER() OVER(ORDER BY pastHealthConditionId ASC), * " +
                    "FROM [PastHealthConditions] where completeProfileId = '" + profile_Id + "' ) as t where rowNum = '" + i + "'";
                string pastHealthCondition = cmd.ExecuteScalar().ToString();
                pastHealthConditions.Add(pastHealthCondition);
            }
            
            //get insurances:
            for (int i = 1; i <= totalInsurances; i++)
            {
                cmd.CommandText = "select [insuranceId] from(SELECT rowNum = ROW_NUMBER() OVER(ORDER BY insuranceId ASC), * " +
                    "FROM [InsuranceInformation] where completeProfileId = '" + profile_Id + "' ) as t where rowNum = '" + i + "'";
                string insuranceId = cmd.ExecuteScalar().ToString();
                cmd.CommandText = "select insurance_memberId from InsuranceInformation where insuranceId = '" + insuranceId + "'  ";
                string insurance_memberId = cmd.ExecuteScalar().ToString();
                cmd.CommandText = "select insurance_groupId from InsuranceInformation where insuranceId = '" + insuranceId + "'  ";
                string insurance_groupId = cmd.ExecuteScalar().ToString();
                cmd.CommandText = "select insurance_companyName from InsuranceInformation where insuranceId = '" + insuranceId + "'  ";
                string insurance_companyName = cmd.ExecuteScalar().ToString();
                cmd.CommandText = "select insurance_phone1 from InsuranceInformation where insuranceId = '" + insuranceId + "'  ";
                string insurance_phone1 = cmd.ExecuteScalar().ToString();
                cmd.CommandText = "select insurance_phone2 from InsuranceInformation where insuranceId = '" + insuranceId + "'  ";
                string insurance_phone2 = cmd.ExecuteScalar().ToString();
                cmd.CommandText = "select insurance_email from InsuranceInformation where insuranceId = '" + insuranceId + "'  ";
                string insurance_email = cmd.ExecuteScalar().ToString();
                cmd.CommandText = "select insurance_city from InsuranceInformation where insuranceId = '" + insuranceId + "'  ";
                string insurance_city = cmd.ExecuteScalar().ToString();
                cmd.CommandText = "select insurance_state from InsuranceInformation where insuranceId = '" + insuranceId + "'  ";
                string insurance_state = cmd.ExecuteScalar().ToString();
                cmd.CommandText = "select insurance_zip from InsuranceInformation where insuranceId = '" + insuranceId + "'  ";
                string insurance_zip = cmd.ExecuteScalar().ToString();
                cmd.CommandText = "select insurance_address from InsuranceInformation where insuranceId = '" + insuranceId + "'  ";
                string insurance_address = cmd.ExecuteScalar().ToString();
                cmd.CommandText = "select insurance_country from InsuranceInformation where insuranceId = '" + insuranceId + "'  ";
                string insurance_country = cmd.ExecuteScalar().ToString();
                Insurance insurance = new Insurance(insuranceId, profile_Id, insurance_memberId, insurance_groupId, insurance_companyName, insurance_phone1, insurance_phone2,
                    insurance_email, insurance_city, insurance_state, insurance_zip, insurance_address, insurance_country);
                insurances.Add(insurance);
            }
            //get pastPatientIds:
            for (int i = 1; i <= totalPastPatientIds; i++)
            {
                cmd.CommandText = "select [pastPatientId] from(SELECT rowNum = ROW_NUMBER() OVER(ORDER BY pastPatientId ASC), * " +
                    "FROM [pastPatientIds] where completeProfileId = '" + profile_Id + "' ) as t where rowNum = '" + i + "'";
                string pastPatientId = cmd.ExecuteScalar().ToString();
                cmd.CommandText = "select pastPatient_medicalRecordNumber from pastPatientIds where pastPatientId = '" + pastPatientId + "'  ";
                string mrn = cmd.ExecuteScalar().ToString();
                PastPatientID obj_pastPatientID = new PastPatientID(pastPatientId, profile_Id, mrn);
                pastPatientIds.Add(obj_pastPatientID);
                //Count Treatments History records:
                cmd.CommandText = "select count(*) from [TreatmentsHistory] where pastPatientId = '" + pastPatientId + "' ";
                int totalTreatments = Convert.ToInt32(cmd.ExecuteScalar());
                //get treatments:
                for (int j = 1; j <= totalTreatments; j++)
                {
                    cmd.CommandText = "select [treatmentId] from(SELECT rowNum = ROW_NUMBER() OVER(ORDER BY treatmentId ASC), * " +
                    "FROM [TreatmentsHistory] where completeProfileId = '" + profile_Id + "' ) as t where rowNum = '" + j + "'";
                    string treatmentId = cmd.ExecuteScalar().ToString();
                    cmd.CommandText = "select treatment_physicianFirstName from TreatmentsHistory where treatmentId = '" + treatmentId + "'  ";
                    string treatment_physicianFirstName = cmd.ExecuteScalar().ToString();
                    cmd.CommandText = "select treatment_physicianLastName from TreatmentsHistory where treatmentId = '" + treatmentId + "'  ";
                    string treatment_physicianLastName = cmd.ExecuteScalar().ToString();
                    cmd.CommandText = "select treatment_startDate from TreatmentsHistory where treatmentId = '" + treatmentId + "'  ";
                    string treatment_startDate = cmd.ExecuteScalar().ToString();
                    cmd.CommandText = "select treatment_hospitalName from TreatmentsHistory where treatmentId = '" + treatmentId + "'  ";
                    string treatment_hospitalName = cmd.ExecuteScalar().ToString();
                    cmd.CommandText = "select treatment_hospitalCity from TreatmentsHistory where treatmentId = '" + treatmentId + "'  ";
                    string treatment_hospitalCity = cmd.ExecuteScalar().ToString();
                    cmd.CommandText = "select treatment_hospitalState from TreatmentsHistory where treatmentId = '" + treatmentId + "'  ";
                    string treatment_hospitalState = cmd.ExecuteScalar().ToString();
                    cmd.CommandText = "select treatment_hospitalZip from TreatmentsHistory where treatmentId = '" + treatmentId + "'  ";
                    string treatment_hospitalZip = cmd.ExecuteScalar().ToString();
                    cmd.CommandText = "select treatment_hospitalAddress from TreatmentsHistory where treatmentId = '" + treatmentId + "'  ";
                    string treatment_hospitalAddress = cmd.ExecuteScalar().ToString();
                    cmd.CommandText = "select treatment_hospitalCountry from TreatmentsHistory where treatmentId = '" + treatmentId + "'  ";
                    string treatment_hospitalCountry = cmd.ExecuteScalar().ToString();
                    Treatment treatment = new Treatment(treatmentId, pastPatientId, treatment_physicianFirstName, treatment_physicianLastName, 
                        Layouts.getBirthdateFormat(treatment_startDate), treatment_hospitalName, treatment_hospitalCity, 
                        treatment_hospitalState, treatment_hospitalZip, treatment_hospitalCountry, treatment_hospitalAddress);
                    treatments.Add(treatment);
                }
            }
            //get emergencyContacts:
            for (int i = 1; i <= totalEmergencyContacts; i++)
            {
                cmd.CommandText = "select [EmergencyContactId] from(SELECT rowNum = ROW_NUMBER() OVER(ORDER BY EmergencyContactId ASC), * " +
                    "FROM [EmergencyContacts] where completeProfileId = '" + profile_Id + "' ) as t where rowNum = '" + i + "'";
                string emergencyContactId = cmd.ExecuteScalar().ToString();
                cmd.CommandText = "select emergencyContact_firstname from EmergencyContacts where EmergencyContactId = '"+ emergencyContactId + "' ";
                string emergencyContact_firstname = cmd.ExecuteScalar().ToString();
                cmd.CommandText = "select emergencyContact_lastname from EmergencyContacts where EmergencyContactId = '" + emergencyContactId + "' ";
                string emergencyContact_lastname = cmd.ExecuteScalar().ToString();
                cmd.CommandText = "select emergencyContact_phone1 from EmergencyContacts where EmergencyContactId = '" + emergencyContactId + "' ";
                string emergencyContact_phone1 = cmd.ExecuteScalar().ToString();
                cmd.CommandText = "select emergencyContact_phone2 from EmergencyContacts where EmergencyContactId = '" + emergencyContactId + "' ";
                string emergencyContact_phone2 = cmd.ExecuteScalar().ToString();
                cmd.CommandText = "select emergencyContact_phone3 from EmergencyContacts where EmergencyContactId = '" + emergencyContactId + "' ";
                string emergencyContact_phone3 = cmd.ExecuteScalar().ToString();
                cmd.CommandText = "select emergencyContact_email from EmergencyContacts where EmergencyContactId = '" + emergencyContactId + "' ";
                string emergencyContact_email = cmd.ExecuteScalar().ToString();
                cmd.CommandText = "select emergencyContact_city from EmergencyContacts where EmergencyContactId = '" + emergencyContactId + "' ";
                string emergencyContact_city = cmd.ExecuteScalar().ToString();
                cmd.CommandText = "select emergencyContact_state from EmergencyContacts where EmergencyContactId = '" + emergencyContactId + "' ";
                string emergencyContact_state = cmd.ExecuteScalar().ToString();
                cmd.CommandText = "select emergencyContact_zip from EmergencyContacts where EmergencyContactId = '" + emergencyContactId + "' ";
                string emergencyContact_zip = cmd.ExecuteScalar().ToString();
                cmd.CommandText = "select emergencyContact_address from EmergencyContacts where EmergencyContactId = '" + emergencyContactId + "' ";
                string emergencyContact_address = cmd.ExecuteScalar().ToString();
                cmd.CommandText = "select emergencyContact_country from EmergencyContacts where EmergencyContactId = '" + emergencyContactId + "' ";
                string emergencyContact_country = cmd.ExecuteScalar().ToString();
                EmergencyContact emergencyContact = new EmergencyContact(emergencyContactId, profile_Id, emergencyContact_firstname, emergencyContact_lastname,
                    emergencyContact_phone1, emergencyContact_phone2, emergencyContact_phone3, emergencyContact_email, emergencyContact_city,
                    emergencyContact_state, emergencyContact_zip, emergencyContact_address, emergencyContact_country);
                majorDiagnoses.Add(emergencyContact);
            }
            connect.Close();
            Id = profile_Id;
            OnDialysis = completeProfile_onDialysis;
            KidneyDisease = completeProfile_kidneyDisease;
            IssueStartDate = Layouts.getBirthdateFormat(IssueStartDate);
            BloodType = completeProfile_bloodType;
            City = completeProfile_city;
            State = completeProfile_state;
            Zip = completeProfile_zip;
            Address = completeProfile_address;
            Emails = emails;
            Phones = phones;
            Allergies = allergies;
            MajorDiagnoses = majorDiagnoses;
            PastHealthConditions = pastHealthConditions;
            Treatments = treatments;
            Insurances = insurances;
            PastPatientIds = pastPatientIds;
            EmergencyContacts = emergencyContacts;
            Country = completeProfile_country;
            
        }
        public string Id { get; set; }
        public string OnDialysis { get; set; }
        public string KidneyDisease { get; set; }
        public string IssueStartDate { get; set; }
        public string BloodType { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Address { get; set; }
        public string Country { get; set; }
        public List<EmailObject> Emails { get; set; }
        public List<Phone> Phones { get; set; }
        public ArrayList Allergies { get; set; }
        public ArrayList MajorDiagnoses { get; set; }
        public ArrayList PastHealthConditions { get; set; }
        public List<Treatment> Treatments { get; set; }
        public List<Insurance> Insurances { get; set; }
        public List<PastPatientID> PastPatientIds { get; set; }
        public List<EmergencyContact> EmergencyContacts { get; set; }
    }
    public class EmailObject
    {
        public EmailObject(string id, string emailAddress, int isDefault)
        {
            ID = id;
            EmailAddress = emailAddress;
            IsDefault = isDefault;
        }
        public string ID { get; set; }
        public string EmailAddress { get; set; }
        public int IsDefault { get; set; }
    }
    public class Phone
    {
        public Phone(string id, string phoneNumber, int isDefault)
        {
            ID = id;
            PhoneNumber = phoneNumber;
            IsDefault = isDefault;
        }
        public string ID { get; set; }
        public string PhoneNumber { get; set; }
        public int IsDefault { get; set; }
    }
    public class EmergencyContact
    {
        public EmergencyContact(string id, string completeProfileId, string firstname, string lastname, string phone1,
            string phone2, string phone3, string email, string city, string state, string zip, string address, string country)
        {
            ID = id;
            CompleteProfileId = completeProfileId;
            Firstname = firstname;
            Lastname = lastname;
            Phone1 = phone1;
            Phone2 = phone2;
            Phone3 = phone3;
            Email = email;
            City = city;
            State = state;
            Zip = zip;
            Address = address;
            Country = country;
        }
        public string ID { get; set; }
        public string CompleteProfileId { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Phone1 { get; set; }
        public string Phone2 { get; set; }
        public string Phone3 { get; set; }
        public string Email { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Address { get; set; }
        public string Country { get; set; }
    }
    public class PastPatientID
    {
        public PastPatientID(string id, string completeProfileId, string mrn)
        {
            ID = id;
            CompleteProfileId = completeProfileId;
            MRN = mrn;
        }
        public string ID { get; set; }
        public string CompleteProfileId { get; set; }
        public string MRN { get; set; }
    }
    public class Treatment
    {
        public Treatment(string id, string pastPatientId, string physicianFirstName, string physicianLastName, string startDate, string hospitalName,
            string hospitalCity, string hospitalState, string hospitalZip, string hospitalCountry, string hospitalAddress)
        {
            ID = id;
            PastPatientId = pastPatientId;
            PhysicianFirstName = physicianFirstName;
            PhysicianLastName = physicianLastName;
            StartDate = startDate;
            HospitalName = hospitalName;
            HospitalCity = hospitalCity;
            HospitalState = hospitalState;
            HospitalZip = hospitalZip;
            HospitalCountry = hospitalCountry;
            HospitalAddress = hospitalAddress;
        }
        public string ID { get; set; }
        public string PastPatientId { get; set; }
        public string PhysicianFirstName { get; set; }
        public string PhysicianLastName { get; set; }
        public string StartDate { get; set; }
        public string HospitalName { get; set; }
        public string HospitalCity { get; set; }
        public string HospitalState { get; set; }
        public string HospitalZip { get; set; }
        public string HospitalCountry { get; set; }
        public string HospitalAddress { get; set; }
    }
    public class Insurance
    {
        public Insurance(string id, string insurance_completeProfileId, string memberId, string groupId, string companyName, string phone1, string phone2,
            string email, string city, string state, string zip, string address, string country)
        {
            ID = id;
            CompleteProfileId = insurance_completeProfileId;
            MemberId = memberId;
            GroupId = groupId;
            CompanyName = companyName;
            Phone1 = phone1;
            Phone2 = phone2;
            Email = email;
            City = city;
            State = state;
            Zip = zip;
            Address = address;
            Country = country;
        }
        public string ID { get; set; }
        public string CompleteProfileId { get; set; }
        public string MemberId { get; set; }
        public string GroupId { get; set; }
        public string CompanyName { get; set; }
        public string Phone1 { get; set; }
        public string Phone2 { get; set; }
        public string Email { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Address { get; set; }
        public string Country { get; set; }
    }
}