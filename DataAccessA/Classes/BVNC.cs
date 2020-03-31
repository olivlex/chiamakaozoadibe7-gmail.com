using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessA.Classes
{
    public partial class BVNC
    {

        public string respCode { get; set; }

        public string respDescription { get; set; }
        public string BVN { get; set; }

        public string Phone { get; set; }

        public string FirstNAme { get; set; }

        public string LastName { get; set; }
        public string MiddleName { get; set; }

        public string Dateofbirth { get; set; }
        public string RegistrationDate { get; set; }

        public string EnrollmentBank { get; set; }

        public string EnrollmentBranch { get; set; }


        public string gender { get; set; }

        public string Nationality { get; set; }


        public string image_base_64 { get; set; }

        public string address { get; set; }

        public string email { get; set; }

        public string watch_listed { get; set; }

        public string marital_status { get; set; }

        public string state_of_residence { get; set; }

        public string lga_of_residence { get; set; }

        public string errormessage { get; set; }

    }
}
