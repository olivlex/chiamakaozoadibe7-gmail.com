
using DataAccessA.Classes;
using DataAccessA.DataManager;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;

namespace DataAccessA
{

    public class Helper
    {
        UvlotAEntities uvDb = new UvlotAEntities();
        // DataReader _DR = new DataReader();
        // static string prepaidStr = "OFFLINE_PREPAID";
        DataAccessA.DataManager.DataReader _DR = new DataAccessA.DataManager.DataReader();

        public static bool isDate(string date)

        {

            try

            {

                DateTime dt = DateTime.Parse(date);

                return true;

            }
            catch

            {

                return false;

            }

        }
        public static string CalculateSum(int firstno, int secondno)

        {

            try

            {

                string thirdno = (firstno + secondno).ToString();

                return thirdno;

            }
            catch

            {

                return "0";

            }

        }
        public static DateTime getLastDateOfMonth(DateTime date, out bool isValid)
        {
            string mydate = "";
            isValid = true;
            try
            {
                var lastDayOfMonth = DateTime.DaysInMonth(date.Year, date.Month);
                mydate = date.Year + "/" + date.Month + "/" + lastDayOfMonth;
                return Convert.ToDateTime(mydate);
            }
            catch
            {
                isValid = false;
                return date;
            }
        }
        public static string GetRemitaBankCodeByFlutterCode(string flutterCode)

        {
            string myCode = flutterCode;
            try
            {
                //uvDb
                UvlotAEntities uvDB = new UvlotAEntities();
                var userObj = uvDB.Banks.Where(x => x.FlutterWaveBankCode == flutterCode).FirstOrDefault();
                myCode = userObj == null ? flutterCode : userObj.Code;


            }
            catch
            {

            }
            return myCode;

        }
        public static string GetFlutterwaveBankCodeByRemitaCode(string RemitaCode)

        {
            string myCode = RemitaCode;
            try
            {
                //uvDb
                UvlotAEntities uvDB = new UvlotAEntities();
                var userObj = uvDB.Banks.Where(x => x.Code == RemitaCode ).FirstOrDefault();
                myCode = userObj == null ? RemitaCode : userObj.FlutterWaveBankCode;

              
            }
            catch
            {
               
            }
            return myCode;

        }
        public static int ValidateDisbursement(string loanRefNumber)

        {
            try
            {
                //uvDb
                UvlotAEntities uvDB = new UvlotAEntities();
                var userObj = uvDB.NyscLoanApplications.Where(x => x.RefNumber == loanRefNumber && x.NYSCApplicationStatus_FK == 5).FirstOrDefault();

                return userObj == null ? 0 : 1;
            }
            catch
            {
                return 1;
            }

        }

        public static int ValidateReferralCode(string referralCode)

        {
            try
            {
                //uvDb
                UvlotAEntities uvDB = new UvlotAEntities();
                var userObj = uvDB.Users.Where(x => x.ReferralCode == referralCode).FirstOrDefault();

                return userObj == null ? 1 : 2;
            }
            catch
            {
                return 1;
            }

        }
        public static string PayrollLoanCalc(double PA, int LT, double INTR)

        {
            // Fomular to get Repayment amount for each month using the Loan Amount, Loan tenure and the Interest .
            try
            {

                // PA= Principal Amount/Loan Amount.
                // LT= Loan Tenure.
                // INTR= Interest Rate in percentage.
                // Multilpy the principal amount with the loan tenure, 
                // then multiply your result with the interest rate which is in percentage 
                // then add the pricipal amount to the newly derived answer 
                // and finally divide the over all result with the Loan tenure.

                string monthlyRepayment = (((PA + (PA * LT) * (INTR * 0.01))) / LT).ToString();



                return monthlyRepayment;

            }
            catch

            {

                return "0";

            }


        }

