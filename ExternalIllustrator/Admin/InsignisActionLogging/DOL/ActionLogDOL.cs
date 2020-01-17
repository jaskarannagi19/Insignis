using Insignis.Asset.Management.Action.Logging.Entities;
using Octavo.Gate.Nabu.DOL.MSSQL;
using Octavo.Gate.Nabu.Entities;
using Octavo.Gate.Nabu.Entities.Error;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;

namespace Insignis.Asset.Management.Action.Logging.DOL
{
    public class ActionLogDOL : BaseDOL
    {
        public ActionLogDOL() : base()
        {
        }

        public ActionLogDOL(string pConnectionString, string pErrorLogFile) : base(pConnectionString, pErrorLogFile)
        {
        }

        public ActionLog Get(int pActionLogID)
        {
            ActionLog actionLog = new ActionLog();

            using (SqlConnection sqlConnection = new SqlConnection(base.ConnectionString))
            {
                SqlCommand sqlCommand = new SqlCommand("[ActionLog_Get]");
                try
                {
                    sqlConnection.Open();
                    sqlCommand.Connection = sqlConnection;
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.Add(new SqlParameter("@ActionLogID", pActionLogID));

                    SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                    if (sqlDataReader.Read())
                    {
                        actionLog = new ActionLog(sqlDataReader.GetInt32(0));
                        if (sqlDataReader.IsDBNull(1) == false)
                            actionLog.Reference = sqlDataReader.GetString(1);
                        if (sqlDataReader.IsDBNull(2) == false)
                            actionLog.System = sqlDataReader.GetString(2);
                        if (sqlDataReader.IsDBNull(3) == false)
                            actionLog.Module = sqlDataReader.GetString(3);
                        if (sqlDataReader.IsDBNull(4) == false)
                            actionLog.Method = sqlDataReader.GetString(4);
                        if (sqlDataReader.IsDBNull(5) == false)
                            actionLog.Action = sqlDataReader.GetString(5);
                        if (sqlDataReader.IsDBNull(6) == false)
                            actionLog.ActionInitiatedByOrganisationName = sqlDataReader.GetString(6);
                        if (sqlDataReader.IsDBNull(7) == false)
                            actionLog.ActionInitiatedByContactName = sqlDataReader.GetString(7);
                        if (sqlDataReader.IsDBNull(8) == false)
                            actionLog.ActionInitiatedByContactEmail = sqlDataReader.GetString(8);
                        if (sqlDataReader.IsDBNull(9) == false)
                            actionLog.ActionInitatedOn = sqlDataReader.GetInt32(9);
                        if (sqlDataReader.IsDBNull(10) == false)
                            actionLog.ActionInitiatedAt = sqlDataReader.GetInt32(10);
                        if (sqlDataReader.IsDBNull(11) == false)
                            actionLog.ActionStatus = sqlDataReader.GetString(11);
                        if (sqlDataReader.IsDBNull(12) == false)
                            actionLog.ActionCompletedOn = sqlDataReader.GetInt32(12);
                        if (sqlDataReader.IsDBNull(13) == false)
                            actionLog.ActionCompletedAt = sqlDataReader.GetInt32(13);
                    }

                    sqlDataReader.Close();
                }
                catch (Exception exc)
                {
                    actionLog.ErrorsDetected = true;

                    MethodBase currentMethod = MethodBase.GetCurrentMethod(); base.LogError(currentMethod.DeclaringType.Name, currentMethod.Name, exc.Message);

                    actionLog.ErrorDetails.Add(new ErrorDetail(-1, currentMethod.Name + " : " + exc.Message));
                }
                finally
                {
                    sqlCommand.Connection.Close(); sqlCommand.Connection.Dispose(); sqlCommand.Dispose();
                    sqlConnection.Close();
                    sqlConnection.Dispose();
                }
            }
            return actionLog;
        }

