using System;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartConfig.DataStores.SQLite
{
    class Program
    {
        static void Main(string[] args)
        {
            var conn = new SQLiteConnection("Data Source=config.db;Version=3;");
            conn.Open();
            var cmd = new SQLiteCommand("select * from setting", conn);
            var rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                Console.WriteLine(rdr["Value"]);
            }
        }
    }
}
