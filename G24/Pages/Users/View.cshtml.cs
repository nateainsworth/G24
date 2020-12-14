using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using G24.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MigraDocCore.DocumentObjectModel;
using MigraDocCore.DocumentObjectModel.MigraDoc.DocumentObjectModel.Shapes;
using MigraDocCore.DocumentObjectModel.Tables;
using MigraDocCore.Rendering;
using PdfSharpCore.Utils;
using SixLabors.ImageSharp.PixelFormats;
using Document = MigraDocCore.DocumentObjectModel.Document;

namespace G24.Pages.Users
{
    public class ViewModel : PageModel
    {

        public List<User> UserRecords { get; set; }

        [BindProperty(SupportsGet =true)]
        public string Type { get; set; }

        IWebHostEnvironment _env;

        public ViewModel(IWebHostEnvironment env)
        {
            _env = env;
        }


        public List<int> AccountType { get; set; } = new List<int> { 0, 1, 2 };

        [BindProperty]
        public SessionActive ActiveRecord { get; set; }

        public const string Session_SessionID = "sessionID";
        public const string Session_EmailAddress = "emailAddress";
        public const string Session_FirstName = "firstName";
        public const string Session_ModLevel = "modLevel";


        public IActionResult OnGet(string PDF)
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


                //PDF code here!
                if (PDF == "1")
            {
                //Create an object for pdf document
                Document doc = new Document();
                Section sec = doc.AddSection();
                Paragraph para = sec.AddParagraph();

                //Adding picture
                ImageSource.ImageSourceImpl = new ImageSharpImageSource<Rgba32>();
                Paragraph para2 = sec.AddParagraph();
                var picpath = Path.Combine(_env.WebRootPath, "Files", "UserPhoto.png");
                var image = para2.AddImage(ImageSource.FromFile(picpath));
                image.Width = Unit.FromCentimeter(17);
                para2.Format.SpaceAfter = Unit.FromCentimeter(2);

                para.Format.Font.Name = "Arial";
                para.Format.Font.Size = 14;
                para.Format.Font.Color = Color.FromCmyk(0, 0, 0, 100); //black colour
                para.AddFormattedText("User Report : ", TextFormat.Bold);
                para.Format.SpaceAfter = "1.0cm";

                //Table
                Table tab = new Table();
                tab.Borders.Width = 0.75;
                tab.TopPadding = 5;
                tab.BottomPadding = 5;

                //Column
                Column col = tab.AddColumn(Unit.FromCentimeter(1.5));
                col.Format.Alignment = ParagraphAlignment.Justify;
                tab.AddColumn(Unit.FromCentimeter(4));
                tab.AddColumn(Unit.FromCentimeter(4));
                tab.AddColumn(Unit.FromCentimeter(6));
                tab.AddColumn(Unit.FromCentimeter(1.5));

                //Row
                Row row = tab.AddRow();
                row.Shading.Color = Colors.Green;

                //Cell for header
                Cell cell = new Cell();
                cell = row.Cells[0];
                cell.AddParagraph("User ID");
                cell = row.Cells[1];
                cell.AddParagraph("First Name");
                cell = row.Cells[2];
                cell.AddParagraph("Last Name");
                cell = row.Cells[3];
                cell.AddParagraph("Email");
                cell = row.Cells[4];
                cell.AddParagraph("Mod Level");

                    

                //Add data to table 
                for (int i = 0; i < UserRecords.Count; i++)
                {
                    row = tab.AddRow();
                    cell = row.Cells[0];
                    cell.AddParagraph(Convert.ToString(UserRecords[i].UserID));
                    cell = row.Cells[1];
                    cell.AddParagraph(UserRecords[i].FirstName);
                    cell = row.Cells[2];
                    cell.AddParagraph(UserRecords[i].LastName);
                    cell = row.Cells[3];
                    cell.AddParagraph(UserRecords[i].EmailAddress);
                    cell = row.Cells[4];
                    cell.AddParagraph(Convert.ToString(UserRecords[i].ModLevel));
                    }

                tab.SetEdge(0, 0, 4, (UserRecords.Count + 1), Edge.Box, BorderStyle.Single, 1, Colors.Gray);
                sec.Add(tab);
                    

                //Rendering
                PdfDocumentRenderer pdfRen = new PdfDocumentRenderer();
                pdfRen.Document = doc;
                pdfRen.RenderDocument();

                //Create a memory stream
                MemoryStream stream = new MemoryStream();
                pdfRen.PdfDocument.Save(stream); //saving the file into the stream

                Response.Headers.Add("content-disposition", new[] { "inline; filename = UserRecord.pdf" });
                return File(stream, "application/pdf");

            }



            }

            return Page();
        }

         
    }
}
