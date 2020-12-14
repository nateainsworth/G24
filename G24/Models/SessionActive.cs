using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace G24.Models
{
    public class SessionActive
    {
        [Display(Name = "Active Session")]
        public string Active_SessionID { get; set; }
        [Display(Name = "First Name")]
        public string Active_FirstName { get; set; }
        [Display(Name = "Email Address")]
        public string Active_EmailAddress { get; set; }
        [Display(Name = "Mod Level")]
        public int? Active_ModLevel { get; set; }
        [Display(Name = "User ID")]
        public int? Active_UserID { get; set; }

        public bool Active_Sesson { get; set; }
        

    }
}
