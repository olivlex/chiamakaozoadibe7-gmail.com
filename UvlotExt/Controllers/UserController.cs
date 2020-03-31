using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UvlotApp.HelperClasses;
using System.Web.Mvc;
using DataAccessA.DataManager;
using Utilities;

using System.Web.Security;
using System.Configuration;
using DataAccessA;
using System.Web.Hosting;
using UvlotApplication.Classes;

namespace UvlotApp.Controllers
{
    public class UserController : Controller
    {
        DataWriter _DM = new DataWriter();
        DataReader _DR = new DataReader();
        // GET: User
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Sigin()
        {
            TempData["ErrMsg"] = ""; TempData["SucMsg"] = "";
            return View();
        }
        [HttpGet]
        public ActionResult Signin()
        {
            TempData["ErrMsg"] = ""; TempData["SucMsg"] = "";
            return View();
        }
        [HttpGet]
        public ActionResult Signup()
        {
            TempData["ErrMsg"] = ""; TempData["SucMsg"] = "";
            string Refid = Request.QueryString["Regid"];
          
            if (Refid != null || Refid != "")
            {
                WebLog.Log("Refid" + Refid);
                TempData["Refid"]= Refid;
              
            }
            int val = 0;
            ViewData["nLGAs"] = new SelectList(_DR.GetAllLGAs(), "ID", "NAME", val);

          
            ViewData["nStates"] = new SelectList(_DR.GetNigerianStates(), "ID", "NAME", val);
            return View();
        }
        [HttpPost]
        public ActionResult Signin(FormCollection form)
        {
            try

            {
                WebLog.Log("Valid" );
                // Signinx(form);
                TempData["ErrMsg"] = ""; TempData["SucMsg"] = "";
                User user = new User();
                user.EmailAddress = Convert.ToString(form["username"]);
                user.PaswordVal = Convert.ToString(form["password"]);
                WebLog.Log("EmailAddress " + user.EmailAddress);



                var EncrypPassword = new HelperClasses.CryptographyManager().ComputeHash(user.PaswordVal, HashName.SHA256);
                user.PaswordVal = EncrypPassword;
                WebLog.Log(" user.PaswordVal  " + user.PaswordVal);
                var valid = _DR.loggedIn(user.EmailAddress, EncrypPassword);
                WebLog.Log("Valid1" + valid);
                if (valid == true)
                {
                    WebLog.Log("Valid2" + valid);
                    if (user.EmailAddress != null)
                    {
                        WebLog.Log("users.Email 2" + user.EmailAddress);
                        Session["id"] = LoggedInEmail(user.EmailAddress);
                        Session["User"] = Session["id"];
                        WebLog.Log("Am here");
                       
                        return RedirectToAction("Index", "AdminA");
                    }
                    else
                        TempData["ErrMsg"] = "Invalid User Try Again";
                    return View("Signin");
                }
                else
                {
                    WebLog.Log("Valid3" + valid);
                    TempData["ErrMsg"] = "User Does Not Exist";
                    return View("Signin");
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
                        WebLog.Log(message);
                    }
                }

                WebLog.Log(raise);
                TempData["ErrMsg"] = raise.Message;
                return View("Signin");
            }
            //catch (Exception ex)
            //{
            //    WebLog.Log(ex.Message.ToString());
            //    TempData["ErrMsg"] = ex.Message;
            //    return View("Signin");
            //}
        }

