using DataAccessA.DataManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UvlotExt.Classes
{
    public class LogginHelper
    {
        UvlotAEntities db = new UvlotAEntities();
        public string LoggedInUser()
        {
            try
            {
                string sessionUserId = HttpContext.Current.Session["id"].ToString().Trim();


                return sessionUserId;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message );
                return null;
            }
        }

        public int LoggedInUserID(string email)
        {
            try
            {

                var userid = (from a in db.Users where a.EmailAddress == email select a.ID).FirstOrDefault();

                return userid;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return 0;
            }
        }

        public bool getCurrentUser()
        {
            try
            {
                string sessionUserId = HttpContext.Current.Session["id"].ToString().Trim();

                if (sessionUserId != null)
                {

                    return true;
                }
                else
                {


                    return false;
                }
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message);
                return false;
            }
        }

    }
}