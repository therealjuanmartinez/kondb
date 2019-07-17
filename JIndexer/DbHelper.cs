using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JIndexer
{
    public sealed class ConnSingleton
    {
        private static ConnSingleton instance = null;
        private static readonly object padlock = new object();

        ConnSingleton()
        {
        }

        SQLiteConnection m_dbConnection;
        bool isOpen = false;

        public SQLiteConnection GetOpenConnection()
        {
            if (!isOpen)
            {
                m_dbConnection = new SQLiteConnection("Data Source=jindexer.db;Version=3;");
                m_dbConnection.Open();
                isOpen = true;
            }
            return m_dbConnection;
        }
        public void CloseConnection()
        {
            if (isOpen)
            {
                m_dbConnection.Close();
                isOpen = false;
            }
        }

        /// <summary>
        /// MUST run optionalEndTransactionForSpeed() if running this.  should make things fast
        /// </summary>
        public void optionalBeginTransactionForSpeed()
        {
            var conn = ConnSingleton.Instance.GetOpenConnection();
            transaction = conn.BeginTransaction();
        }
        SQLiteTransaction transaction;

        /// <summary>
        /// MUST run this if ran optionalBeginTransactionForSpeed
        /// </summary>
        public void optionalEndTransactionForSpeed()
        {
            var conn = ConnSingleton.Instance.GetOpenConnection();
            transaction.Commit();
        }



        public static ConnSingleton Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new ConnSingleton();
                    }
                    return instance;
                }
            }
        }
    }

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
                         ", file TEXT PRIMARY KEY " +
                         ", stars TINYINT " +
                         ", tags TEXT " +
                         ", loadingFails TINYINT " +
                         ", size INT );" +
                         "" +
                         "" +
                         "" +
                         "create table settings (key text PRIMARY KEY, value text);";

                SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
                command.ExecuteNonQuery();

                m_dbConnection.Close();
            }
        }


        public static void optionalBeginTransactionForSpeed()
        {
            ConnSingleton.Instance.optionalBeginTransactionForSpeed();
        }

        /// <summary>
        /// MUST run this if ran optionalBeginTransactionForSpeed
        /// </summary>
        public static void optionalEndTransactionForSpeed()
        {
            ConnSingleton.Instance.optionalEndTransactionForSpeed();
        }



        public static List<string> Dedupe(List<Instrument> selectedInstruments, bool favorTopAlpha)
        {
            List<string> removedFilenames = new List<string>();

            SQLiteConnection m_dbConnection = ConnSingleton.Instance.GetOpenConnection();
            /*
            SQLiteConnection m_dbConnection;
            m_dbConnection = new SQLiteConnection("Data Source=" + dbname + ";Version=3;");
            m_dbConnection.Open();
            */

            //m_dbConnection.Open();

            //for each record (not necessarily in this order... added a bunch of linq later)
            //   select all with matching name in set of files provided, order based on favortopalpha
            //    make sure it's >=2 that come back
            //    delete the first one
            //    add too collection
            //     move to next one

            List<string> filesSelected = selectedInstruments.Select(inst => inst.GetFile()).ToList();

            foreach (Instrument i in selectedInstruments)
            {

                SQLiteCommand command = new SQLiteCommand(m_dbConnection);

                command.CommandType = CommandType.Text;

                command.CommandText = "select file from items where name = @name ";//and file in (";
                SQLiteParameter lookupValue = new SQLiteParameter("@name");
                command.Parameters.Add(lookupValue);
                lookupValue.Value = i.GetName();

                SQLiteDataReader r = command.ExecuteReader();

                List<string> deleteFileList = new List<string>();
                List<string> deleteNameList = new List<string>();

                while (r.Read())
                {
                    var file = Convert.ToString(r["file"]);
                    //var name = Convert.ToString(r["name"]);
                    deleteFileList.Add(file);
                }

                deleteFileList.RemoveAll(file => !filesSelected.Contains(file));

                //create list of instruments that is in deletefilelist

                var deleteInstrumentList = selectedInstruments.Where(si => deleteFileList.Contains(si.GetFile()));
                var uniqueFileSizes = deleteInstrumentList.Select(inst => inst.GetSize()).ToList().Distinct();

                foreach (var filesize in uniqueFileSizes)
                {
                    List<Instrument> deleteInstrumentListUniqueBySize;
                    if (favorTopAlpha)
                        deleteInstrumentListUniqueBySize = deleteInstrumentList.Where(inst => inst.GetSize().Equals(filesize)).OrderByDescending(inst => inst.GetFile()).ToList();
                    else
                        deleteInstrumentListUniqueBySize = deleteInstrumentList.Where(inst => inst.GetSize().Equals(filesize)).OrderBy(inst => inst.GetFile()).ToList();


                    if (deleteInstrumentListUniqueBySize.Count() > 1)
                    {
                        for (int j = 0; j < (deleteInstrumentListUniqueBySize.Count - 1); j++) //only process all but last, otherwise would delete them all
                        {
                            var fileToDelete = deleteInstrumentListUniqueBySize[j].GetFile();
                            bool includedInListFromUI = selectedInstruments.Any(instr => instr.GetFile() == fileToDelete);
                            command = new SQLiteCommand("delete from items where file = @file", m_dbConnection);
                            lookupValue = new SQLiteParameter("@file");
                            command.Parameters.Add(lookupValue);
                            lookupValue.Value = fileToDelete;
                            command.ExecuteNonQuery();
                            removedFilenames.Add(fileToDelete);
                        }
                    }
                }
            }

            //m_dbConnection.Close();

            return removedFilenames;

        }


      
        //public static void insertRec(List<Instrument> instruments)
        public static void insertRec(Instrument i)
        {
            SQLiteConnection m_dbConnection = ConnSingleton.Instance.GetOpenConnection();

            //m_dbConnection = new SQLiteConnection("Data Source="+dbname+";Version=3;");
            //m_dbConnection.Open();

            //foreach (Instrument i in instruments)
            if (true)
            {
                //string sql = 
                //SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);

                SQLiteCommand command = new SQLiteCommand(m_dbConnection);
                command.CommandText = "insert OR IGNORE into items (name, file, stars, tags, loadingFails, size) values (@name, @file, @stars, @tags, @loadingFails, @size);";
                command.Parameters.AddWithValue("file", i.GetFile());
                command.Parameters.AddWithValue("name", i.GetName());
                command.Parameters.AddWithValue("stars", i.GetStars());
                command.Parameters.AddWithValue("tags", i.GetTags());
                command.Parameters.AddWithValue("loadingFails", i.GetLoadingFails());
                command.Parameters.AddWithValue("size", i.GetSize());

                command.ExecuteNonQuery();
            }

           // m_dbConnection.Close();
        }


        public static void setSetting(string key, string value)
        {
            SQLiteConnection m_dbConnection = ConnSingleton.Instance.GetOpenConnection();
            /*
            SQLiteConnection m_dbConnection;
            m_dbConnection = new SQLiteConnection("Data Source=" + dbname + ";Version=3;");
            m_dbConnection.Open();
            */

            //foreach (Instrument i in instruments)
            if (true)
            {
                //string sql = 
                //SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);

                SQLiteCommand command = new SQLiteCommand(m_dbConnection);
                command.CommandText = "insert into settings (key, value) values (@key, @value) on conflict (key) DO UPDATE set value=@value;";
                command.Parameters.AddWithValue("key", key);
                command.Parameters.AddWithValue("value", value);

                command.ExecuteNonQuery();
            }

            //m_dbConnection.Close();
        }

        public static string getSetting(string key)
        {
            SQLiteConnection m_dbConnection = ConnSingleton.Instance.GetOpenConnection();
            /*
            SQLiteConnection m_dbConnection;
            m_dbConnection = new SQLiteConnection("Data Source=" + dbname + ";Version=3;");
            m_dbConnection.Open();
            */

            string ret = null;

            List<Instrument> instruments = new List<Instrument>();
            using (SQLiteCommand fmd = m_dbConnection.CreateCommand())
            {
                fmd.CommandText = @"SELECT value from settings ";
                fmd.CommandText += " where key = @val ";

                SQLiteParameter lookupValue = new SQLiteParameter("@val");

                fmd.CommandText += ";";

                fmd.Parameters.Add(lookupValue);
                lookupValue.Value = key;

                fmd.CommandType = CommandType.Text;
                //m_dbConnection.Open();
                try
                {
                    object res = fmd.ExecuteScalar();
                    ret = Convert.ToString(res);
                }
                catch (Exception e) { }
            }

            //m_dbConnection.Close();
            return ret;
        }




        public static void markDoesntWork(string file)
        {
            SQLiteConnection m_dbConnection = ConnSingleton.Instance.GetOpenConnection();
            /*
            SQLiteConnection m_dbConnection;
            m_dbConnection = new SQLiteConnection("Data Source=" + dbname + ";Version=3;");
            m_dbConnection.Open();
            */

            //foreach (Instrument i in instruments)
            if (true)
            {
                SQLiteCommand command = new SQLiteCommand(m_dbConnection);
                command.CommandText = "update items set loadingFails = 1 where file = @val ;"; //todo parameterize

                SQLiteParameter lookupValue = new SQLiteParameter("@val");
                command.Parameters.Add(lookupValue);
                lookupValue.Value = file;

                command.ExecuteNonQuery();
            }

            //m_dbConnection.Close();
        }

        public static void markWorks(string file) //todo refactor with above
        {
            SQLiteConnection m_dbConnection = ConnSingleton.Instance.GetOpenConnection();
            /*
            SQLiteConnection m_dbConnection;
            m_dbConnection = new SQLiteConnection("Data Source=" + dbname + ";Version=3;");
            m_dbConnection.Open();
            */

            //foreach (Instrument i in instruments)
            if (true)
            {
                SQLiteCommand command = new SQLiteCommand(m_dbConnection);
                command.CommandText = "update items set loadingFails = 0 where file = '" + file + "';"; //todo parameterize
                command.ExecuteNonQuery();
            }

            //m_dbConnection.Close();
        }


        public static void markFavorite(string file, bool isFavorite = true) //todo refactor with above
        {
            SQLiteConnection m_dbConnection = ConnSingleton.Instance.GetOpenConnection();
            /*
            SQLiteConnection m_dbConnection;
            m_dbConnection = new SQLiteConnection("Data Source=" + dbname + ";Version=3;");
            m_dbConnection.Open();
            */

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

            //m_dbConnection.Close();
        }

        public static void markStars(string file, int stars) //todo refactor with above
        {
            SQLiteConnection m_dbConnection = ConnSingleton.Instance.GetOpenConnection();
            /*
            SQLiteConnection m_dbConnection;
            m_dbConnection = new SQLiteConnection("Data Source=" + dbname + ";Version=3;");
            m_dbConnection.Open();
            */

            //foreach (Instrument i in instruments)
            if (true)
            {
                SQLiteCommand command = new SQLiteCommand(m_dbConnection);
                command.CommandText = @"update items set stars = " + stars + " where file = @val;";

                command.Parameters.AddWithValue("@val", file);
                command.ExecuteNonQuery();
            }

            //m_dbConnection.Close();
        }


        public static void Delete(string file) //todo refactor with above
        {
            SQLiteConnection m_dbConnection = ConnSingleton.Instance.GetOpenConnection();
            /*
            SQLiteConnection m_dbConnection;
            m_dbConnection = new SQLiteConnection("Data Source=" + dbname + ";Version=3;");
            m_dbConnection.Open();
            */

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

            //m_dbConnection.Close();
        }



        public static bool IsNotInDatabase(string file)
        {
            SQLiteConnection m_dbConnection = ConnSingleton.Instance.GetOpenConnection();
            /*
            SQLiteConnection m_dbConnection;
            m_dbConnection = new SQLiteConnection("Data Source=" + dbname + ";Version=3;");
            m_dbConnection.Open();
            */

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
                m_dbConnection.Open();
                //SQLiteDataReader r = fmd.ExecuteReader();
                object res = fmd.ExecuteScalar();
                /*while (r.Read())
                {
                    count = Convert.ToInt32(r["cnt"]);
                }*/
                count = Convert.ToInt32(res);
            }


            //m_dbConnection.Close();
            return count == 0;
        }

        public static List<Instrument> GetInstruments(string searchPattern = "", bool favoritesOnly = false, bool showWorking = true)
        {
            SQLiteConnection m_dbConnection = ConnSingleton.Instance.GetOpenConnection();
            /*
            SQLiteConnection m_dbConnection;
            m_dbConnection = new SQLiteConnection("Data Source=" + dbname + ";Version=3;");
            m_dbConnection.Open();
            */


            var patterns = searchPattern.Trim().Split('+');
            
            List<Instrument> instruments = new List<Instrument>();
            using (SQLiteCommand fmd = m_dbConnection.CreateCommand())
            {
                fmd.CommandText = @"SELECT name, stars, file, tags, size, loadingFails FROM items ";
                bool hasWhere = false;
                if (searchPattern.Trim().Length > 0)
                {
                    fmd.CommandText += " where ";
                    hasWhere = true;
                    var count = 0;
                   foreach (var pattern in patterns)
                    {
                        if (pattern.Trim().Length > 0)
                        {
                            var patternnn = pattern.Trim();
                            if (count > 0)
                            {
                                fmd.CommandText += " and ";
                            }
                            if (patternnn.ToCharArray()[0] != '-')
                            {
                                fmd.CommandText += " (upper(name) like @val" + count;
                                fmd.CommandText += " or upper(tags) like @val" + count;
                                fmd.CommandText += " or upper(file) like @val" + count;
                                fmd.CommandText += ")";
                            }
                            else
                            {
                                patternnn = patternnn.Remove(0, 1).Trim();
                                fmd.CommandText += " (upper(name) not like @val" + count;
                                fmd.CommandText += " and upper(tags) not like @val" + count;
                                fmd.CommandText += " and upper(file) not like @val" + count;
                                fmd.CommandText += ")";
                            }
                            //                        fmd.Parameters.AddWithValue("@val", searchPattern.ToUpper().Trim());
                            SQLiteParameter lookupValue = new SQLiteParameter("@val" + count);
                            fmd.Parameters.Add(lookupValue);
                            lookupValue.Value = "%" + patternnn.ToUpper().Trim() + "%";

                            count++;
                        }
                    }

                }

                if (favoritesOnly) {
                    if (hasWhere) {
                        fmd.CommandText += " and stars > 0 ";
                    }
                    else {
                        fmd.CommandText += " where stars > 0 ";
                        hasWhere = true;
                    }
                }

                if (!showWorking)
                {
                    if (hasWhere)
                    {
                        fmd.CommandText += " and loadingFails == 1 ";
                    }
                    else
                    {
                        fmd.CommandText += " where loadingFails == 1 ";
                        hasWhere = true;
                    }
                }

                fmd.CommandText += ";";

                /*
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
                */
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
