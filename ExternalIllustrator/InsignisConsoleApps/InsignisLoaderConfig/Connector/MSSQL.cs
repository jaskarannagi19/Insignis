using Octavo.Gate.Nabu.DOL.MSSQL;
using Octavo.Gate.Nabu.Entities;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insignis.Console.Apps.Data.Loader.Config.Connector
{
    public class MSSQL
    {
        private string ConnectionString = "";
        private string ErrorLog = "";

        public MSSQL(string pConnectionString, string pErrorLog)
        {
            ConnectionString = pConnectionString;
            ErrorLog = pErrorLog;
        }

        public BaseBoolean TestConnection(Echo pEcho)
        {
            BaseBoolean result = new BaseBoolean();
            try
            {

                BaseDOL baseDOL = new BaseDOL(ConnectionString, ErrorLog);
                BaseString[] recordSet = baseDOL.CustomQuery("SELECT * FROM sys.objects;", false);
                foreach (BaseString record in recordSet)
                {
                    if (record.ErrorsDetected == false)
                    {
                        result.Value = true;
                        break;
                    }
                    else
                    {
                        result.Value = false;
                        result.ErrorsDetected = true;
                        result.ErrorDetails = record.ErrorDetails;
                    }
                }
                recordSet = null;
                baseDOL = null;
            }
            catch (Exception exc)
            {
                if (pEcho != null)
                {
                    pEcho.WriteLine("Connector.MSSQL.TestConnection - Caught Error : " + exc.Message);
                    pEcho.WriteLine(exc.StackTrace);
                }
            }
            return result;
        }

        public BaseBoolean TableExists(string pTableName, Echo pEcho)
        {
            BaseBoolean result = new BaseBoolean();
            try
            {
                string tableExistsSQL = "";
                tableExistsSQL += "SELECT name from sys.objects ";
                if (pTableName.Contains("*") || pTableName.Contains("%"))
                    tableExistsSQL += "WHERE name LIKE '" + pTableName.Replace("*","%") + "' ";
                else
                    tableExistsSQL += "WHERE name='" + pTableName + "' ";
                tableExistsSQL += "AND type='U'";
                BaseDOL mssqlDOL = new BaseDOL(ConnectionString, ErrorLog);
                BaseString[] recordSet = mssqlDOL.CustomQuery(tableExistsSQL);
                if (recordSet.Length > 0)
                {
                    if (recordSet[0].ErrorsDetected == false)
                    {
                        // we have an existing table with this name
                        result.Value = true;
                    }
                    else
                    {
                        result.ErrorsDetected = true;
                        result.ErrorDetails = recordSet[0].ErrorDetails;
                    }
                }
            }
            catch (Exception exc)
            {
                if (pEcho != null)
                {
                    pEcho.WriteLine("Connector.MSSQL.TableExists - Caught Error : " + exc.Message);
                    pEcho.WriteLine(exc.StackTrace);
                }
            }
            return result;
        }

        public string[] GetMatchingTableNames(string pTableName, Echo pEcho)
        {
            List<string> matchingTableNames = new List<string>();
            try
            {
                string tableExistsSQL = "";
                tableExistsSQL += "SELECT name from sys.objects ";
                if (pTableName.Contains("*") || pTableName.Contains("%"))
                    tableExistsSQL += "WHERE name LIKE '" + pTableName.Replace("*", "%") + "' ";
                else
                    tableExistsSQL += "WHERE name='" + pTableName + "' ";
                tableExistsSQL += "AND type='U'";
                BaseDOL mssqlDOL = new BaseDOL(ConnectionString, ErrorLog);
                BaseString[] recordSet = mssqlDOL.CustomQuery(tableExistsSQL);
                foreach(BaseString record in recordSet)
                {
                    if (record.ErrorsDetected == false)
                        matchingTableNames.Add(record.Value.Replace("'",""));
                }
            }
            catch (Exception exc)
            {
                if (pEcho != null)
                {
                    pEcho.WriteLine("Connector.MSSQL.GetMatchingTableNames - Caught Error : " + exc.Message);
                    pEcho.WriteLine(exc.StackTrace);
                }
            }
            return matchingTableNames.ToArray();
        }

        public BaseBoolean DropTable(string pTableName, Echo pEcho)
        {
            BaseBoolean result = new BaseBoolean();
            try
            {
                string dropTableSQL = "";
                dropTableSQL += "DROP TABLE " + pTableName;
                BaseDOL mssqlDOL = new BaseDOL(ConnectionString, ErrorLog);
                result = mssqlDOL.CustomNonQuery(dropTableSQL, false);
            }
            catch (Exception exc)
            {
                if (pEcho != null)
                {
                    pEcho.WriteLine("Connector.MSSQL.DropTable - Caught Error : " + exc.Message);
                    pEcho.WriteLine(exc.StackTrace);
                }
            }
            return result;
        }

        public BaseString[] ExecuteCustomQuery(string pSQL)
        {
            BaseDOL mssqlDOL = new BaseDOL(ConnectionString, ErrorLog);
            return mssqlDOL.CustomQuery(pSQL, false);
        }

        public BaseKeyPair[] ExecuteCustomQueryKeyPair(string pSQL)
        {
            BaseDOL mssqlDOL = new BaseDOL(ConnectionString, ErrorLog);
            return mssqlDOL.CustomQueryKeyPair(pSQL, false);
        }

        public BaseBoolean ExecuteNonQuery(string pSQL)
        {
            BaseBoolean result = new BaseBoolean();
            BaseDOL mssqlDOL = new BaseDOL(ConnectionString, ErrorLog);
            result = mssqlDOL.CustomNonQuery(pSQL, false);
            mssqlDOL = null;
            return result;
        }
    }
}
