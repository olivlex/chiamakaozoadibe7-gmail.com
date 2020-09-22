
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UvlotExt.Classes;
using System.Text;
using System.Configuration;
using System.IO;
using System.Data;
using System.Data.OleDb;
using DataAccessA.DataManager;
using DataAccessA.Classes;
using DataAccessA;
using ExcelUpload.Models;
using UvlotExt.Models;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.WebControls;
using Newtonsoft.Json.Linq;
using UvlotApp.Classes;
using UvlotExt.Controllers.NYSCLOAN;
using System.Web.Hosting;
using System.Net;
using System.Net.Mail;
using UvlotApplication.Classes;
using Utilities;
using static Utilities.CryptographyManager;

namespace UvlotExt.Controllers
{
    public class AdminAController : Controller
    {
        //UserController _usr = new UserController();

        DataWriter _DM = new DataWriter();
        DataReader _DR = new DataReader();


        PageAuthentication _pa = new PageAuthentication();

        DataAccessA.DataManager.Page pages = new DataAccessA.DataManager.Page();
        string user = "";
        UvlotAEntities db = new UvlotAEntities();
        StudentRecord StdRec = new StudentRecord();
        int ApplicationFk = 0;
        // string respMessage = "";
        int AppStatFk = 0;
        int userid = 0;
        // string id = "";
        List<string> rol = new List<string>();
        // GET: Admin
        public ActionResult Index()
        {
            //bool valid = validatePage();
            var LoggedInuser = new LogginHelper();
            user = LoggedInuser.LoggedInUser();

            var appUser = user;
            var User = _DR.getUser(appUser);
            if (appUser == null)
            {
                return RedirectToAction("/", "Home");
            }
            GetMenus();
            int userid = _DR.getUserID(user);
            var Records = _DR.getCommisioRecords(userid);
            var resp = Comisionsum(Records);
            double TotalRepayment = Convert.ToDouble(resp);
            TotalRepayment = Math.Round(TotalRepayment);
            ViewBag.Balance = TotalRepayment;
           var referalCode = User.MyReferralCode;
           TempData["referalCode"] = referalCode;

            return View();
        }



        private double Comisionsum(List<NYSCReferralLedger> Record)
        {
            try
            {
                List<string> AmountList = new List<string>();
                double Total = 0;
                {
                    for (int i = 0; i < Record.Count; i++)
                    {

                        Total += Convert.ToDouble(Record[i].Debit) - Convert.ToDouble(Record[i].Credit);
                    }
                }
                return Total;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return 0;
            }
        }
        [HttpGet]
        public ActionResult Exportoexcel()
        {
            ExportToExcel();
            GetMenus();
            return View("AllApprovedLoans");
        }

        public void ExportToExcel()
        {
            //DataTable dt = new DataTable("GridView_Data");
            var gv = new GridView();
            gv.DataSource = Session["AllTransaction"];
            gv.DataBind();


            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=AllTransaction.xls");
            Response.ContentType = "application/ms-excel";

            Response.Charset = "";
            StringWriter objStringWriter = new StringWriter();
            HtmlTextWriter objHtmlTextWriter = new HtmlTextWriter(objStringWriter);

            gv.RenderControl(objHtmlTextWriter);

            Response.Output.Write(objStringWriter.ToString());
            Response.Flush();
            Response.End();

        }
        [HttpGet]
        public ActionResult ExportToEx()
        {
            ExportToExcel();
            GetMenus();
            return View("DisbursedApplicantReport");
        }

        [HttpGet]
        public ActionResult ExportToEx1()
        {
            ExportToExcel();
            GetMenus();
            return View("RegisteredApplicant");
        }

        [HttpGet]
        public ActionResult ExportToEx2()
        {
            ExportToExcel();
            GetMenus();
            return View("BorrowedLoan");
        }

        [HttpGet]
        public ActionResult ExportToEx3()
        {
            ExportToExcel();
            GetMenus();
            return View("DisbursedLoansRep");
        }
        [HttpGet]
        public ActionResult ExportToEx4()
        {
            ExportToExcel();
            GetMenus();
            return View("Repayment");
        }

        [HttpGet]
        public ActionResult ExportToEx5()
        {
            ExportToExcel();
            GetMenus();
            return View("RevenueReceived");
        }
        [HttpGet]
        public ActionResult ExportToEx6()
        {
            ExportToExcel();
            GetMenus();
            return View("RevenueEarned");
        }
        [HttpGet]
        public ActionResult ExportToEx7()
        {
            ExportToExcel();
            GetMenus();
            return View("OutStandingLoan");
        }

        //[HttpGet]
        //public ActionResult ExportToEx8()
        //{
        //    ExportToExcel();
        //    GetMenus();
        //    return View("DisbursedApplicantReport");
        //}





        public ActionResult BVNValidation(DataAccessA.Classes.LoanApplication Apploans, FormCollection form, string bvnNumber)
        {

            try
            {
                //var helper = new Helper();
                var BC = Helper.BVNValidationResps(Apploans.BVN);


                return Json(new { BC });
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }

        }

        [HttpPost]
        public ActionResult InterestRate(int Tenure)
        {
            try
            {
                DataReader dr = new DataReader();

                var LoanTenure = dr.GetInterestRate(Tenure);

                var InterestRate = Convert.ToDouble(LoanTenure.InterestRate);

                return Json(new { InterestRate });
            }

            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }

        }


        [HttpGet]
        public ActionResult LoanCalulator()

        {
            //GetMenus();
            return View();
        }
        [HttpPost]

        public ActionResult LoanCalulator(FormCollection form, DataAccessA.Classes.LoanViewModel instobj)
        {
            try
            {
                DataReader dr = new DataReader();
                LoanInterestRate IntR = new LoanInterestRate();
                //LoanApplication instObj = new LoanApplication();
                {

                    // ViewBag.InterestRate = db.InterestRates.ToList();
                    var LoanAmount = Convert.ToDouble(instobj.LoanApplication.LoanAmount);
                    var LoanTenur = (int)instobj.LoanApplication.LoanTenure;
                    int Tenure = Convert.ToInt16(LoanTenur);

                    var LoanTenure = dr.GetInterestRate(Tenure);

                    var InterestRate = Convert.ToDouble(LoanTenure.InterestRate);

                    if (InterestRate == 0)
                    {
                        TempData["ErrMsg"] = "Invalid Loan Tenure";
                        return View();
                    }

                    ViewBag.LoanCal = Helper.PayrollLoanCalc(LoanAmount, LoanTenur, InterestRate);


                };
                //  GetMenus();
                return View();
            }

            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }

        }


        [HttpGet]
        public ActionResult ImportExcel1()
        {

            ViewBag.LoanCal = Helper.PayrollLoanCalc(112500, 11, 4.50).ToString();
            ViewBag.mySum = Helper.CalculateSum(23, 32).ToString();

            GetMenus();
            return View();
        }

        [ActionName("Importexcel1")]
        [HttpPost]
        public ActionResult Importexcel1()
        {


            if (Request.Files["FileUpload1"].ContentLength > 0)
            {
                string extension = System.IO.Path.GetExtension(Request.Files["FileUpload1"].FileName).ToLower();
                // string query = null;
                string connString = "";

                Session["myDtVal"] = null;


                string[] validFileTypes = { ".xls", ".xlsx", ".csv" };

                string path1 = string.Format("{0}/{1}", Server.MapPath("~/Content/Uploads"), Request.Files["FileUpload1"].FileName);
                if (!Directory.Exists(path1))
                {
                    Directory.CreateDirectory(Server.MapPath("~/Content/Uploads"));
                }
                if (validFileTypes.Contains(extension))
                {
                    if (System.IO.File.Exists(path1))
                    {
                        System.IO.File.Delete(path1);
                    }
                    Request.Files["FileUpload1"].SaveAs(path1);
                    if (extension == ".csv")
                    {
                        DataTable dt = Utility.ConvertCSVtoDataTable(path1);
                        ViewBag.Data = dt;
                        Session["myDtVal"] = dt;
                    }
                    //Connection String to Excel Workbook  
                    else if (extension.Trim() == ".xls")
                    {
                        connString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path1 + ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=2\"";
                        DataTable dt = Utility.ConvertXSLXtoDataTable(path1, connString);
                        ViewBag.Data = dt;
                        Session["myDtVal"] = dt;
                    }
                    else if (extension.Trim() == ".xlsx")
                    {
                        connString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + path1 + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
                        DataTable dt = Utility.ConvertXSLXtoDataTable(path1, connString);
                        ViewBag.Data = dt;
                        Session["myDtVal"] = dt;
                    }

                }
                else
                {
                    ViewBag.Error = "Please Upload Files in .xls, .xlsx or .csv format";
                }
                GetMenus();
            }
            return View();
        }

        [HttpPost]
        public ActionResult uploaddata(LoginViewModel lvm, FormCollection form)
        {
            try
            {
                float PorationValue = 0;
                UvlotAEntities db = new UvlotAEntities();
                //string refNumber = MyUtility.GenerateRefNo();
                DataTable dt = (DataTable)Session["myDtVal"];
                DataReader dr = new DataReader();
                GetMenus();

                foreach (DataRow row in dt.Rows)


                    try
                    {
                        //  LoanApplication inst = new LoanApplication();
                        if (row["SURNAME"] != null)
                        {
                            if (row["SURNAME"].ToString().Length < 2 && row["FIRST NAME"].ToString().Length < 2)
                            {
                                break;
                            }
                        }

                        DataAccessA.DataManager.LoanApplication instObj = new DataAccessA.DataManager.LoanApplication();

                        instObj.AccomodationType_FK = dr.GetAccomType(row["ACCOMMODATION TYPE"].ToString());
                        instObj.AccountName = row["ACCOUNT NAME"].ToString();
                        instObj.AccountNumber = row["ACCOUNT NUMBER"].ToString();

                        instObj.ApplicantID = row["MEANS OF ID"].ToString();
                        instObj.ApplicationStatus_FK = 3;//Pending status,
                        instObj.BVN = row["BANK VERIFICATION NUMBER"].ToString();
                        instObj.ClosestBusStop = row["CLOSEST BUSTOP"].ToString();

                        string DOB = row["DATE OF BIRTH"].ToString();
                        instObj.ContactAddress = row["HOME ADDRESS"].ToString();
                        instObj.CreatedBy = ""; //Change To User ID

                        instObj.DateCreated = MyUtility.getCurrentLocalDateTime();
                        //string inputString = row["DISBURSEMRNT DATE"].ToString();
                        //CultureInfo culture = new CultureInfo("en-US");

                        //DateTime dateTimeObj = Convert.ToDateTime(inputString, culture);
                        //instObj.DateCreated = dateTimeObj;
                        instObj.DateModified = MyUtility.getCurrentLocalDateTime();
                        instObj.DateOfBirth = DOB;// Helper.isDate(DOB) == true ? Convert.ToDateTime(DOB) : MyUtility.getCurrentLocalDateTime();
                        instObj.EmailAddress = row["PERSONAL EMAIL"].ToString();
                        instObj.ExistingLoan = row["ANY EXISTING LOAN"].ToString().ToUpper() == "YES" ? true : false;

                        instObj.ExistingLoan_NoOfMonthsLeft = 0;


                        instObj.ExistingLoan_OutstandingAmount = 0;
                        instObj.Firstname = row["FIRST NAME"].ToString();
                        instObj.Gender_FK = row["GENDER"].ToString().ToUpper() == "MALE" ? 1 : 2;

                        instObj.IdentficationNumber = row["IDENTIFICATION NUMBER"].ToString();
                        instObj.Landmark = row["LANDMARK"].ToString();
                        instObj.LGA_FK = dr.GetLocalGovt(row["LGA"].ToString());
                        instObj.LoanAmount = Convert.ToDouble(row["LOAN AMOUNT"]);
                        instObj.LoanTenure = Convert.ToInt16(row["TENURE"]);
                        instObj.LoanComment = "";
                        instObj.LoanRefNumber = MyUtility.GenerateRefNo();
                        instObj.MaritalStatus_FK = dr.GetStatus(row["MARITAL STATUS"].ToString());
                        instObj.BankCode = Convert.ToString(dr.GetBankCode(row["BANK NAME"].ToString().Trim()));
                        instObj.MeansOfID_FK = dr.GetMeansofIdbyname(row["MEANS OF ID"].ToString());

                        instObj.NOK_EmailAddress = row["NOK_EMAIL ADDRESS"].ToString();
                        instObj.NOK_FullName = row["NOK_NAME"].ToString();
                        instObj.NOK_HomeAddress = row["NOK_HOME ADDRESS"].ToString();
                        instObj.NOK_PhoneNumber = row["NOK_PHONE"].ToString();


                        instObj.NOK_Relationship = row["NOK_RELATIONSHIP"].ToString();
                        instObj.Organization = row["EMPLOYER"].ToString();
                        instObj.Othernames = "";
                        instObj.PhoneNumber = row["MOBILE NUMBER"].ToString();

                        instObj.Institution_FK = dr.GetInstitution(row["INSTITUTION"].ToString());
                        instObj.RepaymentMethod_FK = 0;
                        instObj.StateofResidence_FK = dr.GetState(row["STATE OF RESIDENCE"].ToString());
                        instObj.Surname = row["SURNAME"].ToString();
                        instObj.Title_FK = dr.GetTitleIDByName(row["TITLE"].ToString());

                        instObj.IsVisible = 1;
                        instObj.ValueDate = MyUtility.getCurrentLocalDateTime().ToString("yyyy/MM/dd");
                        instObj.ValueTime = MyUtility.getCurrentLocalDateTime().ToString("H:mmss");
                        db.LoanApplications.Add(instObj);
                        db.SaveChanges();


                        EmployerLoanDetail empObj = new EmployerLoanDetail();
                        empObj.ClosestBusStop = row["CLOSEST BUSTOP"].ToString();
                        empObj.DateCreated = MyUtility.getCurrentLocalDateTime();
                        empObj.DateModified = MyUtility.getCurrentLocalDateTime();

                        empObj.Department = row["DEPARTMENT"].ToString();
                        empObj.Designation = row["DESIGNATION"].ToString();
                        empObj.EmployerID = row["EMPLOYEE ID"].ToString();
                        empObj.EmploymentStatus_FK = dr.GetEmpoStatus(row["EMPLOYMENT STATUS"].ToString());

                        empObj.IsVisible = 1;
                        empObj.LandMark = row["LANDMARK"].ToString();
                        var loss = row["LENGTH OF SERVICE WITH CURRENT EMPLOYER"].ToString();
                        int los = Helper.isNumeric(loss) == true ? Convert.ToInt16(loss) : 0;

                        empObj.LengthOfServiceInMth = los;
                        empObj.LGA_FK = row["LGA"].ToString();
                        empObj.NetMonthlyIncome = Convert.ToDouble(row["NET MONTHLY INCOME"]);
                        empObj.Occupation = row["OCCUPATION"].ToString();

                        empObj.OfficialEmailAddress = row["EMAIL"].ToString();
                        empObj.ValueDate = MyUtility.getCurrentLocalDateTime().ToString("yyyy/MM/dd");
                        empObj.ValueTime = MyUtility.getCurrentLocalDateTime().ToString("H:mmss");
                        empObj.LoanApplication_FK = instObj.ID;

                        db.EmployerLoanDetails.Add(empObj);
                        db.SaveChanges();
                        instObj.MeansOfID_FK = dr.GetMeansofIdbyname(row["MEANS OF ID"].ToString());
                        int Tenure = Convert.ToInt16(instObj.LoanTenure);
                        var LoanTenure = dr.GetInterestRate(Tenure);
                        if (LoanTenure == null)
                            return null;

                        var LoanAmount = (double)instObj.LoanAmount;
                        var LoanTenur = (int)instObj.LoanTenure;
                        var InterestRate = Convert.ToDouble(LoanTenure.InterestRate);

                        var disburseDate = instObj.DateCreated;
                        var disbursementDate = MyUtility.getCurrentLocalDateTime();
                        int currentDay = disbursementDate.Day;
                        int LastDayOfTheMonth = DateTime.DaysInMonth(disbursementDate.Year, disbursementDate.Month);
                        int DaysLeft = LastDayOfTheMonth - currentDay;
                        var Repayment = Helper.PayrollLoanCalc(LoanAmount, LoanTenur, InterestRate);
                        var repay = Convert.ToDouble(Repayment);
                        if (DaysLeft > 10)
                        {
                            Repayment = "0";
                            PorationValue = 0;
                            // left to you
                        }
                        int i = 0;
                        if (DaysLeft <= 10)
                        {

                            var MonthlyInt = (repay * (InterestRate * 0.01));
                            PorationValue = (float)(MonthlyInt * (DaysLeft / LastDayOfTheMonth));

                        }

                        for (i = 0; i <= LoanTenur; i++)
                        {

                            LoanLedger lonObj = new LoanLedger();
                            lonObj.ApplicantID = row["EMPLOYEE ID"].ToString();
                            lonObj.Credit = 0;
                            if (i == 1)
                            {

                                lonObj.Debit = repay + PorationValue;

                            }
                            if (i > 1)
                            {

                                lonObj.Debit = repay;

                            }
                            lonObj.Institution_FK = instObj.Institution_FK;
                            lonObj.IsVisible = 1;
                            lonObj.LastUpdated = MyUtility.getCurrentLocalDateTime();
                            lonObj.RefNumber = instObj.LoanRefNumber;

                            lonObj.PartnerRefNumber = "";
                            lonObj.TranxDate = instObj.DateCreated.Value.AddMonths(i);
                            lonObj.ValueDate = MyUtility.getCurrentLocalDateTime().ToString("yyyy/MM/dd");
                            lonObj.ValueTime = MyUtility.getCurrentLocalDateTime().ToString("H:mmss");

                            db.LoanLedgers.Add(lonObj);
                            db.SaveChanges();
                        }

                        //for (i = 0; i <= LoanTenur; i++)
                        //{


                        //    LoanLedger lonObj = new LoanLedger();
                        //    lonObj.ApplicantID = row["EMPLOYEE ID"].ToString();
                        //    lonObj.Credit = 0;
                        //    if(i == 1)
                        //    {

                        //        lonObj.Debit = repay + PorationValue ;

                        //    }
                        //    if (i > 1)
                        //    {

                        //        lonObj.Debit = repay ;

                        //    }
                        //    lonObj.Institution_FK = instObj.Institution_FK;
                        //    lonObj.IsVisible = 1;
                        //    lonObj.LastUpdated = MyUtility.getCurrentLocalDateTime();
                        //    lonObj.RefNumber = instObj.LoanRefNumber;

                        //    lonObj.PartnerRefNumber = "";
                        //    lonObj.TranxDate = MyUtility.getCurrentLocalDateTime().AddMonths(i);
                        //    lonObj.ValueDate = MyUtility.getCurrentLocalDateTime().ToString("yyyy/MM/dd");
                        //    lonObj.ValueTime = MyUtility.getCurrentLocalDateTime().ToString("H:mmss");

                        //    db.LoanLedgers.Add(lonObj);
                        //    db.SaveChanges();
                        //}
                        //DataWriter.CreateLoanApplication(instObj);
                    }

                    catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
                    {
                        Exception raise = dbEx;
                        foreach (var validationErrors in dbEx.EntityValidationErrors)
                        {
                            foreach (var validationError in validationErrors.ValidationErrors)
                            {
                                string message = string.Format("{0}:{1}", validationErrors.Entry.Entity.ToString(), validationError.ErrorMessage);
                                //raise a new exception inserting the current one as the InnerException
                                raise = new InvalidOperationException(message, raise);
                            }
                        }
                        WebLog.Log(raise);
                    }
            }


            catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
            {
                Exception raise = dbEx;
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        string message = string.Format("{0}:{1}", validationErrors.Entry.Entity.ToString(), validationError.ErrorMessage);
                        //raise a new exception inserting the current one as the InnerException
                        raise = new InvalidOperationException(message, raise);
                    }
                }
                WebLog.Log(raise);
            }

            return View("ImportExcel1");

        }

        public string GetMenus()
        {
            try
            {
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                if (appUser == null)
                {
                    return null;
                }

                var mc = _DR.getUserID(appUser);

                //the line does the same thing
                // var ids = (from a in gb.UserRoles where a.UserId == mc.id select a.RoleId).ToList();
                var ids = _DR.getUserRols(mc);
                //var roles = DataReaders.getUserRoles(ids.Cast<int>().ToList());
                var roles = _DR.getUserRoles(ids.Cast<int>().ToList());
                foreach (var item in roles)
                { rol.Add(item.RoleName); }
                var results = _DR.getResults(rol);
                //var Menus = results.ToList().Distinct().GroupBy(k => (k.pageName)).OrderBy(k => k.Key).ToDictionary(k => k.Key, v => v.ToList());
                var Menus = results.ToList().Distinct().GroupBy(k => (k.pageheader)).OrderBy(k => k.Key).ToDictionary(k => k.Key, v => v.ToList());
                return ViewBag.Menus = Menus;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


        public ActionResult CompanyProfile()
        {
            return View();
        }
        //public ActionResult Acknowledgement()
        //{
        //    return View();
        //}
        [HttpGet]
        public ActionResult CheckEligibility()
        {
            GetMenus();
            ViewBag.Institution = db.Institutions.ToList();
            return View();
        }

        [HttpPost]
        public ActionResult CheckEligibility(LoanViewModel lvm, FormCollection form, AppLoans Apploan)
        {

            {
                ViewBag.Institution = db.Institutions.ToList();
                StdRec.Institution_FK = Convert.ToInt32(form["CheckEligibility"]);
                StdRec.MatriculationNumber = lvm.StudentRecord.MatriculationNumber;
                StdRec.PhoneNumber = lvm.StudentRecord.PhoneNumber;
                Apploan = _DR.CheckInstitution(StdRec);

                //ViewBag.Firstname = Rec.Firstname;
                //ViewBag.data = Rec.Lastname;
                //ViewBag.data = Rec.PhoneNumber;
                //ViewBag.data = Rec.Faculty;
                //ViewBag.data = Rec.AmountBorrowed;
                //db.StudentRecords.Add(StdRec);
                // db.SaveChanges();
            }
            GetMenus();
            return View("CheckEligibility2", Apploan);
        }

        [HttpGet]
        public ActionResult CheckEligibility2(LoanViewModel lvm)
        {
            GetMenus();
            return View();
        }

        [HttpPost]
        public ActionResult CheckEligibility2(FormCollection form, LoanViewModel lvm)
        {
            GetMenus();
            return View();

        }


        [HttpGet]
        public ActionResult CheckLoanStatus()
        {
            GetMenus();

            return View();
        }

        [HttpPost]

        public ActionResult CheckLoanStatus(FormCollection form, LoanViewModel lvm)
        {
            //LoanApplication lonobj = new LoanApplication();
            try

            {
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }
                else
                {
                    var ApplicationFk = Convert.ToString(form["RefNumber"]);
                    ViewBag.Data = _DR.CheckLoanStatus(ApplicationFk);
                     
                }
                GetMenus();
                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        //[HttpGet]
        //public ActionResult LoanRepayment()
        //{
        //    GetMenus();
        //    // ViewBag.Data = _DR.CheckAppStatus(ApplicationFk);
        //    var to = DateTime.Today;
        //    var From = DateTime.Today.AddDays(-100);

        //    ViewBag.Data = _DR.LoanRepayment(to, From);
        //    return View();
        //}

        //[HttpPost]
        //public ActionResult LoanRepayment(FormCollection form)
        //{
        //    try

        //    {
        //        var LoggedInuser = new LogginHelper();
        //        user = LoggedInuser.LoggedInUser();

        //        var appUser = user;
        //        if (appUser == null)
        //        {
        //            return RedirectToAction("/", "Home");
        //        }
        //        else
        //        {
        //            var to = DateTime.Today;
        //            var From = DateTime.Today.AddDays(-10);

        //            ViewBag.Data = _DR.LoanRepayment(to, From);
        //        }
        //        GetMenus();
        //        return View();
        //    }
        //    catch (Exception ex)
        //    {
        //        WebLog.Log(ex.Message.ToString());
        //        return null;
        //    }

        //}

        //[HttpGet]
        //public ActionResult LoanTransactionbyDate()
        //{
        //    GetMenus();
        //    // ViewBag.Data = _DR.CheckAppStatus(ApplicationFk);
        //    var to = DateTime.Today;
        //    var From = DateTime.Today.AddDays(-100);

        //    ViewBag.Data = _DR.LoanTransactionbyDate(to, From);
        //    return View();
        //}


        //[HttpPost]
        //public ActionResult LoanTransactionbyDate(FormCollection form)
        //{
        //    try

        //    {
        //        var LoggedInuser = new LogginHelper();
        //        user = LoggedInuser.LoggedInUser();

        //        var appUser = user;
        //        if (appUser == null)
        //        {
        //            return RedirectToAction("/", "Home");
        //        }
        //        else
        //        {
        //            var to = DateTime.Today;
        //            var From = DateTime.Today.AddDays(-10);

        //            ViewBag.Data = _DR.LoanTransactionbyDate(to, From);
        //        }
        //        GetMenus();
        //        return View();
        //    }
        //    catch (Exception ex)
        //    {
        //        WebLog.Log(ex.Message.ToString());
        //        return null;
        //    }

        //}



        [HttpGet]
        public ActionResult EditPassword()
        {
            try
            {
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = ""; TempData["Error"] = "";
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("Index", "Home");
                }
                GetMenus();
                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        [HttpPost]
        public ActionResult EditPassword(FormCollection form)
        {
            try
            {
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = ""; TempData["Error"] = "";
                string password = "";
                var confirmPassword = "";
                string EncrypPassword = "";
                User users = new User();
                if (Session["id"] != null || Session["id"].ToString() != "")
                {
                    password = Convert.ToString(form["Password"]);
                    users.PaswordVal = Convert.ToString(form["newPassword"]);
                    confirmPassword = Convert.ToString(form["confirmPassword"]);
                    EncrypPassword = new CryptographyManager().ComputeHash(password, HashName.SHA256);
                }
                if (confirmPassword != users.PaswordVal)
                {
                    TempData["ErrMsg"] = "New Password and Confirm Password Must Match";

                    return View();
                }
                else
                {
                    var NewEncrypPassword = new CryptographyManager().ComputeHash(users.PaswordVal, HashName.SHA256);
                    users.PaswordVal = NewEncrypPassword;
                    users.EmailAddress = Session["id"].ToString();
                    var valid = _DR.loggedIn(users.EmailAddress, EncrypPassword);
                    users.PaswordVal = NewEncrypPassword;

                    string Email = users.EmailAddress;
                    User id = _DR.getUser(Email);
                    users.ID = id.ID;
                    if (valid == true)
                    {

                        _DR.UpdatePassword(users);
                        TempData["ErrMsg"] = "Password Succesfully Updated";
                        GetMenus();
                    }
                    else
                    {
                        TempData["ErrMsg"] = "Please Try Again";
                    }

                }
                GetMenus();
                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }



        //[HttpGet]
        //public ActionResult CheckNYSCLoan()
        //{

        //    GetMenus();

        //    return View();
        //}


        //[HttpPost]
        //public ActionResult CheckNYSCLoan(FormCollection form, LoanViewModel lvm)
        //{
        //    NyscLoanApplication lonobj = new NyscLoanApplication();

        //        try

        //        {
        //            GetMenus();
        //            var LoggedInuser = new LogginHelper();
        //            user = LoggedInuser.LoggedInUser();

        //            var appUser = user;
        //            if (appUser == null)
        //            {
        //                return RedirectToAction("/", "Home");
        //            }
        //            else
        //            {
        //                var ApplicationFk = Convert.ToString(form["RefNumber"]);

        //                DataAccessA.Classes.AppLoanss Apploan = _DR.CheckAppStatus(ApplicationFk);
        //                if (Apploan == null)
        //                {
        //                    TempData["ErrMsg"] = "Record Not Found! ";
        //                    return View();
        //                }

        //                return View(Apploan);
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            WebLog.Log(ex.Message.ToString());
        //            TempData["ErrMsg"] = "Record Not Found! ";
        //            return null;
        //        }
        //    }
            

        [HttpGet]
        public ActionResult CheckAppStatus()
        {
            TempData["SucMsg"] = ""; TempData["ErrMsg"] = ""; TempData["Error"] = "";
            GetMenus();
            // ViewBag.Data = _DR.CheckAppStatus(ApplicationFk);
            return View();
        }


        [HttpPost]
        public ActionResult CheckAppStatus(FormCollection form, LoanViewModel lvm)
        {
            //LoanApplication lonobj = new LoanApplication();
            try

            {
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = ""; TempData["Error"] = "";
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }
                else
                {
                    var ApplicationFk = Convert.ToString(form["RefNumber"]);
                    ViewBag.Data = _DR.CheckAppStatus(ApplicationFk);
                    // TempData["LoanAppp"] = _DR.CheckAppStatus(ApplicationFk);
                    /// lvm.LoanApplication = TempData["LoanAppp"];
                }
                GetMenus();
                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


        [HttpGet]
        public ActionResult AllApprovedLoans()
        {


            GetMenus();
            ViewBag.ApplicationStatus = db.ApplicationStatus.ToList();
            ApplicationFk = 2;
            ViewBag.Data = _DR.ApprovedloanReport(ApplicationFk);
            Session["AllTransaction"] = ViewBag.Data;
            //ViewBag.Data = _DR.LoanApRep();
            // var List = ViewBag.Data.Count();

            return View();
        }

        [HttpPost]
        public ActionResult AllApprovedLoans(FormCollection form)
        {
            //LoanApplication lonobj = new LoanApplication();
            try

            {
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }
                else
                {
                    ViewBag.ApplicationStatus = db.ApplicationStatus.ToList();
                    ApplicationFk = Convert.ToInt32(form["ApplicationStatus"]);
                    ViewBag.Data = _DR.ApprovedloanReport(ApplicationFk);
                    Session["AllTransaction"] = ViewBag.Data;
                }
                GetMenus();
                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        [HttpGet]
        public ActionResult LoanReport()
        {
            GetMenus();
            // ViewBag.Data = _DR.LoanAppReport();

            return View();
        }



        [HttpPost]
        public ActionResult LoanReport(DataAccessA.DataManager.LoanApplication loanApp)
        {
            try
            {
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }
                else
                {
                    ViewBag.Data = _DR.LoanAppReport();
                }
                // GetMenus();
                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        public ActionResult SetupMandate()

        {
            GetMenus();
            var BanlList = _DR.getAllRemitaBanks();
            ViewBag.BankList = BanlList;
            //var Drp = new List<SelectListItem>
            //  {
            //new SelectListItem{ Text="Daily", Value = "0", Selected = true },
            //new SelectListItem{ Text="DD", Value = "DD" },
            //new SelectListItem{ Text="MM", Value = "Monthly" },

            //};
            //     ViewData["DrpLst"] = Drp;
            return View();
        }

        [HttpPost]
        public ActionResult SetupMandate(FormCollection form)
        {

            DataReader dr = new DataReader();
            try
            {
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();
                GetMenus();
                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }
                else
                {
                    dynamic jobj = new JObject();
                    jobj.payerName = Convert.ToString(form["payerName"]);
                    jobj.payerEmail = Convert.ToString(form["payerEmail"]);
                    jobj.payerPhone = Convert.ToString(form["payerPhone"]);
                    jobj.payerBankCode = Convert.ToString(form["payerBankCode"]);
                    jobj.payerAccount = Convert.ToString(form["payerAccount"]);
                    jobj.amount = Convert.ToString(form["amount"]);
                    jobj.startDate = Convert.ToString(form["StartDate"]);
                    jobj.endDate = Convert.ToString(form["EndDate"]);
                    jobj.mandateType = Convert.ToString(form["MandateType"]);

                    jobj.maxNoOfDebits = Convert.ToString(form["maxNoOfDebits"]);
                    var json = jobj.ToString();
                    var url = ConfigurationManager.AppSettings["MandateSetUpTest"];
                    string data = Helper.DoPost(url, "", "", "", "", "", "", json);


                    data = data.Replace("jsonp (", "");
                    data = data.Replace("})", "}");
                    dynamic myJobj = JObject.Parse(data);

                    TempData["RefNum"] = myJobj.remitaTransRef;
                }
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
            }

            return View("OTPValidation");
        }


        [HttpGet]
        public ActionResult OTPValidation()
        {
            GetMenus();
            return View();
        }
        [HttpPost]
        public ActionResult OTPValidation(FormCollection form)
        {

            try
            {
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();
                GetMenus();
                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }
                else
                {
                    dynamic jobj = new JObject();
                    jobj.otp = Convert.ToString(form["otp"]);
                    jobj.cardNo = Convert.ToString(form["cardNo"]);
                    jobj.remitaRef = Convert.ToString(form["RefNum"]);
                    var json = jobj.ToString();
                    var url = ConfigurationManager.AppSettings["OTPvalidationTest"];
                    string data = Helper.DoPost(url, "", "", "", "", "", "", json);

                    data = data.Replace("jsonp (", "");
                    data = data.Replace("})", "}");
                    dynamic myJobj = JObject.Parse(data);

                    TempData["MandateID"] = myJobj.mandateID;

                }
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
            }
            return RedirectToAction("DebitInstruction");
        }

        [HttpGet]
        public ActionResult DebitInstruction()
        {
            GetMenus();
            var BanlList = _DR.getAllRemitaBanks();
            ViewBag.BankList = BanlList;
            return View();
        }

        [HttpPost]
        public ActionResult DebitInstruction(FormCollection form)
        {

            try
            {
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();
                GetMenus();
                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }
                else
                {
                    dynamic jobj = new JObject();
                    jobj.totalAmount = Convert.ToString(form["totalAmount"]);
                    jobj.mandateId = Convert.ToString(form["MandateID"]);
                    jobj.fundingAccount = Convert.ToString(form["fundingAccount"]);
                    jobj.fundingBankCode = Convert.ToString(form["fundingBankCode"]);
                    var json = jobj.ToString();
                    var url = ConfigurationManager.AppSettings["DebitInstructionTest"];

                    string data = Helper.DoPost(url, "", "", "", "", "", "", json);
                    dynamic jObj = JObject.Parse(data);


                    TempData["requestID"] = jObj.requestID;
                }
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
            }

            return RedirectToAction("DebitInstruction");
        }
        [HttpGet]
        public ActionResult DebitStatus()
        {
            GetMenus();
            return View();
        }

        [HttpPost]
        public ActionResult DebitStatus(FormCollection form)
        {

            try
            {
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();
                GetMenus();
                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }
                else
                {
                    dynamic jobj = new JObject();

                    jobj.mandateId = Convert.ToString(form["mandateId"]);
                    jobj.requestId = Convert.ToString(form["requestID"]);
                    var json = jobj.ToString();
                    var url = ConfigurationManager.AppSettings["DebitStatusTest"];
                    string data = Helper.DoPost(url, "", "", "", "", "", "", json);
                    dynamic jObj = JObject.Parse(data);

                    TempData["value1"] = jObj.amount;
                    TempData["value2"] = jObj.RRR;
                    TempData["value3"] = jObj.description;
                }
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
            }
            return View();
        }
        [HttpGet]
        public ActionResult CheckMandateStatus()
        {
            GetMenus();
            return View();
        }

        [HttpPost]
        public ActionResult CheckMandateStatus(FormCollection form)
        {

            try
            {
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();
                GetMenus();
                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }
                else
                {
                    dynamic jobj = new JObject();

                    jobj.mandateId = Convert.ToString(form["mandateId"]);
                    jobj.requestId = Convert.ToString(form["requestId"]);
                    var json = jobj.ToString();
                    var url = ConfigurationManager.AppSettings["MandateStatusTest"];
                    string data = Helper.DoPost(url, "", "", "", "", "", "", json);

                    dynamic jObj = JObject.Parse(data);

                    TempData["value1"] = jObj.Startdate;
                    TempData["value2"] = jObj.Enddate;
                    TempData["value3"] = jObj.RegistrationDate;
                    TempData["value4"] = jObj.isActive;
                    TempData["value5"] = jObj.description;

                }
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
            }
            return View();
        }



        //[HttpPost]
        //public ActionResult GetLoanParam(string Tenure)
        //{
        //    try
        //    {
        //        Institution Inst = new Institution();
        //        string respmsg = ""; ;
        //       // double repaymentmsg = 0;
        //        bool valid = false;


        //        int Tenures = Convert.ToInt16(Tenure);

        //        // List<LRepay> LoanRecords = _DR.GetloanParam(LoanTenure);
        //        NYSCLoanSetUp Nysc = new NYSCLoanSetUp();
        //        Nysc = _DR.GetloanParam(Tenures);
        //        if(Nysc != null )
        //        {
        //             valid = true;
        //          //  return valid ;
        //        }
        //        else
        //        {
        //            valid = false;
        //        }
        //        return Json(new { response = valid, Data = respmsg, Loanrec = Nysc });
        //    }
        //    catch (Exception ex)
        //    {
        //        WebLog.Log(ex.Message.ToString());
        //        return null;
        //    }
        //}

        //public ActionResult Acknowledgement(string Refid)

        //{
        //    MyUtility utilities = new MyUtility();
        //    if (Refid == null || Refid == "")
        //    {
        //        return RedirectToAction("/");
        //    }
        //    var LoanApps = _DR.LoanDetails(Refid);
        //    string LoanAmount = LoanApps.LoanAmount.ToString();
        //    LoanApps.ConvertedAmount = MyUtility.ConvertToCurrency(LoanAmount);
        //    if (LoanApps == null)
        //    {
        //        return RedirectToAction("/");
        //    }

        //    return View(LoanApps);
        //}



        //[HttpGet]
        //public ActionResult NYSCLoanAppForm()


        //{
        //    GetMenus();
        //    ViewBag.nBanks = _DR.GetBanks();
        //    ViewBag.nRepmtMethods = _DR.GetRepaymentMethods();
        //    ViewBag.nStates = _DR.GetNigerianStates();
        //    ViewBag.nMeansOfIDs = _DR.GetMeansOfIdentifications();
        //    ViewBag.nAccomodationTypes = _DR.GetAccomodationTypes();
        //    ViewBag.nLGAs = _DR.GetAllLGAs();
        //    ViewBag.nTitles = _DR.GetTitles();
        //    ViewBag.nMarital = _DR.GetMaritalStatus();
        //    int val = 0;
        //    ViewData["nTitles"] = new SelectList(_DR.GetTitles(), "ID", "NAME", val);
        //    ViewData["nMarital"] = new SelectList(_DR.GetMaritalStatus(), "ID", "NAME", val);
        //    ViewData["nLGAs"] = new SelectList(_DR.GetAllLGAs(), "ID", "NAME", val);

        //    ViewData["nAccomodationTypes"] = new SelectList(_DR.GetAccomodationTypes(), "ID", "NAME", val);
        //    ViewData["nMeansOfIDs"] = new SelectList(_DR.GetMeansOfIdentifications(), "ID", "NAME", val);
        //    ViewData["nStates"] = new SelectList(_DR.GetNigerianStates(), "ID", "NAME", val);
        //    ViewData["nBanks"] = new SelectList(_DR.GetBanks(), "Code", "NAME", val);
        //    ViewData["nRepmtMethods"] = new SelectList(_DR.GetRepaymentMethods(), "ID", "NAME", val);
        //    ViewData["nLoanTenure"] = new SelectList(_DR.GetAllTenure(), "ID", "Tenure", val);
        //    ViewData["nGender"] = new SelectList(GetAllGender(), "Value", "Text", val);
        //    ViewData["nemploymentStatus"] = new SelectList(GetAppStatus(), "Value", "Text", val);
        //    return View();
        //}

        //public List<SelectListItem> GetAllGender()
        //{
        //    try
        //    {
        //        List<SelectListItem> Gender = new List<SelectListItem>();
        //        Gender.Add(new SelectListItem { Value = "1", Text = "Male" });
        //        Gender.Add(new SelectListItem { Value = "2", Text = "Female" });
        //        return Gender;
        //    }
        //    catch (Exception ex)
        //    {
        //        WebLog.Log(ex.Message.ToString());
        //        return null;
        //    }
        //}

        //public List<SelectListItem> GetAppStatus()
        //{
        //    try
        //    {

        //        List<SelectListItem> Contract = new List<SelectListItem>();
        //        Contract.Add(new SelectListItem { Value = "1", Text = "Contract" });
        //        Contract.Add(new SelectListItem { Value = "2", Text = "Parmanent" });
        //        return Contract;
        //    }
        //    catch (Exception ex)
        //    {
        //        WebLog.Log(ex.Message.ToString());
        //        return null;
        //    }
        //}
        //[HttpPost]
        //public ActionResult NYSCLoanAppForm(FormCollection form, TableObjects.LoanApplication lApObjs, DataAccessA.Classes.LoanApplication lApObj)
        //{
        //    try
        //    {
        //        GetMenus();
        //        var LoggedInuser = new LogginHelper();
        //        user = LoggedInuser.LoggedInUser();
        //        int userid = LoggedInuser.LoggedInUserID(user);
        //        var Marital = lApObj.MaritalStatus_FK;
        //        var Tittle = lApObj.Title_FK;
        //        //Marital = _DR.GetMaritalStatus(Convert.ToString(Marital));
        //        var lgaList = Convert.ToInt16(form["lgaList"]);
        //        var lgaLists = Convert.ToInt16(form["lgaLists"]);
        //        var lgaListss = Convert.ToInt16(form["lgaListsss"]);
        //        //var LGA = _DR.GetLocalGovt(Convert.ToString(form["lgaList"]));
        //        //var MeansOfID_FK = Convert.ToInt32(form["meansOfID"]);

        //        var Gender_FK = lApObj.Gender_FK;//Convert.ToInt32(form["selectGender"]);
        //        //var AccomodationType_FK = Convert.ToInt32(form["AccomodationTypes"]);
        //        string DOB = lApObj.DateOfBirth.ToString();
        //        //DataAccessA.Classes.LoanApplication lnapp = new DataAccessA.Classes.LoanApplication
        //        DataAccessA.DataManager.NyscLoanApplication NyscLA = new DataAccessA.DataManager.NyscLoanApplication
        //        {
        //            AccountNumber = lApObj.AccountNumber,
        //            AccountName = lApObj.AccountName,
        //            Firstname = lApObj.Firstname,
        //            ApplicationStatus_FK = 3,
        //            RefNumber = "NYSC"+MyUtility.GenerateRefNo(),
        //            Gender_FK = Gender_FK,//Convert.ToInt32(form["selectGender"]),
        //            MaritalStatus_FK = lApObj.MaritalStatus_FK,//Convert.ToInt16(form["Marital"]),
        //            Surname = lApObj.Surname,
        //            CreatedBy = Convert.ToString(userid),
        //            DateOfBirth = Convert.ToString(lApObj.DateOfBirth),
        //            Title_FK = lApObj.Title_FK,//Convert.ToInt32(form["Titles"]),
        //            PhoneNumber = lApObj.PhoneNumber,
        //            EmailAddress = lApObj.EmailAddress,
        //            PermanentAddress = lApObj.PermanentAddress,
        //            Landmark = lApObj.Landmark,
        //            ClosestBusStop = lApObj.ClosestBusStop,
        //            LGA_FK =  Convert.ToInt16(form["lgaList"]),
        //            TempLGA_FK =Convert.ToInt16(form["lgaLists"]),
        //            NyscLGA_FK = Convert.ToInt16(form["lgaListsss"]),
        //            StateofResidence_FK =lApObj.StateofResidence_FK ,////Convert.ToInt32(form["States"]),
        //            TempStateofResidence_FK = lApObj.TempStateofResidence_FK,//Convert.ToInt32(form["States"]),
        //            NyscStateofResidence_FK = lApObj.NyscStateofResidence_FK,//Convert.ToInt32(form["States"]),
        //            TemporaryAddress = lApObj.TemporaryAddress,
        //            OfficialAddress = lApObj.OfficialAddress,
        //            StateCode = lApObj.StateCode,
        //            Employer = lApObj.Employer,
        //            PassOutMonth = lApObj.PassOutMonth,
        //            CDSDay = lApObj.CDSDay,
        //            TempLandmark = lApObj.TempLandmark,
        //            TempClosestBusStop = lApObj.TempClosestBusStop,

        //            BVN = lApObj.BVN,
        //            CDSGroup = lApObj.CDSGroup,
        //            NetMonthlyIncome = Convert.ToDouble(lApObj.NetMonthlyIncome),
        //            EMG_EmailAddress = lApObj.EMG_EmailAddress,
        //            EMG_FullName = lApObj.EMG_FullName,
        //            EMG_HomeAddress = lApObj.EMG_HomeAddress,
        //            EMG_PhoneNumber = lApObj.EMG_PhoneNumber,
        //            EMG_Relationship = lApObj.EMG_Relationship,
        //            LoanAmount = Convert.ToDouble(lApObj.LoanAmount),
        //            LoanTenure = lApObj.LoanTenure,
        //            ExistingLoan = lApObj.ExistingLoan,
        //            ExistingLoan_NoOfMonthsLeft = lApObj.ExistingLoan_NoOfMonthsLeft,
        //            ExistingLoan_OutstandingAmount = lApObj.ExistingLoan_OutstandingAmount,
        //            BankCode = lApObj.BankCode,//Convert.ToString(form["Bank"]),
        //            IsVisible = 1,
        //            DateCreated = MyUtility.getCurrentLocalDateTime(),
        //            DateModified = MyUtility.getCurrentLocalDateTime(),
        //            ValueDate = MyUtility.getCurrentLocalDateTime().ToString("yyyy/MM/dd"),
        //            ValueTime = MyUtility.getCurrentLocalDateTime().ToString("H:mmss"),
        //        };
        //       var id = DataWriter.CreateNYSCLoanApplication(NyscLA);

        //        if (id == 0)
        //        {
        //            TempData["Error"] = "Please Check Next Of kin Phone Number";
        //            NYSCLoanAppForm();
        //             ViewData["nTitles"] = new SelectList(_DR.GetTitles(), "ID", "NAME", lApObj.Title_FK);
        //             ViewData["nMarital"] = new SelectList(_DR.GetMaritalStatus(), "ID", "NAME", lApObj.MaritalStatus_FK);
        //             ViewData["nLGAs"] = new SelectList(_DR.GetAllLGAs(), "ID", "NAME", lApObj.LGA_FK);

        //             ViewData["nAccomodationTypes"] = new SelectList(_DR.GetAccomodationTypes(), "ID", "NAME", lApObj.AccomodationType_FK);
        //             ViewData["nMeansOfIDs"] = new SelectList(_DR.GetMeansOfIdentifications(), "ID", "NAME", lApObj.MeansOfID_FK);
        //             ViewData["nStates"] = new SelectList(_DR.GetNigerianStates(), "ID", "NAME", lApObj.StateofResidence_FK);
        //             ViewData["nBanks"] = new SelectList(_DR.GetBanks(), "Code", "NAME", lApObj.Bank_FK);
        //             ViewData["nRepmtMethods"] = new SelectList(_DR.GetRepaymentMethods(), "ID", "NAME", lApObj.RepaymentMethod_FK);
        //             ViewData["nLoanTenure"] = new SelectList(_DR.GetAllTenure(), "ID", "LoanTenure", lApObj.LoanTenure);
        //             ViewData["nGender"] = new SelectList(GetAllGender(), "Value", "Text", lApObj.Gender_FK);
        //             ViewData["nemploymentStatus"] = new SelectList(GetAppStatus(), "Value", "Text", lApObj.Contract);
        //            return View("NYSCLoanAppForm", NyscLA);
        //        }

        //        if (id != 0)
        //        {
        //            return RedirectToAction("Acknowledgement", new { @Refid = NyscLA.RefNumber });
        //        }
        //        return View();
        //    }
        //    catch (Exception ex)
        //    {
        //        WebLog.Log(ex.Message.ToString());
        //        return null;
        //    }
        //}
        ////Admin Side


        //public void SendNextLevelEmail(string user, string instEmail)
        //{
        //    try
        //    {

        //        var bodyTxt = System.IO.File.ReadAllText(HostingEnvironment.MapPath("~/EmailNotifications/NextLevelEmailNotification.html"));
        //        //bodyTxt = bodyTxt.Replace("$UserName",user).Replace("$MerchantName", $"{lvm.Firstname} {lvm.Surname}").Replace("$LoanComment", $"{lvm.LoanComment}");
        //        bodyTxt = bodyTxt.Replace("$UserName", user);

        //        var msgHeader = $"Welcome to Uvlot";
        //        var sendMail = NotificationService.SendMail(msgHeader, bodyTxt, user, instEmail, null);
        //    }
        //    catch (Exception ex)
        //    {
        //        WebLog.Log(ex.Message.ToString());
        //    }
        //}

        //public static bool SendMail(string msgSubject, string msgBody, string addressTo, string addressCc, string addressBcc)
        //{

        //    try
        //    {
        //        ServicePointManager.ServerCertificateValidationCallback +=
        //           (sender, certificate, chain, sslPolicyErrors) => true;

        //        var smtpServer = ConfigurationManager.AppSettings["MailServerAddress"];
        //        WebLog.Log("smtpServer:" + smtpServer);
        //        var smtpServerPort = ConfigurationManager.AppSettings["SMTPServerPort"];
        //        WebLog.Log("SMTPServerPort:" + smtpServerPort);
        //        var mailFrom = ConfigurationManager.AppSettings["MailFrom"];
        //        WebLog.Log("mailFrom:" + mailFrom);
        //        var mailFromPassword = ConfigurationManager.AppSettings["MailFromPassword"];
        //        WebLog.Log("mailFromPassword:" + mailFromPassword);

        //        var email = new MailMessage
        //        {
        //            From = new MailAddress(mailFrom, msgSubject),
        //            Subject = msgSubject,
        //            IsBodyHtml = true,
        //            Body = msgBody,
        //        };
        //        WebLog.Log("email:" + email.From + email.Subject + email.IsBodyHtml + email.Body);
        //        email.Attachments.Add(new Attachment(addressBcc));
        //        //mm.Attachments.Add(New Attachment(New MemoryStream(bytes), "iTextSharpPDF.pdf"));
        //        email.To.Add(addressTo);
        //        WebLog.Log("email:" + email);
        //        if (addressCc != null) email.CC.Add(addressCc);
        //        if (addressBcc != null) email.Bcc.Add(addressBcc);

        //        var mailClient = new SmtpClient();

        //        var basicAuthenticationInfo = new NetworkCredential(mailFrom, mailFromPassword);
        //        WebLog.Log("basicAuthenticationInfo:" + basicAuthenticationInfo);
        //        mailClient.Host = smtpServer;
        //        WebLog.Log("mailClient.Host:" + mailClient.Host);
        //        mailClient.Credentials = basicAuthenticationInfo;
        //        WebLog.Log("mailClient.Credentials:" + mailClient.Credentials);
        //        mailClient.Port = int.Parse(smtpServerPort);
        //        WebLog.Log("mailClient.Port:" + mailClient.Port);
        //        mailClient.EnableSsl = bool.Parse(ConfigurationManager.AppSettings["EnableSsl"]);
        //        WebLog.Log("mailClient.EnableSsl:" + mailClient.EnableSsl);
        //        mailClient.Send(email);


        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        WebLog.Log(ex);
        //    }
        //    return false;
        //}


        [HttpPost]
        public JsonResult getLGAsByStateID(int id)
        {
            var ddlLGA = _DR.GetLGAsByStateFK(id).ToList();
            List<SelectListItem> liLGAs = new List<SelectListItem>();

            liLGAs.Add(new SelectListItem { Text = "SELECT LGA", Value = "0" });
            if (ddlLGA != null)
            {
                foreach (var x in ddlLGA)
                {
                    liLGAs.Add(new SelectListItem { Text = x.Name, Value = x.ID.ToString() });
                }
            }
            return Json(new SelectList(liLGAs, "Value", "Text", JsonRequestBehavior.AllowGet));
        }

        [HttpGet]
        public ActionResult AllNYSCLoanReport(DataAccessA.DataManager.NyscLoanApplication loanApp)
            

        {
            try
            {
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = ""; TempData["Error"] = "";
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }
                else
                {
                    ViewBag.Data = _DR.NYSCLoanAppReport();
                }
                Session["AllTransaction"] = ViewBag.Data;
                GetMenus();
                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        [HttpGet]
        public ActionResult Ex2Excel()
        {
            ExportToExcel();
            GetMenus();
            return View("AllNYSCLoanReport");
            //return View("");
        }


        [HttpGet]
        public ActionResult CheckNYSCLoan()
        {
            TempData["SucMsg"] = ""; TempData["ErrMsg"] = ""; TempData["Error"] = "";

            GetMenus();
            // ViewBag.Data = _DR.CheckAppStatus(ApplicationFk);

            return View();
        }
        

        [HttpPost]
        public ActionResult CheckNYSCLoan(FormCollection form, LoanViewModel lvm)
        {
            NyscLoanApplication lonobj = new NyscLoanApplication();

            try

            {
               
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = ""; TempData["Error"] = "";

                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }
                string msg = "";
                var ApplicationFk = Convert.ToString(form["RefNumber"]);

                DataAccessA.Classes.AppLoanss Apploan = _DR.CheckAppStatus(ApplicationFk);
                Apploan.RepaymentAmount = _DR.GetRepaymenrAmount(Apploan.LoanTenure);
                Apploan.RepaymentAmount = MyUtility.ConvertToCurrency(Apploan.RepaymentAmount);
                if (Apploan == null)
                {
                    TempData["ErrMsg"] = "Record Not Found! ";
                    return View();
                }
                GetMenus();
                return View(Apploan);

            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                TempData["ErrMsg"] = "Record Not Found! ";
                return null;
            }
        }

        [HttpGet]
        // 
        public ActionResult EditMyApplication()

        {

            try
            {
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = ""; TempData["Error"] = "";
                AppStatFk = 11;
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }
                
                     var recObj = _DR.ApproveLoansss(AppStatFk, appUser);
                //var recObj = _DR.EditApprovedLoans(AppStatFk, appUser);

                List<AppLoanss> appLoanList = new List<AppLoanss>();
                foreach (AppLoanss app in recObj)
                {
                    app.ConvertedAmount = MyUtility.ConvertToCurrency(app.LoanAmount.ToString());
                    appLoanList.Add(app);
                }

                ViewBag.Data = appLoanList;
                GetMenus();
                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


        public void GetApplicantInfoss(string Refid, TableObjects.LoanApplication LoanApp, AppLoanss Apploan, AppLoanss LoanApps, NyscLoanApplication nysc)
        {
            try
            {
                NYSCLOANController nyscc = new NYSCLOANController();

                nysc.NyscIdCardFilePath = nysc.NyscIdCardFilePath;
                nysc.STA_FilePath = nysc.STA_FilePath;
                nysc.NyscpassportFilePath = nysc.NyscpassportFilePath;
                nysc.NyscCallUpLetterFilePath = nysc.NyscCallUpLetterFilePath;

                nysc.NyscPostingLetterFllePath = nysc.NyscPostingLetterFllePath;
                nysc.NyscProfileDashboardFilePath = nysc.NyscProfileDashboardFilePath;
              
                string IdentImage = nysc.NyscIdCardFilePath.After("h:\\root\\home\\paelyt-001\\www\\paelyt\\uvlotapublish\\").Replace("\\", "/");
                string IdentImages = nysc.STA_FilePath.After("h:\\root\\home\\paelyt-001\\www\\paelyt\\uvlotapublish\\").Replace("\\", "/");
                string IdentImagess = nysc.NyscpassportFilePath.After("h:\\root\\home\\paelyt-001\\www\\paelyt\\uvlotapublish\\").Replace("\\", "/");
                string IdentImgs = nysc.NyscPostingLetterFllePath.After("h:\\root\\home\\paelyt-001\\www\\paelyt\\uvlotapublish\\").Replace("\\", "/");
                string IdentImgss = nysc.NyscCallUpLetterFilePath.After("h:\\root\\home\\paelyt-001\\www\\paelyt\\uvlotapublish\\").Replace("\\", "/");
                string IdentImgsss = nysc.NyscProfileDashboardFilePath.After("h:\\root\\home\\paelyt-001\\www\\paelyt\\uvlotapublish\\").Replace("\\", "/");
                //h:\root\home\paelyt - 001\www\paelyt\uvlotapublish\Images\PowerScreenGrab.PNG
                //string IdentImage = LoanApps.IdentficationNumberImage.After("h:\\root\\home\\paelyt-001\\www\\Uvlot\\").Replace("\\", "/");
                WebLog.Log("Image Path" + IdentImage);
                string slash = "/";
                nysc.NyscIdCardFilePath = string.IsNullOrEmpty(IdentImage) ? "none" : slash + IdentImage;
                nysc.STA_FilePath = string.IsNullOrEmpty(IdentImages) ? "none" : slash + IdentImages;
                nysc.NyscpassportFilePath = string.IsNullOrEmpty(IdentImagess) ? "none" : slash + IdentImagess;
                nysc.NyscPostingLetterFllePath = string.IsNullOrEmpty(IdentImgs) ? "none" : slash + IdentImgs;
                nysc.NyscCallUpLetterFilePath = string.IsNullOrEmpty(IdentImgss) ? "none" : slash + IdentImgss;
                nysc.NyscProfileDashboardFilePath = string.IsNullOrEmpty(IdentImgsss) ? "none" : slash + IdentImgsss;
                 
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
            }
        }


        //[HttpGet]
        //public ActionResult NYSCLoanAppFormEdit(string Refid, TableObjects.LoanApplication LoanApp, DataAccessA.Classes.AppLoanss Apploan, AppLoanss LoanApps, NyscLoanApplication nysc)
        //{
        //    TempData["SucMsg"] = ""; TempData["ErrMsg"] = ""; TempData["Error"] = "";
        //    var data = _DR.GetRefid(Refid);
        //    nysc = data;
        //    nysc.NyscStateofResidence_FK = 0;
        //    nysc.StateofResidence_FK = 0;
        //    nysc.TempStateofResidence_FK = 0;
        //    nysc.PassOutMonth = "";
        //    var Nyscfileimg = nysc.STA_FilePath;
        //    if (Nyscfileimg == null || Nyscfileimg == "")
        //    {

        //        Nyscfileimg = nysc.STA_FilePath;

        //    }

        //    WebLog.Log("ENTERED");
        //    TempData["Error"] = "";
        //    string referralCode = Request.QueryString["referralCode"];
        //    WebLog.Log("refelcode" + referralCode);
        //    if (referralCode != null || referralCode != "")
        //    {
        //        WebLog.Log("refelcode0" + referralCode);
        //        nysc.ReferralCode = referralCode;
        //        WebLog.Log("refelcode1" + referralCode);
        //    }

        //    ViewBag.nBanks = _DR.GetBanks();
        //    ViewBag.nRepmtMethods = _DR.GetRepaymentMethods();
        //    ViewBag.nStates = _DR.GetNigerianStates();
        //    ViewBag.nMeansOfIDs = _DR.GetMeansOfIdentifications();
        //    ViewBag.nAccomodationTypes = _DR.GetAccomodationTypes();
        //    ViewBag.nLGAs = _DR.GetAllLGAs();
        //    ViewBag.nTitles = _DR.GetTitles();
        //    ViewBag.nMarital = _DR.GetMaritalStatus();
        //    int val = 0;
        //    ViewData["nTitles"] = new SelectList(_DR.GetTitles(), "ID", "NAME", val);
        //    ViewData["nMarital"] = new SelectList(_DR.GetMaritalStatus(), "ID", "NAME", val);
        //    ViewData["nLGAs"] = new SelectList(_DR.GetAllLGAs(), "ID", "NAME", val);

        //    ViewData["nAccomodationTypes"] = new SelectList(_DR.GetAccomodationTypes(), "ID", "NAME", val);
        //    ViewData["nMeansOfIDs"] = new SelectList(_DR.GetMeansOfIdentifications(), "ID", "NAME", val);
        //    ViewData["nStates"] = new SelectList(_DR.GetNigerianStates(), "ID", "NAME", val);
        //    ViewData["nBanks"] = new SelectList(_DR.GetBanks(), "FlutterWaveBankCode", "NAME", val);
        //    ViewData["nRepmtMethods"] = new SelectList(_DR.GetRepaymentMethods(), "ID", "NAME", val);
        //    ViewData["nLoanTenure"] = new SelectList(_DR.GetAllTenure(), "ID", "Tenure", val);
        //    ViewData["nGender"] = new SelectList(GetAllGender(), "Value", "Text", val);
        //    ViewData["nCDDays"] = new SelectList(GetDayOfTheWeek(), "Value", "Text", val);
        //    ViewData["nPassOutMonths"] = new SelectList(GetPassOutMonths(), "Value", "Text", val);
        //    ViewData["nemploymentStatus"] = new SelectList(GetAppStatus(), "Value", "Text", val);
        //    WebLog.Log("refelcode4" + referralCode);
        //    var channels = _DR.GetMarketChannel();
        //    ViewBag.channel = channels;
        //    GetApplicantInfoss(Refid, LoanApp, Apploan, LoanApps, nysc);
        //    return View(nysc);
        //}
       
        public ActionResult FileUploadDetails()

        {

            try
            {
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = ""; TempData["Error"] = "";
                AppStatFk = 2;
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }

                var recObj = _DR.EditApprovedLoans(AppStatFk, appUser);

                List<AppLoanss> appLoanList = new List<AppLoanss>();
                foreach (AppLoanss app in recObj)
                {
                    app.ConvertedAmount = MyUtility.ConvertToCurrency(app.LoanAmount.ToString());
                    appLoanList.Add(app);
                }

                ViewBag.Data = appLoanList;
                GetMenus();
                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        [HttpGet]

        public ActionResult NYSCFileUploads(string Refid, TableObjects.LoanApplication LoanApp, DataAccessA.Classes.LoanApplication lApObj, FormCollection form, DataAccessA.Classes.AppLoanss Apploan, AppLoanss LoanApps)
        {
            TempData["SucMsg"] = ""; TempData["ErrMsg"] = ""; TempData["Error"] = "";

            var loanrefID = form["loanrefID"];

            AppStatFk = 2;
            var LoggedInuser = new LogginHelper();
            user = LoggedInuser.LoggedInUser();

            var appUser = user;
            var User = _DR.getUser(appUser);
            if (appUser == null)
            {
                return RedirectToAction("/", "Home");
            }
            if (Refid == null || Refid == "")
            {
                return RedirectToAction("FileUploadDetails");
            }

            var LoanAppss = _DR.GetNYSCLoanApplication(Refid, AppStatFk);
            if (LoanAppss == null)
            {

                TempData["Error"] = "Check The Status Of The Application";
                return RedirectToAction("FileUploadDetails");
            }

            GetApplicantInfo(Refid, LoanApp, lApObj, Apploan, LoanAppss);
            TempData["Username"] = User.Firstname;
            TempData["LoanObj"] = Apploan;
            GetMenus();
            return View(Apploan);

        }

        [HttpPost]
        public ActionResult NYSCFileUploads(FormCollection form, string Refid, HttpPostedFileBase PostedFile, DataAccessA.Classes.LoanApplication lApObj, DataAccessA.Classes.AppLoanss Apploan, HttpPostedFileBase NyscIDCard, HttpPostedFileBase StatementOfAccount, HttpPostedFileBase NyscPassport, HttpPostedFileBase NyscPostingLetter, HttpPostedFileBase NyscCallUpLetter, HttpPostedFileBase NyscProfileDashboard, HttpPostedFileBase LetterOfundertaken, string submit)
        {
            try
            {
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = ""; TempData["Error"] = "";

                Apploan = (DataAccessA.Classes.AppLoanss)TempData["LoanObj"];

                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                var User = _DR.getUser(appUser);
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }
                var refnum = Convert.ToString(form["loanrefID"]);

                Apploan.LoanRefNumber = refnum;

                //string NyscIDCards = saveImage(NyscIDCard);
                //WebLog.Log("nyscPath Path" + NyscIDCards);
                //string StatementOfAccounts = saveImages(StatementOfAccount);
                //WebLog.Log("StatementOfAccounts Path" + StatementOfAccounts);
                string NyscPassports = saveImgs(NyscPassport);
                WebLog.Log("NyscPassports Path" + NyscPassports);
                string NyscPostingLetters = saveImgss(NyscPostingLetter);
                WebLog.Log("NyscPostingLetters Path" + NyscPostingLetters);
                string NyscCallUpLetters = saveImg(NyscCallUpLetter);
                WebLog.Log("NyscCallUpLetters Path" + NyscCallUpLetter);
                string NyscProfileDashboards = saveImgsss(NyscProfileDashboard);
                WebLog.Log("NyscProfileDashboards Path" + NyscProfileDashboard);
                 string LetterOfundertakens = saveImgssss(LetterOfundertaken);
                WebLog.Log("LetterOfundertaken Path" + LetterOfundertakens);
                //if (NyscIDCard.ContentLength == 0)
                //{
                //    TempData["Error"] = "Input Nysc ID Card";
                //    // NYSCLoanAppForm();
                //    ViewData["nTitles"] = new SelectList(_DR.GetTitles(), "ID", "NAME", lApObj.Title_FK);
                //    ViewData["nMarital"] = new SelectList(_DR.GetMaritalStatus(), "ID", "NAME", lApObj.MaritalStatus_FK);
                //    ViewData["nLGAs"] = new SelectList(_DR.GetAllLGAs(), "ID", "NAME", lApObj.LGA_FK);
                //    ViewData["nRelative"] = new SelectList(_DR.GetRelative(), "ID", "NAME", lApObj.RelativeRelationship_FK);
                //    ViewData["nAccomodationTypes"] = new SelectList(_DR.GetAccomodationTypes(), "ID", "NAME", lApObj.AccomodationType_FK);
                //    ViewData["nMeansOfIDs"] = new SelectList(_DR.GetMeansOfIdentifications(), "ID", "NAME", lApObj.MeansOfID_FK);
                //    ViewData["nStates"] = new SelectList(_DR.GetNigerianStates(), "ID", "NAME", lApObj.StateofResidence_FK);
                //    ViewData["nBanks"] = new SelectList(_DR.GetBanks(), "FlutterWaveBankCode", "NAME", lApObj.Bank_FK);
                //    ViewData["nRepmtMethods"] = new SelectList(_DR.GetRepaymentMethods(), "ID", "NAME", lApObj.RepaymentMethod_FK);
                //    ViewData["nLoanTenure"] = new SelectList(_DR.GetAllTenure(), "ID", "Tenure", lApObj.LoanTenure);
                //    ViewData["nGender"] = new SelectList(GetAllGender(), "Value", "Text", lApObj.Gender_FK);
                //    ViewData["nemploymentStatus"] = new SelectList(GetAppStatus(), "Value", "Text", lApObj.Contract);
                //    ViewData["nCDDays"] = new SelectList(GetDayOfTheWeek(), "Value", "Text", lApObj.CDSDay);
                //    var channels = _DR.GetMarketChannel();
                //    ViewBag.channel = channels;
                //    return View("NYSCLoanAppForm", lApObj);
                //}

                //if (StatementOfAccount.ContentLength == 0)
                //{
                //    TempData["Error"] = "Input Nysc Statement Of Account";

                //    ViewData["nTitles"] = new SelectList(_DR.GetTitles(), "ID", "NAME", lApObj.Title_FK);
                //    ViewData["nMarital"] = new SelectList(_DR.GetMaritalStatus(), "ID", "NAME", lApObj.MaritalStatus_FK);
                //    ViewData["nLGAs"] = new SelectList(_DR.GetAllLGAs(), "ID", "NAME", lApObj.LGA_FK);
                //    ViewData["nRelative"] = new SelectList(_DR.GetRelative(), "ID", "NAME", lApObj.RelativeRelationship_FK);
                //    ViewData["nAccomodationTypes"] = new SelectList(_DR.GetAccomodationTypes(), "ID", "NAME", lApObj.AccomodationType_FK);
                //    ViewData["nMeansOfIDs"] = new SelectList(_DR.GetMeansOfIdentifications(), "ID", "NAME", lApObj.MeansOfID_FK);
                //    ViewData["nStates"] = new SelectList(_DR.GetNigerianStates(), "ID", "NAME", lApObj.StateofResidence_FK);
                //    ViewData["nBanks"] = new SelectList(_DR.GetBanks(), "FlutterWaveBankCode", "NAME", lApObj.Bank_FK);
                //    ViewData["nRepmtMethods"] = new SelectList(_DR.GetRepaymentMethods(), "ID", "NAME", lApObj.RepaymentMethod_FK);
                //    ViewData["nLoanTenure"] = new SelectList(_DR.GetAllTenure(), "ID", "Tenure", lApObj.LoanTenure);
                //    ViewData["nGender"] = new SelectList(GetAllGender(), "Value", "Text", lApObj.Gender_FK);
                //    ViewData["nemploymentStatus"] = new SelectList(GetAppStatus(), "Value", "Text", lApObj.Contract);
                //    ViewData["nCDDays"] = new SelectList(GetDayOfTheWeek(), "Value", "Text", lApObj.CDSDay);
                //    var channels = _DR.GetMarketChannel();
                //    ViewBag.channel = channels;
                //    return View("NYSCLoanAppForm", lApObj);
                //}

                if (LetterOfundertaken.ContentLength == 0)
                {
                    TempData["Error"] = "Input Letter Of undertaken";

                    ViewData["nTitles"] = new SelectList(_DR.GetTitles(), "ID", "NAME", lApObj.Title_FK);
                    ViewData["nMarital"] = new SelectList(_DR.GetMaritalStatus(), "ID", "NAME", lApObj.MaritalStatus_FK);
                    ViewData["nLGAs"] = new SelectList(_DR.GetAllLGAs(), "ID", "NAME", lApObj.LGA_FK);
                    ViewData["nRelative"] = new SelectList(_DR.GetRelative(), "ID", "NAME", lApObj.RelativeRelationship_FK);
                    ViewData["nAccomodationTypes"] = new SelectList(_DR.GetAccomodationTypes(), "ID", "NAME", lApObj.AccomodationType_FK);
                    ViewData["nMeansOfIDs"] = new SelectList(_DR.GetMeansOfIdentifications(), "ID", "NAME", lApObj.MeansOfID_FK);
                    ViewData["nStates"] = new SelectList(_DR.GetNigerianStates(), "ID", "NAME", lApObj.StateofResidence_FK);
                    ViewData["nBanks"] = new SelectList(_DR.GetBanks(), "FlutterWaveBankCode", "NAME", lApObj.Bank_FK);
                    ViewData["nRepmtMethods"] = new SelectList(_DR.GetRepaymentMethods(), "ID", "NAME", lApObj.RepaymentMethod_FK);
                    ViewData["nLoanTenure"] = new SelectList(_DR.GetAllTenure(), "ID", "Tenure", lApObj.LoanTenure);
                    ViewData["nGender"] = new SelectList(GetAllGender(), "Value", "Text", lApObj.Gender_FK);
                    ViewData["nemploymentStatus"] = new SelectList(GetAppStatus(), "Value", "Text", lApObj.Contract);
                    ViewData["nCDDays"] = new SelectList(GetDayOfTheWeek(), "Value", "Text", lApObj.CDSDay);
                    var channels = _DR.GetMarketChannel();
                    ViewBag.channel = channels;
                    return View("NYSCLoanAppForm", lApObj);
                }


                if (NyscPassport.ContentLength == 0)
                {
                    TempData["Error"] = "Input Nysc Passport";

                    ViewData["nTitles"] = new SelectList(_DR.GetTitles(), "ID", "NAME", lApObj.Title_FK);
                    ViewData["nMarital"] = new SelectList(_DR.GetMaritalStatus(), "ID", "NAME", lApObj.MaritalStatus_FK);
                    ViewData["nLGAs"] = new SelectList(_DR.GetAllLGAs(), "ID", "NAME", lApObj.LGA_FK);
                    ViewData["nRelative"] = new SelectList(_DR.GetRelative(), "ID", "NAME", lApObj.RelativeRelationship_FK);
                    ViewData["nAccomodationTypes"] = new SelectList(_DR.GetAccomodationTypes(), "ID", "NAME", lApObj.AccomodationType_FK);
                    ViewData["nMeansOfIDs"] = new SelectList(_DR.GetMeansOfIdentifications(), "ID", "NAME", lApObj.MeansOfID_FK);
                    ViewData["nStates"] = new SelectList(_DR.GetNigerianStates(), "ID", "NAME", lApObj.StateofResidence_FK);
                    ViewData["nBanks"] = new SelectList(_DR.GetBanks(), "FlutterWaveBankCode", "NAME", lApObj.Bank_FK);
                    ViewData["nRepmtMethods"] = new SelectList(_DR.GetRepaymentMethods(), "ID", "NAME", lApObj.RepaymentMethod_FK);
                    ViewData["nLoanTenure"] = new SelectList(_DR.GetAllTenure(), "ID", "Tenure", lApObj.LoanTenure);
                    ViewData["nGender"] = new SelectList(GetAllGender(), "Value", "Text", lApObj.Gender_FK);
                    ViewData["nemploymentStatus"] = new SelectList(GetAppStatus(), "Value", "Text", lApObj.Contract);
                    ViewData["nCDDays"] = new SelectList(GetDayOfTheWeek(), "Value", "Text", lApObj.CDSDay);
                    var channels = _DR.GetMarketChannel();
                    ViewBag.channel = channels;
                    return View("NYSCLoanAppForm", lApObj);
                }



                if (NyscPostingLetter.ContentLength == 0)
                {
                    TempData["Error"] = "Input Nysc Posting Letter";

                    ViewData["nTitles"] = new SelectList(_DR.GetTitles(), "ID", "NAME", lApObj.Title_FK);
                    ViewData["nMarital"] = new SelectList(_DR.GetMaritalStatus(), "ID", "NAME", lApObj.MaritalStatus_FK);
                    ViewData["nLGAs"] = new SelectList(_DR.GetAllLGAs(), "ID", "NAME", lApObj.LGA_FK);
                    ViewData["nRelative"] = new SelectList(_DR.GetRelative(), "ID", "NAME", lApObj.RelativeRelationship_FK);
                    ViewData["nAccomodationTypes"] = new SelectList(_DR.GetAccomodationTypes(), "ID", "NAME", lApObj.AccomodationType_FK);
                    ViewData["nMeansOfIDs"] = new SelectList(_DR.GetMeansOfIdentifications(), "ID", "NAME", lApObj.MeansOfID_FK);
                    ViewData["nStates"] = new SelectList(_DR.GetNigerianStates(), "ID", "NAME", lApObj.StateofResidence_FK);
                    ViewData["nBanks"] = new SelectList(_DR.GetBanks(), "FlutterWaveBankCode", "NAME", lApObj.Bank_FK);
                    ViewData["nRepmtMethods"] = new SelectList(_DR.GetRepaymentMethods(), "ID", "NAME", lApObj.RepaymentMethod_FK);
                    ViewData["nLoanTenure"] = new SelectList(_DR.GetAllTenure(), "ID", "Tenure", lApObj.LoanTenure);
                    ViewData["nGender"] = new SelectList(GetAllGender(), "Value", "Text", lApObj.Gender_FK);
                    ViewData["nemploymentStatus"] = new SelectList(GetAppStatus(), "Value", "Text", lApObj.Contract);
                    ViewData["nCDDays"] = new SelectList(GetDayOfTheWeek(), "Value", "Text", lApObj.CDSDay);
                    var channels = _DR.GetMarketChannel();
                    ViewBag.channel = channels;
                    return View("NYSCLoanAppForm", lApObj);
                }

                if (NyscCallUpLetter.ContentLength == 0)
                {
                    TempData["Error"] = "Input Nysc CallUp Letter";

                    ViewData["nTitles"] = new SelectList(_DR.GetTitles(), "ID", "NAME", lApObj.Title_FK);
                    ViewData["nMarital"] = new SelectList(_DR.GetMaritalStatus(), "ID", "NAME", lApObj.MaritalStatus_FK);
                    ViewData["nLGAs"] = new SelectList(_DR.GetAllLGAs(), "ID", "NAME", lApObj.LGA_FK);
                    ViewData["nRelative"] = new SelectList(_DR.GetRelative(), "ID", "NAME", lApObj.RelativeRelationship_FK);
                    ViewData["nAccomodationTypes"] = new SelectList(_DR.GetAccomodationTypes(), "ID", "NAME", lApObj.AccomodationType_FK);
                    ViewData["nMeansOfIDs"] = new SelectList(_DR.GetMeansOfIdentifications(), "ID", "NAME", lApObj.MeansOfID_FK);
                    ViewData["nStates"] = new SelectList(_DR.GetNigerianStates(), "ID", "NAME", lApObj.StateofResidence_FK);
                    ViewData["nBanks"] = new SelectList(_DR.GetBanks(), "FlutterWaveBankCode", "NAME", lApObj.Bank_FK);
                    ViewData["nRepmtMethods"] = new SelectList(_DR.GetRepaymentMethods(), "ID", "NAME", lApObj.RepaymentMethod_FK);
                    ViewData["nLoanTenure"] = new SelectList(_DR.GetAllTenure(), "ID", "Tenure", lApObj.LoanTenure);
                    ViewData["nGender"] = new SelectList(GetAllGender(), "Value", "Text", lApObj.Gender_FK);
                    ViewData["nemploymentStatus"] = new SelectList(GetAppStatus(), "Value", "Text", lApObj.Contract);
                    ViewData["nCDDays"] = new SelectList(GetDayOfTheWeek(), "Value", "Text", lApObj.CDSDay);
                    var channels = _DR.GetMarketChannel();
                    ViewBag.channel = channels;
                    return View("NYSCLoanAppForm", lApObj);
                }

                if (NyscProfileDashboard.ContentLength == 0)
                {
                    TempData["Error"] = "Input Nysc Profile Dashboard screenshot";

                    ViewData["nTitles"] = new SelectList(_DR.GetTitles(), "ID", "NAME", lApObj.Title_FK);
                    ViewData["nMarital"] = new SelectList(_DR.GetMaritalStatus(), "ID", "NAME", lApObj.MaritalStatus_FK);
                    ViewData["nLGAs"] = new SelectList(_DR.GetAllLGAs(), "ID", "NAME", lApObj.LGA_FK);
                    ViewData["nRelative"] = new SelectList(_DR.GetRelative(), "ID", "NAME", lApObj.RelativeRelationship_FK);
                    ViewData["nAccomodationTypes"] = new SelectList(_DR.GetAccomodationTypes(), "ID", "NAME", lApObj.AccomodationType_FK);
                    ViewData["nMeansOfIDs"] = new SelectList(_DR.GetMeansOfIdentifications(), "ID", "NAME", lApObj.MeansOfID_FK);
                    ViewData["nStates"] = new SelectList(_DR.GetNigerianStates(), "ID", "NAME", lApObj.StateofResidence_FK);
                    ViewData["nBanks"] = new SelectList(_DR.GetBanks(), "FlutterWaveBankCode", "NAME", lApObj.Bank_FK);
                    ViewData["nRepmtMethods"] = new SelectList(_DR.GetRepaymentMethods(), "ID", "NAME", lApObj.RepaymentMethod_FK);
                    ViewData["nLoanTenure"] = new SelectList(_DR.GetAllTenure(), "ID", "Tenure", lApObj.LoanTenure);
                    ViewData["nGender"] = new SelectList(GetAllGender(), "Value", "Text", lApObj.Gender_FK);
                    ViewData["nemploymentStatus"] = new SelectList(GetAppStatus(), "Value", "Text", lApObj.Contract);
                    ViewData["nCDDays"] = new SelectList(GetDayOfTheWeek(), "Value", "Text", lApObj.CDSDay);
                    var channels = _DR.GetMarketChannel();
                    ViewBag.channel = channels;
                    return View("NYSCLoanAppForm", lApObj);

                }



                {
                    DataAccessA.Classes.AppLoanss Apploans = new DataAccessA.Classes.AppLoanss

                    {
                        AccountNumber = lApObj.AccountNumber,
                        AccountName = lApObj.AccountName,
                        Firstname = lApObj.Firstname,
                        Othernames = lApObj.Othernames,
                        PPA_Department = lApObj.PPA_Department,
                        PPA_EmailAddress = lApObj.PPA_EmailAddress,
                        PPA_PhoneNumber = lApObj.PPA_PhoneNumber,
                        PPA_ROle = lApObj.PPA_ROle,
                        PPA_supervisorEmail = lApObj.PPA_supervisorEmail,
                        PPA_supervisorName = lApObj.PPA_supervisorName,
                        PPA_supervisorPhonenumber = lApObj.PPA_supervisorPhonenumber,
                        LoanRefNumber = Apploan.LoanRefNumber,
                        NYSCApplicationStatus_FK = 1,
                        //NyscIdCardFilePath = NyscIDCards,
                        //STA_FilePath = StatementOfAccounts,
                        NyscpassportFilePath = NyscPassports,
                        NyscCallUpLetterFilePath = NyscCallUpLetters,
                        NyscPostingLetterFllePath = NyscPostingLetters,
                        NyscProfileDashboardFilePath = NyscProfileDashboards,
                        LetterOfundertaken= LetterOfundertakens,
                        FacebookName = lApObj.FacebookName,
                        InstagramHandle = lApObj.InstagramHandle,
                        TwitterHandle = lApObj.TwitterHandle,
                        // RepaymentAmount = repaymentAmt,
                        //RefNumber = "NY" + MyUtility.GenerateRefNo(),
                        // Gender_FK = Gender_FK,//Convert.ToInt32(form["selectGender"]),
                        MaritalStatus_FK = lApObj.MaritalStatus_FK,//Convert.ToInt16(form["Marital"]),
                        Surname = lApObj.Surname,
                        CreatedBy = Convert.ToString(userid),
                        DateOfBirth = Convert.ToString(lApObj.DateOfBirth),
                        Title_FK = lApObj.Title_FK,//Convert.ToInt32(form["Titles"]),
                        PhoneNumber = lApObj.PhoneNumber,
                        EmailAddress = lApObj.EmailAddress,
                        PermanentAddress = lApObj.PermanentAddress,
                        Landmark = lApObj.Landmark,
                        ClosestBusStop = lApObj.ClosestBusStop,
                        LGA_FK = Convert.ToInt16(form["lgaList"]),
                        TempLGA_FK = Convert.ToInt16(form["lgaLists"]),
                        NyscLGA_FK = Convert.ToInt16(form["lgaListsss"]),
                        StateofResidence_FK = lApObj.StateofResidence_FK,////Convert.ToInt32(form["States"]),
                        TempStateofResidence_FK = lApObj.TempStateofResidence_FK,//Convert.ToInt32(form["States"]),
                        NyscStateofResidence_FK = lApObj.NyscStateofResidence_FK,//Convert.ToInt32(form["States"]),
                        TemporaryAddress = lApObj.TemporaryAddress,
                        OfficialAddress = lApObj.OfficialAddress,
                        StateCode = lApObj.StateCode,
                        Employer = lApObj.Employer,
                        PassOutMonth = lApObj.PassOutMonth,
                        CDSDay = lApObj.CDSDay,
                        TempLandmark = lApObj.TempLandmark,
                        TempClosestBusStop = lApObj.TempClosestBusStop,
                        ReferralCode = lApObj.ReferalCode,
                        BVN = lApObj.BVN,
                        CDSGroup = lApObj.CDSGroup,
                        NetMonthlyIncome = Convert.ToDouble(lApObj.NetMonthlyIncome),
                        EMG_EmailAddress = lApObj.EMG_EmailAddress,
                        EMG_FullName = lApObj.EMG_FullName,
                        EMG_HomeAddress = lApObj.EMG_HomeAddress,
                        EMG_PhoneNumber = lApObj.EMG_PhoneNumber,
                        EMG_Relationship = lApObj.EMG_Relationship,
                        EMG_FullName2 = lApObj.EMG_FullName2,
                        EMG_EmailAddress2 = lApObj.EMG_EmailAddress2,
                        EMG_HomeAddress2 = lApObj.EMG_HomeAddress2,
                        EMG_PhoneNumber2 = lApObj.EMG_PhoneNumber2,
                        EMG_Relationship2 = lApObj.EMG_Relationship2,
                        FirstRelativeName = lApObj.FirstRelativeName,
                        FirstRelativePhoneNumber = lApObj.FirstRelativePhoneNumber,
                        RelativeRelationship2_FK = lApObj.RelativeRelationship2_FK,
                        SecondRelativeName = lApObj.SecondRelativeName,
                        SecondRelativePhoneNumber = lApObj.SecondRelativePhoneNumber,
                        RelativeRelationship_FK = lApObj.RelativeRelationship_FK,
                        LoanAmount = Convert.ToDouble(lApObj.LoanAmount),
                        // LoanTenure = LoanTenure,
                        ExistingLoan = lApObj.ExistingLoan,
                        LoanComment = lApObj.LoanComment,
                        ExistingLoan_NoOfMonthsLeft = Convert.ToInt16(lApObj.ExistingLoan_NoOfMonthsLeft),
                        ExistingLoan_OutstandingAmount = lApObj.ExistingLoan_OutstandingAmount,
                        BankCode = Helper.GetRemitaBankCodeByFlutterCode(lApObj.BankCode),
                        IsVisible = 1,
                        DateCreated = MyUtility.getCurrentLocalDateTime(),
                        DateModified = MyUtility.getCurrentLocalDateTime(),
                        //ValueDate = MyUtility.getCurrentLocalDateTime().ToString("yyyy/MM/dd"),
                        ValueDate = MyUtility.getCurrentLocalDateTime().ToString("dddd, dd MMMM yyyy"),
                        ValueTime = MyUtility.getCurrentLocalDateTime().ToString("H:mmss"),

                    };


                    //WebLog.Log("nyscPath Path 2" + NyscIDCards);
                    //WebLog.Log("StatementOfAccounts Path 2" + StatementOfAccounts);
                    WebLog.Log("NyscPassports Path 2" + NyscPassports);
                    WebLog.Log("NyscProfileDashboards Path 2" + NyscProfileDashboards);
                    WebLog.Log("NyscCallUpLetters Path 2" + NyscCallUpLetters);
                    WebLog.Log("NyscPostingLetters Path 2" + NyscPostingLetters);
                    WebLog.Log("LetterOfundertakens Path 2" + LetterOfundertakens);
                    //var id = DataWriter.CreateNYSCLoanApplication(NyscLA);
                    var id = _DR.UpdateNyscfiles(Apploans);
                    if (id == null)
                    {
                        TempData["Error"] = "Please Check Next Of kin Phone Number";
                        //NYSCLoanAppForm();
                        ViewData["nTitles"] = new SelectList(_DR.GetTitles(), "ID", "NAME", lApObj.Title_FK);
                        ViewData["nMarital"] = new SelectList(_DR.GetMaritalStatus(), "ID", "NAME", lApObj.MaritalStatus_FK);
                        ViewData["nLGAs"] = new SelectList(_DR.GetAllLGAs(), "ID", "NAME", lApObj.LGA_FK);
                        ViewData["nRelative"] = new SelectList(_DR.GetRelative(), "ID", "NAME", lApObj.RelativeRelationship_FK);
                        ViewData["nAccomodationTypes"] = new SelectList(_DR.GetAccomodationTypes(), "ID", "NAME", lApObj.AccomodationType_FK);
                        ViewData["nMeansOfIDs"] = new SelectList(_DR.GetMeansOfIdentifications(), "ID", "NAME", lApObj.MeansOfID_FK);
                        ViewData["nStates"] = new SelectList(_DR.GetNigerianStates(), "ID", "NAME", lApObj.StateofResidence_FK);
                        ViewData["nBanks"] = new SelectList(_DR.GetBanks(), "FlutterWaveBankCode", "NAME", lApObj.Bank_FK);
                        ViewData["nRepmtMethods"] = new SelectList(_DR.GetRepaymentMethods(), "ID", "NAME", lApObj.RepaymentMethod_FK);
                        ViewData["nLoanTenure"] = new SelectList(_DR.GetAllTenure(), "ID", "LoanTenure", lApObj.LoanTenure);
                        ViewData["nGender"] = new SelectList(GetAllGender(), "Value", "Text", lApObj.Gender_FK);
                        ViewData["nemploymentStatus"] = new SelectList(GetAppStatus(), "Value", "Text", lApObj.Contract);
                        var channels = _DR.GetMarketChannel();
                        ViewBag.channel = channels;
                        return View("NYSCLoanAppForm", Apploans);
                    }

                }
                GetMenus();
                //TempData["SucMsg"] = "Your Application has been submitted. You can track progress of your loan application on your dashboard, please check your mail for your login details.";
                //return RedirectToAction("SaveAcknowledgementss");
                return RedirectToAction("SaveAcknowledgementss", new { @Refid = Apploan.LoanRefNumber });

            }
            catch (Exception ex)

            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }





     
        public static bool Verify(out string token)
        {

            token = null;
            dynamic obj = new JObject();
            obj.apiKey = System.Web.Configuration.WebConfigurationManager.AppSettings["MoneyWave_Api_Key_Live"];
            obj.secret = System.Web.Configuration.WebConfigurationManager.AppSettings["MoneyWave_Secret_Live"];
            var MoneyWaveVerify = System.Web.Configuration.WebConfigurationManager.AppSettings["MoneyWave_ApiBase_Live"];
            var json = obj.ToString();
            var resp = MyUtility.DoPostToken(json, MoneyWaveVerify);

            if (resp == null) return false;
            var jvalue = JObject.Parse(resp);
            var status = (string)jvalue["status"];
            token = status.ToLower() == "success" ? (string)jvalue["token"] : string.Empty;
            return !string.IsNullOrWhiteSpace(token);
        }
        [HttpPost]
        public ActionResult VerifyAccountNumber(string Account)
        {
            try
            {
                dynamic obj = new JObject();
                var acctNumber = Account.After("&");
                var BankCode = Account.Before("&");
                obj.account_number = acctNumber;
                obj.bank_code = BankCode;
                var json = obj.ToString();
                string token = "";
                Verify(out token);
                var MoneyWaveResolveAccount = System.Web.Configuration.WebConfigurationManager.AppSettings["MoneyWaveResolveAccount"];
                var resp = MyUtility.DoPost(json, MoneyWaveResolveAccount, token);
                dynamic respa = JObject.Parse(resp);
                data dt = new data();
                dt.responseMsg = respa?.status;
                if (dt.responseMsg == "success")
                {
                    //dt.responseMsg = respa?.status;
                    dt.respMSg = true;
                    dt.account_name = respa?.data?.account_name;

                }
                if (dt.responseMsg == "error")
                {
                    dt.respMSg = false;
                    dt.responseMsg = respa?.status;
                    dt.account_name = "Invalid Account Number";

                }

                //  { "status":"error","code":"UNAUTHORIZED_ACCESS","message":"Unauthorized access."}
                return Json(new { dt });
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }












        //[HttpGet]
        //public ActionResult NYSCLoanAppFormEdit(string Refid, TableObjects.LoanApplication LoanApp, DataAccessA.Classes.AppLoanss Apploan, AppLoanss LoanApps, NyscLoanApplication nysc)
        //{
        //    TempData["SucMsg"] = ""; TempData["ErrMsg"] = ""; TempData["Error"] = "";
        //    nysc = _DR.GetRefid(Refid);
        //    //nysc = data;
        //    nysc.NyscStateofResidence_FK = 0;
        //    nysc.StateofResidence_FK = 0;
        //    nysc.TempStateofResidence_FK = 0;
        //    nysc.PassOutMonth = "";
        //    var Nyscfileimg = nysc.STA_FilePath;
        //    if (Nyscfileimg == null || Nyscfileimg == "")
        //    {

        //        Nyscfileimg = nysc.STA_FilePath;

        //    }

        //    WebLog.Log("ENTERED");
        //    TempData["Error"] = "";
        //    string referralCode = Request.QueryString["referralCode"];
        //    WebLog.Log("refelcode" + referralCode);
        //    if (referralCode != null || referralCode != "")
        //    {
        //        WebLog.Log("refelcode0" + referralCode);
        //        nysc.ReferralCode = referralCode;
        //        WebLog.Log("refelcode1" + referralCode);
        //    }

        //    ViewBag.nBanks = _DR.GetBanks();
        //    ViewBag.nRepmtMethods = _DR.GetRepaymentMethods();
        //    ViewBag.nStates = _DR.GetNigerianStates();
        //    ViewBag.nMeansOfIDs = _DR.GetMeansOfIdentifications();
        //    ViewBag.nAccomodationTypes = _DR.GetAccomodationTypes();
        //    ViewBag.nLGAs = _DR.GetAllLGAs();
        //    ViewBag.nTitles = _DR.GetTitles();
        //    ViewBag.nMarital = _DR.GetMaritalStatus();
        //    ViewBag.nRelative = _DR.GetRelative();
        //    int val = 0;
        //    ViewData["nTitles"] = new SelectList(_DR.GetTitles(), "ID", "NAME", val);
        //    ViewData["nMarital"] = new SelectList(_DR.GetMaritalStatus(), "ID", "NAME", val);
        //    ViewData["nLGAs"] = new SelectList(_DR.GetAllLGAs(), "ID", "NAME", val);
        //    ViewData["nRelative"] = new SelectList(_DR.GetRelative(), "ID", "NAME", val);
        //    ViewData["nAccomodationTypes"] = new SelectList(_DR.GetAccomodationTypes(), "ID", "NAME", val);
        //    ViewData["nMeansOfIDs"] = new SelectList(_DR.GetMeansOfIdentifications(), "ID", "NAME", val);
        //    ViewData["nStates"] = new SelectList(_DR.GetNigerianStates(), "ID", "NAME", val);
        //    ViewData["nBanks"] = new SelectList(_DR.GetBanks(), "FlutterWaveBankCode", "NAME", val);
        //    ViewData["nRepmtMethods"] = new SelectList(_DR.GetRepaymentMethods(), "ID", "NAME", val);
        //    ViewData["nLoanTenure"] = new SelectList(_DR.GetAllTenure(), "ID", "Tenure", val);
        //    ViewData["nGender"] = new SelectList(GetAllGender(), "Value", "Text", val);
        //    ViewData["nCDDays"] = new SelectList(GetDayOfTheWeek(), "Value", "Text", val);
        //    ViewData["nPassOutMonths"] = new SelectList(GetPassOutMonths(), "Value", "Text", val);
        //    ViewData["nemploymentStatus"] = new SelectList(GetAppStatus(), "Value", "Text", val);
        //    WebLog.Log("refelcode4" + referralCode);
        //    var channels = _DR.GetMarketChannel();
        //    ViewBag.channel = channels;

        //    string IdentImage = nysc.NyscIdCardFilePath.After("h:\\root\\home\\paelyt-001\\www\\paelyt\\uvlotapublish\\").Replace("\\", "/");
        //    string IdentImages = nysc.STA_FilePath.After("h:\\root\\home\\paelyt-001\\www\\paelyt\\uvlotapublish\\").Replace("\\", "/");
        //    string IdentImagess = nysc.NyscpassportFilePath.After("h:\\root\\home\\paelyt-001\\www\\paelyt\\uvlotapublish\\").Replace("\\", "/");
        //    string IdentImgs = nysc.NyscPostingLetterFllePath.After("h:\\root\\home\\paelyt-001\\www\\paelyt\\uvlotapublish\\").Replace("\\", "/");
        //    string IdentImgss = nysc.NyscCallUpLetterFilePath.After("h:\\root\\home\\paelyt-001\\www\\paelyt\\uvlotapublish\\").Replace("\\", "/");
        //    string IdentImgsss = nysc.NyscProfileDashboardFilePath.After("h:\\root\\home\\paelyt-001\\www\\paelyt\\uvlotapublish\\").Replace("\\", "/");

        //    if (IdentImage != "" || IdentImages != "" || IdentImagess != "" || IdentImgs != "" || IdentImgs != "" || IdentImgsss != "")
        //    {

        //        WebLog.Log("Image Path" + IdentImage);
        //        string slash = "/";
        //        nysc.NyscIdCardFilePath = string.IsNullOrEmpty(IdentImage) ? "none" : slash + IdentImage;
        //        nysc.STA_FilePath = string.IsNullOrEmpty(IdentImages) ? "none" : slash + IdentImages;
        //        nysc.NyscpassportFilePath = string.IsNullOrEmpty(IdentImagess) ? "none" : slash + IdentImagess;
        //        nysc.NyscPostingLetterFllePath = string.IsNullOrEmpty(IdentImgs) ? "none" : slash + IdentImgs;
        //        nysc.NyscCallUpLetterFilePath = string.IsNullOrEmpty(IdentImgss) ? "none" : slash + IdentImgss;
        //        nysc.NyscProfileDashboardFilePath = string.IsNullOrEmpty(IdentImgsss) ? "none" : slash + IdentImgsss;

        //    }
        //    return View(nysc);
        //}


        //[HttpGet]
        //public ActionResult NYSCLoanAppFormEdit(string Refid, TableObjects.LoanApplication LoanApp, DataAccessA.Classes.AppLoanss Apploan, AppLoanss LoanApps, NyscLoanApplication nysc)
        //{
        //    TempData["SucMsg"] = ""; TempData["ErrMsg"] = ""; TempData["Error"] = "";
        //    nysc = _DR.GetRefid(Refid);
        //    //nysc = data;
        //    //nysc.NyscStateofResidence_FK = 0;
        //    //nysc.StateofResidence_FK = 0;
        //    //nysc.TempStateofResidence_FK = 0;
        //    // nysc.PassOutMonth = "";


        //    var Nyscfileimg = nysc.STA_FilePath;
        //    if (Nyscfileimg == null || Nyscfileimg == "")
        //    {

        //        Nyscfileimg = nysc.STA_FilePath;

        //    }

        //    WebLog.Log("ENTERED");
        //    TempData["Error"] = "";
        //    string referralCode = Request.QueryString["referralCode"];
        //    WebLog.Log("refelcode" + referralCode);
        //    if (referralCode != null || referralCode != "")
        //    {
        //        WebLog.Log("refelcode0" + referralCode);
        //        nysc.ReferralCode = referralCode;
        //        WebLog.Log("refelcode1" + referralCode);
        //    }

        //    ViewBag.nBanks = _DR.GetBanks();
        //    ViewBag.nRepmtMethods = _DR.GetRepaymentMethods();
        //    ViewBag.nStates = _DR.GetNigerianStates();
        //    ViewBag.nMeansOfIDs = _DR.GetMeansOfIdentifications();
        //    ViewBag.nAccomodationTypes = _DR.GetAccomodationTypes();
        //    ViewBag.nLGAs = _DR.GetAllLGAs();
        //    ViewBag.nTitles = _DR.GetTitles();
        //    ViewBag.nMarital = _DR.GetMaritalStatus();
        //    ViewBag.nLoanTenure = _DR.GetAllTenure();
        //    int val = 1;
        //    ViewData["nTitles"] = new SelectList(_DR.GetTitles(), "ID", "NAME", val);
        //    ViewData["nMarital"] = new SelectList(_DR.GetMaritalStatus(), "ID", "NAME", val);
        //    ViewData["nLGAs"] = new SelectList(_DR.GetAllLGAs(), "ID", "NAME", val);
        //    ViewData["nRelative"] = new SelectList(_DR.GetRelative(), "ID", "NAME", nysc.RelativeRelationship_FK);
        //    ViewData["nAccomodationTypes"] = new SelectList(_DR.GetAccomodationTypes(), "ID", "NAME", val);
        //    ViewData["nMeansOfIDs"] = new SelectList(_DR.GetMeansOfIdentifications(), "ID", "NAME", val);
        //    ViewData["nStates"] = new SelectList(_DR.GetNigerianStates(), "ID", "NAME", val);
        //    //nysc.StateofResidence_FKs = new SelectList(_DR.GetNigerianStates(), "ID", "NAME", 1);
        //    ViewData["nBanks"] = new SelectList(_DR.GetBanks(), "FlutterWaveBankCode", "NAME", val);
        //    ViewData["nRepmtMethods"] = new SelectList(_DR.GetRepaymentMethods(), "ID", "NAME", val);
        //    //ViewData["nLoanTenure"] = new SelectList(_DR.GetAllTenure(), "ID", "Tenure", val);
        //    ViewData["nGender"] = new SelectList(GetAllGender(), "Value", "Text", val);
        //    ViewData["nCDDays"] = new SelectList(GetDayOfTheWeek(), "Value", "Text", val);
        //    ViewData["nPassOutMonths"] = new SelectList(GetPassOutMonths(), "Value", "Text", val);
        //    ViewData["nemploymentStatus"] = new SelectList(GetAppStatus(), "Value", "Text", val);
        //    //ViewData["nLoanTenure"] = new SelectList(_DR.GetAllTenure(), "amount", "Tenure", val);
        //    ViewData["nlgaList"] = new SelectList(_DR.GetLGAsByStateFK((int)nysc.StateofResidence_FK), "ID", "NAME", nysc.StateofResidence_FK);
        //    ViewData["nlgaLists"] = new SelectList(_DR.GetLGAsByStateFK((int)nysc.TempStateofResidence_FK), "ID", "NAME", nysc.TempStateofResidence_FK);
        //    ViewData["nlgaListsss"] = new SelectList(_DR.GetLGAsByStateFK((int)nysc.NyscStateofResidence_FK), "ID", "NAME", nysc.NyscStateofResidence_FK);
        //    ViewData["nLoanTenure"] = new SelectList(_DR.GetAllTenure(), "Tenure", "Tenure", val);

        //    nysc.StateofResidence_FKs = Convert.ToString(nysc.StateofResidence_FK);
        //    nysc.TStateofResidence_FKss = Convert.ToString(nysc.TempStateofResidence_FK);
        //    nysc.NStateofResidence_FKsss = Convert.ToString(nysc.NyscStateofResidence_FK);

        //    nysc.LoanTenures = Convert.ToString(nysc.LoanTenure);
        //    nysc.LGA_FKs = Convert.ToString(nysc.LGA_FK);
        //    nysc.LGA_FKss = Convert.ToString(nysc.TempLGA_FK);
        //    nysc.LGA_FKsss = Convert.ToString(nysc.NyscLGA_FK);
        //    WebLog.Log("refelcode4" + referralCode);
        //    var channels = _DR.GetMarketChannel();
        //    ViewBag.channel = channels;
        //    // GetApplicantInfoss(Refid, LoanApp, Apploan, LoanApps, nysc);

        //    nysc.NyscIdCardFilePath = nysc.NyscIdCardFilePath;
        //   nysc.STA_FilePath = nysc.STA_FilePath;

        //  string IdentImage = nysc.NyscIdCardFilePath;



        //    string imgPath = " h:\\root\\home\\paelyt - 001\\www\\paelyt\\uvlotapublish\\Images\\";
        //    imgPath = imgPath.Replace("\\", "/");
        //    imgPath = imgPath + nysc.NyscIdCardFilePath;


        //    var IdentImages = nysc.STA_FilePath;
        //    string imgPaths = " h:\\root\\home\\paelyt - 001\\www\\paelyt\\uvlotapublish\\Images\\";
        //    imgPaths = imgPaths.Replace("\\", "/");
        //   imgPaths = imgPaths + nysc.STA_FilePath;


        //    if (IdentImage != "" || IdentImages != ""/* ||*//*IdentImagess != "" || IdentImgs != "" || IdentImgs != "" || IdentImgsss != ""*/)
        //    {

        //        WebLog.Log("Image Path" + IdentImage);

        //        nysc.NyscIdCardFilePath = string.IsNullOrEmpty(IdentImage) ? "none" : /*slash + */imgPath;
        //        nysc.STA_FilePath = string.IsNullOrEmpty(IdentImages) ? "none" : /*slash +*/ imgPaths;

        //    }
        //    return View(nysc);
        //}



        [HttpGet]
        public ActionResult NYSCLoanAppFormEdit(string Refid, TableObjects.LoanApplication LoanApp, DataAccessA.Classes.AppLoanss Apploan, AppLoanss LoanApps, NyscLoanApplication nysc)

        {
            try

            { 
            TempData["SucMsg"] = ""; TempData["ErrMsg"] = ""; TempData["Error"] = "";
            nysc = _DR.GetRefid(Refid);
            NyscLoanApplication nyscx = new NyscLoanApplication();
            nyscx = nysc;
            //nysc = data;
            //nysc.NyscStateofResidence_FK = 0;
            //nysc.StateofResidence_FK = 0;
            //nysc.TempStateofResidence_FK = 0;
            // nysc.PassOutMonth = "";


            var Nyscfileimg = nysc.STA_FilePath;
            if (Nyscfileimg == null || Nyscfileimg == "")
            {

                Nyscfileimg = nysc.STA_FilePath;

            }

            WebLog.Log("ENTERED");
            TempData["Error"] = "";
            string referralCode = Request.QueryString["referralCode"];
            WebLog.Log("refelcode" + referralCode);
            if (referralCode != null || referralCode != "")
            {
                WebLog.Log("refelcode0" + referralCode);
                nysc.ReferralCode = referralCode;
                WebLog.Log("refelcode1" + referralCode);
            }

            ViewBag.nBanks = _DR.GetBanks();
            ViewBag.nRepmtMethods = _DR.GetRepaymentMethods();
            ViewBag.nStates = _DR.GetNigerianStates();
            ViewBag.nMeansOfIDs = _DR.GetMeansOfIdentifications();
            ViewBag.nAccomodationTypes = _DR.GetAccomodationTypes();
            ViewBag.nLGAs = _DR.GetAllLGAs();
            ViewBag.nTitles = _DR.GetTitles();
            ViewBag.nMarital = _DR.GetMaritalStatus();
            ViewBag.nLoanTenure = _DR.GetAllTenure();
            int val = 1;
            ViewData["nTitles"] = new SelectList(_DR.GetTitles(), "ID", "NAME", val);
            ViewData["nMarital"] = new SelectList(_DR.GetMaritalStatus(), "ID", "NAME", val);
            ViewData["nLGAs"] = new SelectList(_DR.GetAllLGAs(), "ID", "NAME", val);
            ViewData["nRelative"] = new SelectList(_DR.GetRelative(), "ID", "NAME", nysc.RelativeRelationship_FK);
            ViewData["nAccomodationTypes"] = new SelectList(_DR.GetAccomodationTypes(), "ID", "NAME", val);
            ViewData["nMeansOfIDs"] = new SelectList(_DR.GetMeansOfIdentifications(), "ID", "NAME", val);
            ViewData["nStates"] = new SelectList(_DR.GetNigerianStates(), "ID", "NAME", val);
            //nysc.StateofResidence_FKs = new SelectList(_DR.GetNigerianStates(), "ID", "NAME", 1);
            ViewData["nBanks"] = new SelectList(_DR.GetBanks(), "FlutterWaveBankCode", "NAME", val);
            ViewData["nRepmtMethods"] = new SelectList(_DR.GetRepaymentMethods(), "ID", "NAME", val);
            //ViewData["nLoanTenure"] = new SelectList(_DR.GetAllTenure(), "ID", "Tenure", val);
            ViewData["nGender"] = new SelectList(GetAllGender(), "Value", "Text", val);
            ViewData["nCDDays"] = new SelectList(GetDayOfTheWeek(), "Value", "Text", val);
            ViewData["nPassOutMonths"] = new SelectList(GetPassOutMonths(), "Value", "Text", val);
            ViewData["nemploymentStatus"] = new SelectList(GetAppStatus(), "Value", "Text", val);
            //ViewData["nLoanTenure"] = new SelectList(_DR.GetAllTenure(), "amount", "Tenure", val);
            ViewData["nlgaList"] = new SelectList(_DR.GetLGAsByStateFK((int)nysc.StateofResidence_FK), "ID", "NAME", nysc.StateofResidence_FK);
            ViewData["nlgaLists"] = new SelectList(_DR.GetLGAsByStateFK((int)nysc.TempStateofResidence_FK), "ID", "NAME", nysc.TempStateofResidence_FK);
            ViewData["nlgaListsss"] = new SelectList(_DR.GetLGAsByStateFK((int)nysc.NyscStateofResidence_FK), "ID", "NAME", nysc.NyscStateofResidence_FK);
            ViewData["nLoanTenure"] = new SelectList(_DR.GetAllTenure(), "Tenure", "Tenure", val);
                nysc.StateofResidence_FKs = Convert.ToString(nysc.StateofResidence_FK);
                nysc.TStateofResidence_FKss = Convert.ToString(nysc.TempStateofResidence_FK);
                nysc.NStateofResidence_FKsss = Convert.ToString(nysc.NyscStateofResidence_FK);

                nysc.LoanTenures = Convert.ToString(nysc.LoanTenure);
                nysc.LGA_FKs = Convert.ToString(nysc.LGA_FK);
                nysc.LGA_FKss = Convert.ToString(nysc.TempLGA_FK);
                nysc.LGA_FKsss = Convert.ToString(nysc.NyscLGA_FK);


                //nyscx.StateofResidence_FKs = Convert.ToString(nysc.StateofResidence_FK);
                //nyscx.TStateofResidence_FKss = Convert.ToString(nysc.TempStateofResidence_FK);
                //nyscx.NStateofResidence_FKsss = Convert.ToString(nysc.NyscStateofResidence_FK);

                //nyscx.LoanTenures = Convert.ToString(nysc.LoanTenure);
                //nyscx.LGA_FKs = Convert.ToString(nysc.LGA_FK);
                //nyscx.LGA_FKss = Convert.ToString(nysc.TempLGA_FK);
                //nyscx.LGA_FKsss = Convert.ToString(nysc.NyscLGA_FK);

                WebLog.Log("refelcode4" + referralCode);
            var channels = _DR.GetMarketChannel();
            ViewBag.channel = channels;
            // GetApplicantInfoss(Refid, LoanApp, Apploan, LoanApps, nysc);

            string IdentImage = nysc.NyscIdCardFilePath.After("h:\\root\\home\\paelyt-001\\www\\paelyt\\uvlotapublish\\").Replace("\\", "/");
            string IdentImages = nysc.STA_FilePath.After("h:\\root\\home\\paelyt-001\\www\\paelyt\\uvlotapublish\\").Replace("\\", "/");


            if (IdentImage != "" || IdentImages != "")
            {

                WebLog.Log("Image Path" + IdentImage);

                string slash = "/";

                nysc.NyscIdCardFilePath = string.IsNullOrEmpty(IdentImage) ? "none" : slash + IdentImage;
                nysc.STA_FilePath = string.IsNullOrEmpty(IdentImages) ? "none" : slash + IdentImages;
                nyscx.NyscIdCardFilePath = string.IsNullOrEmpty(IdentImage) ? "none" : slash + IdentImage;
                nyscx.STA_FilePath = string.IsNullOrEmpty(IdentImages) ? "none" : slash + IdentImages;
                    
            }
                return View(nyscx);
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }

        }




        public List<SelectListItem> GetAllGender()
        {
            try
            {
                List<SelectListItem> Gender = new List<SelectListItem>();
                Gender.Add(new SelectListItem { Value = "1", Text = "Male" });
                Gender.Add(new SelectListItem { Value = "2", Text = "Female" });
                return Gender;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        public List<SelectListItem> GetDayOfTheWeek()
        {
            try
            {
                List<SelectListItem> CDDay = new List<SelectListItem>();
                CDDay.Add(new SelectListItem { Value = "Monday", Text = "Monday" });
                CDDay.Add(new SelectListItem { Value = "Tuesday", Text = "Tuesday" });
                CDDay.Add(new SelectListItem { Value = "Wednesday", Text = "Wednesday" });
                CDDay.Add(new SelectListItem { Value = "Thursday", Text = "Thursday" });
                CDDay.Add(new SelectListItem { Value = "Friday", Text = "Friday" });
                return CDDay;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }




        public List<SelectListItem> GetPassOutMonths()
        {
            try
            {
                List<SelectListItem> PassOutMonths = new List<SelectListItem>();
                PassOutMonths.Add(new SelectListItem { Value = "January", Text = "January" });
                PassOutMonths.Add(new SelectListItem { Value = "February", Text = "February" });
                PassOutMonths.Add(new SelectListItem { Value = "March", Text = "March" });
                PassOutMonths.Add(new SelectListItem { Value = "April", Text = "April" });

                PassOutMonths.Add(new SelectListItem { Value = "May", Text = "May" });
                PassOutMonths.Add(new SelectListItem { Value = "June", Text = "June" });
                PassOutMonths.Add(new SelectListItem { Value = "July", Text = "July" });
                PassOutMonths.Add(new SelectListItem { Value = "August", Text = "August" });
                PassOutMonths.Add(new SelectListItem { Value = "September", Text = "September" });
                PassOutMonths.Add(new SelectListItem { Value = "October", Text = "October" });
                PassOutMonths.Add(new SelectListItem { Value = "November", Text = "November" });
                PassOutMonths.Add(new SelectListItem { Value = "December", Text = "December" });
                return PassOutMonths;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        public List<SelectListItem> GetAppStatus()
        {
            try
            {

                List<SelectListItem> Contract = new List<SelectListItem>();
                Contract.Add(new SelectListItem { Value = "1", Text = "Contract" });
                Contract.Add(new SelectListItem { Value = "2", Text = "Parmanent" });
                return Contract;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

       

        [HttpPost]
        public ActionResult NYSCLoanAppFormEdit(FormCollection form, HttpPostedFileBase PostedFile, DataAccessA.Classes.LoanApplication lApObj, HttpPostedFileBase NyscIDCard, HttpPostedFileBase StatementOfAccount, HttpPostedFileBase NyscPassport, HttpPostedFileBase NyscPostingLetter, HttpPostedFileBase NyscCallUpLetter, HttpPostedFileBase NyscProfileDashboard, NyscLoanApplication nysc)
        {
            try
            {
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = ""; TempData["Error"] = "";
                var channellist = Request["checkboxName"];

                if (channellist == null)
                {
                    TempData["Error"] = "Please Select Marketing Channel";
                    // NYSCLoanAppForm();
                    ViewData["nTitles"] = new SelectList(_DR.GetTitles(), "ID", "NAME", lApObj.Title_FK);
                    ViewData["nMarital"] = new SelectList(_DR.GetMaritalStatus(), "ID", "NAME", lApObj.MaritalStatus_FK);
                    ViewData["nLGAs"] = new SelectList(_DR.GetAllLGAs(), "ID", "NAME", lApObj.LGA_FK);
                    ViewData["nRelative"] = new SelectList(_DR.GetRelative(), "ID", "NAME", lApObj.RelativeRelationship_FK);
                    ViewData["nAccomodationTypes"] = new SelectList(_DR.GetAccomodationTypes(), "ID", "NAME", lApObj.AccomodationType_FK);
                    ViewData["nMeansOfIDs"] = new SelectList(_DR.GetMeansOfIdentifications(), "ID", "NAME", lApObj.MeansOfID_FK);
                    ViewData["nStates"] = new SelectList(_DR.GetNigerianStates(), "ID", "NAME", lApObj.StateofResidence_FK);
                    ViewData["nBanks"] = new SelectList(_DR.GetBanks(), "FlutterWaveBankCode", "NAME", lApObj.Bank_FK);
                    ViewData["nRepmtMethods"] = new SelectList(_DR.GetRepaymentMethods(), "ID", "NAME", lApObj.RepaymentMethod_FK);
                    ViewData["nLoanTenure"] = new SelectList(_DR.GetAllTenure(), "Tenure", "Tenure", lApObj.LoanTenures);
                    ViewData["nGender"] = new SelectList(GetAllGender(), "Value", "Text", lApObj.Gender_FK);
                    ViewData["nemploymentStatus"] = new SelectList(GetAppStatus(), "Value", "Text", lApObj.Contract);
                    ViewData["nCDDays"] = new SelectList(GetDayOfTheWeek(), "Value", "Text", lApObj.CDSDay);
                    var channels = _DR.GetMarketChannel();
                    ViewBag.channel = channels;
                    return View("NYSCLoanAppForm", lApObj);
                }
                string[] arr = channellist.Split(',');
                var chanList = removestring(arr);

                TempData["Error"] = "";
                var cPassOutMonth = lApObj.PassOutMonth;
                // DateTime cv = Convert.ToDateTime(lApObj.PassOutMonth);
                string respMsg = "";
                // validatePasout(cPassOutMonth,lApObj.LoanTenure);
                calc(cPassOutMonth, lApObj.LoanTenure, out respMsg);
                if (respMsg != "0")
                {
                    TempData["Error"] = respMsg;
                    // NYSCLoanAppForm();
                    ViewData["nTitles"] = new SelectList(_DR.GetTitles(), "ID", "NAME", lApObj.Title_FK);
                    ViewData["nMarital"] = new SelectList(_DR.GetMaritalStatus(), "ID", "NAME", lApObj.MaritalStatus_FK);
                    ViewData["nLGAs"] = new SelectList(_DR.GetAllLGAs(), "ID", "NAME", lApObj.LGA_FK);
                    ViewData["nRelative"] = new SelectList(_DR.GetRelative(), "ID", "NAME", lApObj.RelativeRelationship_FK);
                    ViewData["nAccomodationTypes"] = new SelectList(_DR.GetAccomodationTypes(), "ID", "NAME", lApObj.AccomodationType_FK);
                    ViewData["nMeansOfIDs"] = new SelectList(_DR.GetMeansOfIdentifications(), "ID", "NAME", lApObj.MeansOfID_FK);
                    ViewData["nStates"] = new SelectList(_DR.GetNigerianStates(), "ID", "NAME", lApObj.StateofResidence_FK);
                    ViewData["nBanks"] = new SelectList(_DR.GetBanks(), "FlutterWaveBankCode", "NAME", lApObj.Bank_FK);
                    ViewData["nRepmtMethods"] = new SelectList(_DR.GetRepaymentMethods(), "ID", "NAME", lApObj.RepaymentMethod_FK);
                    ViewData["nLoanTenure"] = new SelectList(_DR.GetAllTenure(), "Tenure", "Tenure", lApObj.LoanTenure);
                
                    ViewData["nGender"] = new SelectList(GetAllGender(), "Value", "Text", lApObj.Gender_FK);
                    ViewData["nemploymentStatus"] = new SelectList(GetAppStatus(), "Value", "Text", lApObj.Contract);
                    ViewData["nCDDays"] = new SelectList(GetDayOfTheWeek(), "Value", "Text", lApObj.CDSDay);
                    var channels = _DR.GetMarketChannel();
                    ViewBag.channel = channels;
                    return View("NYSCLoanAppForm", lApObj);
                }
                var dCDSDay = Convert.ToString(form["selectDays"]);
                //nysc.LoanTenures = Convert.ToString(nysc.LoanTenure);
                int LoanTenure = Convert.ToInt16(lApObj.LoanTenure);

                cPassOutMonth = lApObj.PassOutMonth;
                dCDSDay = lApObj.CDSDay;
                var Marital = lApObj.MaritalStatus_FK;
                var Tittle = lApObj.Title_FK;
                nysc.LGA_FKs = Convert.ToString(nysc.LGA_FK);
                nysc.LGA_FKss = Convert.ToString(nysc.TempLGA_FK);
                nysc.LGA_FKsss = Convert.ToString(nysc.NyscLGA_FK);
                nysc.StateofResidence_FKs = Convert.ToString(nysc.StateofResidence_FK);
                nysc.TStateofResidence_FKss = Convert.ToString(nysc.TempStateofResidence_FK);
                nysc.NStateofResidence_FKsss = Convert.ToString(nysc.NyscStateofResidence_FK);

                //var lgaList = Convert.ToInt16(form["lgaList"]);
                //var lgaLists = Convert.ToInt16(form["lgaLists"]);
                //var lgaListss = Convert.ToInt16(form["lgaListsss"]);
                var imgpath = "h:\\root\\home\\paelyt-001\\www\\paelyt\\uvlotapublish\\Images\\";


                string StatementOfAccounts = saveImages(StatementOfAccount);


                if (StatementOfAccounts == null || StatementOfAccounts == "")
                {

                    StatementOfAccounts = imgpath + nysc.STA_FilePath.After("/Images/");

                }


                string NyscIDCards = saveImage(NyscIDCard);

                if (NyscIDCards == null || NyscIDCards == "")
                {


                    NyscIDCards = imgpath + nysc.NyscIdCardFilePath.After("/Images/");
                }


                //string NyscIDCards = saveImage(NyscIDCard);
                //WebLog.Log("nyscPath Path" + NyscIDCards);
                //string StatementOfAccounts = saveImages(StatementOfAccount);
                //WebLog.Log("StatementOfAccounts Path" + StatementOfAccounts);

                //string Nycfileimgss = saveImage(NyscIDCard);


                //if (Nycfileimgss == null || Nycfileimgss == "")
                //{



                //   Nycfileimgss = imgpath + nysc.NyscIdCardFilePath.After("/Images/");

                //}


                //string NyscCallUpLetters = saveImg(NyscCallUpLetter);

                //if (NyscCallUpLetters == null || NyscCallUpLetters == "")
                //{



                //    NyscCallUpLetters = imgpath + nysc.NyscCallUpLetterFilePath.After("/Images/");

                //}




                //string NyscPassports = saveImgs(NyscPassport);

                //if (NyscPassports == null || NyscPassports == "")
                //{



                //    NyscPassports = imgpath + nysc.NyscpassportFilePath.After("/Images/");

                //}


                //string NyscPostingLetters = saveImgss(NyscPostingLetter);


                //if (NyscPostingLetters == null || NyscPostingLetters == "")
                //{



                //    NyscPostingLetters = imgpath + nysc.NyscPostingLetterFllePath.After("/Images/");
                //}

                //string NyscProfileDashboards = saveImgsss(NyscProfileDashboard); 


                //if (NyscProfileDashboards == null || NyscProfileDashboards == "")
                //{


                //    NyscProfileDashboards = imgpath + nysc.NyscProfileDashboardFilePath.After("/Images/");
                //}



                //if (nysc.NyscProfileDashboardFilePath == null || nysc.NyscProfileDashboardFilePath == "")
                //{
                //    TempData["Error"] = "Input Statement Of Accounts";

                //    ViewData["nTitles"] = new SelectList(_DR.GetTitles(), "ID", "NAME", lApObj.Title_FK);
                //    ViewData["nMarital"] = new SelectList(_DR.GetMaritalStatus(), "ID", "NAME", lApObj.MaritalStatus_FK);
                //    ViewData["nLGAs"] = new SelectList(_DR.GetAllLGAs(), "ID", "NAME", lApObj.LGA_FK);
                //    ViewData["nRelative"] = new SelectList(_DR.GetRelative(), "ID", "NAME", lApObj.RelativeRelationship_FK);
                //    ViewData["nAccomodationTypes"] = new SelectList(_DR.GetAccomodationTypes(), "ID", "NAME", lApObj.AccomodationType_FK);
                //    ViewData["nMeansOfIDs"] = new SelectList(_DR.GetMeansOfIdentifications(), "ID", "NAME", lApObj.MeansOfID_FK);
                //    ViewData["nStates"] = new SelectList(_DR.GetNigerianStates(), "ID", "NAME", lApObj.StateofResidence_FK);
                //    ViewData["nBanks"] = new SelectList(_DR.GetBanks(), "FlutterWaveBankCode", "NAME", lApObj.Bank_FK);
                //    ViewData["nRepmtMethods"] = new SelectList(_DR.GetRepaymentMethods(), "ID", "NAME", lApObj.RepaymentMethod_FK);
                //    ViewData["nLoanTenure"] = new SelectList(_DR.GetAllTenure(), "ID", "Tenure", lApObj.LoanTenure);
                //    ViewData["nGender"] = new SelectList(GetAllGender(), "Value", "Text", lApObj.Gender_FK);
                //    ViewData["nemploymentStatus"] = new SelectList(GetAppStatus(), "Value", "Text", lApObj.Contract);
                //    ViewData["nCDDays"] = new SelectList(GetDayOfTheWeek(), "Value", "Text", lApObj.CDSDay);
                //    var channels = _DR.GetMarketChannel();
                //    ViewBag.channel = channels;
                //    return View("NYSCLoanAppForm", lApObj);
                //}
                if (nysc.STA_FilePath == null || nysc.STA_FilePath == "")

                {
                    TempData["Error"] = "Input Nysc Statement Of Account";

                    ViewData["nTitles"] = new SelectList(_DR.GetTitles(), "ID", "NAME", lApObj.Title_FK);
                    ViewData["nMarital"] = new SelectList(_DR.GetMaritalStatus(), "ID", "NAME", lApObj.MaritalStatus_FK);
                    ViewData["nLGAs"] = new SelectList(_DR.GetAllLGAs(), "ID", "NAME", lApObj.LGA_FK);
                    ViewData["nRelative"] = new SelectList(_DR.GetRelative(), "ID", "NAME", lApObj.RelativeRelationship_FK);
                    ViewData["nAccomodationTypes"] = new SelectList(_DR.GetAccomodationTypes(), "ID", "NAME", lApObj.AccomodationType_FK);
                    ViewData["nMeansOfIDs"] = new SelectList(_DR.GetMeansOfIdentifications(), "ID", "NAME", lApObj.MeansOfID_FK);
                    ViewData["nStates"] = new SelectList(_DR.GetNigerianStates(), "ID", "NAME", lApObj.StateofResidence_FK);
                    ViewData["nBanks"] = new SelectList(_DR.GetBanks(), "FlutterWaveBankCode", "NAME", lApObj.Bank_FK);
                    ViewData["nRepmtMethods"] = new SelectList(_DR.GetRepaymentMethods(), "ID", "NAME", lApObj.RepaymentMethod_FK);
                    ViewData["nLoanTenure"] = new SelectList(_DR.GetAllTenure(), "ID", "Tenure", lApObj.LoanTenure);
                    ViewData["nGender"] = new SelectList(GetAllGender(), "Value", "Text", lApObj.Gender_FK);
                    ViewData["nemploymentStatus"] = new SelectList(GetAppStatus(), "Value", "Text", lApObj.Contract);
                    ViewData["nCDDays"] = new SelectList(GetDayOfTheWeek(), "Value", "Text", lApObj.CDSDay);
                    var channels = _DR.GetMarketChannel();
                    ViewBag.channel = channels;
                    return View("NYSCLoanAppForm", lApObj);
                }


                if (nysc.NyscIdCardFilePath == null || nysc.NyscIdCardFilePath == "")
                {
                    TempData["Error"] = "Input Nysc ID Cards";

                    ViewData["nTitles"] = new SelectList(_DR.GetTitles(), "ID", "NAME", lApObj.Title_FK);
                    ViewData["nMarital"] = new SelectList(_DR.GetMaritalStatus(), "ID", "NAME", lApObj.MaritalStatus_FK);
                    ViewData["nLGAs"] = new SelectList(_DR.GetAllLGAs(), "ID", "NAME", lApObj.LGA_FK);
                    ViewData["nRelative"] = new SelectList(_DR.GetRelative(), "ID", "NAME", lApObj.RelativeRelationship_FK);
                    ViewData["nAccomodationTypes"] = new SelectList(_DR.GetAccomodationTypes(), "ID", "NAME", lApObj.AccomodationType_FK);
                    ViewData["nMeansOfIDs"] = new SelectList(_DR.GetMeansOfIdentifications(), "ID", "NAME", lApObj.MeansOfID_FK);
                    ViewData["nStates"] = new SelectList(_DR.GetNigerianStates(), "ID", "NAME", lApObj.StateofResidence_FK);
                    ViewData["nBanks"] = new SelectList(_DR.GetBanks(), "FlutterWaveBankCode", "NAME", lApObj.Bank_FK);
                    ViewData["nRepmtMethods"] = new SelectList(_DR.GetRepaymentMethods(), "ID", "NAME", lApObj.RepaymentMethod_FK);
                    ViewData["nLoanTenure"] = new SelectList(_DR.GetAllTenure(), "ID", "Tenure", lApObj.LoanTenure);
                    ViewData["nGender"] = new SelectList(GetAllGender(), "Value", "Text", lApObj.Gender_FK);
                    ViewData["nemploymentStatus"] = new SelectList(GetAppStatus(), "Value", "Text", lApObj.Contract);
                    ViewData["nCDDays"] = new SelectList(GetDayOfTheWeek(), "Value", "Text", lApObj.CDSDay);
                    var channels = _DR.GetMarketChannel();
                    ViewBag.channel = channels;
                    return View("NYSCLoanAppForm", lApObj);

                }
                //if (NyscIDCard.ContentLength == 0)
                //{
                //    TempData["Error"] = "Input Nysc ID Card";

                //    ViewData["nTitles"] = new SelectList(_DR.GetTitles(), "ID", "NAME", lApObj.Title_FK);
                //    ViewData["nMarital"] = new SelectList(_DR.GetMaritalStatus(), "ID", "NAME", lApObj.MaritalStatus_FK);
                //    ViewData["nLGAs"] = new SelectList(_DR.GetAllLGAs(), "ID", "NAME", lApObj.LGA_FK);
                //    ViewData["nRelative"] = new SelectList(_DR.GetRelative(), "ID", "NAME", lApObj.RelativeRelationship_FK);
                //    ViewData["nAccomodationTypes"] = new SelectList(_DR.GetAccomodationTypes(), "ID", "NAME", lApObj.AccomodationType_FK);
                //    ViewData["nMeansOfIDs"] = new SelectList(_DR.GetMeansOfIdentifications(), "ID", "NAME", lApObj.MeansOfID_FK);
                //    ViewData["nStates"] = new SelectList(_DR.GetNigerianStates(), "ID", "NAME", lApObj.StateofResidence_FK);
                //    ViewData["nBanks"] = new SelectList(_DR.GetBanks(), "FlutterWaveBankCode", "NAME", lApObj.Bank_FK);
                //    ViewData["nRepmtMethods"] = new SelectList(_DR.GetRepaymentMethods(), "ID", "NAME", lApObj.RepaymentMethod_FK);
                //    ViewData["nLoanTenure"] = new SelectList(_DR.GetAllTenure(), "ID", "Tenure", lApObj.LoanTenure);
                //    ViewData["nGender"] = new SelectList(GetAllGender(), "Value", "Text", lApObj.Gender_FK);
                //    ViewData["nemploymentStatus"] = new SelectList(GetAppStatus(), "Value", "Text", lApObj.Contract);
                //    ViewData["nCDDays"] = new SelectList(GetDayOfTheWeek(), "Value", "Text", lApObj.CDSDay);
                //    var channels = _DR.GetMarketChannel();
                //    ViewBag.channel = channels;
                //    return View("NYSCLoanAppForm", lApObj);
                //}

                //if (StatementOfAccount.ContentLength == 0)
                //{
                //    TempData["Error"] = "Input Nysc Statement Of Account";

                //    ViewData["nTitles"] = new SelectList(_DR.GetTitles(), "ID", "NAME", lApObj.Title_FK);
                //    ViewData["nMarital"] = new SelectList(_DR.GetMaritalStatus(), "ID", "NAME", lApObj.MaritalStatus_FK);
                //    ViewData["nLGAs"] = new SelectList(_DR.GetAllLGAs(), "ID", "NAME", lApObj.LGA_FK);
                //    ViewData["nRelative"] = new SelectList(_DR.GetRelative(), "ID", "NAME", lApObj.RelativeRelationship_FK);
                //    ViewData["nAccomodationTypes"] = new SelectList(_DR.GetAccomodationTypes(), "ID", "NAME", lApObj.AccomodationType_FK);
                //    ViewData["nMeansOfIDs"] = new SelectList(_DR.GetMeansOfIdentifications(), "ID", "NAME", lApObj.MeansOfID_FK);
                //    ViewData["nStates"] = new SelectList(_DR.GetNigerianStates(), "ID", "NAME", lApObj.StateofResidence_FK);
                //    ViewData["nBanks"] = new SelectList(_DR.GetBanks(), "FlutterWaveBankCode", "NAME", lApObj.Bank_FK);
                //    ViewData["nRepmtMethods"] = new SelectList(_DR.GetRepaymentMethods(), "ID", "NAME", lApObj.RepaymentMethod_FK);
                //    ViewData["nLoanTenure"] = new SelectList(_DR.GetAllTenure(), "ID", "Tenure", lApObj.LoanTenure);
                //    ViewData["nGender"] = new SelectList(GetAllGender(), "Value", "Text", lApObj.Gender_FK);
                //    ViewData["nemploymentStatus"] = new SelectList(GetAppStatus(), "Value", "Text", lApObj.Contract);
                //    ViewData["nCDDays"] = new SelectList(GetDayOfTheWeek(), "Value", "Text", lApObj.CDSDay);
                //    var channels = _DR.GetMarketChannel();
                //    ViewBag.channel = channels;
                //    return View("NYSCLoanAppForm", lApObj);
                //}


                //if (nysc.NyscPostingLetterFllePath == null || nysc.NyscPostingLetterFllePath == "")
                //{
                //    TempData["Error"] = "Input Nysc Posting Letter";

                //    ViewData["nTitles"] = new SelectList(_DR.GetTitles(), "ID", "NAME", lApObj.Title_FK);
                //    ViewData["nMarital"] = new SelectList(_DR.GetMaritalStatus(), "ID", "NAME", lApObj.MaritalStatus_FK);
                //    ViewData["nLGAs"] = new SelectList(_DR.GetAllLGAs(), "ID", "NAME", lApObj.LGA_FK);
                //    ViewData["nRelative"] = new SelectList(_DR.GetRelative(), "ID", "NAME", lApObj.RelativeRelationship_FK);
                //    ViewData["nAccomodationTypes"] = new SelectList(_DR.GetAccomodationTypes(), "ID", "NAME", lApObj.AccomodationType_FK);
                //    ViewData["nMeansOfIDs"] = new SelectList(_DR.GetMeansOfIdentifications(), "ID", "NAME", lApObj.MeansOfID_FK);
                //    ViewData["nStates"] = new SelectList(_DR.GetNigerianStates(), "ID", "NAME", lApObj.StateofResidence_FK);
                //    ViewData["nBanks"] = new SelectList(_DR.GetBanks(), "FlutterWaveBankCode", "NAME", lApObj.Bank_FK);
                //    ViewData["nRepmtMethods"] = new SelectList(_DR.GetRepaymentMethods(), "ID", "NAME", lApObj.RepaymentMethod_FK);
                //    ViewData["nLoanTenure"] = new SelectList(_DR.GetAllTenure(), "ID", "Tenure", lApObj.LoanTenure);
                //    ViewData["nGender"] = new SelectList(GetAllGender(), "Value", "Text", lApObj.Gender_FK);
                //    ViewData["nemploymentStatus"] = new SelectList(GetAppStatus(), "Value", "Text", lApObj.Contract);
                //    ViewData["nCDDays"] = new SelectList(GetDayOfTheWeek(), "Value", "Text", lApObj.CDSDay);
                //    var channels = _DR.GetMarketChannel();
                //    ViewBag.channel = channels;
                //    return View("NYSCLoanAppForm", lApObj);
                //}

                //if (nysc.NyscCallUpLetterFilePath == null || nysc.NyscCallUpLetterFilePath == "")
                //{
                //    TempData["Error"] = "Input Nysc CallUp Letter";

                //    ViewData["nTitles"] = new SelectList(_DR.GetTitles(), "ID", "NAME", lApObj.Title_FK);
                //    ViewData["nMarital"] = new SelectList(_DR.GetMaritalStatus(), "ID", "NAME", lApObj.MaritalStatus_FK);
                //    ViewData["nLGAs"] = new SelectList(_DR.GetAllLGAs(), "ID", "NAME", lApObj.LGA_FK);
                //    ViewData["nRelative"] = new SelectList(_DR.GetRelative(), "ID", "NAME", lApObj.RelativeRelationship_FK);
                //    ViewData["nAccomodationTypes"] = new SelectList(_DR.GetAccomodationTypes(), "ID", "NAME", lApObj.AccomodationType_FK);
                //    ViewData["nMeansOfIDs"] = new SelectList(_DR.GetMeansOfIdentifications(), "ID", "NAME", lApObj.MeansOfID_FK);
                //    ViewData["nStates"] = new SelectList(_DR.GetNigerianStates(), "ID", "NAME", lApObj.StateofResidence_FK);
                //    ViewData["nBanks"] = new SelectList(_DR.GetBanks(), "FlutterWaveBankCode", "NAME", lApObj.Bank_FK);
                //    ViewData["nRepmtMethods"] = new SelectList(_DR.GetRepaymentMethods(), "ID", "NAME", lApObj.RepaymentMethod_FK);
                //    ViewData["nLoanTenure"] = new SelectList(_DR.GetAllTenure(), "ID", "Tenure", lApObj.LoanTenure);
                //    ViewData["nGender"] = new SelectList(GetAllGender(), "Value", "Text", lApObj.Gender_FK);
                //    ViewData["nemploymentStatus"] = new SelectList(GetAppStatus(), "Value", "Text", lApObj.Contract);
                //    ViewData["nCDDays"] = new SelectList(GetDayOfTheWeek(), "Value", "Text", lApObj.CDSDay);
                //    var channels = _DR.GetMarketChannel();
                //    ViewBag.channel = channels;
                //    return View("NYSCLoanAppForm", lApObj);
                //}

                //if (nysc.NyscProfileDashboardFilePath == null || nysc.NyscProfileDashboardFilePath == "")
                //{
                //    TempData["Error"] = "Input Nysc Profile Dashboard screenshot";

                //    ViewData["nTitles"] = new SelectList(_DR.GetTitles(), "ID", "NAME", lApObj.Title_FK);
                //    ViewData["nMarital"] = new SelectList(_DR.GetMaritalStatus(), "ID", "NAME", lApObj.MaritalStatus_FK);
                //    ViewData["nLGAs"] = new SelectList(_DR.GetAllLGAs(), "ID", "NAME", lApObj.LGA_FK);
                //    ViewData["nRelative"] = new SelectList(_DR.GetRelative(), "ID", "NAME", lApObj.RelativeRelationship_FK);
                //    ViewData["nAccomodationTypes"] = new SelectList(_DR.GetAccomodationTypes(), "ID", "NAME", lApObj.AccomodationType_FK);
                //    ViewData["nMeansOfIDs"] = new SelectList(_DR.GetMeansOfIdentifications(), "ID", "NAME", lApObj.MeansOfID_FK);
                //    ViewData["nStates"] = new SelectList(_DR.GetNigerianStates(), "ID", "NAME", lApObj.StateofResidence_FK);
                //    ViewData["nBanks"] = new SelectList(_DR.GetBanks(), "FlutterWaveBankCode", "NAME", lApObj.Bank_FK);
                //    ViewData["nRepmtMethods"] = new SelectList(_DR.GetRepaymentMethods(), "ID", "NAME", lApObj.RepaymentMethod_FK);
                //    ViewData["nLoanTenure"] = new SelectList(_DR.GetAllTenure(), "ID", "Tenure", lApObj.LoanTenure);
                //    ViewData["nGender"] = new SelectList(GetAllGender(), "Value", "Text", lApObj.Gender_FK);
                //    ViewData["nemploymentStatus"] = new SelectList(GetAppStatus(), "Value", "Text", lApObj.Contract);
                //    ViewData["nCDDays"] = new SelectList(GetDayOfTheWeek(), "Value", "Text", lApObj.CDSDay);
                //    var channels = _DR.GetMarketChannel();
                //    ViewBag.channel = channels;
                //    return View("NYSCLoanAppForm", lApObj);
                //}

                var OutstandingAmount = lApObj.ExistingLoan_OutstandingAmount;
                if (OutstandingAmount == null)
                {
                    lApObj.ExistingLoan_OutstandingAmount = 0;
                }
                int Noofmonthsleft = 0;
                int.TryParse(lApObj.ExistingLoan_NoOfMonthsLeft, out Noofmonthsleft);

                lApObj.ExistingLoan_NoOfMonthsLeft = Noofmonthsleft.ToString();

                var Gender_FK = lApObj.Gender_FK;
                string DOB = lApObj.DateOfBirth.ToString();

                if (lApObj.AccountName == null || lApObj.AccountName == "" || lApObj.AccountName == "Invalid Account Number")
                {
                    TempData["Error"] = "Invalid Account Name";
                   
                    ViewData["nTitles"] = new SelectList(_DR.GetTitles(), "ID", "NAME", lApObj.Title_FK);
                    ViewData["nMarital"] = new SelectList(_DR.GetMaritalStatus(), "ID", "NAME", lApObj.MaritalStatus_FK);
                    ViewData["nLGAs"] = new SelectList(_DR.GetAllLGAs(), "ID", "NAME", lApObj.LGA_FK);
                    ViewData["nRelative"] = new SelectList(_DR.GetRelative(), "ID", "NAME", lApObj.RelativeRelationship_FK);
                    ViewData["nAccomodationTypes"] = new SelectList(_DR.GetAccomodationTypes(), "ID", "NAME", lApObj.AccomodationType_FK);
                    ViewData["nMeansOfIDs"] = new SelectList(_DR.GetMeansOfIdentifications(), "ID", "NAME", lApObj.MeansOfID_FK);
                    ViewData["nStates"] = new SelectList(_DR.GetNigerianStates(), "ID", "NAME", lApObj.StateofResidence_FK);
                    ViewData["nBanks"] = new SelectList(_DR.GetBanks(), "FlutterWaveBankCode", "NAME", lApObj.Bank_FK);
                    ViewData["nRepmtMethods"] = new SelectList(_DR.GetRepaymentMethods(), "ID", "NAME", lApObj.RepaymentMethod_FK);
                    ViewData["nLoanTenure"] = new SelectList(_DR.GetAllTenure(), "ID", "Tenure", lApObj.LoanTenure);
                    ViewData["nGender"] = new SelectList(GetAllGender(), "Value", "Text", lApObj.Gender_FK);
                    ViewData["nemploymentStatus"] = new SelectList(GetAppStatus(), "Value", "Text", lApObj.Contract);
                    ViewData["nCDDays"] = new SelectList(GetDayOfTheWeek(), "Value", "Text", lApObj.CDSDay);
                    var channels = _DR.GetMarketChannel();
                    ViewBag.channel = channels;
                    return View("NYSCLoanAppForm", lApObj);
                }
                double repaymentAmt = 0;
                double.TryParse(lApObj.RepaymentAmount, out repaymentAmt);

                DataAccessA.DataManager.NyscLoanApplication NyscLA = new DataAccessA.DataManager.NyscLoanApplication
                {
                    ID = lApObj.ID,
                    AccountNumber = lApObj.AccountNumber,
                    AccountName = lApObj.AccountName,
                    Firstname = lApObj.Firstname,
                    Othernames = lApObj.Othernames,
                    NYSCApplicationStatus_FK = 1,
                    PPA_Department = lApObj.PPA_Department,
                    PPA_EmailAddress = lApObj.PPA_EmailAddress,
                    PPA_PhoneNumber = lApObj.PPA_PhoneNumber,
                    PPA_ROle = lApObj.PPA_ROle,
                    PPA_supervisorEmail = lApObj.PPA_supervisorEmail,
                    PPA_supervisorName = lApObj.PPA_supervisorName,
                    PPA_supervisorPhonenumber = lApObj.PPA_supervisorPhonenumber,
                    EMG_FullName2 = lApObj.EMG_FullName2,
                    EMG_EmailAddress2 = lApObj.EMG_EmailAddress2,
                    EMG_HomeAddress2 = lApObj.EMG_HomeAddress2,
                    EMG_PhoneNumber2 = lApObj.EMG_PhoneNumber2,
                    EMG_Relationship2 = lApObj.EMG_Relationship2,
                    FirstRelativeName = lApObj.FirstRelativeName,
                    FirstRelativePhoneNumber = lApObj.FirstRelativePhoneNumber,
                    RelativeRelationship2_FK = lApObj.RelativeRelationship2_FK,
                    SecondRelativeName = lApObj.SecondRelativeName,
                    SecondRelativePhoneNumber = lApObj.SecondRelativePhoneNumber,
                    RelativeRelationship_FK = lApObj.RelativeRelationship_FK,
                    NyscIdCardFilePath = NyscIDCards,
                    STA_FilePath = StatementOfAccounts,
                    //NyscpassportFilePath = NyscPassports,
                    //NyscCallUpLetterFilePath = NyscCallUpLetters,
                    //NyscPostingLetterFllePath = NyscPostingLetters,
                    //NyscProfileDashboardFilePath = NyscProfileDashboards,
                    FacebookName = lApObj.FacebookName,
                    InstagramHandle = lApObj.InstagramHandle,
                    TwitterHandle = lApObj.TwitterHandle,
                    RepaymentAmount = repaymentAmt,
                    RefNumber = lApObj.LoanRefNumber,
                    Gender_FK = Gender_FK,//Convert.ToInt32(form["selectGender"]),
                    MaritalStatus_FK = lApObj.MaritalStatus_FK,//Convert.ToInt16(form["Marital"]),
                    Surname = lApObj.Surname,
                    //CreatedBy = Convert.ToString(userid),
                    DateOfBirth = Convert.ToString(lApObj.DateOfBirth),
                    Title_FK = lApObj.Title_FK,//Convert.ToInt32(form["Titles"]),
                    PhoneNumber = lApObj.PhoneNumber,
                    EmailAddress = lApObj.EmailAddress,
                    PermanentAddress = lApObj.PermanentAddress,
                    Landmark = lApObj.Landmark,
                    ClosestBusStop = lApObj.ClosestBusStop,
                //    nysc.LGA_FKs = Convert.ToString(nysc.LGA_FK);
                //nysc.LGA_FKss = Convert.ToString(nysc.TempLGA_FK);
                //nysc.LGA_FKsss = Convert.ToString(nysc.NyscLGA_FK);
                LGA_FK = Convert.ToInt16(lApObj.LGA_FKs),////Convert.ToInt32(form["States"]),
                    TempLGA_FK = Convert.ToInt16(lApObj.LGA_FKss),//Convert.ToInt32(form["States"]),
                    NyscLGA_FK = Convert.ToInt16(lApObj.LGA_FKsss),//Convert.ToInt32(form["States"]),
                    StateofResidence_FK = Convert.ToInt16(lApObj.StateofResidence_FKs),////Convert.ToInt32(form["States"]),
                    TempStateofResidence_FK = Convert.ToInt16(lApObj.TStateofResidence_FKss),//Convert.ToInt32(form["States"]),
                    NyscStateofResidence_FK = Convert.ToInt16(lApObj.NStateofResidence_FKsss),//Convert.ToInt32(form["States"]),
                    TemporaryAddress = lApObj.TemporaryAddress,
                    OfficialAddress = lApObj.OfficialAddress,
                    StateCode = lApObj.StateCode,
                    Employer = lApObj.Employer,
                    PassOutMonth = lApObj.PassOutMonth,
                    CDSDay = lApObj.CDSDay,
                    TempLandmark = lApObj.TempLandmark,
                    TempClosestBusStop = lApObj.TempClosestBusStop,
                    ReferralCode = lApObj.ReferalCode,
                    BVN = lApObj.BVN,
                    CDSGroup = lApObj.CDSGroup,
                    NetMonthlyIncome = Convert.ToDouble(lApObj.NetMonthlyIncome),
                    EMG_EmailAddress = lApObj.EMG_EmailAddress,
                    EMG_FullName = lApObj.EMG_FullName,
                    EMG_HomeAddress = lApObj.EMG_HomeAddress,
                    EMG_PhoneNumber = lApObj.EMG_PhoneNumber,
                    EMG_Relationship = lApObj.EMG_Relationship,
                   
                    LoanAmount = Convert.ToDouble(lApObj.LoanAmount),
                    //LoanTenure = Convert.ToInt16(lApObj.LoanTenures),
                    LoanTenure =LoanTenure,
                    ExistingLoan = lApObj.ExistingLoan,
                    LoanComment = lApObj.LoanComment,
                    ExistingLoan_NoOfMonthsLeft = Convert.ToInt16(lApObj.ExistingLoan_NoOfMonthsLeft),
                    ExistingLoan_OutstandingAmount = lApObj.ExistingLoan_OutstandingAmount,
                    BankCode = Helper.GetRemitaBankCodeByFlutterCode(lApObj.BankCode),
                    IsVisible = 1,
                    DateCreated = MyUtility.getCurrentLocalDateTime(),
                    DateModified = MyUtility.getCurrentLocalDateTime(),
                    //ValueDate = MyUtility.getCurrentLocalDateTime().ToString("yyyy/MM/dd"),
                    ValueDate = MyUtility.getCurrentLocalDateTime().ToString("dddd, dd MMMM yyyy"),
                    ValueTime = MyUtility.getCurrentLocalDateTime().ToString("H:mmss"),
                    MarketingChannel = chanList.ToString(),
                };
                        var id = _DR.UpdateNyscLoanApplications(NyscLA);

                if (id == 0)
                {
                    TempData["Error"] = "Please Check Next Of kin Phone Number";
                    //NYSCLoanAppForm();
                    ViewData["nTitles"] = new SelectList(_DR.GetTitles(), "ID", "NAME", lApObj.Title_FK);
                    ViewData["nMarital"] = new SelectList(_DR.GetMaritalStatus(), "ID", "NAME", lApObj.MaritalStatus_FK);
                    ViewData["nLGAs"] = new SelectList(_DR.GetAllLGAs(), "ID", "NAME", lApObj.LGA_FK);
                    ViewData["nRelative"] = new SelectList(_DR.GetRelative(), "ID", "NAME", lApObj.RelativeRelationship_FK);
                    ViewData["nAccomodationTypes"] = new SelectList(_DR.GetAccomodationTypes(), "ID", "NAME", lApObj.AccomodationType_FK);
                    ViewData["nMeansOfIDs"] = new SelectList(_DR.GetMeansOfIdentifications(), "ID", "NAME", lApObj.MeansOfID_FK);
                    ViewData["nStates"] = new SelectList(_DR.GetNigerianStates(), "ID", "NAME", lApObj.StateofResidence_FK);
                    ViewData["nBanks"] = new SelectList(_DR.GetBanks(), "FlutterWaveBankCode", "NAME", lApObj.Bank_FK);
                    ViewData["nRepmtMethods"] = new SelectList(_DR.GetRepaymentMethods(), "ID", "NAME", lApObj.RepaymentMethod_FK);
                    ViewData["nLoanTenure"] = new SelectList(_DR.GetAllTenure(), "ID", "LoanTenure", lApObj.LoanTenure);
                    ViewData["nGender"] = new SelectList(GetAllGender(), "Value", "Text", lApObj.Gender_FK);
                    ViewData["nemploymentStatus"] = new SelectList(GetAppStatus(), "Value", "Text", lApObj.Contract);
                    var channels = _DR.GetMarketChannel();
                    ViewBag.channel = channels;
                    return View("NYSCLoanAppFormEdit", NyscLA);
                }

                if (id != 0)
                {

                    string idv = "";
                    MarketingChannel Mc = new MarketingChannel();
                    if (arr.Length > 0)
                    {
                        for (var i = 0; i < arr.Length; i++)
                        {
                            string arrc = Convert.ToString(arr[i]);
                            insertMarketChannel(arrc, id);
                        }

                    }
                    var email = _DR.getUser(NyscLA.EmailAddress);
                    string password = "";
                    string referralCode = "";
                    if (email == null)
                    {

                        referralCode = createUser(NyscLA, out password);
                        SendReferralEmail(NyscLA, referralCode, password);

                    }

                    SendEmail(NyscLA);
                    
                    return RedirectToAction("Acknowledgements", new { @ID = NyscLA.ID });
                }
                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }







        //Old
        //[HttpPost]
        //public ActionResult NYSCLoanAppFormEdit(FormCollection form, HttpPostedFileBase PostedFile, DataAccessA.Classes.LoanApplication lApObj, HttpPostedFileBase NyscIDCard, HttpPostedFileBase StatementOfAccount, HttpPostedFileBase NyscPassport, HttpPostedFileBase NyscPostingLetter, HttpPostedFileBase NyscCallUpLetter, HttpPostedFileBase NyscProfileDashboard, NyscLoanApplication nysc)
        //{
        //    try
        //    {
        //        TempData["SucMsg"] = ""; TempData["ErrMsg"] = ""; TempData["Error"] = "";
        //        var channellist = Request["checkboxName"];

        //        if (channellist == null)
        //        {
        //            TempData["Error"] = "Please Select Marketing Channel";
        //            // NYSCLoanAppForm();
        //            ViewData["nTitles"] = new SelectList(_DR.GetTitles(), "ID", "NAME", lApObj.Title_FK);
        //            ViewData["nMarital"] = new SelectList(_DR.GetMaritalStatus(), "ID", "NAME", lApObj.MaritalStatus_FK);
        //            ViewData["nLGAs"] = new SelectList(_DR.GetAllLGAs(), "ID", "NAME", lApObj.LGA_FK);
        //            ViewData["nAccomodationTypes"] = new SelectList(_DR.GetAccomodationTypes(), "ID", "NAME", lApObj.AccomodationType_FK);
        //            ViewData["nMeansOfIDs"] = new SelectList(_DR.GetMeansOfIdentifications(), "ID", "NAME", lApObj.MeansOfID_FK);
        //            ViewData["nStates"] = new SelectList(_DR.GetNigerianStates(), "ID", "NAME", lApObj.StateofResidence_FK);
        //            ViewData["nBanks"] = new SelectList(_DR.GetBanks(), "FlutterWaveBankCode", "NAME", lApObj.Bank_FK);
        //            ViewData["nRepmtMethods"] = new SelectList(_DR.GetRepaymentMethods(), "ID", "NAME", lApObj.RepaymentMethod_FK);
        //            ViewData["nLoanTenure"] = new SelectList(_DR.GetAllTenure(), "ID", "Tenure", lApObj.LoanTenure);
        //            ViewData["nGender"] = new SelectList(GetAllGender(), "Value", "Text", lApObj.Gender_FK);
        //            ViewData["nemploymentStatus"] = new SelectList(GetAppStatus(), "Value", "Text", lApObj.Contract);
        //            ViewData["nCDDays"] = new SelectList(GetDayOfTheWeek(), "Value", "Text", lApObj.CDSDay);
        //            var channels = _DR.GetMarketChannel();
        //            ViewBag.channel = channels;
        //            return View("NYSCLoanAppForm", lApObj);
        //        }
        //        string[] arr = channellist.Split(',');
        //        var chanList = removestring(arr);

        //        TempData["Error"] = "";
        //        var cPassOutMonth = lApObj.PassOutMonth;
        //        // DateTime cv = Convert.ToDateTime(lApObj.PassOutMonth);
        //        string respMsg = "";
        //        // validatePasout(cPassOutMonth,lApObj.LoanTenure);
        //        calc(cPassOutMonth, lApObj.LoanTenure, out respMsg);
        //        if (respMsg != "0")
        //        {
        //            TempData["Error"] = respMsg;
        //            // NYSCLoanAppForm();
        //            ViewData["nTitles"] = new SelectList(_DR.GetTitles(), "ID", "NAME", lApObj.Title_FK);
        //            ViewData["nMarital"] = new SelectList(_DR.GetMaritalStatus(), "ID", "NAME", lApObj.MaritalStatus_FK);
        //            ViewData["nLGAs"] = new SelectList(_DR.GetAllLGAs(), "ID", "NAME", lApObj.LGA_FK);
        //            ViewData["nAccomodationTypes"] = new SelectList(_DR.GetAccomodationTypes(), "ID", "NAME", lApObj.AccomodationType_FK);
        //            ViewData["nMeansOfIDs"] = new SelectList(_DR.GetMeansOfIdentifications(), "ID", "NAME", lApObj.MeansOfID_FK);
        //            ViewData["nStates"] = new SelectList(_DR.GetNigerianStates(), "ID", "NAME", lApObj.StateofResidence_FK);
        //            ViewData["nBanks"] = new SelectList(_DR.GetBanks(), "FlutterWaveBankCode", "NAME", lApObj.Bank_FK);
        //            ViewData["nRepmtMethods"] = new SelectList(_DR.GetRepaymentMethods(), "ID", "NAME", lApObj.RepaymentMethod_FK);
        //            ViewData["nLoanTenure"] = new SelectList(_DR.GetAllTenure(), "ID", "Tenure", lApObj.LoanTenure);
        //            ViewData["nGender"] = new SelectList(GetAllGender(), "Value", "Text", lApObj.Gender_FK);
        //            ViewData["nemploymentStatus"] = new SelectList(GetAppStatus(), "Value", "Text", lApObj.Contract);
        //            ViewData["nCDDays"] = new SelectList(GetDayOfTheWeek(), "Value", "Text", lApObj.CDSDay);
        //            var channels = _DR.GetMarketChannel();
        //            ViewBag.channel = channels;
        //            return View("NYSCLoanAppForm", lApObj);
        //        }
        //        var dCDSDay = Convert.ToString(form["selectDays"]);
        //        int LoanTenure = Convert.ToInt16(lApObj.LoanTenure);
        //        //  LoanTenure = LoanTenure;
        //        // lApObj.ValueDate = MyUtility.getCurrentLocalDateTime().ToString("dd/MM/yyyy H:mm ss");
        //        //lApObj.RepaymentAmount= _DR.GetRepaymenrAmount(LoanTenure);
        //        // lApObj.LoanTenureStr = LoanTenure.ToString() + " months";
        //        cPassOutMonth = lApObj.PassOutMonth;
        //        dCDSDay = lApObj.CDSDay;
        //        var Marital = lApObj.MaritalStatus_FK;
        //        var Tittle = lApObj.Title_FK;

        //        var lgaList = Convert.ToInt16(form["lgaList"]);
        //        var lgaLists = Convert.ToInt16(form["lgaLists"]);
        //        var lgaListss = Convert.ToInt16(form["lgaListsss"]);

        //        string Nycfileimg = saveImages(StatementOfAccount);

        //        if (Nycfileimg == null || Nycfileimg == "")

        //        {


        //            var imgpath = ("h:\\root\\home\\paelyt-001\\www\\paelyt\\uvlotapublish").Replace("\\", "/");

        //            Nycfileimg = imgpath + nysc.STA_FilePath;

        //        }


        //        string Nycfileimgs = saveImage(NyscIDCard);

        //        if (Nycfileimgs == null || Nycfileimgs == "")

        //        {


        //            var imgpath = ("h:\\root\\home\\paelyt-001\\www\\paelyt\\uvlotapublish").Replace("\\", "/");

        //            Nycfileimgs = imgpath + nysc.NyscIdCardFilePath;

        //        }


        //        string Nycfileimgss = saveImage(NyscIDCard);

        //        if (Nycfileimgss == null || Nycfileimgss == "")

        //        {


        //            var imgpath = ("h:\\root\\home\\paelyt-001\\www\\paelyt\\uvlotapublish").Replace("\\", "/");

        //            Nycfileimgss = imgpath + nysc.NyscIdCardFilePath;

        //        }


        //        string Nycfileimgsss = saveImg(NyscCallUpLetter);

        //        if (Nycfileimgsss == null || Nycfileimgsss == "")

        //        {


        //            var imgpath = ("h:\\root\\home\\paelyt-001\\www\\paelyt\\uvlotapublish").Replace("\\", "/");

        //            Nycfileimgsss = imgpath + nysc.NyscCallUpLetterFilePath;

        //        }




        //        string Nyscfileimg = saveImgs(NyscPassport);

        //        if (Nyscfileimg == null || Nyscfileimg == "")

        //        {


        //            var imgpath = ("h:\\root\\home\\paelyt-001\\www\\paelyt\\uvlotapublish").Replace("\\", "/");

        //            Nyscfileimg = imgpath + nysc.NyscpassportFilePath;

        //        }


        //        string Nyscfileimgs = saveImgss(NyscPostingLetter);

        //        if (Nyscfileimgs == null || Nyscfileimgs == "")

        //        {


        //            var imgpath = ("h:\\root\\home\\paelyt-001\\www\\paelyt\\uvlotapublish").Replace("\\", "/");

        //            Nyscfileimgs = imgpath + nysc.NyscPostingLetterFllePath;

        //        }

        //        string Nyscfileimgss = saveImgsss(NyscProfileDashboard);

        //        if (Nyscfileimgss == null || Nyscfileimgss == "")

        //        {


        //            var imgpath = ("h:\\root\\home\\paelyt-001\\www\\paelyt\\uvlotapublish").Replace("\\", "/");
        //            Nyscfileimgss = imgpath + nysc.NyscProfileDashboardFilePath;

        //        }

        //        string NyscIDCards = saveImage(NyscIDCard);
        //        WebLog.Log("nyscPath Path" + NyscIDCards);
        //        string StatementOfAccounts = saveImages(StatementOfAccount);
        //        WebLog.Log("StatementOfAccounts Path" + StatementOfAccounts);
        //        string NyscPassports = saveImgs(NyscPassport);
        //        WebLog.Log("NyscPassports Path" + NyscPassports);
        //        string NyscPostingLetters = saveImgss(NyscPostingLetter);
        //        WebLog.Log("NyscPostingLetters Path" + NyscPostingLetter);
        //        string NyscCallUpLetters = saveImg(NyscCallUpLetter);
        //        WebLog.Log("NyscCallUpLetters Path" + NyscCallUpLetter);
        //        string NyscProfileDashboards = saveImgsss(NyscProfileDashboard);
        //        WebLog.Log("NyscProfileDashboards Path" + NyscProfileDashboard);





        //        if (nysc.NyscProfileDashboardFilePath == null || nysc.NyscProfileDashboardFilePath == "")
        //        {
        //            TempData["Error"] = "Input Statement Of Accounts";
        //            // NYSCLoanAppForm();
        //            ViewData["nTitles"] = new SelectList(_DR.GetTitles(), "ID", "NAME", lApObj.Title_FK);
        //            ViewData["nMarital"] = new SelectList(_DR.GetMaritalStatus(), "ID", "NAME", lApObj.MaritalStatus_FK);
        //            ViewData["nLGAs"] = new SelectList(_DR.GetAllLGAs(), "ID", "NAME", lApObj.LGA_FK);

        //            ViewData["nAccomodationTypes"] = new SelectList(_DR.GetAccomodationTypes(), "ID", "NAME", lApObj.AccomodationType_FK);
        //            ViewData["nMeansOfIDs"] = new SelectList(_DR.GetMeansOfIdentifications(), "ID", "NAME", lApObj.MeansOfID_FK);
        //            ViewData["nStates"] = new SelectList(_DR.GetNigerianStates(), "ID", "NAME", lApObj.StateofResidence_FK);
        //            ViewData["nBanks"] = new SelectList(_DR.GetBanks(), "FlutterWaveBankCode", "NAME", lApObj.Bank_FK);
        //            ViewData["nRepmtMethods"] = new SelectList(_DR.GetRepaymentMethods(), "ID", "NAME", lApObj.RepaymentMethod_FK);
        //            ViewData["nLoanTenure"] = new SelectList(_DR.GetAllTenure(), "ID", "Tenure", lApObj.LoanTenure);
        //            ViewData["nGender"] = new SelectList(GetAllGender(), "Value", "Text", lApObj.Gender_FK);
        //            ViewData["nemploymentStatus"] = new SelectList(GetAppStatus(), "Value", "Text", lApObj.Contract);
        //            ViewData["nCDDays"] = new SelectList(GetDayOfTheWeek(), "Value", "Text", lApObj.CDSDay);
        //            var channels = _DR.GetMarketChannel();
        //            ViewBag.channel = channels;
        //            return View("NYSCLoanAppForm", lApObj);
        //        }
        //        if (nysc.STA_FilePath == null || nysc.STA_FilePath == "")

        //        {
        //            TempData["Error"] = "Input Nysc Statement Of Account";
        //            // NYSCLoanAppForm();
        //            ViewData["nTitles"] = new SelectList(_DR.GetTitles(), "ID", "NAME", lApObj.Title_FK);
        //            ViewData["nMarital"] = new SelectList(_DR.GetMaritalStatus(), "ID", "NAME", lApObj.MaritalStatus_FK);
        //            ViewData["nLGAs"] = new SelectList(_DR.GetAllLGAs(), "ID", "NAME", lApObj.LGA_FK);

        //            ViewData["nAccomodationTypes"] = new SelectList(_DR.GetAccomodationTypes(), "ID", "NAME", lApObj.AccomodationType_FK);
        //            ViewData["nMeansOfIDs"] = new SelectList(_DR.GetMeansOfIdentifications(), "ID", "NAME", lApObj.MeansOfID_FK);
        //            ViewData["nStates"] = new SelectList(_DR.GetNigerianStates(), "ID", "NAME", lApObj.StateofResidence_FK);
        //            ViewData["nBanks"] = new SelectList(_DR.GetBanks(), "FlutterWaveBankCode", "NAME", lApObj.Bank_FK);
        //            ViewData["nRepmtMethods"] = new SelectList(_DR.GetRepaymentMethods(), "ID", "NAME", lApObj.RepaymentMethod_FK);
        //            ViewData["nLoanTenure"] = new SelectList(_DR.GetAllTenure(), "ID", "Tenure", lApObj.LoanTenure);
        //            ViewData["nGender"] = new SelectList(GetAllGender(), "Value", "Text", lApObj.Gender_FK);
        //            ViewData["nemploymentStatus"] = new SelectList(GetAppStatus(), "Value", "Text", lApObj.Contract);
        //            ViewData["nCDDays"] = new SelectList(GetDayOfTheWeek(), "Value", "Text", lApObj.CDSDay);
        //            var channels = _DR.GetMarketChannel();
        //            ViewBag.channel = channels;
        //            return View("NYSCLoanAppForm", lApObj);
        //        }


        //        if (nysc.NyscIdCardFilePath == null || nysc.NyscIdCardFilePath == "")
        //        {
        //            TempData["Error"] = "Input Nysc ID Cards";
        //            // NYSCLoanAppForm();
        //            ViewData["nTitles"] = new SelectList(_DR.GetTitles(), "ID", "NAME", lApObj.Title_FK);
        //            ViewData["nMarital"] = new SelectList(_DR.GetMaritalStatus(), "ID", "NAME", lApObj.MaritalStatus_FK);
        //            ViewData["nLGAs"] = new SelectList(_DR.GetAllLGAs(), "ID", "NAME", lApObj.LGA_FK);

        //            ViewData["nAccomodationTypes"] = new SelectList(_DR.GetAccomodationTypes(), "ID", "NAME", lApObj.AccomodationType_FK);
        //            ViewData["nMeansOfIDs"] = new SelectList(_DR.GetMeansOfIdentifications(), "ID", "NAME", lApObj.MeansOfID_FK);
        //            ViewData["nStates"] = new SelectList(_DR.GetNigerianStates(), "ID", "NAME", lApObj.StateofResidence_FK);
        //            ViewData["nBanks"] = new SelectList(_DR.GetBanks(), "FlutterWaveBankCode", "NAME", lApObj.Bank_FK);
        //            ViewData["nRepmtMethods"] = new SelectList(_DR.GetRepaymentMethods(), "ID", "NAME", lApObj.RepaymentMethod_FK);
        //            ViewData["nLoanTenure"] = new SelectList(_DR.GetAllTenure(), "ID", "Tenure", lApObj.LoanTenure);
        //            ViewData["nGender"] = new SelectList(GetAllGender(), "Value", "Text", lApObj.Gender_FK);
        //            ViewData["nemploymentStatus"] = new SelectList(GetAppStatus(), "Value", "Text", lApObj.Contract);
        //            ViewData["nCDDays"] = new SelectList(GetDayOfTheWeek(), "Value", "Text", lApObj.CDSDay);
        //            var channels = _DR.GetMarketChannel();
        //            ViewBag.channel = channels;
        //            return View("NYSCLoanAppForm", lApObj);

        //        }


        //        if (nysc.NyscPostingLetterFllePath == null || nysc.NyscPostingLetterFllePath == "")
        //        {
        //            TempData["Error"] = "Input Nysc Posting Letter";
        //            // NYSCLoanAppForm();
        //            ViewData["nTitles"] = new SelectList(_DR.GetTitles(), "ID", "NAME", lApObj.Title_FK);
        //            ViewData["nMarital"] = new SelectList(_DR.GetMaritalStatus(), "ID", "NAME", lApObj.MaritalStatus_FK);
        //            ViewData["nLGAs"] = new SelectList(_DR.GetAllLGAs(), "ID", "NAME", lApObj.LGA_FK);

        //            ViewData["nAccomodationTypes"] = new SelectList(_DR.GetAccomodationTypes(), "ID", "NAME", lApObj.AccomodationType_FK);
        //            ViewData["nMeansOfIDs"] = new SelectList(_DR.GetMeansOfIdentifications(), "ID", "NAME", lApObj.MeansOfID_FK);
        //            ViewData["nStates"] = new SelectList(_DR.GetNigerianStates(), "ID", "NAME", lApObj.StateofResidence_FK);
        //            ViewData["nBanks"] = new SelectList(_DR.GetBanks(), "FlutterWaveBankCode", "NAME", lApObj.Bank_FK);
        //            ViewData["nRepmtMethods"] = new SelectList(_DR.GetRepaymentMethods(), "ID", "NAME", lApObj.RepaymentMethod_FK);
        //            ViewData["nLoanTenure"] = new SelectList(_DR.GetAllTenure(), "ID", "Tenure", lApObj.LoanTenure);
        //            ViewData["nGender"] = new SelectList(GetAllGender(), "Value", "Text", lApObj.Gender_FK);
        //            ViewData["nemploymentStatus"] = new SelectList(GetAppStatus(), "Value", "Text", lApObj.Contract);
        //            ViewData["nCDDays"] = new SelectList(GetDayOfTheWeek(), "Value", "Text", lApObj.CDSDay);
        //            var channels = _DR.GetMarketChannel();
        //            ViewBag.channel = channels;
        //            return View("NYSCLoanAppForm", lApObj);
        //        }

        //        if (nysc.NyscCallUpLetterFilePath == null || nysc.NyscCallUpLetterFilePath == "")
        //        {
        //            TempData["Error"] = "Input Nysc CallUp Letter";
        //            // NYSCLoanAppForm();
        //            ViewData["nTitles"] = new SelectList(_DR.GetTitles(), "ID", "NAME", lApObj.Title_FK);
        //            ViewData["nMarital"] = new SelectList(_DR.GetMaritalStatus(), "ID", "NAME", lApObj.MaritalStatus_FK);
        //            ViewData["nLGAs"] = new SelectList(_DR.GetAllLGAs(), "ID", "NAME", lApObj.LGA_FK);

        //            ViewData["nAccomodationTypes"] = new SelectList(_DR.GetAccomodationTypes(), "ID", "NAME", lApObj.AccomodationType_FK);
        //            ViewData["nMeansOfIDs"] = new SelectList(_DR.GetMeansOfIdentifications(), "ID", "NAME", lApObj.MeansOfID_FK);
        //            ViewData["nStates"] = new SelectList(_DR.GetNigerianStates(), "ID", "NAME", lApObj.StateofResidence_FK);
        //            ViewData["nBanks"] = new SelectList(_DR.GetBanks(), "FlutterWaveBankCode", "NAME", lApObj.Bank_FK);
        //            ViewData["nRepmtMethods"] = new SelectList(_DR.GetRepaymentMethods(), "ID", "NAME", lApObj.RepaymentMethod_FK);
        //            ViewData["nLoanTenure"] = new SelectList(_DR.GetAllTenure(), "ID", "Tenure", lApObj.LoanTenure);
        //            ViewData["nGender"] = new SelectList(GetAllGender(), "Value", "Text", lApObj.Gender_FK);
        //            ViewData["nemploymentStatus"] = new SelectList(GetAppStatus(), "Value", "Text", lApObj.Contract);
        //            ViewData["nCDDays"] = new SelectList(GetDayOfTheWeek(), "Value", "Text", lApObj.CDSDay);
        //            var channels = _DR.GetMarketChannel();
        //            ViewBag.channel = channels;
        //            return View("NYSCLoanAppForm", lApObj);
        //        }

        //        if (nysc.NyscProfileDashboardFilePath == null || nysc.NyscProfileDashboardFilePath == "")
        //        {
        //            TempData["Error"] = "Input Nysc Profile Dashboard screenshot";
        //            // NYSCLoanAppForm();
        //            ViewData["nTitles"] = new SelectList(_DR.GetTitles(), "ID", "NAME", lApObj.Title_FK);
        //            ViewData["nMarital"] = new SelectList(_DR.GetMaritalStatus(), "ID", "NAME", lApObj.MaritalStatus_FK);
        //            ViewData["nLGAs"] = new SelectList(_DR.GetAllLGAs(), "ID", "NAME", lApObj.LGA_FK);

        //            ViewData["nAccomodationTypes"] = new SelectList(_DR.GetAccomodationTypes(), "ID", "NAME", lApObj.AccomodationType_FK);
        //            ViewData["nMeansOfIDs"] = new SelectList(_DR.GetMeansOfIdentifications(), "ID", "NAME", lApObj.MeansOfID_FK);
        //            ViewData["nStates"] = new SelectList(_DR.GetNigerianStates(), "ID", "NAME", lApObj.StateofResidence_FK);
        //            ViewData["nBanks"] = new SelectList(_DR.GetBanks(), "FlutterWaveBankCode", "NAME", lApObj.Bank_FK);
        //            ViewData["nRepmtMethods"] = new SelectList(_DR.GetRepaymentMethods(), "ID", "NAME", lApObj.RepaymentMethod_FK);
        //            ViewData["nLoanTenure"] = new SelectList(_DR.GetAllTenure(), "ID", "Tenure", lApObj.LoanTenure);
        //            ViewData["nGender"] = new SelectList(GetAllGender(), "Value", "Text", lApObj.Gender_FK);
        //            ViewData["nemploymentStatus"] = new SelectList(GetAppStatus(), "Value", "Text", lApObj.Contract);
        //            ViewData["nCDDays"] = new SelectList(GetDayOfTheWeek(), "Value", "Text", lApObj.CDSDay);
        //            var channels = _DR.GetMarketChannel();
        //            ViewBag.channel = channels;
        //            return View("NYSCLoanAppForm", lApObj);
        //        }
        //        var OutstandingAmount = lApObj.ExistingLoan_OutstandingAmount;
        //        if (OutstandingAmount == null)
        //        {
        //            lApObj.ExistingLoan_OutstandingAmount = 0;
        //        }
        //        int Noofmonthsleft = 0;
        //        int.TryParse(lApObj.ExistingLoan_NoOfMonthsLeft, out Noofmonthsleft);

        //        lApObj.ExistingLoan_NoOfMonthsLeft = Noofmonthsleft.ToString();

        //        var Gender_FK = lApObj.Gender_FK;
        //        string DOB = lApObj.DateOfBirth.ToString();

        //        if (lApObj.AccountName == null || lApObj.AccountName == "" || lApObj.AccountName == "Invalid Account Number")
        //        {
        //            TempData["Error"] = "Invalid Account Name";
        //            // NYSCLoanAppForm();
        //            ViewData["nTitles"] = new SelectList(_DR.GetTitles(), "ID", "NAME", lApObj.Title_FK);
        //            ViewData["nMarital"] = new SelectList(_DR.GetMaritalStatus(), "ID", "NAME", lApObj.MaritalStatus_FK);
        //            ViewData["nLGAs"] = new SelectList(_DR.GetAllLGAs(), "ID", "NAME", lApObj.LGA_FK);

        //            ViewData["nAccomodationTypes"] = new SelectList(_DR.GetAccomodationTypes(), "ID", "NAME", lApObj.AccomodationType_FK);
        //            ViewData["nMeansOfIDs"] = new SelectList(_DR.GetMeansOfIdentifications(), "ID", "NAME", lApObj.MeansOfID_FK);
        //            ViewData["nStates"] = new SelectList(_DR.GetNigerianStates(), "ID", "NAME", lApObj.StateofResidence_FK);
        //            ViewData["nBanks"] = new SelectList(_DR.GetBanks(), "FlutterWaveBankCode", "NAME", lApObj.Bank_FK);
        //            ViewData["nRepmtMethods"] = new SelectList(_DR.GetRepaymentMethods(), "ID", "NAME", lApObj.RepaymentMethod_FK);
        //            ViewData["nLoanTenure"] = new SelectList(_DR.GetAllTenure(), "ID", "Tenure", lApObj.LoanTenure);
        //            ViewData["nGender"] = new SelectList(GetAllGender(), "Value", "Text", lApObj.Gender_FK);
        //            ViewData["nemploymentStatus"] = new SelectList(GetAppStatus(), "Value", "Text", lApObj.Contract);
        //            ViewData["nCDDays"] = new SelectList(GetDayOfTheWeek(), "Value", "Text", lApObj.CDSDay);
        //            var channels = _DR.GetMarketChannel();
        //            ViewBag.channel = channels;
        //            return View("NYSCLoanAppForm", lApObj);
        //        }
        //        double repaymentAmt = 0;
        //        double.TryParse(lApObj.RepaymentAmount, out repaymentAmt);

        //        DataAccessA.DataManager.NyscLoanApplication NyscLA = new DataAccessA.DataManager.NyscLoanApplication
        //        {
        //            ID = lApObj.ID,
        //            AccountNumber = lApObj.AccountNumber,
        //            AccountName = lApObj.AccountName,
        //            Firstname = lApObj.Firstname,
        //            Othernames = lApObj.Othernames,
        //            NYSCApplicationStatus_FK = 1,
        //            //NyscIdCardFilePath = nysc.NyscIdCardFilePath,
        //            //STA_FilePath = nysc.STA_FilePath,
        //            //NyscpassportFilePath = nysc.NyscpassportFilePath,
        //            //NyscCallUpLetterFilePath = nysc.NyscCallUpLetterFilePath,
        //            //NyscPostingLetterFllePath = nysc.NyscPostingLetterFllePath,
        //            //NyscProfileDashboardFilePath = nysc.NyscProfileDashboardFilePath,
        //            NyscIdCardFilePath = NyscIDCards,
        //            STA_FilePath = StatementOfAccounts,
        //            NyscpassportFilePath = NyscPassports,
        //            NyscCallUpLetterFilePath = NyscCallUpLetters,
        //            NyscPostingLetterFllePath = NyscPostingLetters,
        //            NyscProfileDashboardFilePath = NyscProfileDashboards,
        //            FacebookName = lApObj.FacebookName,
        //            InstagramHandle = lApObj.InstagramHandle,
        //            TwitterHandle = lApObj.TwitterHandle,
        //            RepaymentAmount = repaymentAmt,
        //            RefNumber = lApObj.LoanRefNumber,
        //            Gender_FK = Gender_FK,//Convert.ToInt32(form["selectGender"]),
        //            MaritalStatus_FK = lApObj.MaritalStatus_FK,//Convert.ToInt16(form["Marital"]),
        //            Surname = lApObj.Surname,
        //            //CreatedBy = Convert.ToString(userid),
        //            DateOfBirth = Convert.ToString(lApObj.DateOfBirth),
        //            Title_FK = lApObj.Title_FK,//Convert.ToInt32(form["Titles"]),
        //            PhoneNumber = lApObj.PhoneNumber,
        //            EmailAddress = lApObj.EmailAddress,
        //            PermanentAddress = lApObj.PermanentAddress,
        //            Landmark = lApObj.Landmark,
        //            ClosestBusStop = lApObj.ClosestBusStop,
        //            LGA_FK = Convert.ToInt16(form["lgaList"]),
        //            TempLGA_FK = Convert.ToInt16(form["lgaLists"]),
        //            NyscLGA_FK = Convert.ToInt16(form["lgaListsss"]),
        //            StateofResidence_FK = lApObj.StateofResidence_FK,////Convert.ToInt32(form["States"]),
        //            TempStateofResidence_FK = lApObj.TempStateofResidence_FK,//Convert.ToInt32(form["States"]),
        //            NyscStateofResidence_FK = lApObj.NyscStateofResidence_FK,//Convert.ToInt32(form["States"]),
        //            TemporaryAddress = lApObj.TemporaryAddress,
        //            OfficialAddress = lApObj.OfficialAddress,
        //            StateCode = lApObj.StateCode,
        //            Employer = lApObj.Employer,
        //            PassOutMonth = lApObj.PassOutMonth,
        //            CDSDay = lApObj.CDSDay,
        //            TempLandmark = lApObj.TempLandmark,
        //            TempClosestBusStop = lApObj.TempClosestBusStop,
        //            ReferralCode = lApObj.ReferalCode,
        //            BVN = lApObj.BVN,
        //            CDSGroup = lApObj.CDSGroup,
        //            NetMonthlyIncome = Convert.ToDouble(lApObj.NetMonthlyIncome),
        //            EMG_EmailAddress = lApObj.EMG_EmailAddress,
        //            EMG_FullName = lApObj.EMG_FullName,
        //            EMG_HomeAddress = lApObj.EMG_HomeAddress,
        //            EMG_PhoneNumber = lApObj.EMG_PhoneNumber,
        //            EMG_Relationship = lApObj.EMG_Relationship,
        //            LoanAmount = Convert.ToDouble(lApObj.LoanAmount),
        //            LoanTenure = LoanTenure,
        //            ExistingLoan = lApObj.ExistingLoan,
        //            LoanComment = lApObj.LoanComment,
        //            ExistingLoan_NoOfMonthsLeft = Convert.ToInt16(lApObj.ExistingLoan_NoOfMonthsLeft),
        //            ExistingLoan_OutstandingAmount = lApObj.ExistingLoan_OutstandingAmount,
        //            BankCode = Helper.GetRemitaBankCodeByFlutterCode(lApObj.BankCode),
        //            IsVisible = 1,
        //            DateCreated = MyUtility.getCurrentLocalDateTime(),
        //            DateModified = MyUtility.getCurrentLocalDateTime(),
        //            //ValueDate = MyUtility.getCurrentLocalDateTime().ToString("yyyy/MM/dd"),
        //            ValueDate = MyUtility.getCurrentLocalDateTime().ToString("dddd, dd MMMM yyyy"),
        //            ValueTime = MyUtility.getCurrentLocalDateTime().ToString("H:mmss"),
        //            MarketingChannel = chanList.ToString(),
        //        };



        //        var id = _DR.UpdateNyscLoanApplications(NyscLA);

        //        if (id == 0)
        //        {
        //            TempData["Error"] = "Please Check Next Of kin Phone Number";
        //            //NYSCLoanAppForm();
        //            ViewData["nTitles"] = new SelectList(_DR.GetTitles(), "ID", "NAME", lApObj.Title_FK);
        //            ViewData["nMarital"] = new SelectList(_DR.GetMaritalStatus(), "ID", "NAME", lApObj.MaritalStatus_FK);
        //            ViewData["nLGAs"] = new SelectList(_DR.GetAllLGAs(), "ID", "NAME", lApObj.LGA_FK);

        //            ViewData["nAccomodationTypes"] = new SelectList(_DR.GetAccomodationTypes(), "ID", "NAME", lApObj.AccomodationType_FK);
        //            ViewData["nMeansOfIDs"] = new SelectList(_DR.GetMeansOfIdentifications(), "ID", "NAME", lApObj.MeansOfID_FK);
        //            ViewData["nStates"] = new SelectList(_DR.GetNigerianStates(), "ID", "NAME", lApObj.StateofResidence_FK);
        //            ViewData["nBanks"] = new SelectList(_DR.GetBanks(), "FlutterWaveBankCode", "NAME", lApObj.Bank_FK);
        //            ViewData["nRepmtMethods"] = new SelectList(_DR.GetRepaymentMethods(), "ID", "NAME", lApObj.RepaymentMethod_FK);
        //            ViewData["nLoanTenure"] = new SelectList(_DR.GetAllTenure(), "ID", "LoanTenure", lApObj.LoanTenure);
        //            ViewData["nGender"] = new SelectList(GetAllGender(), "Value", "Text", lApObj.Gender_FK);
        //            ViewData["nemploymentStatus"] = new SelectList(GetAppStatus(), "Value", "Text", lApObj.Contract);
        //            var channels = _DR.GetMarketChannel();
        //            ViewBag.channel = channels;
        //            return View("NYSCLoanAppFormEdit", NyscLA);
        //        }

        //        if (id != 0)
        //        {

        //            string idv = "";
        //            MarketingChannel Mc = new MarketingChannel();
        //            if (arr.Length > 0)
        //            {
        //                for (var i = 0; i < arr.Length; i++)
        //                {
        //                    string arrc = Convert.ToString(arr[i]);
        //                    insertMarketChannel(arrc, id);
        //                }

        //            }
        //            var email = _DR.getUser(NyscLA.EmailAddress);
        //            string password = "";
        //            string referralCode = "";
        //            if (email == null)
        //            {

        //                referralCode = createUser(NyscLA, out password);
        //                SendReferralEmail(NyscLA, referralCode, password);

        //            }

        //            SendEmail(NyscLA);



        //            return RedirectToAction("Acknowledgements", new { @ID = NyscLA.ID });
        //        }
        //        return View();
        //    }
        //    catch (Exception ex)
        //    {
        //        WebLog.Log(ex.Message.ToString());
        //        return null;
        //    }
        //}








        // Herrrrrrr


        //[HttpPost]
        //public ActionResult NYSCLoanAppFormEdit(FormCollection form, HttpPostedFileBase PostedFile, DataAccessA.Classes.LoanApplication lApObj, HttpPostedFileBase NyscIDCard, HttpPostedFileBase StatementOfAccount, HttpPostedFileBase NyscPassport, HttpPostedFileBase NyscPostingLetter, HttpPostedFileBase NyscCallUpLetter, HttpPostedFileBase NyscProfileDashboard, NyscLoanApplication nysc)
        //{
        //    try
        //    {

        //        var channellist = Request["checkboxName"];

        //        if (channellist == null)
        //        {
        //            TempData["Error"] = "Please Select Marketing Channel";
        //            // NYSCLoanAppForm();
        //            ViewData["nTitles"] = new SelectList(_DR.GetTitles(), "ID", "NAME", lApObj.Title_FK);
        //            ViewData["nMarital"] = new SelectList(_DR.GetMaritalStatus(), "ID", "NAME", lApObj.MaritalStatus_FK);
        //            ViewData["nLGAs"] = new SelectList(_DR.GetAllLGAs(), "ID", "NAME", lApObj.LGA_FK);
        //            ViewData["nAccomodationTypes"] = new SelectList(_DR.GetAccomodationTypes(), "ID", "NAME", lApObj.AccomodationType_FK);
        //            ViewData["nMeansOfIDs"] = new SelectList(_DR.GetMeansOfIdentifications(), "ID", "NAME", lApObj.MeansOfID_FK);
        //            ViewData["nStates"] = new SelectList(_DR.GetNigerianStates(), "ID", "NAME", lApObj.StateofResidence_FK);
        //            ViewData["nBanks"] = new SelectList(_DR.GetBanks(), "FlutterWaveBankCode", "NAME", lApObj.Bank_FK);
        //            ViewData["nRepmtMethods"] = new SelectList(_DR.GetRepaymentMethods(), "ID", "NAME", lApObj.RepaymentMethod_FK);
        //            ViewData["nLoanTenure"] = new SelectList(_DR.GetAllTenure(), "ID", "Tenure", lApObj.LoanTenure);
        //            ViewData["nGender"] = new SelectList(GetAllGender(), "Value", "Text", lApObj.Gender_FK);
        //            ViewData["nemploymentStatus"] = new SelectList(GetAppStatus(), "Value", "Text", lApObj.Contract);
        //            ViewData["nCDDays"] = new SelectList(GetDayOfTheWeek(), "Value", "Text", lApObj.CDSDay);
        //            var channels = _DR.GetMarketChannel();
        //            ViewBag.channel = channels;
        //            return View("NYSCLoanAppFormEdit", lApObj);
        //        }
        //        string[] arr = channellist.Split(',');
        //        var chanList = removestring(arr);

        //        TempData["Error"] = "";
        //        var cPassOutMonth = lApObj.PassOutMonth;
        //        // DateTime cv = Convert.ToDateTime(lApObj.PassOutMonth);
        //        string respMsg = "";
        //        // validatePasout(cPassOutMonth,lApObj.LoanTenure);
        //        calc(cPassOutMonth, lApObj.LoanTenure, out respMsg);
        //        if (respMsg != "0")
        //        {
        //            TempData["Error"] = respMsg;
        //            // NYSCLoanAppForm();
        //            ViewData["nTitles"] = new SelectList(_DR.GetTitles(), "ID", "NAME", lApObj.Title_FK);
        //            ViewData["nMarital"] = new SelectList(_DR.GetMaritalStatus(), "ID", "NAME", lApObj.MaritalStatus_FK);
        //            ViewData["nLGAs"] = new SelectList(_DR.GetAllLGAs(), "ID", "NAME", lApObj.LGA_FK);
        //            ViewData["nAccomodationTypes"] = new SelectList(_DR.GetAccomodationTypes(), "ID", "NAME", lApObj.AccomodationType_FK);
        //            ViewData["nMeansOfIDs"] = new SelectList(_DR.GetMeansOfIdentifications(), "ID", "NAME", lApObj.MeansOfID_FK);
        //            ViewData["nStates"] = new SelectList(_DR.GetNigerianStates(), "ID", "NAME", lApObj.StateofResidence_FK);
        //            ViewData["nBanks"] = new SelectList(_DR.GetBanks(), "FlutterWaveBankCode", "NAME", lApObj.Bank_FK);
        //            ViewData["nRepmtMethods"] = new SelectList(_DR.GetRepaymentMethods(), "ID", "NAME", lApObj.RepaymentMethod_FK);
        //            ViewData["nLoanTenure"] = new SelectList(_DR.GetAllTenure(), "ID", "Tenure", lApObj.LoanTenure);
        //            ViewData["nGender"] = new SelectList(GetAllGender(), "Value", "Text", lApObj.Gender_FK);
        //            ViewData["nemploymentStatus"] = new SelectList(GetAppStatus(), "Value", "Text", lApObj.Contract);
        //            ViewData["nCDDays"] = new SelectList(GetDayOfTheWeek(), "Value", "Text", lApObj.CDSDay);
        //            var channels = _DR.GetMarketChannel();
        //            ViewBag.channel = channels;
        //            return View("NYSCLoanAppFormEdit", lApObj);
        //        }
        //        var dCDSDay = Convert.ToString(form["selectDays"]);
        //        int LoanTenure = Convert.ToInt16(lApObj.LoanTenure);
        //        //  LoanTenure = LoanTenure;
        //        // lApObj.ValueDate = MyUtility.getCurrentLocalDateTime().ToString("dd/MM/yyyy H:mm ss");
        //        //lApObj.RepaymentAmount= _DR.GetRepaymenrAmount(LoanTenure);
        //        // lApObj.LoanTenureStr = LoanTenure.ToString() + " months";
        //        cPassOutMonth = lApObj.PassOutMonth;
        //        dCDSDay = lApObj.CDSDay;
        //        var Marital = lApObj.MaritalStatus_FK;
        //        var Tittle = lApObj.Title_FK;

        //        var lgaList = Convert.ToInt16(form["lgaList"]);
        //        var lgaLists = Convert.ToInt16(form["lgaLists"]);
        //        var lgaListss = Convert.ToInt16(form["lgaListsss"]);


        //        string NyscIDCards = saveImage(NyscIDCard);
        //        WebLog.Log("nyscPath Path" + NyscIDCards);
        //        string StatementOfAccounts = saveImages(StatementOfAccount);
        //        WebLog.Log("StatementOfAccounts Path" + StatementOfAccounts);
        //        string NyscPassports = saveImgs(NyscPassport);
        //        WebLog.Log("NyscPassports Path" + NyscPassports);
        //        string NyscPostingLetters = saveImgss(NyscPostingLetter);
        //        WebLog.Log("NyscPostingLetters Path" + NyscPostingLetter);
        //        string NyscCallUpLetters = saveImg(NyscCallUpLetter);
        //        WebLog.Log("NyscCallUpLetters Path" + NyscCallUpLetter);
        //        string NyscProfileDashboards = saveImgsss(NyscProfileDashboard);
        //        WebLog.Log("NyscProfileDashboards Path" + NyscProfileDashboard);

        //        if (NyscIDCard.ContentLength == 0)
        //        {
        //            TempData["Error"] = "Input Nysc ID Card";
        //            // NYSCLoanAppForm();
        //            ViewData["nTitles"] = new SelectList(_DR.GetTitles(), "ID", "NAME", lApObj.Title_FK);
        //            ViewData["nMarital"] = new SelectList(_DR.GetMaritalStatus(), "ID", "NAME", lApObj.MaritalStatus_FK);
        //            ViewData["nLGAs"] = new SelectList(_DR.GetAllLGAs(), "ID", "NAME", lApObj.LGA_FK);

        //            ViewData["nAccomodationTypes"] = new SelectList(_DR.GetAccomodationTypes(), "ID", "NAME", lApObj.AccomodationType_FK);
        //            ViewData["nMeansOfIDs"] = new SelectList(_DR.GetMeansOfIdentifications(), "ID", "NAME", lApObj.MeansOfID_FK);
        //            ViewData["nStates"] = new SelectList(_DR.GetNigerianStates(), "ID", "NAME", lApObj.StateofResidence_FK);
        //            ViewData["nBanks"] = new SelectList(_DR.GetBanks(), "FlutterWaveBankCode", "NAME", lApObj.Bank_FK);
        //            ViewData["nRepmtMethods"] = new SelectList(_DR.GetRepaymentMethods(), "ID", "NAME", lApObj.RepaymentMethod_FK);
        //            ViewData["nLoanTenure"] = new SelectList(_DR.GetAllTenure(), "ID", "Tenure", lApObj.LoanTenure);
        //            ViewData["nGender"] = new SelectList(GetAllGender(), "Value", "Text", lApObj.Gender_FK);
        //            ViewData["nemploymentStatus"] = new SelectList(GetAppStatus(), "Value", "Text", lApObj.Contract);
        //            ViewData["nCDDays"] = new SelectList(GetDayOfTheWeek(), "Value", "Text", lApObj.CDSDay);
        //            var channels = _DR.GetMarketChannel();
        //            ViewBag.channel = channels;
        //            return View("NYSCLoanAppFormEdit", lApObj);
        //        }

        //        if (StatementOfAccount.ContentLength == 0)
        //        {
        //            TempData["Error"] = "Input Nysc Statement Of Account";
        //            // NYSCLoanAppForm();
        //            ViewData["nTitles"] = new SelectList(_DR.GetTitles(), "ID", "NAME", lApObj.Title_FK);
        //            ViewData["nMarital"] = new SelectList(_DR.GetMaritalStatus(), "ID", "NAME", lApObj.MaritalStatus_FK);
        //            ViewData["nLGAs"] = new SelectList(_DR.GetAllLGAs(), "ID", "NAME", lApObj.LGA_FK);

        //            ViewData["nAccomodationTypes"] = new SelectList(_DR.GetAccomodationTypes(), "ID", "NAME", lApObj.AccomodationType_FK);
        //            ViewData["nMeansOfIDs"] = new SelectList(_DR.GetMeansOfIdentifications(), "ID", "NAME", lApObj.MeansOfID_FK);
        //            ViewData["nStates"] = new SelectList(_DR.GetNigerianStates(), "ID", "NAME", lApObj.StateofResidence_FK);
        //            ViewData["nBanks"] = new SelectList(_DR.GetBanks(), "FlutterWaveBankCode", "NAME", lApObj.Bank_FK);
        //            ViewData["nRepmtMethods"] = new SelectList(_DR.GetRepaymentMethods(), "ID", "NAME", lApObj.RepaymentMethod_FK);
        //            ViewData["nLoanTenure"] = new SelectList(_DR.GetAllTenure(), "ID", "Tenure", lApObj.LoanTenure);
        //            ViewData["nGender"] = new SelectList(GetAllGender(), "Value", "Text", lApObj.Gender_FK);
        //            ViewData["nemploymentStatus"] = new SelectList(GetAppStatus(), "Value", "Text", lApObj.Contract);
        //            ViewData["nCDDays"] = new SelectList(GetDayOfTheWeek(), "Value", "Text", lApObj.CDSDay);
        //            var channels = _DR.GetMarketChannel();
        //            ViewBag.channel = channels;
        //            return View("NYSCLoanAppFormEdit", lApObj);
        //        }

        //        if (NyscPassport.ContentLength == 0)
        //        {
        //            TempData["Error"] = "Input Nysc Passport";
        //            // NYSCLoanAppForm();
        //            ViewData["nTitles"] = new SelectList(_DR.GetTitles(), "ID", "NAME", lApObj.Title_FK);
        //            ViewData["nMarital"] = new SelectList(_DR.GetMaritalStatus(), "ID", "NAME", lApObj.MaritalStatus_FK);
        //            ViewData["nLGAs"] = new SelectList(_DR.GetAllLGAs(), "ID", "NAME", lApObj.LGA_FK);

        //            ViewData["nAccomodationTypes"] = new SelectList(_DR.GetAccomodationTypes(), "ID", "NAME", lApObj.AccomodationType_FK);
        //            ViewData["nMeansOfIDs"] = new SelectList(_DR.GetMeansOfIdentifications(), "ID", "NAME", lApObj.MeansOfID_FK);
        //            ViewData["nStates"] = new SelectList(_DR.GetNigerianStates(), "ID", "NAME", lApObj.StateofResidence_FK);
        //            ViewData["nBanks"] = new SelectList(_DR.GetBanks(), "FlutterWaveBankCode", "NAME", lApObj.Bank_FK);
        //            ViewData["nRepmtMethods"] = new SelectList(_DR.GetRepaymentMethods(), "ID", "NAME", lApObj.RepaymentMethod_FK);
        //            ViewData["nLoanTenure"] = new SelectList(_DR.GetAllTenure(), "ID", "Tenure", lApObj.LoanTenure);
        //            ViewData["nGender"] = new SelectList(GetAllGender(), "Value", "Text", lApObj.Gender_FK);
        //            ViewData["nemploymentStatus"] = new SelectList(GetAppStatus(), "Value", "Text", lApObj.Contract);
        //            ViewData["nCDDays"] = new SelectList(GetDayOfTheWeek(), "Value", "Text", lApObj.CDSDay);
        //            var channels = _DR.GetMarketChannel();
        //            ViewBag.channel = channels;
        //            return View("NYSCLoanAppFormEdit", lApObj);
        //        }

        //        if (NyscPostingLetter.ContentLength == 0)
        //        {
        //            TempData["Error"] = "Input Nysc Posting Letter";
        //            // NYSCLoanAppForm();
        //            ViewData["nTitles"] = new SelectList(_DR.GetTitles(), "ID", "NAME", lApObj.Title_FK);
        //            ViewData["nMarital"] = new SelectList(_DR.GetMaritalStatus(), "ID", "NAME", lApObj.MaritalStatus_FK);
        //            ViewData["nLGAs"] = new SelectList(_DR.GetAllLGAs(), "ID", "NAME", lApObj.LGA_FK);

        //            ViewData["nAccomodationTypes"] = new SelectList(_DR.GetAccomodationTypes(), "ID", "NAME", lApObj.AccomodationType_FK);
        //            ViewData["nMeansOfIDs"] = new SelectList(_DR.GetMeansOfIdentifications(), "ID", "NAME", lApObj.MeansOfID_FK);
        //            ViewData["nStates"] = new SelectList(_DR.GetNigerianStates(), "ID", "NAME", lApObj.StateofResidence_FK);
        //            ViewData["nBanks"] = new SelectList(_DR.GetBanks(), "FlutterWaveBankCode", "NAME", lApObj.Bank_FK);
        //            ViewData["nRepmtMethods"] = new SelectList(_DR.GetRepaymentMethods(), "ID", "NAME", lApObj.RepaymentMethod_FK);
        //            ViewData["nLoanTenure"] = new SelectList(_DR.GetAllTenure(), "ID", "Tenure", lApObj.LoanTenure);
        //            ViewData["nGender"] = new SelectList(GetAllGender(), "Value", "Text", lApObj.Gender_FK);
        //            ViewData["nemploymentStatus"] = new SelectList(GetAppStatus(), "Value", "Text", lApObj.Contract);
        //            ViewData["nCDDays"] = new SelectList(GetDayOfTheWeek(), "Value", "Text", lApObj.CDSDay);
        //            var channels = _DR.GetMarketChannel();
        //            ViewBag.channel = channels;
        //            return View("NYSCLoanAppFormEdit", lApObj);
        //        }

        //        if (NyscCallUpLetter.ContentLength == 0)
        //        {
        //            TempData["Error"] = "Input Nysc CallUp Letter";
        //            // NYSCLoanAppForm();
        //            ViewData["nTitles"] = new SelectList(_DR.GetTitles(), "ID", "NAME", lApObj.Title_FK);
        //            ViewData["nMarital"] = new SelectList(_DR.GetMaritalStatus(), "ID", "NAME", lApObj.MaritalStatus_FK);
        //            ViewData["nLGAs"] = new SelectList(_DR.GetAllLGAs(), "ID", "NAME", lApObj.LGA_FK);

        //            ViewData["nAccomodationTypes"] = new SelectList(_DR.GetAccomodationTypes(), "ID", "NAME", lApObj.AccomodationType_FK);
        //            ViewData["nMeansOfIDs"] = new SelectList(_DR.GetMeansOfIdentifications(), "ID", "NAME", lApObj.MeansOfID_FK);
        //            ViewData["nStates"] = new SelectList(_DR.GetNigerianStates(), "ID", "NAME", lApObj.StateofResidence_FK);
        //            ViewData["nBanks"] = new SelectList(_DR.GetBanks(), "FlutterWaveBankCode", "NAME", lApObj.Bank_FK);
        //            ViewData["nRepmtMethods"] = new SelectList(_DR.GetRepaymentMethods(), "ID", "NAME", lApObj.RepaymentMethod_FK);
        //            ViewData["nLoanTenure"] = new SelectList(_DR.GetAllTenure(), "ID", "Tenure", lApObj.LoanTenure);
        //            ViewData["nGender"] = new SelectList(GetAllGender(), "Value", "Text", lApObj.Gender_FK);
        //            ViewData["nemploymentStatus"] = new SelectList(GetAppStatus(), "Value", "Text", lApObj.Contract);
        //            ViewData["nCDDays"] = new SelectList(GetDayOfTheWeek(), "Value", "Text", lApObj.CDSDay);
        //            var channels = _DR.GetMarketChannel();
        //            ViewBag.channel = channels;
        //            return View("NYSCLoanAppFormEdit", lApObj);
        //        }

        //        if (NyscProfileDashboard.ContentLength == 0)
        //        {
        //            TempData["Error"] = "Input Nysc Profile Dashboard screenshot";
        //            // NYSCLoanAppForm();
        //            ViewData["nTitles"] = new SelectList(_DR.GetTitles(), "ID", "NAME", lApObj.Title_FK);
        //            ViewData["nMarital"] = new SelectList(_DR.GetMaritalStatus(), "ID", "NAME", lApObj.MaritalStatus_FK);
        //            ViewData["nLGAs"] = new SelectList(_DR.GetAllLGAs(), "ID", "NAME", lApObj.LGA_FK);

        //            ViewData["nAccomodationTypes"] = new SelectList(_DR.GetAccomodationTypes(), "ID", "NAME", lApObj.AccomodationType_FK);
        //            ViewData["nMeansOfIDs"] = new SelectList(_DR.GetMeansOfIdentifications(), "ID", "NAME", lApObj.MeansOfID_FK);
        //            ViewData["nStates"] = new SelectList(_DR.GetNigerianStates(), "ID", "NAME", lApObj.StateofResidence_FK);
        //            ViewData["nBanks"] = new SelectList(_DR.GetBanks(), "FlutterWaveBankCode", "NAME", lApObj.Bank_FK);
        //            ViewData["nRepmtMethods"] = new SelectList(_DR.GetRepaymentMethods(), "ID", "NAME", lApObj.RepaymentMethod_FK);
        //            ViewData["nLoanTenure"] = new SelectList(_DR.GetAllTenure(), "ID", "Tenure", lApObj.LoanTenure);
        //            ViewData["nGender"] = new SelectList(GetAllGender(), "Value", "Text", lApObj.Gender_FK);
        //            ViewData["nemploymentStatus"] = new SelectList(GetAppStatus(), "Value", "Text", lApObj.Contract);
        //            ViewData["nCDDays"] = new SelectList(GetDayOfTheWeek(), "Value", "Text", lApObj.CDSDay);
        //            var channels = _DR.GetMarketChannel();
        //            ViewBag.channel = channels;
        //            return View("NYSCLoanAppFormEdit", lApObj);
        //        }
        //        var OutstandingAmount = lApObj.ExistingLoan_OutstandingAmount;
        //        if (OutstandingAmount == null)
        //        {
        //            lApObj.ExistingLoan_OutstandingAmount = 0;
        //        }
        //        int Noofmonthsleft = 0;
        //        int.TryParse(lApObj.ExistingLoan_NoOfMonthsLeft, out Noofmonthsleft);

        //        lApObj.ExistingLoan_NoOfMonthsLeft = Noofmonthsleft.ToString();

        //        var Gender_FK = lApObj.Gender_FK;
        //        string DOB = lApObj.DateOfBirth.ToString();

        //        if (lApObj.AccountName == null || lApObj.AccountName == "" || lApObj.AccountName == "Invalid Account Number")
        //        {
        //            TempData["Error"] = "Invalid Account Name";
        //            // NYSCLoanAppForm();
        //            ViewData["nTitles"] = new SelectList(_DR.GetTitles(), "ID", "NAME", lApObj.Title_FK);
        //            ViewData["nMarital"] = new SelectList(_DR.GetMaritalStatus(), "ID", "NAME", lApObj.MaritalStatus_FK);
        //            ViewData["nLGAs"] = new SelectList(_DR.GetAllLGAs(), "ID", "NAME", lApObj.LGA_FK);

        //            ViewData["nAccomodationTypes"] = new SelectList(_DR.GetAccomodationTypes(), "ID", "NAME", lApObj.AccomodationType_FK);
        //            ViewData["nMeansOfIDs"] = new SelectList(_DR.GetMeansOfIdentifications(), "ID", "NAME", lApObj.MeansOfID_FK);
        //            ViewData["nStates"] = new SelectList(_DR.GetNigerianStates(), "ID", "NAME", lApObj.StateofResidence_FK);
        //            ViewData["nBanks"] = new SelectList(_DR.GetBanks(), "FlutterWaveBankCode", "NAME", lApObj.Bank_FK);
        //            ViewData["nRepmtMethods"] = new SelectList(_DR.GetRepaymentMethods(), "ID", "NAME", lApObj.RepaymentMethod_FK);
        //            ViewData["nLoanTenure"] = new SelectList(_DR.GetAllTenure(), "ID", "Tenure", lApObj.LoanTenure);
        //            ViewData["nGender"] = new SelectList(GetAllGender(), "Value", "Text", lApObj.Gender_FK);
        //            ViewData["nemploymentStatus"] = new SelectList(GetAppStatus(), "Value", "Text", lApObj.Contract);
        //            ViewData["nCDDays"] = new SelectList(GetDayOfTheWeek(), "Value", "Text", lApObj.CDSDay);
        //            var channels = _DR.GetMarketChannel();
        //            ViewBag.channel = channels;
        //            return View("NYSCLoanAppFormEdit", lApObj);
        //        }
        //        double repaymentAmt = 0;
        //        double.TryParse(lApObj.RepaymentAmount, out repaymentAmt);




        //            DataAccessA.DataManager.NyscLoanApplication NyscLA = new DataAccessA.DataManager.NyscLoanApplication
        //            {
        //                AccountNumber = lApObj.AccountNumber,
        //                AccountName = lApObj.AccountName,
        //                Firstname = lApObj.Firstname,
        //                Othernames = lApObj.Othernames,
        //                NYSCApplicationStatus_FK = 1,
        //                NyscIdCardFilePath = NyscIDCards,
        //                STA_FilePath = StatementOfAccounts,
        //                NyscpassportFilePath = NyscPassports,
        //                NyscCallUpLetterFilePath = NyscCallUpLetters,
        //                NyscPostingLetterFllePath = NyscPostingLetters,
        //                NyscProfileDashboardFilePath = NyscProfileDashboards,
        //                FacebookName = lApObj.FacebookName,
        //                InstagramHandle = lApObj.InstagramHandle,
        //                TwitterHandle = lApObj.TwitterHandle,
        //                RepaymentAmount = repaymentAmt,
        //                RefNumber = "NY" + MyUtility.GenerateRefNo(),
        //                Gender_FK = Gender_FK,//Convert.ToInt32(form["selectGender"]),
        //                MaritalStatus_FK = lApObj.MaritalStatus_FK,//Convert.ToInt16(form["Marital"]),
        //                Surname = lApObj.Surname,
        //                //CreatedBy = Convert.ToString(userid),
        //                DateOfBirth = Convert.ToString(lApObj.DateOfBirth),
        //                Title_FK = lApObj.Title_FK,//Convert.ToInt32(form["Titles"]),
        //                PhoneNumber = lApObj.PhoneNumber,
        //                EmailAddress = lApObj.EmailAddress,
        //                PermanentAddress = lApObj.PermanentAddress,
        //                Landmark = lApObj.Landmark,
        //                ClosestBusStop = lApObj.ClosestBusStop,
        //                LGA_FK = Convert.ToInt16(form["lgaList"]),
        //                TempLGA_FK = Convert.ToInt16(form["lgaLists"]),
        //                NyscLGA_FK = Convert.ToInt16(form["lgaListsss"]),
        //                StateofResidence_FK = lApObj.StateofResidence_FK,////Convert.ToInt32(form["States"]),
        //                TempStateofResidence_FK = lApObj.TempStateofResidence_FK,//Convert.ToInt32(form["States"]),
        //                NyscStateofResidence_FK = lApObj.NyscStateofResidence_FK,//Convert.ToInt32(form["States"]),
        //                TemporaryAddress = lApObj.TemporaryAddress,
        //                OfficialAddress = lApObj.OfficialAddress,
        //                StateCode = lApObj.StateCode,
        //                Employer = lApObj.Employer,
        //                PassOutMonth = lApObj.PassOutMonth,
        //                CDSDay = lApObj.CDSDay,
        //                TempLandmark = lApObj.TempLandmark,
        //                TempClosestBusStop = lApObj.TempClosestBusStop,
        //                ReferralCode = lApObj.ReferalCode,
        //                BVN = lApObj.BVN,
        //                CDSGroup = lApObj.CDSGroup,
        //                NetMonthlyIncome = Convert.ToDouble(lApObj.NetMonthlyIncome),
        //                EMG_EmailAddress = lApObj.EMG_EmailAddress,
        //                EMG_FullName = lApObj.EMG_FullName,
        //                EMG_HomeAddress = lApObj.EMG_HomeAddress,
        //                EMG_PhoneNumber = lApObj.EMG_PhoneNumber,
        //                EMG_Relationship = lApObj.EMG_Relationship,
        //                LoanAmount = Convert.ToDouble(lApObj.LoanAmount),
        //                LoanTenure = LoanTenure,
        //                ExistingLoan = lApObj.ExistingLoan,
        //                LoanComment = lApObj.LoanComment,
        //                ExistingLoan_NoOfMonthsLeft = Convert.ToInt16(lApObj.ExistingLoan_NoOfMonthsLeft),
        //                ExistingLoan_OutstandingAmount = lApObj.ExistingLoan_OutstandingAmount,
        //                BankCode = Helper.GetRemitaBankCodeByFlutterCode(lApObj.BankCode),
        //                IsVisible = 1,
        //                DateCreated = MyUtility.getCurrentLocalDateTime(),
        //                DateModified = MyUtility.getCurrentLocalDateTime(),
        //                //ValueDate = MyUtility.getCurrentLocalDateTime().ToString("yyyy/MM/dd"),
        //                ValueDate = MyUtility.getCurrentLocalDateTime().ToString("dddd, dd MMMM yyyy"),
        //                ValueTime = MyUtility.getCurrentLocalDateTime().ToString("H:mmss"),
        //                MarketingChannel = chanList.ToString(),
        //            };


        //            WebLog.Log("nyscPath Path 2" + NyscIDCards);
        //            WebLog.Log("StatementOfAccounts Path 2" + StatementOfAccounts);
        //            WebLog.Log("NyscPassports Path 2" + NyscPassports);
        //            WebLog.Log("NyscProfileDashboards Path 2" + NyscProfileDashboards);
        //            WebLog.Log("NyscCallUpLetters Path 2" + NyscCallUpLetters);
        //            WebLog.Log("NyscPostingLetters Path 2" + NyscPostingLetters);
        //            var id = DataWriter.CreateNYSCLoanApplication(NyscLA);

        //            if (id == 0)
        //            {
        //                TempData["Error"] = "Please Check Next Of kin Phone Number";
        //                //NYSCLoanAppForm();
        //                ViewData["nTitles"] = new SelectList(_DR.GetTitles(), "ID", "NAME", lApObj.Title_FK);
        //                ViewData["nMarital"] = new SelectList(_DR.GetMaritalStatus(), "ID", "NAME", lApObj.MaritalStatus_FK);
        //                ViewData["nLGAs"] = new SelectList(_DR.GetAllLGAs(), "ID", "NAME", lApObj.LGA_FK);

        //                ViewData["nAccomodationTypes"] = new SelectList(_DR.GetAccomodationTypes(), "ID", "NAME", lApObj.AccomodationType_FK);
        //                ViewData["nMeansOfIDs"] = new SelectList(_DR.GetMeansOfIdentifications(), "ID", "NAME", lApObj.MeansOfID_FK);
        //                ViewData["nStates"] = new SelectList(_DR.GetNigerianStates(), "ID", "NAME", lApObj.StateofResidence_FK);
        //                ViewData["nBanks"] = new SelectList(_DR.GetBanks(), "FlutterWaveBankCode", "NAME", lApObj.Bank_FK);
        //                ViewData["nRepmtMethods"] = new SelectList(_DR.GetRepaymentMethods(), "ID", "NAME", lApObj.RepaymentMethod_FK);
        //                ViewData["nLoanTenure"] = new SelectList(_DR.GetAllTenure(), "ID", "LoanTenure", lApObj.LoanTenure);
        //                ViewData["nGender"] = new SelectList(GetAllGender(), "Value", "Text", lApObj.Gender_FK);
        //                ViewData["nemploymentStatus"] = new SelectList(GetAppStatus(), "Value", "Text", lApObj.Contract);
        //                var channels = _DR.GetMarketChannel();
        //                ViewBag.channel = channels;
        //                return View("NYSCLoanAppFormEdit", NyscLA);
        //            }

        //            if (id != 0)
        //            {

        //                string idv = "";
        //                MarketingChannel Mc = new MarketingChannel();
        //                //if (arr.Length == 1)
        //                //{

        //                //    insertMarketChannel(arr[0],id);

        //                //}
        //                //else 
        //                if (arr.Length > 0)
        //                {
        //                    for (var i = 0; i < arr.Length; i++)
        //                    {
        //                        string arrc = Convert.ToString(arr[i]);
        //                        insertMarketChannel(arrc, id);
        //                    }

        //                }

        //                var email = _DR.getUser(NyscLA.EmailAddress);
        //                string password = "";
        //                string referralCode = "";
        //                if (email == null)
        //                {
        //                referralCode = createUser(NyscLA, out password);
        //                SendReferralEmail(NyscLA, referralCode, password);

        //            }

        //            SendEmail(NyscLA);

        //            return RedirectToAction("Acknowledgements", new { @ID = NyscLA.ID });
        //        }


        //        return View();
        //        //return View();
        //    }

        //    catch (Exception ex)
        //    {
        //        WebLog.Log(ex.Message.ToString());
        //        return null;
        //    }
        //}
        

        [HttpGet]
        public ActionResult testDare()
        {

            return View();
        }
       

        public ActionResult Acknowledgements(int ID)
        {
            TempData["SucMsg"] = ""; TempData["ErrMsg"] = "";// TempData["Error"] = "";
            // MyUtility utilities = new MyUtility();
            if (ID == 0 )
            {
                return RedirectToAction("/");
            }
            
            var LoanApps = _DR.LoanDetailss(ID);
            string LoanAmount = LoanApps.LoanAmount.ToString();
            LoanApps.ConvertedAmount = MyUtility.ConvertToCurrency(LoanAmount);
           LoanApps.RepaymentAmount = _DR.GetRepaymenrAmount(LoanApps.LoanTenure);
            LoanApps.RepaymentAmount = MyUtility.ConvertToCurrency(LoanApps.RepaymentAmount);
            TempData["SucMsg"] = "Your Application has been submitted. You can track progress of your loan application on your dashboard, please check your mail for your login details.";
            if (LoanApps == null)
            {
                return RedirectToAction("/");
            }

            return View(LoanApps);
        }




        public ActionResult SaveAcknowledgementss(string Refid)
        {
            // MyUtility utilities = new MyUtility();
            TempData["SucMsg"] = ""; TempData["ErrMsg"] = "";// TempData["Error"] = "";
            if (Refid == null || Refid == "")
            {
                return RedirectToAction("/");
            }
            // var LoanApps = _DR.LoanDetails(Refid);
            //string LoanAmount = LoanApps.LoanAmount.ToString();
            //LoanApps.ConvertedAmount = MyUtility.ConvertToCurrency(LoanAmount);
            //LoanApps.RepaymentAmount = _DR.GetRepaymenrAmount(LoanApps.LoanTenure);
            //LoanApps.RepaymentAmount = MyUtility.ConvertToCurrency(LoanApps.RepaymentAmount);
            TempData["SucMsg"] = "Your Application Uploads were successfully.!";
            //if (LoanApps == null)
            //{
            //    return RedirectToAction("/");
            //}
            return View();
            // return View(LoanApps);
        }

        public string createUser(NyscLoanApplication nyscObj, out string password)
        {
            UvlotAEntities uvDb = new UvlotAEntities();
            User user = new User();
            password = "";
            try
            {
                user.MyReferralCode = DataAccessA.MyUtility.getReferralCode(user.ID.ToString());
                user.EmailAddress = nyscObj.EmailAddress;
                user.Audit = nyscObj.StateCode;
                user.ContactAddress = nyscObj.PermanentAddress;
                user.DateOfBirth = nyscObj.DateOfBirth;
                user.Firstname = nyscObj.Firstname;
                user.PhoneNumber = nyscObj.PhoneNumber;
                user.PaswordVal = MyUtility.GenerateRefNos();
                password = user.PaswordVal;
                user.ReferralCode = nyscObj.ReferralCode;
                var EncrypPassword = new Utilities.CryptographyManager().ComputeHash(user.PaswordVal, HashName.SHA256);
                user.PaswordVal = EncrypPassword;
                uvDb.Users.Add(user);
                uvDb.SaveChanges();
                createUserRole(user, nyscObj);
                //string servicechannels = "";

                //    CreateMarketingDetails(user.ID, out servicechannels);
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message, ex.StackTrace);
            }
            return user.MyReferralCode;
        }

        public void createUserRole(User user, NyscLoanApplication nyscObj)
        {
            try
            {

                UserRole UserRoles = new UserRole();
                UserRoles.User_FK = user.ID;
                UserRoles.Role_FK = Convert.ToInt16(ConfigurationManager.AppSettings["NYSCApplicantRole"]);
                UserRoles.IsVisible = 1;
                _DM.InsertUserRoles(UserRoles);


            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());

            }
        }
        public void SendEmail(NyscLoanApplication nyscObj)
        {
            try
            {
                int loanTenure = Convert.ToInt16(nyscObj.LoanTenure);
                string repayment = _DR.GetRepaymenrAmount(loanTenure);
                string myname = nyscObj.Firstname + " " + nyscObj.Surname;
                string loanAmount = MyUtility.ConvertToCurrency(nyscObj.LoanAmount.ToString());
                string repaymentAmount = MyUtility.ConvertToCurrency(repayment);
                var bodyTxt = System.IO.File.ReadAllText(HostingEnvironment.MapPath("~/EmailNotifications/NYSCWelcomeEmail.html"));
                bodyTxt = bodyTxt.Replace("$ApplicantName", myname).Replace("$refNumber", nyscObj.RefNumber);
                bodyTxt = bodyTxt.Replace("$LoanAmount", loanAmount).Replace("$LoanTenure", nyscObj.LoanTenure.ToString());
                bodyTxt = bodyTxt.Replace("$RepaymentAmt", repaymentAmount);
                var msgHeader = $"CashNowNow NYSC Loan Application Confirmation - " + nyscObj.RefNumber;
                var sendMail = NotificationService.SendMailOut(msgHeader, bodyTxt, nyscObj.EmailAddress, null, null);
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
            }
        }


        public void SendReferralEmail(NyscLoanApplication nyscObj, string referralCode, string passWord)
        {
            try
            {
                string referralLink = ConfigurationManager.AppSettings["ReferralLink"] + referralCode;
                var bodyTxt = System.IO.File.ReadAllText(HostingEnvironment.MapPath("~/EmailNotifications/ReferralWelcomeEmail.html"));

                string myname = nyscObj.Firstname;
                bodyTxt = bodyTxt.Replace("$firstName", myname);
                bodyTxt = bodyTxt.Replace("$UserName", nyscObj.EmailAddress);
                bodyTxt = bodyTxt.Replace("$passwordVal", passWord);
                bodyTxt = bodyTxt.Replace("$RefNumber", nyscObj.RefNumber);
                bodyTxt = bodyTxt.Replace("$refferalCode", referralCode);
                bodyTxt = bodyTxt.Replace("$referralLink", referralLink);
                //bodyTxt = bodyTxt.Replace("$RepaymentAmt", MyUtility.ConvertToCurrency(apObj.RepaymentAmount));
                //WebLog.Log(bodyTxt);
                // WebLog.Log("apObj.EmailAddress: " + apObj.EmailAddress);
                var msgHeader = $"Welcome to CashNowNow Referral Network";
                WebLog.Log("msgHeader " + msgHeader);
                var sendMail = NotificationService.SendMailOut(msgHeader, bodyTxt, nyscObj.EmailAddress, null, null);
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
            }
        }

        public string removestring(string[] channelist)
        {
            try
            {
                string arrc = "";
                List<string> vc = new List<string>();
                char[] mychar = { '?', '1', '2', '3', '4', '5', '6', '7', '8', '9', '0' };
                if (channelist.Length > 0)
                {
                    for (var i = 0; i < channelist.Length; i++)
                    {
                        arrc = Convert.ToString(channelist[i]);
                        arrc = arrc.TrimEnd(mychar);

                        vc.Add(arrc);
                    }

                }
                string ListChannel = string.Join(", ", vc.ToArray());
                return (ListChannel);

            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }
        public int insertMarketChannel(string arrs, int userfk)
        {
            try
            {
                var chan = arrs.Before("?");
                var id = arrs.After("?");
                MarketingDetail MD = new MarketingDetail();
                MD.User_FK = userfk;
                MD.MarketingChannel_FK = Convert.ToInt16(id);
                MD.IsVisible = 1;
                MD.DateCreated = MyUtility.getCurrentLocalDateTime();
                MD.ValueDate = "";
                MD.ValueTime = "";
                var Mgtchannel = _DR.insertMarketChannels(MD);

                return Mgtchannel;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return 0;
            }
        }



        
        public string saveImage(HttpPostedFileBase NyscIDCard)
        {
            try
            {
                string filePath = "";
                if (NyscIDCard != null && NyscIDCard.ContentLength > 0)
                {
                    string filename = Path.GetFileName(NyscIDCard.FileName);
                    //filePath = System.IO.Path.Combine(Server.MapPath("~/Images"), pic);
                    string fileExt = Path.GetExtension(filename);

                    if (fileExt == ".jpg" || fileExt == ".JPG" || fileExt == ".png" || fileExt == ".PNG")
                    {
                        filePath = Path.Combine(Server.MapPath(@"~/Images"), filename);
                        WebLog.Log("file Path" + filePath);
                        NyscIDCard.SaveAs((filePath));
                    }

                }
                return filePath;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }
        

        public string saveImg(HttpPostedFileBase NyscCallUpLetter)
        {
            try
            {
                string filePath = "";
                if (NyscCallUpLetter != null && NyscCallUpLetter.ContentLength > 0)
                {
                    string filename = Path.GetFileName(NyscCallUpLetter.FileName);
                    //filePath = System.IO.Path.Combine(Server.MapPath("~/Images"), pic);
                    string fileExt = Path.GetExtension(filename);

                    if (fileExt == ".jpg" || fileExt == ".JPG" || fileExt == ".png" || fileExt == ".PNG")
                    {
                        filePath = Path.Combine(Server.MapPath(@"~/Images"), filename);
                        WebLog.Log("file Path" + filePath);
                        NyscCallUpLetter.SaveAs((filePath));
                    }

                }
                return filePath;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }



        public string saveImgs(HttpPostedFileBase NyscPassport)
        {
            try
            {
                string filePath = "";
                if (NyscPassport != null && NyscPassport.ContentLength > 0)
                {
                    string filename = Path.GetFileName(NyscPassport.FileName);
                    //filePath = System.IO.Path.Combine(Server.MapPath("~/Images"), pic);
                    string fileExt = Path.GetExtension(filename);

                    if (fileExt == ".jpg" || fileExt == ".JPG" || fileExt == ".png" || fileExt == ".PNG")
                    {
                        filePath = Path.Combine(Server.MapPath(@"~/Images"), filename);
                        WebLog.Log("file Path" + filePath);
                        NyscPassport.SaveAs((filePath));
                    }

                }
                return filePath;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


        public static string calc(string dt1, int Tenure, out string respMsg)
        {
            // var dt = DateTime.Parse(dt1);
            //dt = DateTime.ParseExact(dt1, "yyyy/mm/dd", CultureInfo.InvariantCulture); 
            DateTime dtx = DateTime.Parse(dt1, new CultureInfo("en-CA"));

            var Yearvalue = dtx.Year;
            var Datevalue = dtx.Month;
            var Dayvalue = dtx.Day;
            // var diffs = YearMonthDiff(new DateTime(2019, 10, 1), DateTime.Today);
            var diffs = YearMonthDiff(DateTime.Today, new DateTime(Yearvalue, Datevalue, Dayvalue));

            var tM = diffs.TotalMonths;
            var cM = diffs.Years;
            var dM = diffs.Months;

            if (Tenure > tM)
            {
                respMsg = "Tenure Does Not Match With Passout Date";
                return respMsg;
            }
            else
            {
                respMsg = "0";
                return respMsg;
            }

        }
        public static YearsMonths YearMonthDiff(DateTime startDate, DateTime endDate)
        {
            // Simply subtracting Month won't work as Jan 2014 will be less than Dec 2013.
            // So first multiply the month with year to get absolute month
            int monthDiff = ((endDate.Year * 12) + endDate.Month) - ((startDate.Year * 12) + startDate.Month) + 1;
            int years = (int)Math.Floor((decimal)(monthDiff / 12));
            int months = monthDiff % 12;
            return new YearsMonths
            {
                TotalMonths = monthDiff,
                Years = years,
                Months = months
            };
        }


        public static int calcs(string dt1)
        {

            DateTime dtx = DateTime.Parse(dt1, new CultureInfo("en-CA"));
            var Yearvalue = dtx.Year;
            var Datevalue = dtx.Month;
            var Dayvalue = dtx.Day;
            var diffs = YearMonthDiff(DateTime.Today, new DateTime(Yearvalue, Datevalue, Dayvalue));

            var tM = diffs.TotalMonths;
            var cM = diffs.Years;
            var dM = diffs.Months;

            return dM;

        }

        [HttpPost]
        public ActionResult ValidatePassoutMonth(string Data)
        {
            try
            {
                string respMsg = "";
                var Tenure = Convert.ToInt16(Data.After("&"));
                var PassoutDT = Data.Before("&").ToString();

                respMsg = calc(PassoutDT, Tenure, out respMsg);

                return Json(respMsg);

            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


        [HttpPost]
        public ActionResult GetAmount(int Datas)
        {

            string repayment = "0";
            data dt = new data();
            var Amount = _DR.GetAmountByTenure(Datas, out repayment);

            if (Amount != 0)
            {

                dt.respMSg = true;
                dt.TotalAmounyt = Amount;
                dt.RepaymentAmount = repayment;
            }
            return Json(dt);
        }

        [HttpPost]
        public ActionResult PassOutValidation(string Data)
        {
            try
            {

                var PassoutDT = Data.ToString();

                int MonthDiff = calcs(PassoutDT) - 1;

                var Tenure = _DR.GetTenureByPassoutMonth(MonthDiff);
                // ViewBag.ServicesList = dp.GetTenureByInstCode(code);
                data dt = new data();

                return Json(new { Success = "true", Data = Tenure });

                //return Json(Tenure);

            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        public string saveImages(HttpPostedFileBase StatementOfAccount)
        {
            try
            {
                string filePath = "";
                if (StatementOfAccount != null && StatementOfAccount.ContentLength > 0)
                {
                    string filename = Path.GetFileName(StatementOfAccount.FileName);
                    //filePath = System.IO.Path.Combine(Server.MapPath("~/Images"), pic);
                    string fileExt = Path.GetExtension(filename);

                    if (fileExt == ".jpg" || fileExt == ".JPG" || fileExt == ".png" || fileExt == ".PNG" || fileExt.ToLower().ToString() == ".pdf")
                    {
                        filePath = Path.Combine(Server.MapPath(@"~/Images"), filename);
                        WebLog.Log("file Path" + filePath);
                        StatementOfAccount.SaveAs((filePath));
                    }

                }
                return filePath;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }
        
        public string saveImgss(HttpPostedFileBase NyscPostingLetter)
        {
            try
            {
                string filePath = "";
                if (NyscPostingLetter != null && NyscPostingLetter.ContentLength > 0)
                {
                    string filename = Path.GetFileName(NyscPostingLetter.FileName);
                    //filePath = System.IO.Path.Combine(Server.MapPath("~/Images"), pic);
                    string fileExt = Path.GetExtension(filename);

                    if (fileExt == ".jpg" || fileExt == ".JPG" || fileExt == ".png" || fileExt == ".PNG" || fileExt.ToLower().ToString() == ".pdf")
                    {
                        filePath = Path.Combine(Server.MapPath(@"~/Images"), filename);
                        WebLog.Log("file Path" + filePath);
                        NyscPostingLetter.SaveAs((filePath));
                    }

                }
                return filePath;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }
        
             public string saveImgssss(HttpPostedFileBase LetterOfundertaken)
        {
            try
            {
                string filePath = "";
                if (LetterOfundertaken != null && LetterOfundertaken.ContentLength > 0)
                {
                    string filename = Path.GetFileName(LetterOfundertaken.FileName);
                    //filePath = System.IO.Path.Combine(Server.MapPath("~/Images"), pic);
                    string fileExt = Path.GetExtension(filename);

                    if (fileExt == ".jpg" || fileExt == ".JPG" || fileExt == ".png" || fileExt == ".PNG" || fileExt.ToLower().ToString() == ".pdf")
                    {
                        filePath = Path.Combine(Server.MapPath(@"~/Images"), filename);
                        WebLog.Log("file Path" + filePath);
                        LetterOfundertaken.SaveAs((filePath));
                    }

                }
                return filePath;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }











        public string saveImgsss(HttpPostedFileBase NyscProfileDashboard)
        {
            try
            {
                string filePath = "";
                if (NyscProfileDashboard != null && NyscProfileDashboard.ContentLength > 0)
                {
                    string filename = Path.GetFileName(NyscProfileDashboard.FileName);
                    //filePath = System.IO.Path.Combine(Server.MapPath("~/Images"), pic);
                    string fileExt = Path.GetExtension(filename);

                    if (fileExt == ".jpg" || fileExt == ".JPG" || fileExt == ".png" || fileExt == ".PNG" || fileExt.ToLower().ToString() == ".pdf")
                    {
                        filePath = Path.Combine(Server.MapPath(@"~/Images"), filename);
                        WebLog.Log("file Path" + filePath);
                        NyscProfileDashboard.SaveAs((filePath));
                    }

                }
                return filePath;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }



        [HttpGet]
       
        public ActionResult RecommendLoan()

        {

            try
            {
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = "";// TempData["Error"] = "";
                AppStatFk = 1;
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }
               
                var recObj= _DR.ApproveLoanss(AppStatFk);

                List<AppLoanss> appLoanList = new List<AppLoanss>();
                foreach (AppLoanss app in recObj)
                {
                    app.ConvertedAmount = MyUtility.ConvertToCurrency(app.LoanAmount.ToString());
                    appLoanList.Add(app);
                }
               
                ViewBag.Data = appLoanList;
                GetMenus();
                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }



        [HttpGet]
        public ActionResult Approve(string Refid, TableObjects.LoanApplication LoanApp, DataAccessA.Classes.LoanApplication lApObj, DataAccessA.Classes.AppLoanss  Apploan, AppLoanss LoanApps)

        {

            try
            {
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = ""; TempData["Error"] = "";
                AppStatFk = 1;
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                var User = _DR.getUser(appUser);
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }
                if (Refid == null || Refid == "")
                {
                    return RedirectToAction("RecommendLoan");
                }

                var LoanAppss = _DR.GetNYSCLoanApplication(Refid, AppStatFk);
                if (LoanAppss == null)
                {
                    // return RedirectToAction("RecommendLoanSecondLevel");
                    TempData["Error"] = "Check The Status Of The Application";
                    return RedirectToAction("RecommendLoan");
                }

                GetApplicantInfo(Refid, LoanApp,  lApObj, Apploan, LoanAppss);
                TempData["Username"] = User.Firstname;
                TempData["LoanObj"] = Apploan;
                GetMenus();

                return View(Apploan);
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message, ex.StackTrace);
                return null;
            }
        }

        
        [HttpPost]
        public ActionResult Approve(DataAccessA.Classes.AppLoanss Apploans, FormCollection form, string submit)
        {
            try
            {

                PatnerTransactLog PL = new PatnerTransactLog();
                WebLog.Log("Approve1");
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = ""; TempData["Error"] = "";
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();
                Apploans = (DataAccessA.Classes.AppLoanss)TempData["LoanObj"];
                var appUser = user;
                var User = _DR.getUser(appUser);
                var id = Convert.ToInt16(form["ID"]);
                var approve = Convert.ToInt16(form["Accept"]);
                Apploans.ID = id;
                Apploans.ApplicationStatus_FK = approve;
                WebLog.Log("Approve2");

                if(approve == 6)
                {
                     _DR.UpdateNyscLoanApplication(Apploans);
                    return RedirectToAction("RecommendLoan");
                }

                if (approve == 11)
                {
                    _DR.UpdateNyscLoanApplication(Apploans);
                    return RedirectToAction("RecommendLoan");
                }
                var loanrefID = form["loanrefID"];
                WebLog.Log("loanrefID: " + loanrefID);
                var LoanAppss = _DR.GetNYSCLoanApplication(loanrefID, 1);
                WebLog.Log("Approve3 " + LoanAppss.AccountName);
                LoanAppss.RepaymentAmount = _DR.GetRepaymenrAmount(LoanAppss.LoanTenure);

                WebLog.Log("LoanAppss.RepaymentAmount: " + LoanAppss.RepaymentAmount);
                Apploans = LoanAppss;
                Apploans.ApplicationStatus_FK = approve;
                WebLog.Log("Approve311: " + Apploans.AccountName);
               
                Apploans.CreatedBy = Convert.ToString(User.ID);
                WebLog.Log("Approve311");
                Apploans.comment = Convert.ToString(form["comment"]);
                Apploans.ValueDate = MyUtility.getCurrentLocalDateTime().ToString();
                Apploans.DateCreated = MyUtility.getCurrentLocalDateTime();
                var LoanComment = insertIntoLoanApproval(Apploans);
                int loantenure = Apploans.LoanTenure;
                // SEND REMITA HERE
                string bankCode = Apploans.bankcodes; //_DR.GetBankCodeByName(Apploans.BankCode);
                WebLog.Log("Approve4");
                double remitaAmount = Convert.ToDouble(Apploans.RepaymentAmount) * Apploans.LoanTenure;
                remitaAmount = remitaAmount + Convert.ToDouble(Apploans.RepaymentAmount);
                dynamic obj = new JObject();
                obj.payerName = Apploans.AccountName;
                obj.payerEmail = Apploans.EmailAddress;
                obj.payerPhone = Apploans.PhoneNumber;
                obj.payerBankCode = bankCode;
                obj.refNumber = MyUtility.GenerateRefNo();
                obj.payerAccount = Apploans.AccountNumber;
                obj.amount = remitaAmount.ToString();
                obj.startDate = MyUtility.getCurrentLocalDateTime().AddDays(1).ToString("dd/MM/yyyy");
                obj.endDate = MyUtility.getCurrentLocalDateTime().AddMonths(loantenure).ToString("dd/MM/yyyy");
                obj.maxNoOfDebits = loantenure.ToString();
                string json = obj.ToString();
                string mandateSetupurl = ConfigurationManager.AppSettings["mandateSetupurl"];
                WebLog.Log("Before remita");
                var data = MyUtility.DoRemitaPost(mandateSetupurl, json);
                WebLog.Log("Before remita data: " +data);
                if (data == null)
                {
                    //respObj.respCode = "012";
                    //respObj.respDescription = "Direct debit cannot be created now, try again later.";
                    // ViewBag.Data = "Connectivity error";
                    TempData["Error"] = "Remita standing order failed, please try again";
                    return RedirectToAction("RecommendLoan");
                }

                WebLog.Log("Approve5");
                WebLog.Log("data: " + data);
                dynamic jObj = JObject.Parse(data);
                string myDDstr = "";
                if (jObj.respCode != "00")
                {
                    ViewBag.Data = jObj.respDescription;
                    TempData["Error"] = jObj.respDescription;
                    return RedirectToAction("RecommendLoan");

                }
                if (jObj.respCode == "00")
                {
                    myDDstr = jObj.mandateform;
                    var resp = _DR.UpdateNyscLoanApplication(Apploans);
                    SendEmail(Apploans, myDDstr);
                    WebLog.Log("After Email");
                    insertRemita(jObj, Apploans);
                }
               
                myDDstr = jObj.mandateform.ToString();

                //SEND EMAIL HERE
               // SendEmail(Apploans, myDDstr);
               // insertRemita(jObj, Apploans);
                //END


            }
            catch (Exception ex)
            {
                WebLog.Log("exception");
                WebLog.Log(ex.Message, ex.StackTrace);
            }
            return RedirectToAction("RecommendLoan");
        }

        public string insertRemita(dynamic MyDDStr , dynamic Apploans)
        {
            try
            {
                PatnerTransactLog PL = new PatnerTransactLog();
                //DataAccessA.Classes.AppLoanss Applons = new DataAccessA.Classes.AppLoanss Applons();
                //dynamic jObj = JObject.Parse(MyDDStr);
                PL.PatnerUrl = MyDDStr?.mandateform;
                PL.RefNum = Apploans.LoanRefNumber;
                PL.PatnerCode = "REMITA";
                PL.PatnerResponse = Convert.ToString(MyDDStr);
                PL.DateCreated = MyUtility.getCurrentLocalDateTime();
                PL.BankName = Apploans.AccountName;
                PL.BankAcct = Apploans.AccountNumber;
                PL.PatnerReference = MyDDStr?.requestID;

                int resp =_DM.insertRemita(PL); 
                return PL.RefNum ;
            } 
            catch(Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }
        public void SendEmail(AppLoanss apObj, string remitalink)
        {
            try
            {

                var bodyTxt = System.IO.File.ReadAllText(HostingEnvironment.MapPath("~/EmailNotifications/remitaformEmail.html"));
                
              
                string myname = apObj.Surname + " " + apObj.Firstname;
                bodyTxt = bodyTxt.Replace("$ApplicantName", myname);
                bodyTxt = bodyTxt.Replace("$remitalink", remitalink);
                bodyTxt = bodyTxt.Replace("$LoanAmount", MyUtility.ConvertToCurrency(apObj.LoanAmount.ToString()));
                bodyTxt = bodyTxt.Replace("$LoanTenure", apObj.LoanTenure.ToString());
                bodyTxt = bodyTxt.Replace("$RepaymentAmt", MyUtility.ConvertToCurrency(apObj.RepaymentAmount));
                WebLog.Log(bodyTxt);
                WebLog.Log("apObj.EmailAddress: " + apObj.EmailAddress);
                var msgHeader = $"Urgent: CashNowNow NYSC Loan Update - " + apObj.LoanRefNumber;
                WebLog.Log("msgHeader " + msgHeader);
                var sendMail = NotificationService.SendMailOuts(msgHeader, bodyTxt, apObj.EmailAddress, null, null);
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
            }
        }


        [HttpGet]
        public ActionResult sendrem()
        {
            try
            {
                SendRem();
                return View();
            }
            catch(Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        public void SendRem()
        {
            try
            {

                var bodyTxt = System.IO.File.ReadAllText(HostingEnvironment.MapPath("~/EmailNotifications/remitaformEmails.html"));
                var email = "froshmalam@gmail.com";
                //string myname = apObj.Surname + " " + apObj.Firstname;
                //bodyTxt = bodyTxt.Replace("$ApplicantName", myname);
                //bodyTxt = bodyTxt.Replace("$remitalink", remitalink);
                //bodyTxt = bodyTxt.Replace("$LoanAmount", MyUtility.ConvertToCurrency(apObj.LoanAmount.ToString()));
                //bodyTxt = bodyTxt.Replace("$LoanTenure", apObj.LoanTenure.ToString());
                //bodyTxt = bodyTxt.Replace("$RepaymentAmt", MyUtility.ConvertToCurrency(apObj.RepaymentAmount));
                //WebLog.Log(bodyTxt);
                //WebLog.Log("apObj.EmailAddress: " + apObj.EmailAddress);
                var msgHeader = $"Urgent: CashNowNow NYSC Loan Update - " + "NY15840395423903";
                WebLog.Log("msgHeader " + msgHeader);
                var sendMail = NotificationService.SendMailOut(msgHeader, bodyTxt, email, null, null);
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
            }
        }


        [HttpGet]
        public ActionResult SecondLevelApproval()
        {
            try
            {
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = ""; TempData["Error"] = "";
                AppStatFk = 2;
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }
                


                var lstMenus = _DR.ApproveLoans(AppStatFk);
                List<ExcelList> appLoanList = new List<ExcelList>();
                foreach (ExcelList app in lstMenus)
                {
                    app.ConvertedAmount = MyUtility.ConvertToCurrency(app.LoanAmount.ToString());
                    appLoanList.Add(app);
                }
                ViewBag.Data = appLoanList;

                Session["AllTransaction"] = ViewBag.Data;

                GetMenus();


                return View();
            }

            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }

        }



public ActionResult SecondApproval(string Refid, TableObjects.LoanApplication LoanApp, DataAccessA.Classes.LoanApplication lApObj, AppLoanss Apploan, AppLoanss LoanApps)
        {
            try
            {
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = ""; TempData["Error"] = "";
                AppStatFk = 2;
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                var User = _DR.getUser(appUser);
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }
                if (Refid == null || Refid == "")
                {
                    return RedirectToAction("SecondLevelApproval");
                }

                var LoanAppss = _DR.GetNYSCLoanApplication(Refid, AppStatFk);
                var comment = GetComment(User.ID, LoanAppss.ID);
                ViewBag.Data = comment;
                if (comment == null)
                {
                    Apploan.LoanComment = "none";
                }
                else
                {
                    for (var i = 0; i <= comment.Count; i++)
                    {
                        if (i == 0)
                        {

                            Apploan.LoanComment = comment[0].comment;
                            TempData["Username"] = comment[0].commentBy;
                            TempData["ApprovalDate"] = comment[0].ValueDate;
                        }
                        else
                        {
                            Apploan.LoanComment = comment[0].comment;
                            TempData["Username"] = comment[0].commentBy;
                            TempData["ApprovalDate"] = comment[0].ValueDate;
                        }


                    }


                    //if (i == 0)
                    //{
                    //    ViewBag.FirstCommentBy = LoanComments[i].Firstname;
                    //    var fuser = _DR.getUserByID(ViewBag.FirstCommentBy);
                    //    ViewBag.FirstCommentBy = fuser.Firstname;
                    //    ViewBag.FirstLoanComment = LoanComments[i].LoanComment;
                    //}
                    //if (i == 1)
                    //{
                    //    ViewBag.SecondCommentBy = LoanComments[i].Firstname;
                    //    var suser = _DR.getUserByID(ViewBag.SecondCommentBy);
                    //    ViewBag.SecondCommentBy = suser.Firstname;

                    //    ViewBag.SecondLoanComment = LoanComments[i].LoanComment;
                    //}
                    //  Apploan.LoanComment = string.IsNullOrEmpty(comment[0]) ? "none" : comment[0];
                }
                if (LoanAppss == null)
                {
                    // return RedirectToAction("RecommendLoanSecondLevel");
                    TempData["Error"] = "Check The Status Of The Application";
                    return RedirectToAction("SecondLevelApproval");
                }

                GetApplicantInfo(Refid, LoanApp,  lApObj, Apploan, LoanAppss);
                TempData["Username"] = User.Firstname;
                TempData["LoanObj"] = Apploan;
                GetMenus();

                return View(Apploan);
            }

            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        public List<DataReader.getcomment> GetComment(int userfk, int loanNumfk)
        {
            try
            {
                var comment = _DR.getCommentByUser(userfk, loanNumfk);

                return comment;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        [HttpPost]
        public ActionResult SecondApproval(DataAccessA.Classes.AppLoanss Apploans, FormCollection form)
        {
            TempData["SucMsg"] = ""; TempData["ErrMsg"] = ""; TempData["Error"] = "";
            var LoggedInuser = new LogginHelper();
            user = LoggedInuser.LoggedInUser();

            var appUser = user;
            var User = _DR.getUser(appUser);
            var id = Convert.ToInt16(form["ID"]);
            var approve = Convert.ToInt16(form["Accept"]);
            Apploans.ID = id;
            Apploans.ApplicationStatus_FK = approve;
            if (approve == 7)
            {
                _DR.UpdateNyscLoanApplication(Apploans);
                return RedirectToAction("SecondLevelApproval");
            }
            Apploans.ApplicationStatus_FK = approve;
            var resp = _DR.UpdateNyscLoanApplication(Apploans);
            Apploans.CreatedBy = Convert.ToString(User.ID);
            Apploans.comment = Convert.ToString(form["comment"]);
            Apploans.ValueDate = MyUtility.getCurrentLocalDateTime().ToString();
            Apploans.DateCreated = MyUtility.getCurrentLocalDateTime();
            var LoanComment = insertIntoLoanApproval(Apploans);
            return RedirectToAction("SecondLevelApproval");

        }

        [HttpGet]
        public ActionResult thirdLevelApproval()
        {
            try
            {
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = ""; TempData["Error"] = "";
                AppStatFk = 3;
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }

                var lstMenus = _DR.ApproveLoans(AppStatFk);
                List<ExcelList> appLoanList = new List<ExcelList>();
                foreach (ExcelList app in lstMenus)
                {
                    app.ConvertedAmount = MyUtility.ConvertToCurrency(app.LoanAmount.ToString());
                    appLoanList.Add(app);
                }
                ViewBag.Data = appLoanList;



                //var lstMenus = _DR.ApproveLoans(AppStatFk);
                //ViewBag.Data = lstMenus;

                Session["AllTransaction"] = ViewBag.Data;

                GetMenus();

                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }

        }

        [HttpGet]
        public ActionResult ThirdApproval(string Refid, TableObjects.LoanApplication LoanApp, DataAccessA.Classes.LoanApplication lApObj, AppLoanss Apploan, AppLoanss LoanApps)
        {
            try
            {
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = ""; TempData["Error"] = "";
                AppStatFk = 3;
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                var User = _DR.getUser(appUser);
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }
                if (Refid == null || Refid == "")
                {
                    return RedirectToAction("ThirdApproval");
                }

                var LoanAppss = _DR.GetNYSCLoanApplication(Refid, AppStatFk);
                if (LoanAppss == null)
                {

                    TempData["Error"] = "Check The Status Of The Application";
                    return RedirectToAction("ThirdApproval");
                }
                var comment = GetComment(User.ID, LoanAppss.ID);
                ViewBag.Data = comment;
                if (comment == null)
                {
                    Apploan.LoanComment = "none";
                }
                else
                {
                    for (var i = 0; i <= comment.Count; i++)
                    {
                        if (i == 2)
                        {
                            Apploan.LoanComment = comment[0].comment;
                            TempData["Username"] = comment[0].commentBy;
                            TempData["ApprovalDate"] = comment[0].ValueDate;
                            Apploan.comment = comment[1].comment;
                            TempData["Usernames"] = comment[1].commentBy;
                            TempData["ApprovalDates"] = comment[1].ValueDate;
                        }
                        else
                        {
                            if (i == 0)
                            {
                                Apploan.LoanComment = comment[0].comment;
                                TempData["Username"] = comment[0].commentBy;
                                TempData["ApprovalDate"] = comment[0].ValueDate;
                                Apploan.comment = "No Comment";
                                TempData["Usernames"] = "none";
                                TempData["ApprovalDates"] = "none";
                            }
                            /*  Apploan.LoanComment = comment[0].comment;
                              TempData["Username"] = comment[0].commentBy;
                              Apploan.comment = comment[1].comment;
                              TempData["Usernames"] = comment[1].comment;*/
                        }


                    }
                    //  Apploan.LoanComment = string.IsNullOrEmpty(comment[0]) ? "none" : comment[0];
                }


                GetApplicantInfo(Refid, LoanApp, lApObj, Apploan, LoanAppss);
                // TempData["Username"] = User.Firstname;
                TempData["LoanObj"] = Apploan;
                GetMenus();

                return View(Apploan);
            }

            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        [HttpPost]
        public ActionResult ThirdApproval(DataAccessA.Classes.AppLoanss Apploans, FormCollection form)
        {
            TempData["SucMsg"] = ""; TempData["ErrMsg"] = ""; TempData["Error"] = "";
            var LoggedInuser = new LogginHelper();
            user = LoggedInuser.LoggedInUser();

            var appUser = user;
            var User = _DR.getUser(appUser);
            var id = Convert.ToInt16(form["ID"]);
            var approve = Convert.ToInt16(form["Accept"]);
            Apploans.ID = id;
            Apploans.ApplicationStatus_FK = approve;

            if (approve == 8)
            {
                _DR.UpdateNyscLoanApplication(Apploans);
                return RedirectToAction("thirdLevelApproval");
            }
            var refNum = _DR.UpdateNyscLoanApplication(Apploans);
            Apploans.CreatedBy = Convert.ToString(User.ID);
            Apploans.comment = Convert.ToString(form["comment"]);
            Apploans.ValueDate = MyUtility.getCurrentLocalDateTime().ToString();
            Apploans.DateCreated = MyUtility.getCurrentLocalDateTime();
            var LoanComment = insertIntoLoanApproval(Apploans);
            //NYSCReferralLedger RL = new NYSCReferralLedger();
            //RL.ReferralCode = User.ReferralCode;
            //RL.ReferenceNumber = refNum;
            //insertIntoReferealLedger(RL);
            //insertIntoloanLedger(NLL);
            // var refcode = _DR.UpdateReferralLedger(LoanApp);
            return RedirectToAction("thirdLevelApproval");

        }

      
        //public int insertIntoReferealLedger(NYSCReferralLedger RL)
        //{
        //    try
        //    {
        //        var Nam = _DR.insertReferalLedger(RL);

        //        return Nam;
        //    }
        //    catch (Exception ex)
        //    {
        //        WebLog.Log(ex.Message.ToString());
        //        return 0;
        //    }
        //}

        //public int insertIntoloanLedger(NYSCLoanLedger NLL)
        //{
        //    try
        //    {
        //        var Nam = _DR.insertIntoloanLedger(NLL);

        //        return Nam;
        //    }
        //    catch (Exception ex)
        //    {
        //        WebLog.Log(ex.Message.ToString());
        //        return 0;
        //    }
        //}
        [HttpGet]
        public ActionResult AllApprovedThirdLevel()

        {

            try
            {
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = ""; TempData["Error"] = "";
                AppStatFk = 4;
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }


                var lstMenus = _DR.FourthApproval(AppStatFk);
                ViewBag.Data = lstMenus;

                // Session["AllTransaction"] = ViewBag.Data;




                GetMenus();
                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        [HttpGet]
        public ActionResult ApprovedThirdLoan(string Refid, TableObjects.LoanApplication LoanApp, DataAccessA.Classes.LoanApplication lApObj, AppLoanss Apploan, AppLoanss LoanApps)

        {

            try
            {
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = ""; TempData["Error"] = "";
                AppStatFk = 3;
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                var User = _DR.getUser(appUser);
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }
                if (Refid == null || Refid == "")
                {
                    return RedirectToAction("ApprovedThirdLoan");
                }

                var LoanAppss = _DR.GetNYSCLoanApplication(Refid, AppStatFk);
                if (LoanAppss == null)
                {
                    // return RedirectToAction("RecommendLoanSecondLevel");
                    TempData["Error"] = "Check The Status Of The Application";
                    return RedirectToAction("ApprovedThirdLoan");
                }

                GetApplicantInfo(Refid, LoanApp, lApObj, Apploan, LoanAppss);
                TempData["Username"] = User.Firstname;
                TempData["LoanObj"] = Apploan;
                GetMenus();

                return View(Apploan);
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message, ex.StackTrace);
                return null;
            }
        }
        [HttpPost]
        public ActionResult ApprovedThirdLoan(DataAccessA.Classes.AppLoanss Apploans, FormCollection form)
        {
            TempData["SucMsg"] = ""; TempData["ErrMsg"] = ""; TempData["Error"] = "";
            var id = Convert.ToInt16(form["ID"]);
            var approve = Convert.ToInt16(form["Accept"]);
            Apploans.ID = id;
            Apploans.ApplicationStatus_FK = approve;
            var resp = _DR.UpdateNyscLoanApplication(Apploans);


            ViewBag.Data = resp;
            Session["AllTransaction"] = ViewBag.Data;
            return RedirectToAction("ApprovedThirdLoan");

        }
        [HttpPost]
        public ActionResult DisburseToList(string ItemList,FormCollection form)
        {
            try
            {
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                var User = _DR.getUser(appUser);
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }
                var ReferenceList = Request["Check"];
                string[] arr = ItemList.Split(',');
                TableObjects.LoanApplication LoanAp = new TableObjects.LoanApplication();
                DataAccessA.Classes.AppLoanss Apploans = new DataAccessA.Classes.AppLoanss();
                LoanAp.ApplicationStatus_FK = 6;
                var appstafk = 4;
                if(arr.Length > 0)
                {
                    for(var i = 0; i<arr.Length; i++)
                    {
                        
                        arr[i] = arr[i].Before("+");
                        Apploans = _DR.GetNYSCLoanApplication(arr[i], appstafk);
                       var resp = DisburseCash(Apploans);

                        if(resp == null)
                        {
                            TempData["ErrMsg"] = "Disbursement Failed! Please Try Again";
                            return RedirectToAction("FinalApproval");
                        }
                        if (resp != null)
                        {
                            //Inserting Disbursement
                            insertintoTransactLog(resp, Apploans.LoanRefNumber);

                            //Ends Here
                            dynamic result = JObject.Parse(resp);
                            if (result?.status == "true")
                            {

                                var refNum = _DR.UpdateNyscLoanApplication(Apploans);
                                Apploans.CreatedBy = Convert.ToString(User.ID);
                                Apploans.comment = Convert.ToString(form["comment"]);
                                Apploans.ValueDate = MyUtility.getCurrentLocalDateTime().ToString();
                                Apploans.DateCreated = MyUtility.getCurrentLocalDateTime();
                                var LoanComment = insertIntoLoanApproval(Apploans);

                                insertRepayment(Apploans);
                                int stageFlag = 1; //StageFlag ==1 
                               
                                insertIntoNYSCReferralLedger(Apploans, stageFlag);
                                return RedirectToAction("FinalApproval");
                            }
                            else if (result?.status == "false")
                            {
                                TempData["ErrMsg"] = result?.message;
                                // return View();
                                //return RedirectToAction("FourthApproval", new { @Refid = Apploans.LoanRefNumber });
                                return RedirectToAction("FinalApproval");
                            }
                        }
                    }
                }
                
               
                return View();
            }
            catch(Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

       // Final Approval Here
        [HttpGet]
        public ActionResult FinalApproval()
        {
            try
            {
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = ""; TempData["Error"] = "";
                AppStatFk = 4;
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }
                var patnerCode = "REMITA";
               // var lstMenus = _DR.ApproveLoans(AppStatFk);
                var lstMenus = _DR.ApproveLoansRemFlu(AppStatFk,patnerCode); 
                List<ExcelList> appLoanList = new List<ExcelList>();
                foreach (ExcelList app in lstMenus)
                {
                    app.ConvertedAmount = MyUtility.ConvertToCurrency(app.LoanAmount.ToString());
                    appLoanList.Add(app);
                }
                ViewBag.Data = appLoanList;



                //var lstMenus = _DR.ApproveLoans(AppStatFk);
                //ViewBag.Data = lstMenus;

                Session["AllTransaction"] = ViewBag.Data;

                GetMenus();

                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }

        }




        [HttpGet]
        public ActionResult FourthApproval(string Refid, TableObjects.LoanApplication LoanApp, DataAccessA.Classes.LoanApplication lApObj, AppLoanss Apploan, AppLoanss LoanApps)
        {
            try
            {
                TempData["SucMsg"] = "";TempData["Error"] = "";
                //TempData["ErrMsg"] = "";
                AppStatFk = 4;
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                var User = _DR.getUser(appUser);
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }
                if (Refid == null || Refid == "")
                {
                    return RedirectToAction("FourthApproval");
                }

                var LoanAppss = _DR.GetNYSCLoanApplication(Refid, AppStatFk);
                if (LoanAppss == null)
                {

                    TempData["Error"] = "Check The Status Of The Application";
                    return RedirectToAction("FourthApproval");
                }
                var comment = GetComment(User.ID, LoanAppss.ID);
                ViewBag.Data = comment;
                if (comment == null)
                {
                    Apploan.LoanComment = "none";
                }
                


                GetApplicantInfo(Refid, LoanApp,lApObj, Apploan, LoanAppss);
                // TempData["Username"] = User.Firstname;
                TempData["LoanObj"] = Apploan;
                GetMenus();

                return View(Apploan);
            }

            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }
        


        
        [HttpPost]
        public ActionResult FourthApproval(DataAccessA.Classes.AppLoanss Apploans, FormCollection form)
        {
            try
            {
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = ""; TempData["Error"] = "";
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                Apploans = (DataAccessA.Classes.AppLoanss)TempData["LoanObj"];
                int disburseFlag = Helper.ValidateDisbursement(Apploans.LoanRefNumber);

                var User = _DR.getUser(appUser);
                var id = Convert.ToInt16(form["ID"]);
                var approve = Convert.ToInt16(form["Accept"]);
                Apploans.ID = id;
                Apploans.ApplicationStatus_FK = approve;
                if (approve == 10)
                {
                    _DR.UpdateNyscLoanApplication(Apploans);
                    return RedirectToAction("FinalApproval");
                }
                var check = ConfigurationManager.AppSettings["RemitaFlag"];
                
                check = disburseFlag == 0 ? check : "110";
                if (check == "1")
                {
                    string resp = "";

                    resp =DisburseCash(Apploans);
                    WebLog.Log("DisburseCash: " + resp);
                    // Sendmail
                    var email = _DR.getUser(Apploans.EmailAddress);
                    if (email != null)
                    {
                        SendDisbursementEmail(Apploans);
                    }
                       
                    if (resp == null)
                    {
                        TempData["ErrMsg"] = "Disbursement Failed! Please Try Again";
                        return RedirectToAction("FourthApproval", new { @Refid = Apploans.LoanRefNumber });
                    }
                    if (resp != null)
                    {
                        //Inserting Disbursement
                        insertintoTransactLog(resp, Apploans.LoanRefNumber);

                        //Ends Here
                        dynamic result = JObject.Parse(resp);
                        if (result?.status == "true")
                        {
                           
                            var refNum = _DR.UpdateNyscLoanApplication(Apploans);
                            Apploans.CreatedBy = Convert.ToString(User.ID);
                            Apploans.comment = Convert.ToString(form["comment"]);
                            Apploans.ValueDate = MyUtility.getCurrentLocalDateTime().ToString();
                            Apploans.DateCreated = MyUtility.getCurrentLocalDateTime();
                            var LoanComment = insertIntoLoanApproval(Apploans);
                            
                            insertRepayment(Apploans);
                            int stageFlag = 1; //StageFlag ==1 
                            insertIntoNYSCReferralLedger(Apploans, stageFlag);
                            return RedirectToAction("FinalApproval");
                        }
                        else if (result?.status == "false")
                        {
                            TempData["ErrMsg"] = result?.message;
                            // return View();
                            return RedirectToAction("FourthApproval", new { @Refid = Apploans.LoanRefNumber });
                        }
                    }
                }
                else if (check == "0")
                {
                    TempData["ErrMsg"] = "Disbursement ";
                    return RedirectToAction("FourthApproval", new { @Refid = Apploans.LoanRefNumber });
                }
                else if (check == "110")
                {
                    TempData["ErrMsg"] = "Transaction has already been approved ";
                    return RedirectToAction("FourthApproval", new { @Refid = Apploans.LoanRefNumber });
                }


                return View();
            }
            catch(Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
            }


        public int insertIntoNYSCReferralLedger(DataAccessA.Classes.AppLoanss Apploans, int stageflag)
        {
            try
            {
                string amount = stageflag == 1 ? ConfigurationManager.AppSettings["ReferralFirstAmount"] : ConfigurationManager.AppSettings["ReferralSecondAmount"];

                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                var User = _DR.getUser(appUser);

                NYSCReferralLedger RL = new NYSCReferralLedger();
               
                  
                RL.TrnDate = MyUtility.getCurrentLocalDateTime();
                RL.ReferenceNumber = Apploans.LoanRefNumber;
                RL.User_FK = User.ID;
                RL.IsVisible = 1;
                RL.Debit = 0;
                RL.Credit = Convert.ToDouble(amount); ;
                RL.ValueDate = MyUtility.getCurrentLocalDateTime().ToString("yyyy/MM/dd");
                RL.ValueTime = MyUtility.getCurrentLocalDateTime().ToString("H:mmss");
                RL.ReferralCode = User.ReferralCode;
                
               
                var payLoan = _DR.insertIntoNYSCReferralLedger(RL); //TODO....Check 4 first Level Refrral and compensate accordingly
                
                return payLoan;
            }
            
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return 0;
            }
        }

        public int insertRepayment( DataAccessA.Classes.AppLoanss Apploans)
        {
            try
            {
               
                NYSCLoanLedger NL = new NYSCLoanLedger();
                
                NL.TrnDate = MyUtility.getCurrentLocalDateTime();
                NL.ReferenceNumber = Apploans.LoanRefNumber;
                NL.LoanApplication_FK = Apploans.ID;
                NL.IsVisible = 1;
                NL.Debit =Apploans.LoanAmount;
                NL.Credit = 0;
                NL.PaymentFlag = 1;
                NL.ValueDate = MyUtility.getCurrentLocalDateTime().ToString("yyyy/MM/dd");
                NL.ValueTime = MyUtility.getCurrentLocalDateTime().ToString("H:mmss");
                NL.Audit = "";
                NL.DueDate = MyUtility.getCurrentLocalDateTime();
                var payLoan = _DR.inserRepaymentAmount(NL);
                DateTime locatime = MyUtility.getCurrentLocalDateTime(); 
                if (payLoan > 0)
                {
                    string Repayment = _DR.GetRepaymenrAmount(Apploans.LoanTenure);
                    for (var i = 1; i <= Apploans.LoanTenure ; i++)
                    {
                        
                        NL.TrnDate =MyUtility.getCurrentLocalDateTime();
                        NL.ReferenceNumber = Apploans.LoanRefNumber;
                        NL.LoanApplication_FK = Apploans.ID;
                        NL.IsVisible = 1;
                        NL.Debit = 0;
                        NL.Credit = Convert.ToDouble(Repayment);
                        NL.PaymentFlag = 0;
                        NL.ValueDate = MyUtility.getCurrentLocalDateTime().ToString("yyyy/MM/dd");
                        NL.ValueTime = MyUtility.getCurrentLocalDateTime().ToString("H:mmss");
                        NL.Audit = "";
                        bool IsValid = false;
                        NL.DueDate = Helper.getLastDateOfMonth(locatime,out IsValid);
                        _DR.inserRepaymentAmount(NL);
                        locatime = MyUtility.getCurrentLocalDateTime().AddMonths(i);
                    }
                }

                return payLoan;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return 0;
            }
        }


        public string insertintoTransactLog(dynamic resp, string loanRefNumber)
        {
            try
            {
                resp = JObject.Parse(resp);
                PatnerTransactLog PTL = new PatnerTransactLog();
                PTL.PatnerReference = resp.transactionid;
                 PTL.PatnerCode = "FLUTTERWAVE";
                PTL.PatnerResponse = resp.message;
                PTL.RefNum = loanRefNumber;
                PTL.DateCreated = MyUtility.getCurrentLocalDateTime();
               var TransLog = _DR.insertintoTransactLog(PTL);
               
                return TransLog.RefNum;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        public void SendDisbursementEmail(DataAccessA.Classes.AppLoanss Apploans)
        {
            try
            {
                int loanTenure = Convert.ToInt16(Apploans.LoanTenure);
                string repayment = _DR.GetRepaymenrAmount(loanTenure);
                string myname = Apploans.Firstname + " " + Apploans.Surname;
                string loanAmount = MyUtility.ConvertToCurrency(Apploans.LoanAmount.ToString());
                string repaymentAmount = MyUtility.ConvertToCurrency(repayment);
                var bodyTxt = System.IO.File.ReadAllText(HostingEnvironment.MapPath("~/EmailNotifications/DisbursementEmail.html"));
                bodyTxt = bodyTxt.Replace("$ApplicantName", myname).Replace("$refNumber", Apploans.LoanRefNumber);
                bodyTxt = bodyTxt.Replace("$LoanAmount", loanAmount).Replace("$LoanTenure", Apploans.LoanTenure.ToString());
                bodyTxt = bodyTxt.Replace("$RepaymentAmt", repaymentAmount);
                var msgHeader = $"CashNowNow NYSC Loan Disbursement Notification - " + Apploans.LoanRefNumber;
                var sendMail = NotificationService.SendMailOut(msgHeader, bodyTxt, Apploans.EmailAddress, null, null);
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
            }
        }


        [HttpGet]
        public ActionResult ComissionCashout()
        {
            try
            {
               // TempData["ErrMsg"] = "";
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                var userFk = _DR.getUserID(appUser);
                GetMenus();
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }
                int val = 0;
                ViewData["nBanks"] = new SelectList(_DR.GetBanks(), "FlutterWaveBankCode", "NAME", val);
                var Records = _DR.getCommisioRecords(userFk);
                var resp = Comisionsums(Records);
                ViewBag.Records = Records;
                double TotalRepayment = Convert.ToDouble(resp);
                TotalRepayment = Math.Round(TotalRepayment);
                ViewBag.Balance = TotalRepayment;
                TempData["Records"] = Records;
                TempData["Amount"] = TotalRepayment;
                return View();
            }
            catch (Exception ex)
            {

                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


        private double Comisionsums(List<NYSCReferralLedger> Record)
        {
            try
            {
                List<string> AmountList = new List<string>();
                double Total = 0;
                {
                    for (int i = 0; i < Record.Count; i++)
                    {

                        Total += Convert.ToDouble(Record[i].Credit) - Convert.ToDouble(Record[i].Debit);
                    }
                }
                return Total;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return 0;
            }
        }



        [HttpPost]
        public ActionResult ComissionCashout(FormCollection form, UvlotExt.Classes.TableObjects.LoanApplication LP)
        {
            try
            {
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                var userFk = _DR.getUserID(appUser);
                GetMenus();
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }
              
                double TotAmt = Convert.ToDouble(TempData["Amount"]);
                double amt = Convert.ToDouble(form["TotAmt"]);
                double Casoutamt = Convert.ToDouble(form["Amt"]);
                var CashoutLimit = Convert.ToDouble(ConfigurationManager.AppSettings["CashoutLimit"]);
                //double CashoutLimit = Convert.ToDouble(ConfigurationManager.AppSettings["CashoutLimit"]);
                if (amt > TotAmt || Casoutamt > TotAmt || Casoutamt < CashoutLimit)
                {
                    TempData["ErrMsg"] = "Invalid Amount";
                    return RedirectToAction("ComissionCashout");
                }
                double RealAmt = TotAmt - Casoutamt;
                double mode = 0; double div = 0;
                var num = someCalculations(Casoutamt, out mode, out div);
                div = Math.Truncate(div);
                //LoansLedger Ln = new LoansLedger();
                List<NYSCReferralLedger> Ln = new List<NYSCReferralLedger>();


                dynamic CusObj = new JObject();
                CusObj.bankcodes = LP.BankCode;
                CusObj.AccountNumber = LP.AccountNumber;
                CusObj.LoanAmount = Casoutamt;
                CusObj.AccountName = LP.AccountName;
                CusObj.narration = "Commission Earned:" + "" + CusObj.LoanAmount + "For" + CusObj.AccountName;
                CusObj.currency = ConfigurationManager.AppSettings["Currency"];
                CusObj.LoanRefNumber = MyUtility.GenerateRefNo();
                var resp = DisburseComisionCash(CusObj);
                if (resp != null)
                {
                    int res = 0;
                    int vals = 0;
                    dynamic result = JObject.Parse(resp);
                    if (result?.status == "true")
                    {
                        if (mode > 0)
                        {
                            div = div + 1;
                            vals = Convert.ToInt16(div);
                            Ln = _DR.getCommisioRecordsExact(userFk, vals);
                            res = _DR.UpdateNYSCLoanReferalLeder(Ln, mode, div);

                        }
                        if (mode == 0)
                        {
                            vals = Convert.ToInt16(div);
                            Ln = _DR.getCommisioRecordsExact(userFk, vals);
                            res = _DR.UpdateNYSCLoanReferalLeder(Ln, mode, div);
                        }
                        TempData["SucMsg"] = "Transaction Succesful";
                        return RedirectToAction("ComissionCashout");
                    }


                }

                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


        public double someCalculations(double realAmt, out double Mode, out double div)
        {
            try
            {
                Mode = realAmt % 100;
                div = realAmt / 100;

                return 0;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                Mode = 0;
                div = 0;
                return 0;
            }
        }




        [HttpGet]
        public string DisburseComisionCash(dynamic cusObj)
        {
            try
            {
                dynamic obj = new JObject();
                dynamic headervalues = new JObject();

                string seckey = ConfigurationManager.AppSettings["DiburseSeckey"];

                obj.callbackurl = ConfigurationManager.AppSettings["DisburseCllbackUrl"];
                obj.bankcode = cusObj.bankcodes;
                obj.account_number = cusObj.AccountNumber;
                string LoanAmount = Convert.ToString(cusObj.LoanAmount);
                obj.amount = LoanAmount;
                obj.narration = ConfigurationManager.AppSettings["Narration"] + "" + obj.amount + "For" + cusObj.AccountName;
                obj.currency = ConfigurationManager.AppSettings["Currency"];

                obj.trxRef = cusObj.LoanRefNumber;

                obj.beneficiary_name = cusObj.AccountName;
                var builder = new StringBuilder();
                builder.Append(seckey).Append(obj.callbackurl);

                var hash = new CryptographyManager().ComputeHash(builder.ToString(), HashName.SHA512);

                // For The Signature


                obj.hashValue = hash;
                var json = obj.ToString();
                WebLog.Log("BuyPayLoad" + json);
                string callbackUrl = obj.callbackurl;
                string hashval = hash.ToString();
                var PostUrl = ConfigurationManager.AppSettings["DisburseCash"];
                WebLog.Log("BuyPayLoad" + json);
                var data = MyUtility.DoPosts(json, $"{PostUrl}", seckey, callbackUrl, hashval);


                WebLog.Log("BuyPayLoad" + json);
                WebLog.Log("PostUrl" + PostUrl);
                return data;

            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }
        [HttpGet]
        public ActionResult AllApprovedFinalLevel()

        {

            try
            {
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = ""; TempData["Error"] = "";
                AppStatFk = 5;
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }

                //var patnerCode = "FLUTTERWAVE";
             
                //var lstMenus = _DR.ApproveLoansRemFlu(AppStatFk,patnerCode); 
              var lstMenus = _DR.FourthApproval(AppStatFk);
                ViewBag.Data = lstMenus;
                Session["AllTransaction"] = ViewBag.Data;
                // Session["AllTransaction"] = ViewBag.Data;




                GetMenus();
                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        [HttpGet]
        public ActionResult ApprovedFinalLevel(string Refid, TableObjects.LoanApplication LoanApp, DataAccessA.Classes.LoanApplication lApObj, AppLoanss Apploan, AppLoanss LoanApps)

        {

            try
            {
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = ""; TempData["Error"] = "";
                AppStatFk = 5;
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                var User = _DR.getUser(appUser);
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }
                if (Refid == null || Refid == "")
                {
                    return RedirectToAction("ApprovedSecondLoan");
                }

                var LoanAppss = _DR.GetNYSCLoanApplication(Refid, AppStatFk);
                if (LoanAppss == null)
                {
                    // return RedirectToAction("RecommendLoanSecondLevel");
                    TempData["Error"] = "Check The Status Of The Application";
                    return RedirectToAction("ApprovedFinalLevel");
                }

                GetApplicantInfo(Refid, LoanApp, lApObj, Apploan, LoanAppss);
                TempData["Username"] = User.Firstname;
                TempData["LoanObj"] = Apploan;
                GetMenus();

                return View(Apploan);
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message, ex.StackTrace);
                return null;
            }
        }
        [HttpPost]
        public ActionResult ApprovedFinalLevel(DataAccessA.Classes.AppLoanss Apploans, FormCollection form)
        {
            TempData["SucMsg"] = ""; TempData["ErrMsg"] = ""; TempData["Error"] = "";
            var id = Convert.ToInt16(form["ID"]);
            var approve = Convert.ToInt16(form["Accept"]);
            Apploans.ID = id;
            Apploans.ApplicationStatus_FK = approve;
            var resp = _DR.UpdateNyscLoanApplication(Apploans);


            ViewBag.Data = resp;
            Session["AllTransaction"] = ViewBag.Data;
            return RedirectToAction("ApprovedFinalLevel");

        }


        [HttpGet]
        public ActionResult AllApprovedSecondLevel()

        {

            try
            {
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = ""; TempData["Error"] = "";
                AppStatFk = 3;
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }


                var lstMenus = _DR.ApproveLoans(AppStatFk);
                ViewBag.Data = lstMenus;

                // Session["AllTransaction"] = ViewBag.Data;




                GetMenus();
                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        [HttpGet]
        public ActionResult ApprovedSecondLoan(string Refid, TableObjects.LoanApplication LoanApp, DataAccessA.Classes.LoanApplication lApObj, AppLoanss Apploan, AppLoanss LoanApps)

        {

            try
            {
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = ""; TempData["Error"] = "";
                AppStatFk = 3;
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                var User = _DR.getUser(appUser);
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }
                if (Refid == null || Refid == "")
                {
                    return RedirectToAction("ApprovedSecondLoan");
                }

                var LoanAppss = _DR.GetNYSCLoanApplication(Refid, AppStatFk);
                if (LoanAppss == null)
                {
                    // return RedirectToAction("RecommendLoanSecondLevel");
                    TempData["Error"] = "Check The Status Of The Application";
                    return RedirectToAction("ApprovedSecondLoan");
                }

                GetApplicantInfo(Refid, LoanApp, lApObj, Apploan, LoanAppss);
                TempData["Username"] = User.Firstname;
                TempData["LoanObj"] = Apploan;
                GetMenus();

                return View(Apploan);
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message, ex.StackTrace);
                return null;
            }
        }
        [HttpPost]
        public ActionResult ApprovedSecondLoan(DataAccessA.Classes.AppLoanss Apploans, FormCollection form)
        {
            TempData["SucMsg"] = ""; TempData["ErrMsg"] = ""; TempData["Error"] = "";
            var id = Convert.ToInt16(form["ID"]);
            var approve = Convert.ToInt16(form["Accept"]);
            Apploans.ID = id;
            Apploans.ApplicationStatus_FK = approve;
            var resp = _DR.UpdateNyscLoanApplication(Apploans);


            ViewBag.Data = resp;
            Session["AllTransaction"] = ViewBag.Data;
            return RedirectToAction("ApprovedSecondLoan");

        }

        [HttpGet]
        public ActionResult GetPaymentResponse()
        {
            try
            {
                return View();
            }
            catch(Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


        [HttpGet]
        public string DisburseCash(dynamic cusObj)
        {
            try
            {
                dynamic obj = new JObject();
                dynamic headervalues = new JObject();

                string seckey = ConfigurationManager.AppSettings["DiburseSeckey"];

                obj.callbackurl = ConfigurationManager.AppSettings["DisburseCllbackUrl"];
                obj.bankcode = Helper.GetFlutterwaveBankCodeByRemitaCode( cusObj.bankcodes);
               var bankc = Helper.GetFlutterwaveBankCodeByRemitaCode(cusObj.bankcodes);
                obj.account_number = cusObj.AccountNumber;
                string LoanAmount = Convert.ToString(cusObj.LoanAmount);
                obj.amount = LoanAmount;
                obj.narration = ConfigurationManager.AppSettings["Narration"] + "" + obj.amount + "For" + cusObj.AccountName;
                obj.currency = ConfigurationManager.AppSettings["Currency"];

                obj.trxRef = cusObj.LoanRefNumber;


                obj.beneficiary_name = cusObj.AccountName;
                var builder = new StringBuilder();
                builder.Append(seckey).Append(obj.callbackurl);

                var hash = new CryptographyManager().ComputeHash(builder.ToString(), HashName.SHA512);

                // For The Signature


                obj.hashValue = hash;
                var json = obj.ToString();
                WebLog.Log("BuyPayLoad" + json);
                string callbackUrl = obj.callbackurl;
                string hashval = hash.ToString();
                var PostUrl = ConfigurationManager.AppSettings["DisburseCash"];
                WebLog.Log("BuyPayLoad" + json);
                var data = MyUtility.DoPosts(json, $"{PostUrl}", seckey, callbackUrl, hashval);


                WebLog.Log("BuyPayLoad" + json);
                WebLog.Log("PostUrl" + PostUrl);
                return data;

            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        [HttpGet]
        public ActionResult MyReferredNYSCApplications()

        {

            try
            {
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = ""; TempData["Error"] = "";
               
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();
               
              
                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }

                
                var lstMenus = _DR.NYSCApplications(user);
                ViewBag.Data = lstMenus;

                Session["AllTransaction"] = ViewBag.Data;




                GetMenus();
                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        [HttpGet]
        public ActionResult AllApprovedLoan()

        {

            try
            {
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = ""; TempData["Error"] = "";
                AppStatFk = 2;
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }


                var lstMenus = _DR.ApproveLoans(AppStatFk);

                if (lstMenus == null)
                {
                    return RedirectToAction("AllApprovedLoan");
                }
                ViewBag.Data = lstMenus;

                Session["AllTransaction"] = ViewBag.Data;




                GetMenus();
                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }



        [HttpGet]
        public ActionResult Exportoexcel2()
        {
            ExportToExcel();
            GetMenus();
            return View("AllApprovedLoan");
            //return View("");
        }

        [HttpGet]
        public ActionResult ApprovedLoan(string Refid, TableObjects.LoanApplication LoanApp, DataAccessA.Classes.LoanApplication lApObj, AppLoanss Apploan, AppLoanss LoanApps)

        {

            try
            {
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = ""; TempData["Error"] = "";
                AppStatFk = 2;
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                var User = _DR.getUser(appUser);
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }
                if (Refid == null || Refid == "")
                {
                    return RedirectToAction("ApproveLoan");
                }

                var LoanAppss = _DR.GetNYSCLoanApplication(Refid, AppStatFk);
                if (LoanAppss == null)
                {
                    // return RedirectToAction("RecommendLoanSecondLevel");
                    TempData["Error"] = "Check The Status Of The Application";
                    return RedirectToAction("ApproveLoan");
                }

                GetApplicantInfo(Refid, LoanApp,  lApObj, Apploan, LoanAppss);
                TempData["Username"] = User.Firstname;
                TempData["LoanObj"] = Apploan;
                GetMenus();

                return View(Apploan);
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message, ex.StackTrace);
                return null;
            }
        }
        [HttpPost]
        public ActionResult ApprovedLoan(DataAccessA.Classes.AppLoanss Apploans, FormCollection form)
        {
            TempData["SucMsg"] = ""; TempData["ErrMsg"] = ""; TempData["Error"] = "";
            var id = Convert.ToInt16(form["ID"]);
            var approve = Convert.ToInt16(form["Accept"]);
            Apploans.ID = id;
            Apploans.ApplicationStatus_FK = approve;
            var resp = _DR.UpdateNyscLoanApplication(Apploans);


            ViewBag.Data = resp;
            Session["AllTransaction"] = ViewBag.Data;
            return RedirectToAction("ApprovedLoan");

        }

        public int insertIntoLoanApproval(DataAccessA.Classes.AppLoanss Apploans)
        {
            try
            {
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                var User = _DR.getUser(appUser);

                NYSCLoanApproval La = new NYSCLoanApproval();
                La.LoanApplication_FK = Apploans.ID;
                La.CommentBy = User.ID;
                La.ApplicationStatus_FK = Apploans.ApplicationStatus_FK; //Convert.ToInt16(form["Accept"]);
                La.Comment = Apploans.comment; //Convert.ToString(form["comment"]);
                La.DateCreated = MyUtility.getCurrentLocalDateTime();
                La.ValueDate = MyUtility.getCurrentLocalDateTime().ToString("dd/MM/yyyy H:mm ss");
                La.IsVisible = 1;
                var resp = _DM.insertLoanApproval(La);
                return resp;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return 0;
            }
        }

        public void GetApplicantInfo(string Refid, TableObjects.LoanApplication LoanApp, DataAccessA.Classes.LoanApplication lApObj, AppLoanss Apploan, AppLoanss LoanApps)
        {
            try
            {
                NYSCLOANController nysc = new NYSCLOANController();
                // AppLoan LoanApps = new AppLoan();
                Apploan.ID = LoanApp.ID;
                Apploan.Firstname = string.IsNullOrEmpty(LoanApps.Firstname) ? "none" : LoanApps.Firstname;
                Apploan.AccountName = string.IsNullOrEmpty(LoanApps.AccountName) ? "none" : LoanApps.AccountName;
                Apploan.AccountNumber = string.IsNullOrEmpty(LoanApps.AccountNumber) ? "none" : LoanApps.AccountNumber;
                Apploan.ApplicantID = string.IsNullOrEmpty(LoanApps.ApplicantID) ? "none" : LoanApps.ApplicantID;
                Apploan.BankCode = string.IsNullOrEmpty(LoanApps.BankCode) ? "none" : LoanApps.BankCode;
                Apploan.bankcodes = string.IsNullOrEmpty(LoanApps.bankcodes) ? "none" : LoanApps.bankcodes;
                Apploan.BVN = string.IsNullOrEmpty(LoanApps.BVN) ? "none" : LoanApps.BVN;
                Apploan.StateCode = string.IsNullOrEmpty(LoanApps.StateCode) ? "none" : LoanApps.StateCode;
                Apploan.DateOfBirth = string.IsNullOrEmpty(LoanApps.DateOfBirth) ? "none" : LoanApps.DateOfBirth;
                Apploan.PermanentAddress = string.IsNullOrEmpty(LoanApps.PermanentAddress) ? "none" : LoanApps.PermanentAddress;
                Apploan.Landmark = string.IsNullOrEmpty(LoanApps.Landmark) ? "none" : LoanApps.Landmark;
                Apploan.ClosestBusStop = string.IsNullOrEmpty(LoanApps.ClosestBusStop) ? "none" : LoanApps.ClosestBusStop;
                Apploan.LGAs = string.IsNullOrEmpty(LoanApps.LGAs) ? "none" : LoanApps.LGAs;
                Apploan.TempLGAs = string.IsNullOrEmpty(LoanApps.TempLGAs) ? "none" : LoanApps.TempLGAs;
                Apploan.NyscLGAs = string.IsNullOrEmpty(LoanApps.NyscLGAs) ? "none" : LoanApps.NyscLGAs;
                Apploan.NyscStateofResidence = string.IsNullOrEmpty(LoanApps.NyscStateofResidence) ? "none" : LoanApps.NyscStateofResidence;
                Apploan.TempStateofResidence = string.IsNullOrEmpty(LoanApps.TempStateofResidence) ? "none" : LoanApps.TempStateofResidence;
                Apploan.TemporaryAddress = string.IsNullOrEmpty(LoanApps.TemporaryAddress) ? "none" : LoanApps.TemporaryAddress;
                Apploan.StateofResidence = string.IsNullOrEmpty(LoanApps.StateofResidence) ? "none" : LoanApps.StateofResidence;
                Apploan.TempLandmark = string.IsNullOrEmpty(LoanApps.TempLandmark) ? "none" : LoanApps.TempLandmark;
                Apploan.TempClosestBusStop = string.IsNullOrEmpty(LoanApps.TempClosestBusStop) ? "none" : LoanApps.TempClosestBusStop;
                Apploan.Employer = string.IsNullOrEmpty(LoanApps.Employer) ? "none" : LoanApps.Employer;
                Apploan.OfficialAddress = string.IsNullOrEmpty(LoanApps.OfficialAddress) ? "none" : LoanApps.OfficialAddress;
                Apploan.PassOutMonth = string.IsNullOrEmpty(LoanApps.PassOutMonth) ? "none" : LoanApps.PassOutMonth;
                Apploan.CDSDay = string.IsNullOrEmpty(LoanApps.CDSDay) ? "none" : LoanApps.CDSDay;
                Apploan.CDSGroup = string.IsNullOrEmpty(LoanApps.CDSGroup) ? "none" : LoanApps.CDSGroup;

                Apploan.EmailAddress = string.IsNullOrEmpty(LoanApps.EmailAddress) ? "none" : LoanApps.EmailAddress;
                Apploan.ValueTime = string.IsNullOrEmpty(LoanApps.ValueTime) ? "none" : LoanApps.ValueTime;
                Apploan.ValueDate = string.IsNullOrEmpty(LoanApps.ValueDate) ? "none" : LoanApps.ValueDate;
                Apploan.Title = string.IsNullOrEmpty(LoanApps.Title) ? "none" : LoanApps.Title;
                Apploan.Surname = string.IsNullOrEmpty(LoanApps.Surname) ? "none" : LoanApps.Surname;
                Apploan.NigerianStates = string.IsNullOrEmpty(LoanApps.NigerianStates) ? "none" : LoanApps.NigerianStates;
                 Apploan.PPA_Department = string.IsNullOrEmpty(LoanApps.PPA_Department) ? "none" : LoanApps.PPA_Department;
                Apploan.PhoneNumber = string.IsNullOrEmpty(LoanApps.PhoneNumber) ? "none" : LoanApps.PhoneNumber;
                Apploan.Othernames = string.IsNullOrEmpty(LoanApps.Othernames) ? "none" : LoanApps.Othernames;
                Apploan.PPA_EmailAddress =
                   string.IsNullOrEmpty(LoanApps.PPA_EmailAddress) ? "none" : LoanApps.PPA_EmailAddress;
                Apploan.EMG_Relationship =
                    string.IsNullOrEmpty(LoanApps.EMG_Relationship) ? "none" : LoanApps.EMG_Relationship;
                Apploan.EMG_PhoneNumber = string.IsNullOrEmpty(LoanApps.EMG_PhoneNumber) ? "none" : LoanApps.EMG_PhoneNumber;
                Apploan.EMG_HomeAddress = string.IsNullOrEmpty(LoanApps.EMG_HomeAddress) ? "none" : LoanApps.EMG_HomeAddress;
                Apploan.EMG_FullName = string.IsNullOrEmpty(LoanApps.EMG_FullName) ? "none" : LoanApps.EMG_FullName;
                Apploan.EMG_EmailAddress =
                    string.IsNullOrEmpty(LoanApps.EMG_EmailAddress) ? "none" : LoanApps.EMG_EmailAddress;


                Apploan.EMG_Relationship2 =
                    string.IsNullOrEmpty(LoanApps.EMG_Relationship2) ? "none" : LoanApps.EMG_Relationship2;
                Apploan.EMG_PhoneNumber2 = string.IsNullOrEmpty(LoanApps.EMG_PhoneNumber2) ? "none" : LoanApps.EMG_PhoneNumber2;
                Apploan.EMG_HomeAddress2 = string.IsNullOrEmpty(LoanApps.EMG_HomeAddress2) ? "none" : LoanApps.EMG_HomeAddress2;
                Apploan.EMG_FullName2 = string.IsNullOrEmpty(LoanApps.EMG_FullName2) ? "none" : LoanApps.EMG_FullName2;
                Apploan.EMG_EmailAddress2 =
                    string.IsNullOrEmpty(LoanApps.EMG_EmailAddress2) ? "none" : LoanApps.EMG_EmailAddress2;
                Apploan.PPA_PhoneNumber =
                 string.IsNullOrEmpty(LoanApps.PPA_PhoneNumber) ? "none" : LoanApps.PPA_PhoneNumber;
                Apploan.MaritalStatus =
                    string.IsNullOrEmpty(LoanApps.MaritalStatus) ? "none" : LoanApps.MaritalStatus;
                Apploan.LoanTenure = Convert.ToInt32(LoanApps.LoanTenure);
                 Apploan.PPA_ROle = string.IsNullOrEmpty(LoanApps.PPA_ROle) ? "none" : LoanApps.PPA_ROle;
                Apploan.LoanAmount = LoanApps.LoanAmount;
                string LoanAmt = Convert.ToString(Apploan.LoanAmount);
                Apploan.ConvertedAmount = MyUtility.ConvertToCurrency(LoanAmt);
                Apploan.LoanAmount = Convert.ToDouble(Apploan.LoanAmount);
                Apploan.Landmark =
                    string.IsNullOrEmpty(LoanApps.Landmark) ? "none" : LoanApps.Landmark;
                 Apploan.PPA_supervisorEmail = string.IsNullOrEmpty(LoanApps.PPA_supervisorEmail) ? "none" : LoanApps.PPA_supervisorEmail;
                Apploan.ExistingLoan = LoanApps.ExistingLoan;

                Apploan.ExistingLoan_NoOfMonthsLeft = Convert.ToInt16(LoanApps.ExistingLoan_NoOfMonthsLeft);
                Apploan.ExistingLoan_OutstandingAmount = LoanApps.ExistingLoan_OutstandingAmount;
                Apploan.Firstname =
                    string.IsNullOrEmpty(LoanApps.Firstname) ? "none" : LoanApps.Firstname;
                Apploan.ID = LoanApps.ID;
                Apploan.LoanRefNumber =
                    string.IsNullOrEmpty(LoanApps.LoanRefNumber) ? "none" : LoanApps.LoanRefNumber;
                Apploan.ClosestBusStop =
               string.IsNullOrEmpty(LoanApps.ClosestBusStop) ? "none" : LoanApps.ClosestBusStop;
                Apploan.Department =
                string.IsNullOrEmpty(LoanApps.Department) ? "none" : LoanApps.Department;
                Apploan.Occupation =
               string.IsNullOrEmpty(LoanApps.Occupation) ? "none" : LoanApps.Occupation;
                Apploan.Gender = string.IsNullOrEmpty(LoanApps.Gender) ? "none" : LoanApps.Gender;
                Apploan.PPA_supervisorName = string.IsNullOrEmpty(LoanApps.PPA_supervisorName) ? "none" : LoanApps.PPA_supervisorName;
                Apploan.PPA_supervisorPhonenumber = string.IsNullOrEmpty(LoanApps.PPA_supervisorPhonenumber) ? "none" : LoanApps.PPA_supervisorPhonenumber;
                Apploan.Designation = string.IsNullOrEmpty(LoanApps.Designation) ? "none" : LoanApps.Designation;
                Apploan.RelativeRelationship2_FK = string.IsNullOrEmpty(LoanApps.RelativeRelationship2_FK) ? "none" : LoanApps.RelativeRelationship2_FK;
                Apploan.RelativeRelationship_FK = string.IsNullOrEmpty(LoanApps.RelativeRelationship_FK) ? "none" : LoanApps.RelativeRelationship_FK;

                Apploan.SecondRelativeName = string.IsNullOrEmpty(LoanApps.SecondRelativeName) ? "none" : LoanApps.SecondRelativeName;
                Apploan.SecondRelativePhoneNumber = string.IsNullOrEmpty(LoanApps.SecondRelativePhoneNumber) ? "none" : LoanApps.SecondRelativePhoneNumber;
                Apploan.FirstRelativeName = string.IsNullOrEmpty(LoanApps.FirstRelativeName) ? "none" : LoanApps.FirstRelativeName;
                Apploan.FirstRelativePhoneNumber = string.IsNullOrEmpty(LoanApps.FirstRelativePhoneNumber) ? "none" : LoanApps.FirstRelativePhoneNumber;


                Apploan.TwitterHandle = string.IsNullOrEmpty(LoanApps.TwitterHandle) ? "none" : LoanApps.TwitterHandle;
                Apploan.FacebookName = string.IsNullOrEmpty(LoanApps.FacebookName) ? "none" : LoanApps.FacebookName;
                Apploan.InstagramHandle = string.IsNullOrEmpty(LoanApps.InstagramHandle) ? "none" : LoanApps.InstagramHandle;
                Apploan.NetMonthlyIncome = LoanApps.SalaryAmount;
                string SalAmt = Convert.ToString(Apploan.LoanAmount);
                Apploan.ConvertedAmount = MyUtility.ConvertToCurrency(SalAmt);
                Apploan.NyscIdCardFilePath = LoanApps.NyscIdCardFilePath;
                Apploan.STA_FilePath = LoanApps.STA_FilePath;
                Apploan.NyscpassportFilePath = LoanApps.NyscpassportFilePath;
                Apploan.NyscCallUpLetterFilePath = LoanApps.NyscCallUpLetterFilePath;

                Apploan.NyscPostingLetterFllePath = LoanApps.NyscPostingLetterFllePath;
                Apploan.NyscProfileDashboardFilePath = LoanApps.NyscProfileDashboardFilePath;
                Apploan.ReferralCode = string.IsNullOrEmpty(LoanApps.ReferralCode) ? "none" : LoanApps.ReferralCode;
                //string IdentImage = LoanApps.NyscIdCardFilePath.After("C:\\Users\\PAELYT SOLUTION 6\\Documents\\Visual Studio 2015\\Projects\\UvlotExt\\UvlotExt\\UvlotExt\\").Replace("\\", "/");
                //string IdentImages = LoanApps.STA_FilePath.After("C:\\Users\\PAELYT SOLUTION 6\\Documents\\Visual Studio 2015\\Projects\\UvlotExt\\UvlotExt\\UvlotExt\\").Replace("\\", "/");
                string slash = "/";
                if (LoanApps.NyscIdCardFilePath != null)
                {
                    string IdentImage = LoanApps.NyscIdCardFilePath.After("h:\\root\\home\\paelyt-001\\www\\paelyt\\uvlotapublish\\").Replace("\\", "/");
                    Apploan.NyscIdCardFilePath = string.IsNullOrEmpty(IdentImage) ? "none" : slash + IdentImage;
                }
                if (LoanApps.NyscIdCardFilePath == null)
                {
                    Apploan.NyscIdCardFilePath = "none";
                }

                if (LoanApps.STA_FilePath != null)
                {
                    string IdentImages = LoanApps.STA_FilePath.After("h:\\root\\home\\paelyt-001\\www\\paelyt\\uvlotapublish\\").Replace("\\", "/");
                    Apploan.STA_FilePath = string.IsNullOrEmpty(IdentImages) ? "none" : slash + IdentImages;
                }
                if (LoanApps.STA_FilePath == null)
                {
                    Apploan.STA_FilePath = "none";
                }
                if (LoanApps.NyscpassportFilePath != null)
                {
                    string IdentImagess = LoanApps.NyscpassportFilePath.After("h:\\root\\home\\paelyt-001\\www\\paelyt\\uvlotapublish\\").Replace("\\", "/");
                    Apploan.NyscpassportFilePath = string.IsNullOrEmpty(IdentImagess) ? "none" : slash + IdentImagess;
                }
                if (LoanApps.NyscpassportFilePath == null)
                {
                    Apploan.NyscpassportFilePath = "none";
                }
                if (LoanApps.NyscPostingLetterFllePath != null)
                {
                    string IdentImgs = LoanApps.NyscPostingLetterFllePath.After("h:\\root\\home\\paelyt-001\\www\\paelyt\\uvlotapublish\\").Replace("\\", "/");
                    Apploan.NyscPostingLetterFllePath = string.IsNullOrEmpty(IdentImgs) ? "none" : slash + IdentImgs;
                }
                if (LoanApps.NyscPostingLetterFllePath == null)
                {
                    Apploan.NyscPostingLetterFllePath = "none";
                }
                if (LoanApps.NyscCallUpLetterFilePath != null)
                {
                    string IdentImgss = LoanApps.NyscCallUpLetterFilePath.After("h:\\root\\home\\paelyt-001\\www\\paelyt\\uvlotapublish\\").Replace("\\", "/");
                    Apploan.NyscCallUpLetterFilePath = string.IsNullOrEmpty(IdentImgss) ? "none" : slash + IdentImgss;
                }
                if (LoanApps.NyscCallUpLetterFilePath == null)
                {
                    Apploan.NyscCallUpLetterFilePath = "none";
                }
                if (LoanApps.NyscProfileDashboardFilePath != null)
                {
                    string IdentImgsss = LoanApps.NyscProfileDashboardFilePath.After("h:\\root\\home\\paelyt-001\\www\\paelyt\\uvlotapublish\\").Replace("\\", "/");
                    Apploan.NyscProfileDashboardFilePath = string.IsNullOrEmpty(IdentImgsss) ? "none" : slash + IdentImgsss;
                }
                if (LoanApps.NyscProfileDashboardFilePath == null)
                {
                    Apploan.NyscProfileDashboardFilePath = "none";
                }

                
                if (LoanApps.LetterOfundertaken != null)
                {
                    string IdentImgssss = LoanApps.LetterOfundertaken.After("h:\\root\\home\\paelyt-001\\www\\paelyt\\uvlotapublish\\").Replace("\\", "/");
                    Apploan.LetterOfundertaken = string.IsNullOrEmpty(IdentImgssss) ? "none" : slash + IdentImgssss;
                }
                if (LoanApps.LetterOfundertaken == null)
                {
                    Apploan.LetterOfundertaken = "none";
                }


                //h:\root\home\paelyt - 001\www\paelyt\uvlotapublish\Images\PowerScreenGrab.PNG
                //string IdentImage = LoanApps.IdentficationNumberImage.After("h:\\root\\home\\paelyt-001\\www\\Uvlot\\").Replace("\\", "/");
                /* WebLog.Log("Image Path" + IdentImage);
                 string slash = "/";
                 Apploan.NyscIdCardFilePath = string.IsNullOrEmpty(IdentImage) ? "none" : slash + IdentImage;
                 Apploan.STA_FilePath = string.IsNullOrEmpty(IdentImages) ? "none" : slash + IdentImages;
                 Apploan.NyscpassportFilePath = string.IsNullOrEmpty(IdentImagess) ? "none" : slash + IdentImagess;
                 Apploan.NyscPostingLetterFllePath = string.IsNullOrEmpty(IdentImgs) ? "none" : slash + IdentImgs;
                 Apploan.NyscCallUpLetterFilePath = string.IsNullOrEmpty(IdentImgss) ? "none" : slash + IdentImgss;
                 Apploan.NyscProfileDashboardFilePath = string.IsNullOrEmpty(IdentImgsss) ? "none" : slash + IdentImgsss;*/
                Apploan.ApplicationStatus_FK = LoanApp.ApplicationStatus_FK;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
            }
        }


        //public void GetApplicantInfo(string Refid, TableObjects.LoanApplication LoanApp, DataAccessA.Classes.LoanApplication lApObj, AppLoanss Apploan, AppLoanss LoanApps)
        //{
        //    try
        //    {
        //        NYSCLOANController nysc = new NYSCLOANController();

        //        Apploan.ID = LoanApp.ID;
        //        Apploan.Firstname = string.IsNullOrEmpty(LoanApps.Firstname) ? "none" : LoanApps.Firstname;
        //        Apploan.AccountName = string.IsNullOrEmpty(LoanApps.AccountName) ? "none" : LoanApps.AccountName;
        //        Apploan.AccountNumber = string.IsNullOrEmpty(LoanApps.AccountNumber) ? "none" : LoanApps.AccountNumber;
        //        Apploan.ApplicantID = string.IsNullOrEmpty(LoanApps.ApplicantID) ? "none" : LoanApps.ApplicantID;
        //        Apploan.BankCode = string.IsNullOrEmpty(LoanApps.BankCode) ? "none" : LoanApps.BankCode;
        //        Apploan.bankcodes = string.IsNullOrEmpty(LoanApps.bankcodes) ? "none" : LoanApps.bankcodes;
        //        Apploan.BVN = string.IsNullOrEmpty(LoanApps.BVN) ? "none" : LoanApps.BVN;
        //        Apploan.StateCode = string.IsNullOrEmpty(LoanApps.StateCode) ? "none" : LoanApps.StateCode;
        //        Apploan.DateOfBirth = string.IsNullOrEmpty(LoanApps.DateOfBirth) ? "none" : LoanApps.DateOfBirth;
        //        Apploan.PermanentAddress = string.IsNullOrEmpty(LoanApps.PermanentAddress) ? "none" : LoanApps.PermanentAddress;
        //        Apploan.Landmark = string.IsNullOrEmpty(LoanApps.Landmark) ? "none" : LoanApps.Landmark;
        //        Apploan.ClosestBusStop = string.IsNullOrEmpty(LoanApps.ClosestBusStop) ? "none" : LoanApps.ClosestBusStop;
        //        Apploan.LGAs = string.IsNullOrEmpty(LoanApps.LGAs) ? "none" : LoanApps.LGAs;
        //        Apploan.TempLGAs = string.IsNullOrEmpty(LoanApps.TempLGAs) ? "none" : LoanApps.TempLGAs;
        //        Apploan.NyscLGAs = string.IsNullOrEmpty(LoanApps.NyscLGAs) ? "none" : LoanApps.NyscLGAs;
        //        Apploan.NyscStateofResidence = string.IsNullOrEmpty(LoanApps.NyscStateofResidence) ? "none" : LoanApps.NyscStateofResidence;
        //        Apploan.TempStateofResidence = string.IsNullOrEmpty(LoanApps.TempStateofResidence) ? "none" : LoanApps.TempStateofResidence;
        //        Apploan.TemporaryAddress = string.IsNullOrEmpty(LoanApps.TemporaryAddress) ? "none" : LoanApps.TemporaryAddress;
        //        Apploan.StateofResidence = string.IsNullOrEmpty(LoanApps.StateofResidence) ? "none" : LoanApps.StateofResidence;
        //        Apploan.TempLandmark = string.IsNullOrEmpty(LoanApps.TempLandmark) ? "none" : LoanApps.TempLandmark;
        //        Apploan.TempClosestBusStop = string.IsNullOrEmpty(LoanApps.TempClosestBusStop) ? "none" : LoanApps.TempClosestBusStop;
        //        Apploan.Employer = string.IsNullOrEmpty(LoanApps.Employer) ? "none" : LoanApps.Employer;
        //        Apploan.OfficialAddress = string.IsNullOrEmpty(LoanApps.OfficialAddress) ? "none" : LoanApps.OfficialAddress;
        //        Apploan.PassOutMonth = string.IsNullOrEmpty(LoanApps.PassOutMonth) ? "none" : LoanApps.PassOutMonth;
        //        Apploan.CDSDay = string.IsNullOrEmpty(LoanApps.CDSDay) ? "none" : LoanApps.CDSDay;
        //        Apploan.CDSGroup = string.IsNullOrEmpty(LoanApps.CDSGroup) ? "none" : LoanApps.CDSGroup;

        //        Apploan.EmailAddress = string.IsNullOrEmpty(LoanApps.EmailAddress) ? "none" : LoanApps.EmailAddress;
        //        Apploan.ValueTime = string.IsNullOrEmpty(LoanApps.ValueTime) ? "none" : LoanApps.ValueTime;
        //        Apploan.ValueDate = string.IsNullOrEmpty(LoanApps.ValueDate) ? "none" : LoanApps.ValueDate;
        //        Apploan.Title = string.IsNullOrEmpty(LoanApps.Title) ? "none" : LoanApps.Title;
        //        Apploan.Surname = string.IsNullOrEmpty(LoanApps.Surname) ? "none" : LoanApps.Surname;
        //        Apploan.NigerianStates = string.IsNullOrEmpty(LoanApps.NigerianStates) ? "none" : LoanApps.NigerianStates;

        //        Apploan.PhoneNumber = string.IsNullOrEmpty(LoanApps.PhoneNumber) ? "none" : LoanApps.PhoneNumber;
        //        Apploan.Othernames = string.IsNullOrEmpty(LoanApps.Othernames) ? "none" : LoanApps.Othernames;

        //        Apploan.EMG_Relationship =
        //            string.IsNullOrEmpty(LoanApps.EMG_Relationship) ? "none" : LoanApps.EMG_Relationship;
        //        Apploan.EMG_PhoneNumber = string.IsNullOrEmpty(LoanApps.EMG_PhoneNumber) ? "none" : LoanApps.EMG_PhoneNumber;
        //        Apploan.EMG_HomeAddress = string.IsNullOrEmpty(LoanApps.EMG_HomeAddress) ? "none" : LoanApps.EMG_HomeAddress;
        //        Apploan.EMG_FullName = string.IsNullOrEmpty(LoanApps.EMG_FullName) ? "none" : LoanApps.EMG_FullName;
        //        Apploan.EMG_EmailAddress =
        //            string.IsNullOrEmpty(LoanApps.EMG_EmailAddress) ? "none" : LoanApps.EMG_EmailAddress;

        //        Apploan.MaritalStatus =
        //            string.IsNullOrEmpty(LoanApps.MaritalStatus) ? "none" : LoanApps.MaritalStatus;
        //        Apploan.LoanTenure = Convert.ToInt32(LoanApps.LoanTenure);

        //        Apploan.LoanAmount = LoanApps.LoanAmount;
        //        string LoanAmt = Convert.ToString(Apploan.LoanAmount);
        //        Apploan.ConvertedAmount = MyUtility.ConvertToCurrency(LoanAmt);
        //        Apploan.LoanAmount = Convert.ToDouble(Apploan.LoanAmount);
        //        Apploan.Landmark =
        //            string.IsNullOrEmpty(LoanApps.Landmark) ? "none" : LoanApps.Landmark;

        //        Apploan.ExistingLoan = LoanApps.ExistingLoan;

        //        Apploan.ExistingLoan_NoOfMonthsLeft = Convert.ToInt16(LoanApps.ExistingLoan_NoOfMonthsLeft);
        //        Apploan.ExistingLoan_OutstandingAmount = LoanApps.ExistingLoan_OutstandingAmount;
        //        Apploan.Firstname =
        //            string.IsNullOrEmpty(LoanApps.Firstname) ? "none" : LoanApps.Firstname;
        //        Apploan.ID = LoanApps.ID;
        //        Apploan.LoanRefNumber =
        //            string.IsNullOrEmpty(LoanApps.LoanRefNumber) ? "none" : LoanApps.LoanRefNumber;
        //        Apploan.ClosestBusStop =
        //       string.IsNullOrEmpty(LoanApps.ClosestBusStop) ? "none" : LoanApps.ClosestBusStop;
        //        Apploan.Department =
        //        string.IsNullOrEmpty(LoanApps.Department) ? "none" : LoanApps.Department;
        //        Apploan.Occupation =
        //       string.IsNullOrEmpty(LoanApps.Occupation) ? "none" : LoanApps.Occupation;
        //        Apploan.Gender = string.IsNullOrEmpty(LoanApps.Gender) ? "none" : LoanApps.Gender;

        //        Apploan.Designation = string.IsNullOrEmpty(LoanApps.Designation) ? "none" : LoanApps.Designation;

        //        Apploan.NetMonthlyIncome = LoanApps.SalaryAmount;
        //        string SalAmt = Convert.ToString(Apploan.LoanAmount);
        //        Apploan.ConvertedAmount = MyUtility.ConvertToCurrency(SalAmt);
        //        Apploan.NyscIdCardFilePath = LoanApps.NyscIdCardFilePath;
        //        Apploan.STA_FilePath = LoanApps.STA_FilePath;
        //        Apploan.NyscpassportFilePath = LoanApps.NyscpassportFilePath;
        //        Apploan.NyscCallUpLetterFilePath = LoanApps.NyscCallUpLetterFilePath;

        //        Apploan.NyscPostingLetterFllePath = LoanApps.NyscPostingLetterFllePath;
        //        Apploan.NyscProfileDashboardFilePath = LoanApps.NyscProfileDashboardFilePath;
        //        Apploan.ReferralCode = string.IsNullOrEmpty(LoanApps.ReferralCode) ? "none" : LoanApps.ReferralCode;

        //        string IdentImage = LoanApps.NyscIdCardFilePath.After("h:\\root\\home\\paelyt-001\\www\\paelyt\\uvlotapublish\\").Replace("\\", "/");
        //        string IdentImages = LoanApps.STA_FilePath.After("h:\\root\\home\\paelyt-001\\www\\paelyt\\uvlotapublish\\").Replace("\\", "/");
        //        string IdentImagess = LoanApps.NyscpassportFilePath.After("h:\\root\\home\\paelyt-001\\www\\paelyt\\uvlotapublish\\").Replace("\\", "/");
        //        string IdentImgs= LoanApps.NyscPostingLetterFllePath.After("h:\\root\\home\\paelyt-001\\www\\paelyt\\uvlotapublish\\").Replace("\\", "/");
        //        string IdentImgss = LoanApps.NyscCallUpLetterFilePath.After("h:\\root\\home\\paelyt-001\\www\\paelyt\\uvlotapublish\\").Replace("\\", "/");
        //        string IdentImgsss = LoanApps.NyscProfileDashboardFilePath.After("h:\\root\\home\\paelyt-001\\www\\paelyt\\uvlotapublish\\").Replace("\\", "/");

        //        if (IdentImage != "" || IdentImages != "" || IdentImagess != "" || IdentImgs != "" || IdentImgs != "" || IdentImgsss != "")

        //        { 
        //            WebLog.Log("Image Path" + IdentImage);
        //        string slash = "/";
        //        Apploan.NyscIdCardFilePath = string.IsNullOrEmpty(IdentImage) ? "none" : slash + IdentImage;
        //        Apploan.STA_FilePath = string.IsNullOrEmpty(IdentImages) ? "none" : slash + IdentImages;
        //        Apploan.NyscpassportFilePath = string.IsNullOrEmpty(IdentImagess) ? "none" : slash + IdentImagess;
        //        Apploan.NyscPostingLetterFllePath = string.IsNullOrEmpty(IdentImgs) ? "none" : slash + IdentImgs;
        //        Apploan.NyscCallUpLetterFilePath = string.IsNullOrEmpty(IdentImgss) ? "none" : slash + IdentImgss;
        //        Apploan.NyscProfileDashboardFilePath = string.IsNullOrEmpty(IdentImgsss) ? "none" : slash + IdentImgsss;
        //        Apploan.ApplicationStatus_FK = LoanApp.ApplicationStatus_FK;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        WebLog.Log(ex.Message.ToString());
        //    }
        //}

        //disburse
        [HttpGet]
        public ActionResult DisburseLoan()

        {

            try
            {
                AppStatFk = 1;
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }

                ViewBag.Data = _DR.DisburseLoan(AppStatFk);
                GetMenus();
                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }





        [HttpGet]
        public ActionResult Disburse(string Refid, TableObjects.LoanApplication LoanApp, AppLoanss Apploan, AppLoanss LoanApps)

        {

            try
            {
                AppStatFk = 1;
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                var User = _DR.getUser(appUser);
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }
                if (Refid == null || Refid == "")
                {
                    return RedirectToAction("DisburseLoan");
                }

                var LoanAppss = _DR.GetNYSCLoanApplication(Refid, AppStatFk);
                if (LoanAppss == null)
                {
                    // return RedirectToAction("RecommendLoanSecondLevel");
                    TempData["Error"] = "Check The Status Of The Application";
                    return RedirectToAction("ApproveLoan");
                }

                GetApplicantInfos(Refid, LoanApp, Apploan, LoanAppss);
                TempData["Username"] = User.Firstname;
                TempData["LoanObj"] = Apploan;
                GetMenus();

                return View(Apploan);
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message, ex.StackTrace);
                return null;
            }
        }
        [HttpPost]
        public ActionResult Disburse(DataAccessA.Classes.AppLoanss Apploans, FormCollection form)
        {
            var id = Convert.ToInt16(form["ID"]);
            var Disburse = Convert.ToInt16(form["Accept"]);
            Apploans.ID = id;
            Apploans.ApplicationStatus_FK = Disburse;
            var resp = _DR.UpdateNyscLoanApplication(Apploans);
            return RedirectToAction("DisburseLoan");

        }
        public void GetApplicantInfos(string Refid, TableObjects.LoanApplication LoanApp, AppLoanss Apploan, AppLoanss LoanApps)
        {
            try
            {
                // AppLoan LoanApps = new AppLoan();
                Apploan.ID = LoanApp.ID;
                Apploan.Firstname = string.IsNullOrEmpty(LoanApps.Firstname) ? "none" : LoanApps.Firstname;
                Apploan.AccountName = string.IsNullOrEmpty(LoanApps.AccountName) ? "none" : LoanApps.AccountName;
                Apploan.AccountNumber = string.IsNullOrEmpty(LoanApps.AccountNumber) ? "none" : LoanApps.AccountNumber;
                Apploan.ApplicantID = string.IsNullOrEmpty(LoanApps.ApplicantID) ? "none" : LoanApps.ApplicantID;
                Apploan.BankCode = string.IsNullOrEmpty(LoanApps.BankCode) ? "none" : LoanApps.BankCode;
                Apploan.BVN = string.IsNullOrEmpty(LoanApps.BVN) ? "none" : LoanApps.BVN;
                Apploan.StateCode = string.IsNullOrEmpty(LoanApps.StateCode) ? "none" : LoanApps.StateCode;
                Apploan.DateOfBirth = string.IsNullOrEmpty(LoanApps.DateOfBirth) ? "none" : LoanApps.DateOfBirth;
                Apploan.PermanentAddress = string.IsNullOrEmpty(LoanApps.PermanentAddress) ? "none" : LoanApps.PermanentAddress;
                Apploan.Landmark = string.IsNullOrEmpty(LoanApps.Landmark) ? "none" : LoanApps.Landmark;
                Apploan.ClosestBusStop = string.IsNullOrEmpty(LoanApps.ClosestBusStop) ? "none" : LoanApps.ClosestBusStop;
                Apploan.LGAs = string.IsNullOrEmpty(LoanApps.LGAs) ? "none" : LoanApps.LGAs;
                Apploan.TempLGAs = string.IsNullOrEmpty(LoanApps.TempLGAs) ? "none" : LoanApps.TempLGAs;
                Apploan.NyscLGAs = string.IsNullOrEmpty(LoanApps.NyscLGAs) ? "none" : LoanApps.NyscLGAs;
                Apploan.NyscStateofResidence = string.IsNullOrEmpty(LoanApps.NyscStateofResidence) ? "none" : LoanApps.NyscStateofResidence;
                Apploan.TempStateofResidence = string.IsNullOrEmpty(LoanApps.TempStateofResidence) ? "none" : LoanApps.TempStateofResidence;
                Apploan.TemporaryAddress = string.IsNullOrEmpty(LoanApps.TemporaryAddress) ? "none" : LoanApps.TemporaryAddress;
                Apploan.StateofResidence = string.IsNullOrEmpty(LoanApps.StateofResidence) ? "none" : LoanApps.StateofResidence;
                Apploan.TempLandmark = string.IsNullOrEmpty(LoanApps.TempLandmark) ? "none" : LoanApps.TempLandmark;
                Apploan.TempClosestBusStop = string.IsNullOrEmpty(LoanApps.TempClosestBusStop) ? "none" : LoanApps.TempClosestBusStop;
                Apploan.Employer = string.IsNullOrEmpty(LoanApps.Employer) ? "none" : LoanApps.Employer;
                Apploan.OfficialAddress = string.IsNullOrEmpty(LoanApps.OfficialAddress) ? "none" : LoanApps.OfficialAddress;
                Apploan.PassOutMonth = string.IsNullOrEmpty(LoanApps.PassOutMonth) ? "none" : LoanApps.PassOutMonth;
                Apploan.CDSDay = string.IsNullOrEmpty(LoanApps.CDSDay) ? "none" : LoanApps.CDSDay;
                Apploan.CDSGroup = string.IsNullOrEmpty(LoanApps.CDSGroup) ? "none" : LoanApps.CDSGroup;

                Apploan.EmailAddress = string.IsNullOrEmpty(LoanApps.EmailAddress) ? "none" : LoanApps.EmailAddress;
                Apploan.ValueTime = string.IsNullOrEmpty(LoanApps.ValueTime) ? "none" : LoanApps.ValueTime;
                Apploan.ValueDate = string.IsNullOrEmpty(LoanApps.ValueDate) ? "none" : LoanApps.ValueDate;
                Apploan.Title = string.IsNullOrEmpty(LoanApps.Title) ? "none" : LoanApps.Title;
                Apploan.Surname = string.IsNullOrEmpty(LoanApps.Surname) ? "none" : LoanApps.Surname;
                Apploan.NigerianStates = string.IsNullOrEmpty(LoanApps.NigerianStates) ? "none" : LoanApps.NigerianStates;
                // Apploan.Repayment = string.IsNullOrEmpty(LoanApps.Repayment) ? "none" : LoanApps.Repayment;
                Apploan.PhoneNumber = string.IsNullOrEmpty(LoanApps.PhoneNumber) ? "none" : LoanApps.PhoneNumber;
                Apploan.Othernames = string.IsNullOrEmpty(LoanApps.Othernames) ? "none" : LoanApps.Othernames;
                //Apploan.Organization =
                //    string.IsNullOrEmpty(LoanApps.Organization) ? "none" : LoanApps.Organization;
                Apploan.EMG_Relationship =
                    string.IsNullOrEmpty(LoanApps.EMG_Relationship) ? "none" : LoanApps.EMG_Relationship;
                Apploan.EMG_PhoneNumber = string.IsNullOrEmpty(LoanApps.EMG_PhoneNumber) ? "none" : LoanApps.EMG_PhoneNumber;
                Apploan.EMG_HomeAddress = string.IsNullOrEmpty(LoanApps.EMG_HomeAddress) ? "none" : LoanApps.EMG_HomeAddress;
                Apploan.EMG_FullName = string.IsNullOrEmpty(LoanApps.EMG_FullName) ? "none" : LoanApps.EMG_FullName;
                Apploan.EMG_EmailAddress =
                    string.IsNullOrEmpty(LoanApps.EMG_EmailAddress) ? "none" : LoanApps.EMG_EmailAddress;
                //Apploan.MeansOfIdentifications =
                // string.IsNullOrEmpty(LoanApps.MeansOfIdentifications) ? "none" : LoanApps.MeansOfIdentifications;
                Apploan.MaritalStatus =
                    string.IsNullOrEmpty(LoanApps.MaritalStatus) ? "none" : LoanApps.MaritalStatus;
                Apploan.LoanTenure = Convert.ToInt32(LoanApps.LoanTenure);
                Apploan.LoanComment = string.IsNullOrEmpty(LoanApps.LoanComment) ? "none" : LoanApps.LoanComment;
                Apploan.LoanAmount = LoanApps.LoanAmount;
                string LoanAmt = Convert.ToString(Apploan.LoanAmount);
                Apploan.ConvertedAmount = MyUtility.ConvertToCurrency(LoanAmt);
                Apploan.LoanAmount = Convert.ToDouble(Apploan.LoanAmount);
                Apploan.Landmark =
                    string.IsNullOrEmpty(LoanApps.Landmark) ? "none" : LoanApps.Landmark;
                // Apploan.IdentficationNumber = string.IsNullOrEmpty(LoanApps.IdentficationNumber) ? "none" : LoanApps.IdentficationNumber;
                Apploan.ExistingLoan = LoanApps.ExistingLoan;

                Apploan.ExistingLoan_NoOfMonthsLeft = Convert.ToInt16(LoanApps.ExistingLoan_NoOfMonthsLeft);
                Apploan.ExistingLoan_OutstandingAmount = LoanApps.ExistingLoan_OutstandingAmount;
                Apploan.Firstname =
                    string.IsNullOrEmpty(LoanApps.Firstname) ? "none" : LoanApps.Firstname;
                Apploan.ID = LoanApps.ID;
                Apploan.LoanRefNumber =
                    string.IsNullOrEmpty(LoanApps.LoanRefNumber) ? "none" : LoanApps.LoanRefNumber;
                Apploan.ClosestBusStop =
               string.IsNullOrEmpty(LoanApps.ClosestBusStop) ? "none" : LoanApps.ClosestBusStop;
                Apploan.Department =
                string.IsNullOrEmpty(LoanApps.Department) ? "none" : LoanApps.Department;
                Apploan.Occupation =
               string.IsNullOrEmpty(LoanApps.Occupation) ? "none" : LoanApps.Occupation;
                Apploan.Gender = string.IsNullOrEmpty(LoanApps.Gender) ? "none" : LoanApps.Gender;
                // Apploan.EmployeeStatus = string.IsNullOrEmpty(LoanApps.EmployeeStatus) ? "none" : LoanApps.EmployeeStatus;
                //Apploan.ApplicationStatus = LoanApps.AppStat == 3 ? "Recommended" : "none" ;
                Apploan.Designation = string.IsNullOrEmpty(LoanApps.Designation) ? "none" : LoanApps.Designation;
                //Apploan.Salary = LoanApps.Salary;
                Apploan.NetMonthlyIncome = LoanApps.SalaryAmount;
                string SalAmt = Convert.ToString(Apploan.LoanAmount);
                Apploan.ConvertedAmount = MyUtility.ConvertToCurrency(SalAmt);

            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
            }
        }


        [HttpGet]
        public ActionResult RejectedApplications()
        {
            try
            {
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = "";
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("Index", "Home");
                }
                GetMenus();
                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        [HttpPost]
        public ActionResult RejectedApplications(FormCollection form)
        {
            try
            {
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = "";

                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                var User = _DR.getUser(user);
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }

                GetMenus();
                //ViewBag.ApplicationStatus = uvdb.ApplicationStatus.ToList();
                int AppStatFk = Convert.ToUInt16(form["ApplicationStatus"]);
                //AppStatFk = 1;
              var lstMenus = _DR.RejectedloanReport(AppStatFk);

                List<ExcelList> appLoanList = new List<ExcelList>();
                foreach (ExcelList app in lstMenus)
                {
                    app.ConvertedAmount = MyUtility.ConvertToCurrency(app.LoanAmount.ToString());
                    appLoanList.Add(app);
                }
                ViewBag.Data = appLoanList;



                Session["AllTransaction"] = ViewBag.Data;
                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }










        [HttpGet]
        public ActionResult ReferralApplications()
        {
            try
            {
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = "";
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("Index", "Home");
                }
                GetMenus();
                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        [HttpPost]
        public ActionResult ReferralApplications(FormCollection form)
        {
            try
            {
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = "";

                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                var User = _DR.getUser(user);
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }

                GetMenus();
                //ViewBag.ApplicationStatus = uvdb.ApplicationStatus.ToList();
                int AppStatFk = Convert.ToUInt16(form["ApplicationStatus"]);
                //AppStatFk = 1;
                var lstMenus = _DR.GetReferrals(AppStatFk);

               
                ViewBag.Data = lstMenus;



                Session["AllTransaction"] = ViewBag.Data;
                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


        [HttpGet]
        public ActionResult SpinTest()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        [HttpPost]
        public ActionResult SpinTest(FormCollection form)
        {
            try
            {
                return View();
            }
            catch(Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                    return null;
            }
        }

        [HttpGet]
        public ActionResult ReferralRecords()
        {
            try
            {


                TempData["SucMsg"] = "";  TempData["Error"] = "";
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                var User = _DR.getUser(user);
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }

                GetMenus();
               
                userid = _DR.getUserID(user);
                ViewBag.Data = _DR.MyReferralsdetails(userid);
               

                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        [HttpGet]
        public ActionResult RegisteredApplicant()
        {
            try
            {


                TempData["SucMsg"] = ""; TempData["Error"] = "";
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                var User = _DR.getUser(user);
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }

                GetMenus();

                //userid = _DR.getUserID(user);
                var recObj = _DR.RegisteredApplicant();
                List<AppLoanss> appLoanList = new List<AppLoanss>();
                foreach (AppLoanss app in recObj)
                {
                    app.ConvertedAmount = MyUtility.ConvertToCurrency(app.LoanAmount.ToString());
                    appLoanList.Add(app);
                }
                ViewBag.Data = appLoanList;
                Session["AllTransaction"] = ViewBag.Data;
                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


        //public ActionResult RegisteredReferral()
        //{
        //    try
        //    {


        //        TempData["SucMsg"] = ""; TempData["Error"] = "";
        //        var LoggedInuser = new LogginHelper();
        //        user = LoggedInuser.LoggedInUser();

        //        var appUser = user;
        //        var User = _DR.getUser(user);
        //        if (appUser == null)
        //        {
        //            return RedirectToAction("/", "Home");
        //        }

        //        GetMenus();

        //        //userid = _DR.getUserID(user);
        //        var recObj = _DR.GetReferrals();
        //        List<ReferralDetails> appLoanList = new List<ReferralDetails>();
        //        //foreach (ReferralDetails app in recObj)
        //        //{
        //        //    app.ConvertedAmount = MyUtility.ConvertToCurrency(app.LoanAmount.ToString());
        //        //    appLoanList.Add(app);
        //        //}
        //        ViewBag.Data = recObj;
        //        Session["AllTransaction"] = ViewBag.Data;
        //        return View();
        //    }
        //    catch (Exception ex)
        //    {
        //        WebLog.Log(ex.Message.ToString());
        //        return null;
        //    }
        //}












        [HttpGet]
        public ActionResult BVNReport()
        {
            try
            {


                TempData["SucMsg"] = ""; TempData["Error"] = "";
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                var User = _DR.getUser(user);
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }

                GetMenus();

               // userid = _DR.getUserID(user);
                ViewBag.Data = _DR.Mybvndetails();


                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


        [HttpGet]
        public ActionResult NYSCLoanSummary()
        {
            try
            {

                TempData["SucMsg"] = ""; TempData["Error"] = "";
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                var User = _DR.getUser(user);
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }

                GetMenus();

                ViewBag.Data = _DR.GetNYSCLoanApplicationSummary();
                
                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }
        
        [HttpGet]
        public ActionResult ReferralActivity()

        {
            try
            {
                
                TempData["SucMsg"] = ""; TempData["Error"] = "";
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                var User = _DR.getUser(user);
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }
                GetMenus();
                ViewBag.Data = _DR.GetReferralActivity();
                
                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }



        [HttpGet]
        public ActionResult NYSCDefaultLoans()

        {
            try
            {

                TempData["SucMsg"] = ""; TempData["Error"] = "";
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                var User = _DR.getUser(user);
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }
                GetMenus();
                ViewBag.Data = _DR.GetNYSCDefaultLoans();

                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        [HttpGet]
        public ActionResult DisbursedApplicantReport()

        {

            try
            {
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = ""; TempData["Error"] = "";
                AppStatFk = 5;
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }

                //var patnerCode = "FLUTTERWAVE";

                //var lstMenus = _DR.ApproveLoansRemFlu(AppStatFk,patnerCode); 
                var lstMenus = _DR.FourthApproval(AppStatFk);
                ViewBag.Data = lstMenus;
                Session["AllTransaction"] = ViewBag.Data;
                // Session["AllTransaction"] = ViewBag.Data;




                GetMenus();
                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }







        [HttpGet]
        public ActionResult ReferredApplicants(string Refid)

        {
            try
            {
                TempData["SucMsg"] = ""; TempData["Error"] = "";
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                var User = _DR.getUser(user);
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }
                if (Refid == null || Refid == "")
                {
                    return RedirectToAction("ReferralActivity");
                }
                GetMenus();
                var recObj = _DR.GetReferredApp(Refid);
                List<AppLoanss> appLoanList = new List<AppLoanss>();
                foreach (AppLoanss app in recObj)
                {
                    app.ConvertedAmount = MyUtility.ConvertToCurrency(app.LoanAmount.ToString());
                    appLoanList.Add(app);
                }
                ViewBag.Data = appLoanList;

                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


        [HttpGet]
        public ActionResult ApprovalTime(/*string Refid*/)

        {
            try
            {
                TempData["SucMsg"] = ""; TempData["Error"] = "";
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                var User = _DR.getUser(user);
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }
                //if (Refid == null || Refid == "")
                //{
                //    return RedirectToAction("ReferralActivity");
                //}
                GetMenus();
              var recObj = _DR.GetKPITime();
                List<NyscLoanApplication> appLoanList = new List<NyscLoanApplication>();
                foreach (NyscLoanApplication app in recObj)
                {
                    var ConvertedAmount = MyUtility.ConvertToCurrency(app.LoanAmount.ToString());
                    appLoanList.Add(app);
                }
                ViewBag.Data = appLoanList;

                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }
        

        [HttpGet]
        public ActionResult ApprovalTimess(int Refid)

        {
            try
            {
                TempData["SucMsg"] = ""; TempData["Error"] = "";
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                var User = _DR.getUser(user);
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }
                if (Refid == 0 )
                {
                    return RedirectToAction("ReferralActivity");
                }
                GetMenus();
                var recObj = _DR.GetKpiInMinutes(Refid);
                List<double> KpiMinutes = new List<double>();

                foreach (double app in recObj)
                {
                    KpiMinutes.Add(app);
                }
                ViewBag.Data = KpiMinutes;


                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }
        [HttpGet]
        public ActionResult LoanDueForDebit()



        {
            try
            {
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = ""; TempData["Error"] = "";
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                var User = _DR.getUser(user);
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }
                GetMenus();
                return View();
            }
            catch(Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }






        [HttpPost]
        public ActionResult LoanDueForDebit(DataAccessA.Classes.AppLoans lApObj)

        {
            try
            {


                TempData["SucMsg"] = ""; TempData["ErrMsg"] = ""; TempData["Error"] = "";
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                var User = _DR.getUser(user);
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }
                var DOB = lApObj.Enddate;
             
                GetMenus();
                ViewBag.Data = _DR.LoanDueForDebits(DOB);

                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


        [HttpGet]
        public ActionResult ApplicantRelated()



        {
            try
            {
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = ""; TempData["Error"] = "";
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                var User = _DR.getUser(user);
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }
                GetMenus();
                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }






        [HttpPost]
        public ActionResult ApplicantRelated(DataAccessA.Classes.AppLoans lApObj)

        {
            try
            {


                TempData["SucMsg"] = ""; TempData["ErrMsg"] = ""; TempData["Error"] = "";
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                var User = _DR.getUser(user);
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }
                var DOB = lApObj.Startdate;
                var DOBs = lApObj.Enddate;

                GetMenus();
                ViewBag.Data = _DR.ApplicantTRelated(DOB, DOBs);

                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }











        [HttpGet]
        public ActionResult OutStandingLoan()



        {
            try
            {
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = ""; TempData["Error"] = "";
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                var User = _DR.getUser(user);
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }
                GetMenus();
                Session["AllTransaction"] = ViewBag.Data;
                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }






        [HttpPost]
        public ActionResult OutStandingLoan(DataAccessA.Classes.AppLoans lApObj)

        {
            try
            {


                TempData["SucMsg"] = ""; TempData["ErrMsg"] = ""; TempData["Error"] = "";
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                var User = _DR.getUser(user);
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }
                var DOB = lApObj.Enddate;

                GetMenus();
                ViewBag.Data = _DR.OutStandingLoans(DOB);
                Session["AllTransaction"] = ViewBag.Data;
                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }







        [HttpGet]
        public ActionResult Repayment()



        {
            try
            {
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = ""; TempData["Error"] = "";
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                var User = _DR.getUser(user);
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }
                GetMenus();
                Session["AllTransaction"] = ViewBag.Data;
                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }






        [HttpPost]
        public ActionResult Repayment(DataAccessA.Classes.AppLoans lApObj)

        {
            try
            {


                TempData["SucMsg"] = ""; TempData["ErrMsg"] = ""; TempData["Error"] = "";
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                var User = _DR.getUser(user);
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }
                var DOBS = lApObj.Startdate;

              var DOB = lApObj.Enddate;

                GetMenus();
                ViewBag.Data = _DR.Repayment(DOB, DOBS);
                Session["AllTransaction"] = ViewBag.Data;
                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }




        [HttpGet]
        public ActionResult RevenueReceived()



        {
            try
            {
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = ""; TempData["Error"] = "";
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                var User = _DR.getUser(user);
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }
                GetMenus();
                Session["AllTransaction"] = ViewBag.Data;
                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }






        [HttpPost]
        public ActionResult RevenueReceived(DataAccessA.Classes.AppLoans lApObj)

        {
            try
            {


                TempData["SucMsg"] = ""; TempData["ErrMsg"] = ""; TempData["Error"] = "";
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                var User = _DR.getUser(user);
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }
                var DOB = lApObj.Enddate;

                GetMenus();
                ViewBag.Data = _DR.RevenueReceived(DOB);

                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }





        [HttpGet]
        public ActionResult RevenueEarned()



        {
            try
            {
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = ""; TempData["Error"] = "";
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                var User = _DR.getUser(user);
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }
                GetMenus();
                Session["AllTransaction"] = ViewBag.Data;
                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }






        [HttpPost]
        public ActionResult RevenueEarned(DataAccessA.Classes.AppLoans lApObj)

        {
            try
            {


                TempData["SucMsg"] = ""; TempData["ErrMsg"] = ""; TempData["Error"] = "";
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                var User = _DR.getUser(user);
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }
                var DOB = lApObj.Enddate;

                GetMenus();
                ViewBag.Data = _DR.RevenueEarned(DOB);
                Session["AllTransaction"] = ViewBag.Data;
                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


        [HttpGet]
        public ActionResult To50RefferalPerformance()



        {
            try
            {
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = ""; TempData["Error"] = "";
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                var User = _DR.getUser(user);
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }
                GetMenus();
                ViewBag.Data = _DR.Top50ReferralPerformance();

                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        [HttpGet]
        public ActionResult AgentPerformance()



        {
            try
            {
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = ""; TempData["Error"] = "";
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                var User = _DR.getUser(user);
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }
                GetMenus();
                ViewBag.Data = _DR.ReferralAgentPerformance();

                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }



        [HttpGet]
        public ActionResult DisbursedLoansRep()



        {
            try
            {
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = ""; TempData["Error"] = "";
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                var User = _DR.getUser(user);
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }
                GetMenus();
                ViewBag.Data = _DR.DisbursedLoans();
                Session["AllTransaction"] = ViewBag.Data;
                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


        [HttpGet]
        public ActionResult BorrowedLoan()



        {
            try
            {
                TempData["SucMsg"] = ""; TempData["ErrMsg"] = ""; TempData["Error"] = "";
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                var User = _DR.getUser(user);
                if (appUser == null)
                {
                    return RedirectToAction("/", "Home");
                }
                GetMenus();
                ViewBag.Data = _DR.BorrowedLoan();
                Session["AllTransaction"] = ViewBag.Data;
                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }





        //[HttpGet]
        //public ActionResult MyCommisions()
        //{
        //    try
        //    {


        //        TempData["SucMsg"] = ""; TempData["Error"] = "";
        //        var LoggedInuser = new LogginHelper();
        //        user = LoggedInuser.LoggedInUser();

        //        var appUser = user;
        //        var User = _DR.getUser(user);
        //        if (appUser == null)
        //        {
        //            return RedirectToAction("/", "Home");
        //        }

        //        GetMenus();
        //        userid = _DR.getUserID(user);
        //        var Records = _DR.getCommisioRecords(userid);
        //        var resp = Comisionsum(Records);
        //        double TotalRepayment = Convert.ToDouble(resp);
        //        TotalRepayment = Math.Round(TotalRepayment);
        //        ViewBag.Balance = TotalRepayment;

        //        return View();
        //    }
        //    catch (Exception ex)
        //    {
        //        WebLog.Log(ex.Message.ToString());
        //        return null;
        //    }
        //}



        //[HttpPost]
        //public ActionResult Approve(FormCollection form, TableObjects.LoanApplication LoanApps, DataAccessA.Classes.AppLoanss LoanApp)
        //{
        //    try
        //    {
        //        double InterestValue = 0;
        //        double PoratedAmt = 0;
        //        LoansLedger Ledger = new LoansLedger();
        //        var LoggedInuser = new LogginHelper();
        //        user = LoggedInuser.LoggedInUser();
        //        var userid = LoggedInuser.LoggedInUserID(user);
        //        var appUser = user;
        //        if (appUser == null)
        //        {
        //            return RedirectToAction("/", "Home");
        //        }
        //        LoanApp = (DataAccessA.Classes.AppLoanss)TempData["LoanObj"];

        //        DataAccessA.Classes.LoanApplication LoanAp = new DataAccessA.Classes.LoanApplication();
        //        DataAccessA.DataManager.LoanApproval LoanApproval = new DataAccessA.DataManager.LoanApproval();
        //        LoanApproval.Comment = Convert.ToString(form["comment"]);
        //        LoanApproval.CommentBy = Convert.ToInt16(userid);
        //        var ido = Convert.ToString(form["Accept"]);
        //        LoanApproval.ApplicationStatus_FK = Convert.ToInt32(form["Accept"]);
        //        LoanApproval.DateCreated = MyUtility.getCurrentLocalDateTime();
        //        LoanApproval.ValueDate = LoanApp.ValueDate;
        //        LoanApproval.ValueTime = LoanApp.ValueTime;
        //        LoanApproval.LoanApplication_FK = LoanApp.ID;
        //        LoanApproval.IsVisible = 1;
        //        LoanAp.LoanRefNumber = LoanApp.LoanRefNumber;
        //        LoanAp.ApplicationStatus_FK = Convert.ToInt32(form["Accept"]);
        //        LoanAp.LoanComment = Convert.ToString(form["comment"]);
        //        LoanAp.DateModified = MyUtility.getCurrentLocalDateTime();

        //        if (LoanApproval.ApplicationStatus_FK == 5)
        //        {
        //            //_DR.



        //(LoanAp);
        //            TempData["ErrMsg"] = LoanAp.LoanComment;
        //            return RedirectToAction("Reject");
        //        }
        //        AdminAController Ln = new AdminAController();

        //        // var salary = _DR.GetSalary(LoanApp.ID);
        //        if (salary == null)
        //        {
        //            TempData["ErrMsg"] = "Please Check The Salary";
        //            RedirectToAction("Reject");
        //        }
        //        var LoanAmount = Convert.ToDouble(LoanApp.LoanAmount);
        //        var salaryAmount = Convert.ToDouble(salary.NetMonthlyIncome);
        //        var LoanLegder = Ln.PayrollLoanCalculation(salaryAmount, LoanAmount, LoanApp.LoanTenure, out repayment, out respMessage);


        //        if (respMessage == "0")
        //        {
        //            int i;
        //            var CurrentDate = MyUtility.getCurrentLocalDateTime();
        //            var Dayval = CurrentDate.Day;
        //            int lastday = DateTime.DaysInMonth(CurrentDate.Year, CurrentDate.Month);
        //            int RemainingDays = lastday - Dayval;
        //            if (RemainingDays > 10)
        //            {
        //                PoratedAmt = 0;
        //            }
        //            if (RemainingDays <= 10)

        //            {
        //                var InterestRate = _DR.getInterestRate(LoanApp.LoanTenure);
        //                if (RemainingDays != 0)
        //                {

        //                    InterestRate = InterestRate / 100;
        //                    InterestValue = InterestRate * repayment;
        //                    PoratedAmt = (double)RemainingDays / lastday;
        //                    PoratedAmt = (double)PoratedAmt * InterestValue;
        //                    PoratedAmt = Math.Round(PoratedAmt, 3);
        //                }
        //            }

        //            for (i = 1; i <= LoanApp.LoanTenure; i++)
        //            {

        //                Ledger.ApplicantID = LoanApp.ApplicantID;
        //                Ledger.RefNumber = LoanApp.LoanRefNumber;
        //                Ledger.IsVisible = 1;
        //                Ledger.trnDate = MyUtility.getCurrentLocalDateTime().AddMonths(i);
        //                Ledger.Credit = 0;
        //                if (i == 1)
        //                {
        //                    Ledger.Debit = repayment + PoratedAmt;
        //                }
        //                if (i > 1)
        //                {
        //                    Ledger.Debit = repayment;
        //                }
        //                Ledger.Institution_FK = LoanAp.Institution_FK;
        //                Ledger.LastUpdated = MyUtility.getCurrentLocalDateTime();
        //                _DM.InsertLoansLedger(Ledger);

        //            }
        //            //_DM.UpdateLoanApp(LoanApp.LoanRefNumber, Ledger.trnDate.Value);
        //            var recommend = _DM.UpdateLoanApplication(LoanAp);
        //            if (recommend > 0)
        //            {
        //                // LoanApproval = _DM.UpdateLoanApproval(LoanApproval,LoanAp);
        //                LoanApproval = _DM.CreateLoanApproval(LoanApproval);
        //                if (LoanApproval.ID > 0)
        //                {
        //                    // For Sending EMails to Customers
        //                    lc.SendEmail(LoanApp);
        //                    TempData["Offer"] = "0";
        //                    return RedirectToAction("Acknowledgement", new { @Refid = LoanApp.LoanRefNumber });
        //                    // return RedirectToAction("Acknowledgement", new { @Refid = LoanApp.LoanRefNumber });

        //                }
        //                else
        //                {
        //                    TempData["Error"] = "Error Approving Loan! Try Again";
        //                    return RedirectToAction("ApproveLoan", new { @Refid = LoanApp.LoanRefNumber });
        //                }
        //            }
        //            else
        //            {
        //                TempData["Error"] = "Error Approving Loan! Try Again";
        //                return RedirectToAction("ApproveLoan", new { @Refid = LoanApp.LoanRefNumber });
        //            }
        //        }
        //        else if (respMessage != "0")
        //        {
        //            TempData["Error"] = respMessage;
        //            return RedirectToAction("Approve", new { @Refid = LoanApp.LoanRefNumber });

        //        }

        //        return View();
        //    }
        //    catch (Exception ex)
        //    {
        //        return null;
        //    }
        //}


        //public bool PayrollLoanCalculation(double salaryAmt, double LoanAmt, int Tenure, out double repaymentAmount, out string respMessage)
        //{
        //    repaymentAmount = 0;
        //    respMessage = "";
        //    try
        //    {
        //        //since 250/100 is 0.4 divide by salary to get RealLoanAmt
        //        var maxLoanAllowed = salaryAmt * 2.5;
        //        // 60% salary
        //        var sixtyPercentSal = (0.67 * salaryAmt);///two third of the salary fpr repaYMENT
        //        if (LoanAmt > maxLoanAllowed)
        //        {
        //            respMessage = "Invalid Loan Amount! You cannot apply for more than 2.5x your salary";
        //            return false;
        //        }
        //        var LoanRate = db.GetRate(Tenure);
        //        if (LoanRate == null)
        //        {
        //            respMessage = "Invalid Tenure";
        //            return false;
        //        }
        //        if (LoanRate.LoanTenure > 12)
        //        {
        //            respMessage = "Invalid Tenure: Tenure cannot be greater than 12 months";
        //            return false;
        //        }
        //        var LI = LoanRate.InterestRate / 100;
        //        repaymentAmount = (LoanAmt + (LoanAmt * Tenure) * LI) / Tenure;
        //        if (repaymentAmount >= sixtyPercentSal)
        //        {
        //            respMessage = "Invalid Loan Amount: Monthly repayment amount cannot be greater than 2/3 your salary";
        //            return false;
        //        }
        //        else
        //        {
        //            respMessage = "0";
        //            return true;
        //        }


        //    }
        //    catch (Exception ex)
        //    {
        //        WebLog.Log(ex.Message.ToString());
        //        return false;
        //    }
        //

    }


}


