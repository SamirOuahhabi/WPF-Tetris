using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Finisar.SQLite;
using System.Diagnostics;

namespace Tetris
{
    public class SQLiteDatabase
    {
        protected SQLiteConnection _conn;
        protected string _filename;
        protected bool _connected;
        protected bool _tableReady;

        public SQLiteDatabase(string filename)
        {
            _filename = filename;
            initDb();
            initTable();
        }

        public bool execNonQuery(string query)
        {
            if (Ready)
            {
                try
                {
                    SQLiteCommand cmd = _conn.CreateCommand();
                    cmd.CommandText = query;
                    cmd.ExecuteNonQuery();
                    return true;
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.ToString());
                    return false;
                }
            }
            else
                return false;
        }

        public SQLiteDataReader execReader(string query)
        {
            if (Ready)
            {
                try
                {
                    SQLiteCommand cmd = _conn.CreateCommand();
                    cmd.CommandText = query;
                    SQLiteDataReader datareader = cmd.ExecuteReader();
                    return datareader;
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.ToString());
                    return null;
                }
            }
            return null;
        }

        public DataTable getDataTable(string query)
        {
            if(Ready)
            {
                DataTable dt = new DataTable();
                DataColumn c1, c2, c3, c4, c5, c6;
                DataRow row;

                c1 = new DataColumn();
                c1.DataType = Type.GetType("System.Int32");
                c1.ColumnName = "ID";
                dt.Columns.Add(c1);

                c2 = new DataColumn();
                c2.DataType = Type.GetType("System.String");
                c2.ColumnName = "Filename";
                dt.Columns.Add(c2);

                c3 = new DataColumn();
                c3.DataType = Type.GetType("System.String");
                c3.ColumnName = "Path";
                dt.Columns.Add(c3);

                c4 = new DataColumn();
                c4.DataType = Type.GetType("System.String");
                c4.ColumnName = "Event";
                dt.Columns.Add(c4);

                c5 = new DataColumn();
                c5.DataType = Type.GetType("System.String");
                c5.ColumnName = "Time";
                dt.Columns.Add(c5);

                c6 = new DataColumn();
                c6.DataType = Type.GetType("System.String");
                c6.ColumnName = "Date";
                dt.Columns.Add(c6);


                try
                {
                    SQLiteDataReader reader = execReader(query);
                    while (reader.Read())
                    {
                        row = dt.NewRow();
                        row["ID"] = reader["id"];
                        row["Filename"] = reader["filename"];
                        row["Path"] = reader["path"];
                        row["Event"] = reader["event"];
                        string time = new DateTime(Int64.Parse((string)reader["eventTime"])).ToString("T");
                        string date = new DateTime(Int64.Parse((string)reader["eventTime"])).ToString("d");
                        row["Time"] = time;
                        row["Date"] = date;
                        dt.Rows.Add(row);
                    }

                    return dt;
                }
                catch(Exception e)
                {
                    Debug.WriteLine(e.ToString());
                    Debug.WriteLine(dt.GetErrors());
                    return null;
                }
            }
            return null;
        }

        public void closeDb()
        {
            _conn.Close();
        }

        private void initDb()
        {
            _conn = new SQLiteConnection("Data Source=" + _filename + ";Version=3;Compress=True;");

            try
            {
                _conn.Open();
                _connected = true;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
                _connected = false;
            }

            if (!_connected)
            {
                _conn = new SQLiteConnection("Data Source=" + _filename + ";New=True;Version=3;Compress=True;");

                try
                {
                    _conn.Open();
                    _connected = true;
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.ToString());
                    _connected = false;
                }
            }

        }

        private void initTable()
        {
            if (_connected)
            {
                bool tableExists = false;
                SQLiteCommand cmd = _conn.CreateCommand();
                cmd.CommandText = "SELECT * FROM events";
                try
                {
                    cmd.ExecuteNonQuery();
                    tableExists = true;
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.ToString());
                    tableExists = false;
                }

                if (!tableExists)
                {
                    cmd.CommandText = "CREATE TABLE events (id integer primary key, filename varchar(100),"
                        + "path varchar(255), event varchar(100), eventTime varchar(100))";
                    try
                    {
                        cmd.ExecuteNonQuery();
                        _tableReady = true;
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e.ToString());
                        _tableReady = false;
                    }
                }
            }
            else
                _tableReady = false;
        }

        public bool Ready
        {
            get
            {
                return _connected;
            }
        }

        public override string ToString()
        {
            return _conn.ToString();
        }
    }
}
