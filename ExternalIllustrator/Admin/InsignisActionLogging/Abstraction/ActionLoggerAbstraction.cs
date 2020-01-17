using Insignis.Asset.Management.Action.Logging.Entities;
using Octavo.Gate.Nabu.Abstraction;
using Octavo.Gate.Nabu.Entities;

namespace Insignis.Asset.Management.Action.Logging.Abstraction
{
    public class ActionLoggerAbstraction : BaseAbstraction
    {
        public ActionLoggerAbstraction() : base()
        {
        }

        public ActionLoggerAbstraction(string pConnectionString, Octavo.Gate.Nabu.Entities.DatabaseType pDBType, string pErrorLogFile) : base(pConnectionString, pDBType, pErrorLogFile)
        {
        }

        /**********************************************************************
         * ACTION LOG
         *********************************************************************/
        public ActionLog GetActionLog(int pActionLogID)
        {
            if (base.DBType == Octavo.Gate.Nabu.Entities.DatabaseType.MSSQL)
            {
                DOL.ActionLogDOL DOL = new DOL.ActionLogDOL(base.ConnectionString, base.ErrorLogFile);
                return DOL.Get(pActionLogID);
            }
            else
                return null;
        }

        public ActionLog GetActionLogByReference(string pReference)
        {
            if (base.DBType == Octavo.Gate.Nabu.Entities.DatabaseType.MSSQL)
            {
                DOL.ActionLogDOL DOL = new DOL.ActionLogDOL(base.ConnectionString, base.ErrorLogFile);
                return DOL.GetByReference(pReference);
            }
            else
                return null;
        }

        public BaseString[] ListSystems()
        {
            if (base.DBType == Octavo.Gate.Nabu.Entities.DatabaseType.MSSQL)
            {
                DOL.ActionLogDOL DOL = new DOL.ActionLogDOL(base.ConnectionString, base.ErrorLogFile);
                return DOL.ListSystems();
            }
            else
                return null;
        }

        public BaseString[] ListModules(string pSystem)
        {
            if (base.DBType == Octavo.Gate.Nabu.Entities.DatabaseType.MSSQL)
            {
                DOL.ActionLogDOL DOL = new DOL.ActionLogDOL(base.ConnectionString, base.ErrorLogFile);
                return DOL.ListModules(pSystem);
            }
            else
                return null;
        }

        public BaseString[] ListMethods(string pSystem, string pModule)
        {
            if (base.DBType == Octavo.Gate.Nabu.Entities.DatabaseType.MSSQL)
            {
                DOL.ActionLogDOL DOL = new DOL.ActionLogDOL(base.ConnectionString, base.ErrorLogFile);
                return DOL.ListMethods(pSystem, pModule);
            }
            else
                return null;
        }

        public BaseString[] ListActions(string pSystem, string pModule, string pMethod)
        {
            if (base.DBType == Octavo.Gate.Nabu.Entities.DatabaseType.MSSQL)
            {
                DOL.ActionLogDOL DOL = new DOL.ActionLogDOL(base.ConnectionString, base.ErrorLogFile);
                return DOL.ListActions(pSystem, pModule, pMethod);
            }
            else
                return null;
        }

        public ActionLog[] ListActionLog(string pSystem, string pModule, string pMethod, string pAction)
        {
            if (base.DBType == Octavo.Gate.Nabu.Entities.DatabaseType.MSSQL)
            {
                DOL.ActionLogDOL DOL = new DOL.ActionLogDOL(base.ConnectionString, base.ErrorLogFile);
                return DOL.List(pSystem, pModule, pMethod,pAction);
            }
            else
                return null;
        }

        public ActionLog InsertActionLog(ActionLog pActionLog)
        {
            if (base.DBType == Octavo.Gate.Nabu.Entities.DatabaseType.MSSQL)
            {
                DOL.ActionLogDOL DOL = new DOL.ActionLogDOL(base.ConnectionString, base.ErrorLogFile);
                return DOL.Insert(pActionLog);
            }
            else
                return null;
        }

        public ActionLog UpdateActionLog(ActionLog pActionLog)
        {
            if (base.DBType == Octavo.Gate.Nabu.Entities.DatabaseType.MSSQL)
            {
                DOL.ActionLogDOL DOL = new DOL.ActionLogDOL(base.ConnectionString, base.ErrorLogFile);
                return DOL.Update(pActionLog);
            }
            else
                return null;
        }

