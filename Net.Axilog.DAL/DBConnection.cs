using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using IBM.Data.DB2.iSeries;

namespace Net.Axilog.DAL
{
    public class DBConnection
    {

        // this class creates a static connection that can acessed by any class within
        // this classes namespace. Be certain to close as needed

        public const string DB_CONN_STRING = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=WS_SMS.mdb";
        
        private static iDB2Connection DB_CONN; // = new iDB2Connection(_connectionstring);


        public static iDB2Connection GetDBConnection()
        {
            if (DB_CONN.State == ConnectionState.Closed)
            {
                DB_CONN.Open();
            }
            return DB_CONN;
        }

        public static void OpenDBConnection(String _connectionstring)
        {
            try
            {

                DB_CONN = new iDB2Connection(_connectionstring);

                DB_CONN.Open();
                                
            }
            catch (IBM.Data.DB2.iSeries.iDB2SQLErrorException ex)
            {
                throw ex;
            }
        }
    
    
    
    }


}
