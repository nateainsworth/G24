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
    public class IndexModel : PageModel
    {
        [BindProperty]
        public Images ImgRecord { get; set; }

        [BindProperty]
        public SessionActive ActiveRecord { get; set; }

        public const string Session_SessionID = "sessionID";
        public const string Session_EmailAddress = "emailAddress";
        public const string Session_FirstName = "firstName";
        public const string Session_ModLevel = "modLevel";

        public String UploadUser;

        public readonly IWebHostEnvironment _env;

        public IndexModel(IWebHostEnvironment env)
        {
            _env = env;
        }


        public async Task<IActionResult> OnGetAsync(int? imgid, int? download)
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

            DBConnect G24database_connection = new DBConnect();
            string DBconnection = G24database_connection.DatabaseString();
            Console.WriteLine(DBconnection);

            SqlConnection connect = new SqlConnection(DBconnection);
            connect.Open();

            ImgRecord = new Images();

            using (SqlCommand command = new SqlCommand())
            {
                command.Connection = connect;

                command.CommandText = "SELECT * FROM Images WHERE ImgID = @ID";

                command.Parameters.AddWithValue("@ID", imgid);

                Console.WriteLine("The id: " + imgid);

                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    ImgRecord.ImgID = reader.GetInt32(0);
                    ImgRecord.ImgURL = reader.GetString(1);
                    ImgRecord.Type = reader.GetString(2);
                    ImgRecord.ImgName = reader.GetString(3);
                    ImgRecord.UserID = reader.GetInt32(4);
                }

                reader.Close();
            }

            if(download == 1)
            {
                /*var path = Path.GetFullPath("./wwwroot/images/school-assets/" + filename);
                //MemoryStream memory = new MemoryStream();
                using (FileStream stream = new FileStream(path, FileMode.Open))
                {
                    //await stream.CopyToAsync(memory);
                }
                memory.Position = 0;
                return File(memory, "image/png", Path.GetFileName(path));
                */
                // ************************************************************************

                const string Path2 = "ImgUploads";
                var FileToDownload = Path.Combine(_env.WebRootPath, Path2, ImgRecord.ImgURL);
                MemoryStream memory = new MemoryStream();
                using (FileStream Fstream = new FileStream(FileToDownload, FileMode.Open))
                {
                    await Fstream.CopyToAsync(memory);//await
                }
                memory.Position = 0;
                return File(memory, "image/jpg", Path.GetFileName(FileToDownload));

            }

            using (SqlCommand command = new SqlCommand())
            {
                command.Connection = connect;
                command.CommandText = "SELECT FirstName, LastName FROM Users WHERE UserID = @ID";
                command.Parameters.AddWithValue("@ID", ImgRecord.UserID);

                SqlDataReader name_reader = command.ExecuteReader();

                while (name_reader.Read())
                {
                    UploadUser = name_reader.GetString(0) + " " + name_reader.GetString(1);
                }

                connect.Close();


                return Page();
            }
        }
    }
}
