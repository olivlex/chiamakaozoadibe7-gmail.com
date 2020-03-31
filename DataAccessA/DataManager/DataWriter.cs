using DataAccessA.Classes;
using DataAccessA.DataManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessA.DataManager
{
   public class DataWriter
    {
        static UvlotAEntities uvDb = new UvlotAEntities();

        public static int SaveBVNDetails(BVNC bvnc)
        {
            int i = 0;
            try
            {

                BanksManager bObj = new BanksManager
                {
                    BankName = bvnc.EnrollmentBank,
                    ContactAddress = bvnc.address,
                    DateOfBirth = bvnc.Dateofbirth,
                    EnrollmentBranch = bvnc.EnrollmentBranch,
                    Firstname = bvnc.FirstNAme,
                    Gender = bvnc.gender,
                    IsVisible = 1,
                    Lastname = bvnc.LastName,
                    Marital_Status = bvnc.marital_status,
                    Nationlaity = bvnc.Nationality,
                    Othernames = bvnc.MiddleName,
                    ValueDate = MyUtility.getCurrentLocalDateTime().ToString("yyyy/MM/dd"),
                    VerifiedStatus = bvnc.respCode == "00" ? 1 : 0,
                    ServiceResponse = bvnc.respDescription
                };
                uvDb.BanksManagers.Add(bObj);
                uvDb.SaveChanges();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message);
            }
            return i;
        }
        public int InsertUser(User user)
        {
           
            try
            {
                uvDb.Users.Add(user);
                uvDb.SaveChanges();
                return user.ID;

            }

            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return ex.Message.Count();

            }

        }

        public int insertRemita(PatnerTransactLog PL)
        {

            try
            {
                uvDb.PatnerTransactLogs.Add(PL);
                uvDb.SaveChanges();
                return PL.ID;

            }

            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return ex.Message.Count();

            }

        }
        
        public int insertLoanApproval(NYSCLoanApproval La)
        {

            try
            {
                uvDb.NYSCLoanApprovals.Add(La);
                uvDb.SaveChanges();
                return La.ID;

            }

            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return ex.Message.Count();

            }

        }
        
        public static LoanApplication CreateLoanApplication(LoanApplication instObj)
        {
            try
            {
                // uvDb= new UvlotEntities();
                uvDb.LoanApplications.Add(instObj);
                uvDb.SaveChanges();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message);
            }
            return instObj;
        }
        
              public  int InsertUserRoles(UserRole userrole)
        {
            try
            {

                uvDb.UserRoles.Add(userrole);
                uvDb.SaveChanges();
                return userrole.ID;

            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message);
                return 0;
            }

        }

       
             public int CreateReferalCode(User user)
            {
            try
            {

                var valu = (uvDb.Users.Find(user.EmailAddress));

                if(valu != null)
                {
                    valu.MyReferralCode = user.MyReferralCode;
                    uvDb.SaveChanges();
                    return valu.ID;
                }
                return valu.ID;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message);
                return 0;
            }

        }

        public static int  CreateNYSCLoanApplication(NyscLoanApplication NyscLA)
        {
            try
            {
               
                uvDb.NyscLoanApplications.Add(NyscLA);
                uvDb.SaveChanges();
                return NyscLA.ID;
              
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message);
                return 0;
            }
          
        }
        
    }
}
