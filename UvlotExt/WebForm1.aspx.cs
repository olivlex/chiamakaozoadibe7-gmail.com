using DataAccessA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace UvlotExt
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            DateTime mydate =Convert.ToDateTime( "2020/01/01");
            DateTime mydate1 = mydate.AddMonths(1);
            DateTime mydate2 = mydate.AddMonths(2);
            DateTime mydate3 = mydate.AddMonths(3);
        }
    }
}