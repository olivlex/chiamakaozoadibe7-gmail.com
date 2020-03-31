
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Utilities;

namespace DataAccessA
{
    public static class MyUtility
    {
        public static string Before(string @this, string a)
        {
            try
            {
                var posA = @this.IndexOf(a, StringComparison.Ordinal);
                return posA == -1 ? "" : @this.Substring(0, posA);
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }
        public static string getReferralCode(string userID)
        {
            string referralcode = Left(GenerateRefNo(), 6);
            try
            {
                if (userID.Length == 1)
                {
                    referralcode = userID + Left(GenerateRefNo(), 5);
                }
                else if (userID.Length == 2)
                {
                    referralcode = userID + Left(GenerateRefNo(), 4);
                }
                else if (userID.Length == 3)
                {
                    referralcode = userID + Left(GenerateRefNo(), 3);
                }
                else if (userID.Length == 4)
                {
                    referralcode = userID + Left(GenerateRefNo(), 2);
                }
                else if (userID.Length == 5)
                {
                    referralcode = userID + Left(GenerateRefNo(), 1);
                }
                else
                {
                    referralcode = userID;
                }
            }
            catch
            {
                referralcode = userID;
            }
            return referralcode;
        }
        public static string Left(this string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value)) return value;
            maxLength = Math.Abs(maxLength);

