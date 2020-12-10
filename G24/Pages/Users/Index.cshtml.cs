using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using G24.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace G24.Pages.Users
{
    public class IndexModel : PageModel
    {

        /*
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
            
            */
        [BindProperty]
        public SessionActive ActiveRecord { get; set; }

        public const string Session_SessionID = "sessionID";
        public const string Session_EmailAddress = "emailAddress";
        public const string Session_FirstName = "firstName";
        public const string Session_ModLevel = "modLevel";

        public IActionResult OnGet()
        {

            ActiveRecord = new SessionActive();

            ActiveRecord.Active_SessionID = HttpContext.Session.GetString(Session_SessionID);
            ActiveRecord.Active_EmailAddress = HttpContext.Session.GetString(Session_EmailAddress);
            ActiveRecord.Active_FirstName = HttpContext.Session.GetString(Session_FirstName);
            ActiveRecord.Active_ModLevel = HttpContext.Session.GetInt32(Session_ModLevel);

            if (string.IsNullOrEmpty(ActiveRecord.Active_EmailAddress) && string.IsNullOrEmpty(ActiveRecord.Active_FirstName) && string.IsNullOrEmpty(ActiveRecord.Active_SessionID))
            {
                ActiveRecord.Active_Sesson = false;
                return RedirectToPage("/Login/Login");
            }
            else
            {
                ActiveRecord.Active_Sesson = true;
                return Page();
            }

           
        }
    }
}
