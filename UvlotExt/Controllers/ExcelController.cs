using DataAccessA;
using DataAccessA.DataManager;
using ExcelUpload.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UvlotExt.Classes;
using UvlotExt.Models;

namespace UvlotApp.Controllers
{
    public class ExcelController : Controller
    {
        // GET: Excel
        DataWriter _DM = new DataWriter();
        DataReader _DR = new DataReader();
        PageAuthentication _pa = new PageAuthentication();
        string user = "";
       List<string> rol = new List<string>();

        public ActionResult Index()
        {
            return View();
        }

        //        [HttpGet]
        //        public ActionResult LoanAppF()

        //        {
        //            UvlotAEntities db = new UvlotAEntities();
        //            ViewBag.AccomodationTypes = db.AccomodationTypes.ToList();
        //            ViewBag.ApplicationStatus = db.ApplicationStatus.ToList();
        //            ViewBag.LGAs = db.LGAs.ToList();
        //             ViewBag.LoanTenures = db.LoanTenures.ToList();
        //            ViewBag.LoanProducts = db.LoanProducts.ToList();
        //            ViewBag.MaritalStatus = db.MaritalStatus.ToList();
        //            ViewBag.MeansOFID = db.MeansOfIdentifications.ToList();
        //            ViewBag.RepaymentMethods = db.RepaymentMethods.ToList();
        //            ViewBag.NigerianStates = db.NigerianStates.ToList();
        //            ViewBag.Titles = db.Titles.ToList();
        //            var list = new List<SelectListItem>
        //{
        //       new SelectListItem{ Text="Select Gender", Value = "0", Selected = true },
        //       new SelectListItem{ Text="Male", Value = "Male" },

        //       new SelectListItem{ Text="Female", Value = "Female" },



        //       };
        //            ViewData["ddList"] = list;
        //            return View();
        //        }

        //[HttpPost]
        //public ActionResult LoanAppF(LoanViewModel lvm, FormCollection form)
        //{
        //    UvlotAEntities db = new UvlotAEntities();

        //    LoanApplication lonobj = new LoanApplication();
        //    {
        //        lonobj.AccomodationType_FK = Convert.ToInt32(form["AccomodationType"]);
        //        lonobj.AccountName = lvm.LoanApplication.AccountName;
        //        lonobj.AccountNumber = lvm.LoanApplication.AccountNumber;
        //        lonobj.ApplicantID = lvm.LoanApplication.ApplicantID;
        //        lonobj.ApplicationStatus_FK = Convert.ToInt32(form["ApplicationStatus"]);
        //        lonobj.BankCode = lvm.LoanApplication.BankCode;
        //        lonobj.BVN = lvm.LoanApplication.BVN;
        //        lonobj.ClosestBusStop = lvm.LoanApplication.ClosestBusStop;
        //        lonobj.ContactAddress = lvm.LoanApplication.ContactAddress;
        //        lonobj.CreatedBy = "";
        //        lonobj.DateCreated = MyUtility.getCurrentLocalDateTime();
        //        lonobj.DateModified = MyUtility.getCurrentLocalDateTime();
        //        lonobj.DateOfBirth = lvm.LoanApplication.DateOfBirth;
        //        lonobj.EmailAddress = lvm.LoanApplication.EmailAddress;
        //        lonobj.ExistingLoan = lvm.LoanApplication.ExistingLoan.ToString().ToUpper() == "YES" ? true : false;
        //        lonobj.ExistingLoan_NoOfMonthsLeft = 0;
        //        lonobj.ExistingLoan_OutstandingAmount = 0;
        //        lonobj.Firstname = lvm.LoanApplication.Firstname;
        //        lonobj.Gender_FK = Convert.ToInt32(form["Gender"]);
        //        lonobj.IdentficationNumber = lvm.LoanApplication.IdentficationNumber;
        //        lonobj.IsVisible = 1;
        //        lonobj.Landmark = lvm.LoanApplication.Landmark;
        //        lonobj.LGA_FK = Convert.ToInt32(form["LGA"]);
        //        lonobj.LoanAmount = Convert.ToDouble(form["LoanAmount"]);
        //        lonobj.LoanComment = lvm.LoanApplication.LoanComment;
        //        lonobj.LoanProduct_FK = Convert.ToInt32(form["LoanProduct"]);
        //        lonobj.LoanRefNumber = MyUtility.GenerateRefNo();
        //        lonobj.LoanTenure = Convert.ToInt32(form["LoanTenure"]);
        //        lonobj.MaritalStatus_FK = Convert.ToInt32(form["MaritalStatus"]);
        //        lonobj.MeansOfID_FK = Convert.ToInt32(form["MeansOfID"]);
        //        lonobj.NOK_EmailAddress = lvm.LoanApplication.NOK_EmailAddress;
        //        lonobj.NOK_FullName = lvm.LoanApplication.NOK_FullName;
        //        lonobj.NOK_HomeAddress = lvm.LoanApplication.NOK_HomeAddress;
        //        lonobj.NOK_PhoneNumber = lvm.LoanApplication.NOK_PhoneNumber;
        //        lonobj.NOK_Relationship = lvm.LoanApplication.NOK_Relationship;
        //        lonobj.Organization = lvm.LoanApplication.Organization;
        //        lonobj.Othernames = lvm.LoanApplication.Othernames;
        //        lonobj.PhoneNumber = lvm.LoanApplication.PhoneNumber;
        //        lonobj.RepaymentMethod_FK = Convert.ToInt32(form["RepaymentMethod"]);
        //        lonobj.StateofResidence_FK = Convert.ToInt32(form["NigerianStates"]);
        //        lonobj.Surname = lvm.LoanApplication.Surname;
        //        lonobj.Title_FK = Convert.ToInt32(form["Title"]);
        //        lonobj.ValueDate = MyUtility.getCurrentLocalDateTime().ToString("yyyy/MM/dd");
        //        lonobj.ValueTime = MyUtility.getCurrentLocalDateTime().ToString("H:mmss");
        //        db.LoanApplications.Add(lonobj);
        //        db.SaveChanges();

