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

namespace G24.Pages.Login
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public User UserRecord { get; set; }
        public String Message { get; set; }

        public string SessionID;

        public IActionResult OnPost()
        {
            DBConnect G24database_connection = new DBConnect();
            string DBconnection = G24database_connection.DatabaseString();
            Console.WriteLine(DBconnection);

            SqlConnection connect = new SqlConnection(DBconnection);
            connect.Open();

            //UserRecord = new User();

            Console.WriteLine(UserRecord.EmailAddress);
            Console.WriteLine(UserRecord.Password);

            using (SqlCommand command = new SqlCommand())
            {


                command.Connection = connect;
                //sets all new users to a modlevel of 0
                command.CommandText = "SELECT UserID,FirstName,LastName,ModLevel FROM Users WHERE EmailAddress = @Email AND Password = @Pwd";

                command.Parameters.AddWithValue("@Email", UserRecord.EmailAddress);
                command.Parameters.AddWithValue("@Pwd", UserRecord.Password);

                //SqlDataReader reader = command.ExecuteReader();
                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    UserRecord.UserID = reader.GetInt32(0);
                    UserRecord.FirstName = reader.GetString(1);
                    UserRecord.LastName = reader.GetString(2);
                    //UserRecord.EmailAddress = reader.GetString(3);
                   // UserRecord.Password = reader.GetString(4);
                    UserRecord.ModLevel = reader.GetInt32(3);

                }

            }

            if (!string.IsNullOrEmpty(UserRecord.FirstName))
            {
               SessionID = HttpContext.Session.Id;
               HttpContext.Session.SetString("sessionID", SessionID);
               HttpContext.Session.SetString("emailAddress", UserRecord.EmailAddress);
               HttpContext.Session.SetString("firstName", UserRecord.FirstName);


                if (UserRecord.ModLevel == 1)
                {
                    return RedirectToPage("Users/View");
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


        public void OnGet()
        {
        }
    }
}