        public static BVNC BVNValidationResps(string bvnNumber)
        {

            BVNC BC = new BVNC();
            //errormessage = "Enquiry failed";
            try
            {
                if (bvnNumber.Length < 11)
                {
                    BC.errormessage = "Please enter valid BVN number";
                    BC.respCode = "0001";
                    BC.respDescription = BC.errormessage;
                    return BC;
                }

                var Url = ConfigurationManager.AppSettings["ValidateBVN"];
                var secKey = ConfigurationManager.AppSettings["secKey"];

                Url = Url.Replace("{$BVNnumber}", bvnNumber.ToString()).Trim().Replace("{$secKey}", secKey.ToString()).Trim();
                var resp = DoGet(Url);


                dynamic resps = JObject.Parse(resp);
                if (resps.status != "success")
                {
                    BC.respCode = "009";
                    BC.respDescription = resps.message;
                    BC.errormessage = "Please enter valid BVN number";
                }
                else
                {
                    BC.respCode = "00";
                    BC.respDescription = "Request Successful";
                    BC.FirstNAme = resps.data.first_name;
                    BC.MiddleName = resps.data.middle_name;
                    BC.LastName = resps.data.last_name;
                    BC.Dateofbirth = resps.data.date_of_birth;
                    BC.Phone = resps.data.phone_number;
                    BC.RegistrationDate = resps.data.registration_date;
                    BC.EnrollmentBank = resps.data.enrollment_bank == null ? "" : resps.data.enrollment_bank;
                    BC.EnrollmentBank = Helper.GetRemitaBankCodeByFlutterCode(BC.EnrollmentBank);
                    BC.EnrollmentBranch = resps.data.enrollment_branch;
                    //BC.image_base_64 = resps.data.image_base_64;
                    BC.address = resps.data.address;
                    BC.gender = resps.data.gender;
                    BC.email = resps.data.email;
                    BC.watch_listed = resps.data.watch_listed;
                    BC.Nationality = resps.data.nationality;
                    BC.marital_status = resps.data.marital_status;
                    BC.state_of_residence = resps.data.state_of_residence;
                    BC.lga_of_residence = resps.data.lga_of_residence;
                    BC.errormessage = "";
                }
                return BC;
            }
            catch (Exception ex)
            {
                BC.respCode = "0091";
                BC.respDescription = "Error validating BVN";
                BC.errormessage = BC.respDescription;
                WebLog.Log(ex.Message);
                return BC;
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



        public static string DoGet(string Url)
        {

            string resp;
            try
            {
                //string agentID = "test1";
                // string agentKey = "test2";
                // url = url + "?agentID=" + agentID + "&agentKey=" + agentKey;
                // WebLog.Log("sessionidurl: " + url);
                using (var client = new WebClient())
                {
                    ServicePointManager.Expect100Continue = true;
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    resp = client.DownloadString(Url);
                }



            }
            catch (WebException wex)
            {
                //WebLog.Log(wex);
                using (var response = (HttpWebResponse)wex.Response)
                {
                    var statusCode = response != null ? (int)response.StatusCode : 500;
                    if (statusCode == 500 && response == null) return null;
                    var dataStream = response?.GetResponseStream();
                    if (dataStream == null) return null;
                    using (var tReader = new StreamReader(dataStream))
                    {
                        resp = tReader.ReadToEnd();
                    }
                }
            }
            catch (Exception ex)
            {
                //WebLog.Log(ex);
                resp = ex.Message;
            }
            return resp;
        }

        public int SaveBVNDetails(BVNC bvnc)
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
        public static bool isNumeric(string date)

        {

            try

            {

                int dt = int.Parse(date);

                return true;

            }
            catch

            {

                return false;

            }

        }

        private static readonly Random Random = new Random();
        public static string RandomString(int length, Mode mode = Mode.AlphaNumeric)
        {
            var characters = new List<char>();

            if (mode == Mode.Numeric || mode == Mode.AlphaNumeric || mode == Mode.AlphaNumericUpper || mode == Mode.AlphaNumericLower)
                for (var c = '0'; c <= '9'; c++)
                    characters.Add(c);

            if (mode == Mode.AlphaNumeric || mode == Mode.AlphaNumericUpper || mode == Mode.AlphaUpper)
                for (var c = 'A'; c <= 'Z'; c++)
                    characters.Add(c);

            if (mode == Mode.AlphaNumeric || mode == Mode.AlphaNumericLower || mode == Mode.AlphaLower)
                for (var c = 'a'; c <= 'z'; c++)
                    characters.Add(c);

            return new string(Enumerable.Repeat(characters, length)
              .Select(s => s[Random.Next(s.Count)]).ToArray());
        }
        public enum Mode
        {
            AlphaNumeric = 1,
            AlphaNumericUpper = 2,
            AlphaNumericLower = 3,
            AlphaUpper = 4,
            AlphaLower = 5,
            Numeric = 6
        }
        public static string DoPost(string url, [Optional]string Authorization, [Optional]string MERCHANT_ID, [Optional]string API_KEY, [Optional]string REQUEST_ID, [Optional]string REQUEST_TS, [Optional]string API_DETAILS_HASH, string json)
        {
            string resp;
            try
            {
                using (var client = new WebClient())
                {
                    client.Headers[HttpRequestHeader.ContentType] = "application/json";
                    if (!string.IsNullOrWhiteSpace(MERCHANT_ID))
                    {
                        //client.Headers[HttpRequestHeader.Authorization] = Authorization;
                        client.Headers.Set("MERCHANT_ID", MERCHANT_ID);
                        client.Headers.Set("API_KEY", API_KEY);
                        client.Headers.Set("REQUEST_ID", REQUEST_ID);
                        client.Headers.Set("REQUEST_TS", REQUEST_TS);
                        client.Headers.Set("API_DETAILS_HASH", API_DETAILS_HASH);

                    }
                    resp = client.UploadString(url, "POST", json);
                }
            }
            catch (WebException wex)
            {
                //WebLog.Log(wex);
                using (var response = (HttpWebResponse)wex.Response)
                {
                    var statusCode = response != null ? (int)response.StatusCode : 500;
                    if (statusCode == 500 && response == null) return null;
                    var dataStream = response?.GetResponseStream();
                    if (dataStream == null) return null;
                    using (var tReader = new StreamReader(dataStream))
                    {
                        resp = tReader.ReadToEnd();
                    }
                }
            }
            catch (Exception ex)
            {
                // WebLog.Log(ex);
                resp = ex.Message;
            }
            return resp;
        }

        private static void WaitTime(double seconds)
        {
            DateTime dt = DateTime.Now + TimeSpan.FromSeconds(seconds);

            do { } while (DateTime.Now < dt);
        }
        public static string ConvertToCurrency(string amount)
        {
            return string.Format("{0:N2}", Convert.ToDecimal(amount));
        }
        public static string ConvertToNairaCurrency(string amount)
        {
            return string.Format("₦{0:N2}", Convert.ToDecimal(amount));
        }
        public static string ConvertToNairaCurrencyNoZero(string amount)
        {
            string newamount = string.Format("₦{0:N2}", Convert.ToDecimal(amount));
            return newamount.Remove(newamount.Length - 3, 3);

        }
        public static string ConvertTonNairaCurrencyNoZero(string amount)
        {
            string newamount = string.Format("N{0:N2}", Convert.ToDecimal(amount));
            return newamount.Remove(newamount.Length - 3, 3);

        }
        public static string ConvertTourrencyNoZero(string amount)
        {
            string newamount = string.Format("{0:N2}", Convert.ToDecimal(amount));
            return newamount.Remove(newamount.Length - 3, 3);

        }
    }
}
