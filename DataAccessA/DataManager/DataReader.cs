using DataAccessA.Classes;
using DataAccessA.DataManager;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DataAccessA.DataManager
{
    public class DataReader
    {
        static UvlotAEntities uvDb = new UvlotAEntities();



        public List<ReferralRecords> MyReferralsdetails(int userid)
        {
            try
            {
                //string user = Convert.ToString(userid);
                //var Loans = (from a in uvDb.NYSCReferralLedgers
                //             join b in uvDb.Users on a.User_FK equals b.ID
                //             where a.ReferenceNumber == user
                //             select a).ToList();

                var Loans = (from a in uvDb.NYSCReferralLedgers
                             join b in uvDb.Users on a.User_FK equals b.ID
                             where a.User_FK == userid
                             select new ReferralRecords
                             {
                                 ID = a.ID,
                                 ReferralCode = a.ReferralCode,
                                 ReferenceNumber = a.ReferenceNumber,
                                 Debit = (Double)a.Debit,
                                 Credit = (Double)a.Credit,
                                 ValueDate = a.ValueDate,
                             }).OrderByDescending(x => x.ID).ToList();


                if (Loans == null)
                {
                    return null;
                }
                return Loans;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }



        public List<AppLoanss> RegisteredApplicant()
        {
            try
            {
                var Loans = (from a in uvDb.NyscLoanApplications
                             join b in uvDb.Users on a.StateCode equals b.Audit
                             join c in uvDb.ApplicationStatus
                               on a.NYSCApplicationStatus_FK equals c.ID
                             where a.IsVisible == 1
                             select new AppLoanss
                             {
                                 ID = a.ID,
                                 LoanRefNumber = a.RefNumber,
                                 Firstname = a.Firstname,
                                 Surname = a.Surname,
                                 EmailAddress = a.EmailAddress,
                                 LoanAmount = (double)a.LoanAmount,
                                 LoanTenure = (int)a.LoanTenure,
                                 StateCode = b.Audit,
                                 ApplicationStatus = c.Name,
                             }).OrderByDescending(x => x.ID).ToList();


                if (Loans == null)
                {
                    return null;
                }
                return Loans;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }




        public List<ReferralDetails> RegisteredReferral()
        {
            try
            {
                var Loans = (from a in uvDb.Users
                             join b in uvDb.UserRoles on a.ID equals b.User_FK
                             
                             where a.IsVisible == 1  
                             select new ReferralDetails
                             {
                                 ID = a.ID,
                                
                                 Firstname = a.Firstname,
                                 Lastname = a.Lastname,
                                 EmailAddress = a.EmailAddress,
                                 PhoneNumber = a.PhoneNumber,
                                 UserAddress=a.UserAddress,
                                 ContactAddress = a.ContactAddress,
                                 ReferralCode = a.ReferralCode,
                                 MyReferralCode = a.MyReferralCode,
                               //  StateCode = a.Audit,
                               
                             }).OrderByDescending(x => x.ID).ToList();


                if (Loans == null)
                {
                    return null;
                }
                return Loans;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        public List<BVNRecords> Mybvndetails()
        {
            try
            {


                var Loans = (from a in uvDb.BanksManagers


                             select new BVNRecords
                             {
                                 ID = a.ID,
                                 Lastname = a.Lastname,
                                 Firstname = a.Firstname,
                                 Othernames = a.Othernames,
                                 DateOfBirth = a.DateOfBirth,
                                 Gender = a.Gender,
                                 BankName = a.BankName,
                                 EnrollmentBranch = a.EnrollmentBranch,
                                 Nationlaity = a.Nationlaity,
                                 Marital_Status = a.Marital_Status,
                                 ContactAddress = a.ContactAddress,
                                 VerifiedStatus = (int)a.VerifiedStatus,
                                 ServiceResponse = a.ServiceResponse,
                                 IsVisible = (int)a.IsVisible,
                                 ValueDate = a.ValueDate,

                             }).OrderByDescending(x => x.ID).ToList();


                if (Loans == null)
                {
                    return null;
                }
                return Loans;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


        public dynamic GetNYSCLoanApplicationSummary()
        {
            try
            {
                var Apploan = uvDb.GetNYSCLoanApplicationSummary().ToList();

                if (Apploan == null)
                {

                    return null;
                }

                return Apploan;

            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


        public dynamic GetReferrals(int AppStatFk)
        {
            try
            {

                var Apploan = uvDb.GetReferrals(AppStatFk).ToList();

                if (Apploan == null)
                {

                    return null;
                }

                return Apploan;

            }
            //catch (Exception ex)
            //{
            //    WebLog.Log(ex.Message.ToString());
            //    return null;
            //}
            catch (DbEntityValidationException dbEx)
            {
                var sb = new StringBuilder();
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        sb.AppendLine(string.Format("Property: {0} Error: {1}",
                        validationError.PropertyName, validationError.ErrorMessage));
                    }
                }
                throw new Exception(sb.ToString(), dbEx);
            }

            }

        public dynamic GetReferralActivity()
        {
            try
            {
                var Apploan = uvDb.GetReferralActivity().ToList();

                if (Apploan == null)
                {

                    return null;
                }

                return Apploan;

            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


      
        public dynamic GetNYSCDefaultLoans()
        {
            try
            {
                var Apploan = uvDb.GetNYSCDefaultLoans().Distinct().ToList();

                if (Apploan == null)
                {

                    return null;
                }

                return Apploan;

            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }




        public dynamic LoanDueForDebits(DateTime DOB)
        {
            try
            {
                var Apploan = uvDb.LoanDueForDebit(DOB).ToList();

                if (Apploan == null)
                {

                    return null;
                }

                return Apploan;

            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


        public dynamic ApplicantTRelated(DateTime DOB, DateTime DOBs)
        {
            try
            {
                var Apploan = uvDb.NyscApplicationRelated(DOB, DOBs).ToList();

                if (Apploan == null)
                {

                    return null;
                }

                return Apploan;

            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }



        public dynamic OutStandingLoans(DateTime DOB)
        {
            try
            {
                var Apploan = uvDb.OutStandingLoan(DOB).Distinct().ToList();

                if (Apploan == null)
                {

                    return null;
                }

                return Apploan;

            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }

        }

        public dynamic Repayment(DateTime DOB, DateTime DOBS)
        {
            try
            {
                var Apploan = uvDb.Repayment(DOB, DOBS).Distinct().ToList();

                if (Apploan == null)
                {

                    return null;
                }

                return Apploan;

            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        public dynamic RevenueReceived(DateTime DOB)
        {
            try
            {
                var Apploan = uvDb.RevenueReceived(DOB).Distinct().ToList();

                if (Apploan == null)
                {

                    return null;
                }

                return Apploan;

            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


        public dynamic RevenueEarned(DateTime DOB)
        {
            try
            {
                var Apploan = uvDb.RevenueEarned(DOB).Distinct().ToList();

                if (Apploan == null)
                {

                    return null;
                }

                return Apploan;

            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }
        public List<AppLoanss> GetReferredApp(string Refid)
        {
            try

            {
                var Apploan = (from a in uvDb.NyscLoanApplications
                                   //join b in uvDb.NyscLoanApplications on a.MyReferralCode equals b.ReferralCode
                               join C in uvDb.LGAs on a.NyscLGA_FK equals C.ID
                               join d in uvDb.NigerianStates on a.NyscStateofResidence_FK equals d.ID
                               where a.ReferralCode == Refid
                               select new AppLoanss
                               {
                                   ID = a.ID,

                                   ReferralCode = a.ReferralCode,

                                   LoanRefNumber = a.RefNumber,

                                   StateCode = a.StateCode,

                                   Firstname = a.Firstname,
                                   Surname = a.Surname,

                                   LoanAmount = a.LoanAmount.Value,

                                   LoanTenure = a.LoanTenure.Value,

                                   NyscStateofResidence = d.Name,

                                   NyscLGAs = C.Name,


                               }).OrderByDescending(x => x.ID).ToList();

                if (Apploan == null)
                {

                    return null;
                }

                return Apploan;

            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }



        public List<double> GetKpiInMinutes(int Refid)
        {

            DateTime TD = DateTime.Now;
            double TimeDiffx = 0;
            double kpi = 0;
            try
            {

                List<double> KpiMinutes = new List<double>();

                var Rec = (from a in uvDb.NyscLoanApplications
                           join b in uvDb.NYSCLoanApprovals on a.ID equals b.LoanApplication_FK

                           where a.ID == Refid
                           select new AppLoanss
                           {
                               LoanRefNumber = a.RefNumber,

                               APplicationDate = (DateTime)a.DateCreated,
                               ApplicationApprove = (DateTime)b.DateCreated

                           }).ToList();



                if (Rec.Count > 0)
                {
                    var Count = Rec.Count;
                    var AppDate = Rec.Select(x => x.APplicationDate).FirstOrDefault();
                    for (var i = 0; i <= Rec.Count; i++)
                    {
                        var TimeDiff = Rec.Select(x => x.ApplicationApprove).ElementAt(i);
                        if (i + 1 == Count)
                        {
                            return KpiMinutes;
                        }
                        else
                        {
                            if (i == 0)
                            {
                                TD = Rec.Select(x => x.ApplicationApprove).FirstOrDefault();
                                TimeDiffx = TD.Subtract(AppDate).TotalMinutes;
                                kpi = Convert.ToInt16(TimeDiffx);
                                KpiMinutes.Add(kpi);
                            }
                            TD = Rec.Select(x => x.ApplicationApprove).ElementAt(i + 1);
                            TimeDiffx = TD.Subtract(TimeDiff).TotalMinutes;
                            kpi = Convert.ToInt16(TimeDiffx);
                            KpiMinutes.Add(kpi);
                        }

                    }
                }
                //if (Rec.Count > 0)
                //{
                //    var Count = Rec.Count;
                //    var AppDate = Rec.Select(x => x.APplicationDate).FirstOrDefault();
                //    for (var i = 0; i <= Rec.Count; i++)
                //    {
                //        var TimeDiff = Rec.Select(x => x.ApplicationApprove).ElementAt(i);
                //        if (i + 1 >= Count)
                //        {
                //            return KpiMinutes;
                //        }

                //        if (Count != Count - 1)
                //        {
                //            if (i == 0)
                //            {
                //                var TD = Rec.Select(x => x.ApplicationApprove).ElementAt(i);
                //                var TimeDiffx = TD.Subtract(AppDate).TotalMinutes;
                //                double kpi = Convert.ToInt16(TimeDiffx);
                //                KpiMinutes.Add(kpi);
                //            }
                //            else
                //            {
                //                var TD = Rec.Select(x => x.ApplicationApprove).ElementAt(i + 1);
                //                var TimeDiffx = TD.Subtract(TimeDiff).TotalMinutes;
                //                double kpi = Convert.ToInt16(TimeDiffx);

                //                KpiMinutes.Add(kpi);
                //            }
                //        }

                //if (Rec.Count > 0)
                //{
                //    var Count = Rec.Count;
                //    var AppDate = Rec.Select(x => x.APplicationDate).FirstOrDefault();
                //    for (var i = 0; i <= Rec.Count; i++)
                //    {
                //        var TimeDiff = Rec.Select(x => x.ApplicationApprove).ElementAt(i);
                //        if (i + 1 >= Count)
                //        {
                //            return KpiMinutes;
                //        }
                //        if (Count != Count - 1)
                //        {
                //            var TD = Rec.Select(x => x.ApplicationApprove).ElementAt(i + 1);
                //            var TimeDiffx = TD.Subtract(TimeDiff).TotalMinutes;
                //            double kpi = Convert.ToInt16(TimeDiffx);

                //            KpiMinutes.Add(kpi);
                //        }

                //    }
                //}


                return KpiMinutes;
            }

            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


        public dynamic Top50ReferralPerformance()
        {
            try
            {
                var Apploan = uvDb.Top50ReferralPerformance().ToList();

                if (Apploan == null)
                {

                    return null;
                }

                return Apploan;

            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }



        public dynamic ReferralAgentPerformance()
        {
            try
            {
                var Apploan = uvDb.ReferralAgentPerformance().ToList();

                if (Apploan == null)
                {

                    return null;
                }

                return Apploan;

            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        public dynamic DisbursedLoans()
        {
            try
            {
                var Apploan = uvDb.DisbursedLoans().ToList();

                if (Apploan == null)
                {

                    return null;
                }

                return Apploan;

            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


        public dynamic BorrowedLoan()
        {
            try
            {
                var Apploan = uvDb.BorroweredLoans().ToList();

                if (Apploan == null)
                {

                    return null;
                }

                return Apploan;

            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }



        //public List<Top50Referral> Top50ReferralPerformance()
        //{
        //    try

        //    {
        //        var Apploan = (from a in uvDb.NyscLoanApplications

        //                       join b in uvDb.NYSCLoanLedgers on a.ID equals b.LoanApplication_FK
        //                       join c in uvDb.UserRoles on
        //                       where a.ID == b.LoanApplication_FK
        //                       select new Top50Referral
        //                       {
        //                           ID = a.ID,


        //                           Surname = a.Surname,

        //                           Firstname = a.Firstname,
        //                           Disbursedloan = a.Surname,
        //                           LoanTenure = a.LoanTenure.Value,
        //                           LoanAmount = a.LoanAmount.Value,

        //                           ValueDate = a.ValueDate,

        //                           ValueDates = b.ValueDate,




        //                       }).OrderByDescending(x => x.ID).ToList();

        //        if (Apploan == null)
        //        {

        //            return null;
        //        }

        //        return Apploan;

        //    }
        //    catch (Exception ex)
        //    {
        //        WebLog.Log(ex.Message.ToString());
        //        return null;
        //    }
        //}

        //public List<AppLoanss> GetKPITime()
        //{
        //    try

        //    {
        //        var Apploan = (from a in uvDb.NyscLoanApplications

        //                       join b in uvDb.NYSCLoanApprovals on a.ID equals b.LoanApplication_FK

        //                       where a.ID == b.LoanApplication_FK && a.IsVisible == 1
        //                       select new AppLoanss
        //                       {
        //                           ID = a.ID,


        //                           LoanRefNumber = a.RefNumber,

        //                           Firstname = a.Firstname,
        //                           Surname = a.Surname,
        //                           LoanTenure = a.LoanTenure.Value,
        //                           LoanAmount = a.LoanAmount.Value,

        //                           ValueDate = a.ValueDate,

        //                           ValueDates = b.ValueDate,



        //                           //}).distinct(X=>x.LoanRefNum).ToList()
        //                       }).OrderByDescending(x => x.ID).ToList();

        //        if (Apploan == null)
        //        {

        //            return null;
        //        }

        //        return Apploan;

        //    }
        //    catch (Exception ex)
        //    {
        //        WebLog.Log(ex.Message.ToString());
        //        return null;
        //    }
        //}




        public List<NyscLoanApplication> GetKPITime()
        {
            try

            {
                var Apploan = (from a in uvDb.NyscLoanApplications where a.IsVisible == 1 && a.NYSCApplicationStatus_FK != 6 && a.NYSCApplicationStatus_FK != 7
                               && a.NYSCApplicationStatus_FK != 8 && a.NYSCApplicationStatus_FK != 11 && a.NYSCApplicationStatus_FK != null && a.Audit == null

                               select a).Distinct().ToList();

              
                if (Apploan == null)
                {

                    return null;
                }

                return Apploan;

            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


        //public List<AppLoanss> GetKPITime()
        //{
        //    try

        //    {
        //        var Apploan = (from a in uvDb.NyscLoanApplications

        //                       join b in uvDb.NYSCLoanApprovals on a.ID equals b.LoanApplication_FK

        //                       where a.ID == b.LoanApplication_FK && a.IsVisible == 1 && a.Audit != "OfflineRecords"
        //                       select new AppLoanss
        //                       {
        //                           ID = a.ID,


        //                           LoanRefNumber = a.RefNumber,

        //                           Firstname = a.Firstname,
        //                           Surname = a.Surname,
        //                           LoanTenure = a.LoanTenure.Value,
        //                           LoanAmount = a.LoanAmount.Value,

        //                           ValueDate = a.ValueDate,

        //                           ValueDates = b.ValueDate,




        //                       }).Distinct().ToList();

        //        if (Apploan == null)
        //        {

        //            return null;
        //        }

        //        return Apploan;

        //    }
        //    catch (Exception ex)
        //    {
        //        WebLog.Log(ex.Message.ToString());
        //        return null;
        //    }
        //}
        public List<NYSCReferralLedger> getCommisioRecords(int userid)
        {
            try
            {

                var recoreds = (from a in uvDb.NYSCReferralLedgers where a.User_FK == userid select a).ToList();

                var newRec = (from a in uvDb.NYSCReferralLedgers
                              group a by a.ID into g
                              where g.Sum(s => s.Credit) - g.Sum(s => s.Debit) > 0
                              from NYSCReferralLedger in g
                              select NYSCReferralLedger);

                // var Credit = recoreds.Where(q => q.Credit != 0).Select(x => x.Credit).Count();

                // var DebitSum = recoreds.Sum(x => x.Debit);

                if (recoreds == null)
                {
                    return null;
                }

                return recoreds.ToList();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }



        public List<NYSCReferralLedger> getCommisioRecordsExact(int userfk, int Num)
        {
            try
            {

                var recoreds = (from a in uvDb.NYSCReferralLedgers where a.User_FK == userfk && a.Debit == 0 select a).OrderBy(a => a.ID).ToList().Take(Num);



                if (recoreds == null)
                {
                    return null;
                }

                return recoreds.ToList();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


        public int UpdateNYSCLoanReferalLeder(List<NYSCReferralLedger> Ln, double mode, double div)
        {
            try
            {
                NYSCReferralLedger NL = new NYSCReferralLedger();
                var bn = 0;
                int nb = Ln.Count;
                for (var c = 0; c < nb; c++)
                {
                    int cv = Ln[c].ID;
                    var original = (from a in uvDb.NYSCReferralLedgers where a.ID == cv select a).SingleOrDefault();

                    if (c + 1 == div && mode != 0)
                    {
                        original.Debit = mode;
                    }

                    else
                    {
                        original.Debit = Ln[c].Credit;
                    }
                    uvDb.SaveChanges();

                    bn = original.ID;
                }

                if (div > Ln.Count)
                {

                    NL.User_FK = Ln[0].User_FK;
                    NL.Debit = mode;
                    NL.Credit = 0;
                    NL.TrnDate = null;
                    uvDb.NYSCReferralLedgers.Add(NL);
                    uvDb.SaveChanges();

                }

                return bn;


            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return 0;
            }
        }


        //Public List<apploan> Referrals(Int32 userid)
        //{
        //    var amtlst = (from c in uvdb.nyscreferalledger Join b in user on c.user_fk equal b.id where c.user_fk == userid select New apploan({ Pass your varaiable to display hereSN, Date, Reference Number, Debit, Credit } ).To list();

        public NYSCLoanSetUp GetloanParam(int tenure)
        {
            try
            {
                var test = tenure;
                var param = (from a in uvDb.NYSCLoanSetUps where a.Tenure == tenure select a).FirstOrDefault();
                if (param == null)
                {
                    return null;
                }
                return param;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        public string GetRepaymenrAmount(int Tenure)
        {
            string repayment = "0";
            try
            {

                var xx = (from a in uvDb.NYSCLoanSetUps where a.Tenure == Tenure select a).FirstOrDefault();

                if (xx == null)
                {
                    return repayment;
                }
                repayment = xx.Repayment.ToString();

            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());

            }
            return repayment;
        }
        public string GetBankCodeByName(string BankName)
        {
            string BankCode = "0";
            try
            {

                var xx = (from a in uvDb.Banks where a.Name == BankName select a).FirstOrDefault();

                if (xx == null)
                {
                    return BankCode;
                }
                BankCode = xx.Code;

            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());

            }
            return BankCode;
        }

        public double GetAmountByTenure(int Tenure, out string repayment)
        {
            repayment = "0";
            try
            {

                var xx = (from a in uvDb.NYSCLoanSetUps where a.Tenure == Tenure select a).FirstOrDefault();

                if (xx == null)
                {
                    return 0;
                }
                repayment = xx.Repayment.ToString();
                return Convert.ToDouble(xx.Amount);
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return 0;
            }
        }

        public int getUserID(string email)
        {

            try
            {
                var userid = (from a in uvDb.Users where a.EmailAddress == email select a.ID).FirstOrDefault();

                if (userid == 0)
                {
                    return 0;
                }

                return userid;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return 0;
            }
        }

        public List<int> getUserRols(int id)
        {
            try
            {
                var userroles = (from a in uvDb.UserRoles where a.User_FK == id select a.Role_FK).ToList();
                return userroles.ToList();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }

        }
        public List<Role> getUserRoles(List<int> roleids)
        {
            //  here i am 
            try
            {
                var roles = (from p in uvDb.Roles where roleids.Contains((int)(p.RoleId)) select p).ToList();
                if (roles == null)
                {
                    return null;
                }

                return roles;
            }
            catch (Exception ex)
            {

                WebLog.Log(ex.Message.ToString());
                return null;
            }


        }

        // Get Remita Banks 
        public List<Bank> getAllRemitaBanks()
        {
            try
            {
                var Banks = (from a in uvDb.Banks select a).ToList();
                if (Banks == null)
                {
                    return null;
                }
                return Banks;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }
        public List<Bank> GetBanks()
        {
            try
            {
                var Banks = (from a in uvDb.Banks select a).ToList();
                if (Banks == null)
                {
                    return null;
                }
                return Banks;
            }
            //catch (Exception ex)
            //{
            //    WebLog.Log(ex.Message.ToString());
            //    return null;
            //}

            catch (DbEntityValidationException ex)
            {
                string errormessages = string.Join("; ", ex.EntityValidationErrors.SelectMany(x =>
                x.ValidationErrors).Select(x => x.PropertyName + ": " + x.ErrorMessage));
                throw new DbEntityValidationException(errormessages);
            }
        }
        public List<Menus> getResults(List<string> rol)
        {
            try
            {
                var results = (from p in uvDb.Pages
                               join pa in uvDb.PageAuthentications on p.PageName equals pa.PageName
                               join ph in uvDb.pageHeaders on p.PageHeader equals ph.id
                               join r in uvDb.Roles on pa.Role_FK equals r.RoleId

                               where rol.Contains(r.RoleName)
                               select new Menus
                               {
                                   pageName = pa.PageName,
                                   roleid = (int)pa.Role_FK,
                                   pageheader = ph.page_header,
                                   pageurl = p.PageUrl,

                               }).Distinct();

                return results.ToList();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }
        //public dynamic LoanRepayment(DateTime to, DateTime from)
        //{
        //    try
        //    {
        //        var Apploan = uvDb.LoanRepayment(from, to).ToList();

        //        if (Apploan == null)
        //        {

        //            return null;
        //        }

        //        return Apploan;

        //    }
        //    catch (Exception ex)
        //    {
        //        WebLog.Log(ex.Message.ToString());
        //        return null;
        //    }
        //}


        public User getUser(string email)
        {
            try
            {
                var users = (from a in uvDb.Users where a.EmailAddress == email select a).FirstOrDefault();

                if (users == null)
                {
                    return null;
                }

                return users;
            }
            catch (Exception ex)
            {

                WebLog.Log(ex.Message.ToString());
                return null;
            }


        }


        // To be treated
        //public NYSCLoanSetUps GetRate(int tenure)
        //{
        //    try
        //    {
        //        var Repayment = (from a in uvDb.NYSCLoanSetUps where a.Tenure == tenure select a).FirstOrDefault();

        //        if (Repayment == null)
        //        {
        //            return null;
        //        }
        //        else

        //            return Repayment;

        //    }
        //    catch (Exception ex)
        //    {
        //        WebLog.Log(ex.Message.ToString());
        //        return null;
        //    }
        //}
        //To be treated

        public List<ExcelList> NYSCApplications(string userEmail)

        {
            try
            {


                User appUser = getUser(userEmail);
                string myreferralCode = appUser.MyReferralCode;

                var Loans = (from a in uvDb.NyscLoanApplications
                             join b in uvDb.NigerianStates on a.NyscStateofResidence_FK equals b.ID
                             join c in uvDb.NYSCApplicationStatus on a.NYSCApplicationStatus_FK equals c.ID
                             where a.ReferralCode == myreferralCode
                             select new ExcelList
                             {
                                 ID = a.ID,
                                 ApplicationStatus = c.Name,
                                 AccountName = a.AccountName,

                                 ReferralCode = a.ReferralCode,

                                 LoanRefNumber = a.RefNumber,

                                 AccountNumber = a.AccountNumber,

                                 PermanentAddress = a.PermanentAddress,

                                 Firstname = a.Firstname,

                                 LoanAmount = a.LoanAmount.Value,

                                 LoanTenure = a.LoanTenure.Value,

                                 ValueDate = a.ValueDate,

                                 Surname = a.Surname,


                             }).OrderByDescending(x => x.ID).ToList();

                if (Loans == null)
                {
                    return null;
                }
                return Loans;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }
        public List<ExcelList> ApproveLoans(int AppStatFk)

        {
            try
            {
                var Loans = (from a in uvDb.NyscLoanApplications
                             join b in uvDb.NigerianStates on a.NyscStateofResidence_FK equals b.ID
                             join c in uvDb.NYSCApplicationStatus on a.NYSCApplicationStatus_FK equals c.ID
                             join d in uvDb.PatnerTransactLogs on a.RefNumber equals d.RefNum
                             where a.NYSCApplicationStatus_FK == AppStatFk && a.IsVisible == 1
                             select new ExcelList
                             {
                                 ID = a.ID,
                                 ApplicationStatus = c.Name,
                                 AccountName = a.AccountName,

                                 //ReferralCode = a.MyReferralCode,

                                 LoanRefNumber = a.RefNumber,

                                 AccountNumber = a.AccountNumber,

                                 PermanentAddress = a.PermanentAddress,

                                 Firstname = a.Firstname,

                                 LoanAmount = a.LoanAmount.Value,

                                 LoanTenure = a.LoanTenure.Value,

                                 ValueDate = a.ValueDate,

                                 Surname = a.Surname,

                                 RemitaLink = string.IsNullOrEmpty(d.PatnerUrl) ? "none" : d.PatnerUrl,


                             }).OrderByDescending(x => x.ID).ToList();

                if (Loans == null)
                {
                    return null;
                }
                return Loans;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }




        public List<ExcelList> FourthApproval(int AppStatFk)

        {
            try
            {
                var Loans = (from a in uvDb.NyscLoanApplications
                             join b in uvDb.NigerianStates on a.NyscStateofResidence_FK equals b.ID
                             join c in uvDb.NYSCApplicationStatus on a.NYSCApplicationStatus_FK equals c.ID

                             where a.NYSCApplicationStatus_FK == AppStatFk && a.IsVisible == 1
                             select new ExcelList
                             {
                                 ID = a.ID,
                                 ApplicationStatus = c.Name,
                                 AccountName = a.AccountName,
                                 EmailAddress = a.EmailAddress,
                                 //ReferralCode = a.MyReferralCode,

                                 LoanRefNumber = a.RefNumber,

                                 AccountNumber = a.AccountNumber,

                                 PermanentAddress = a.PermanentAddress,
                                 PhoneNumber = a.PhoneNumber,
                                 Firstname = a.Firstname,

                                 LoanAmount = a.LoanAmount.Value,

                                 LoanTenure = a.LoanTenure.Value,

                                 ValueDate = a.ValueDate,

                                 Surname = a.Surname,

                                 // RemitaLink = string.IsNullOrEmpty(d.PatnerUrl) ? "none" : d.PatnerUrl,


                             }).OrderByDescending(x => x.ID).ToList();

                if (Loans == null)
                {
                    return null;
                }
                return Loans;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


        public List<ExcelList> ApproveLoansRemFlu(int AppStatFk, string PatnerCode)

        {
            try
            {
                var Loans = (from a in uvDb.NyscLoanApplications
                             join b in uvDb.NigerianStates on a.NyscStateofResidence_FK equals b.ID
                             join c in uvDb.NYSCApplicationStatus on a.NYSCApplicationStatus_FK equals c.ID
                             join d in uvDb.PatnerTransactLogs on a.RefNumber equals d.RefNum
                             where a.NYSCApplicationStatus_FK == AppStatFk && a.IsVisible == 1 && d.PatnerCode == PatnerCode
                             select new ExcelList
                             {
                                 ID = a.ID,
                                 ApplicationStatus = c.Name,
                                 AccountName = a.AccountName,

                                 //ReferralCode = a.MyReferralCode,

                                 LoanRefNumber = a.RefNumber,

                                 AccountNumber = a.AccountNumber,

                                 PermanentAddress = a.PermanentAddress,

                                 Firstname = a.Firstname,

                                 LoanAmount = a.LoanAmount.Value,

                                 LoanTenure = a.LoanTenure.Value,

                                 ValueDate = a.ValueDate,

                                 Surname = a.Surname,

                                 RemitaLink = string.IsNullOrEmpty(d.PatnerUrl) ? "none" : d.PatnerUrl,


                             }).OrderByDescending(x => x.ID).ToList();

                if (Loans == null)
                {
                    return null;
                }
                return Loans;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        public List<AppLoanss> ApproveLoanss(int AppstatFk)
        {
            try
            {
                var Loans = (from a in uvDb.NyscLoanApplications
                             join b in uvDb.NigerianStates on a.NyscStateofResidence_FK equals b.ID
                          


                            

                             join c in uvDb.Titles on a.Title_FK equals c.ID
                             join d in uvDb.MaritalStatus on a.MaritalStatus_FK equals d.ID

                             join f in uvDb.NigerianStates on a.StateofResidence_FK equals f.ID
                             join g in uvDb.NigerianStates on a.TempStateofResidence_FK equals g.ID
                             join h in uvDb.NigerianStates on a.NyscStateofResidence_FK equals h.ID
                             join i in uvDb.LGAs on a.LGA_FK equals i.ID
                             join j in uvDb.LGAs on a.TempLGA_FK equals j.ID
                             join k in uvDb.LGAs on a.NyscLGA_FK equals k.ID

                             join l in uvDb.Banks on a.BankCode equals l.Code
                             



                             where a.NYSCApplicationStatus_FK == AppstatFk && a.IsVisible == 1
                             select new AppLoanss
                           
                              {
                                  ID = a.ID,
                                 
                                  AccountName = a.AccountName,
                                  Title = c.Name,
                                  StateofResidence = f.Name,
                                  StateCode = a.StateCode,
                                  ApplicationStatus_FK = AppstatFk,
                                  NYSCApplicationStatus_FK = AppstatFk,
                                  TempStateofResidence = g.Name,
                                  NyscStateofResidence = h.Name,
                                  LGAs = i.Name,
                                  Gender = a.Gender_FK == 1 ? "Male" : "Female",
                                  LoanRefNumber = a.RefNumber,
                                  MaritalStatus = d.Name,
                                 
                                  NetMonthlyIncome = a.NetMonthlyIncome.Value,
                                  AccountNumber = a.AccountNumber,
                                 
                                  BankCode = l.Name,
                                  bankcodes = l.Code,
                                  BVN = a.BVN,
                                  ClosestBusStop = a.ClosestBusStop,
                                  PermanentAddress = a.PermanentAddress,
                                  DateOfBirth = a.DateOfBirth,
                                  EmailAddress = a.EmailAddress,
                                  ExistingLoan = a.ExistingLoan.HasValue ? a.ExistingLoan.Value : false,
                                  ExistingLoan_NoOfMonthsLeft = a.ExistingLoan_NoOfMonthsLeft.Value,
                                  ExistingLoan_OutstandingAmount = a.ExistingLoan_OutstandingAmount.Value > 0 ? a.ExistingLoan_OutstandingAmount.Value : 0,
                                  Firstname = a.Firstname,
                                 
                                  LoanAmount = a.LoanAmount.Value,
                                 
                                  LoanTenure = a.LoanTenure.Value,
                                  EMG_EmailAddress = a.EMG_EmailAddress,
                                  EMG_FullName = a.EMG_FullName,
                                  EMG_HomeAddress = a.EMG_HomeAddress,
                                  EMG_PhoneNumber = a.EMG_PhoneNumber,
                                  EMG_Relationship = a.EMG_Relationship,
                                  EMG_EmailAddress2 = a.EMG_EmailAddress2,
                                  EMG_FullName2 = a.EMG_FullName2,
                                  EMG_HomeAddress2 = a.EMG_HomeAddress2,
                                  EMG_PhoneNumber2 = a.EMG_PhoneNumber2,
                                  EMG_Relationship2 = a.EMG_Relationship2,
                                  TemporaryAddress = a.TemporaryAddress,
                                  TempClosestBusStop = a.TempClosestBusStop,
                                  TempLandmark = a.TempLandmark,
                                  PhoneNumber = a.PhoneNumber,
                                  ValueDate = a.ValueDate,
                                  ValueTime = a.ValueTime,
                                  Landmark = a.Landmark,
                                  Othernames = a.Othernames,
                                  Surname = a.Surname,
                                  TempLGAs = j.Name,
                                  NyscLGAs = k.Name,
                                  PassOutMonth = a.PassOutMonth,
                                  OfficialAddress = a.OfficialAddress,
                                  Employer = a.Employer,
                                  CDSDay = a.CDSDay,
                                  CDSGroup = a.CDSGroup,
                                  SalaryAmount = (double)a.NetMonthlyIncome,


                                  PPA_Department = a.PPA_Department,
                                  PPA_EmailAddress = a.PPA_EmailAddress,
                                  PPA_PhoneNumber = a.PPA_PhoneNumber,
                                  PPA_ROle = a.PPA_ROle,
                                  PPA_supervisorEmail = a.PPA_supervisorEmail,
                                  PPA_supervisorName = a.PPA_supervisorName,
                                  PPA_supervisorPhonenumber = a.PPA_supervisorPhonenumber,

                                  FacebookName = a.FacebookName,
                                  TwitterHandle = a.TwitterHandle,
                                  InstagramHandle = a.InstagramHandle,
                                  NyscIdCardFilePath = a.NyscIdCardFilePath,
                                  STA_FilePath = a.STA_FilePath,
                                  NyscpassportFilePath = a.NyscpassportFilePath,
                                  NyscCallUpLetterFilePath = a.NyscCallUpLetterFilePath,
                                  NyscPostingLetterFllePath = a.NyscPostingLetterFllePath,
                                  NyscProfileDashboardFilePath = a.NyscProfileDashboardFilePath,
                                  LetterOfundertaken = a.LetterOfundertaken,
                                  ReferralCode = a.ReferralCode,
                                  RelativeRelationship2_FK = a.RelativeRelationship2_FK,
                                  RelativeRelationship_FK = a.RelativeRelationship_FK,
                                  SecondRelativeName = a.SecondRelativeName,
                                  SecondRelativePhoneNumber = a.SecondRelativePhoneNumber,
                                  FirstRelativeName = a.FirstRelativeName,
                                  FirstRelativePhoneNumber = a.FirstRelativePhoneNumber,

                              }).OrderByDescending(x => x.ID).ToList();


                if (Loans == null)
                {
                    return null;
                }
                return Loans;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }



        public List<AppLoanss> ApproveLoansss(int AppstatFk, string userEmail)
        {
            try
            {
                var Loans = (from a in uvDb.NyscLoanApplications
                             //join b in uvDb.NigerianStates on a.NyscStateofResidence_FK equals b.ID

                             //join c in uvDb.Titles on a.Title_FK equals c.ID
                             //join d in uvDb.MaritalStatus on a.MaritalStatus_FK equals d.ID

                             //join f in uvDb.NigerianStates on a.StateofResidence_FK equals f.ID
                             //join g in uvDb.NigerianStates on a.TempStateofResidence_FK equals g.ID
                             //join h in uvDb.NigerianStates on a.NyscStateofResidence_FK equals h.ID
                             //join i in uvDb.LGAs on a.LGA_FK equals i.ID
                             //join j in uvDb.LGAs on a.TempLGA_FK equals j.ID
                             //join k in uvDb.LGAs on a.NyscLGA_FK equals k.ID

                             //join l in uvDb.Banks on a.BankCode equals l.Code

                             //where a.NYSCApplicationStatus_FK == AppstatFk && a.IsVisible == 1
                             where a.NYSCApplicationStatus_FK == AppstatFk && a.IsVisible == 1 && a.EmailAddress == userEmail

                             select new AppLoanss

                             {
                                 ID = a.ID,
                                 MaritalStatus = a.MaritalStatus_FK.ToString(),
                                 // Title = c.Name,
                                 Title = a.Title_FK.ToString(),
                                 Othernames = a.Othernames,
                                 Surname = a.Surname,
                                 Firstname = a.Firstname,
                                 DateOfBirth = a.DateOfBirth,
                                 Gender = a.Gender_FK == 1 ? "Male" : "Female",
                                 EmailAddress= a.EmailAddress,
                                 PhoneNumber = a.PhoneNumber,








                                 AccountName = a.AccountName,
                               
                                 StateofResidence = a.StateofResidence_FK.ToString(),
                                 StateCode = a.StateCode,
                                 ApplicationStatus_FK = AppstatFk,
                                 NYSCApplicationStatus_FK = AppstatFk,
                                 TempStateofResidence = a.TempStateofResidence_FK.ToString(),
                                 //NyscStateofResidence = h.Name,
                                 LGAs = a.LGA_FK.ToString(),
                              
                                 LoanRefNumber = a.RefNumber,
                          

                                 NetMonthlyIncome = a.NetMonthlyIncome.Value,
                                 AccountNumber = a.AccountNumber,

                                 //BankCode = l.Name,
                                 //bankcodes = l.Code,
                                 BVN = a.BVN,
                                 ClosestBusStop = a.ClosestBusStop,
                                 PermanentAddress = a.PermanentAddress,
                                 
                                
                                 ExistingLoan = a.ExistingLoan.HasValue ? a.ExistingLoan.Value : false,
                                 ExistingLoan_NoOfMonthsLeft = a.ExistingLoan_NoOfMonthsLeft.Value,
                                 ExistingLoan_OutstandingAmount = a.ExistingLoan_OutstandingAmount.Value > 0 ? a.ExistingLoan_OutstandingAmount.Value : 0,
                       

                                // LoanAmount = a.LoanAmount.Value,
                                 IsVisible = 1,
                                // LoanTenure = a.LoanTenure.Value,
                                 EMG_EmailAddress = a.EMG_EmailAddress,
                                 EMG_FullName = a.EMG_FullName,
                                 EMG_HomeAddress = a.EMG_HomeAddress,
                                 EMG_PhoneNumber = a.EMG_PhoneNumber,
                                 EMG_Relationship = a.EMG_Relationship,
                                 EMG_EmailAddress2 = a.EMG_EmailAddress2,
                                 EMG_FullName2 = a.EMG_FullName2,
                                 EMG_HomeAddress2 = a.EMG_HomeAddress2,
                                 EMG_PhoneNumber2 = a.EMG_PhoneNumber2,
                                 EMG_Relationship2 = a.EMG_Relationship2,
                                 TemporaryAddress = a.TemporaryAddress,
                                 TempClosestBusStop = a.TempClosestBusStop,
                                 TempLandmark = a.TempLandmark,
                               
                                 ValueDate = a.ValueDate,
                                 ValueTime = a.ValueTime,
                                 Landmark = a.Landmark,
                             
                                 TempLGAs = a.TempLGA_FK.ToString(),
                                // NyscLGAs = k.Name,
                                 PassOutMonth = a.PassOutMonth,
                                 OfficialAddress = a.OfficialAddress,
                                 Employer = a.Employer,
                                 CDSDay = a.CDSDay,
                                 CDSGroup = a.CDSGroup,
                                 //SalaryAmount = (double)a.NetMonthlyIncome,


                                 PPA_Department = a.PPA_Department,
                                 PPA_EmailAddress = a.PPA_EmailAddress,
                                 PPA_PhoneNumber = a.PPA_PhoneNumber,
                                 PPA_ROle = a.PPA_ROle,
                                 PPA_supervisorEmail = a.PPA_supervisorEmail,
                                 PPA_supervisorName = a.PPA_supervisorName,
                                 PPA_supervisorPhonenumber = a.PPA_supervisorPhonenumber,

                                 FacebookName = a.FacebookName,
                                 TwitterHandle = a.TwitterHandle,
                                 InstagramHandle = a.InstagramHandle,
                                 NyscIdCardFilePath = a.NyscIdCardFilePath,
                                 STA_FilePath = a.STA_FilePath,
                                 //NyscpassportFilePath = a.NyscpassportFilePath,
                                 //NyscCallUpLetterFilePath = a.NyscCallUpLetterFilePath,
                                 //NyscPostingLetterFllePath = a.NyscPostingLetterFllePath,
                                 //NyscProfileDashboardFilePath = a.NyscProfileDashboardFilePath,
                                 //LetterOfundertaken = a.LetterOfundertaken,
                                 ReferralCode = a.ReferralCode,
                                 //RelativeRelationship2_FK = a.RelativeRelationship2_FK,
                                 //RelativeRelationship_FK = a.RelativeRelationship_FK,
                                 SecondRelativeName = a.SecondRelativeName,
                                 SecondRelativePhoneNumber = a.SecondRelativePhoneNumber,
                                 //FirstRelativeName = a.FirstRelativeName,
                                 //FirstRelativePhoneNumber = a.FirstRelativePhoneNumber,

                             }).OrderByDescending(x => x.ID).ToList();


                if (Loans == null)
                {
                    return null;
                }
                return Loans;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

















        public NyscLoanApplication GetRefid(string Refid)
        {
            try
            {

                var resp = (from a in uvDb.NyscLoanApplications where a.RefNumber == Refid select a).FirstOrDefault();

                   if (resp == null)
                    {
                        return null;
                    }
                    else

                        return resp;


                }
           
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        public List<AppLoanss> EditApprovedLoans(int AppstatFk, string userEmail)
        {
            try
            {
                var Loans = (from a in uvDb.NyscLoanApplications
                             join b in uvDb.NigerianStates on a.NyscStateofResidence_FK equals b.ID
                             join c in uvDb.Banks on a.BankCode equals c.Code
                             join d in uvDb.Titles on a.Title_FK equals d.ID
                             join f in uvDb.NigerianStates on a.StateofResidence_FK equals f.ID
                             join g in uvDb.NigerianStates on a.TempStateofResidence_FK equals g.ID
                             join h in uvDb.NigerianStates on a.NyscStateofResidence_FK equals h.ID
                             join j in uvDb.LGAs on a.TempLGA_FK equals j.ID
                             join k in uvDb.LGAs on a.NyscLGA_FK equals k.ID
                             join m in uvDb.LGAs on a.LGA_FK equals m.ID

                             //join l in uvDb.NYSCRelatives on a.RelativeRelationship_FK equals l.ID

                             where a.NYSCApplicationStatus_FK == AppstatFk && a.IsVisible == 1 && a.EmailAddress == userEmail
                           
                             select new AppLoanss
                             {
                                 ID = a.ID,
                                 ApplicationStatus = b.Name,
                                 AccountName = a.AccountName,
                                 Title = d.Name,
                                 StateofResidence = b.Name,
                                 StateCode = a.StateCode,

                                 TempStateofResidence = f.Name,
                                 NyscStateofResidence = b.Name,
                                 LGAs = m.Name,
                                 
                                 Gender = a.Gender_FK == 1 ? "Male" : "Female",
                                 LoanRefNumber = a.RefNumber,
                                 MaritalStatus = d.Name,

                                 NetMonthlyIncome = a.NetMonthlyIncome.Value,
                                 AccountNumber = a.AccountNumber,
                                 ReferralCode = a.ReferralCode,
                                 BankCode = c.Name,
                                 BVN = a.BVN,
                                 ClosestBusStop = a.ClosestBusStop,
                                 PermanentAddress = a.PermanentAddress,
                                 DateOfBirth = a.DateOfBirth,
                                 EmailAddress = a.EmailAddress,
                                 ExistingLoan = a.ExistingLoan.HasValue ? a.ExistingLoan.Value : false,
                                 ExistingLoan_NoOfMonthsLeft = a.ExistingLoan_NoOfMonthsLeft.Value,
                                 ExistingLoan_OutstandingAmount = a.ExistingLoan_OutstandingAmount.Value > 0 ? a.ExistingLoan_OutstandingAmount.Value : 0,
                                 Firstname = a.Firstname,

                                 LoanAmount = a.LoanAmount.Value,
                                 PPA_Department = a.PPA_Department,
                                 PPA_supervisorPhonenumber = a.PPA_supervisorPhonenumber,
                                 PPA_supervisorName = a.PPA_supervisorName,
                                 PPA_supervisorEmail = a.PPA_supervisorEmail,
                                 PPA_ROle = a.PPA_ROle,
                                 PPA_PhoneNumber = a.PPA_PhoneNumber,
                                 PPA_EmailAddress=a.PPA_EmailAddress,
                                 LoanTenure = a.LoanTenure.Value,
                                 EMG_EmailAddress = a.EMG_EmailAddress,
                                 EMG_FullName = a.EMG_FullName,
                                 EMG_HomeAddress = a.EMG_HomeAddress,
                                 EMG_PhoneNumber = a.EMG_PhoneNumber,
                                 EMG_Relationship = a.EMG_Relationship,
                                 EMG_EmailAddress2 = a.EMG_EmailAddress2,
                                 EMG_FullName2 = a.EMG_FullName2,
                                 EMG_HomeAddress2 = a.EMG_HomeAddress2,
                                 EMG_PhoneNumber2 = a.EMG_PhoneNumber2,
                                 EMG_Relationship2 = a.EMG_Relationship2,
                                 TemporaryAddress = a.TemporaryAddress,
                                 TempClosestBusStop = a.TempClosestBusStop,
                                 TempLandmark = a.TempLandmark,
                                 PhoneNumber = a.PhoneNumber,
                                 ValueDate = a.ValueDate,
                                 ValueTime = a.ValueTime,
                                 Landmark = a.Landmark,
                                 Othernames = a.Othernames,
                                 Surname = a.Surname,
                                TempLGAs = j.Name,
                                 NyscLGAs = k.Name,
                                PassOutMonth = a.PassOutMonth,
                                 OfficialAddress = a.OfficialAddress,
                                 Employer = a.Employer,
                                 CDSDay = a.CDSDay,
                                 CDSGroup = a.CDSGroup,
                                 SalaryAmount = (double)a.NetMonthlyIncome,
                                 InstagramHandle = a.InstagramHandle,
                                 FacebookName = a.FacebookName,
                                 TwitterHandle= a.TwitterHandle,
                                 NyscCallUpLetterFilePath = a.NyscCallUpLetterFilePath,
                                 NyscIdCardFilePath = a.NyscIdCardFilePath,
                                 NyscpassportFilePath = a.NyscpassportFilePath,
                                 NyscPostingLetterFllePath = a.NyscPostingLetterFllePath,
                                 NyscProfileDashboardFilePath = a.NyscProfileDashboardFilePath,
                                 FirstRelativeName = a.FirstRelativeName,
                                 FirstRelativePhoneNumber = a.FirstRelativePhoneNumber,
                                  SecondRelativeName = a.SecondRelativeName,
                                    SecondRelativePhoneNumber = a.SecondRelativePhoneNumber,
                                 RelativeRelationship2_FK = a.RelativeRelationship2_FK,
                                 RelativeRelationship_FK = a.RelativeRelationship_FK,

                             }).OrderByDescending(x => x.ID).ToList();


                if (Loans == null)
                {
                    return null;
                }
                return Loans;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }
        //public AppLoanss DisburseLoan(int AppstatFk)
        //{
        //    try
        //    {
        //        var AppLoan = (from a in uvDb.NyscLoanApplications

        //                       join c in uvDb.Titles on a.Title_FK equals c.ID
        //                       join d in uvDb.MaritalStatus on a.MaritalStatus_FK equals d.ID
        //                       //join e in uvDb.MeansOfIdentifications on a.MeansOfID_FK equals e.ID
        //                       join f in uvDb.NigerianStates on a.StateofResidence_FK equals f.ID
        //                       join g in uvDb.NigerianStates on a.TempStateofResidence_FK equals g.ID
        //                       join h in uvDb.NigerianStates on a.NyscStateofResidence_FK equals h.ID
        //                       join i in uvDb.LGAs on a.LGA_FK equals i.ID
        //                       join j in uvDb.LGAs on a.TempLGA_FK equals j.ID
        //                       join k in uvDb.LGAs on a.NyscLGA_FK equals k.ID
        //                       //join h in uvDb.LoanProducts on a.LoanProduct_FK equals h.LoanType_FK
        //                       join l in uvDb.Banks on a.BankCode equals l.Code


        //                       //join m in uvDb.EmployerLoanDetails on a.ID equals m.LoanApplication_FK
        //                       //join x in uvDb.RepaymentMethods on a.RepaymentMethod_FK equals x.ID
        //                       // join es in uvDb.EmploymentStatus on m.EmploymentStatus_FK equals es.ID
        //                       where a.ApplicationStatus_FK == AppstatFk
        //                       select new AppLoanss
        //                       {
        //                           ID = a.ID,
        //                           // MeansOfIdentifications = e.Name,
        //                           AccountName = a.AccountName,
        //                           Title = c.Name,
        //                           StateofResidence = f.Name,
        //                           StateCode = a.StateCode,

        //                           TempStateofResidence = g.Name,
        //                           NyscStateofResidence = h.Name,
        //                           LGAs = g.Name,
        //                           Gender = a.Gender_FK == 1 ? "Male" : "Female",
        //                           LoanRefNumber = a.RefNumber,
        //                           MaritalStatus = d.Name,
        //                           //LoanProduct = h.LoanProduct1,
        //                           //AppStat = (int)a.ApplicationStatus_FK,
        //                           NetMonthlyIncome = a.NetMonthlyIncome.Value,
        //                           AccountNumber = a.AccountNumber,
        //                           // ApplicantID = a.ApplicantID,
        //                           BankCode = l.Name,
        //                           BVN = a.BVN,
        //                           ClosestBusStop = a.ClosestBusStop,
        //                           PermanentAddress = a.PermanentAddress,
        //                           DateOfBirth = a.DateOfBirth,
        //                           EmailAddress = a.EmailAddress,
        //                           ExistingLoan = a.ExistingLoan.HasValue ? a.ExistingLoan.Value : false,
        //                           ExistingLoan_NoOfMonthsLeft = a.ExistingLoan_NoOfMonthsLeft.Value,
        //                           ExistingLoan_OutstandingAmount = a.ExistingLoan_OutstandingAmount.Value > 0 ? a.ExistingLoan_OutstandingAmount.Value : 0,
        //                           Firstname = a.Firstname,
        //                           // IdentficationNumber = a.IdentficationNumber,
        //                           LoanAmount = a.LoanAmount.Value,
        //                           // Organization = a.Organization,
        //                           // Designation = m.Designation,
        //                           LoanTenure = a.LoanTenure.Value,
        //                           EMG_EmailAddress = a.EMG_EmailAddress,
        //                           EMG_FullName = a.EMG_FullName,
        //                           EMG_HomeAddress = a.EMG_HomeAddress,
        //                           EMG_PhoneNumber = a.EMG_PhoneNumber,
        //                           EMG_Relationship = a.EMG_Relationship,
        //                           TemporaryAddress = a.TemporaryAddress,
        //                           TempClosestBusStop = a.TempClosestBusStop,
        //                           TempLandmark = a.TempLandmark,
        //                           PhoneNumber = a.PhoneNumber,
        //                           ValueDate = a.ValueDate,
        //                           ValueTime = a.ValueTime,
        //                           Landmark = a.Landmark,
        //                           Othernames = a.Othernames,
        //                           Surname = a.Surname,
        //                           TempLGAs = j.Name,
        //                           NyscLGAs = k.Name,
        //                           PassOutMonth = a.PassOutMonth,
        //                           OfficialAddress = a.OfficialAddress,
        //                           Employer = a.Employer,
        //                           CDSDay = a.CDSDay,
        //                           CDSGroup = a.CDSGroup,
        //                           SalaryAmount = (double)a.NetMonthlyIncome,

        //                           //Occupation = m.Occupation,
        //                           // Department = m.Department,
        //                           //  Repayment = x.Name,
        //                           // LoanComment = a.LoanComment,
        //                           //EmployeeStatus = es.Name,
        //                       }).FirstOrDefault();

        //        if (AppLoan == null)
        //        {

        //            return null;
        //        }

        //        return AppLoan;

        //    }
        //    catch (Exception ex)
        //    {
        //        WebLog.Log(ex.Message.ToString());
        //        return null;
        //    }
        //}





        public List<NyscLoanApplication> DisburseLoan(int AppstatFk)
        {
            try
            {
                var Loans = (from a in uvDb.NyscLoanApplications where a.NYSCApplicationStatus_FK == AppstatFk select a).OrderByDescending(x => x.ID).ToList();

                if (Loans == null)
                {
                    return null;
                }
                return Loans;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        public int UpdateLoanApplication(NyscLoanApplication LoanApp)
        {
            try
            {

                var original = uvDb.NyscLoanApplications.Find(LoanApp.RefNumber);


                if (original != null)
                {

                    original.NYSCApplicationStatus_FK = LoanApp.NYSCApplicationStatus_FK;
                    // original.LoanComment = LoanApp.LoanComment; 
                    original.DateModified = LoanApp.DateModified;

                    uvDb.SaveChanges();
                }
                return original.ID;

            }


            catch (Exception ex)
            {

                WebLog.Log(ex.Message.ToString());
                return 0;
            }

        }
        public int insertMarketChannels(MarketingDetail MC)
        {
            try
            {

                uvDb.MarketingDetails.Add(MC);
                uvDb.SaveChanges();
                return MC.ID;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return 0;
            }
        }

        public int inserRepaymentAmount(NYSCLoanLedger NL)
        {
            try
            {

                uvDb.NYSCLoanLedgers.Add(NL);
                uvDb.SaveChanges();
                return NL.ID;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return 0;
            }
        }





        public List<NYSCLoanSetUp> GetTenureByPassoutMonth(int Tenure)
        {
            try
            {
                int myTenure = 0;
                // var localtime = MyUtility.getCurrentLocalDateTime();
                var localtime = DateTime.Now;
                int _today = Convert.ToUInt16(localtime.ToString("dd"));
                //myTenure = _today > 22 ? Tenure - 1 : myTenure;
                myTenure = _today > 22 ? Tenure - 1 : Tenure;
                var LoanTenures = (from a in uvDb.NYSCLoanSetUps where a.Tenure <= myTenure select a).OrderByDescending(x => x.ID).ToList();

                if (LoanTenures == null)
                {
                    return null;
                }
                return LoanTenures;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        //public List<NYSCLoanSetUp> GetTenureByPassoutMonth(int Tenure)
        //{
        //    try
        //    {
        //        var LoanTenures = (from a in uvDb.NYSCLoanSetUps where a.Tenure <= Tenure select a).OrderByDescending(x => x.ID).ToList();

        //        if (LoanTenures == null)
        //        {
        //            return null;
        //        }
        //        return LoanTenures;
        //    }
        //    catch (Exception ex)
        //    {
        //        WebLog.Log(ex.Message.ToString());
        //        return null;
        //    }
        //}


        public int insertIntoNYSCReferralLedger(NYSCReferralLedger RL)
        {
            try
            {
                //var valid = (from a in uvDb.NYSCReferralLedgers where a.ReferralCode == RL.ReferralCode select a.ID).FirstOrDefault();

                //if (valid != 0)
                //{
                //    RL.Credit = 50;
                //}
                //else if (valid == 0)
                //{
                //    RL.Credit = 100;
                //}


                uvDb.NYSCReferralLedgers.Add(RL);
                uvDb.SaveChanges();
                return RL.ID;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return 0;
            }
        }


        public int insertIntoloanLedger(NYSCLoanLedger NL)
        {
            try
            {


                uvDb.NYSCLoanLedgers.Add(NL);
                uvDb.SaveChanges();
                return NL.ID;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return 0;
            }
        }

        public PatnerTransactLog insertintoTransactLog(PatnerTransactLog PTL)
        {

            try
            {

                //var valid = (from a in uvDb.PartnersTransLogs where a.RefNum == PTL.RefNum select a).FirstOrDefault();

                //if (valid == null)
                //{
                //    return null;
                //}
                //else 
                //{
                //    return valid;
                //}


                uvDb.PatnerTransactLogs.Add(PTL);
                uvDb.SaveChanges();
                return PTL;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }





        public int UpdateReferralLedger(NyscLoanApplication LoanApp)
        {
            try
            {

                var Refcode = uvDb.NyscLoanApplications.Find(LoanApp.ReferralCode);


                if (Refcode != null)
                {

                    Refcode.ReferralCode = LoanApp.ReferralCode;

                    uvDb.SaveChanges();
                }
                return Refcode.ID;

            }

            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return 0;
            }

        }



        public int UpdateNyscLoanApplications(DataAccessA.DataManager.NyscLoanApplication Apploans)
        {
            try
            {
                var resp = (from a in uvDb.NyscLoanApplications where a.ID == Apploans.ID select a).FirstOrDefault();

                if (resp.ID != 0)
                {
                    resp.NYSCApplicationStatus_FK = Apploans.NYSCApplicationStatus_FK;
                    resp.ID = Apploans.ID;
                    resp.AccountNumber = Apploans.AccountNumber;
                    resp.AccountName = Apploans.AccountName;
                    resp.Firstname = Apploans.Firstname;
                    resp.Othernames = Apploans.Othernames;
                    resp.NYSCApplicationStatus_FK = 1;
                    resp.NyscIdCardFilePath = Apploans.NyscIdCardFilePath;
                    resp.STA_FilePath = Apploans.STA_FilePath;
                    resp.NyscpassportFilePath = Apploans.NyscIdCardFilePath;
                    resp.NyscCallUpLetterFilePath = Apploans.NyscCallUpLetterFilePath;
                    resp.NyscPostingLetterFllePath = Apploans.NyscPostingLetterFllePath;
                    resp.NyscProfileDashboardFilePath = Apploans.NyscProfileDashboardFilePath;
                    resp.FacebookName = Apploans.FacebookName;
                    resp.InstagramHandle = Apploans.InstagramHandle;
                    resp.TwitterHandle = Apploans.TwitterHandle;
                    resp.RepaymentAmount = Apploans.RepaymentAmount;
                    // resp.RefNumber = Apploans.RefNumber;
                    resp.Gender_FK = Apploans.Gender_FK;//Convert.ToInt32(form["selectGender"]),
                    resp.MaritalStatus_FK = Apploans.MaritalStatus_FK;//Convert.ToInt16(form["Marital"]),
                    resp.Surname = Apploans.Surname;
                    //CreatedBy = Convert.ToString(userid),
                    resp.DateOfBirth = Convert.ToString(Apploans.DateOfBirth);
                    resp.Title_FK = Apploans.Title_FK;//Convert.ToInt32(form["Titles"]),
                    resp.PhoneNumber = Apploans.PhoneNumber;
                    resp.EmailAddress = Apploans.EmailAddress;
                    resp.PermanentAddress = Apploans.PermanentAddress;
                    resp.Landmark = Apploans.Landmark;
                    resp.ClosestBusStop = Apploans.ClosestBusStop;
                    resp.LGA_FK = Apploans.LGA_FK;
                    resp.TempLGA_FK = Apploans.TempLGA_FK;
                    resp.NyscLGA_FK = Apploans.NyscLGA_FK;

                    //resp.LGA_FK = Convert.ToInt16(form["lgaList"]);
                    //resp.TempLGA_FK = Convert.ToInt16(form["lgaLists"]);
                    //resp.NyscLGA_FK = Convert.ToInt16(form["lgaListsss"]);
                    resp.StateofResidence_FK = Apploans.StateofResidence_FK;////Convert.ToInt32(form["States"]),
                    resp.TempStateofResidence_FK = Apploans.TempStateofResidence_FK;//Convert.ToInt32(form["States"]),
                    resp.NyscStateofResidence_FK = Apploans.NyscStateofResidence_FK;//Convert.ToInt32(form["States"]),
                    resp.TemporaryAddress = Apploans.TemporaryAddress;
                    resp.OfficialAddress = Apploans.OfficialAddress;
                    resp.StateCode = Apploans.StateCode;
                    resp.Employer = Apploans.Employer;
                    resp.PassOutMonth = Apploans.PassOutMonth;
                    resp.CDSDay = Apploans.CDSDay;
                    resp.TempLandmark = Apploans.TempLandmark;
                    resp.TempClosestBusStop = Apploans.TempClosestBusStop;
                    resp.ReferralCode = Apploans.ReferralCode;
                    resp.BVN = Apploans.BVN;
                    resp.CDSGroup = Apploans.CDSGroup;
                    resp.NetMonthlyIncome = Convert.ToDouble(Apploans.NetMonthlyIncome);
                    resp.EMG_EmailAddress = Apploans.EMG_EmailAddress;
                    resp.EMG_FullName = Apploans.EMG_FullName;
                    resp.EMG_HomeAddress = Apploans.EMG_HomeAddress;
                    resp.EMG_PhoneNumber = Apploans.EMG_PhoneNumber;
                    resp.EMG_Relationship = Apploans.EMG_Relationship;
                    resp.EMG_EmailAddress2 = Apploans.EMG_EmailAddress2;
                    resp.EMG_FullName2 = Apploans.EMG_FullName2;
                    resp.EMG_HomeAddress2 = Apploans.EMG_HomeAddress2;
                    resp.EMG_PhoneNumber2 = Apploans.EMG_PhoneNumber2;
                    resp.EMG_Relationship2 = Apploans.EMG_Relationship2;
                    resp.PPA_Department = Apploans.PPA_Department;
                    resp.PPA_EmailAddress = Apploans.PPA_EmailAddress;
                    resp.PPA_PhoneNumber = Apploans.PPA_PhoneNumber;
                    resp.PPA_ROle = Apploans.PPA_ROle;
                    resp.PPA_supervisorEmail = Apploans.PPA_supervisorEmail;
                    resp.PPA_supervisorName = Apploans.PPA_supervisorName;
                    resp.PPA_supervisorPhonenumber = Apploans.PPA_supervisorPhonenumber;
                    resp.FirstRelativeName = Apploans.FirstRelativeName;
                    resp.FirstRelativePhoneNumber = Apploans.FirstRelativePhoneNumber;
                    resp.RelativeRelationship2_FK = Apploans.RelativeRelationship2_FK;
                    resp.SecondRelativeName = Apploans.SecondRelativeName;
                    resp.SecondRelativePhoneNumber = Apploans.SecondRelativePhoneNumber;
                    resp.RelativeRelationship_FK = Apploans.RelativeRelationship_FK;
                    resp.LoanAmount = Convert.ToDouble(Apploans.LoanAmount);
                    resp.LoanTenure = Apploans.LoanTenure;
                    resp.ExistingLoan = Apploans.ExistingLoan;
                    resp.LoanComment = Apploans.LoanComment;
                    resp.ExistingLoan_NoOfMonthsLeft = Convert.ToInt16(Apploans.ExistingLoan_NoOfMonthsLeft);
                    resp.ExistingLoan_OutstandingAmount = Apploans.ExistingLoan_OutstandingAmount;
                    resp.BankCode = Helper.GetRemitaBankCodeByFlutterCode(Apploans.BankCode);
                    resp.IsVisible = 1;
                    resp.DateCreated = MyUtility.getCurrentLocalDateTime();
                    resp.DateModified = MyUtility.getCurrentLocalDateTime();
                    //ValueDate = MyUtility.getCurrentLocalDateTime().ToString("yyyy/MM/dd"),
                    resp.ValueDate = MyUtility.getCurrentLocalDateTime().ToString("dddd, dd MMMM yyyy");
                    resp.ValueTime = MyUtility.getCurrentLocalDateTime().ToString("H:mmss");
                    resp.MarketingChannel = Apploans.MarketingChannel.ToString();
                    // uvDb.NyscLoanApplications.Add(Apploans);
                    uvDb.SaveChanges();
                }
                return Apploans.ID;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return 0;
            }
        }


        public string UpdateNyscfiles(DataAccessA.Classes.AppLoanss Apploans)
        {
            try
            {
                var resp = (from a in uvDb.NyscLoanApplications where a.RefNumber == Apploans.LoanRefNumber select a).FirstOrDefault();
                if (resp.RefNumber != null)
                {
                    //resp.NyscIdCardFilePath = Apploans.NyscIdCardFilePath;
                    //resp.STA_FilePath = Apploans.STA_FilePath;
                    resp.NyscpassportFilePath = Apploans.NyscpassportFilePath;
                    resp.NyscCallUpLetterFilePath = Apploans.NyscCallUpLetterFilePath;
                    resp.NyscPostingLetterFllePath = Apploans.NyscPostingLetterFllePath;
                    resp.NyscProfileDashboardFilePath = Apploans.NyscProfileDashboardFilePath;
                    resp.LetterOfundertaken = Apploans.LetterOfundertaken;

                    uvDb.SaveChanges();
                }
                return resp.RefNumber;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
               
            }
        }




        //public string UpdateNyscfiles(DataAccessA.DataManager.NyscLoanApplication Apploans)
        //{
        //    try
        //    {
        //        var resp = (from a in uvDb.NyscLoanApplications where a.RefNumber == Apploans.RefNumber select a).FirstOrDefault();

        //        if (resp.ID != null)
        //        {
        //            resp.NYSCApplicationStatus_FK = Apploans.NYSCApplicationStatus_FK;
        //            resp.ID = Apploans.ID;
        //            resp.AccountNumber = Apploans.AccountNumber;
        //            resp.AccountName = Apploans.AccountName;
        //            resp.Firstname = Apploans.Firstname;
        //            resp.Othernames = Apploans.Othernames;
        //            resp.NYSCApplicationStatus_FK = 1;
        //            resp.NyscIdCardFilePath = Apploans.NyscIdCardFilePath;
        //            resp.STA_FilePath = Apploans.STA_FilePath;
        //            resp.NyscpassportFilePath = Apploans.NyscIdCardFilePath;
        //            resp.NyscCallUpLetterFilePath = Apploans.NyscCallUpLetterFilePath;
        //            resp.NyscPostingLetterFllePath = Apploans.NyscPostingLetterFllePath;
        //            resp.NyscProfileDashboardFilePath = Apploans.NyscProfileDashboardFilePath;
        //            resp.FacebookName = Apploans.FacebookName;
        //            resp.InstagramHandle = Apploans.InstagramHandle;
        //            resp.TwitterHandle = Apploans.TwitterHandle;
        //            resp.RepaymentAmount = Apploans.RepaymentAmount;
        //            // resp.RefNumber = Apploans.RefNumber;
        //            resp.Gender_FK = Apploans.Gender_FK;//Convert.ToInt32(form["selectGender"]),
        //            resp.MaritalStatus_FK = Apploans.MaritalStatus_FK;//Convert.ToInt16(form["Marital"]),
        //            resp.Surname = Apploans.Surname;
        //            //CreatedBy = Convert.ToString(userid),
        //            resp.DateOfBirth = Convert.ToString(Apploans.DateOfBirth);
        //            resp.Title_FK = Apploans.Title_FK;//Convert.ToInt32(form["Titles"]),
        //            resp.PhoneNumber = Apploans.PhoneNumber;
        //            resp.EmailAddress = Apploans.EmailAddress;
        //            resp.PermanentAddress = Apploans.PermanentAddress;
        //            resp.Landmark = Apploans.Landmark;
        //            resp.ClosestBusStop = Apploans.ClosestBusStop;
        //            resp.LGA_FK = Apploans.LGA_FK;
        //            resp.TempLGA_FK = Apploans.TempLGA_FK;
        //            resp.NyscLGA_FK = Apploans.NyscLGA_FK;

        //            //resp.LGA_FK = Convert.ToInt16(form["lgaList"]);
        //            //resp.TempLGA_FK = Convert.ToInt16(form["lgaLists"]);
        //            //resp.NyscLGA_FK = Convert.ToInt16(form["lgaListsss"]);
        //            resp.StateofResidence_FK = Apploans.StateofResidence_FK;////Convert.ToInt32(form["States"]),
        //            resp.TempStateofResidence_FK = Apploans.TempStateofResidence_FK;//Convert.ToInt32(form["States"]),
        //            resp.NyscStateofResidence_FK = Apploans.NyscStateofResidence_FK;//Convert.ToInt32(form["States"]),
        //            resp.TemporaryAddress = Apploans.TemporaryAddress;
        //            resp.OfficialAddress = Apploans.OfficialAddress;
        //            resp.StateCode = Apploans.StateCode;
        //            resp.Employer = Apploans.Employer;
        //            resp.PassOutMonth = Apploans.PassOutMonth;
        //            resp.CDSDay = Apploans.CDSDay;
        //            resp.TempLandmark = Apploans.TempLandmark;
        //            resp.TempClosestBusStop = Apploans.TempClosestBusStop;
        //            resp.ReferralCode = Apploans.ReferralCode;
        //            resp.BVN = Apploans.BVN;
        //            resp.CDSGroup = Apploans.CDSGroup;
        //            resp.NetMonthlyIncome = Convert.ToDouble(Apploans.NetMonthlyIncome);
        //            resp.EMG_EmailAddress = Apploans.EMG_EmailAddress;
        //            resp.EMG_FullName = Apploans.EMG_FullName;
        //            resp.EMG_HomeAddress = Apploans.EMG_HomeAddress;
        //            resp.EMG_PhoneNumber = Apploans.EMG_PhoneNumber;
        //            resp.EMG_Relationship = Apploans.EMG_Relationship;
        //            resp.LoanAmount = Convert.ToDouble(Apploans.LoanAmount);
        //            resp.LoanTenure = Apploans.LoanTenure;
        //            resp.ExistingLoan = Apploans.ExistingLoan;
        //            resp.LoanComment = Apploans.LoanComment;
        //            resp.ExistingLoan_NoOfMonthsLeft = Convert.ToInt16(Apploans.ExistingLoan_NoOfMonthsLeft);
        //            resp.ExistingLoan_OutstandingAmount = Apploans.ExistingLoan_OutstandingAmount;
        //            resp.BankCode = Helper.GetRemitaBankCodeByFlutterCode(Apploans.BankCode);
        //            resp.IsVisible = 1;
        //            resp.DateCreated = MyUtility.getCurrentLocalDateTime();
        //            resp.DateModified = MyUtility.getCurrentLocalDateTime();
        //            //ValueDate = MyUtility.getCurrentLocalDateTime().ToString("yyyy/MM/dd"),
        //            resp.ValueDate = MyUtility.getCurrentLocalDateTime().ToString("dddd, dd MMMM yyyy");
        //            resp.ValueTime = MyUtility.getCurrentLocalDateTime().ToString("H:mmss");
        //            resp.MarketingChannel = Apploans.MarketingChannel.ToString();
        //            // uvDb.NyscLoanApplications.Add(Apploans);
        //            uvDb.SaveChanges();
        //        }
        //        return Apploans.RefNumber;
        //    }
        //    catch (Exception ex)
        //    {
        //        WebLog.Log(ex.Message.ToString());
        //        return null;
        //    }
        //}



        public string UpdateNyscLoanApplication(DataAccessA.Classes.AppLoanss Apploans)
        {
            try
            {
                var resp = (from a in uvDb.NyscLoanApplications where a.ID == Apploans.ID select a).FirstOrDefault();

                if (resp.ID != 0)
                {
                    resp.NYSCApplicationStatus_FK = Apploans.ApplicationStatus_FK;
                    uvDb.SaveChanges();
                }
                return resp.RefNumber;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        public string UpdateNyscLoanApplication4Serial(DataAccessA.Classes.AppLoanss Apploans)
        {
            try
            {
                var localTime = MyUtility.getCurrentLocalDateTime();
                int serialNo = 1;
                var sObj = uvDb.LoanSerialNoes.FirstOrDefault();
                if (sObj != null)
                {
                    serialNo = Convert.ToInt16(sObj.NYSCSerialNo) + 1;
                }


                string serialNumb = "";
                if (serialNo.ToString().Length == 1)
                {
                    serialNumb = "00000" + serialNo.ToString();
                }
                else if (serialNo.ToString().Length == 2)
                {
                    serialNumb = "0000" + serialNo.ToString();
                }
                else if (serialNo.ToString().Length == 3)
                {
                    serialNumb = "000" + serialNo.ToString();
                }
                else if (serialNo.ToString().Length == 4)
                {
                    serialNumb = "00" + serialNo.ToString();
                }
                else if (serialNo.ToString().Length == 5)
                {
                    serialNumb = "0" + serialNo.ToString();
                }
                else
                {
                    serialNumb = serialNo.ToString();
                }
                var resp = (from a in uvDb.NyscLoanApplications where a.ID == Apploans.ID select a).FirstOrDefault();

                if (resp.ID != 0)
                {
                    resp.NYSCApplicationStatus_FK = Apploans.ApplicationStatus_FK;
                    resp.LoanSerialNo = "NY-" + localTime.ToString("yy") + serialNumb;
                    resp.CustomerNo = localTime.ToString("yyyymmddhhmmss");
                    uvDb.SaveChanges();
                }
                return resp.RefNumber;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }



        //public List<AppLoanss> getCommentByUser(int AppID)
        //{
        //    try
        //    {
        //        var Comment = (from a in uvDb.LoanApprovals
        //                           //join u in uvDb.LoanApprovals on e.ID equals u.CommentBy
        //                       where a.LoanApplication_FK == AppID
        //                       select new AppLoanss
        //                       {
        //                           ID = a.ID,
        //                           LoanComment = a.Comment,
        //                           Firstname = a.CommentBy,
        //                           ValueDate = a.ValueDate,

        //                       }).ToList();


        //        if (Comment == null)
        //        {
        //            return null;
        //        }

        //        return Comment.ToList();

        //    }
        //    catch (Exception ex)
        //    {
        //        WebLog.Log(ex.Message.ToString());
        //        return null;
        //    }
        //}

        public class getcomment
        {
            public string comment { get; set; }
            public string commentBy { get; set; }
            public string ValueDate { get; set; }
        }

        public List<getcomment> getCommentByUser(int userfk, int loanNumfk)
        {
            try
            {
                List<getcomment> comments = new List<getcomment>();
                //&& a.CommentBy == userfk
                comments = (from a in uvDb.NYSCLoanApprovals
                            join b in uvDb.Users on a.CommentBy
equals b.ID
                            where a.LoanApplication_FK == loanNumfk
                            select new getcomment
                            {
                                comment = a.Comment,
                                commentBy = b.EmailAddress,
                                ValueDate = a.ValueDate
                            }).OrderByDescending(x => x.ValueDate).ToList();

                if (comments == null)
                {
                    return null;
                }

                return comments.ToList();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }



        public AppLoanss GetNYSCLoanApplication(string LoanID, int AppstatFk)
        {
            try
            {
                var AppLoan = (from a in uvDb.NyscLoanApplications

                               join c in uvDb.Titles on a.Title_FK equals c.ID
                               join d in uvDb.MaritalStatus on a.MaritalStatus_FK equals d.ID
                               
                               join f in uvDb.NigerianStates on a.StateofResidence_FK equals f.ID
                               join g in uvDb.NigerianStates on a.TempStateofResidence_FK equals g.ID
                               join h in uvDb.NigerianStates on a.NyscStateofResidence_FK equals h.ID
                               join i in uvDb.LGAs on a.LGA_FK equals i.ID
                               join j in uvDb.LGAs on a.TempLGA_FK equals j.ID
                               join k in uvDb.LGAs on a.NyscLGA_FK equals k.ID
                              
                               join l in uvDb.Banks on a.BankCode equals l.Code

                               // join m in uvDb.LoanApprovals on a.ID equals m.LoanApplication_FK
                               //join m in uvDb.
                               //join m in uvDb.EmployerLoanDetails on a.ID equals m.LoanApplication_FK
                               //join x in uvDb.RepaymentMethods on a.RepaymentMethod_FK equals x.ID
                               // join es in uvDb.EmploymentStatus on m.EmploymentStatus_FK equals es.ID
                               where a.RefNumber == LoanID && a.NYSCApplicationStatus_FK == AppstatFk
                               select new AppLoanss
                               {
                                   ID = a.ID,
                                   // MeansOfIdentifications = e.Name,
                                   AccountName = a.AccountName,
                                   Title = c.Name,
                                   StateofResidence = f.Name,
                                   StateCode = a.StateCode,
                                   ApplicationStatus_FK = AppstatFk,//(int)a.NYSCApplicationStatus_FK,
                                   NYSCApplicationStatus_FK = AppstatFk,//(int)a.NYSCApplicationStatus_FK,
                                   TempStateofResidence = g.Name,
                                   NyscStateofResidence = h.Name,
                                   LGAs = i.Name,
                                   Gender = a.Gender_FK == 1 ? "Male" : "Female",
                                   LoanRefNumber = a.RefNumber,
                                   MaritalStatus = d.Name,
                                   //LoanProduct = h.LoanProduct1,
                                   //AppStat = (int)a.ApplicationStatus_FK,
                                   NetMonthlyIncome = a.NetMonthlyIncome.Value,
                                   AccountNumber = a.AccountNumber,
                                   // ApplicantID = a.ApplicantID,
                                   BankCode = l.Name,
                                   bankcodes = l.Code,
                                   BVN = a.BVN,
                                   ClosestBusStop = a.ClosestBusStop,
                                   PermanentAddress = a.PermanentAddress,
                                   DateOfBirth = a.DateOfBirth,
                                   EmailAddress = a.EmailAddress,
                                   ExistingLoan = a.ExistingLoan.HasValue ? a.ExistingLoan.Value : false,
                                   ExistingLoan_NoOfMonthsLeft = a.ExistingLoan_NoOfMonthsLeft.Value,
                                   ExistingLoan_OutstandingAmount = a.ExistingLoan_OutstandingAmount.Value > 0 ? a.ExistingLoan_OutstandingAmount.Value : 0,
                                   Firstname = a.Firstname,
                                   // IdentficationNumber = a.IdentficationNumber,
                                   LoanAmount = a.LoanAmount.Value,
                                   // Organization = a.Organization,
                                   // Designation = m.Designation,
                                   LoanTenure = a.LoanTenure.Value,
                                   EMG_EmailAddress = a.EMG_EmailAddress,
                                   EMG_FullName = a.EMG_FullName,
                                   EMG_HomeAddress = a.EMG_HomeAddress,
                                   EMG_PhoneNumber = a.EMG_PhoneNumber,
                                   EMG_Relationship = a.EMG_Relationship,
                                   EMG_EmailAddress2 = a.EMG_EmailAddress2,
                                   EMG_FullName2 = a.EMG_FullName2,
                                   EMG_HomeAddress2 = a.EMG_HomeAddress2,
                                   EMG_PhoneNumber2 = a.EMG_PhoneNumber2,
                                   EMG_Relationship2 = a.EMG_Relationship2,
                                   TemporaryAddress = a.TemporaryAddress,
                                   TempClosestBusStop = a.TempClosestBusStop,
                                   TempLandmark = a.TempLandmark,
                                   PhoneNumber = a.PhoneNumber,
                                   ValueDate = a.ValueDate,
                                   ValueTime = a.ValueTime,
                                   Landmark = a.Landmark,
                                   Othernames = a.Othernames,
                                   Surname = a.Surname,
                                   TempLGAs = j.Name,
                                   NyscLGAs = k.Name,
                                   PassOutMonth = a.PassOutMonth,
                                   OfficialAddress = a.OfficialAddress,
                                   Employer = a.Employer,
                                   CDSDay = a.CDSDay,
                                   CDSGroup = a.CDSGroup,
                                   SalaryAmount = (double)a.NetMonthlyIncome,

                                  
                                   PPA_Department = a.PPA_Department,
                                   PPA_EmailAddress= a.PPA_EmailAddress,
                                   PPA_PhoneNumber =a.PPA_PhoneNumber,
                                   PPA_ROle =a.PPA_ROle,
                                   PPA_supervisorEmail=a.PPA_supervisorEmail,
                                   PPA_supervisorName=a.PPA_supervisorName,
                                   PPA_supervisorPhonenumber=a.PPA_supervisorPhonenumber,
                                   
                                   FacebookName = a.FacebookName,
                                   TwitterHandle = a.TwitterHandle,
                                   InstagramHandle = a.InstagramHandle,
                                   NyscIdCardFilePath = a.NyscIdCardFilePath,
                                   STA_FilePath = a.STA_FilePath,
                                   NyscpassportFilePath = a.NyscpassportFilePath,
                                   NyscCallUpLetterFilePath = a.NyscCallUpLetterFilePath,
                                   NyscPostingLetterFllePath = a.NyscPostingLetterFllePath,
                                   NyscProfileDashboardFilePath = a.NyscProfileDashboardFilePath,
                                   LetterOfundertaken = a.LetterOfundertaken,
                                   ReferralCode = a.ReferralCode,
                                   RelativeRelationship2_FK = a.RelativeRelationship2_FK,
                                   RelativeRelationship_FK =a.RelativeRelationship_FK,
                                   SecondRelativeName= a.SecondRelativeName,
                                   SecondRelativePhoneNumber=a.SecondRelativePhoneNumber,
                                   FirstRelativeName =a.FirstRelativeName,
                                   FirstRelativePhoneNumber = a.FirstRelativePhoneNumber,
                                   
                               }).FirstOrDefault();

                if (AppLoan == null)
                {

                    return null;
                }

                return AppLoan;

            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


        public dynamic LoanTransactionbyDate(DateTime to, DateTime from)
        {
            try
            {
                var Apploan = uvDb.LoanTransactionbyDate(from, to).ToList();

                if (Apploan == null)
                {

                    return null;
                }

                return Apploan;

            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


        public LoanInterestRate GetInterestRate(int tenure)
        {
            try
            {

                var IntRate = (from a in uvDb.LoanInterestRates
                               where a.LoanTenure == tenure
                               select a).FirstOrDefault();
                if (IntRate == null)
                {

                    return null;
                }

                return IntRate;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.StackTrace);
                return null;
            }
        }



        public dynamic CheckLoanStatus(string AppStatusFk)
        {
            try
            {
                var AppLoan = (from a in uvDb.LoanApplications
                               join b in uvDb.StudentRecords
                               on a.IdentficationNumber equals b.MatriculationNumber
                               join c in uvDb.Institutions on b.Institution_FK equals c.InstitutionType_FK
                               join d in uvDb.ApplicationStatus on a.ApplicationStatus_FK equals d.ID

                               where a.LoanRefNumber == AppStatusFk
                               select new AppLoans
                               {
                                   Institutions = c.Name,
                                   MatriculationNumber = b.MatriculationNumber,
                                   Surname = a.Surname,
                                   Firstname = a.Firstname,
                                   LoanAmount = a.LoanAmount.Value,
                                   LoanRefNumber = a.LoanRefNumber,
                                   ApplicationStatus = d.Description,

                               }).OrderByDescending(x => x.ID).ToList();

                if (AppLoan == null)
                {

                    return null;
                }

                return AppLoan;

            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


        public AppLoanss CheckAppStatus(string AppstatFk)
        {
            try
            {
                var AppLoan = (from a in uvDb.NyscLoanApplications

                               join c in uvDb.Titles on a.Title_FK equals c.ID
                               join d in uvDb.MaritalStatus on a.MaritalStatus_FK equals d.ID
                               join e in uvDb.NYSCApplicationStatus on a.NYSCApplicationStatus_FK equals e.ID
                               join f in uvDb.NigerianStates on a.StateofResidence_FK equals f.ID
                               join g in uvDb.NigerianStates on a.TempStateofResidence_FK equals g.ID
                               join h in uvDb.NigerianStates on a.NyscStateofResidence_FK equals h.ID
                               join i in uvDb.LGAs on a.LGA_FK equals i.ID
                               join j in uvDb.LGAs on a.TempLGA_FK equals j.ID
                               join k in uvDb.LGAs on a.NyscLGA_FK equals k.ID
                               //join h in uvDb.LoanProducts on a.LoanProduct_FK equals h.LoanType_FK
                               join l in uvDb.Banks on a.BankCode equals l.Code


                               //join m in uvDb.EmployerLoanDetails on a.ID equals m.LoanApplication_FK
                               //join x in uvDb.RepaymentMethods on a.RepaymentMethod_FK equals x.ID
                               // join es in uvDb.EmploymentStatus on m.EmploymentStatus_FK equals es.ID
                               where a.RefNumber == AppstatFk
                               select new AppLoanss
                               {
                                   ID = a.ID,
                                   // MeansOfIdentifications = e.Name,
                                   AccountName = a.AccountName,
                                   Title = c.Name,
                                   StateofResidence = f.Name,
                                   StateCode = a.StateCode,
                                   RepaymentAmount = a.LoanTenure.ToString(),
                                   TempStateofResidence = g.Name,
                                   NyscStateofResidence = h.Name,
                                   LGAs = g.Name,
                                   Gender = a.Gender_FK == 1 ? "Male" : "Female",
                                   LoanRefNumber = a.RefNumber,
                                   MaritalStatus = d.Name,
                                   //LoanProduct = h.LoanProduct1,
                                   AppStat = e.Name,
                                   AccountNumber = a.AccountNumber,
                                   // ApplicantID = a.ApplicantID,
                                   BankCode = l.Name,
                                   BVN = a.BVN,
                                   ClosestBusStop = a.ClosestBusStop,
                                   PermanentAddress = a.PermanentAddress,
                                   DateOfBirth = a.DateOfBirth,
                                   EmailAddress = a.EmailAddress,
                                   ExistingLoan = a.ExistingLoan.HasValue ? a.ExistingLoan.Value : false,
                                   ExistingLoan_NoOfMonthsLeft = a.ExistingLoan_NoOfMonthsLeft.Value,
                                   ExistingLoan_OutstandingAmount = a.ExistingLoan_OutstandingAmount.Value > 0 ? a.ExistingLoan_OutstandingAmount.Value : 0,
                                   Firstname = a.Firstname,
                                   // IdentficationNumber = a.IdentficationNumber,
                                   LoanAmount = a.LoanAmount.Value,
                                   // Organization = a.Organization,
                                   // Designation = m.Designation,
                                   LoanTenure = a.LoanTenure.Value,
                                   EMG_EmailAddress = a.EMG_EmailAddress,
                                   EMG_FullName = a.EMG_FullName,
                                   EMG_HomeAddress = a.EMG_HomeAddress,
                                   EMG_PhoneNumber = a.EMG_PhoneNumber,
                                   EMG_Relationship = a.EMG_Relationship,
                                   TemporaryAddress = a.TemporaryAddress,
                                   TempClosestBusStop = a.TempClosestBusStop,
                                   TempLandmark = a.TempLandmark,
                                   PhoneNumber = a.PhoneNumber,
                                   ValueDate = a.ValueDate,
                                   ValueTime = a.ValueTime,
                                   Landmark = a.Landmark,
                                   Othernames = a.Othernames,
                                   Surname = a.Surname,
                                   TempLGAs = j.Name,
                                   NyscLGAs = k.Name,
                                   PassOutMonth = a.PassOutMonth,
                                   OfficialAddress = a.OfficialAddress,
                                   Employer = a.Employer,
                                   CDSDay = a.CDSDay,
                                   CDSGroup = a.CDSGroup,
                                   SalaryAmount = (double)a.NetMonthlyIncome,

                                   //Occupation = m.Occupation,
                                   // Department = m.Department,
                                   //  Repayment = x.Name,
                                   // LoanComment = a.LoanComment,
                                   //EmployeeStatus = es.Name,
                               }).FirstOrDefault();

                if (AppLoan == null)
                {

                    return null;
                }

                return AppLoan;

            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }



        //public dynamic CheckAppStatus(string AppStatusFk)
        //{
        //    try
        //    {
        //        var AppLoan = (from a in uvDb.NyscLoanApplications
        //                       join b in uvDb.ApplicationStatus
        //                       on a.ApplicationStatus_FK equals b.ID
        //                       join c in uvDb.Titles on a.Title_FK equals c.ID
        //                       join d in uvDb.MaritalStatus on a.MaritalStatus_FK equals d.ID

        //                       join f in uvDb.NigerianStates on a.StateofResidence_FK equals f.ID
        //                       join g in uvDb.LGAs on a.LGA_FK equals g.ID

        //                       join i in uvDb.Banks on a.BankCode equals i.Code
        //                       //join j in uvDb.StudentRecords on a.Institution_FK equals j.Institution_FK
        //                       //join k in uvDb.Institutions on a.Institution_FK equals k.InstitutionType_FK
        //                       where a.RefNumber == AppStatusFk
        //                       select new AppLoans
        //                       {
        //                           //Institutions = k.Name,
        //                           //MatriculationNumber = j.MatriculationNumber,

        //                           AccountName = a.AccountName,
        //                           Title = c.Name,
        //                           NigerianStates = f.Name,
        //                           LGAs = g.Name,
        //                           Gender = a.Gender_FK == 1 ? "Male" : "Female",
        //                           LoanRefNumber = a.RefNumber,
        //                           MaritalStatus = d.Name,

        //                           ApplicationStatus = b.Description,
        //                           AccountNumber = a.AccountNumber,

        //                           BankCode = i.Name,
        //                           BVN = a.BVN,
        //                           ClosestBusStop = a.ClosestBusStop,
        //                           ContactAddress = a.ContactAddress,
        //                           DateOfBirth = a.DateOfBirth,
        //                           EmailAddress = a.EmailAddress,
        //                           ExistingLoan = a.ExistingLoan.Value,
        //                           ExistingLoan_NoOfMonthsLeft = a.ExistingLoan_NoOfMonthsLeft.Value,
        //                           ExistingLoan_OutstandingAmount = a.ExistingLoan_OutstandingAmount.Value,
        //                           Firstname = a.Firstname,

        //                           LoanAmount = a.LoanAmount.Value,
        //                           LoanTenure = a.LoanTenure.Value,
        //                           NOK_EmailAddress = a.EMG_EmailAddress,
        //                           NOK_FullName = a.EMG_FullName,
        //                           NOK_HomeAddress = a.EMG_HomeAddress,
        //                           NOK_PhoneNumber = a.EMG_PhoneNumber,
        //                           NOK_Relationship = a.EMG_Relationship,

        //                           PhoneNumber = a.PhoneNumber,
        //                           ValueDate = a.ValueDate,
        //                           Landmark = a.Landmark,
        //                           Othernames = a.Othernames,
        //                           Surname = a.Surname
        //                           }).FirstOrDefault();

        //        if (AppLoan == null)
        //        {

        //            return null;
        //        }

        //        return AppLoan;

        //    }
        //    catch (Exception ex)
        //    {
        //        WebLog.Log(ex.Message.ToString());
        //        return null;
        //    }
        //}


        //public AppLoans CheckAppStatuss(string AppStatusFk)
        //{
        //    try
        //    {
        //        var AppLoan = (from a in uvDb.LoanApplications
        //                       join b in uvDb.ApplicationStatus
        //                       on a.ApplicationStatus_FK equals b.ID
        //                       join c in uvDb.Titles on a.Title_FK equals c.ID
        //                       join d in uvDb.MaritalStatus on a.MaritalStatus_FK equals d.ID
        //                       join e in uvDb.MeansOfIdentifications on a.MeansOfID_FK equals e.ID
        //                       join f in uvDb.NigerianStates on a.StateofResidence_FK equals f.ID
        //                       join g in uvDb.LGAs on a.LGA_FK equals g.ID
        //                       join h in uvDb.LoanProducts on a.LoanProduct_FK equals h.LoanType_FK
        //                       join i in uvDb.Banks on a.BankCode equals i.Code
        //                       //join j in uvDb.StudentRecords on a.Institution_FK equals j.Institution_FK
        //                       //join k in uvDb.Institutions on a.Institution_FK equals k.InstitutionType_FK
        //                       where a.LoanRefNumber == AppStatusFk
        //                       select new AppLoans
        //                       {
        //                           //Institutions = k.Name,
        //                           //MatriculationNumber = j.MatriculationNumber,
        //                           MeansOfIdentifications = e.Name,
        //                           AccountName = a.AccountName,
        //                           Title = c.Name,
        //                           NigerianStates = f.Name,
        //                           LGAs = g.Name,
        //                           Gender = a.Gender_FK == 1 ? "Male" : "Female",
        //                           LoanRefNumber = a.LoanRefNumber,
        //                           MaritalStatus = d.Name,
        //                           LoanProduct = h.LoanProduct1,
        //                           ApplicationStatus = b.Description,
        //                           AccountNumber = a.AccountNumber,
        //                           ApplicantID = a.ApplicantID,
        //                           BankCode = i.Name,
        //                           BVN = a.BVN,
        //                           ClosestBusStop = a.ClosestBusStop,
        //                           ContactAddress = a.ContactAddress,
        //                           DateOfBirth = a.DateOfBirth,
        //                           EmailAddress = a.EmailAddress,
        //                           ExistingLoan = a.ExistingLoan.Value,
        //                           ExistingLoan_NoOfMonthsLeft = a.ExistingLoan_NoOfMonthsLeft.Value,
        //                           ExistingLoan_OutstandingAmount = a.ExistingLoan_OutstandingAmount.Value,
        //                           Firstname = a.Firstname,
        //                           IdentficationNumber = a.IdentficationNumber,
        //                           LoanAmount = a.LoanAmount.Value,
        //                           LoanTenure = a.LoanTenure.Value,
        //                           NOK_EmailAddress = a.NOK_EmailAddress,
        //                           NOK_FullName = a.NOK_FullName,
        //                           NOK_HomeAddress = a.NOK_HomeAddress,
        //                           NOK_PhoneNumber = a.NOK_PhoneNumber,
        //                           NOK_Relationship = a.NOK_Relationship,
        //                           Organization = a.Organization,
        //                           PhoneNumber = a.PhoneNumber,
        //                           ValueDate = a.ValueDate,
        //                           Landmark = a.Landmark,
        //                           Othernames = a.Othernames,
        //                           Surname = a.Surname,

        //                       }).FirstOrDefault();

        //        if (AppLoan == null)
        //        {

        //            return null;
        //        }


        //        return AppLoan;

        //    }
        //    catch (Exception ex)
        //    {
        //        WebLog.Log(ex.Message.ToString());

        //        return null;
        //    }
        //}

        //public List<ExcelList> RejectedloanReport(int AppStatFk)

        //{
        //    try
        //    {




        //        var Loans = (from a in uvDb.NyscLoanApplications
        //                     join b in uvDb.NigerianStates on a.NyscStateofResidence_FK equals b.ID
        //                     join c in uvDb.NYSCApplicationStatus on a.NYSCApplicationStatus_FK equals c.ID
        //                     join d in uvDb.PartnersTransLogs on a.RefNumber equals d.RefNum
        //                     where a.NYSCApplicationStatus_FK == AppStatFk && a.IsVisible == 1
        //                     select new ExcelList
        //                     {
        //                         ID = a.ID,
        //                         ApplicationStatus = c.Name,
        //                         AccountName = a.AccountName,

        //                         //ReferralCode = a.MyReferralCode,

        //                         LoanRefNumber = a.RefNumber,

        //                         AccountNumber = a.AccountNumber,

        //                         PermanentAddress = a.PermanentAddress,

        //                         Firstname = a.Firstname,

        //                         LoanAmount = a.LoanAmount.Value,

        //                         LoanTenure = a.LoanTenure.Value,

        //                         ValueDate = a.ValueDate,

        //                         Surname = a.Surname,

        //                         RemitaLink = string.IsNullOrEmpty(d.PatnerUrl) ? "none" : d.PatnerUrl,


        //                     }).OrderByDescending(x => x.ID).ToList();

        //        if (Loans == null)
        //        {
        //            return null;
        //        }
        //        return Loans;
        //    }
        //    catch (Exception ex)
        //    {
        //        WebLog.Log(ex.Message.ToString());
        //        return null;
        //    }
        //}



        public List<ExcelList> RejectedloanReport(int AppStatusFk)
        {
            try
            {
                var AppLoan = (from a in uvDb.NyscLoanApplications
                               join c in uvDb.Titles on a.Title_FK equals c.ID
                               join d in uvDb.MaritalStatus on a.MaritalStatus_FK equals d.ID
                               join e in uvDb.NYSCApplicationStatus on a.NYSCApplicationStatus_FK equals e.ID
                               join f in uvDb.NigerianStates on a.StateofResidence_FK equals f.ID
                               join g in uvDb.NigerianStates on a.TempStateofResidence_FK equals g.ID
                               join h in uvDb.NigerianStates on a.NyscStateofResidence_FK equals h.ID
                               join i in uvDb.LGAs on a.LGA_FK equals i.ID
                               join j in uvDb.LGAs on a.TempLGA_FK equals j.ID
                               join k in uvDb.LGAs on a.NyscLGA_FK equals k.ID
                               //join h in uvDb.LoanProducts on a.LoanProduct_FK equals h.LoanType_FK
                               join l in uvDb.Banks on a.BankCode equals l.Code


                               //join m in uvDb.EmployerLoanDetails on a.ID equals m.LoanApplication_FK
                               //join x in uvDb.RepaymentMethods on a.RepaymentMethod_FK equals x.ID
                               // join es in uvDb.EmploymentStatus on m.EmploymentStatus_FK equals es.ID
                               where a.NYSCApplicationStatus_FK == AppStatusFk
                               select new ExcelList
                               {
                                   ID = a.ID,
                                   ApplicationStatus = c.Name,
                                   AccountName = a.AccountName,

                                   //ReferralCode = a.MyReferralCode,

                                   LoanRefNumber = a.RefNumber,

                                   AccountNumber = a.AccountNumber,

                                   PermanentAddress = a.PermanentAddress,

                                   Firstname = a.Firstname,

                                   LoanAmount = a.LoanAmount.Value,

                                   LoanTenure = a.LoanTenure.Value,

                                   ValueDate = a.ValueDate,

                                   Surname = a.Surname,


                               }).OrderByDescending(x => x.ID).ToList();

                if (AppLoan == null)
                {

                    return null;
                }

                return AppLoan;

            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }



        public dynamic ApprovedloanReport(int AppStatusFk)
        {
            try
            {
                var AppLoan = (from a in uvDb.LoanApplications
                               join b in uvDb.ApplicationStatus
                               on a.ApplicationStatus_FK equals b.ID
                               join c in uvDb.Titles on a.Title_FK equals c.ID
                               //join d in uvDb.MaritalStatus on a.MaritalStatus_FK equals d.ID
                               //join e in uvDb.MeansOfIdentifications on a.MeansOfID_FK equals e.ID
                               //join f in uvDb.NigerianStates on a.StateofResidence_FK equals f.ID
                               //join g in uvDb.LGAs on a.LGA_FK equals g.ID
                               //join h in uvDb.LoanProducts on a.LoanProduct_FK equals h.ID
                               //join i in uvDb.Banks on a.BankCode equals i.Code

                               where a.ApplicationStatus_FK == AppStatusFk
                               select new AppLoans
                               {
                                   //MeansOfIdentifications = e.Name,
                                   AccountName = a.AccountName,
                                   //  Title = c.Name,
                                   //NigerianStates = f.Name,
                                   //LGAs = g.Name,
                                   // Gender = a.Gender_FK == 1 ? "Male" : "Female",
                                   LoanRefNumber = a.LoanRefNumber,

                                   //MaritalStatus = d.Name,
                                   //LoanProduct = h.LoanProduct1,
                                   // ApplicationStatus = b.Description,
                                   AccountNumber = a.AccountNumber,
                                   //ApplicantID = a.ApplicantID,
                                   //BankCode = i.Name,
                                   //BVN = a.BVN,
                                   //ClosestBusStop = a.ClosestBusStop,
                                   //ContactAddress = a.ContactAddress,
                                   //DateOfBirth = a.DateOfBirth,
                                   //EmailAddress = a.EmailAddress,
                                   //ExistingLoan = a.ExistingLoan.Value,
                                   //ExistingLoan_NoOfMonthsLeft = a.ExistingLoan_NoOfMonthsLeft.Value,
                                   // ExistingLoan_OutstandingAmount = a.ExistingLoan_OutstandingAmount.Value,
                                   Firstname = a.Firstname,
                                   //IdentficationNumber = a.IdentficationNumber,
                                   LoanAmount = a.LoanAmount.Value,
                                   LoanTenure = a.LoanTenure.Value,
                                   //NOK_EmailAddress = a.NOK_EmailAddress,
                                   //NOK_FullName = a.NOK_FullName,
                                   //NOK_HomeAddress = a.NOK_HomeAddress,
                                   //NOK_PhoneNumber = a.NOK_PhoneNumber,
                                   //NOK_Relationship = a.NOK_Relationship,
                                   //Organization = a.Organization,
                                   //PhoneNumber = a.PhoneNumber,
                                   ValueDate = a.ValueDate,
                                   //Landmark = a.Landmark,
                                   //Othernames = a.Othernames,
                                   Surname = a.Surname
                               }).OrderByDescending(x => x.ID).ToList();

                if (AppLoan == null)
                {

                    return null;
                }

                return AppLoan;

            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


        public AppLoans CheckInstitution(StudentRecord StdRec)
        {
            try
            {
                var ChkInst = (from a in uvDb.StudentRecords
                               join b in uvDb.Institutions on
                               a.Institution_FK equals b.InstitutionType_FK
                               where a.PhoneNumber == StdRec.PhoneNumber &&
                               a.MatriculationNumber == StdRec.MatriculationNumber
                               select new AppLoans
                               {
                                   PhoneNumber = a.PhoneNumber,
                                   Firstname = a.Firstname,
                                   Surname = a.Lastname,
                                   Faculty = a.Faculty,
                                   Department = a.Deparment,
                                   CreditLimit = a.CreditLimit,
                                   MatriculationNumber = a.MatriculationNumber,
                                   Institutions = b.Name,

                               }
                              ).FirstOrDefault();


                if (ChkInst == null)
                {

                    return null;


                }
                return ChkInst;

            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


        public dynamic LoanAppReport()
        {
            try
            {
                var LoanApp = (from a in uvDb.LoanApplications
                               join b in uvDb.ApplicationStatus
                               on a.ApplicationStatus_FK equals b.ID
                               join c in uvDb.Titles on a.Title_FK equals c.ID
                               join d in uvDb.MaritalStatus on a.MaritalStatus_FK equals d.ID
                               join e in uvDb.MeansOfIdentifications on a.MeansOfID_FK equals e.ID
                               join f in uvDb.NigerianStates on a.StateofResidence_FK equals f.ID
                               join g in uvDb.LGAs on a.LGA_FK equals g.ID
                               join h in uvDb.LoanProducts on a.LoanProduct_FK equals h.ID
                               //where a.ApplicationStatus_FK == 1
                               select new AppLoans
                               {
                                   MeansOfIdentifications = e.Name,
                                   AccountName = a.AccountName,
                                   Title = c.Name,
                                   NigerianStates = f.Name,
                                   LGAs = g.Name,
                                   Gender = a.Gender_FK == 1 ? "Male" : "Female",
                                   LoanRefNumber = a.LoanRefNumber,
                                   MaritalStatus = d.Name,
                                   LoanProduct = h.LoanProduct1,
                                   ApplicationStatus = b.Description,
                                   AccountNumber = a.AccountNumber,
                                   ApplicantID = a.ApplicantID,
                                   BankCode = a.BankCode,
                                   BVN = a.BVN,
                                   ClosestBusStop = a.ClosestBusStop,
                                   ContactAddress = a.ContactAddress,
                                   DateOfBirth = a.DateOfBirth,
                                   EmailAddress = a.EmailAddress,
                                   ExistingLoan = a.ExistingLoan.Value,
                                   ExistingLoan_NoOfMonthsLeft = a.ExistingLoan_NoOfMonthsLeft.Value,
                                   ExistingLoan_OutstandingAmount = a.ExistingLoan_OutstandingAmount.Value,
                                   Firstname = a.Firstname,
                                   IdentficationNumber = a.IdentficationNumber,
                                   LoanAmount = a.LoanAmount.Value,
                                   LoanTenure = a.LoanTenure.Value,
                                   NOK_EmailAddress = a.NOK_EmailAddress,
                                   NOK_FullName = a.NOK_FullName,
                                   NOK_HomeAddress = a.NOK_HomeAddress,
                                   NOK_PhoneNumber = a.NOK_PhoneNumber,
                                   NOK_Relationship = a.NOK_Relationship,
                                   Organization = a.Organization,
                                   PhoneNumber = a.PhoneNumber,
                                   ValueDate = a.ValueDate,
                                   Landmark = a.Landmark,
                                   Othernames = a.Othernames,
                                   Surname = a.Surname
                               }).OrderByDescending(x => x.ID).ToList();

                if (LoanApp == null)
                {

                    return null;


                }
                return LoanApp;

            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        public dynamic NYSCLoanAppReport()
        {
            try
            {
                var LoanApp = (from a in uvDb.NyscLoanApplications
                               join b in uvDb.NYSCApplicationStatus
                               on a.NYSCApplicationStatus_FK equals b.ID
                               join c in uvDb.Titles on a.Title_FK equals c.ID
                               join d in uvDb.MaritalStatus on a.MaritalStatus_FK equals d.ID
                               join e in uvDb.NigerianStates on a.StateofResidence_FK equals e.ID
                               join f in uvDb.NigerianStates on a.TempStateofResidence_FK equals f.ID
                               join i in uvDb.NigerianStates on a.NyscStateofResidence_FK equals i.ID
                               join j in uvDb.LGAs on a.LGA_FK equals j.ID
                               join k in uvDb.LGAs on a.TempLGA_FK equals k.ID
                               join l in uvDb.LGAs on a.NyscLGA_FK equals l.ID
                               join m in uvDb.Banks on a.BankCode equals m.Code

                               select new AppLoanss
                               {
                                   ID = a.ID,
                                   ApplicationStatus = b.Name,
                                   AccountName = a.AccountName,
                                   Title = c.Name,
                                   StateofResidence = f.Name,
                                   StateCode = a.StateCode,

                                   TempStateofResidence = f.Name,
                                   NyscStateofResidence = i.Name,
                                   LGAs = j.Name,
                                   Gender = a.Gender_FK == 1 ? "Male" : "Female",
                                   LoanRefNumber = a.RefNumber,
                                   MaritalStatus = d.Name,

                                   NetMonthlyIncome = a.NetMonthlyIncome.Value,
                                   AccountNumber = a.AccountNumber,

                                   BankCode = m.Name,
                                   BVN = a.BVN,
                                   ClosestBusStop = a.ClosestBusStop,
                                   PermanentAddress = a.PermanentAddress,
                                   DateOfBirth = a.DateOfBirth,
                                   EmailAddress = a.EmailAddress,
                                  // ExistingLoan = a.ExistingLoan.HasValue ? a.ExistingLoan.Value : false,
                                 //  ExistingLoan_NoOfMonthsLeft = a.ExistingLoan_NoOfMonthsLeft.Value,
                                   //ExistingLoan_OutstandingAmount = a.ExistingLoan_OutstandingAmount.Value > 0 ? a.ExistingLoan_OutstandingAmount.Value : 0,
                                   Firstname = a.Firstname,

                                   LoanAmount = a.LoanAmount.Value,
                                   ReferralCode = a.ReferralCode,
                                   LoanTenure = a.LoanTenure.Value,
                                   EMG_EmailAddress = a.EMG_EmailAddress,
                                   EMG_FullName = a.EMG_FullName,
                                   EMG_HomeAddress = a.EMG_HomeAddress,
                                   EMG_PhoneNumber = a.EMG_PhoneNumber,
                                   EMG_Relationship = a.EMG_Relationship,
                                   TemporaryAddress = a.TemporaryAddress,
                                   TempClosestBusStop = a.TempClosestBusStop,
                                   TempLandmark = a.TempLandmark,
                                   PhoneNumber = a.PhoneNumber,
                                   ValueDate = a.ValueDate,
                                   ValueTime = a.ValueTime,
                                   Landmark = a.Landmark,
                                   Othernames = a.Othernames,
                                   Surname = a.Surname,
                                   TempLGAs = j.Name,
                                   NyscLGAs = k.Name,
                                   PassOutMonth = a.PassOutMonth,
                                   OfficialAddress = a.OfficialAddress,
                                   Employer = a.Employer,
                                   CDSDay = a.CDSDay,
                                   CDSGroup = a.CDSGroup,
                                   SalaryAmount = (double)a.NetMonthlyIncome,

                               }).OrderByDescending(x => x.ID).ToList();

                if (LoanApp == null)
                {

                    return null;


                }
                return LoanApp;

            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


        public int GetAccomType(string ACCOMMODATIONTYPE)
        {
            try
            {
                var Accomodation = (from a in uvDb.AccomodationTypes
                                    where a.Name == ACCOMMODATIONTYPE
                                    select a).FirstOrDefault();
                if (Accomodation == null)
                {

                    return 0;
                }

                return Accomodation.ID;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.StackTrace);
                return 0;
            }
        }
        public int GetMaritalStatus(string status)
        {
            try
            {

                var idname = (from a in uvDb.MaritalStatus
                              where a.Name == status
                              select a).FirstOrDefault();
                if (idname == null)
                {

                    return 0;
                }

                return idname.ID;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.StackTrace);
                return 0;
            }
        }


        public int GetMeansofIdbyname(string MeansOfID)
        {
            try
            {

                var idname = (from a in uvDb.MeansOfIdentifications
                              where a.Name == MeansOfID
                              select a).FirstOrDefault();
                //var idnames = (from a in uvDb.MeansOfIdentifications
                //               where a.Name.StartsWith(MeansOfID.)
                //               select a).FirstOrDefault();
                // var res = uvDb.MeansOfIdentifications.Contains(MeansOfID);
                if (idname == null)
                {

                    return 0;
                }

                return idname.ID;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.StackTrace);
                return 0;
            }
        }



        public  string GetBankCode(string BANKNAME)
        {
            try
            {
                var Bank = (from a in uvDb.Banks
                            where a.Name == BANKNAME
                            select a.Code).FirstOrDefault();
                if (Bank == null)
                {

                    return null;
                }

                return Bank;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.StackTrace);
                return null;
            }
        }

        public int GetState(string STATEOFRESIDENCE)
        {
            try
            {
                var state = (from a in uvDb.NigerianStates
                             where a.Name == STATEOFRESIDENCE
                             select a).FirstOrDefault();
                if (state == null)
                {

                    return 0;
                }

                return state.ID;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.StackTrace);
                return 0;
            }
        }
        public List<RepaymentMethod> GetRepaymentMethods()
        {
            try
            {
                var Services = (from a in uvDb.RepaymentMethods select a).ToList();

                if (Services == null)
                {
                    return null;
                }
                return Services;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message);
                return null;
            }
        }

        public List<NigerianState> GetNigerianStates()
        {
            try
            {
                var Services = (from a in uvDb.NigerianStates select a).ToList();

                if (Services == null)
                {
                    return null;
                }
                return Services;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message);
                return null;
            }
        }

        public List<MeansOfIdentification> GetMeansOfIdentifications()
        {
            try
            {
                var Services = (from a in uvDb.MeansOfIdentifications select a).ToList();

                if (Services == null)
                {
                    return null;
                }
                return Services;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message);
                return null;
            }
        }

        public void UpdatePassword(User user)
        {
            try
            {

                var original = uvDb.Users.Find(user.EmailAddress);


                if (original != null)
                {



                    original.PaswordVal = user.PaswordVal;
                    // original.confirmPassword = user.confirmPassword;


                    uvDb.SaveChanges();
                }

            }


            catch (Exception ex)
            {

                WebLog.Log(ex.Message.ToString());
            }

        }

        public List<MarketingChannel> GetMarketChannel()
        {
            try
            {
                var MgtChannels = (from a in uvDb.MarketingChannels select a).ToList();

                if (MgtChannels == null)
                {
                    return null;
                }
                return MgtChannels;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message);
                return null;
            }
        }


        public List<AccomodationType> GetAccomodationTypes()
        {
            try
            {
                var Services = (from a in uvDb.AccomodationTypes select a).ToList();

                if (Services == null)
                {
                    return null;
                }
                return Services;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message);
                return null;
            }
        }

        public List<LGA> GetAllLGAs()
        {
            try
            {
                var Services = (from a in uvDb.LGAs select a).ToList();

                if (Services == null)
                {
                    return null;
                }
                return Services;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message);
                return null;
            }
        }

        public List<Title> GetTitles()
        {
            try
            {
                var Services = (from a in uvDb.Titles select a).ToList();

                if (Services == null)
                {
                    return null;
                }
                return Services;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message);
                return null;
            }
        }

        public List<MaritalStatu> GetMaritalStatus()
        {
            try
            {
                var Services = (from a in uvDb.MaritalStatus select a).OrderBy(a => a.ID).ToList().Take(3);

                if (Services == null)
                {
                    return null;
                }
                return Services.ToList();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message);
                return null;
            }
        }

        public List<NYSCRelative> GetRelative()
        {
            try
            {
                var Services = (from a in uvDb.NYSCRelatives select a).OrderBy(a => a.ID).ToList()/*.Take(7)*/;

                if (Services == null)
                {
                    return null;
                }
                return Services.ToList();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message);
                return null;
            }
        }


        public List<NYSCLoanSetUp> GetAllTenure()
        {
            try
            {
                var Service = (from a in uvDb.NYSCLoanSetUps select a).OrderBy(x => x.Tenure).ToList();

                if (Service == null)
                {
                    return null;
                }
                return Service.ToList();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message);
                return null;
            }
        }


        public List<LGA> GetLGAsByStateFK(int StateFK)
        {
            try
            {
                var Services = (from a in uvDb.LGAs select a)
                 .Where(x => x.State_FK == StateFK).ToList();

                if (Services == null)
                {
                    return null;
                }
                return Services;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message);
                return null;
            }
        }


        public int GetInstitution(string Institution)
        {
            try
            {
                var InstitutionName = (from a in uvDb.Institutions
                                       where a.Name == Institution
                                       select a).FirstOrDefault();
                if (InstitutionName == null)
                {
                    return 0;
                }
                return InstitutionName.ID;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.StackTrace);
                return 0;
            }
        }
        public int GetTitleIDByName(string Tittle)
        {
            try
            {
                var TitleName = (from a in uvDb.Titles
                                 where a.Name == Tittle
                                 select a).FirstOrDefault();
                if (TitleName == null)
                {
                    return 0;
                }
                return TitleName.ID;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.StackTrace);
                return 0;
            }
        }
        public int GetEmpoStatus(string Empostatus)
        {
            try
            {
                var Status = (from a in uvDb.EmploymentStatus
                              where a.Name == Empostatus
                              select a).FirstOrDefault();
                if (Status == null)
                {

                    return 0;
                }

                return Status.ID;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.StackTrace);
                return 0;
            }
        }
        public int GetLocalGovt(string LGA)
        {
            try
            {

                var Govt = (from a in uvDb.LGAs
                            where a.Name == LGA
                            select a).FirstOrDefault();
                if (Govt == null)
                {

                    return 0;
                }

                return Govt.ID;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.StackTrace);
                return 0;
            }
        }
        public int GetStatus(string MARITALSTATUS)
        {
            try
            {
                var Status = (from a in uvDb.MaritalStatus
                              where a.Name == MARITALSTATUS
                              select a).FirstOrDefault();
                if (Status == null)
                {

                    return 0;
                }

                return Status.ID;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.StackTrace);
                return 0;
            }
        }
        public bool loggedIn(string username, string password)
        {
            try
            {
                WebLog.Log("am new here " + username);
                var Loggedin = (from a in uvDb.Users
                                where a.EmailAddress == username && a.PaswordVal == password
                                select a).FirstOrDefault();

                if (Loggedin != null)
                {
                    WebLog.Log("am new here1 " + username);
                    return true;
                }
                WebLog.Log("am new here0 " + username);
                return false;

            }
            catch (Exception dbEx)
            {
                WebLog.Log(dbEx.Message.ToString());

                return false;


            }
            //catch (DbEntityValidationException ex)
            //{
            //    string errorMessages = string.Join("; ", ex.EntityValidationErrors.SelectMany(x => x.ValidationErrors).Select(x => x.PropertyName + ": " + x.ErrorMessage));
            //    throw new DbEntityValidationException(errorMessages);
            //}

        }

        public bool Validate(string value)
        {
            try
            {
                var user = (from a in uvDb.Users
                            where a.EmailAddress == value
                            select a).FirstOrDefault();
                if (user != null)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return false;
            }
        }

        public AppLoanss LoanDetailss(int Refid)
        {
            try
            {
                var LoanDetails = (from a in uvDb.NyscLoanApplications
                                   join c in uvDb.NYSCApplicationStatus on
a.NYSCApplicationStatus_FK equals c.ID
                                   where a.ID == Refid

                                   select new AppLoanss
                                   {
                                       ID = a.ID,
                                       //LoanRefNumber = a.RefNumber,
                                       Firstname = a.Firstname,
                                       Surname = a.Surname,
                                       Othernames = a.Othernames,
                                       LoanTenure = (int)a.LoanTenure,
                                       LoanAmount = (double)a.LoanAmount,
                                       ApplicationStatus = c.Name,
                                       PhoneNumber = a.PhoneNumber,
                                       EmailAddress = a.EmailAddress,
                                       RepaymentAmount = a.LoanTenure.ToString(),
                                       ValueDate = a.ValueDate,
                                       LoanTenureStr = a.LoanTenure + " months",
                                       CDSDay = a.CDSDay
                                   }).FirstOrDefault();

                if (LoanDetails == null)
                {
                    return null;
                }

                return LoanDetails;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }







        public AppLoanss LoanDetails(string Refid)
        {
            try
            {
                var LoanDetails = (from a in uvDb.NyscLoanApplications
                                   join c in uvDb.NYSCApplicationStatus on
a.NYSCApplicationStatus_FK equals c.ID
                                   where a.RefNumber == Refid

                                   select new AppLoanss
                                   {
                                       ID = a.ID,
                                       LoanRefNumber = a.RefNumber,
                                       Firstname = a.Firstname,
                                       Surname = a.Surname,
                                       Othernames = a.Othernames,
                                       LoanTenure = (int)a.LoanTenure,
                                       LoanAmount = (double)a.LoanAmount,
                                       ApplicationStatus = c.Name,
                                       PhoneNumber = a.PhoneNumber,
                                       EmailAddress = a.EmailAddress,
                                       RepaymentAmount = a.LoanTenure.ToString(),
                                       ValueDate = a.ValueDate,
                                       LoanTenureStr = a.LoanTenure + " months",
                                       CDSDay = a.CDSDay
                                   }).FirstOrDefault();

                if (LoanDetails == null)
                {
                    return null;
                }

                return LoanDetails;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }
    }
}

