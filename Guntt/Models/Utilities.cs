using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Guntt.Models
{
    public class Utilities
    {
        public long currentUnixTime()
        {
            DateTime time;
            long uTime;
            DateTime sTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            try
            {
                time = DateTime.UtcNow;
                uTime = (long)(time - sTime).TotalSeconds;
                //uTime-= 10 * 60 * 60;
                return uTime;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
        public long UnixTime(DateTime d)
        {
            long uTime;
            DateTime sTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            try
            {
                
                uTime = (long)(d - sTime).TotalSeconds;
                return uTime;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

    }
}