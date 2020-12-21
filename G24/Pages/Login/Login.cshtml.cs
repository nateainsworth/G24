using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using G24.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using G24.Pages.Users;

namespace G24.Pages.Login
{

    public class LoginModel : PageModel
    {


        [BindProperty]
        public SessionActive ActiveRecord { get; set; }

        public const string Session_SessionID = "sessionID";
        public const string Session_EmailAddress = "emailAddress";
        public const string Session_FirstName = "firstName";
        public const string Session_ModLevel = "modLevel";
        public const string Session_UserID = "userID";

        [BindProperty]
        public User UserRecord { get; set; }
        public String Message { get; set; }

        public string SessionID;

        public IActionResult OnPost()
        {
           
            // connect to the database
            DBConnect G24database_connection = new DBConnect();
            string DBconnection = G24database_connection.DatabaseString();
            

            SqlConnection connect = new SqlConnection(DBconnection);
            connect.Open();

          

            using (SqlCommand command = new SqlCommand())
            {


                command.Connection = connect;
                // select userid firstname, lastname and modlevel but only where the email address and the password entered in the form exist.
                command.CommandText = "SELECT UserID,FirstName,LastName,ModLevel FROM Users WHERE EmailAddress = @Email AND Password = @Pwd";

                command.Parameters.AddWithValue("@Email", UserRecord.EmailAddress);
                command.Parameters.AddWithValue("@Pwd", UserRecord.Password);

                if (string.IsNullOrEmpty(UserRecord.EmailAddress) || string.IsNullOrEmpty(UserRecord.Password))
                {
                    
                }
                else
                {

                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        UserRecord.UserID = reader.GetInt32(0);
                        UserRecord.FirstName = reader.GetString(1);
                        UserRecord.LastName = reader.GetString(2);
                        UserRecord.ModLevel = reader.GetInt32(3);

                    }
                }
            }
            // if nothing is returned from executing the SQL above then it will give you an invalid login error
            // else if a user is returned it will create a session and log the user in directing them to different pages depending on if there an admin or not.
            if (!string.IsNullOrEmpty(UserRecord.FirstName))
            {
               SessionID = HttpContext.Session.Id;
               HttpContext.Session.SetString("sessionID", SessionID);
               HttpContext.Session.SetString("emailAddress", UserRecord.EmailAddress);
               HttpContext.Session.SetString("firstName", UserRecord.FirstName);
               HttpContext.Session.SetInt32("modLevel", UserRecord.ModLevel);
               HttpContext.Session.SetInt32("userID", UserRecord.UserID);



                if (UserRecord.ModLevel == 1)
                {
                    return RedirectToPage("/Users/Index");
                }
                else
                {
                    return RedirectToPage("/Index");
                }
            }
            else
            {
                Message = "Invalid Username And Passowrd";
                return Page();
            }
            
        }


        public IActionResult OnGet()
        {
            // get session variables
            ActiveRecord = new SessionActive();

            ActiveRecord.Active_SessionID = HttpContext.Session.GetString(Session_SessionID);
            ActiveRecord.Active_EmailAddress = HttpContext.Session.GetString(Session_EmailAddress);
            ActiveRecord.Active_FirstName = HttpContext.Session.GetString(Session_FirstName);
            ActiveRecord.Active_ModLevel = HttpContext.Session.GetInt32(Session_ModLevel);
            ActiveRecord.Active_ModLevel = HttpContext.Session.GetInt32(Session_UserID);

            // if session is active dont give access to the login page redirect to the user account page
            if (string.IsNullOrEmpty(ActiveRecord.Active_EmailAddress) && string.IsNullOrEmpty(ActiveRecord.Active_FirstName) && string.IsNullOrEmpty(ActiveRecord.Active_SessionID))
            {
                ActiveRecord.Active_Sesson = false;
            }
            else
            {
                ActiveRecord.Active_Sesson = true;
                return RedirectToPage("/Users/Index");
            }

            return Page();
        }
    }
}