        public ActionLog GetByReference(string pReference)
        {
            ActionLog actionLog = new ActionLog();

            using (SqlConnection sqlConnection = new SqlConnection(base.ConnectionString))
            {
                SqlCommand sqlCommand = new SqlCommand("[ActionLog_GetByReference]");
                try
                {
                    sqlConnection.Open();
                    sqlCommand.Connection = sqlConnection;
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.Add(new SqlParameter("@Reference", pReference));

                    SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                    if (sqlDataReader.Read())
                    {
                        actionLog = new ActionLog(sqlDataReader.GetInt32(0));
                        if (sqlDataReader.IsDBNull(1) == false)
                            actionLog.Reference = sqlDataReader.GetString(1);
                        if (sqlDataReader.IsDBNull(2) == false)
                            actionLog.System = sqlDataReader.GetString(2);
                        if (sqlDataReader.IsDBNull(3) == false)
                            actionLog.Module = sqlDataReader.GetString(3);
                        if (sqlDataReader.IsDBNull(4) == false)
                            actionLog.Method = sqlDataReader.GetString(4);
                        if (sqlDataReader.IsDBNull(5) == false)
                            actionLog.Action = sqlDataReader.GetString(5);
                        if (sqlDataReader.IsDBNull(6) == false)
                            actionLog.ActionInitiatedByOrganisationName = sqlDataReader.GetString(6);
                        if (sqlDataReader.IsDBNull(7) == false)
                            actionLog.ActionInitiatedByContactName = sqlDataReader.GetString(7);
                        if (sqlDataReader.IsDBNull(8) == false)
                            actionLog.ActionInitiatedByContactEmail = sqlDataReader.GetString(8);
                        if (sqlDataReader.IsDBNull(9) == false)
                            actionLog.ActionInitatedOn = sqlDataReader.GetInt32(9);
                        if (sqlDataReader.IsDBNull(10) == false)
                            actionLog.ActionInitiatedAt = sqlDataReader.GetInt32(10);
                        if (sqlDataReader.IsDBNull(11) == false)
                            actionLog.ActionStatus = sqlDataReader.GetString(11);
                        if (sqlDataReader.IsDBNull(12) == false)
                            actionLog.ActionCompletedOn = sqlDataReader.GetInt32(12);
                        if (sqlDataReader.IsDBNull(13) == false)
                            actionLog.ActionCompletedAt = sqlDataReader.GetInt32(13);
                    }

                    sqlDataReader.Close();
                }
                catch (Exception exc)
                {
                    actionLog.ErrorsDetected = true;

                    MethodBase currentMethod = MethodBase.GetCurrentMethod(); base.LogError(currentMethod.DeclaringType.Name, currentMethod.Name, exc.Message);

                    actionLog.ErrorDetails.Add(new ErrorDetail(-1, currentMethod.Name + " : " + exc.Message));
                }
                finally
                {
                    sqlCommand.Connection.Close(); sqlCommand.Connection.Dispose(); sqlCommand.Dispose();
                    sqlConnection.Close();
                    sqlConnection.Dispose();
                }
            }
            return actionLog;
        }

        public ActionLog[] List(string pSystem, string pModule, string pMethod, string pAction)
        {
            List<ActionLog> actionLogs = new List<ActionLog>();

            using (SqlConnection sqlConnection = new SqlConnection(base.ConnectionString))
            {
                SqlCommand sqlCommand = new SqlCommand("[ActionLog_List]");
                try
                {
                    sqlConnection.Open();
                    sqlCommand.Connection = sqlConnection;
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.Add(new SqlParameter("@System", pSystem));
                    sqlCommand.Parameters.Add(new SqlParameter("@Module", pModule));
                    sqlCommand.Parameters.Add(new SqlParameter("@Method", pMethod));
                    sqlCommand.Parameters.Add(new SqlParameter("@Action", pAction));

                    SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                    while (sqlDataReader.Read())
                    {
                        ActionLog actionLog = new ActionLog(sqlDataReader.GetInt32(0));
                        if (sqlDataReader.IsDBNull(1) == false)
                            actionLog.Reference = sqlDataReader.GetString(1);
                        if (sqlDataReader.IsDBNull(2) == false)
                            actionLog.System = sqlDataReader.GetString(2);
                        if (sqlDataReader.IsDBNull(3) == false)
                            actionLog.Module = sqlDataReader.GetString(3);
                        if (sqlDataReader.IsDBNull(4) == false)
                            actionLog.Method = sqlDataReader.GetString(4);
                        if (sqlDataReader.IsDBNull(5) == false)
                            actionLog.Action = sqlDataReader.GetString(5);
                        if (sqlDataReader.IsDBNull(6) == false)
                            actionLog.ActionInitiatedByOrganisationName = sqlDataReader.GetString(6);
                        if (sqlDataReader.IsDBNull(7) == false)
                            actionLog.ActionInitiatedByContactName = sqlDataReader.GetString(7);
                        if (sqlDataReader.IsDBNull(8) == false)
                            actionLog.ActionInitiatedByContactEmail = sqlDataReader.GetString(8);
                        if (sqlDataReader.IsDBNull(9) == false)
                            actionLog.ActionInitatedOn = sqlDataReader.GetInt32(9);
                        if (sqlDataReader.IsDBNull(10) == false)
                            actionLog.ActionInitiatedAt = sqlDataReader.GetInt32(10);
                        if (sqlDataReader.IsDBNull(11) == false)
                            actionLog.ActionStatus = sqlDataReader.GetString(11);
                        if (sqlDataReader.IsDBNull(12) == false)
                            actionLog.ActionCompletedOn = sqlDataReader.GetInt32(12);
                        if (sqlDataReader.IsDBNull(13) == false)
                            actionLog.ActionCompletedAt = sqlDataReader.GetInt32(13);

                        actionLogs.Add(actionLog);
                    }

                    sqlDataReader.Close();
                }
                catch (Exception exc)
                {
                    ActionLog actionLog = new ActionLog();
                    actionLog.ErrorsDetected = true;

                    MethodBase currentMethod = MethodBase.GetCurrentMethod(); base.LogError(currentMethod.DeclaringType.Name, currentMethod.Name, exc.Message);

                    actionLog.ErrorDetails.Add(new ErrorDetail(-1, currentMethod.Name + " : " + exc.Message));
                    actionLogs.Add(actionLog);
                }
                finally
                {
                    sqlCommand.Connection.Close(); sqlCommand.Connection.Dispose(); sqlCommand.Dispose();
                    sqlConnection.Close();
                    sqlConnection.Dispose();
                }
            }
            return actionLogs.ToArray();
        }

