using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UvlotApp.Classes
{
    public class InstitutionModel
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string InstitutionAddress { get; set; }
        public string InstitutionEmailAddress { get; set; }
        public string InstitutionPhoneNo { get; set; }
        public string ContactPhoneNo { get; set; }
        public string ContactEmailAddress { get; set; }
        public string HeadOfInstition { get; set; }
        public int IsVisible { get; set; }
        public string ValueDate { get; set; }
        public string ValueTime { get; set; }
        public DateTime DateCreated { get; set; }
    }

    public class Page
    {
        public int PageID { get; set; }
        public string PageName { get; set; }
        public Nullable<int> IsVisible { get; set; }
        public string ValueDate { get; set; }
        public string PageDescription { get; set; }
        public string PageUrl { get; set; }
        public string PageHeader { get; set; }
    }

    public class Role
    {
        
        public int RoleId { get; set; }

       
        public string RoleName { get; set; }

       
        public int isVissible { get; set; }

        
        public DateTime Date { get; set; }
       
        public string RoleDescription { get; set; }


    }

    public class GetAssignPages
    {
        public int pageid { get; set; }
        public string Roleid { get; set; }
        public string Rolename { get; set; }



    }

    public class getAllUserAndRoles
    {
        public int userid { get; set; }
        public int roleid { get; set; }
        public string rolename { get; set; }
        public string email { get; set; }
        public int id { get; set; }
    }


    public class getAllPagesAndRoles
    {
        // public int pageid { get; set; }
        public int roleid { get; set; }
        public string rolename { get; set; }
        public string pageName { get; set; }
        public int id { get; set; }






    }
}