using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Data;
using System.ComponentModel;


namespace League_Game_Data
{

    class LeagueDB
    {
        private SqlConnection _sqlConn;
        private SQLiteConnection _sqliteconn;
        private bool _sqlite;
        //keep mapping of db types used in the application
        private Dictionary<DbType, SqlDbType> _typeMapping = new Dictionary<DbType, SqlDbType>()
        {
            {DbType.Int32, SqlDbType.Int },
            {DbType.String, SqlDbType.NVarChar },
            {DbType.DateTime, SqlDbType.DateTime },
            {DbType.Time, SqlDbType.Time },
            {DbType.Boolean, SqlDbType.Bit }
        };
        public LeagueDB(string connString, bool sqlite)
        {
            _sqlite = sqlite;
            if(_sqlite)
            {
                _sqliteconn = new SQLiteConnection(connString);
            }
            else
            {
                _sqlConn = new SqlConnection(connString);
                var test = SqlDbType.Time;
            }
        }

        //Takes in Tuple with Param name, param type, value and type for the value.
        public void executeNonQuery(string query, List<Tuple<string, DbType, string, Type>> parameters)
        {
            if(_sqlite)
            {
                executeSqliteQuery(query, parameters, false);
            }
            else
            {
                executeSqlQuery(query, parameters, false);
            }
        }
        //Used to get gameId when inserting a new record.  This should be made generic at some point
        public int insertGame(string query, List<Tuple<string, DbType, string, Type>> parameters)
        {
            int result = 0;
            if (_sqlite)
            {
                result = (int)executeSqliteQuery(query, parameters, true);
            }
            else
            {
                result = executeSqlQuery(query, parameters, true);
            }
            return result;
        }

        private long executeSqliteQuery(string query, List<Tuple<string, DbType, string, Type>> parameters, bool getGameId)
        {
            long result = 0;
            _sqliteconn.Open();
            var command = new SQLiteCommand(query, _sqliteconn);

            foreach (var parameter in parameters)
            {
                var converter = TypeDescriptor.GetConverter(parameter.Item4);
                command.Parameters.Add(parameter.Item1, parameter.Item2).Value = converter.ConvertFrom(parameter.Item3);
            }

            command.ExecuteNonQuery();
            if(getGameId)
            {
                result = _sqliteconn.LastInsertRowId;
            }
            _sqliteconn.Close();
            return result;
        }

        private int executeSqlQuery(string query, List<Tuple<string, DbType, string, Type>> parameters, bool getGameId)
        {
            int result = 0;
            _sqlConn.Open();
            if(getGameId)
            {
                query = query.Insert(query.IndexOf("VALUES"), "OUTPUT INSERTED.id ");
            }

            var command = new SqlCommand(query, _sqlConn);
            foreach(var parameter in parameters)
            {
               //get a converter to convert from string to applicable datatype
                var converter = TypeDescriptor.GetConverter(parameter.Item4);
                
                //Values passed in as DbType, convert to sqlDbType
                command.Parameters.Add(parameter.Item1, _typeMapping[parameter.Item2]).Value = converter.ConvertFrom(parameter.Item3);
            }
            if(getGameId)
            {
                result = (int)command.ExecuteScalar();
            }
            else
            {
                command.ExecuteNonQuery();
            }
            _sqlConn.Close();
            return result;
        }

        public void dbDispose()
        {
            if(_sqlite)
            {
                _sqliteconn.Dispose();
            }
            else
            {
                _sqlConn.Dispose();
            }
        }
    }
}
