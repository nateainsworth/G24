using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace G24.Pages.Users
{
    public class IndexModel : PageModel
    {


        public string SessionID;
        public const string Session_SessionID = "sessionID";
        public string EmailAddress;
        public const string Session_EmailAddress = "emailAddress";
        public string FirstName;
        public const string Session_FirstName = "firstName";
        public int? ModLevel;
        public const string Session_ModLevel = "modLevel";


        public IActionResult OnGet()
        {
            SessionID = HttpContext.Session.GetString(Session_SessionID);
            EmailAddress = HttpContext.Session.GetString(Session_EmailAddress);
            FirstName = HttpContext.Session.GetString(Session_FirstName);
            ModLevel = HttpContext.Session.GetInt32(Session_ModLevel);
            
            
            if (string.IsNullOrEmpty(EmailAddress) && string.IsNullOrEmpty(FirstName) && string.IsNullOrEmpty(SessionID))
            {
                return RedirectToPage("/Login/Login");
            }

            return Page();
        }
    }
}
