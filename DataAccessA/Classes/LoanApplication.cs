using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessA.Classes
{
    public class LoanApplication
    {
        public int ID { get; set; }
        public string LoanRefNumber { get; set; }
        public int Title_FK { get; set; }
        public string ReferalCode { get; set; }
        public string StateofResidence_FKs { get; set; }
        public string LoanTenures { get; set; }
        public string LGA_FKs { get; set; }
        public string LGA_FKss { get; set; }

        public string TStateofResidence_FKss { get; set; }
        public string NStateofResidence_FKsss { get; set; }

        public string LGA_FKsss { get; set; }
        public string NyscIdCardFilePath { get; set; }
       
        public string NyscpassportFilePath { get; set; }
        public string NyscPostingLetterFllePath { get; set; }
        public string NyscCallUpLetterFilePath { get; set; }
        public string NyscProfileDashboardFilePath { get; set; }
        public string STA_FilePath { get; set; }
        public string Surname { get; set; }
        public string Nyscfileimg { get; set; }
        public string Firstname { get; set; }
        public string FacebookName { get; set; }
        public string InstagramHandle { get; set; }
        public string TwitterHandle { get; set; }
        public string PPA_Department { get; set; }
        public string PPA_ROle { get; set; }
        public string PPA_EmailAddress { get; set; }
        public string PPA_PhoneNumber { get; set; }
        public string PPA_supervisorName { get; set; }
        public string PPA_supervisorEmail { get; set; }
        public string PPA_supervisorPhonenumber { get; set; }
       
        public string Othernames { get; set; }
        public int Gender_FK { get; set; }
        public int MaritalStatus_FK { get; set; }
        public int MeansOfID_FK { get; set; }
        public string IdentficationNumber { get; set; }
        public string DateOfBirth { get; set; }
        public string PhoneNumber { get; set; }
        public string EmailAddress { get; set; }
        public string PermanentAddress { get; set; }
        public string TemporaryAddress { get; set; }
        public string Landmark { get; set; }
        public string ClosestBusStop { get; set; }
        public int StateofResidence_FK { get; set; }

        public int TempStateofResidence_FK { get; set; }
        public int NyscStateofResidence_FK { get; set; }
        public string TempLandmark { get; set; }
        public string TempClosestBusStop { get; set; }
        public int LGA_FK { get; set; }
        public int TempLGA_FK { get; set; }
        public int NyscLGA_FK { get; set; }
    
        public string StateCode { get; set; }
        public string Organization { get; set; }
        public int AccomodationType_FK { get; set; }
        public string Employer { get; set; }
        public string PassOutMonth{ get; set; }
        public string CDSDay { get; set; }
        public string CDSGroup { get; set; }
        
        // public string RepaymentAmount { get; set; }
        public Double NetMonthlyIncome { get; set; }
        public string ApplicantID { get; set; }
        public string EMG_FullName { get; set; }

        public string EMG_HomeAddress { get; set; }
        public string EMG_PhoneNumber { get; set; }
        public string EMG_EmailAddress { get; set; }
        public string EMG_Relationship { get; set; }

        public string EMG_FullName2 { get; set; }

        public string EMG_HomeAddress2 { get; set; }
        public string EMG_PhoneNumber2 { get; set; }
        public string EMG_EmailAddress2 { get; set; }
        public string EMG_Relationship2 { get; set; }
        public string FirstRelativeName { get; set; }
        public string FirstRelativePhoneNumber { get; set; }
        public string SecondRelativeName { get; set; }
        public string SecondRelativePhoneNumber { get; set; }
        public string RelativeRelationship_FK { get; set; }
        public string RelativeRelationship2_FK { get; set; }
        public string NOK_FullName { get; set; }

        public string NOK_Relationship { get; set; }
        public string NOK_PhoneNumber { get; set; }
        public string NOK_EmailAddress { get; set; }
        public string NOK_HomeAddress { get; set; }
        public string LoanAmount { get; set; }
        public int LoanTenure { get; set; }
        public string LoanTenureStr { get; set; }
        public int RepaymentMethod_FK { get; set; }
        public bool ExistingLoan { get; set; }
        public Nullable<double> ExistingLoan_OutstandingAmount { get; set; }
        public string ExistingLoan_NoOfMonthsLeft { get; set; }
        public int Bank_FK { get; set; }
        public string AccountNumber { get; set; }
        public string AccountName { get; set; }
        public string BVN { get; set; }
        public string ValueDate { get; set; }
        public string ValueTime { get; set; }
        public Nullable<System.DateTime> DateCreated { get; set; }
        public Nullable<System.DateTime> DateModified { get; set; }
        public int ApplicationStatus_FK { get; set; }
        public string LoanComment { get; set; }
        public int IsVisible { get; set; }
        public string CreatedBy { get; set; }

        public int Recommend { get; set; }

        public string StateofResidence { get; set; }

        public string SalaryAmount { get; set; }
        public string faculty { get; set; }
        public string Department { get; set; }
        public string Gender { get; set; }

        public string InstitutionAddress { get; set; }

        public string Designation { get; set; }

        public string Occupation { get; set; }

        public string LOS { get; set; }

        public string OfficialAddress { get; set; }
        public string OfficialEmail { get; set; }

        public int Contract { get; set; }

        public string BankCode { get; set; }
        public string RepaymentAmount { get; set; }
    }
}
