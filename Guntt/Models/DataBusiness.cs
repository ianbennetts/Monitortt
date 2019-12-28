using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace Guntt.Models
{
    public class DataBusiness
    {
        private string  project;
        private bool hasBase;
        private int projectInt;

        public void updateLineBaseTT(int linkId,string data)
        {
            SqlConnection con = null;
            // string connectionString = @"Data Source=tcp:sic-australia.database.windows.net;Database=ScadaTT_Staging;User ID = admin-xw7@sic-australia;Password=L]Z2JMh^UTM7;Trusted_Connection=False;Encrypt=True";
            con = new SqlConnection(Startup.getConfigureString());
            SqlCommand command = new SqlCommand("Update BaseTT set BaseTT = @d where LinkId = @i", con);

            command.Parameters.AddWithValue("d", data);
            command.Parameters.AddWithValue("i", linkId);
            con.Open();
            command.ExecuteNonQuery();
            con.Close();
        }
        public int[] getBaseTT(int linkId)
        {
            SqlConnection con = null;
            // string connectionString = @"Data Source=tcp:sic-australia.database.windows.net;Database=ScadaTT_Staging;User ID = admin-xw7@sic-australia;Password=L]Z2JMh^UTM7;Trusted_Connection=False;Encrypt=True";
            con = new SqlConnection(Startup.getConfigureString());
            List<DataModel> sqlData = new List<DataModel>();
            SqlCommand command = con.CreateCommand();
            SqlDataReader Reader;
            command.CommandText = String.Format("Select * from BaseTT where linkId='{0}'", linkId);
            con.Open();
            Reader = command.ExecuteReader();
            JavaScriptSerializer js = new JavaScriptSerializer();
            int[] baseTT = null; 
            while (Reader.Read())
            {
               baseTT= js.Deserialize<int[]>(Convert.ToString(Reader["BaseTT"]));
            }
            con.Close();
            return baseTT;
         }

        public IEnumerable<DataModel> getLinks(string project)
        {
            SqlConnection con = null;
            // string connectionString = @"Data Source=tcp:sic-australia.database.windows.net;Database=ScadaTT_Staging;User ID = admin-xw7@sic-australia;Password=L]Z2JMh^UTM7;Trusted_Connection=False;Encrypt=True";
            con = new SqlConnection(Startup.getConfigureString());
            DataModel data;
            List<DataModel> sqlData = new List<DataModel>();
            SqlCommand command = con.CreateCommand();
            SqlDataReader Reader;
            command.CommandText = String.Format("Select * from Links where project='{0}'", project);
            con.Open();
            Reader = command.ExecuteReader();
            while (Reader.Read())
            {
                data = new DataModel();
                data.SiteNo = Convert.ToInt32(Reader["LinkID"]);
                data.LinkName = Convert.ToString(Reader["LinkText"]);
                data.CurrentTT = Convert.ToInt32(Reader["CurrentAverageTT"]);
                data.LinkLength = Convert.ToInt32(Reader["LinkLength"]);
                data.ID= Convert.ToInt32(Reader["LinkId"]);
                sqlData.Add(data);
            }
            Reader.Close();
            con.Close();
            return sqlData;
        }
        public int[] getTT(int project,int ctt,long start,long fin)
        {
            SqlConnection con = null;
            // string connectionString = @"Data Source=tcp:sic-australia.database.windows.net;Database=ScadaTT_Staging;User ID = admin-xw7@sic-australia;Password=L]Z2JMh^UTM7;Trusted_Connection=False;Encrypt=True";
            con = new SqlConnection(Startup.getConfigureString());
            DataModel data;
            SqlCommand command = con.CreateCommand();
            SqlDataReader Reader;
            long finishtime;
            int ttValue;
            int index;
            int[] TT = new int[2016];
            int[] TTCount = new int[2016];
            command = new SqlCommand("select * from TravelTimes where Linkno =@dev and finishTime > @t and finishtime<@f", con);
            command.Parameters.AddWithValue("@dev", (project));
            command.Parameters.AddWithValue("@t", start);
            command.Parameters.AddWithValue("@f", fin);
            con.Open();
            Reader = command.ExecuteReader();
            while (Reader.Read())
            {
                finishtime = Convert.ToInt64(Reader["finishTime"]);
                ttValue = Convert.ToInt32(Reader["baseTT"]);
                //ttValue = Convert.ToInt32(Reader["TTime"]);
                index = (int)((finishtime - start) / (60*5));
                TT[index] = (int)(TT[index] + ttValue);
                TTCount[index]++;
            }
            Reader.Close();
            if (TT[0] == 0) TT[0] = ctt; 
            for (var i = 1; i < 2016; i++)
            {
                if (TTCount[i] != 0)
                {
                    TT[i] = TT[i] / TTCount[i];
                }else
                {
                    TT[i] = TT[i - 1];
                }
            }
            con.Close();
            return TT;

        }
        public IEnumerable<DataModel> SqlData
        {
            get
            {
                SqlConnection con = null;
                //               string connectionString = @"Data Source=tcp:sic-australia.database.windows.net;Database=ScadaTT_Staging;User ID = admin-xw7@sic-australia;Password=L]Z2JMh^UTM7;Trusted_Connection=False;Encrypt=True";
                con = new SqlConnection(Startup.getConfigureString());

                DateTime timeNow = DateTime.Now.AddHours(10);
                if (project == "GUN") projectInt = 1;
                if (project == "BH") projectInt = 2;
                if (project == "DAAJ1Qw") projectInt = 4;
                if (project == "SW") projectInt = 8;
                if (project == "sw") projectInt = 8;

                //                int day = ((int)timeNow.DayOfWeek == 0) ? 7 : (int)timeNow.DayOfWeek;
                //                day = day - 1;
                hasBase = false;
                int day = (int)timeNow.DayOfWeek;
                int day1 = day + 1;
                if (day1 == 8) { day1 = 1; }
                int baseLineNow = ((day * 24) + timeNow.Hour) * 60 / 5;
                int test = timeNow.Hour;
                baseLineNow = baseLineNow + (int)(timeNow.Minute / 5);// baseLineNow+(int)(timeNow.Minute/5);
                int[,] baseLine = Startup.BaseLine(projectInt);
                int baseLineStart = baseLineNow - 18;
                con.Open();
                int i = 1;
                int noofLinks;
                DataModel data;
                List<DataModel> sqlData = new List<DataModel>();
                SqlCommand command = con.CreateCommand();
                SqlDataReader Reader;
                command.CommandText = String.Format("Select * from Links where project='{0}'",project);
                int linkLength;
                float fspeed;
                short[] shortBuffer = { 1234, 32678, 9012, 9876, 5432, 23 };
                char[] charBuffer = Array.ConvertAll(shortBuffer, Convert.ToChar);
                string buffer = new string(charBuffer);
                // try
                // {
                Reader = command.ExecuteReader() ;
                while (Reader.Read())
                {
                     data = new DataModel();
                    data.ID = i;
                    data.SiteNo = Convert.ToInt32(Reader["LinkID"]);
                    data.LinkName = Convert.ToString(Reader["LinkText"]);
                    data.baseTT = baseLine[(i - 1), baseLineNow];//Convert.ToInt32(Reader["CurrentBaseTT"]);
                    data.CurrentTT = Convert.ToInt32(Reader["CurrentAverageTT"]);
                    //data.CurrentTT = Convert.ToInt32(Reader["CurrentTT"]);
                    linkLength = Convert.ToInt32(Reader["LinkLength"]);
                     fspeed = (3600 / (float)data.baseTT) * ((float)linkLength / 1000);
                    data.baseSpeed =  (int)fspeed;
                    fspeed = ((3600 / (float)data.CurrentTT)) * (((float)linkLength / 1000));
                    data.CurrentSpeed = (int)fspeed;
                    i++;
                    sqlData.Add(data);
                }
                Reader.Close();
                noofLinks = sqlData.Count;
                Utilities utilities = new Utilities();
                long t = utilities.currentUnixTime();
                long f = t;
                t = t - (60 * 90);
                int[] TT = new int[180];
                int[] TTBase = new int[180];
                int[] TTCount = new int[180];
                short[] TTData = new short[180];
                char[] TTDataChar = new char[180];
                long finishtime;
                int ttValue;
                int index;
                string s, bs;
                i = 0;
                foreach (var lk in sqlData)
                {
                    for (int j = 0; j < 180; j++)
                    {
                        TT[j] = 0;
                        TTCount[j] = 0;
                        TTData[j] = 0;
                    }
                    command = new SqlCommand("select * from TravelTimes where Linkno =@dev and finishTime > @t and finishtime<@f", con);
                    command.Parameters.AddWithValue("@dev", (lk.SiteNo));
                    command.Parameters.AddWithValue("@t", t);
                    command.Parameters.AddWithValue("@f", f);
                    Reader = command.ExecuteReader();
                    while (Reader.Read())
                    {
                        finishtime = Convert.ToInt64(Reader["finishTime"]);
                        ttValue = Convert.ToInt32(Reader["baseTT"]);
                        //ttValue = Convert.ToInt32(Reader["TTime"]);
                        index = (int)((finishtime - t) / 30);
                        TT[index] = (int)(TT[index] + ttValue);
                        TTCount[index]++;
                    }
                    Reader.Close();
                    s = " ";
                    bs = "";
                    for (int j = 0; j < 180; j++)
                    {
                        if (TTCount[j] != 0)
                        {
                            TTData[j] = (short)(TT[j] / TTCount[j]);
                        }
                        else
                        {
                            TTData[j] = 1;
                        }
                        TTBase[j] = baseLine[i, baseLineStart + (int)(j / 10)];
                       // TTBase[j] = TTData[j];
                        s = s + TTData[j].ToString() + " ";
                         bs = bs + TTBase[j].ToString() + " ";
                       // bs = bs + TTData[j].ToString() + " ";
                    }

                    // TTDataChar=Array.ConvertAll(TTData, Convert.ToChar);
                    sqlData.ElementAt(i).Last90MinTT = s;
                    sqlData.ElementAt(i++).Last90MinBase = bs;
                    //sqlData.ElementAt(i).Last90MinBase =i.ToString()+ "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
                    //sqlData.ElementAt(i).Last90MinTT = i.ToString() + "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";

                }
                con.Close();
                return sqlData;
                // }
                // catch (Exception ex)
                // {
                //     return null;
                // }
            }
        }
        public IEnumerable<DataModel> SqlDataBase
        {
            get
            {
                SqlConnection con = null;
                con = new SqlConnection(Startup.getConfigureString());
                DateTime timeNow = DateTime.Now.AddHours(10);
                 //                int day = ((int)timeNow.DayOfWeek == 0) ? 7 : (int)timeNow.DayOfWeek;
                //                day = day - 1;
                hasBase = false;
                int day = (int)timeNow.DayOfWeek;
                int day1 = day + 1;
                if (day1 == 8) { day1 = 1; }
                int baseLineNow = ((day * 24) + timeNow.Hour) * 60 / 5;
                int test = timeNow.Hour;
                baseLineNow = baseLineNow + (int)(timeNow.Minute / 5);// baseLineNow+(int)(timeNow.Minute/5);
                int baseLineStart = baseLineNow - 18;
                con.Open();
                int i = 1;
                int noofLinks;
                List<DataModel> sqlData = new List<DataModel>();
                SqlCommand command = con.CreateCommand();
                SqlDataReader Reader;
                int linkLength;
                float fspeed;
                short[] shortBuffer = { 1234, 32678, 9012, 9876, 5432, 23 };
                char[] charBuffer = Array.ConvertAll(shortBuffer, Convert.ToChar);
                string buffer = new string(charBuffer);
              //  int[] testBase;
                // try
                // {
                var LinksList=getLinks(project);
                foreach (var link in LinksList)
                {
                   // testBase=getBaseTT(link.ID);
                    //         data.baseTT = data.baseTT;
                    //         fspeed = (3600 / (float)link.baseTT) * ((float)link.LinkLength / 1000);
                    fspeed = ((3600 / (float)link.CurrentTT)) * (((float)link.LinkLength / 1000));
                    link.baseSpeed = (int)fspeed;
                    fspeed = ((3600 / (float)link.CurrentTT)) * (((float)link.LinkLength / 1000));
                    link.CurrentSpeed = (int)fspeed;
                    sqlData.Add(link);
                }
                noofLinks = sqlData.Count;
                Utilities utilities = new Utilities();
                long t = utilities.currentUnixTime();
                long f = t;
                t = t - (60 * 90);
                int[] TT = new int[180];
                int[] TTBase = new int[180];
                int[] TTCount = new int[180];
                short[] TTData = new short[180];
                char[] TTDataChar = new char[180];
                long finishtime;
                int ttValue;
                int index;
                string s, bs;
                i = 0;
  
                foreach (var lk in sqlData)
                {
                  //  testBase = getBaseTT(lk.ID);  hhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhh
                    for (int j = 0; j < 180; j++)
                    {
                        TT[j] = 0;
                        TTCount[j] = 0;
                        TTData[j] = 0;
                    }
                    //get basedata
                    command = new SqlCommand("select * from TravelTimes where Linkno =@dev and finishTime > @t and finishtime<@f", con);
                    command.Parameters.AddWithValue("@dev", (lk.SiteNo));
                    command.Parameters.AddWithValue("@t", t);
                    command.Parameters.AddWithValue("@f", f);
                    Reader = command.ExecuteReader();
                    while (Reader.Read())
                    {
                        finishtime = Convert.ToInt64(Reader["finishTime"]);
                        ttValue = Convert.ToInt32(Reader["baseTT"]);
                        //ttValue = Convert.ToInt32(Reader["TTime"]);
                        index = (int)((finishtime - t) / 30);
                        TT[index] = (int)(TT[index] + ttValue);
                        TTCount[index]++;
                    }
                    Reader.Close();
                    s = " ";
                    bs = "";
                    for (int j = 0; j < 180; j++)
                    {
                        if (TTCount[j] != 0)
                        {
                            TTData[j] = (short)(TT[j] / TTCount[j]);
                        }
                        else
                        {
                            TTData[j] = 1;
                        }
                    //   TTBase[j] = //testBase[baseLineStart + (int)(j / 10)]; hhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhh
                       TTData[j] = TTData[j];
                        s = s + TTData[j].ToString() + " ";
                        bs =  bs + TTBase[j].ToString() + " ";
                        // bs = bs + TTData[j].ToString() + " ";
                    }

                    // TTDataChar=Array.ConvertAll(TTData, Convert.ToChar);
                    sqlData.ElementAt(i).Last90MinTT = s;
                    sqlData.ElementAt(i++).Last90MinBase = bs;
                    //sqlData.ElementAt(i).Last90MinBase =i.ToString()+ "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
                    //sqlData.ElementAt(i).Last90MinTT = i.ToString() + "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";

                }
                con.Close();
                return sqlData;
            }
        }

        public IEnumerable<DataModel> getSqlData(string s)
        {
            project = s;
            if (project == "GUN") projectInt = 1;
            if (project == "BH") projectInt = 1;
            if (project == "DAAJ1Qw") projectInt = 1;
            if (projectInt==1) return SqlData;
            return SqlDataBase;
          //  return t;
        }
        public IEnumerable<DataModel> SqlDataSth
        {
            get
            {
                SqlConnection con = null;
                //               string connectionString = @"Data Source=tcp:sic-australia.database.windows.net;Database=ScadaTT_Staging;User ID = admin-xw7@sic-australia;Password=L]Z2JMh^UTM7;Trusted_Connection=False;Encrypt=True";
                con = new SqlConnection(Startup.getConfigureString());
                DateTime timeNow = DateTime.Now.AddHours(10);
                int day = ((int)timeNow.DayOfWeek == 0) ? 7 : (int)timeNow.DayOfWeek;
                day = day - 1;
                int day1 = day + 1;
                if (day1 == 8) { day1 = 1; }
                int baseLineNow = ((day1 * 24)+timeNow.Hour)*60/5;
                baseLineNow =baseLineNow+ (int)(timeNow.Minute/5);// baseLineNow+(int)(timeNow.Minute/5);
                int[,] baseLine = Startup.BaseLine(1);
                int baseLineStart = baseLineNow - 18;
                con.Open();
                int i = 1;
                int noofLinks;
                List<DataModel> sqlData = new List<DataModel>();
                SqlCommand command = con.CreateCommand();
                SqlDataReader Reader;
                command.CommandText = "Select * from Links where linkid<5";
                int linkLength;
                float fspeed;
                short[] shortBuffer = { 1234, 32678, 9012, 9876, 5432, 23 };
                char[] charBuffer = Array.ConvertAll(shortBuffer, Convert.ToChar);
                string buffer = new string(charBuffer);
               // try
               // {
                    Reader = command.ExecuteReader();
                    while (Reader.Read())
                    {
                        DataModel data = new DataModel();
                        data.ID = i;
                        data.SiteNo = Convert.ToInt32(Reader["LinkID"]);
                        data.LinkName = Convert.ToString(Reader["LinkText"]);
                        data.baseTT =  baseLine[(i-1), baseLineNow];//Convert.ToInt32(Reader["CurrentBaseTT"]);
                        data.CurrentTT = Convert.ToInt32(Reader["CurrentAverageTT"]);
                        //data.CurrentTT = Convert.ToInt32(Reader["CurrentTT"]);
                        linkLength = Convert.ToInt32(Reader["LinkLength"]);
                        fspeed = (3600 /(float) data.baseTT) * ((float)linkLength / 1000);
                        data.baseSpeed = (int)fspeed;
                        fspeed = ((3600 / (float)data.CurrentTT)) * (((float)linkLength /1000));
                        data.CurrentSpeed = (int)fspeed;
                        i++;
                        sqlData.Add(data);
                    }
                    Reader.Close();
                    noofLinks = sqlData.Count;
                    Utilities utilities = new Utilities();
                    long t = utilities.currentUnixTime();
                    long f = t;
                    t = t - (60*90);
                    int[] TT = new int[180];
                    int[] TTBase = new int[180];
                    int[] TTCount = new int[180];
                    short[] TTData = new short[180];
                    char[] TTDataChar = new char[180];
                    long finishtime;
                    int ttValue;
                    int index;
                string s,bs;
                for (i = 0; i < noofLinks; i++)
                {
                    for (int j = 0; j < 180; j++)
                    {
                        TT[j] = 0;
                        TTCount[j] = 0;
                        TTData[j] = 0;
                    }
                    command = new SqlCommand("select * from TravelTimes where Linkno =@dev and finishTime > @t and finishtime<@f", con);
                    command.Parameters.AddWithValue("@dev", (i + 1));
                    command.Parameters.AddWithValue("@t", t);
                    command.Parameters.AddWithValue("@f", f);
                    Reader = command.ExecuteReader();
                    while (Reader.Read())
                    {
                        finishtime = Convert.ToInt64(Reader["finishTime"]);
                        ttValue = Convert.ToInt32(Reader["baseTT"]);
                        //ttValue = Convert.ToInt32(Reader["TTime"]);
                        index = (int)((finishtime - t) / 30);
                        TT[index] =  (int)(TT[index] + ttValue);
                        TTCount[index]++;                        
                    }
                    Reader.Close();
                    s = " ";
                    bs = "";
                    for (int j = 0; j < 180; j++)
                    {
                           if (TTCount[j] != 0)
                           {
                               TTData[j] = (short)(TT[j] / TTCount[j]);
                           }
                           else
                           {
                               TTData[j] = 1;
                           }
                        TTBase[j] = baseLine[i, baseLineStart + (int)(j / 10)];
                        s = s + TTData[j].ToString() + " ";
                        bs = bs + TTBase[j].ToString() + " ";
                    }

                   // TTDataChar=Array.ConvertAll(TTData, Convert.ToChar);
                    sqlData.ElementAt(i).Last90MinTT = s;
                    sqlData.ElementAt(i).Last90MinBase= bs;
                    //sqlData.ElementAt(i).Last90MinBase =i.ToString()+ "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
                    //sqlData.ElementAt(i).Last90MinTT = i.ToString() + "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
                   
                }
                con.Close();
                return sqlData;
               // }
               // catch (Exception ex)
               // {
               //     return null;
               // }
            }
        }

        public IEnumerable<DataModel> SqlDataNth
        {
            get
            {
                SqlConnection con = null;
                //string connectionString = @"Data Source=tcp:sic-australia.database.windows.net;Database=ScadaTT_Staging;User ID = admin-xw7@sic-australia;Password=L]Z2JMh^UTM7;Trusted_Connection=False;Encrypt=True";
                con = new SqlConnection(Startup.getConfigureString());
                DateTime timeNow = DateTime.Now.AddHours(10);
                int day = ((int)timeNow.DayOfWeek == 0) ? 7 : (int)timeNow.DayOfWeek;
                day = day - 1;
                int baseLineNow = ((day * 24) + timeNow.Hour) * 60 / 5;
                baseLineNow = baseLineNow + (int)(timeNow.Minute / 5);// baseLineNow+(int)(timeNow.Minute/5);
                int[,] baseLine = Startup.BaseLine(1);
                int baseLineStart = baseLineNow - 18;
                con.Open();
                int i = 5;
                int k = 1;
                int noofLinks;
                List<DataModel> sqlData = new List<DataModel>();
                SqlCommand command = con.CreateCommand();
                SqlDataReader Reader;
                command.CommandText = "Select * from Links where linkid>4";
                int linkLength;
                float fspeed;
                short[] shortBuffer = { 1234, 32678, 9012, 9876, 5432, 23 };
                char[] charBuffer = Array.ConvertAll(shortBuffer, Convert.ToChar);
                string buffer = new string(charBuffer);
                // try
                // {
                Reader = command.ExecuteReader();
                while (Reader.Read())
                {
                    DataModel data = new DataModel();
                    data.ID = k;
                    data.SiteNo = Convert.ToInt32(Reader["LinkID"]);
                    data.LinkName = Convert.ToString(Reader["LinkText"]);
                    data.baseTT = baseLine[(i - 1), baseLineNow];//Convert.ToInt32(Reader["CurrentBaseTT"]);
                    data.CurrentTT = Convert.ToInt32(Reader["CurrentAverageTT"]);
                    //data.CurrentTT = Convert.ToInt32(Reader["CurrentTT"]);
                    linkLength = Convert.ToInt32(Reader["LinkLength"]);
                    fspeed = (3600 / (float)data.baseTT) * ((float)linkLength / 1000);
                    data.baseSpeed = (int)fspeed;
                    fspeed = ((3600 / (float)data.CurrentTT)) * (((float)linkLength / 1000));
                    data.CurrentSpeed = (int)fspeed;
                    i++;
                    k++;
                    sqlData.Add(data);
                }
                Reader.Close();
                noofLinks = sqlData.Count;
                Utilities utilities = new Utilities();
                long t = utilities.currentUnixTime();
                long f = t;
                t = t - (60 * 90);
                int[] TT = new int[180];
                int[] TTBase = new int[180];
                int[] TTCount = new int[180];
                short[] TTData = new short[180];
                char[] TTDataChar = new char[180];
                long finishtime;
                int ttValue;
                int index;
                string s, bs;
                for (i = 4; i < noofLinks+4; i++)
                {
                    for (int j = 0; j < 180; j++)
                    {
                        TT[j] = 0;
                        TTCount[j] = 0;
                        TTData[j] = 0;
                    }
                    command = new SqlCommand("select * from TravelTimes where Linkno =@dev and finishTime > @t and finishtime<@f", con);
                    command.Parameters.AddWithValue("@dev", (i + 1));
                    command.Parameters.AddWithValue("@t", t);
                    command.Parameters.AddWithValue("@f", f);
                    Reader = command.ExecuteReader();
                    while (Reader.Read())
                    {
                        finishtime = Convert.ToInt64(Reader["finishTime"]);
                        ttValue = Convert.ToInt32(Reader["baseTT"]);
                        //ttValue = Convert.ToInt32(Reader["TTime"]);
                        index = (int)((finishtime - t) / 30);
                        TT[index] = (int)(TT[index] + ttValue);
                        TTCount[index]++;
                    }
                    Reader.Close();
                    s = " ";
                    bs = "";
                    for (int j = 0; j < 180; j++)
                    {
                        if (TTCount[j] != 0)
                        {
                            TTData[j] = (short)(TT[j] / TTCount[j]);
                        }
                        else
                        {
                            TTData[j] = 1;
                        }
                        TTBase[j] =  baseLine[i, baseLineStart + (int)(j / 10)];
                        s = s + TTData[j].ToString() + " ";
                        bs = bs + TTBase[j].ToString() + " ";
                    }

                    // TTDataChar=Array.ConvertAll(TTData, Convert.ToChar);
                    sqlData.ElementAt(i-4).Last90MinTT = s;
                    sqlData.ElementAt(i-4).Last90MinBase = bs;
                    //sqlData.ElementAt(i).Last90MinBase =i.ToString()+ "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
                    //sqlData.ElementAt(i).Last90MinTT = i.ToString() + "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";

                }
                con.Close();
                return sqlData;
                // }
                // catch (Exception ex)
                // {
                //     return null;
                // }
            }
        }
        public int[,] getBaseData(int baseNo)
        {
            int[,] b;
            if (baseNo != 8) { b = new int[16, 2016]; } else { b = new int[50, 2016]; }
           SqlConnection con = null;
            // string connectionString = @"Data Source=tcp:sic-australia.database.windows.net;Database=ScadaTT_Staging;User ID = admin-xw7@sic-australia;Password=L]Z2JMh^UTM7;Trusted_Connection=False;Encrypt=True";
            string connectionString = Startup.getConfigureString();
            con = new SqlConnection(connectionString);
            con.Open();
            SqlCommand command = con.CreateCommand();
            SqlDataReader Reader;
            command.CommandText = String.Format("Select * from BaseData where BaseId={0} order by linkno,interval",baseNo);
            Reader = command.ExecuteReader();

            while (Reader.Read())
            {
                b[Convert.ToInt32(Reader["LinkNo"])-1, Convert.ToInt32(Reader["Interval"])-1] =Convert.ToInt32(Reader["Seconds"]);
            }
            Reader.Close();
            con.Close();
            Reader.Close();
            return b;
        }
        public int[,] getBaseVolData(int baseNo)
        {
            int[,] b = new int[16, 2016];
            SqlConnection con = null;
            // string connectionString = @"Data Source=tcp:sic-australia.database.windows.net;Database=ScadaTT_Staging;User ID = admin-xw7@sic-australia;Password=L]Z2JMh^UTM7;Trusted_Connection=False;Encrypt=True";
            con = new SqlConnection(Startup.getConfigureString());
            con.Open();
            SqlCommand command = con.CreateCommand();
            SqlDataReader Reader;
            command.CommandText = String.Format("Select * from BaseData where BaseId={0} order by linkno,interval",baseNo);
            Reader = command.ExecuteReader();

            while (Reader.Read())
            {
                b[Convert.ToInt32(Reader["LinkNo"]) - 1, Convert.ToInt32(Reader["Interval"]) - 1] = Convert.ToInt32(Reader["Seconds"]);
            }
            Reader.Close();
            con.Close();
            Reader.Close();
            return b;
        }
        public SqlConnection openSql()
        {
            SqlConnection con = null;
            // string connectionString = @"Data Source=tcp:sic-australia.database.windows.net;Database=ScadaTT_Staging;User ID = admin-xw7@sic-australia;Password=L]Z2JMh^UTM7;Trusted_Connection=False;Encrypt=True";
            con = new SqlConnection(Startup.getConfigureString());
            try {
                con.Open();
                return con;
            }catch(Exception e)
            {
                return null;
            }          
        }
        public int[] getVolDay(int link,int day,long finish, SqlConnection con)
        {
            long start = finish - (24 * 60 * 60);
            Random rnd = new Random();
            int[] data = new int[288];
            int[] number = new int[288];
            if (con==null || con.State == System.Data.ConnectionState.Closed) { return null; }
            long finishtime;
            int ttValue;
            int index;
            SqlCommand command = con.CreateCommand();
            SqlDataReader Reader;
            command = new SqlCommand("select * from TravelTimes where Linkno =@dev and finishTime > @t and finishtime<@f", con);
            command.Parameters.AddWithValue("@dev", (link));
            command.Parameters.AddWithValue("@t", start);
            command.Parameters.AddWithValue("@f", finish);
            Reader = command.ExecuteReader();
            while (Reader.Read())
            {
                finishtime = Convert.ToInt64(Reader["finishTime"]);
                ttValue = Convert.ToInt32(Reader["baseTT"]);
                //ttValue = Convert.ToInt32(Reader["TTime"]);
                index = (int)((finishtime - start) / 300);
                data[index] = (int)(data[index] + ttValue);
                number[index]++;
            }
            for (int i = 0; i < 288; i++)
            {
                if (number[i] != 0)
                {
                    data[i] =  number[i]*rnd.Next(8,12);
                }
                else
                {
                    data[i] = 0;
                }

            }
            Reader.Close();
            return data;
        }
        public int[] getDay(int link, int day, long finish)
        {
            var con = openSql();
            var data= getDay(link, day, finish, con);
            con.Close();
            return data;
        }
        public int[] getDay(int link, int day, long finish, SqlConnection con)
        {
            long start = finish - (24 * 60 * 60);
            int[] data = new int[288];
            int[] number = new int[288];
            if (con == null || con.State == System.Data.ConnectionState.Closed) { return null; }
            long finishtime;
            int ttValue;
            int index;
            SqlCommand command = con.CreateCommand();
            SqlDataReader Reader;
            command = new SqlCommand("select * from TravelTimes where Linkno =@dev and finishTime > @t and finishtime<@f", con);
            command.Parameters.AddWithValue("@dev", (link));
            command.Parameters.AddWithValue("@t", start);
            command.Parameters.AddWithValue("@f", finish);
            Reader = command.ExecuteReader();
            while (Reader.Read())
            {
                finishtime = Convert.ToInt64(Reader["finishTime"]);
                ttValue = Convert.ToInt32(Reader["baseTT"]);
                //ttValue = Convert.ToInt32(Reader["TTime"]);
                index = (int)((finishtime - start) / 300);
                data[index] = (int)(data[index] + ttValue);
                number[index]++;
            }
            for (int i = 0; i < 288; i++)
            {
                if (number[i] != 0)
                {
                    data[i] = data[i] / number[i];
                }
                else
                {
                    data[i] = 0;
                }

            }
            Reader.Close();
            return data;
        }
        public int[] getDayVol(int link, int day, long finish, SqlConnection con)
        {
            long start = finish - (24 * 60 * 60);
            int[] data = new int[288];
            int[] number = new int[288];
            if (con == null || con.State == System.Data.ConnectionState.Closed) { return null; }
            long finishtime;
            int ttValue;
            int index;
            SqlCommand command = con.CreateCommand();
            SqlDataReader Reader;
            command = new SqlCommand("select * from TravelTimes where Linkno =@dev and finishTime > @t and finishtime<@f", con);
            command.Parameters.AddWithValue("@dev", (link));
            command.Parameters.AddWithValue("@t", start);
            command.Parameters.AddWithValue("@f", finish);
            Reader = command.ExecuteReader();
            while (Reader.Read())
            {
                finishtime = Convert.ToInt64(Reader["finishTime"]);
                ttValue = Convert.ToInt32(Reader["baseTT"]);
                //ttValue = Convert.ToInt32(Reader["TTime"]);
                index = (int)((finishtime - start) / 300);
                data[index] = (int)(data[index] + ttValue);
                number[index]++;
            }
            for (int i = 0; i < 288; i++)
            {
                if (number[i] != 0)
                {
                    data[i] = number[i];
                }
                else
                {
                    data[i] = 0;
                }

            }
            Reader.Close();
            return data;
        }

    }
}