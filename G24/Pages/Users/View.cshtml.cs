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
    public class ViewModel : PageModel
    {

        public List<User> UserRecords { get; set; }

        [BindProperty(SupportsGet =true)]
        public string Type { get; set; }

        public List<int> AccountType { get; set; } = new List<int> { 0, 1, 2 };

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
                
            }

            DBConnect G24database_connection = new DBConnect();
            string DBconnection = G24database_connection.DatabaseString();
            Console.WriteLine(DBconnection);

            SqlConnection connect = new SqlConnection(DBconnection);
            connect.Open();

            using (SqlCommand command = new SqlCommand())
            {

                command.Connection = connect;
                //sets all new users to a modlevel of 0
                command.CommandText = @"SELECT * FROM Users";

                if (!(string.IsNullOrEmpty(Type) || Type == "ALL"))
                {
                    command.CommandText += " WHERE ModLevel = @accType";
                    command.Parameters.AddWithValue("@accType", Convert.ToInt32(Type));
                }

                SqlDataReader reader = command.ExecuteReader();

                UserRecords = new List<User>();

                while (reader.Read())
                {
                    User record = new User();
                    record.UserID = reader.GetInt32(0);
                    record.FirstName = reader.GetString(1);
                    record.LastName = reader.GetString(2);
                    record.EmailAddress = reader.GetString(3);
                    record.Password = reader.GetString(4);
                    record.ModLevel = reader.GetInt32(5);


                    UserRecords.Add(record);
                }
                reader.Close();

            }

            return Page();
        }

         
    }
}