        public BaseString[] ListSystems()
        {
            List<BaseString> systems = new List<BaseString>();

            using (SqlConnection sqlConnection = new SqlConnection(base.ConnectionString))
            {
                SqlCommand sqlCommand = new SqlCommand("[ActionLog_ListSystems]");
                try
                {
                    sqlConnection.Open();
                    sqlCommand.Connection = sqlConnection;
                    sqlCommand.CommandType = CommandType.StoredProcedure;

                    SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                    while (sqlDataReader.Read())
                    {
                        BaseString system = new BaseString(sqlDataReader.GetString(0));
                        systems.Add(system);
                    }

                    sqlDataReader.Close();
                }
                catch (Exception exc)
                {
                    BaseString error = new BaseString();
                    error.ErrorsDetected = true;

                    MethodBase currentMethod = MethodBase.GetCurrentMethod(); base.LogError(currentMethod.DeclaringType.Name, currentMethod.Name, exc.Message);

                    error.ErrorDetails.Add(new ErrorDetail(-1, currentMethod.Name + " : " + exc.Message));
                    error.StackTrace = exc.StackTrace;
                    systems.Add(error);
                }
                finally
                {
                    sqlCommand.Connection.Close(); sqlCommand.Connection.Dispose(); sqlCommand.Dispose();
                    sqlConnection.Close();
                    sqlConnection.Dispose();
                }
            }
            return systems.ToArray();
        }

        public BaseString[] ListModules(string pSystem)
        {
            List<BaseString> modules = new List<BaseString>();

            using (SqlConnection sqlConnection = new SqlConnection(base.ConnectionString))
            {
                SqlCommand sqlCommand = new SqlCommand("[ActionLog_ListModules]");
                try
                {
                    sqlConnection.Open();
                    sqlCommand.Connection = sqlConnection;
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.Add(new SqlParameter("@System", pSystem));

                    SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                    while (sqlDataReader.Read())
                    {
                        BaseString module = new BaseString(sqlDataReader.GetString(0));
                        modules.Add(module);
                    }

                    sqlDataReader.Close();
                }
                catch (Exception exc)
                {
                    BaseString error = new BaseString();
                    error.ErrorsDetected = true;

                    MethodBase currentMethod = MethodBase.GetCurrentMethod(); base.LogError(currentMethod.DeclaringType.Name, currentMethod.Name, exc.Message);

                    error.ErrorDetails.Add(new ErrorDetail(-1, currentMethod.Name + " : " + exc.Message));
                    error.StackTrace = exc.StackTrace;
                    modules.Add(error);
                }
                finally
                {
                    sqlCommand.Connection.Close(); sqlCommand.Connection.Dispose(); sqlCommand.Dispose();
                    sqlConnection.Close();
                    sqlConnection.Dispose();
                }
            }
            return modules.ToArray();
        }

