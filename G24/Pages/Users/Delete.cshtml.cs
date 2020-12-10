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
    public class DeleteModel : PageModel
    {
        [BindProperty]
        public User UserRecord { get; set; }

        [BindProperty]
        public SessionActive ActiveRecord { get; set; }

        public const string Session_SessionID = "sessionID";
        public const string Session_EmailAddress = "emailAddress";
        public const string Session_FirstName = "firstName";
        public const string Session_ModLevel = "modLevel";

        public IActionResult OnGet(int? id)
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
            }

            DBConnect G24database_connection = new DBConnect();
            string DBconnection = G24database_connection.DatabaseString();
            Console.WriteLine(DBconnection);

            SqlConnection connect = new SqlConnection(DBconnection);
            connect.Open();

            UserRecord = new User();

            using (SqlCommand command = new SqlCommand())
            {
                command.Connection = connect;
                //sets all new users to a modlevel of 0
                command.CommandText = "SELECT * FROM Users WHERE UserID = @ID";

                command.Parameters.AddWithValue("@ID", id);

                Console.WriteLine("The id: " + id);

                SqlDataReader reader = command.ExecuteReader();

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


            return Page();
        }

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
                command.CommandText = "DELETE Users WHERE UserID = @UID";

                command.Parameters.AddWithValue("@UID", UserRecord.UserID);
                command.ExecuteNonQuery();

            }

            connect.Close();


            return RedirectToPage("/Users/View");
        }




    }

}