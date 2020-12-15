using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using G24.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.IO;
using System.Globalization;

namespace G24.Pages.ImgController
{
    public class UploadModel : PageModel
    {
        [BindProperty]
        public Images ImgRecord { get; set; }

        [BindProperty]
        public IFormFile ImgFile { get; set; }

        public readonly IWebHostEnvironment _env;

        public UploadModel(IWebHostEnvironment env)
        {
            _env = env;
        }

        [BindProperty]
        public SessionActive ActiveRecord { get; set; }

        public const string Session_SessionID = "sessionID";
        public const string Session_EmailAddress = "emailAddress";
        public const string Session_FirstName = "firstName";
        public const string Session_ModLevel = "modLevel";
        public const string Session_UserID = "userID";

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
                return RedirectToPage("/Login/Login");
            }
            else
            {
                ActiveRecord.Active_Sesson = true;

            }

            return Page();
        }



        public IActionResult OnPost()
        {
            DBConnect G24database_connection = new DBConnect();
            string DBconnection = G24database_connection.DatabaseString();
            Console.WriteLine(DBconnection);

            SqlConnection connect = new SqlConnection(DBconnection);
            connect.Open();

            const string Path2 = "ImgUploads";
            var FileToUpload = Path.Combine(_env.WebRootPath, Path2, ImgFile.FileName);
            Console.WriteLine("File name" + FileToUpload);

            using(var Fstream = new FileStream(FileToUpload, FileMode.Create))
            {
                ImgFile.CopyTo(Fstream);
            }




            using (SqlCommand command = new SqlCommand())
            {

                command.Connection = connect;
                //sets all new users to a modlevel of 0
                command.CommandText = @"INSERT INTO Images ( ImgURL, Type, ImgName, UserID) VALUES ( @ImgURL, @Type, @ImgName, @UserID)";

                
                command.Parameters.AddWithValue("@ImgURL", ImgFile.FileName);
                command.Parameters.AddWithValue("@Type", Tidy_case(ImgRecord.Type));
                command.Parameters.AddWithValue("@ImgName", ImgRecord.ImgName);
                command.Parameters.AddWithValue("@UserID", ImgRecord.UserID);

               
                Console.WriteLine(ImgFile.FileName);
                Console.WriteLine(ImgRecord.Type);
                Console.WriteLine(ImgRecord.ImgName);
                Console.WriteLine(ImgRecord.UserID);


                command.ExecuteNonQuery();

            }
            connect.Close();

            ActiveRecord = new SessionActive();

            ActiveRecord.Active_SessionID = HttpContext.Session.GetString(Session_SessionID);
            ActiveRecord.Active_EmailAddress = HttpContext.Session.GetString(Session_EmailAddress);
            ActiveRecord.Active_FirstName = HttpContext.Session.GetString(Session_FirstName);
            ActiveRecord.Active_ModLevel = HttpContext.Session.GetInt32(Session_ModLevel);


            if (string.IsNullOrEmpty(ActiveRecord.Active_EmailAddress) && string.IsNullOrEmpty(ActiveRecord.Active_FirstName) && string.IsNullOrEmpty(ActiveRecord.Active_SessionID))
            {

                ActiveRecord.Active_Sesson = false;
                return RedirectToPage("/Login/Login");
            }
            else
            {
                ActiveRecord.Active_Sesson = true;

            }
            return Page();
        }

        // change the first letter to uppercase and the rest to lower case
        public string Tidy_case(string str)
        {
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(str.ToLower());
        }



    }
   
}
