using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using G24.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace G24.Pages.Logout
{
    public class LogoutModel : PageModel
    {
        [BindProperty]
        public SessionActive ActiveRecord { get; set; }

        public void OnGet()
        {

            ActiveRecord = new SessionActive();
            ActiveRecord.Active_Sesson = false;

            HttpContext.Session.Clear();
        }
    }
}
