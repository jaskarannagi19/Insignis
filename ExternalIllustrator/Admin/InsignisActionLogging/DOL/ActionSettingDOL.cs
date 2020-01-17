using Insignis.Asset.Management.Action.Logging.Entities;
using Octavo.Gate.Nabu.DOL.MSSQL;
using Octavo.Gate.Nabu.Entities.Error;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;

namespace Insignis.Asset.Management.Action.Logging.DOL
{
    public class ActionSettingDOL : BaseDOL
    {
        public ActionSettingDOL() : base()
        {
        }

        public ActionSettingDOL(string pConnectionString, string pErrorLogFile) : base(pConnectionString, pErrorLogFile)
        {
        }

        public ActionSetting Get(int pActionSettingID)
        {
            ActionSetting actionSetting = new ActionSetting();

            using (SqlConnection sqlConnection = new SqlConnection(base.ConnectionString))
            {
                SqlCommand sqlCommand = new SqlCommand("[ActionSetting_Get]");
                try
                {
                    sqlConnection.Open();
                    sqlCommand.Connection = sqlConnection;
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.Add(new SqlParameter("@ActionSettingID", pActionSettingID));

                    SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                    if (sqlDataReader.Read())
                    {
                        actionSetting = new ActionSetting(sqlDataReader.GetInt32(0));
                        if (sqlDataReader.IsDBNull(1) == false)
                            actionSetting.SettingGroupName = sqlDataReader.GetString(1);
                        if (sqlDataReader.IsDBNull(2) == false)
                            actionSetting.SettingName = sqlDataReader.GetString(2);
                        if (sqlDataReader.IsDBNull(3) == false)
                            actionSetting.SettingValue = sqlDataReader.GetString(3);
                    }

                    sqlDataReader.Close();
                }
                catch (Exception exc)
                {
                    actionSetting.ErrorsDetected = true;

                    MethodBase currentMethod = MethodBase.GetCurrentMethod(); base.LogError(currentMethod.DeclaringType.Name, currentMethod.Name, exc.Message);

                    actionSetting.ErrorDetails.Add(new ErrorDetail(-1, currentMethod.Name + " : " + exc.Message));
                }
                finally
                {
                    sqlCommand.Connection.Close(); sqlCommand.Connection.Dispose(); sqlCommand.Dispose();
                    sqlConnection.Close();
                    sqlConnection.Dispose();
                }
            }
            return actionSetting;
        }

        public ActionSetting[] List(int pActionLogID)
        {
            List<ActionSetting> actionSettings = new List<ActionSetting>();

            using (SqlConnection sqlConnection = new SqlConnection(base.ConnectionString))
            {
                SqlCommand sqlCommand = new SqlCommand("[ActionSetting_List]");
                try
                {
                    sqlConnection.Open();
                    sqlCommand.Connection = sqlConnection;
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.Add(new SqlParameter("@ActionLogID", pActionLogID));

                    SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                    while (sqlDataReader.Read())
                    {
                        ActionSetting actionSetting = new ActionSetting(sqlDataReader.GetInt32(0));
                        if (sqlDataReader.IsDBNull(1) == false)
                            actionSetting.SettingGroupName = sqlDataReader.GetString(1);
                        if (sqlDataReader.IsDBNull(2) == false)
                            actionSetting.SettingName = sqlDataReader.GetString(2);
                        if (sqlDataReader.IsDBNull(3) == false)
                            actionSetting.SettingValue = sqlDataReader.GetString(3);

                        actionSettings.Add(actionSetting);
                    }

                    sqlDataReader.Close();
                }
                catch (Exception exc)
                {
                    ActionSetting actionSetting = new ActionSetting();
                    actionSetting.ErrorsDetected = true;

                    MethodBase currentMethod = MethodBase.GetCurrentMethod(); base.LogError(currentMethod.DeclaringType.Name, currentMethod.Name, exc.Message);

                    actionSetting.ErrorDetails.Add(new ErrorDetail(-1, currentMethod.Name + " : " + exc.Message));
                    actionSettings.Add(actionSetting);
                }
                finally
                {
                    sqlCommand.Connection.Close(); sqlCommand.Connection.Dispose(); sqlCommand.Dispose();
                    sqlConnection.Close();
                    sqlConnection.Dispose();
                }
            }
            return actionSettings.ToArray();
        }

