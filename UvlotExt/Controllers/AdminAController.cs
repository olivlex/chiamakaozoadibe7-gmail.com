
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
            var referalCode = User.MyReferralCode;
            TempData["referalCode"] = referalCode;
            GetMenus();
            return View();
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


        public ActionResult BVNValidation()
        {
            return View();
        }
        [HttpPost]
        public ActionResult BVNValidation(FormCollection form, BVNC bvnc)
        {

            string bvn = Convert.ToString(form["BVN"]) == null ? "" : Convert.ToString(form["BVN"]);

            Helper helper = new Helper();
            bvnc = helper.BVNValidationResps(bvn);

            //Insert BVN details here

            DataWriter.SaveBVNDetails(bvnc);

            if (bvnc.respCode == "00")
            {
                return View("BVNValidation", bvnc);

            }
            else
            {
                ViewBag.Data = bvnc.errormessage;
                return View("BVNValidation");
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
                var Menus = results.ToList().Distinct().GroupBy(k => (k.pageName)).OrderBy(k => k.Key).ToDictionary(k => k.Key, v => v.ToList());

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
        [HttpGet]
        public ActionResult CheckAppStatus()
        {
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

        //[HttpGet]
        //public ActionResult AllNYSCLoanReport()
        //{
        //    GetMenus();
        //    ViewBag.Data = _DR.NYSCLoanAppReport();

        //    return View();
        //}


        //[HttpPost]
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
        public ActionResult CheckNYSCLoan()
        {
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
               
                //var LoggedInuser = new LogginHelper();
                //user = LoggedInuser.LoggedInUser();

                //var appUser = user;
                //if (appUser == null)
                //{
                //    return RedirectToAction("/", "Home");
                //}

                var ApplicationFk = Convert.ToString(form["RefNumber"]);

                DataAccessA.Classes.AppLoanss Apploan = _DR.CheckAppStatus(ApplicationFk);

                //string LoanAmt = Convert.ToString(Apploan.LoanAmount);
                //Apploan.ConvertedLoanAmt = utilities.ConvertToCurrency(LoanAmt);
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
        public ActionResult Approve(string Refid, TableObjects.LoanApplication LoanApp, AppLoanss Apploan, AppLoanss LoanApps)

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

                GetApplicantInfo(Refid, LoanApp, Apploan, LoanAppss);
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
        public ActionResult Approve(DataAccessA.Classes.AppLoanss Apploans, FormCollection form)
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
                var sendMail = NotificationService.SendMailOut(msgHeader, bodyTxt, apObj.EmailAddress, null, null);
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



public ActionResult SecondApproval(string Refid, TableObjects.LoanApplication LoanApp, AppLoanss Apploan, AppLoanss LoanApps)
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

                GetApplicantInfo(Refid, LoanApp, Apploan, LoanAppss);
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
        public ActionResult ThirdApproval(string Refid, TableObjects.LoanApplication LoanApp, AppLoanss Apploan, AppLoanss LoanApps)
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


                GetApplicantInfo(Refid, LoanApp, Apploan, LoanAppss);
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
        public ActionResult ApprovedThirdLoan(string Refid, TableObjects.LoanApplication LoanApp, AppLoanss Apploan, AppLoanss LoanApps)

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

                GetApplicantInfo(Refid, LoanApp, Apploan, LoanAppss);
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
        public ActionResult FourthApproval(string Refid, TableObjects.LoanApplication LoanApp, AppLoanss Apploan, AppLoanss LoanApps)
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
                


                GetApplicantInfo(Refid, LoanApp, Apploan, LoanAppss);
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

                NYSCReferralLedger RL = new NYSCReferralLedger();
                User us = new User();
                  
                RL.TrnDate = MyUtility.getCurrentLocalDateTime();
                RL.ReferenceNumber = Apploans.LoanRefNumber;
                RL.User_FK = us.ID;
                RL.IsVisible = 1;
                RL.Debit = 0;
                RL.Credit = Convert.ToDouble(amount); ;
                RL.ValueDate = MyUtility.getCurrentLocalDateTime().ToString("yyyy/MM/dd");
                RL.ValueTime = MyUtility.getCurrentLocalDateTime().ToString("H:mmss");
                RL.ReferralCode = us.ReferralCode;
                
               
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
        public ActionResult ApprovedFinalLevel(string Refid, TableObjects.LoanApplication LoanApp, AppLoanss Apploan, AppLoanss LoanApps)

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

                GetApplicantInfo(Refid, LoanApp, Apploan, LoanAppss);
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
        public ActionResult ApprovedSecondLoan(string Refid, TableObjects.LoanApplication LoanApp, AppLoanss Apploan, AppLoanss LoanApps)

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

                GetApplicantInfo(Refid, LoanApp, Apploan, LoanAppss);
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
            ExportToExcel2();
            GetMenus();
            return View("AllApprovedLoan");
            //return View("");
        }

        public void ExportToExcel2()
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
        public ActionResult ApprovedLoan(string Refid, TableObjects.LoanApplication LoanApp, AppLoanss Apploan, AppLoanss LoanApps)

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

                GetApplicantInfo(Refid, LoanApp, Apploan, LoanAppss);
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
        public void GetApplicantInfo(string Refid, TableObjects.LoanApplication LoanApp, AppLoanss Apploan, AppLoanss LoanApps)
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
                // Apploan.LoanComment = string.IsNullOrEmpty(LoanApps.LoanComment) ? "none" : LoanApps.LoanComment;
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
                Apploan.NyscIdCardFilePath = LoanApps.NyscIdCardFilePath;
                Apploan.STA_FilePath = LoanApps.STA_FilePath;
                Apploan.ReferralCode = string.IsNullOrEmpty(LoanApps.ReferralCode) ? "none" : LoanApps.ReferralCode;
                //string IdentImage = LoanApps.NyscIdCardFilePath.After("C:\\Users\\PAELYT SOLUTION 6\\Documents\\Visual Studio 2015\\Projects\\UvlotExt\\UvlotExt\\UvlotExt\\").Replace("\\", "/");
                //string IdentImages = LoanApps.STA_FilePath.After("C:\\Users\\PAELYT SOLUTION 6\\Documents\\Visual Studio 2015\\Projects\\UvlotExt\\UvlotExt\\UvlotExt\\").Replace("\\", "/");

                string IdentImage = LoanApps.NyscIdCardFilePath.After("h:\\root\\home\\paelyt-001\\www\\paelyt\\uvlotapublish\\").Replace("\\", "/");
                string IdentImages = LoanApps.STA_FilePath.After("h:\\root\\home\\paelyt-001\\www\\paelyt\\uvlotapublish\\").Replace("\\", "/");
                //h:\root\home\paelyt - 001\www\paelyt\uvlotapublish\Images\PowerScreenGrab.PNG
                //string IdentImage = LoanApps.IdentficationNumberImage.After("h:\\root\\home\\paelyt-001\\www\\Uvlot\\").Replace("\\", "/");
                WebLog.Log("Image Path" + IdentImage);
                string slash = "/";
                Apploan.NyscIdCardFilePath = string.IsNullOrEmpty(IdentImage) ? "none" : slash + IdentImage;
                Apploan.STA_FilePath = string.IsNullOrEmpty(IdentImages) ? "none" : slash + IdentImages;
                Apploan.ApplicationStatus_FK = LoanApp.ApplicationStatus_FK;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
            }
        }

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