        public ActionLog DeleteActionLog(ActionLog pActionLog)
        {
            if (base.DBType == Octavo.Gate.Nabu.Entities.DatabaseType.MSSQL)
            {
                DOL.ActionLogDOL DOL = new DOL.ActionLogDOL(base.ConnectionString, base.ErrorLogFile);
                return DOL.Delete(pActionLog);
            }
            else
                return null;
        }
        /**********************************************************************
         * ACTION OUTPUT DOCUMENT
         *********************************************************************/
        public ActionOutputDocument GetActionOutputDocument(int pActionOutputDocumentID)
        {
            if (base.DBType == Octavo.Gate.Nabu.Entities.DatabaseType.MSSQL)
            {
                DOL.ActionOutputDocumentDOL DOL = new DOL.ActionOutputDocumentDOL(base.ConnectionString, base.ErrorLogFile);
                return DOL.Get(pActionOutputDocumentID);
            }
            else
                return null;
        }

        public ActionOutputDocument[] ListActionOutputDocument(int pActionLogID)
        {
            if (base.DBType == Octavo.Gate.Nabu.Entities.DatabaseType.MSSQL)
            {
                DOL.ActionOutputDocumentDOL DOL = new DOL.ActionOutputDocumentDOL(base.ConnectionString, base.ErrorLogFile);
                return DOL.List(pActionLogID);
            }
            else
                return null;
        }

        public ActionOutputDocument InsertActionOutputDocument(ActionOutputDocument pActionOutputDocument, int pActionLogID)
        {
            if (base.DBType == Octavo.Gate.Nabu.Entities.DatabaseType.MSSQL)
            {
                DOL.ActionOutputDocumentDOL DOL = new DOL.ActionOutputDocumentDOL(base.ConnectionString, base.ErrorLogFile);
                return DOL.Insert(pActionOutputDocument, pActionLogID);
            }
            else
                return null;
        }

        public ActionOutputDocument UpdateActionOutputDocument(ActionOutputDocument pActionOutputDocument)
        {
            if (base.DBType == Octavo.Gate.Nabu.Entities.DatabaseType.MSSQL)
            {
                DOL.ActionOutputDocumentDOL DOL = new DOL.ActionOutputDocumentDOL(base.ConnectionString, base.ErrorLogFile);
                return DOL.Update(pActionOutputDocument);
            }
            else
                return null;
        }

        public ActionOutputDocument DeleteActionOutputDocument(ActionOutputDocument pActionOutputDocument)
        {
            if (base.DBType == Octavo.Gate.Nabu.Entities.DatabaseType.MSSQL)
            {
                DOL.ActionOutputDocumentDOL DOL = new DOL.ActionOutputDocumentDOL(base.ConnectionString, base.ErrorLogFile);
                return DOL.Delete(pActionOutputDocument);
            }
            else
                return null;
        }
        /**********************************************************************
         * ACTION OUTPUT TABLE
         *********************************************************************/
        public ActionOutputTable GetActionOutputTable(int pActionOutputTableID)
        {
            if (base.DBType == Octavo.Gate.Nabu.Entities.DatabaseType.MSSQL)
            {
                DOL.ActionOutputTableDOL DOL = new DOL.ActionOutputTableDOL(base.ConnectionString, base.ErrorLogFile);
                return DOL.Get(pActionOutputTableID);
            }
            else
                return null;
        }

        public ActionOutputTable[] ListActionOutputTable(int pActionLogID)
        {
            if (base.DBType == Octavo.Gate.Nabu.Entities.DatabaseType.MSSQL)
            {
                DOL.ActionOutputTableDOL DOL = new DOL.ActionOutputTableDOL(base.ConnectionString, base.ErrorLogFile);
                return DOL.List(pActionLogID);
            }
            else
                return null;
        }

        public ActionOutputTable InsertActionOutputTable(ActionOutputTable pActionOutputTable, int pActionLogID)
        {
            if (base.DBType == Octavo.Gate.Nabu.Entities.DatabaseType.MSSQL)
            {
                DOL.ActionOutputTableDOL DOL = new DOL.ActionOutputTableDOL(base.ConnectionString, base.ErrorLogFile);
                return DOL.Insert(pActionOutputTable, pActionLogID);
            }
            else
                return null;
        }

        public ActionOutputTable UpdateActionOutputTable(ActionOutputTable pActionOutputTable)
        {
            if (base.DBType == Octavo.Gate.Nabu.Entities.DatabaseType.MSSQL)
            {
                DOL.ActionOutputTableDOL DOL = new DOL.ActionOutputTableDOL(base.ConnectionString, base.ErrorLogFile);
                return DOL.Update(pActionOutputTable);
            }
            else
                return null;
        }