        public BaseString[] ListMethods(string pSystem, string pModule)
        {
            List<BaseString> methods = new List<BaseString>();

            using (SqlConnection sqlConnection = new SqlConnection(base.ConnectionString))
            {
                SqlCommand sqlCommand = new SqlCommand("[ActionLog_ListMethods]");
                try
                {
                    sqlConnection.Open();
                    sqlCommand.Connection = sqlConnection;
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.Add(new SqlParameter("@System", pSystem));
                    sqlCommand.Parameters.Add(new SqlParameter("@Module", pModule));

                    SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                    while (sqlDataReader.Read())
                    {
                        BaseString method = new BaseString(sqlDataReader.GetString(0));
                        methods.Add(method);
                    }

                    sqlDataReader.Close();
                }
                catch (Exception exc)
                {
                    BaseString error = new BaseString();
                    error.ErrorsDetected = true;

                    MethodBase currentMethod = MethodBase.GetCurrentMethod(); base.LogError(currentMethod.DeclaringType.Name, currentMethod.Name, exc.Message);

                    error.ErrorDetails.Add(new ErrorDetail(-1, currentMethod.Name + " : " + exc.Message));
                    error.StackTrace = exc.StackTrace;
                    methods.Add(error);
                }
                finally
                {
                    sqlCommand.Connection.Close(); sqlCommand.Connection.Dispose(); sqlCommand.Dispose();
                    sqlConnection.Close();
                    sqlConnection.Dispose();
                }
            }
            return methods.ToArray();
        }

        public BaseString[] ListActions(string pSystem, string pModule, string pMethod)
        {
            List<BaseString> actions = new List<BaseString>();

            using (SqlConnection sqlConnection = new SqlConnection(base.ConnectionString))
            {
                SqlCommand sqlCommand = new SqlCommand("[ActionLog_ListActions]");
                try
                {
                    sqlConnection.Open();
                    sqlCommand.Connection = sqlConnection;
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.Add(new SqlParameter("@System", pSystem));
                    sqlCommand.Parameters.Add(new SqlParameter("@Module", pModule));
                    sqlCommand.Parameters.Add(new SqlParameter("@Method", pMethod));

                    SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                    while (sqlDataReader.Read())
                    {
                        BaseString action = new BaseString(sqlDataReader.GetString(0));
                        actions.Add(action);
                    }

                    sqlDataReader.Close();
                }
                catch (Exception exc)
                {
                    BaseString error = new BaseString();
                    error.ErrorsDetected = true;

                    MethodBase currentMethod = MethodBase.GetCurrentMethod(); base.LogError(currentMethod.DeclaringType.Name, currentMethod.Name, exc.Message);

                    error.ErrorDetails.Add(new ErrorDetail(-1, currentMethod.Name + " : " + exc.Message));
                    error.StackTrace = exc.StackTrace;
                    actions.Add(error);
                }
                finally
                {
                    sqlCommand.Connection.Close(); sqlCommand.Connection.Dispose(); sqlCommand.Dispose();
                    sqlConnection.Close();
                    sqlConnection.Dispose();
                }
            }
            return actions.ToArray();
        }

