using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace G24.Pages.Login
{
    public class LoginModel : PageModel
    {
        public User User { get; set; }
        public String Message { get; set; }

        //public string SessionID;

        public IActionResult OnPost()
        {
            string G24database_connection = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Nate\source\repos\G24\G24\Data\G24Database.mdf;Integrated Security=True;Connect Timeout=30";
            SqlConnection connect = new SqlConnection(G24database_connection);
            connect.Open();

            Console.WriteLine(User.EmailAddress);
            Console.WriteLine(User.Password);

            using (SqlCommand command = new SqlCommand())
            {
                command.Connection = connect;
                //sets all new users to a modlevel of 0
                command.CommandText = "SELECT UserID,FirstName,LastName,EmailAddress,ModLevel FROM Users WHERE EmailAddress = @Email AND Password = @Pwd";

                command.Parameters.AddWithValue("@Email", User.EmailAddress);
                command.Parameters.AddWithValue("@Pwd", User.Password);

                //SqlDataReader reader = command.ExecuteReader();
                var reader = command.ExecuteReader();

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

            if (!string.IsNullOrEmpty(User.FirstName))
            {
               // SessionID = HttpContext.Session.Id;
               // HttpContext.Session.SetString("sessionID", SessionID);
               // HttpContext.Session.SetString("emailAddress", User.EmailAddress);
               // HttpContext.Session.SetString("firstName", User.FirstName);


                if (User.ModLevel == "1")
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
                Message = "Invalid Username And Passowrd"
                return Page();
            }

        }


        public void OnGet()
        {
        }
    }
}
