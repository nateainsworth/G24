using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace G24.Pages
{
    public class DBConnect
    {
            public string DatabaseString()
            {
            string G24database_connection = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Nate\source\repos\G24\G24\Data\G24Database.mdf;Integrated Security=True;Connect Timeout=30";
            return G24database_connection;
            }
        
    }
}
