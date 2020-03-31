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

                var Loans = (from a in uvDb.NYSCReferralLedgers join b in uvDb.Users on a.User_FK equals b.ID where a.User_FK == userid
                             select new ReferralRecords
                             {
                                 ID = a.ID,
                                 ReferralCode = a.ReferralCode,
                                 ReferenceNumber = a.ReferenceNumber,
                                 Debit =(Double) a.Debit,
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
                               // join ph in uvDb.pageHeaders on p.pageHeader equals ph.id
                               join r in uvDb.Roles on pa.Role_FK equals r.RoleId

                               where rol.Contains(r.RoleName)
                               select new Menus
                               {
                                   pageName = pa.PageName,
                                   roleid = (int)pa.Role_FK,
                                   //pageheader = ph.page_header,
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
                var users = (from a in uvDb.Users where a.EmailAddress == email  select a).FirstOrDefault();

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

        public List<ExcelList> NYSCApplications( string userEmail)

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
                             where a.NYSCApplicationStatus_FK== AppStatFk && a.IsVisible == 1
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
                            // join d in uvDb.PatnerTransactLogs on a.RefNumber equals d.RefNum
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


        public List<ExcelList> ApproveLoansRemFlu(int AppStatFk,string PatnerCode)

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
                             join c in uvDb.Banks on a.BankCode equals c.Code
                             
                             where a.NYSCApplicationStatus_FK == AppstatFk && a.IsVisible == 1
                             select new AppLoanss
                             {
                                 ID = a.ID,
                                 ApplicationStatus = b.Name,
                                 AccountName = a.AccountName,
                                 //Title = c.Name,
                                 StateofResidence = b.Name,
                                 StateCode = a.StateCode,

                                 //TempStateofResidence = f.Name,
                                 NyscStateofResidence = b.Name,
                                 //LGAs = j.Name,
                                 //Gender = a.Gender_FK == 1 ? "Male" : "Female",
                                 LoanRefNumber = a.RefNumber,
                                 //MaritalStatus = d.Name,

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
                                 //TempLGAs = j.Name,
                                 //NyscLGAs = k.Name,
                                 PassOutMonth = a.PassOutMonth,
                                 OfficialAddress = a.OfficialAddress,
                                 Employer = a.Employer,
                                 CDSDay = a.CDSDay,
                                 CDSGroup = a.CDSGroup,
                                 SalaryAmount = (double)a.NetMonthlyIncome,

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
                var LoanTenures = (from a in uvDb.NYSCLoanSetUps where a.Tenure <= Tenure select a).OrderByDescending(x => x.ID).ToList();

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
                    serialNo = Convert.ToInt16( sObj.NYSCSerialNo) +1;
                }


                string serialNumb = "";
                if (serialNo.ToString().Length == 1)
                {
                    serialNumb = "00000" + serialNo.ToString();
                }
                else if(serialNo.ToString().Length == 2)
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
                    serialNumb =  serialNo.ToString();
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
                               //join e in uvDb.MeansOfIdentifications on a.MeansOfID_FK equals e.ID
                               join f in uvDb.NigerianStates on a.StateofResidence_FK equals f.ID
                               join g in uvDb.NigerianStates on a.TempStateofResidence_FK equals g.ID
                               join h in uvDb.NigerianStates on a.NyscStateofResidence_FK equals h.ID
                               join i in uvDb.LGAs on a.LGA_FK equals i.ID
                               join j in uvDb.LGAs on a.TempLGA_FK equals j.ID
                               join k in uvDb.LGAs on a.NyscLGA_FK equals k.ID
                               //join h in uvDb.LoanProducts on a.LoanProduct_FK equals h.LoanType_FK
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
                                   //LoanComment = m.Comment,                                   //EmployeeStatus = es.Name,
                                   NyscIdCardFilePath = a.NyscIdCardFilePath,
                                   STA_FilePath = a.STA_FilePath,
                                   ReferralCode = a.ReferralCode,

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


        //public dynamic LoanTransactionbyDate(DateTime to, DateTime from)
        //{
        //    try
        //    {
        //        var Apploan = uvDb.LoanTransactionbyDate(from, to).ToList();

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
                               join b in uvDb.ApplicationStatus
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
                                   ExistingLoan = a.ExistingLoan.HasValue ? a.ExistingLoan.Value : false,
                                   ExistingLoan_NoOfMonthsLeft = a.ExistingLoan_NoOfMonthsLeft.Value,
                                   ExistingLoan_OutstandingAmount = a.ExistingLoan_OutstandingAmount.Value > 0 ? a.ExistingLoan_OutstandingAmount.Value : 0,
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



        public string GetBankCode(string BANKNAME)
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
                                where a.EmailAddress == username  && a.PaswordVal == password
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