        //    }
        //    return View();
        //}



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

                
                var ids = _DR.getUserRols(mc);
              
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

        [HttpGet]
        public ActionResult ImportExcel1()
        {
            ViewBag.LoanCal = Helper.PayrollLoanCalc(112500,11,4.50).ToString();
            ViewBag.mySum = Helper.CalculateSum(23, 32).ToString();
           
            GetMenus();
            return View();
        }

        [HttpPost]
        public ActionResult GetTenure(int searchVal)
        {
            try
            {
                var satId = _DR.GetInterestRate(searchVal);
               // ViewBag.ServicesList = _dr.GetAllServicesBySat(satId);
                return Json(new { Success = "true", Data = satId });
               
               // return View();
            }
           catch(Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
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

        //To Submit uploaded Excel into different tables in database.
        [HttpPost]
        public ActionResult uploaddata(LoginViewModel lvm, FormCollection form)
        {
            try
            {
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
                            if (row["SURNAME"].ToString().Length < 2 && row["FIRST NAME"].ToString().Length<2)
                            {
                                break;
                            }
                    }

                        LoanApplication instObj = new LoanApplication();

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



                        LoanLedger lonObj = new LoanLedger();
                        lonObj.ApplicantID = row["EMPLOYEE ID"].ToString();
                        lonObj.Credit = 0;
                        lonObj.Debit = 0;

                        lonObj.Institution_FK = instObj.Institution_FK;
                        lonObj.IsVisible = 1;
                        lonObj.LastUpdated = MyUtility.getCurrentLocalDateTime();
                        lonObj.RefNumber = instObj.LoanRefNumber;

                        lonObj.PartnerRefNumber = "";
                        lonObj.TranxDate= MyUtility.getCurrentLocalDateTime();
                        lonObj.ValueDate= MyUtility.getCurrentLocalDateTime().ToString("yyyy/MM/dd");
                        lonObj.ValueTime = MyUtility.getCurrentLocalDateTime().ToString("H:mmss");

                        db.LoanLedgers.Add(lonObj);
                        db.SaveChanges();

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
    }
}


