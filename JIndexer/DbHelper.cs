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


        public static List<string> Dedupe(List<Instrument> selectedInstruments, bool favorTopAlpha)
        {
            List<string> removedFilenames = new List<string>();

            SQLiteConnection m_dbConnection;
            m_dbConnection = new SQLiteConnection("Data Source=" + dbname + ";Version=3;");

            m_dbConnection.Open();

            //for each record
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

                /*
                var count = 0;
                foreach (Instrument ii in instruments)
                {
                    bool alreadyDeleted = removedFilenames.Any(file => file == ii.GetFile());
                    if (!alreadyDeleted) //if we've already deleted the file, no reason to include it in list to filter by 
                    {
                        if (count > 0)
                        {
                            command.CommandText += ", ";
                        }
                        command.CommandText += "@file" + count;
                        lookupValue = new SQLiteParameter("@file" + count);
                        command.Parameters.Add(lookupValue);
                        lookupValue.Value = ii.GetFile();
                        count++;
                    }
                }
                command.CommandText += ") ";
                */
                if (favorTopAlpha)
                {
                    command.CommandText += "ORDER BY name DESC;";
                }
                else
                {
                    command.CommandText += "ORDER BY name ASC;";
                }

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


                if (deleteFileList.Count > 1)
                {

                    long size = -1;
                    foreach (var file in deleteFileList) //ensure sizes are equal for all before doing anything
                    {
                        FileInfo fi = new FileInfo(file);
                        if (size > 0 && fi.Length != size)
                        {
                            deleteFileList.Clear(); break; //DO NOT PROCESS ANYTHING IN THIS SET UNLESS SIZES MATCH
                        }
                        size = fi.Length;
                    }

                    for (int j = 0; j < (deleteFileList.Count - 1); j++) //only process all but last
                    {
                        var fileToDelete = deleteFileList[j];
                        bool includedInListFromUI = selectedInstruments.Any(instr => instr.GetFile() == fileToDelete);
                        if (includedInListFromUI)  //Dont delete any items that wre not in the selection from UI
                        {
                            command = new SQLiteCommand("delete from items where file = @file", m_dbConnection);
                            lookupValue = new SQLiteParameter("@file");
                            command.Parameters.Add(lookupValue);
                            lookupValue.Value = deleteFileList[j];
                            command.ExecuteNonQuery();
                            removedFilenames.Add(deleteFileList[j]);
                        }
                    }
                }
            }

            m_dbConnection.Close();

            return removedFilenames;

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


        public static void setSetting(string key, string value)
        {
            SQLiteConnection m_dbConnection;
            m_dbConnection = new SQLiteConnection("Data Source=" + dbname + ";Version=3;");
            m_dbConnection.Open();

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

            m_dbConnection.Close();
        }

        public static string getSetting(string key)
        {
            SQLiteConnection m_dbConnection;
            m_dbConnection = new SQLiteConnection("Data Source=" + dbname + ";Version=3;");

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
                m_dbConnection.Open();
                try
                {
                    object res = fmd.ExecuteScalar();
                    ret = Convert.ToString(res);
                }
                catch (Exception e) { }
            }

            m_dbConnection.Close();
            return ret;
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
                command.CommandText = "update items set loadingFails = 1 where file = @val ;"; //todo parameterize

                SQLiteParameter lookupValue = new SQLiteParameter("@val");
                command.Parameters.Add(lookupValue);
                lookupValue.Value = file;

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

        public static void markStars(string file, int stars) //todo refactor with above
        {
            SQLiteConnection m_dbConnection;
            m_dbConnection = new SQLiteConnection("Data Source=" + dbname + ";Version=3;");
            m_dbConnection.Open();

            //foreach (Instrument i in instruments)
            if (true)
            {
                SQLiteCommand command = new SQLiteCommand(m_dbConnection);
                command.CommandText = @"update items set stars = " + stars + " where file = @val;";

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


            m_dbConnection.Close();
            return count == 0;
        }

        public static List<Instrument> GetInstruments(string searchPattern = "", bool favoritesOnly = false)
        {
            SQLiteConnection m_dbConnection;
            m_dbConnection = new SQLiteConnection("Data Source="+dbname+";Version=3;");
            m_dbConnection.Open();


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
