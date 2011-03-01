using System.Data.SqlClient;

namespace EcoManager.Shared.DataAccess
{
    public class SQLConn
    {

        //Hent forbindelse fra .config fil
        public static string StrConn
        {
            get
            {
//#if (DEBUG)
  //              if (System.Diagnostics.Process.GetCurrentProcess().ProcessName.ToLower() == "devenv")
    //                return null;
//#endif
                return Properties.Settings.Default.ConnectionString;
            }
        }

        //Hent forbindelse fra .config fil
        public static string StrConnMysql
        {
            get
            {
                //#if (DEBUG)
                //              if (System.Diagnostics.Process.GetCurrentProcess().ProcessName.ToLower() == "devenv")
                //                return null;
                //#endif
                return Properties.Settings.Default.MySQLConnectionString;
            }
        }


    
    }


}
