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

            }
            else
            {
                ActiveRecord.Active_Sesson = true;
                if (ActiveRecord.Active_ModLevel != 1)
                {
                    return RedirectToPage("/Users/Index");
                }
            }

            return Page();
        }

        [BindProperty]
        public User UserRecord { get; set; }

        public string SessionID;

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
                command.CommandText = @"INSERT INTO Users (FirstName,LastName,EmailAddress,Password, ModLevel) VALUES ( @FName, @LName, @Email, @Password, @Mlvl)";


                command.Parameters.AddWithValue("@FName", UserRecord.FirstName);
                command.Parameters.AddWithValue("@LName", UserRecord.LastName);
                command.Parameters.AddWithValue("@Email", UserRecord.EmailAddress);
                command.Parameters.AddWithValue("@Password", UserRecord.Password);
                command.Parameters.AddWithValue("@Mlvl", UserRecord.ModLevel);


                Console.WriteLine(UserRecord.FirstName);
                Console.WriteLine(UserRecord.LastName);
                Console.WriteLine(UserRecord.EmailAddress);
                Console.WriteLine(UserRecord.Password);
                Console.WriteLine(UserRecord.ModLevel);
                   
                    
               


                    command.ExecuteNonQuery();

            }
            connect.Close();

            SessionID = HttpContext.Session.Id;
            HttpContext.Session.SetString("sessionID", SessionID);
            HttpContext.Session.SetString("emailAddress", UserRecord.EmailAddress);
            HttpContext.Session.SetString("firstName", UserRecord.FirstName);
            HttpContext.Session.SetInt32("modLevel", UserRecord.ModLevel);
            HttpContext.Session.SetInt32("userID", UserRecord.UserID);


            return RedirectToPage("/Index");
        }

  


    }
   
}
