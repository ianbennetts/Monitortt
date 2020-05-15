using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Guntt.Models;
using System.Net.NetworkInformation;


namespace Guntt.Controllers
{
    public class DataController : ApiController
    {
        [HttpGet]
        public string Get(string project)
        {
            //https://static.wixstatic.com/media/d10897_77bb648cd8ec4d0c95b9b06e667f63ba~mv2_d_5120_3417_s_4_2.jpg
            string s = "";
            DataBusiness dataBusiness = new DataBusiness();
            IEnumerable<DataModel> sqlData = dataBusiness.getSqlData(project);
            DataModel data = new DataModel();
            int length = sqlData.Count();
            s = length.ToString() + ",";
            for (int i = 0; i < length; i++)

            {
                data = sqlData.ElementAt(i);
                //s = s + data.LinkName + "," + data.baseTT.ToString() + "," + data.baseSpeed.ToString() + "," + data.CurrentTT.ToString() + ","
                //    + data.CurrentSpeed.ToString() + "," + data.Last90MinTT + "," + data.Last90MinBase + ",";
                s = s + data.LinkName + "," + data.baseTT.ToString() + "," + data.baseSpeed.ToString() + "," + data.delay.ToString() + ","+ data.CurrentTT.ToString() + ","
                    + data.CurrentSpeed.ToString() + ",";
            }

            return (s);
        }

        [HttpPost]
        public string Graph([FromBody]string project)
        {
            string s = "";
            DataBusiness dataBusiness = new DataBusiness();
            IEnumerable<DataModel> sqlData = dataBusiness.getSqlData(project);
            DataModel data = new DataModel();
            int length = sqlData.Count();
            s = "," + length.ToString() + ",";
            for (int i = 0; i < length; i++)
            {
                data = sqlData.ElementAt(i);
                s = s + data.LinkName + "," + data.baseTT.ToString() + "," + data.baseSpeed.ToString() + "," + data.delay.ToString() + ","  + data.CurrentTT.ToString() + ","
                    + data.CurrentSpeed.ToString() + "," + data.Last90MinTT + "," + data.Last90MinBase + ",";
            }

            return (s);
        }

        //used to be post
        [HttpPost]
        public string Device([FromBody]string project)
        {
            string s = "";

            if (project != "")
            {

                DeviceBusiness deviceBusiness = new DeviceBusiness();
                IEnumerable<DeviceModel> sqlDevices = deviceBusiness.getDevices(project);
                DeviceModel device = new DeviceModel();
                //             string b = deviceBusiness.testSockets();
                int length = sqlDevices.Count();
                s = "," + length.ToString() + ",";
                for (int i = 0; i < length; i++)
                {
                    device = sqlDevices.ElementAt(i);
                    //s = s+device.SiteNo.ToString() + "," + device.BtDevice.ToString() + "," + device.When.ToString()+",";
                    s = s + device.NoOfSightings.ToString() + "," + device.BtDevice.ToString() + "," + device.When.ToString() + ",";

                }
                return (s);
            }

            return null;
        }

    }
}
