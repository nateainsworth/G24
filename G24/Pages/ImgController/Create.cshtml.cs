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
    public class CreateModel : PageModel
    {
        [BindProperty]
        public Images ImgRecord { get; set; }


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
                command.CommandText = @"INSERT INTO Users (ImgID, ImgURL, Type, ImgName, UserID) VALUES ( @ImgID, @ImgURL, @Type, @ImgName, @UserID)";


                command.Parameters.AddWithValue("@ImgID", ImgRecord.ImgID);
                command.Parameters.AddWithValue("@ImgURL", ImgRecord.ImgURL);
                command.Parameters.AddWithValue("@Type", ImgRecord.Type);
                command.Parameters.AddWithValue("@ImgName", ImgRecord.ImgName);
                command.Parameters.AddWithValue("@UserID", ImgRecord.UserID);

                Console.WriteLine(ImgRecord.ImgID);
                Console.WriteLine(ImgRecord.ImgURL);
                Console.WriteLine(ImgRecord.Type);
                Console.WriteLine(ImgRecord.ImgName);
                Console.WriteLine(ImgRecord.UserID);


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
            

            if (string.IsNullOrEmpty(ActiveRecord.Active_EmailAddress) && string.IsNullOrEmpty(ActiveRecord.Active_FirstName) && string.IsNullOrEmpty(ActiveRecord.Active_SessionID))
            {
                ActiveRecord.Active_Sesson = false;
                
            }
            else
            {
                ActiveRecord.Active_Sesson = true;
                return RedirectToPage("/ImgController/Index");
            }

            return Page();
        }

    }
   
}
