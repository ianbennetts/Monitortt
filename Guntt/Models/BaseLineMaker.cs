using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ClosedXML.Excel;
using System.Data.SqlClient;
using System.Web.Script.Serialization;

namespace Guntt.Models
{
    public class BaseLineMaker
    {
        public XLWorkbook createTTBase(DateTime d,int noOfWeeks,string project)
        {
            IXLWorksheet worksheet;
            XLWorkbook excel = new XLWorkbook();
            Utilities utilities = new Utilities();
            DataBusiness db = new DataBusiness();
            if (d.DayOfWeek!=0) d=d.AddDays(7 - (double)d.DayOfWeek);
            DateTime startDate = d;
            DateTime finDate = startDate.AddDays(7 * noOfWeeks);
            if (finDate > DateTime.Now)
            {
                DateTime now = DateTime.Now;
                noOfWeeks = (int)(now - startDate).TotalDays / 7;
            }
            long t = utilities.UnixTime(startDate) + (14 * 60 * 60) ;
            int startLink;
            var links = db.getLinks(project);
            string path = HttpContext.Current.Server.MapPath("~/Content/");
            XLWorkbook excelTemplate = new XLWorkbook(path + "BaseLine WS Template.xlsx");
            worksheet = excelTemplate.Worksheet("Sheet1");
            foreach(var l in links)
            {
                worksheet.Name = l.LinkName;
                int[] baseLineTT = new int[2016];
                for (int i = 0; i < noOfWeeks; i++)
                {
                    string date = startDate.AddDays(7 * i).ToString("dd/MM");
                    long starttime= utilities.UnixTime(startDate.AddDays(7 * i)) + (14 * 60 * 60);
                    var TT = db.getTT(l.ID,l.CurrentTT,starttime, starttime + 24 * 60 * 60*7);
                    worksheet.Cell(1, 3+i).Value = "From "+date;
                    for (int j = 0; j < 2016; j++)
                    {
                        worksheet.Cell(2+j, 3+ i).Value = TT[j];
                        baseLineTT[j] += TT[j];
                    }
                }
                for (int j = 0; j < 2016; j++)
                {
                    baseLineTT[j] = baseLineTT[j] / noOfWeeks;
                    worksheet.Cell(2 + j, 3 + noOfWeeks).Value = baseLineTT[j];
                }
                var jsonData=new JavaScriptSerializer().Serialize(baseLineTT);
                db.updateLineBaseTT(l.ID, jsonData);
                worksheet.Cell(1, 3 + noOfWeeks).Value = "Baseline";
                excel.AddWorksheet(worksheet);
            }
            return excel;
            
            int[] data;
            int[] start = { 440, 441, 426, 185, 410, 195, 180, 330, 440, 130, 130, 410, 220, 130, 240, 350 };
            string[] workname = { "Bruce Sth -> Nth", "Bruce Nth -> Sth","Bruce Nth -> Cal", "Bruce Sth -> Cal", "Bruce Nth -> Steve", "Bruce Sth -> Steve", "Bruce Nth -> Motor",
                "Bruce Sth -> Motor","Cal -> Bruce Nth","Cal -> Bruce Sth","Cal -> Steve","Steve -> Bruce Nth","Steve -> Bruce Sth","Steve -> Cal","Motor -> Bruce Nth","Motor -> Bruce Sth" };
            SqlConnection con = db.openSql();
            if (con == null) return null;
            for (int link = 10; link < 26; link++)
            {
                t = utilities.UnixTime(startDate) + (14 * 60 * 60) - 10;
                worksheet = excel.Worksheet(workname[link - 10]);
                for (int week = 0; week < 8; week++)
                {
                    for (int day = 0; day < 7; day++)
                    {
                        data = db.getDay(link, day, t, con);
                        if (data[0] == 0) { data[0] = start[link - 10]; }
                        for (int i = 2; i < 290; i++)
                        {
                            if (data[i - 2] == 0)
                            {
                                data[i - 2] = data[i - 3];
                            }
                            worksheet.Cell(i + (day * 288), 3 + (week * 1)).Value = data[i - 2];
                        }
                        //do something on the excel spread sheet
                        t += (24 * 60 * 60);
                    }
                }
            }
            return excel;
        }
        public XLWorkbook createVolBase()
        {
            IXLWorksheet worksheet;
            XLWorkbook excel;
            Utilities utilities = new Utilities();
            string sd = "30/10/2016";
            DateTime startDate = Convert.ToDateTime(sd);
            long t = utilities.UnixTime(startDate) + (14 * 60 * 60) - 10;
            int startLink;
            int[,] ws7days = new int[2, 675];
            string path = HttpContext.Current.Server.MapPath("~/Content/");
            excel = new XLWorkbook(path + "VolBaseLine Template.xlsx");

            DataBusiness db = new DataBusiness();
            int[] data;
            int[] start = { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };
            string[] workname = { "Bruce Sth -> Nth", "Bruce Nth -> Sth","Bruce Nth -> Cal", "Bruce Sth -> Cal", "Bruce Nth -> Steve", "Bruce Sth -> Steve", "Bruce Nth -> Motor",
                "Bruce Sth -> Motor","Cal -> Bruce Nth","Cal -> Bruce Sth","Cal -> Steve","Steve -> Bruce Nth","Steve -> Bruce Sth","Steve -> Cal","Motor -> Bruce Nth","Motor -> Bruce Sth" };
            SqlConnection con = db.openSql();
            if (con == null) return null;
            for (int link = 10; link < 26; link++)
            {
                t = utilities.UnixTime(startDate) + (14 * 60 * 60) - 10;
                worksheet = excel.Worksheet(workname[link - 10]);
                for (int week = 0; week < 8; week++)
                {
                    for (int day = 0; day < 7; day++)
                    {
                        data = db.getVolDay(link, day, t, con);
                        if (data[0] == 0) { data[0] = start[link - 10]; }
                        for (int i = 2; i < 290; i++)
                        {
                            if (data[i - 2] == 0)
                            {
                                data[i - 2] = data[i - 3];
                            }
                            worksheet.Cell(i + (day * 288), 3 + (week * 1)).Value = data[i - 2];
                        }
                        //do something on the excel spread sheet
                        t += (24 * 60 * 60);
                    }
                }
            }
            return excel;
        }
        public XLWorkbook createWeek(DateTime dateReq, string project)
        {

            IXLWorksheet worksheet;
            XLWorkbook excel;
            Utilities utilities = new Utilities();
            DateTime startDate = dateReq;
            int linkNo = 1;
            int projectInt = 4;
            if (project == "GUN") projectInt = 1;
            if (project == "BH") projectInt = 3;
            if (project == "DAAJ1Qw") projectInt = 4;
            //DateTime currentDate = startDate;
            long t = utilities.UnixTime(startDate) + (14 * 60 * 60) - 10;
            int day, day1;
            int startLink;
            string[] dayofweek = { "", "Mon", "Tues", "Wed", "Thurs", "Fri", "Sat", "Sun" };
            string[] workname = { "Warrego Highway East WB", "Condamine St", "Cunningham St", "Warrego Highway West WB", "Nicholson St", "Warrego Highway East EB", "Warrego Highway West EB", };
            int[,] ws7days = new int[2, 675];
            string path = HttpContext.Current.Server.MapPath("~/Content/");
            excel = new XLWorkbook(path + "Weekly Template.xlsx");

            //            worksheet.Cell(1, 2).Value = dateReq.ToString("dd/MM/yy");
            DataBusiness db = new DataBusiness();
            int[,] baseLine = Startup.BaseLine(projectInt);
            int[] data;
            day1 = ((int)startDate.DayOfWeek == 0) ? 7 : (int)startDate.DayOfWeek;
            SqlConnection con = db.openSql();
            if (con == null) return null;
            day = day1;
            t = utilities.UnixTime(startDate) + (14 * 60 * 60) - 10;
            for (int j = 0; j < 7; j++)
            {
                day1 = day + 1;
                if (day1 == 8) { day1 = 1; }
                startLink = (day1 - 1) * 288;
                for (linkNo = 28; linkNo < 35; linkNo++)
                {
                    worksheet = excel.Worksheet(workname[linkNo - 28]);
                    if (j == 0)
                    {
                        worksheet.Cell(1, 4).Value = "Actual";
                        worksheet.Cell(1, 3).Value = "Baseline";

                    }
                    worksheet.Cell(2 + (288 * j), 1).Value = dayofweek[day];
                    data = db.getDay(linkNo, day, t, con);
                    for (int i = 2; i < 290; i++)
                    {
                        if (data[i - 2] == 0 && i > 2)
                        {
                            if (data[i - 3] == 0)
                            {
                                data[i - 2] = baseLine[linkNo - 28, startLink + i - 2];
                            }
                            else
                            {
                                data[i - 2] = data[i - 3];
                            }
                        }
                        if ((i == 2) && (j == 0) && (data[0] == 0)) { data[0] = baseLine[linkNo - 28, startLink + i - 2]; }
                        if ((i == 2) && (j != 0) && (data[0] == 0)) { data[0] = (int)worksheet.Cell(1 + (288 * j), 4).GetDouble(); }

                        //                    masterLoc = ((day - 1) * 96) + (int)(i / 3)+2;
                        //                  ws7days[1,masterLoc]+=data[i - 2];
                        //                ws7days[0, masterLoc] += baseLine[linkNo - 1, startLink + i - 2];

                        //                            worksheet.Cell(i + 3, (3 + (j * 2)) ).Value = worksheet.Cell(i + 3, (3 + (j * 2)) ).GetDouble() + data[i - 2];
                        //                            worksheet.Cell(i + 3, (2 + (j * 2)) ).Value = worksheet.Cell(i + 3, (2 + (j * 2)) ).GetDouble() + baseLine[linkNo - 1, startLink + i - 2];
                        worksheet.Cell(i + (288 * j), 4).Value = data[i - 2];
                        worksheet.Cell(i + (288 * j), 3).Value = baseLine[linkNo - 28, startLink + i - 2];

                    }
                }

                //do something on the excel spread sheet
                day++;
                if (day == 8)
                {
                    day = 1;
                }
                t += (24 * 60 * 60);

            }

            return excel;
        }
        public XLWorkbook createDay(DateTime dateReq, string project)
        {
            IXLWorksheet[] worksheets = new IXLWorksheet[1];
            XLWorkbook excel;
            int linkNo = 1;
            int day1;
            int projectInt = 4;
            if (project == "GUN") projectInt = 1;
            if (project == "BH") projectInt = 3;
            if (project == "DAAJ1Qw") projectInt = 4;
            Utilities utilities = new Utilities();
            int day = ((int)dateReq.DayOfWeek == 0) ? 7 : (int)dateReq.DayOfWeek;
            day1 = day + 1;
            if (day1 == 8) { day1 = 1; }
            DataBusiness db = new DataBusiness();
            int[] data;
            SqlConnection con = db.openSql();
            if (con == null) return null;
            long t = utilities.UnixTime(dateReq) + (14 * 60 * 60);

            string path = HttpContext.Current.Server.MapPath("~/Content/");
            excel = new XLWorkbook(path + "DayTemplate1.xlsx");
            worksheets[0] = excel.Worksheet("Days Data");
            worksheets[0].Cell(1, 2).Value = dateReq.ToString("dd/MM/yy");
            int[,] baseLine = Startup.BaseLine(projectInt);
            //    data = db.getDay(linkNo, day, t, con);
            int startLink = (day1 - 1) * 288;
            for (linkNo = 28; linkNo < 35; linkNo++)
            {
                data = db.getDay(linkNo, day, t, con);
                for (int i = 2; i < 290; i++)
                {
                    if (data[i - 2] == 0 && i > 2)
                    {
                        if (data[i - 3] == 0)
                        {
                            data[i - 2] = baseLine[linkNo - 28, startLink + i - 2];// something weird
                        }
                        else
                        {
                            data[i - 2] = data[i - 3];
                        }
                    }
                    if ((i == 2) && (data[0] == 0)) { data[0] = baseLine[linkNo - 28, startLink]; }
                    worksheets[0].Cell(i + 2, 3 + ((linkNo - 28) * 2)).Value = data[i - 2];
                    worksheets[0].Cell(i + 2, 2 + ((linkNo - 28) * 2)).Value = baseLine[linkNo - 28, startLink + i - 2];
                }
            }
            // worksheets[0].Cell(2, 2).Value = "GMT";
            // worksheets[0].Cell(2, 3).Value = t;
            // worksheets[0].Cell(3, 3).Value = dateReq.ToShortDateString();
            // worksheets[0].Cell(2, 2).Value = day;
            return excel;
        }
        public XLWorkbook createWeekVol(DateTime dateReq)
        {
            IXLWorksheet worksheet;
            XLWorkbook excel;
            Utilities utilities = new Utilities();
            DateTime startDate = dateReq;
            Random rnd = new Random();
            int linkNo = 1;
            //DateTime currentDate = startDate;
            long t = utilities.UnixTime(startDate) + (14 * 60 * 60) - 10;
            int day, day1;
            int startLink;
            string[] dayofweek = { "", "Mon", "Tues", "Wed", "Thurs", "Fri", "Sat", "Sun" };
            string[] workname = { "Bruce Sth -> Nth", "Bruce Nth -> Sth","Bruce Nth -> Cal", "Bruce Sth -> Cal", "Bruce Nth -> Steve", "Bruce Sth -> Steve", "Bruce Nth -> Motor",
                "Bruce Sth -> Motor","Cal -> Bruce Nth","Cal -> Bruce Sth","Cal -> Steve","Steve -> Bruce Nth","Steve -> Bruce Sth","Steve -> Cal","Motor -> Bruce Nth","Motor -> Bruce Sth" };
            int[,] ws7days = new int[2, 675];
            string path = HttpContext.Current.Server.MapPath("~/Content/");
            excel = new XLWorkbook(path + "Weekly Vol Template.xlsx");

            //            worksheet.Cell(1, 2).Value = dateReq.ToString("dd/MM/yy");
            DataBusiness db = new DataBusiness();
            int[,] baseLine = Startup.BaseLineVol();
            int[] data;
            day1 = ((int)startDate.DayOfWeek == 0) ? 7 : (int)startDate.DayOfWeek;
            SqlConnection con = db.openSql();
            if (con == null) return null;
            day = day1;
            t = utilities.UnixTime(startDate) + (14 * 60 * 60) - 10;
            for (int j = 0; j < 7; j++)
            {
                day1 = day + 1;
                if (day1 == 8) { day1 = 1; }
                startLink = (day1 - 1) * 288;
                for (linkNo = 10; linkNo < 26; linkNo++)
                {
                    worksheet = excel.Worksheet(workname[linkNo - 10]);
                    if (j == 0)
                    {
                        worksheet.Cell(1, 4).Value = "Actual";
                        worksheet.Cell(1, 3).Value = "Baseline";

                    }
                    worksheet.Cell(2 + (288 * j), 1).Value = dayofweek[day];
                    data = db.getDayVol(linkNo, day, t, con);
                    for (int i = 2; i < 290; i++)
                    {
                        if (data[i - 2] == 0 && i > 2)
                        {
                            if (data[i - 3] == 0)
                            {
                                data[i - 2] = baseLine[linkNo - 10, startLink + i - 2];
                            }
                            else
                            {
                                data[i - 2] = data[i - 3];
                            }
                        }
                        if ((i == 2) && (j == 0) && (data[0] == 0)) { data[0] = baseLine[linkNo - 10, startLink + i - 2]; }
                        if ((i == 2) && (j != 0) && (data[0] == 0)) { data[0] = (int)worksheet.Cell(1 + (288 * j), 4).GetDouble(); }

                        //                    masterLoc = ((day - 1) * 96) + (int)(i / 3)+2;
                        //                  ws7days[1,masterLoc]+=data[i - 2];
                        //                ws7days[0, masterLoc] += baseLine[linkNo - 1, startLink + i - 2];

                        //                            worksheet.Cell(i + 3, (3 + (j * 2)) ).Value = worksheet.Cell(i + 3, (3 + (j * 2)) ).GetDouble() + data[i - 2];
                        //                            worksheet.Cell(i + 3, (2 + (j * 2)) ).Value = worksheet.Cell(i + 3, (2 + (j * 2)) ).GetDouble() + baseLine[linkNo - 1, startLink + i - 2];
                        worksheet.Cell(i + (288 * j), 4).Value = data[i - 2] * rnd.Next(8, 12); ; ;
                        worksheet.Cell(i + (288 * j), 3).Value = baseLine[linkNo - 10, startLink + i - 2];

                    }
                }

                //do something on the excel spread sheet
                day++;
                if (day == 8)
                {
                    day = 1;
                }
                t += (24 * 60 * 60);

            }

            return excel;
        }
        public XLWorkbook createDayVol(DateTime dateReq)
        {
            IXLWorksheet[] worksheets = new IXLWorksheet[1];
            XLWorkbook excel;
            int linkNo = 1;
            int day1;
            Utilities utilities = new Utilities();
            int day = ((int)dateReq.DayOfWeek == 0) ? 7 : (int)dateReq.DayOfWeek;
            day1 = day + 1;
            if (day1 == 8) { day1 = 1; }
            DataBusiness db = new DataBusiness();
            int[] data;
            SqlConnection con = db.openSql();
            if (con == null) return null;
            long t = utilities.UnixTime(dateReq) + (14 * 60 * 60);
            Random rnd = new Random();
            string path = HttpContext.Current.Server.MapPath("~/Content/");
            excel = new XLWorkbook(path + "Day Vol Template.xlsx");
            worksheets[0] = excel.Worksheet("Days Data");
            worksheets[0].Cell(1, 2).Value = dateReq.ToString("dd/MM/yy");
            int[,] baseLineVol = Startup.BaseLineVol();
            //    data = db.getDay(linkNo, day, t, con);
            int startLink = (day1 - 1) * 288;
            for (linkNo = 10; linkNo < 26; linkNo++)
            {
                data = db.getDayVol(linkNo, day, t, con);
                for (int i = 2; i < 290; i++)
                {
                    if (data[i - 2] == 0 && i > 2)
                    {
                        if (data[i - 3] == 0)
                        {
                            data[i - 2] = baseLineVol[linkNo - 10, startLink + i - 2];// something weird
                        }
                        else
                        {
                            data[i - 2] = data[i - 3];
                        }
                    }
                    if ((i == 2) && (data[0] == 0)) { data[0] = baseLineVol[linkNo - 10, startLink]; }
                    worksheets[0].Cell(i + 2, 3 + ((linkNo - 10) * 2)).Value = data[i - 2] * rnd.Next(8, 12); ;
                    worksheets[0].Cell(i + 2, 2 + ((linkNo - 10) * 2)).Value = baseLineVol[linkNo - 10, startLink + i - 2];
                }
            }
            // worksheets[0].Cell(2, 2).Value = "GMT";
            // worksheets[0].Cell(2, 3).Value = t;
            // worksheets[0].Cell(3, 3).Value = dateReq.ToShortDateString();
            // worksheets[0].Cell(2, 2).Value = day;
            return excel;
        }

    }
}