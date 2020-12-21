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


        public Int32 uploadID;
        public IActionResult OnGet()
        {
            // get session variables
            ActiveRecord = new SessionActive();

            ActiveRecord.Active_SessionID = HttpContext.Session.GetString(Session_SessionID);
            ActiveRecord.Active_EmailAddress = HttpContext.Session.GetString(Session_EmailAddress);
            ActiveRecord.Active_FirstName = HttpContext.Session.GetString(Session_FirstName);
            ActiveRecord.Active_UserID = HttpContext.Session.GetInt32(Session_UserID);
            ActiveRecord.Active_ModLevel = HttpContext.Session.GetInt32(Session_ModLevel);

            // if session isn't active then redirect to login page
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


            ActiveRecord = new SessionActive();

            ActiveRecord.Active_SessionID = HttpContext.Session.GetString(Session_SessionID);
            ActiveRecord.Active_EmailAddress = HttpContext.Session.GetString(Session_EmailAddress);
            ActiveRecord.Active_FirstName = HttpContext.Session.GetString(Session_FirstName);
            ActiveRecord.Active_ModLevel = HttpContext.Session.GetInt32(Session_ModLevel);

            // if session isn't active then redirect to login page
            if (string.IsNullOrEmpty(ActiveRecord.Active_EmailAddress) && string.IsNullOrEmpty(ActiveRecord.Active_FirstName) && string.IsNullOrEmpty(ActiveRecord.Active_SessionID))
            {

                ActiveRecord.Active_Sesson = false;
                return RedirectToPage("/Login/Login");
            }
            else
            {
                ActiveRecord.Active_Sesson = true;

            }



            // get database connection
            DBConnect G24database_connection = new DBConnect();
            string DBconnection = G24database_connection.DatabaseString();

            SqlConnection connect = new SqlConnection(DBconnection);
            connect.Open();

            //create the file path to upload the image to
            const string Path2 = "ImgUploads";
            var FileToUpload = Path.Combine(_env.WebRootPath, Path2, ImgFile.FileName);

            // upload the image on the filestream
            using (var Fstream = new FileStream(FileToUpload, FileMode.Create))
            {
                ImgFile.CopyTo(Fstream);
            }




            using (SqlCommand command = new SqlCommand())
            {

                command.Connection = connect;
                // insert the images data into the database
                command.CommandText = @"INSERT INTO Images ( ImgURL, Type, ImgName, UserID) VALUES ( @ImgURL, @Type, @ImgName, @UserID)";

                command.Parameters.AddWithValue("@ImgURL", ImgFile.FileName);
                command.Parameters.AddWithValue("@Type", Tidy_case(ImgRecord.Type));
                command.Parameters.AddWithValue("@ImgName", ImgRecord.ImgName);
                command.Parameters.AddWithValue("@UserID", ImgRecord.UserID);

                command.ExecuteNonQuery();

            }


            using (SqlCommand ID_command = new SqlCommand())
            {

                ID_command.Connection = connect;
                ID_command.CommandText = @"SELECT ImgID FROM Images WHERE ImgURL = @ImgURL";


                ID_command.Parameters.AddWithValue("@ImgURL", ImgFile.FileName);
                SqlDataReader ID_reader = ID_command.ExecuteReader();

                while (ID_reader.Read())
                {

                    uploadID = ID_reader.GetInt32(0);
                }

            }
            //close database connection
            connect.Close();
        
           // redirects to the image page for the image uploaded passing through the image ID
            return RedirectToPage("/ImgController/Index", new { imgid = uploadID });
        }

        // change the first letter to uppercase and the rest to lower case
        public string Tidy_case(string str)
        {
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(str.ToLower());
        }



    }
   
}
