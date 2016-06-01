using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace SURFFactureDetector
{
    public static class DataManaager
    {
        private static SQLiteConnection conn;

        public static SQLiteConnection GetTheConn()
        {
            if (null == conn)
            {
                string dbPath = ConfigurationManager.AppSettings["dbPath"];
                Console.WriteLine("数据库路径为:"+dbPath);
                conn = new SQLiteConnection(""+dbPath);
            }
            return conn;

        }

        public static void Release() {
            conn.Close();
        }
    }
}
