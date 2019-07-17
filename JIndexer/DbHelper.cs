using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JIndexer
{
    static class DbHelper
    {
        static string dbname = "jindexer.db";

        public static void createDbIfNotExists()
        {

            if (!System.IO.File.Exists(dbname))
            {
                SQLiteConnection.CreateFile("jindexer.db");
                SQLiteConnection m_dbConnection;
                m_dbConnection = new SQLiteConnection("Data Source=jindexer.db;Version=3;");
                m_dbConnection.Open();

                string sql = "CREATE TABLE items (" +
                         " name TEXT " +
                         ", file TEXT " +
                         ", stars TINYINT " +
                         ", tags TEXT " +
                         ", loadingFails TINYINT " +
                         ", size INT )";

                SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
                command.ExecuteNonQuery();

                m_dbConnection.Close();
            }
        }


        //public static void insertRec(List<Instrument> instruments)
        public static void insertRec(Instrument i)
        {
            SQLiteConnection m_dbConnection;
            m_dbConnection = new SQLiteConnection("Data Source="+dbname+";Version=3;");
            m_dbConnection.Open();

            //foreach (Instrument i in instruments)
            if (true)
            {
                //string sql = 
                //SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);

                SQLiteCommand command = new SQLiteCommand(m_dbConnection);
                command.CommandText = "Select * From MyTable Where MyColumn = @MyValue";
                command.CommandText = "insert into items (name, file, stars, tags, loadingFails, size) values (@name, @file, @stars, @tags, @loadingFails, @size);";
                command.Parameters.AddWithValue("file", i.GetFile());
                command.Parameters.AddWithValue("name", i.GetName());
                command.Parameters.AddWithValue("stars", i.GetStars());
                command.Parameters.AddWithValue("tags", i.GetTags());
                command.Parameters.AddWithValue("loadingFails", i.GetLoadingFails());
                command.Parameters.AddWithValue("size", i.GetSize());

                command.ExecuteNonQuery();
            }

            m_dbConnection.Close();
        }

        public static void markDoesntWork(string file)
        {
            SQLiteConnection m_dbConnection;
            m_dbConnection = new SQLiteConnection("Data Source="+dbname+";Version=3;");
            m_dbConnection.Open();

            //foreach (Instrument i in instruments)
            if (true)
            {
                SQLiteCommand command = new SQLiteCommand(m_dbConnection);
                command.CommandText = "update items set loadingFails = 1 where file = '" + file + "';"; //todo parameterize
                command.ExecuteNonQuery();
            }

            m_dbConnection.Close();
        }

        public static void markWorks(string file) //todo refactor with above
        {
            SQLiteConnection m_dbConnection;
            m_dbConnection = new SQLiteConnection("Data Source="+dbname+";Version=3;");
            m_dbConnection.Open();

            //foreach (Instrument i in instruments)
            if (true)
            {
                SQLiteCommand command = new SQLiteCommand(m_dbConnection);
                command.CommandText = "update items set loadingFails = 0 where file = '" + file + "';"; //todo parameterize
                command.ExecuteNonQuery();
            }

            m_dbConnection.Close();
        }


        public static void markFavorite(string file, bool isFavorite = true) //todo refactor with above
        {
            SQLiteConnection m_dbConnection;
            m_dbConnection = new SQLiteConnection("Data Source="+dbname+";Version=3;");
            m_dbConnection.Open();

            //foreach (Instrument i in instruments)
            if (true)
            {
                SQLiteCommand command = new SQLiteCommand(m_dbConnection);
                if (isFavorite)
                    command.CommandText = @"update items set stars = 5 where file = @val;"; 
                else
                    command.CommandText = @"update items set stars = 0 where file = @val;"; 

                command.Parameters.AddWithValue("@val", file);
                command.ExecuteNonQuery();
            }

            m_dbConnection.Close();
        }

        public static void Delete(string file) //todo refactor with above
        {
            SQLiteConnection m_dbConnection;
            m_dbConnection = new SQLiteConnection("Data Source="+dbname+";Version=3;");
            m_dbConnection.Open();

            //foreach (Instrument i in instruments)
            if (true)
            {
                SQLiteCommand command = new SQLiteCommand(m_dbConnection);
                command.CommandText = "delete from items where file = @val;";

                SQLiteParameter lookupValue = new SQLiteParameter("@val");

                command.Parameters.Add(lookupValue);
                lookupValue.Value = file;

                command.ExecuteNonQuery();
            }

            m_dbConnection.Close();
        }



        public static bool IsNotInDatabase(string file)
        {
            SQLiteConnection m_dbConnection;
            m_dbConnection = new SQLiteConnection("Data Source="+dbname+";Version=3;");
            m_dbConnection.Open();

            int count = 0;

            List<Instrument> instruments = new List<Instrument>();
            using (SQLiteCommand fmd = m_dbConnection.CreateCommand())
            {
                fmd.CommandText = @"SELECT count(*) as cnt FROM items ";
                fmd.CommandText += " where file = @val ";

                SQLiteParameter lookupValue = new SQLiteParameter("@val");
//                fmd.Parameters.AddWithValue("@val", file);

                fmd.CommandText += ";";

                fmd.Parameters.Add(lookupValue);
                lookupValue.Value = file;

                /*
                string query = "";
                foreach (SQLiteParameter p in fmd.Parameters)
                {
                    query = fmd.CommandText.Replace(p.ParameterName, p.Value.ToString());
                }
*/
                //Todo find a way to do this without creating that 'query' thing
 //               fmd.CommandText = query;
                fmd.CommandType = CommandType.Text;
                SQLiteDataReader r = fmd.ExecuteReader();
                while (r.Read())
                {
                    count = Convert.ToInt32(r["cnt"]);
                }
            }

            m_dbConnection.Close();
            return count == 0;
        }

        public static List<Instrument> GetInstruments(string searchPattern = "")
        {
            SQLiteConnection m_dbConnection;
            m_dbConnection = new SQLiteConnection("Data Source="+dbname+";Version=3;");
            m_dbConnection.Open();
            
            List<Instrument> instruments = new List<Instrument>();
            using (SQLiteCommand fmd = m_dbConnection.CreateCommand())
            {
                fmd.CommandText = @"SELECT name, stars, file, tags, size, loadingFails FROM items ";
                if (searchPattern.Length > 0)
                {
                    fmd.CommandText += " where upper(name) like '%@val%' ";
                    fmd.CommandText += " or upper(tags) like '%@val%' ";
                    fmd.CommandText += " or upper(file) like '%@val%' ";
                    fmd.Parameters.AddWithValue("@val", searchPattern.ToUpper());
                }
                fmd.CommandText += ";";

                string query = "";
                foreach (SQLiteParameter p in fmd.Parameters)
                {
                    query = fmd.CommandText.Replace(p.ParameterName, p.Value.ToString());
                }

                //Todo find a way to do this without creating that 'query' thing
                if (query.Length > 0)
                {
                    fmd.CommandText = query;
                }
                fmd.CommandType = CommandType.Text;
                SQLiteDataReader r = fmd.ExecuteReader();
                while (r.Read())
                {
                    var name = Convert.ToString(r["name"]);
                    var stars = Convert.ToInt32(r["stars"]);

                    Instrument i = new Instrument(
                            name,
                            Convert.ToString(r["file"]),
                            stars,
                            Convert.ToString(r["tags"]),
                            Convert.ToInt32(r["size"]),
                            Convert.ToInt16(r["loadingFails"])
                        );
                    instruments.Add(i);
                }
            }
            return instruments;
        }

    }



}
