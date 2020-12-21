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
    public class UpdateModel : PageModel
    {
        [BindProperty]
        public User UserRecord { get; set; }

        [BindProperty]
        public SessionActive ActiveRecord { get; set; }

        public const string Session_SessionID = "sessionID";
        public const string Session_EmailAddress = "emailAddress";
        public const string Session_FirstName = "firstName";
        public const string Session_ModLevel = "modLevel";


        public IActionResult OnGet(int?id)
        {

            ActiveRecord = new SessionActive();
            // gets the active session records
            ActiveRecord.Active_SessionID = HttpContext.Session.GetString(Session_SessionID);
            ActiveRecord.Active_EmailAddress = HttpContext.Session.GetString(Session_EmailAddress);
            ActiveRecord.Active_FirstName = HttpContext.Session.GetString(Session_FirstName);
            ActiveRecord.Active_ModLevel = HttpContext.Session.GetInt32(Session_ModLevel);
            
            // checks if a session exists 
            if (string.IsNullOrEmpty(ActiveRecord.Active_EmailAddress) && string.IsNullOrEmpty(ActiveRecord.Active_FirstName) && string.IsNullOrEmpty(ActiveRecord.Active_SessionID))
            {
                // if no session exists sends users to login
                ActiveRecord.Active_Sesson = false;
                return RedirectToPage("/Login/Login");
            }
            else
            {
                ActiveRecord.Active_Sesson = true;
                if (ActiveRecord.Active_ModLevel != 1)
                {
                    // if logged in and not a admin direct to account page
                    return RedirectToPage("/Users/Index");
                }
            }

            // creates connection to database
            DBConnect G24database_connection = new DBConnect();
            string DBconnection = G24database_connection.DatabaseString();
            
            // opens an sql connection
            SqlConnection connect = new SqlConnection(DBconnection);
            connect.Open();

            UserRecord = new User();

            using (SqlCommand command = new SqlCommand())
            {
               
                command.Connection = connect;
                // selects all users from the database where the user id = ID 
                command.CommandText = "SELECT * FROM Users WHERE UserID = @ID";

                command.Parameters.AddWithValue("@ID", id);

                SqlDataReader reader = command.ExecuteReader();

                //writes records into the user record array 
                while (reader.Read())
                {
                    UserRecord.UserID = reader.GetInt32(0);
                    UserRecord.FirstName = reader.GetString(1);
                    UserRecord.LastName = reader.GetString(2);
                    UserRecord.EmailAddress = reader.GetString(3);
                    UserRecord.Password = reader.GetString(4);
                    UserRecord.ModLevel = reader.GetInt32(5);

                }



            }
            connect.Close();

            // returns the page
            return Page();
        }

        public IActionResult OnPost()
        {
            DBConnect G24database_connection = new DBConnect();
            string DBconnection = G24database_connection.DatabaseString();
            

            SqlConnection connect = new SqlConnection(DBconnection);
            connect.Open();


            using (SqlCommand command = new SqlCommand())
            {

                command.Connection = connect;
                // Update Users where the ID matches
                command.CommandText = "UPDATE Users SET FirstName = @FName, LastName = @LName, EmailAddress = @Email, Password = @Password, ModLevel = @ModLevel WHERE UserID = @UID";

                command.Parameters.AddWithValue("@UID", UserRecord.UserID);
                command.Parameters.AddWithValue("@FName", UserRecord.FirstName);
                command.Parameters.AddWithValue("@LName", UserRecord.LastName);
                command.Parameters.AddWithValue("@Email", UserRecord.EmailAddress);
                command.Parameters.AddWithValue("@Password", UserRecord.Password);
                command.Parameters.AddWithValue("@ModLevel", UserRecord.ModLevel);

                // execute  
                command.ExecuteNonQuery();

            }

            connect.Close();


            return RedirectToPage("/Users/View");
        }




    }
   
}