            return (value.Length <= maxLength
                   ? value
                   : value.Substring(0, maxLength)
                   );
        }
        public static string Right(this string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value)) return value;
            maxLength = Math.Abs(maxLength);

            return (value.Length >= maxLength
                   ? value
                   : value.Substring(value.Length, maxLength)
                   );
        }

        public static string DoPosts(string json, string url, string seckey, string callbackurl, string hash)
        {
            string resp;

            WebLog.Log("Json" + json);
            try
            {
                using (var client = new WebClient())
                {
                    client.Headers[HttpRequestHeader.ContentType] = "application/json";
                    if (!string.IsNullOrWhiteSpace(seckey))
                    {
                        client.Headers.Add("Authorization", seckey);
                        client.Headers.Add("Hash", hash);

                    }
                    ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
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
                //WebLog.Log(ex);
                resp = ex.Message;
            }
            return resp;
        }


        //Type a message

        public static string After(string @this, string a)
        {
            try
            {
                var posA = @this.LastIndexOf(a, StringComparison.Ordinal);
                if (posA == -1)
                {
                    return "";
                }
                var adjustedPosA = posA + a.Length;
                return adjustedPosA >= @this.Length ? "" : @this.Substring(adjustedPosA);
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        public static string DoPostToken(string json, string url, [Optional]string token)
        {

            string resp;
            try
            {
                using (var client = new WebClient())
                {
                    client.Headers[HttpRequestHeader.ContentType] = "application/json";
                    if (!string.IsNullOrWhiteSpace(token))
                    {
                        client.Headers[HttpRequestHeader.Authorization] = token;
                    }
                    ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                    resp = client.UploadString(url, "POST", json);
                    WebLog.Log("Post from utility" + url + json);
                }
            }
            catch (WebException wex)
            {
                WebLog.Log(wex);
                using (var response = (HttpWebResponse)wex.Response)
                {
                    var statusCode = response != null ? (int)response.StatusCode : 500;
                    if (statusCode == 500 && response == null) return null;
                    var dataStream = response?.GetResponseStream();
                    if (dataStream == null) return null;
                    using (var tReader = new StreamReader(dataStream))
                    {
                        resp = tReader.ReadToEnd();
                        WebLog.Log("response from utility" + resp);
                    }
                }
            }
            catch (Exception ex)
            {
                WebLog.Log(ex);
                resp = ex.Message;
            }
            return resp;
        }



        public static string DoRemitaPost(string url, string json)
        {

            string resp;
            try
            {
                string partnerID = ConfigurationManager.AppSettings["PartnerID"];
                string partnerkey = ConfigurationManager.AppSettings["PartnerKey"];
                using (var client = new WebClient())
                {
                    client.Headers[HttpRequestHeader.ContentType] = "application/json";
                    if (!string.IsNullOrWhiteSpace(partnerkey))
                    {
                        client.Headers.Add("partnerID",partnerID);
                        client.Headers.Add("partnerKey", partnerkey);
                    }
                    ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                    resp = client.UploadString(url, "POST", json);
                    WebLog.Log("Post from utility" + url + json);
                }
            }
            catch (WebException wex)
            {
                WebLog.Log(wex);
                using (var response = (HttpWebResponse)wex.Response)
                {
                    var statusCode = response != null ? (int)response.StatusCode : 500;
                    if (statusCode == 500 && response == null) return null;
                    var dataStream = response?.GetResponseStream();
                    if (dataStream == null) return null;
                    using (var tReader = new StreamReader(dataStream))
                    {
                        resp = tReader.ReadToEnd();
                        WebLog.Log("response from utility" + resp);
                    }
                }
            }
            catch (Exception ex)
            {
                WebLog.Log(ex);
                resp = ex.Message;
            }
            return resp;
        }
        public static string DoPost(string json, string url, [Optional]string token)
        {

            string resp;
            try
            {
                using (var client = new WebClient())
                {
                    client.Headers[HttpRequestHeader.ContentType] = "application/json";
                    if (!string.IsNullOrWhiteSpace(token))
                    {
                        client.Headers[HttpRequestHeader.Authorization] = token;
                    }
                    ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                    resp = client.UploadString(url, "POST", json);
                    WebLog.Log("Post from utility" + url + json);
                }
            }
            catch (WebException wex)
            {
                WebLog.Log(wex);
                using (var response = (HttpWebResponse)wex.Response)
                {
                    var statusCode = response != null ? (int)response.StatusCode : 500;
                    if (statusCode == 500 && response == null) return null;
                    var dataStream = response?.GetResponseStream();
                    if (dataStream == null) return null;
                    using (var tReader = new StreamReader(dataStream))
                    {
                        resp = tReader.ReadToEnd();
                        WebLog.Log("response from utility" + resp);
                    }
                }
            }
            catch (Exception ex)
            {
                WebLog.Log(ex);
                resp = ex.Message;
            }
            return resp;
        }
        public static string ConvertToCurrency(string amount)
        {
            return string.Format("{0:N2}", Convert.ToDecimal(amount));
        }

        public static string GetNonce()
        {
            return getCurrentLocalDateTime().ToString("yyyyMMddHmmss");
        }

        public static string GetNonces()
        {
            return getCurrentLocalDateTime().ToString("dddd, dd MMMM yyyy HH: mm:ss");
        }
        public static string GetUniqueRefNo()
        {

            string milliseconds = DateTime.Now.Ticks.ToString();
            return milliseconds.ToString();

        }
        public static string GetTimestamp(DateTime value)
        {
            DateTime nx = new DateTime(1970, 1, 1);
            // UNIX epoch date          
            TimeSpan ts = DateTime.UtcNow - nx;
            // UtcNow, because timestamp is in GMT            
            return ((int)ts.TotalSeconds).ToString();
        }
        public static string postNewXMLData(string destinationUrl, string requestXml)
        {
            try
            {

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(destinationUrl);
                byte[] bytes;
                bytes = System.Text.Encoding.ASCII.GetBytes(requestXml);
                request.ContentType = "text/xml; encoding='utf-8'";
                request.ContentLength = bytes.Length;
                request.Method = "POST";
               
                Stream requestStream = request.GetRequestStream();
                requestStream.Write(bytes, 0, bytes.Length);
                requestStream.Close();
                HttpWebResponse response;
                response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    Stream responseStream = response.GetResponseStream();
                    string responseStr = new StreamReader(responseStream).ReadToEnd();
                    return responseStr;
                }
                return null;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message);
                return null;
            }
        }
        public static string PostXmlData( string url, string requestXml, string soapAction, out int statusCode)
        {

            //tranx = new TranInformation(); //SHA1(UnitId,uniqueSeqNo,meterNo,Amount,tranxDate)           
            var xmlResponseText = "";
            try
            {
                var httpRequest = (HttpWebRequest)WebRequest.Create(url);
                httpRequest.Headers.Add("SOAPAction", soapAction);
                httpRequest.ContentType = "text/xml;charset=utf-8";
                httpRequest.Method = "POST";
                httpRequest.Timeout = Timeout.Infinite;
                httpRequest.KeepAlive = true;
                httpRequest.UseDefaultCredentials = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                var xmlRequestDocument = new XmlDocument();
                xmlRequestDocument.LoadXml(requestXml);
                using (var stream = httpRequest.GetRequestStream())
                {
                    xmlRequestDocument.Save(stream);
                }
                using (var response = (HttpWebResponse)httpRequest.GetResponse())
                {
                    statusCode = (int)response.StatusCode;
                    // ReSharper disable once AssignNullToNotNullAttribute
                    using (var reader = new StreamReader(response.GetResponseStream()))
                    {
                        xmlResponseText = reader.ReadToEnd();
                    }
                }
                // WebLog.Log("statusCode0: " + statusCode + " :xmlResponseText: " + xmlResponseText);
                return xmlResponseText;
            }
            //to read the body of the server response when status code != 200
            catch (WebException exec)
            {

                using (var response = (HttpWebResponse)exec.Response)
                {
                    statusCode = response != null ? (int)response.StatusCode : 500;
                    if (statusCode != 500)
                    {
                        if (response == null) return xmlResponseText;
                        var dataStream = response.GetResponseStream();
                        if (dataStream == null) return xmlResponseText;
                        using (var tReader = new StreamReader(dataStream))
                        {
                            xmlResponseText = tReader.ReadToEnd();
                        }
                        return xmlResponseText;
                    }
                }
                return null;
            }
            catch (Exception ex)
            {

                statusCode = 500;
                WebLog.Log(ex.Message + "##MyUtility:PostXmlData##", ex.StackTrace);
                return null;
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
        
        public static HttpWebRequest CreateWebRequest(string MethodName)
        {

            string baseurl = ConfigurationManager.AppSettings["EKEDC_Endpoint"] + MethodName;
            WebLog.Log("Base Url: " + baseurl);
            //GetLocalIPAddress();
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(baseurl);
            webRequest.Headers.Add(@"SOAP:Action");
            webRequest.ContentType = "text/xml;charset=\"utf-8\"";
            webRequest.Accept = "text/xml";
            webRequest.Method = "POST";
            return webRequest;
        }
        public static HttpWebRequest CreateWebRequest(string baseurl, string MethodName)
        {
             baseurl = baseurl + MethodName;
           // WebLog.Log("Base Url: " + baseurl);
            //GetLocalIPAddress();
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(baseurl);
            webRequest.Headers.Add(@"SOAP:Action");
            webRequest.ContentType = "text/xml;charset=\"utf-8\"";
            webRequest.Accept = "text/xml";
            webRequest.Method = "POST";
            return webRequest;
        }
        public static HttpWebRequest CreateWebRequest()
        {

            string baseurl = ConfigurationManager.AppSettings["IBEDC_EndPoint"];

            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(baseurl);
            webRequest.Headers.Add(@"SOAP:Action");
            webRequest.ContentType = "text/xml;charset=\"utf-8\"";
            webRequest.Accept = "text/xml";
            webRequest.Method = "POST";
            return webRequest;
        }
        public static string Read_Response(string serverResponse)
        {
            //read the response.
            var xDoc = XDocument.Load(new StringReader(serverResponse));
            var unwrappedResponse = xDoc.Descendants((XNamespace)"http://schemas.xmlsoap.org/soap/envelope/" + "Body")
                .First()
                .FirstNode;
            var soapResponse = unwrappedResponse.ToString();
            const string filter = @"xmlns(:\w+)?=""([^""]+)""|xsi(:\w+)?=""([^""]+)""";
            soapResponse = Regex.Replace(soapResponse, filter, "");
            soapResponse = soapResponse.Replace("a:", "");
            soapResponse = soapResponse.Replace(@"i:nil=", "");
            soapResponse = soapResponse.Replace("true", "");
            soapResponse = soapResponse.Replace(@"""", "");
            soapResponse = soapResponse.Replace("ns2:", "");
            soapResponse = soapResponse.Replace("ns3:", "");
            return soapResponse;
        }
        public static string PostXmlDataWithSsl3(string url, string requestXml, string soapAction, out int statusCode)
        {

            //tranx = new TranInformation(); //SHA1(UnitId,uniqueSeqNo,meterNo,Amount,tranxDate)           
            var xmlResponseText = "";
            try
            {
                var httpRequest = (HttpWebRequest)WebRequest.Create(url);
                httpRequest.Headers.Add("SOAPAction", soapAction);
                //  httpRequest.ContentType = "text/xml;charset=utf-8";
                httpRequest.ContentType = "text/xml;charset=utf-8";
                httpRequest.Method = "POST";
                httpRequest.Timeout = Timeout.Infinite;
                httpRequest.KeepAlive = true;
                httpRequest.UseDefaultCredentials = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3;
                var xmlRequestDocument = new XmlDocument();
                xmlRequestDocument.LoadXml(requestXml);
                using (var stream = httpRequest.GetRequestStream())
                {
                    xmlRequestDocument.Save(stream);
                }
                using (var response = (HttpWebResponse)httpRequest.GetResponse())
                {
                    statusCode = (int)response.StatusCode;
                    // ReSharper disable once AssignNullToNotNullAttribute
                    using (var reader = new StreamReader(response.GetResponseStream()))
                    {
                        xmlResponseText = reader.ReadToEnd();
                    }
                }
                return xmlResponseText;
            }
            //to read the body of the server response when status code != 200
            catch (WebException exec)
            {

                using (var response = (HttpWebResponse)exec.Response)
                {
                    statusCode = response != null ? (int)response.StatusCode : 500;
                    if (statusCode != 500)
                    {
                        if (response == null) return xmlResponseText;
                        var dataStream = response.GetResponseStream();
                        if (dataStream == null) return xmlResponseText;
                        using (var tReader = new StreamReader(dataStream))
                        {
                            xmlResponseText = tReader.ReadToEnd();
                        }
                        return xmlResponseText;
                    }
                }
              //  tranx.ReturnMessage.Add((exec.Message.Split(':'))[0] + ", please try again");
                return null;
            }
            catch (Exception ex)
            {

                statusCode = 500;
               // tranx.ReturnMessage.Add(ex.Message);
                WebLog.Log(ex.Message);
                return null;
            }
        }
        public class TranInformation
        {
            public bool ReturnStatus { get; set; }
            public List<string> ReturnMessage { get; private set; }
            public int TotalRows { get; set; }
            public bool IsAuthenicated { get; set; }
            public string SortExpression { get; set; }
            public string SortDirection { get; set; }
            public int CurrentPageNumber { get; set; }

            public TranInformation()
            {
                ReturnMessage = new List<string>();
                ReturnStatus = true;
                IsAuthenicated = false;
            }
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
        public static string ConvertTourrencyNoZero(string amount)
        {
            string newamount = string.Format("{0:N2}", Convert.ToDecimal(amount));
            return newamount.Remove(newamount.Length - 3, 3);

        }
        public static DateTime getCurrentLocalDateTime()
        {
            // gives you current Time in server timeZone
            var serverTime = DateTime.Now;
            // convert it to Utc using timezone setting of server computer
            var utcTime = serverTime.ToUniversalTime();
            var tzi = TimeZoneInfo.FindSystemTimeZoneById("W. Central Africa Standard Time");
            // convert from utc to local
            var localTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, tzi);
            return localTime;
        }
        public static string GenerateRefNo()
        {

            return UnixTimeStampUtc().ToString() + Create7DigitString(4);
            //  return InstantTimeTicks + InstantTimeSeconds;
        }

        public static string GenerateRefNos()
        {

            return UnixTimeStampUtc().ToString() /*+ Create7DigitString(4)*/;
            //  return InstantTimeTicks + InstantTimeSeconds;
        }
        public static int UnixTimeStampUtc()
        {
            var currentTime = DateTime.Now;
            var zuluTime = currentTime.ToUniversalTime();
            var unixEpoch = new DateTime(1970, 1, 1);
            var unixTimeStamp = (Int32)(zuluTime.Subtract(unixEpoch)).TotalSeconds;
            return unixTimeStamp;
        }
        public static string Create7DigitString(int xter)
        {
            var rng = new Random();
            var builder = new StringBuilder();
            while (builder.Length < xter)
            {
                builder.Append(rng.Next(10).ToString());
            }
            var refNumber = builder.ToString();
            return refNumber;
        }
        public static string GetUniqueKey(int maxSize)
        {
            char[] chars = new char[62];
            chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();
            byte[] data = new byte[1];
            using (RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider())
            {
                crypto.GetNonZeroBytes(data);
                data = new byte[maxSize];
                crypto.GetNonZeroBytes(data);
            }
            StringBuilder result = new StringBuilder(maxSize);
            foreach (byte b in data)
            {
                result.Append(chars[b % (chars.Length)]);
            }
            return result.ToString();
        }
        public static double GetMax(double first, double second)
        {
            if (first > second)
            {
                return first;
            }

            else if (first < second)
            {
                return second;
            }
            else
            {
                return 999999999;
            }
        }
        public enum TrxStatus
        {
            Successful = 0,
            Initiated = 1,
            Processing = 2,
            Cancelled = 3,
            AwaitingValidation = 4,
            Failed = 5,
        }
        public enum TrxType
        {
            Vending = 1,
            Lending = 2,
        }
        public static string TransactionNumb()
        {
            try
            {

                return DateTime.Now.ToString("yyyyMMddHHmmss");

            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        public static string SerialNum()
        {
            try
            {
                string RefNum = RandomString(5);
                return RefNum;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }
    }
}
