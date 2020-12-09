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
    public class UpdateModel : PageModel
    {
        [BindProperty]
        public User UserRecord { get; set; }


        public IActionResult OnGet(int?id)
        {
            string G24database_connection = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Nate\source\repos\G24\G24\Data\G24Database.mdf;Integrated Security=True;Connect Timeout=30";
            SqlConnection connect = new SqlConnection(G24database_connection);
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
            string G24database_connection = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Nate\source\repos\G24\G24\Data\G24Database.mdf;Integrated Security=True;Connect Timeout=30";
            SqlConnection connect = new SqlConnection(G24database_connection);
            connect.Open();

            Console.WriteLine("User ID" + UserRecord.UserID);
            Console.WriteLine("User First Name" + UserRecord.FirstName);
            Console.WriteLine("User Last Name" + UserRecord.LastName);
            Console.WriteLine("User Email Address" + UserRecord.EmailAddress);
            Console.WriteLine("User Password" + UserRecord.Password);
            Console.WriteLine("User Mod Level" + UserRecord.ModLevel);


            using (SqlCommand command = new SqlCommand())
            {

                command.Connection = connect;
                //sets all new users to a modlevel of 0
                command.CommandText = "UPDATE Users SET FirstName = @FName, LastName = @LName, EmailAddress = @Email, Password = @Password, ModLevel = @ModLevel WHERE UserID = @UID";

                command.Parameters.AddWithValue("@UID", UserRecord.UserID);
                command.Parameters.AddWithValue("@FName", UserRecord.FirstName);
                command.Parameters.AddWithValue("@LName", UserRecord.LastName);
                command.Parameters.AddWithValue("@Email", UserRecord.EmailAddress);
                command.Parameters.AddWithValue("@Password", UserRecord.Password);
                command.Parameters.AddWithValue("@ModLevel", UserRecord.ModLevel);


                command.ExecuteNonQuery();

            }

            connect.Close();


            return RedirectToPage("/Users/View");
        }




    }
   
}
