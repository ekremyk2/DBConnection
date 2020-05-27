using System;
using System.Data.SqlClient;
using System.Data;
using System.Diagnostics;

namespace DBConnection
{
    public class DBConnection
    {
        private DBConnection dbconnection = null;

        private SqlConnection Con;
        private SqlCommand Cmd;
        private PerformanceCounter Count;

        public SqlConnection Connection
        {
            get { return Con; }
        }

        private DBConnection(string ConString)
        {
            Con = new SqlConnection(ConString);
            Count = new PerformanceCounter();
            Count.CategoryName = "CSD";
            Count.CounterName = "DBConnection";
            Count.ReadOnly = false;
            Count.MachineName = Environment.MachineName;
        }

        public DBConnection CreateConnection(string ConString)
        {
            if (dbconnection == null)
            {
                dbconnection = new DBConnection(ConString);
            }
            else
            {
                Con = new SqlConnection(ConString);
            }

            return dbconnection;
        }

        private void Connect()
        {
            if (Con.State == ConnectionState.Closed)
            {
                try
                {
                    Con.Open();
                    Count.Increment();
                }
                catch (SqlException e)
                {
                    throw e;
                }
            }
        }

        private void Disconnect()
        {
            if (Con.State == ConnectionState.Open)
            {
                Con.Close();
                Count.Decrement();
            }
        }

        public void Execute(string Query)
        {
            Connect();

            Cmd = Con.CreateCommand();
            Cmd.CommandText = Query;


            Cmd.ExecuteNonQuery();

            Disconnect();
        }

        public void Exec(string Query, params SqlParameter[] parameters)
        {
            Connect();

            Cmd = Con.CreateCommand();
            Cmd.CommandText = Query;

            for (int i = 0; i <= parameters.Length - 1; i++)
            {
                Cmd.Parameters.Add(parameters[i]);
            }

            Cmd.ExecuteNonQuery();

            Disconnect();
        }

        public DataTable GetTable(string Query)
        {
            Connect();

            Cmd = Con.CreateCommand();
            Cmd.CommandText = Query;

            SqlDataReader dataReader = Cmd.ExecuteReader();
            DataTable dataTable = new DataTable();

            dataTable.Load(dataReader);

            Disconnect();

            return dataTable;
        }

        public DataTable GetTable(string Query, params SqlParameter[] parameters)
        {

            Connect();

            Cmd = Con.CreateCommand();
            Cmd.CommandText = Query;

            for (int i = 0; i < parameters.Length - 1; i++)
            {
                Cmd.Parameters.Add(parameters[i]);
            }

            SqlDataReader dataReader = Cmd.ExecuteReader();
            DataTable dataTable = new DataTable();

            dataTable.Load(dataReader);

            Disconnect();

            return dataTable;
        }
    }
}