        public ActionSetting Insert(ActionSetting pActionSetting, int pActionLogID)
        {
            ActionSetting actionSetting = new ActionSetting();

            using (SqlConnection sqlConnection = new SqlConnection(base.ConnectionString))
            {
                SqlCommand sqlCommand = new SqlCommand("[ActionSetting_Insert]");
                try
                {
                    sqlConnection.Open();
                    sqlCommand.Connection = sqlConnection;
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.Add(new SqlParameter("@ActionLogID", pActionLogID));
                    sqlCommand.Parameters.Add(new SqlParameter("@SettingGroupName", pActionSetting.SettingGroupName));
                    sqlCommand.Parameters.Add(new SqlParameter("@SettingName", pActionSetting.SettingName));
                    sqlCommand.Parameters.Add(new SqlParameter("@SettingValue", pActionSetting.SettingValue));
                    SqlParameter actionSettingID = sqlCommand.Parameters.Add("@ActionSettingID", SqlDbType.Int);
                    actionSettingID.Direction = ParameterDirection.Output;

                    sqlCommand.ExecuteNonQuery();

                    actionSetting = new ActionSetting((Int32)actionSettingID.Value);
                    actionSetting.SettingGroupName = pActionSetting.SettingGroupName;
                    actionSetting.SettingName = pActionSetting.SettingName;
                    actionSetting.SettingValue = pActionSetting.SettingValue;
                }
                catch (Exception exc)
                {
                    actionSetting.ErrorsDetected = true;

                    MethodBase currentMethod = MethodBase.GetCurrentMethod(); base.LogError(currentMethod.DeclaringType.Name, currentMethod.Name, exc.Message);

                    actionSetting.ErrorDetails.Add(new ErrorDetail(-1, currentMethod.Name + " : " + exc.Message));
                }
                finally
                {
                    sqlCommand.Connection.Close(); sqlCommand.Connection.Dispose(); sqlCommand.Dispose();
                    sqlConnection.Close();
                    sqlConnection.Dispose();
                }
            }
            return actionSetting;
        }

        public ActionSetting Update(ActionSetting pActionSetting)
        {
            ActionSetting actionSetting = new ActionSetting();

            using (SqlConnection sqlConnection = new SqlConnection(base.ConnectionString))
            {
                SqlCommand sqlCommand = new SqlCommand("[ActionSetting_Update]");
                try
                {
                    sqlConnection.Open();
                    sqlCommand.Connection = sqlConnection;
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.Add(new SqlParameter("@ActionSettingID", pActionSetting.ActionSettingID));
                    sqlCommand.Parameters.Add(new SqlParameter("@SettingGroupName", pActionSetting.SettingGroupName));
                    sqlCommand.Parameters.Add(new SqlParameter("@SettingName", pActionSetting.SettingName));
                    sqlCommand.Parameters.Add(new SqlParameter("@SettingValue", pActionSetting.SettingValue));

                    sqlCommand.ExecuteNonQuery();

                    actionSetting = new ActionSetting(pActionSetting.ActionSettingID);
                    actionSetting.SettingGroupName = pActionSetting.SettingGroupName;
                    actionSetting.SettingName = pActionSetting.SettingName;
                    actionSetting.SettingValue = pActionSetting.SettingValue;
                }
                catch (Exception exc)
                {
                    actionSetting.ErrorsDetected = true;

                    MethodBase currentMethod = MethodBase.GetCurrentMethod(); base.LogError(currentMethod.DeclaringType.Name, currentMethod.Name, exc.Message);
                    base.LogError(currentMethod.DeclaringType.Name, currentMethod.Name, exc.ToString(), false);
                    base.LogError(currentMethod.DeclaringType.Name, currentMethod.Name, "ActionSettingID: " + pActionSetting.ActionSettingID, false);
                    base.LogError(currentMethod.DeclaringType.Name, currentMethod.Name, exc.StackTrace.ToString());
                    actionSetting.ErrorDetails.Add(new ErrorDetail(-1, currentMethod.Name + " : " + exc.Message));
                }
                finally
                {
                    sqlCommand.Connection.Close(); sqlCommand.Connection.Dispose(); sqlCommand.Dispose();
                    sqlConnection.Close();
                    sqlConnection.Dispose();
                }
            }
            return actionSetting;
        }

        public ActionSetting Delete(ActionSetting pActionSetting)
        {
            ActionSetting actionSetting = new ActionSetting();

            using (SqlConnection sqlConnection = new SqlConnection(base.ConnectionString))
            {
                SqlCommand sqlCommand = new SqlCommand("[ActionSetting_Delete]");
                try
                {
                    sqlConnection.Open();
                    sqlCommand.Connection = sqlConnection;
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.Add(new SqlParameter("@ActionSettingID", pActionSetting.ActionSettingID));

                    sqlCommand.ExecuteNonQuery();

                    actionSetting = new ActionSetting(pActionSetting.ActionSettingID);
                }
                catch (Exception exc)
                {
                    actionSetting.ErrorsDetected = true;

                    MethodBase currentMethod = MethodBase.GetCurrentMethod(); base.LogError(currentMethod.DeclaringType.Name, currentMethod.Name, exc.Message);

                    actionSetting.ErrorDetails.Add(new ErrorDetail(-1, currentMethod.Name + " : " + exc.Message));
                }
                finally
                {
                    sqlCommand.Connection.Close(); sqlCommand.Connection.Dispose(); sqlCommand.Dispose();
                    sqlConnection.Close();
                    sqlConnection.Dispose();
                }
            }
            return actionSetting;
        }
    }
}