        public ActionResult LogOut()
        {
            try
            {
                FormsAuthentication.SignOut();
                Session.Clear();
                return RedirectToAction("HomePage", "Home");

            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


        public void Signinx(FormCollection form)
        {
            try
            {
                TempData["ErrMsg"] = ""; TempData["SucMsg"] = "";
                User user = new User();
                user.EmailAddress = Convert.ToString(form["username"]);
                user.PaswordVal = Convert.ToString(form["password"]);
                var EncrypPassword = new HelperClasses.CryptographyManager().ComputeHash(user.PaswordVal, HashName.SHA256);
                user.PaswordVal = EncrypPassword;

                var valid = _DR.loggedIn(user.EmailAddress, EncrypPassword);
                WebLog.Log("Valid1" + valid);
                if (valid == true)
                {
                    WebLog.Log("Valid2" + valid);
                    if (user.EmailAddress != null)
                    {
                        WebLog.Log("users.Email 2" + user.EmailAddress);
                        Session["id"] = user.EmailAddress;
                        Session["User"] = Session["id"];
                    }
                    else
                        TempData["message"] = "Invalid User Try Again";

                }

            }
            catch (System.Data.Entity.Validation.DbEntityValidationException dbEX)
            {
                Exception raise = dbEX;
                foreach (var validationErrors in dbEX.EntityValidationErrors)
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
        [HttpPost]
        public ActionResult Signup(FormCollection form, DataAccessA.Classes.LoanApplication lApObj)
        {
            try
            {
                var Refid = Convert.ToInt16(TempData["Refid"]);
                string referralCode = Request.QueryString["Regid"];
                WebLog.Log("Ref" + Refid);
                if(Refid == 0)
                {
                  return RedirectToAction("HomePage","Home" );
                }
                string respMsg = "";
                TempData["ErrMsg"] = ""; TempData["SucMsg"] = "";
                User Users = new User();
                string password = Convert.ToString(form["password"]);
                string rpassword = Convert.ToString(form["cpassword"]);
                Users.EmailAddress = Convert.ToString(form["email"]);
                Users.Firstname = Convert.ToString(form["fname"]);
                // Users.Lastname = Convert.ToString(form["lname"]);
                Users.PhoneNumber = Convert.ToString(form["phone"]);
                Users.UserAddress = Convert.ToString(form["address"]);
                
                Users.ReferralCode = Convert.ToString(form["ReferralCode"]);
                Users.StateofResidence_FK = lApObj.StateofResidence_FK;
                Users.LGA_FK = Convert.ToInt16(form["lgaList"]);
                Users.ContactAddress = Convert.ToString(form["address"]);
                Users.PaswordVal = password;
                //var EncrypPassword = new HelperClasses.CryptographyManager().ComputeHash(password, HashName.SHA256);
                var EncrypPassword = new HelperClasses.CryptographyManager().ComputeHash(Users.PaswordVal, HashName.SHA256);
                Users.PaswordVal = EncrypPassword;
                bool validatepas = ValidatePassword(password, rpassword);
                if (validatepas == false)
                {
                    TempData["ErrMsg"] = "Password And Confirm Password Must Match";
                }
                if (validatepas == true)
                {
                    bool val = _DR.Validate(Users.EmailAddress);
                    if (val == true)
                    {
                        TempData["ErrMsg"] = "User Already Exist";
                        TempData["SucMsg"] = "";
                        int val1 = 0;
                        ViewData["nLGAs"] = new SelectList(_DR.GetAllLGAs(), "ID", "NAME", val1);


                        ViewData["nStates"] = new SelectList(_DR.GetNigerianStates(), "ID", "NAME", val1);
                        return View("Signup");
                    }
                    else if (val == false)
                    {

                        password = EncrypPassword;
                        Users.PaswordVal = password;
                        //Users.ReferralCode = "new";
                        if (Users.ReferralCode != null)
                        {
                            Users.ReferralLevel = Helper.ValidateReferralCode(Users.ReferralCode);
                        }else
                        {
                            Users.ReferralLevel = 1;
                        }
                       
                        var Userid = _DM.InsertUser(Users);
                        if (Userid != 0)
                        {
                            Users.MyReferralCode = DataAccessA.MyUtility.getReferralCode(Userid.ToString());
                            var id = _DM.CreateReferalCode(Users);
                            TempData["ErrMsg"] = "User Created Succesfully";
                            Session["id"] = LoggedInEmail(Users.EmailAddress);
                            Session["User"] = Session["id"];
                            if (Refid == 1)
                            {
                                // Referrel 
                                CreateUserRole(Users,Refid);
                            }
                            else if (Refid == 2)
                            {

                                //NyscLoanApplication Apllicant
                                CreateUserRole(Users,Refid);
                            }
                           
                         
                           
                           /*  UserRole UserRoles = new UserRole();
                            UserRoles.User_FK = Users.ID;
                            UserRoles.Role_FK = Convert.ToInt16(ConfigurationManager.AppSettings["NYSCAgentRole"]);
                            UserRoles.IsVisible = 1;
                            _DM.InsertUserRoles(UserRoles);*/
                          
                            SendEmail(Users, rpassword);
                            // var referralcode = _DR.GetReferralCode(Userid);
                            return RedirectToAction("index", "AdminA");
                        }
                    }
                }
                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return View("Signup");
            }

        }


        public int CreateUserRole(User users,int flag)
        {
            try
            {
                
                UserRole UserRoles = new UserRole();
                UserRoles.User_FK = users.ID;
                if(flag == 1)
                {
                    UserRoles.Role_FK = Convert.ToInt16(ConfigurationManager.AppSettings["NYSCAgentRole"]);
                }
                if (flag == 2)
                {
                    UserRoles.Role_FK = Convert.ToInt16(ConfigurationManager.AppSettings["NYSCApplicantRole"]);
                }

                UserRoles.IsVisible = 1;
                _DM.InsertUserRoles(UserRoles);
                return users.ID;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return 0;
            }
        }

        public void SendEmail(User userObj, string password )
        {
            try
            {
                string referralLink = ConfigurationManager.AppSettings["ReferralLink"] + userObj.MyReferralCode;
                var bodyTxt = System.IO.File.ReadAllText(HostingEnvironment.MapPath("~/EmailNotifications/ReferralWelcomeEmail.html"));

                string myname = userObj.Firstname;
                bodyTxt = bodyTxt.Replace("$firstName", myname);
                bodyTxt = bodyTxt.Replace("$UserName",userObj.EmailAddress);
                bodyTxt = bodyTxt.Replace("$passwordVal", password);
                bodyTxt = bodyTxt.Replace("$refferalCode", userObj.MyReferralCode);
                bodyTxt = bodyTxt.Replace("$referralLink", referralLink);
                //bodyTxt = bodyTxt.Replace("$RepaymentAmt", MyUtility.ConvertToCurrency(apObj.RepaymentAmount));
                //WebLog.Log(bodyTxt);
               // WebLog.Log("apObj.EmailAddress: " + apObj.EmailAddress);
                var msgHeader = $"Welcome to CashNowNow Referral Network";
                WebLog.Log("msgHeader " + msgHeader);
                var sendMail = NotificationService.SendMailOut(msgHeader, bodyTxt, userObj.EmailAddress, null, null);
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
            }
        }


        public string LoggedInEmail(string email)
        {
            try
            {
                string emails = email;
                return emails;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        public bool ValidatePassword(string Password, string ConfirmPass)
        {
            try
            {
                string value = Password;
                string value1 = ConfirmPass;
                if (value == value1)
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
        [HttpPost]
        public JsonResult getLGAsByStateID(int id)
        {
            List<SelectListItem> liLGAs = new List<SelectListItem>();
            try
            {

          
            var ddlLGA = _DR.GetLGAsByStateFK(id).ToList();
           

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
            catch
            {
                liLGAs.Add(new SelectListItem { Text = "SELECT LGA", Value = "0" });
                return Json(new SelectList(liLGAs, "Value", "Text", JsonRequestBehavior.AllowGet));
            }
        }

    }
}