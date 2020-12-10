using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using G24.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace G24.Pages.Users
{
    public class CreateModel : PageModel
    {
        [BindProperty]
        public User UserRecord { get; set; }


        public IActionResult OnPost()
        {
            DBConnect G24database_connection = new DBConnect();
            string DBconnection = G24database_connection.DatabaseString();
            Console.WriteLine(DBconnection);

            SqlConnection connect = new SqlConnection(DBconnection);
            connect.Open();

            using (SqlCommand command = new SqlCommand())
            {

                command.Connection = connect;
                //sets all new users to a modlevel of 0
                command.CommandText = @"INSERT INTO Users (FirstName,LastName,EmailAddress,Password) VALUES ( @FName, @LName, @Email, @Password)";


                command.Parameters.AddWithValue("@FName", UserRecord.FirstName);
                command.Parameters.AddWithValue("@LName", UserRecord.LastName);
                command.Parameters.AddWithValue("@Email", UserRecord.EmailAddress);
                command.Parameters.AddWithValue("@Password", UserRecord.Password);

                Console.WriteLine(UserRecord.FirstName);
                Console.WriteLine(UserRecord.LastName);
                Console.WriteLine(UserRecord.EmailAddress);
                Console.WriteLine(UserRecord.Password);


                command.ExecuteNonQuery();

            }
            connect.Close();


            return RedirectToPage("/Index");
        }


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
            ActiveRecord.Active_Sesson = false;

            if (string.IsNullOrEmpty(ActiveRecord.Active_EmailAddress) && string.IsNullOrEmpty(ActiveRecord.Active_FirstName) && string.IsNullOrEmpty(ActiveRecord.Active_SessionID))
            {
                ActiveRecord.Active_Sesson = true;
                return RedirectToPage("/Users/Index");
            }

            return Page();
        }

    }
   
}
