using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using G24.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace G24.Pages.ImgController
{
    public class DeleteModel : PageModel
    {
        [BindProperty]
        public Images ImgRecord { get; set; }

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

            ImgRecord = new Images();

            using (SqlCommand command = new SqlCommand())
            {
                command.Connection = connect;
                //sets all new users to a modlevel of 0
                command.CommandText = "SELECT * FROM Images WHERE ImgID = @ID";

                command.Parameters.AddWithValue("@ID", id);

                Console.WriteLine("The id: " + id);

                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    ImgRecord.ImgID = reader.GetInt32(0);
                    ImgRecord.ImgURL = reader.GetString(1);
                    ImgRecord.Type = reader.GetString(2);
                    ImgRecord.ImgName = reader.GetString(3);
                    ImgRecord.UserID = reader.GetInt32(4);

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
                command.CommandText = "DELETE Images WHERE ImgID = @ImgID";

                command.Parameters.AddWithValue("@ImgID", ImgRecord.UserID);
                command.ExecuteNonQuery();

            }

            connect.Close();


            return RedirectToPage("/Users/View");
        }




    }

}