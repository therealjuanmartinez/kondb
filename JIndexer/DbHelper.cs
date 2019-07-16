﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JIndexer
{
    static class DbHelper
    {

        public static void createDb()
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


        //public static void insertRec(List<Instrument> instruments)
        public static void insertRec(Instrument i)
        {
            SQLiteConnection m_dbConnection;
            m_dbConnection = new SQLiteConnection("Data Source=jindexer.db;Version=3;");
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

        public static List<Instrument> GetInstruments()
        {
            SQLiteConnection m_dbConnection;
            m_dbConnection = new SQLiteConnection("Data Source=jindexer.db;Version=3;");
            m_dbConnection.Open();
            
            List<Instrument> instruments = new List<Instrument>();
            using (SQLiteCommand fmd = m_dbConnection.CreateCommand())
            {
                fmd.CommandText = @"SELECT name, stars, file, tags, size, loadingFails FROM items;";
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
