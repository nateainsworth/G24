using G24.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace G24.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public List<Images> ImgRecords { get; set; }
        public List<Images> col1_ImgRecords { get; set; }
        public List<Images> col2_ImgRecords { get; set; }
        public List<Images> col3_ImgRecords { get; set; }

        [BindProperty(SupportsGet = true)]
        public string Type { get; set; }

        public List<int> ImageType { get; set; } = new List<int> { 0, 1, 2 };


        [BindProperty]
        public SessionActive ActiveRecord { get; set; }

        public const string Session_SessionID = "sessionID";
        public const string Session_EmailAddress = "emailAddress";
        public const string Session_FirstName = "firstName";
        public const string Session_ModLevel = "modLevel";

        public List<String> ImageTypeFullSet { get; set; }
        public List<String> ImageTypeSingleSet { get; set; }

        public string ActiveType;

        public IActionResult OnGet(string? ActiveType )
        {
            // checks if there is an active session
            ActiveRecord = new SessionActive();

            ActiveRecord.Active_SessionID = HttpContext.Session.GetString(Session_SessionID);
            ActiveRecord.Active_EmailAddress = HttpContext.Session.GetString(Session_EmailAddress);
            ActiveRecord.Active_FirstName = HttpContext.Session.GetString(Session_FirstName);
            ActiveRecord.Active_ModLevel = HttpContext.Session.GetInt32(Session_ModLevel);
            
            // acts on if there is an active session or not
            if (string.IsNullOrEmpty(ActiveRecord.Active_EmailAddress) && string.IsNullOrEmpty(ActiveRecord.Active_FirstName) && string.IsNullOrEmpty(ActiveRecord.Active_SessionID))
            {
                ActiveRecord.Active_Sesson = false;
            }
            else
            {
                ActiveRecord.Active_Sesson = true;
            }


            //connects to database
            DBConnect G24database_connection = new DBConnect();
            string DBconnection = G24database_connection.DatabaseString();
            Console.WriteLine(DBconnection);

            SqlConnection connect = new SqlConnection(DBconnection);
            connect.Open();

            using (SqlCommand command = new SqlCommand())
            {

                command.Connection = connect;

                //starts building the command to Select all images from the database
                command.CommandText = @"SELECT * FROM Images";

                SqlDataReader type_reader = command.ExecuteReader();

                ImageTypeFullSet = new List<string>();

                while (type_reader.Read())
                {
                    // loops through the types of images available and adds the type into a variable e.g. forest, sea, desert etc.
                    ImageTypeFullSet.Add(type_reader.GetString(2));

                }

                type_reader.Close();
                
                // checks if a filter has been set for displaying a specific image type
                if (!(string.IsNullOrEmpty(ActiveType) || Type == "ALL"))
                {
                    // adds to the database command to select all from images where type = the selected filter
                    command.CommandText += " WHERE Type = @ImgType";
                    command.Parameters.AddWithValue("@ImgType", ActiveType);
                }
                // executes the database command
                SqlDataReader reader = command.ExecuteReader();

                ImgRecords = new List<Images>();
                col1_ImgRecords = new List<Images>();
                col2_ImgRecords = new List<Images>();
                col3_ImgRecords = new List<Images>();

                // distributer is set for looping through the images and distributing them across the 3 collumns
                int distributor = 1;

                //loops through the data returned from the database
                while (reader.Read())
                {
                    Images record = new Images();
                    record.ImgID = reader.GetInt32(0);
                    record.ImgURL = reader.GetString(1);
                    record.Type = reader.GetString(2);
                    record.ImgName = reader.GetString(3);
                    record.UserID = reader.GetInt32(4);
                    ImgRecords.Add(record);

                    if (distributor == 1)
                    {
                        // distributes the image into column 1 and changes the distribution to 2 so the next image goes into the next column, 
                        // it then jumps back to the start of the loop or it would carry on and be placd in column 2 qnd column 3
                        col1_ImgRecords.Add(record);
                        distributor = 2;
                        continue;
                    }
                    if (distributor == 2)
                    {
                        // simliar to the above i statement if gets images and seves them into column 2 but tells the file to save into column 3 next
                        col2_ImgRecords.Add(record);
                        distributor = 3;
                        continue;
                    }
                    if (distributor == 3)
                    {
                        //after column 3 has saved an image it has to tell the functon to save to column one next and it repeats the porcess through each column untill all images are loaded.
                        col3_ImgRecords.Add(record);
                        distributor = 1;
                        continue;
                    }

                }

               

                // this gets the list of types available within the database and deletes duplicates.
                ImageTypeSingleSet = ImageTypeFullSet.Distinct().ToList();

                reader.Close();


            }



            return Page();
        }
    }
}





