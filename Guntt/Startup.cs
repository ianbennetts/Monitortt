using Microsoft.Owin;
using Owin;
using Guntt.Models;
[assembly: OwinStartupAttribute(typeof(Guntt.Startup))]
namespace Guntt
{
    public partial class Startup
    {
        private static int project;
        private static int[,] db = new DataBusiness().getBaseData(4);
        private static int[,] dbv = new DataBusiness().getBaseVolData(1);

        public static int[,] BaseLine(int p) {
            {
                project = p;
                return new DataBusiness().getBaseData(p);
            }
        }
        public static int[,] BaseLineVol()
        {
            {
                return dbv;
            }
        }
        public static string getConfigureString()
        {
            //   return  @"Data Source=tcp:sic-australia.database.windows.net;Database=ScadaTT_Staging;User ID = admin-xw7@sic-australia;Password=L]Z2JMh^UTM7;Trusted_Connection=False;Encrypt=True";
            // return @"Server = ALIENWARE-PC\SQLEXPRESS; Database = ScadaTT_Staging; Trusted_Connection = True";
           // return @"Server =MONITORTT; Database = ScadaTT_Staging; Trusted_Connection = True";
            return @"Data Source=45.76.120.30;Database=ScadaTT_Staging;User ID = ianbennetts;Password=Seahorse99";
        }

        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