        public ActionLog Insert(ActionLog pActionLog)
        {
            ActionLog actionLog = new ActionLog();

            using (SqlConnection sqlConnection = new SqlConnection(base.ConnectionString))
            {
                SqlCommand sqlCommand = new SqlCommand("[ActionLog_Insert]");
                try
                {
                    sqlConnection.Open();
                    sqlCommand.Connection = sqlConnection;
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.Add(new SqlParameter("@Reference", pActionLog.Reference));
                    sqlCommand.Parameters.Add(new SqlParameter("@System", pActionLog.System));
                    sqlCommand.Parameters.Add(new SqlParameter("@Module", pActionLog.Module));
                    sqlCommand.Parameters.Add(new SqlParameter("@Method", pActionLog.Method));
                    sqlCommand.Parameters.Add(new SqlParameter("@Action", pActionLog.Action));
                    sqlCommand.Parameters.Add(new SqlParameter("@ActionInitiatedByOrganisationName", pActionLog.ActionInitiatedByOrganisationName));
                    sqlCommand.Parameters.Add(new SqlParameter("@ActionInitiatedByContactName", pActionLog.ActionInitiatedByContactName));
                    sqlCommand.Parameters.Add(new SqlParameter("@ActionInitiatedByContactEmail", pActionLog.ActionInitiatedByContactEmail));
                    sqlCommand.Parameters.Add(new SqlParameter("@ActionInitatedOn", pActionLog.ActionInitatedOn));
                    sqlCommand.Parameters.Add(new SqlParameter("@ActionInitiatedAt", pActionLog.ActionInitiatedAt));
                    sqlCommand.Parameters.Add(new SqlParameter("@ActionStatus", pActionLog.ActionStatus));
                    sqlCommand.Parameters.Add(new SqlParameter("@ActionCompletedOn", pActionLog.ActionCompletedOn));
                    sqlCommand.Parameters.Add(new SqlParameter("@ActionCompletedAt", pActionLog.ActionCompletedAt));
                    SqlParameter actionLogID = sqlCommand.Parameters.Add("@ActionLogID", SqlDbType.Int);
                    actionLogID.Direction = ParameterDirection.Output;

                    sqlCommand.ExecuteNonQuery();

                    actionLog = new ActionLog((Int32)actionLogID.Value);
                    actionLog.Reference = pActionLog.Reference;
                    actionLog.System = pActionLog.System;
                    actionLog.Module = pActionLog.Module;
                    actionLog.Method = pActionLog.Method;
                    actionLog.Action = pActionLog.Action;
                    actionLog.ActionInitiatedByOrganisationName = pActionLog.ActionInitiatedByOrganisationName;
                    actionLog.ActionInitiatedByContactName = pActionLog.ActionInitiatedByContactName;
                    actionLog.ActionInitiatedByContactEmail = pActionLog.ActionInitiatedByContactEmail;
                    actionLog.ActionInitatedOn = pActionLog.ActionInitatedOn;
                    actionLog.ActionInitiatedAt = pActionLog.ActionInitiatedAt;
                    actionLog.ActionStatus = pActionLog.ActionStatus;
                    actionLog.ActionCompletedOn = pActionLog.ActionCompletedOn;
                    actionLog.ActionCompletedAt = pActionLog.ActionInitiatedAt;

                    actionLog.Settings = pActionLog.Settings;
                    actionLog.Values = pActionLog.Values;
                    actionLog.Tables = pActionLog.Tables;
                    actionLog.Documents = pActionLog.Documents;
                }
                catch (Exception exc)
                {
                    actionLog.ErrorsDetected = true;

                    MethodBase currentMethod = MethodBase.GetCurrentMethod(); base.LogError(currentMethod.DeclaringType.Name, currentMethod.Name, exc.Message);

                    actionLog.ErrorDetails.Add(new ErrorDetail(-1, currentMethod.Name + " : " + exc.Message));
                }
                finally
                {
                    sqlCommand.Connection.Close(); sqlCommand.Connection.Dispose(); sqlCommand.Dispose();
                    sqlConnection.Close();
                    sqlConnection.Dispose();
                }
            }
            return actionLog;
        }

