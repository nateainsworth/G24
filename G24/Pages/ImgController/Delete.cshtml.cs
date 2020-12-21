using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using G24.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace G24.Pages.ImgController
{
    public class DeleteModel : PageModel
    {
        [BindProperty]
        public Images ImgRecord { get; set; }
        public ImgFile ImgFile { get; set; }

        public readonly IWebHostEnvironment _env;

        //a constructor for the class
        public DeleteModel(IWebHostEnvironment env)
        {
            _env = env;
        }


        [BindProperty]
        public SessionActive ActiveRecord { get; set; }

        public const string Session_SessionID = "sessionID";
        public const string Session_EmailAddress = "emailAddress";
        public const string Session_FirstName = "firstName";
        public const string Session_ModLevel = "modLevel";

        public IActionResult OnGet(int? id)
        {
            // get session variables
            ActiveRecord = new SessionActive();

            ActiveRecord.Active_SessionID = HttpContext.Session.GetString(Session_SessionID);
            ActiveRecord.Active_EmailAddress = HttpContext.Session.GetString(Session_EmailAddress);
            ActiveRecord.Active_FirstName = HttpContext.Session.GetString(Session_FirstName);
            ActiveRecord.Active_ModLevel = HttpContext.Session.GetInt32(Session_ModLevel);

            // if session isn't active then redirect to login page if not a mod then send to account page
            if (string.IsNullOrEmpty(ActiveRecord.Active_EmailAddress) && string.IsNullOrEmpty(ActiveRecord.Active_FirstName) && string.IsNullOrEmpty(ActiveRecord.Active_SessionID))
            {
                ActiveRecord.Active_Sesson = false;
                return RedirectToPage("/Login/Login");
            }
            else
            {
                ActiveRecord.Active_Sesson = true;
                if (ActiveRecord.Active_ModLevel != 1)
                {
                    return RedirectToPage("/Users/Index");
                }
            }

            //opensa a database connection
            DBConnect G24database_connection = new DBConnect();
            string DBconnection = G24database_connection.DatabaseString();
           

            SqlConnection connect = new SqlConnection(DBconnection);
            connect.Open();

            ImgRecord = new Images();

            using (SqlCommand command = new SqlCommand())
            {
                command.Connection = connect;
                //select the details for the image which is being deleted.
                command.CommandText = "SELECT * FROM Images WHERE ImgID = @ID";

                command.Parameters.AddWithValue("@ID", id);

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
            
            deleteImg(ImgRecord.ImgID, ImgRecord.ImgURL);

            return RedirectToPage("/ImgController/View");
        }

        public void deleteImg(int ImgID, string FileName)
        {
            //create a database connection
            DBConnect G24database_connection = new DBConnect();
            string DBconnection = G24database_connection.DatabaseString();

            SqlConnection connect = new SqlConnection(DBconnection);
            connect.Open();

            using (SqlCommand command = new SqlCommand())
            {

                command.Connection = connect;
                // delete from images where the img id matches the id within the hidden input in the form element
                command.CommandText = "DELETE FROM Images WHERE ImgID = @ImgID";

                command.Parameters.AddWithValue("@ImgID", ImgID);
                command.ExecuteNonQuery();

            }

            // get the file path and delete the image file assosiated to the line in the database being deleted
            string ImgPath = Path.Combine(_env.WebRootPath, "ImgUploads", FileName);
            System.IO.File.Delete(ImgPath);

            connect.Close();

        }



    }

}