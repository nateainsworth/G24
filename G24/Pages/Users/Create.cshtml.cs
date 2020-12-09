using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using G24.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace G24.Pages.Users
{
    public class CreateModel : PageModel
    {
        [BindProperty]
        public User UserRecord { get; set; }


        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            string G24database_connection = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Nate\source\repos\G24\G24\Data\G24Database.mdf;Integrated Security=True;Connect Timeout=30";
            SqlConnection connect = new SqlConnection(G24database_connection);
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




    }
   
}