        public ActionLog Update(ActionLog pActionLog)
        {
            ActionLog actionLog = new ActionLog();

            using (SqlConnection sqlConnection = new SqlConnection(base.ConnectionString))
            {
                SqlCommand sqlCommand = new SqlCommand("[ActionLog_Update]");
                try
                {
                    sqlConnection.Open();
                    sqlCommand.Connection = sqlConnection;
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.Add(new SqlParameter("@ActionLogID", pActionLog.ActionLogID));
                    sqlCommand.Parameters.Add(new SqlParameter("@Reference", pActionLog.Reference));
                    sqlCommand.Parameters.Add(new SqlParameter("@System", pActionLog.System));
                    sqlCommand.Parameters.Add(new SqlParameter("@Module", pActionLog.Module));
                    sqlCommand.Parameters.Add(new SqlParameter("@Method", pActionLog.Method));
                    sqlCommand.Parameters.Add(new SqlParameter("@Action", pActionLog.Action));
                    sqlCommand.Parameters.Add(new SqlParameter("@ActionInitiatedByOrganisationName", pActionLog.ActionInitiatedByOrganisationName));
                    sqlCommand.Parameters.Add(new SqlParameter("@ActionInitiatedByContactName", pActionLog.ActionInitiatedByContactName));
                    sqlCommand.Parameters.Add(new SqlParameter("@ActionInitiatedByContactEmail", pActionLog.ActionInitiatedByContactEmail));
                    sqlCommand.Parameters.Add(new SqlParameter("@ActionInitatedOn", pActionLog.ActionInitatedOn));
                    sqlCommand.Parameters.Add(new SqlParameter("@ActionInitiatedAt", pActionLog.ActionInitiatedAt));
                    sqlCommand.Parameters.Add(new SqlParameter("@ActionStatus", pActionLog.ActionStatus));
                    sqlCommand.Parameters.Add(new SqlParameter("@ActionCompletedOn", pActionLog.ActionCompletedOn));
                    sqlCommand.Parameters.Add(new SqlParameter("@ActionCompletedAt", pActionLog.ActionCompletedAt));

                    sqlCommand.ExecuteNonQuery();

                    actionLog = new ActionLog(pActionLog.ActionLogID);
                    actionLog.Reference = pActionLog.Reference;
                    actionLog.System = pActionLog.System;
                    actionLog.Module = pActionLog.Module;
                    actionLog.Method = pActionLog.Method;
                    actionLog.Action = pActionLog.Action;
                    actionLog.ActionInitiatedByOrganisationName = pActionLog.ActionInitiatedByOrganisationName;
                    actionLog.ActionInitiatedByContactName = pActionLog.ActionInitiatedByContactName;
                    actionLog.ActionInitiatedByContactEmail = pActionLog.ActionInitiatedByContactEmail;
                    actionLog.ActionInitatedOn = pActionLog.ActionInitatedOn;
                    actionLog.ActionInitiatedAt = pActionLog.ActionInitiatedAt;
                    actionLog.ActionStatus = pActionLog.ActionStatus;
                    actionLog.ActionCompletedOn = pActionLog.ActionCompletedOn;
                    actionLog.ActionCompletedAt = pActionLog.ActionInitiatedAt;

                    actionLog.Settings = pActionLog.Settings;
                    actionLog.Values = pActionLog.Values;
                    actionLog.Tables = pActionLog.Tables;
                    actionLog.Documents = pActionLog.Documents;
                }
                catch (Exception exc)
                {
                    actionLog.ErrorsDetected = true;

                    MethodBase currentMethod = MethodBase.GetCurrentMethod(); base.LogError(currentMethod.DeclaringType.Name, currentMethod.Name, exc.Message);
                    base.LogError(currentMethod.DeclaringType.Name, currentMethod.Name, exc.ToString(), false);
                    base.LogError(currentMethod.DeclaringType.Name, currentMethod.Name, "ActionLogID: " + pActionLog.ActionLogID, false);
                    base.LogError(currentMethod.DeclaringType.Name, currentMethod.Name, exc.StackTrace.ToString());
                    actionLog.ErrorDetails.Add(new ErrorDetail(-1, currentMethod.Name + " : " + exc.Message));
                }
                finally
                {
                    sqlCommand.Connection.Close(); sqlCommand.Connection.Dispose(); sqlCommand.Dispose();
                    sqlConnection.Close();
                    sqlConnection.Dispose();
                }
            }
            return actionLog;
        }

        public ActionLog Delete(ActionLog pActionLog)
        {
            ActionLog actionLog = new ActionLog();

            using (SqlConnection sqlConnection = new SqlConnection(base.ConnectionString))
            {
                SqlCommand sqlCommand = new SqlCommand("[ActionLog_Delete]");
                try
                {
                    sqlConnection.Open();
                    sqlCommand.Connection = sqlConnection;
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.Add(new SqlParameter("@ActionLogID", pActionLog.ActionLogID));

                    sqlCommand.ExecuteNonQuery();

                    actionLog = new ActionLog(pActionLog.ActionLogID);
                }
                catch (Exception exc)
                {
                    actionLog.ErrorsDetected = true;

                    MethodBase currentMethod = MethodBase.GetCurrentMethod(); base.LogError(currentMethod.DeclaringType.Name, currentMethod.Name, exc.Message);

                    actionLog.ErrorDetails.Add(new ErrorDetail(-1, currentMethod.Name + " : " + exc.Message));
                }
                finally
                {
                    sqlCommand.Connection.Close(); sqlCommand.Connection.Dispose(); sqlCommand.Dispose();
                    sqlConnection.Close();
                    sqlConnection.Dispose();
                }
            }
            return actionLog;
        }
    }
}
