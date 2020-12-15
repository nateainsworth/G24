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
    public class ViewModel : PageModel
    {

        public readonly IWebHostEnvironment _env;

        //a constructor for the class
        public ViewModel(IWebHostEnvironment env)
        {
            _env = env;
        }

        [BindProperty]
        public List<Images> Img { get; set; }
        
        [BindProperty]
        public List<bool> IsSelect { get; set; }

        
        public List<Images> ImgToDelete { get; set; }

        public List<Images> ImgRecords { get; set; }

        [BindProperty(SupportsGet =true)]
        public string Type { get; set; }

        public List<int> ImageType { get; set; } = new List<int> { 0, 1, 2 };

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

            DBConnect G24database_connection = new DBConnect();
            string DBconnection = G24database_connection.DatabaseString();
            Console.WriteLine(DBconnection);

            SqlConnection connect = new SqlConnection(DBconnection);
            connect.Open();

            using (SqlCommand command = new SqlCommand())
            {

                command.Connection = connect;
                command.CommandText = @"SELECT * FROM Images";


                if (!(string.IsNullOrEmpty(Type) || Type == "ALL"))
                {
                    command.CommandText += " WHERE Type = @ImgType";
                    command.Parameters.AddWithValue("@ImgType",Type);
                }

                SqlDataReader reader = command.ExecuteReader();

                Img = new List<Images>();
                IsSelect = new List<bool>();
                while (reader.Read())
                {
                    Images record = new Images();
                    record.ImgID = reader.GetInt32(0);
                    record.ImgURL = reader.GetString(1);
                    record.Type = reader.GetString(2);
                    record.ImgName = reader.GetString(3);
                    record.UserID = reader.GetInt32(4);


                    Img.Add(record);
                    IsSelect.Add(false);
                }

               reader.Close();
                

            }

            return Page();
        }


        public IActionResult OnPost()
        {
            ImgToDelete = new List<Images>();
            for(int i = 0; i < Img.Count; i++)
            {
                if(IsSelect[i] == true)
                {
                    ImgToDelete.Add(Img[i]);
                }
            }
            for(int i=0; i < ImgToDelete.Count(); i++)
            {
                deleteImg(ImgToDelete[i].ImgID, ImgToDelete[i].ImgURL);
            }
            
            return RedirectToPage("/ImgController/View");
        }

        public void deleteImg(int ImgID, string FileName)
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
                command.CommandText = "DELETE FROM Images WHERE ImgID = @ImgID";

                command.Parameters.AddWithValue("@ImgID", ImgID);
                command.ExecuteNonQuery();

            }

            string ImgPath = Path.Combine(_env.WebRootPath, "ImgUploads", FileName);
            System.IO.File.Delete(ImgPath);
            Console.WriteLine("Image Deleted");

            connect.Close();

        }



    }
}
