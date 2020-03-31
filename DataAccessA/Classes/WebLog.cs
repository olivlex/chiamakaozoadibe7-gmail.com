using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Web;


public class WebLog : Exception
	{
		public WebLog()
		{
			Log(new Exception());
		}

		public WebLog(Exception exception)
		{
			Log(exception);
		}

		public static void Log(Exception exception)
		{
			var localDate = DateTime.Now.ToString("yyyy/MM/dd");
		    var localTime = DateTime.Now.ToString("HH:mm:ss");
			var errorDateTime = localDate + " @ " + localTime;

			try
			{
  
            HttpContext context = HttpContext.Current;
            
            // Get location of ErrorLogFile from Web.config file
            var filePath = context.Server.MapPath(Convert.ToString(ConfigurationManager.AppSettings["ErrorLogFile"]));


             //   var filePath = Convert.ToString(ConfigurationManager.AppSettings["ErrorLogFile"]);
				var file = new FileInfo(filePath);
			    file.Directory?.Create();

			    var stackTrace = new StackTrace(exception, true);
				var methodname = stackTrace.GetFrame(0).GetMethod().Name;
				var declaringType = stackTrace.GetFrame(0).GetMethod().DeclaringType;
				var lineNumber = stackTrace.GetFrame(0).GetFileLineNumber();

				var sw = new StreamWriter(filePath, true);
				sw.WriteLine("--------------------------");
				sw.WriteLine(errorDateTime);
				sw.WriteLine("--------------------------");

				if (declaringType != null)
				{
					sw.WriteLine("Executing Assembly: {0}", declaringType.AssemblyQualifiedName);
				}
				sw.WriteLine("Executing Method: {0}", methodname);
				sw.WriteLine("Executing Line Number: {0}", lineNumber);
				sw.WriteLine("Exeption Message: {0}", exception.Message);
				if (exception.InnerException != null)
				{
					sw.WriteLine("Inner Exeption: {0}", exception.InnerException);
				}
				sw.WriteLine();
				sw.Close();
			}
			catch (Exception ex)
			{
				Debug.Write(ex.Message);
			}
		}

		public static void Log(string message,  string moduleName="")
		{
            var localDate = DateTime.Now.ToString("yyyy/MM/dd");
            var localTime = DateTime.Now.ToString("HH:mm:ss");
            var errorDateTime = localDate + " @ " + localTime;

			try
			{
                HttpContext context = HttpContext.Current;

                // Get location of ErrorLogFile from Web.config file
                string filePath = context.Server.MapPath(Convert.ToString(ConfigurationManager.AppSettings["ErrorLogFile"]));

                var file = new FileInfo(filePath);
			    file.Directory?.Create();

			    var sw = new StreamWriter(filePath, true);
				sw.WriteLine("--------------------------");
				sw.WriteLine(errorDateTime);
				sw.WriteLine("--------------------------");
				sw.WriteLine("Message: {0}", message);
				sw.WriteLine();
				sw.Close();
			}
			catch (Exception ex)
			{
				Debug.Write(ex.Message);
			}
		}
	}