        public ActionOutputTable DeleteActionOutputTable(ActionOutputTable pActionOutputTable)
        {
            if (base.DBType == Octavo.Gate.Nabu.Entities.DatabaseType.MSSQL)
            {
                DOL.ActionOutputTableDOL DOL = new DOL.ActionOutputTableDOL(base.ConnectionString, base.ErrorLogFile);
                return DOL.Delete(pActionOutputTable);
            }
            else
                return null;
        }
        /**********************************************************************
         * ACTION OUTPUT VALUE
         *********************************************************************/
        public ActionOutputValue GetActionOutputValue(int pActionOutputValueID)
        {
            if (base.DBType == Octavo.Gate.Nabu.Entities.DatabaseType.MSSQL)
            {
                DOL.ActionOutputValueDOL DOL = new DOL.ActionOutputValueDOL(base.ConnectionString, base.ErrorLogFile);
                return DOL.Get(pActionOutputValueID);
            }
            else
                return null;
        }

        public ActionOutputValue[] ListActionOutputValue(int pActionLogID)
        {
            if (base.DBType == Octavo.Gate.Nabu.Entities.DatabaseType.MSSQL)
            {
                DOL.ActionOutputValueDOL DOL = new DOL.ActionOutputValueDOL(base.ConnectionString, base.ErrorLogFile);
                return DOL.List(pActionLogID);
            }
            else
                return null;
        }

        public ActionOutputValue InsertActionOutputValue(ActionOutputValue pActionOutputValue, int pActionLogID)
        {
            if (base.DBType == Octavo.Gate.Nabu.Entities.DatabaseType.MSSQL)
            {
                DOL.ActionOutputValueDOL DOL = new DOL.ActionOutputValueDOL(base.ConnectionString, base.ErrorLogFile);
                return DOL.Insert(pActionOutputValue, pActionLogID);
            }
            else
                return null;
        }

        public ActionOutputValue UpdateActionOutputValue(ActionOutputValue pActionOutputValue)
        {
            if (base.DBType == Octavo.Gate.Nabu.Entities.DatabaseType.MSSQL)
            {
                DOL.ActionOutputValueDOL DOL = new DOL.ActionOutputValueDOL(base.ConnectionString, base.ErrorLogFile);
                return DOL.Update(pActionOutputValue);
            }
            else
                return null;
        }

        public ActionOutputValue DeleteActionOutputValue(ActionOutputValue pActionOutputValue)
        {
            if (base.DBType == Octavo.Gate.Nabu.Entities.DatabaseType.MSSQL)
            {
                DOL.ActionOutputValueDOL DOL = new DOL.ActionOutputValueDOL(base.ConnectionString, base.ErrorLogFile);
                return DOL.Delete(pActionOutputValue);
            }
            else
                return null;
        }
        /**********************************************************************
         * ACTION SETTING
         *********************************************************************/
        public ActionSetting GetActionSetting(int pActionSettingID)
        {
            if (base.DBType == Octavo.Gate.Nabu.Entities.DatabaseType.MSSQL)
            {
                DOL.ActionSettingDOL DOL = new DOL.ActionSettingDOL(base.ConnectionString, base.ErrorLogFile);
                return DOL.Get(pActionSettingID);
            }
            else
                return null;
        }

        public ActionSetting[] ListActionSetting(int pActionLogID)
        {
            if (base.DBType == Octavo.Gate.Nabu.Entities.DatabaseType.MSSQL)
            {
                DOL.ActionSettingDOL DOL = new DOL.ActionSettingDOL(base.ConnectionString, base.ErrorLogFile);
                return DOL.List(pActionLogID);
            }
            else
                return null;
        }

        public ActionSetting InsertActionSetting(ActionSetting pActionSetting, int pActionLogID)
        {
            if (base.DBType == Octavo.Gate.Nabu.Entities.DatabaseType.MSSQL)
            {
                DOL.ActionSettingDOL DOL = new DOL.ActionSettingDOL(base.ConnectionString, base.ErrorLogFile);
                return DOL.Insert(pActionSetting, pActionLogID);
            }
            else
                return null;
        }

        public ActionSetting UpdateActionSetting(ActionSetting pActionSetting)
        {
            if (base.DBType == Octavo.Gate.Nabu.Entities.DatabaseType.MSSQL)
            {
                DOL.ActionSettingDOL DOL = new DOL.ActionSettingDOL(base.ConnectionString, base.ErrorLogFile);
                return DOL.Update(pActionSetting);
            }
            else
                return null;
        }

        public ActionSetting DeleteActionSetting(ActionSetting pActionSetting)
        {
            if (base.DBType == Octavo.Gate.Nabu.Entities.DatabaseType.MSSQL)
            {
                DOL.ActionSettingDOL DOL = new DOL.ActionSettingDOL(base.ConnectionString, base.ErrorLogFile);
                return DOL.Delete(pActionSetting);
            }
            else
                return null;
        }
    }
}
