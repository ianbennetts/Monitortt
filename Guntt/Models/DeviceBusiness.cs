using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Sockets;
using System.Web;

namespace Guntt.Models
{
    public class DeviceBusiness
    {
        private string project;
        public IEnumerable<DeviceModel> SqlDevices
        {
            get
            {
                SqlConnection con = null;
                //string connectionString = @"Data Source=tcp:sic-australia.database.windows.net;Database=ScadaTT_Staging;User ID = admin-xw7@sic-australia;Password=L]Z2JMh^UTM7;Trusted_Connection=False;Encrypt=True";
                con = new SqlConnection(Startup.getConfigureString());
                con.Open();
                int i = 1;
                int noofDevices;
                List<DeviceModel> sqlDevices = new List<DeviceModel>();               
                SqlCommand command = con.CreateCommand();
                    SqlDataReader Reader;
                    command.CommandText = "Select * from devices where project=@proj";
                    command.Parameters.AddWithValue("@proj", project);
                try {
                    Reader = command.ExecuteReader();
                    while (Reader.Read())
                    {
                        DeviceModel device = new DeviceModel();
                        device.ID = i++;
                        device.SiteNo = Convert.ToInt32(Reader["DeviceNo"]);
                        device.BtDevice = Convert.ToInt16(Reader["Connected"]);
                        device.Location = Convert.ToInt16(Reader["CurrentLocation"]);
                        device.Modem = device.BtDevice;
                        device.NoOfSightings = 0;
                        //if (rdr["Day2Date"] != null) {
                        DateTime? dt = Reader["Timeof"] as DateTime?;
                        device.When = dt;
                        sqlDevices.Add(device);
                    }
                    noofDevices = sqlDevices.Count;
                    Utilities utilities = new Utilities();
                    long t = utilities.currentUnixTime();
                    t = t - 300;
                    Reader.Close();
                    i = 0;
                    foreach(var d in sqlDevices)
                    {
 
                        command = new SqlCommand("select count (*) from sighting where deviceno =@dev and sightingdepart > @t", con);
                        command.Parameters.AddWithValue("@dev", (d.SiteNo));
                        command.Parameters.AddWithValue("@t", t);
                        Reader = command.ExecuteReader();
                        Reader.Read();
                        sqlDevices.ElementAt(i++).NoOfSightings = (int)(Reader.GetValue(0)) * 12;
                        Reader.Close();
                    }
                    con.Close();
                    return sqlDevices;
                }catch(Exception ex)
                {
                    return null;
                }
            }
        }
        public IEnumerable<DeviceModel> getDevices(string p)
        {
            project = p;
            return SqlDevices;
        }
        public string testSockets()
        {

            NetworkStream n;
            TcpClient tcp;
            bool validIP = false;
            int no, i;
            string[] s;
            byte[] ip = new byte[4];
            tcp = new TcpClient();
            try
            {
                tcp.Connect("serversoftgineering.eairlink.com", 443);
                n = tcp.GetStream();
                tcp.Close();
                return "Ok";
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
    }
}