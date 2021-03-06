﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;

using MeterDataRequestHandlerWebSite.Codes;
namespace MeterDataRequestHandlerWebSite.Services
{
    public class PublicSQL
    {
        public static string MODE_INSERT = "insert";
        public static string MODE_UPDATE = "update";
        public static string TYPE_DAYSPERMONTH = "DaysPerMonth";
        public static string TYPE_HOURSPERDAY = "HoursPerDay";
        public static string TYPE_HOURSPERMONTH = "HoursPerMonth";
        
        protected SqlConnection cnn = new SqlConnection();
        protected SqlCommand cmd = new SqlCommand();
        public PublicSQL(string uid, string passwd, string database)
        {
            cnn.ConnectionString = "server="+Constants.DB_SERVER_IP+";uid=" + uid + ";pwd=" + passwd + ";database=" + database;
            cmd.Connection = cnn;
        }
        public string getSensorData()
        {
            if (cnn.State == ConnectionState.Closed)
                cnn.Open();
            cmd.Parameters.Clear();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText =
            " select top 1 dateTime as DateTime,temperatureData as TemperatureData," +
            " humidityData as HumidityData,gasData as GasData,lightData as LightData," +
            " humanStatus as HumanStatus,fireData as FireData"+
            " from SensorData"+
            " order by dateTime desc";
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataSet dataset = new DataSet();
            adapter.Fill(dataset);
            DataTable dt = dataset.Tables[0];
            cnn.Close();
            return Utilities.ToJson(dt);
        }

        public string getDaysPerMonth(string year,string month)
        {
            if (cnn.State == ConnectionState.Closed)
                cnn.Open();
            cmd.Parameters.Clear();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = 
            " select sum(Consumption) as Consumption,D" +
            " from ConsumptionPerHour" +
            " where y=@y and m=@m" +
            " group by D" +
            " order by D asc";
            cmd.Parameters.AddWithValue("@y",year);
            cmd.Parameters.AddWithValue("@m",month);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataSet dataset = new DataSet();
            adapter.Fill(dataset);
            DataTable dt = dataset.Tables[0];

            cnn.Close();
            return Utilities.ToJson(dt);
        }

        public string getHoursPerMonth(string year, string month)
        {
            if (cnn.State == ConnectionState.Closed)
                cnn.Open();
            cmd.Parameters.Clear();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText =
            " select sum(Consumption) as Consumption,H"+
            " from ConsumptionPerHour" +
            " where y=@y and m=@m" +
            " group by H" +
            " order by H asc";
            cmd.Parameters.AddWithValue("@y", year);
            cmd.Parameters.AddWithValue("@m", month);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataSet dataset = new DataSet();
            adapter.Fill(dataset);
            DataTable dt = dataset.Tables[0];

            cnn.Close();
            return Utilities.ToJson(dt);
        }

        public string getHoursPerDay(string year, string month,string day)
        {
            if (cnn.State == ConnectionState.Closed)
                cnn.Open();
            cmd.Parameters.Clear();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText =
            " select sum(Consumption) as Consumption,H" +
            " from ConsumptionPerHour" +
            " where y=@y and m=@m and d=@d" +
            " group by H" +
            " order by H asc";
            cmd.Parameters.AddWithValue("@y", year);
            cmd.Parameters.AddWithValue("@m", month);
            cmd.Parameters.AddWithValue("@d", day);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataSet dataset = new DataSet();
            adapter.Fill(dataset);
            DataTable dt = dataset.Tables[0];

            cnn.Close();
            return Utilities.ToJson(dt);
        }
        public void DataOperate(object instance, string mode)
        {
            if (cnn.State == ConnectionState.Closed)
                cnn.Open();

            Type type = instance.GetType();
            PropertyInfo[] properties = type.GetProperties();
            string identity = GetIdentity(type.Name);
            //Console.WriteLine("pKey=" + pKey);

            cmd.Parameters.Clear();
            foreach (PropertyInfo p in properties)
            {
                //Console.WriteLine(p.GetValue(product, null));
                if (p.Name.ToLower() == identity.ToLower() && identity != "No identity column defined.")
                    continue;

                cmd.Parameters.AddWithValue("@" + p.Name, p.GetValue(instance, null));
            }
            string cmdString = "insert into " + type.Name + "(";

            if (mode == "update")
                cmdString = "update " + type.Name + " set ";

            foreach (SqlParameter p in cmd.Parameters)
            {
                if (mode == "update")
                    cmdString += p.ParameterName.Substring(1) + "=" + p.ParameterName + ",";
                else
                    cmdString += p.ParameterName.Substring(1) + ",";
            }


            if (mode == "update")
            {
                cmdString = cmdString.Substring(0, cmdString.Length - 1) + " where " + GetPkey(type.Name) + "=@" + GetPkey(type.Name);

                foreach (PropertyInfo p in properties)
                {
                    if (p.Name.ToLower() == identity.ToLower())
                        cmd.Parameters.AddWithValue("@" + p.Name, Convert.ToInt16(p.GetValue(instance, null)));
                }
            }
            else
                cmdString = cmdString.Substring(0, cmdString.Length - 1) + ") values(";

            if (mode == "insert")
            {
                foreach (SqlParameter p in cmd.Parameters)
                {
                    if (p.ParameterName.ToLower() == identity.ToLower() && identity != "No identity column defined.")
                        continue;

                    cmdString += p.ParameterName + ",";
                }

                cmdString = cmdString.Substring(0, cmdString.Length - 1) + ")";
            }

            cmd.CommandText = cmdString;
            cmd.CommandType = CommandType.Text;
            Console.WriteLine(cmdString);
            cmd.ExecuteNonQuery();

            cnn.Close();

            //System.Web.HttpContext.Current.Response.Write(cmdString);
        }
        /// <summary>
        /// 根据指定的表查找主键列
        /// </summary>
        /// <param name="table">表名</param>
        /// <returns>主键列名称</returns>
        private string GetPkey(string table)
        {
            //cmd.Parameters.Clear();
            SqlCommand command = new SqlCommand("select index_col(@table ,1,1)", cnn);

            command.CommandType = CommandType.Text;

            command.Parameters.AddWithValue("@table", table);

            object obj = command.ExecuteScalar();


            if (obj != null)
                return obj.ToString();

            return "";
        }
        private string GetIdentity(string table)
        {
            cmd.Parameters.Clear();

            cmd.CommandText = "sp_help";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@objname", table);

            SqlDataReader reader = cmd.ExecuteReader();
            string identity = "";
            if (reader.NextResult())
            {
                if (reader.NextResult())
                {
                    if (reader.Read())
                    {
                        identity = reader["identity"].ToString();
                    }
                }
            }
            reader.Close();
            //No identity column defined.
            return identity;
        }
    }
}
