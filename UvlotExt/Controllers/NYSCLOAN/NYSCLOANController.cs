using DataAccessA;
using DataAccessA.Classes;
using DataAccessA.DataManager;
using ExcelUpload.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using UvlotApp.HelperClasses;
using UvlotApplication.Classes;
using UvlotExt.Classes;

namespace UvlotExt.Controllers.NYSCLOAN
{
    public class NYSCLOANController : Controller
    {

        DataWriter _DM = new DataWriter();
        DataReader _DR = new DataReader();
        //int AppStatFk = 0;
        // GET: NYSCLOAN
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult GetLoanParam(string Tenure)
        {
            try
            {
                Institution Inst = new Institution();
                string respmsg = ""; ;
                // double repaymentmsg = 0;
                bool valid = false;


                int Tenures = Convert.ToInt16(Tenure);

                Tenures = Tenures + 1;
                //Tenures = Tenures ;//TO DO 
                // List<LRepay> LoanRecords = _DR.GetloanParam(LoanTenure);
                NYSCLoanSetUp Nysc = new NYSCLoanSetUp();
                Nysc = _DR.GetloanParam(Tenures);
                if (Nysc != null)
                {
                    valid = true;
                    //  return valid ;
                }
                else
                {
                    valid = false;
                }
                return Json(new { response = valid, Data = respmsg, Loanrec = Nysc });
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

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

        public ActionResult Acknowledgement(string Refid)
        {
            // MyUtility utilities = new MyUtility();
            if (Refid == null || Refid == "")
            {
                return RedirectToAction("/");
            }
            var LoanApps = _DR.LoanDetails(Refid);
            string LoanAmount = LoanApps.LoanAmount.ToString();
            LoanApps.ConvertedAmount = MyUtility.ConvertToCurrency(LoanAmount);
            LoanApps.RepaymentAmount = _DR.GetRepaymenrAmount(LoanApps.LoanTenure);
            LoanApps.RepaymentAmount = MyUtility.ConvertToCurrency(LoanApps.RepaymentAmount);
            if (LoanApps == null)
            {
                return RedirectToAction("/");
            }

            return View(LoanApps);
        }

        [HttpGet]
        public ActionResult testDare()
        {

            return View();
        }

        [HttpGet]
        public ActionResult NYSCLoanAppForm(DataAccessA.Classes.LoanApplication laobj)
        {

            WebLog.Log("ENTERED");
            TempData["Error"] = "";
            string referralCode = Request.QueryString["referralCode"];
            WebLog.Log("refelcode" + referralCode);
            if (referralCode != null || referralCode != "")
            {
                WebLog.Log("refelcode0" + referralCode);
                laobj.ReferalCode = referralCode;
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
            int val = 0;
            ViewData["nTitles"] = new SelectList(_DR.GetTitles(), "ID", "NAME", val);
            ViewData["nMarital"] = new SelectList(_DR.GetMaritalStatus(), "ID", "NAME", val);
            ViewData["nLGAs"] = new SelectList(_DR.GetAllLGAs(), "ID", "NAME", val);

            ViewData["nAccomodationTypes"] = new SelectList(_DR.GetAccomodationTypes(), "ID", "NAME", val);
            ViewData["nMeansOfIDs"] = new SelectList(_DR.GetMeansOfIdentifications(), "ID", "NAME", val);
            ViewData["nStates"] = new SelectList(_DR.GetNigerianStates(), "ID", "NAME", val);
            ViewData["nBanks"] = new SelectList(_DR.GetBanks(), "FlutterWaveBankCode", "NAME", val);
            ViewData["nRepmtMethods"] = new SelectList(_DR.GetRepaymentMethods(), "ID", "NAME", val);
            ViewData["nLoanTenure"] = new SelectList(_DR.GetAllTenure(), "ID", "Tenure", val);
            ViewData["nGender"] = new SelectList(GetAllGender(), "Value", "Text", val);
            ViewData["nCDDays"] = new SelectList(GetDayOfTheWeek(), "Value", "Text", val);
            ViewData["nPassOutMonths"] = new SelectList(GetPassOutMonths(), "Value", "Text", val);
            ViewData["nemploymentStatus"] = new SelectList(GetAppStatus(), "Value", "Text", val);
            WebLog.Log("refelcode4" + referralCode);
            var channels = _DR.GetMarketChannel();
            ViewBag.channel = channels;
            return View(laobj);
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

        [HttpPost]
        public ActionResult NYSCLoanAppForm(FormCollection form, HttpPostedFileBase PostedFile, DataAccessA.Classes.LoanApplication lApObj, HttpPostedFileBase NyscIDCard, HttpPostedFileBase StatementOfAccount)
        {
            try
            {
                var channellist = Request["checkboxName"];
              
                if(channellist == null)
                {
                    TempData["Error"] = "Please Select Marketing Channel";
                    // NYSCLoanAppForm();
                    ViewData["nTitles"] = new SelectList(_DR.GetTitles(), "ID", "NAME", lApObj.Title_FK);
                    ViewData["nMarital"] = new SelectList(_DR.GetMaritalStatus(), "ID", "NAME", lApObj.MaritalStatus_FK);
                    ViewData["nLGAs"] = new SelectList(_DR.GetAllLGAs(), "ID", "NAME", lApObj.LGA_FK);
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
                var dCDSDay = Convert.ToString(form["selectDays"]);
                int LoanTenure = Convert.ToInt16(lApObj.LoanTenure);
                //  LoanTenure = LoanTenure;
                // lApObj.ValueDate = MyUtility.getCurrentLocalDateTime().ToString("dd/MM/yyyy H:mm ss");
                //lApObj.RepaymentAmount= _DR.GetRepaymenrAmount(LoanTenure);
                // lApObj.LoanTenureStr = LoanTenure.ToString() + " months";
                cPassOutMonth = lApObj.PassOutMonth;
                dCDSDay = lApObj.CDSDay;
                var Marital = lApObj.MaritalStatus_FK;
                var Tittle = lApObj.Title_FK;

                var lgaList = Convert.ToInt16(form["lgaList"]);
                var lgaLists = Convert.ToInt16(form["lgaLists"]);
                var lgaListss = Convert.ToInt16(form["lgaListsss"]);


                string NyscIDCards = saveImage(NyscIDCard);
                WebLog.Log("nyscPath Path" + NyscIDCards);
                string StatementOfAccounts = saveImages(StatementOfAccount);
                WebLog.Log("StatementOfAccounts Path" + StatementOfAccounts);

                if (NyscIDCard.ContentLength == 0)
                {
                    TempData["Error"] = "Input Statement Of Accounts";
                    // NYSCLoanAppForm();
                    ViewData["nTitles"] = new SelectList(_DR.GetTitles(), "ID", "NAME", lApObj.Title_FK);
                    ViewData["nMarital"] = new SelectList(_DR.GetMaritalStatus(), "ID", "NAME", lApObj.MaritalStatus_FK);
                    ViewData["nLGAs"] = new SelectList(_DR.GetAllLGAs(), "ID", "NAME", lApObj.LGA_FK);

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

                if (StatementOfAccount.ContentLength == 0)
                {
                    TempData["Error"] = "Input Nysc ID Cards";
                    // NYSCLoanAppForm();
                    ViewData["nTitles"] = new SelectList(_DR.GetTitles(), "ID", "NAME", lApObj.Title_FK);
                    ViewData["nMarital"] = new SelectList(_DR.GetMaritalStatus(), "ID", "NAME", lApObj.MaritalStatus_FK);
                    ViewData["nLGAs"] = new SelectList(_DR.GetAllLGAs(), "ID", "NAME", lApObj.LGA_FK);

                    ViewData["nAccomodationTypes"] = new SelectList(_DR.GetAccomodationTypes(), "ID", "NAME", lApObj.AccomodationType_FK);
                    ViewData["nMeansOfIDs"] = new SelectList(_DR.GetMeansOfIdentifications(), "ID", "NAME", lApObj.MeansOfID_FK);
                    ViewData["nStates"] = new SelectList(_DR.GetNigerianStates(), "ID", "NAME", lApObj.StateofResidence_FK);
                    ViewData["nBanks"] = new SelectList(_DR.GetBanks(), "FlutterWaveBankCode", "NAME", lApObj.Bank_FK);
                    ViewData["nRepmtMethods"] = new SelectList(_DR.GetRepaymentMethods(), "ID", "NAME", lApObj.RepaymentMethod_FK);
                    ViewData["nLoanTenure"] = new SelectList(_DR.GetAllTenure(), "ID", "Tenure", lApObj.LoanTenure);
                    ViewData["nGender"] = new SelectList(GetAllGender(), "Value", "Text", lApObj.Gender_FK);
                    ViewData["nemploymentStatus"] = new SelectList(GetAppStatus(), "Value", "Text", lApObj.Contract);
                    ViewData["nCDDays"] = new SelectList(GetDayOfTheWeek(), "Value", "Text",lApObj.CDSDay);
                    var channels = _DR.GetMarketChannel();
                    ViewBag.channel = channels;
                    return View("NYSCLoanAppForm", lApObj);
                }


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
                    // NYSCLoanAppForm();
                    ViewData["nTitles"] = new SelectList(_DR.GetTitles(), "ID", "NAME", lApObj.Title_FK);
                    ViewData["nMarital"] = new SelectList(_DR.GetMaritalStatus(), "ID", "NAME", lApObj.MaritalStatus_FK);
                    ViewData["nLGAs"] = new SelectList(_DR.GetAllLGAs(), "ID", "NAME", lApObj.LGA_FK);

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
                    AccountNumber = lApObj.AccountNumber,
                    AccountName = lApObj.AccountName,
                    Firstname = lApObj.Firstname,
                    Othernames = lApObj.Othernames,
                    NYSCApplicationStatus_FK = 1,
                    NyscIdCardFilePath = NyscIDCards,
                    STA_FilePath = StatementOfAccounts,
                    RepaymentAmount = repaymentAmt,
                    RefNumber = "NY" + MyUtility.GenerateRefNo(),
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
                    LoanAmount = Convert.ToDouble(lApObj.LoanAmount),
                    LoanTenure = LoanTenure,
                    ExistingLoan = lApObj.ExistingLoan,
                    LoanComment = lApObj.LoanComment,
                    ExistingLoan_NoOfMonthsLeft = Convert.ToInt16(lApObj.ExistingLoan_NoOfMonthsLeft),
                    ExistingLoan_OutstandingAmount = lApObj.ExistingLoan_OutstandingAmount,
                    BankCode =Helper.GetRemitaBankCodeByFlutterCode(lApObj.BankCode),  
                    IsVisible = 1,
                    DateCreated = MyUtility.getCurrentLocalDateTime(),
                    DateModified = MyUtility.getCurrentLocalDateTime(),
                    //ValueDate = MyUtility.getCurrentLocalDateTime().ToString("yyyy/MM/dd"),
                    ValueDate = MyUtility.getCurrentLocalDateTime().ToString("dddd, dd MMMM yyyy"),
                    ValueTime = MyUtility.getCurrentLocalDateTime().ToString("H:mmss"),
                    MarketingChannel = chanList.ToString(),
                };


                WebLog.Log("nyscPath Path 2" + NyscIDCards);


                WebLog.Log("StatementOfAccounts Path 2" + StatementOfAccounts);
                var id = DataWriter.CreateNYSCLoanApplication(NyscLA);

                if (id == 0)
                {
                    TempData["Error"] = "Please Check Next Of kin Phone Number";
                    //NYSCLoanAppForm();
                    ViewData["nTitles"] = new SelectList(_DR.GetTitles(), "ID", "NAME", lApObj.Title_FK);
                    ViewData["nMarital"] = new SelectList(_DR.GetMaritalStatus(), "ID", "NAME", lApObj.MaritalStatus_FK);
                    ViewData["nLGAs"] = new SelectList(_DR.GetAllLGAs(), "ID", "NAME", lApObj.LGA_FK);

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
                    return View("NYSCLoanAppForm", NyscLA);
                }

                if (id != 0)
                {
                   
                    string idv = "";
                    MarketingChannel Mc = new MarketingChannel();
                    //if (arr.Length == 1)
                    //{

                    //    insertMarketChannel(arr[0],id);

                    //}
                    //else 
                    if (arr.Length > 0)
                    {
                        for (var i = 0; i < arr.Length; i++)
                        {
                            string arrc = Convert.ToString(arr[i]);
                            insertMarketChannel(arrc,id);
                        }
                      
                    }
                    var email = _DR.getUser(NyscLA.EmailAddress);
                    string password = "";
                    string referralCode = "";
                    if (email == null)
                    {

                        referralCode = createUser(NyscLA, out password);
                        SendReferralEmail(NyscLA, referralCode, password);
                        //NyscLA.MyReferralCode = refCode;
                        //NyscLA.PaswordVal = password;
                    }
                    //var generateCode=  DataAccessA.MyUtility.getReferralCode(Userid.ToString());
                    SendEmail(NyscLA);
                    //var paswrd = "password";
                   

                    return RedirectToAction("Acknowledgement", new { @Refid = NyscLA.RefNumber });
                }
                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
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
                var EncrypPassword = new UvlotApp.HelperClasses.CryptographyManager().ComputeHash(user.PaswordVal, HashName.SHA256);
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
        public int insertMarketChannel(string arrs,int userfk)
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




        [HttpPost]
        public ActionResult GenerateInvoice(FormCollection form, string ItemList, TableObjects.LoanApplication LoanAP)

        {
            try
            {
                /*  TempData["SucMsg"] = ""; TempData["ErrMsg"] = "";
                  TableObjects.LoanApplication LoanApps = new TableObjects.LoanApplication();
                  var LoanAps = new AppLoan();
                  AppLoan Apploan = new AppLoan();
                  LoanApplication LoanAp = new LoanApplication();
                  DataAccessLayerT.DataManager.LoanLedger LoanLdger = new LoanLedger();
                  Institution Inst = new Institution();

                  // Get The First Value On The ItemList

                  //Ends Here//
                  int LoanApxs = 0;
                  string[] arr = ItemList.Split(',');
                  if (arr.Length == 1)
                  {
                      Inst.ID = _DR.getInstitutionByLedgerID(arr[0]);
                  }
                  else if (arr.Length > 1)
                  {
                      Inst.ID = _DR.getInstitutionByLedgerID(arr[0]);
                  }
                  IEnumerable<int> ID = arr.Select(int.Parse);
                  string IDv = "";
                  double TotalAmount = 0;
                  double Amount = 0;
                  var TodaysDate = MyUtility.getCurrentLocalDateTime();
                  TempData["Date"] = TodaysDate;
                  TempData["DateDue"] = TodaysDate.AddDays(10);
                  TempData["VAT"] = 0;
                  //if (Inst.ID != 0)
                  //{
                  //    Inst = _DR.getInstitutionById(Inst.ID);
                  //}
                  if (arr.Length > 1)
                  {
                      for (var m = 0; m < arr.Length; m++)
                      {
                          IDv = arr[m];
                          Amount = _DR.GetAmount(IDv);
                          TotalAmount = (double)(Amount + TotalAmount);
                          TempData["TotalAmount"] = TotalAmount;
                      }
                      data dt = new data();
                      dt.InstiD = Inst.ID;
                      dt.TotalAmounyt = Convert.ToDouble(TempData["TotalAmount"]);
                      // return Json(new { data = Inst.ID,datas = TempData["TotalAmount"] });
                      return Json(new { data = dt });
                      // return RedirectToAction("invoices", new { @instid = Inst.ID });
                  }
                  if (arr.Length == 1)
                  {
                      IDv = arr[0];
                      Amount = _DR.GetAmount(IDv);
                      TotalAmount = (double)(Amount + TotalAmount);
                      TempData["TotalAmount"] = TotalAmount;
                      data dt = new data();
                      dt.InstiD = Inst.ID;
                      dt.TotalAmounyt = Convert.ToDouble(TempData["TotalAmount"]);
                      return Json(new { data = dt });
                  }
                  return View("invoices");
              */
                var VALS = Request["channel"];
                return View();
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


        

        //private void CreateMarketingDetails(int User_FK, out string servicechannels)
        //{
        //    UvlotAEntities uvdb = new UvlotAEntities();
        //    servicechannels = "";
        //    try
        //    {
             
        //        foreach (ListItem listItem in chkMarketing.Items)
        //        {
        //            if (listItem.Selected)
        //            {
        //                //do some work 
        //                MarketingDetail markObj = new MarketingDetail
        //                {
        //                    User_FK = User_FK,
        //                    IsVisible = 1,
        //                    MarketingChannel_FK = Convert.ToInt16(listItem.Value),
        //                    DateCreated = MyUtility.getCurrentLocalDateTime(),
        //                    ValueDate = MyUtility.getCurrentLocalDateTime().ToString("yyyy/MM/dd"),
        //                    ValueTime = MyUtility.getCurrentLocalDateTime().ToString("H:mm ss")

        //                }; 
        //                servicechannels = servicechannels.Length < 1 ? listItem.Text : servicechannels + "," + listItem.Text;
        //                uvdb.MarketingDetails.Add(markObj);
        //                uvdb.SaveChanges();
        //            }
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        WebLog.Log(ex.Message.ToString() + "###CreateMarketingDetails###" + ex.StackTrace.ToString());

        //    }
        //}





        [HttpGet]
        public ActionResult CheckNYSCLoan()
        {

            // ViewBag.Data = _DR.CheckAppStatus(ApplicationFk);

            return View();
        }


        [HttpPost]
        public ActionResult CheckNYSCLoan(FormCollection form, LoanViewModel lvm)
        {
            NyscLoanApplication lonobj = new NyscLoanApplication();

            try

            {

                // Utility utilities = new Utility();
                string msg = "";
                var ApplicationFk = Convert.ToString(form["RefNumber"]);

                DataAccessA.Classes.AppLoanss Apploan = _DR.CheckAppStatus(ApplicationFk);

                //string LoanAmt = Convert.ToString(Apploan.LoanAmount);
                //Apploan.ConvertedLoanAmt = utilities.ConvertToCurrency(LoanAmt);
                if (Apploan == null)
                {
                    TempData["ErrMsg"] = "Record Not Found! ";
                    return View();
                }

                return View(Apploan);
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                TempData["ErrMsg"] = "Record Not Found! ";
                return null;
            }
        }

        //Admin Side

        //[HttpGet]
        //public ActionResult ApproveLoan()
        //// public ActionResult RecommendLoanSecondLevel()

        //{
        //    try
        //    {
        //        AppStatFk = 3;
        //        var LoggedInuser = new LogginHelper();
        //        user = LoggedInuser.LoggedInUser();

        //        var appUser = user;
        //        if (appUser == null)
        //        {
        //            return RedirectToAction("/", "Home");
        //        }

        //        ViewBag.Data = _DR.ApproveLoans(AppStatFk);

        //        return View();
        //    }
        //    catch (Exception ex)
        //    {
        //        WebLog.Log(ex.Message.ToString());
        //        return null;
        //    }
        //}



        //[HttpGet]
        //public ActionResult Approve(string Refid, TableObjects.LoanApplication LoanApp, AppLoanss Apploan, AppLoanss LoanApps)

        //{

        //    try
        //    {
        //        AppStatFk = 3;
        //        //var LoggedInuser = new LogginHelper();
        //        //user = LoggedInuser.LoggedInUser();

        //        //var appUser = user;
        //        //var User = _DR.getUser(appUser);
        //        //if (appUser == null)
        //        //{
        //        //    return RedirectToAction("/", "Home");
        //        //}
        //        if (Refid == null || Refid == "")
        //        {
        //            return RedirectToAction("ApproveLoan");
        //        }

        //        var LoanAppss = _DR.GetNYSCLoanApplication(Refid, AppStatFk);
        //        if (LoanAppss == null)
        //        {
        //            // return RedirectToAction("RecommendLoanSecondLevel");
        //            TempData["Error"] = "Check The Status Of The Application";
        //            return RedirectToAction("ApproveLoan");
        //        }

        //        GetApplicantInfo(Refid, LoanApp, Apploan, LoanAppss);
        //       // TempData["Username"] = User.Firstname;
        //        TempData["LoanObj"] = Apploan;
        //        //GetMenus();

        //        return View(Apploan);
        //    }
        //    catch (Exception ex)
        //    {
        //        return null;
        //    }
        //}


      





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


        public void SendReferralEmail(NyscLoanApplication nyscObj,string referralCode, string passWord)
        {
            try
            {
                string referralLink = ConfigurationManager.AppSettings["ReferralLink"] + referralCode;
                var bodyTxt = System.IO.File.ReadAllText(HostingEnvironment.MapPath("~/EmailNotifications/ReferralWelcomeEmail.html"));

                string myname = nyscObj.Firstname;
                bodyTxt = bodyTxt.Replace("$firstName", myname);
                bodyTxt = bodyTxt.Replace("$UserName", nyscObj.EmailAddress);
                bodyTxt = bodyTxt.Replace("$passwordVal", passWord);
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
        public void GetApplicantInfo(string Refid, TableObjects.LoanApplication LoanApp, AppLoanss Apploan, AppLoanss LoanApps)
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
                string repaymentAmt = Convert.ToString(Apploan.RepaymentAmount);
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

       

       


        //[HttpGet]
        //public ActionResult AllNYSCLoanReport()
        //{
        //    //GetMenus();
        //    ViewBag.Data = _DR.NYSCLoanAppReport();

        //    return View();
        //}


        //[HttpPost]
        //public ActionResult AllNYSCLoanReport(DataAccessA.DataManager.NyscLoanApplication loanApp)
        //{
        //    try
        //    {
        //        //var LoggedInuser = new LogginHelper();
        //        //user = LoggedInuser.LoggedInUser();

        //        //var appUser = user;
        //        //if (appUser == null)
        //        //{
        //        //    return RedirectToAction("/", "Home");
        //        //}
        //        //else
        //        {
        //            ViewBag.Data = _DR.NYSCLoanAppReport();
        //        }
        //        // GetMenus();
        //        return View();
        //    }
        //    catch (Exception ex)
        //    {
        //        WebLog.Log(ex.Message.ToString());
        //        return null;
        //    }
        //}
        //[HttpPost]
        //public ActionResult Approve(DataAccessA.Classes.AppLoanss Apploans, FormCollection form)
        //{
        //    var id = Convert.ToInt16(form["ID"]);
        //    var approve = Convert.ToInt16(form["Accept"]);
        //    Apploans.ID = id;
        //    Apploans.ApplicationStatus_FK = approve;
        //    var resp = _DR.UpdateNyscLoanApplication(Apploans);
        //    return RedirectToAction("ApproveLoan");

        //}
        //[HttpPost]
        //public ActionResult Approve(DataAccessA.Classes.AppLoanss Apploans, FormCollection form)
        //{
        //    var id = Convert.ToInt16(form["ID"]);
        //    var approve = Convert.ToInt16(form["Accept"]);
        //    Apploans.ID = id;
        //    Apploans.ApplicationStatus_FK = approve;
        //    var resp = _DR.UpdateNyscLoanApplication(Apploans);
        //    return RedirectToAction("ApproveLoan");
        //}

    }
}
public class YearsMonths
{
    public int Years;
    public int TotalMonths;
    public int Months;
}