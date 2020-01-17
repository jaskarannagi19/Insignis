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
    public class ActionOutputValueDOL : BaseDOL
    {
        public ActionOutputValueDOL() : base()
        {
        }

        public ActionOutputValueDOL(string pConnectionString, string pErrorLogFile) : base(pConnectionString, pErrorLogFile)
        {
        }

        public ActionOutputValue Get(int pActionOutputValueID)
        {
            ActionOutputValue actionOutputValue = new ActionOutputValue();

            using (SqlConnection sqlConnection = new SqlConnection(base.ConnectionString))
            {
                SqlCommand sqlCommand = new SqlCommand("[ActionOutputValue_Get]");
                try
                {
                    sqlConnection.Open();
                    sqlCommand.Connection = sqlConnection;
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.Add(new SqlParameter("@ActionOutputValueID", pActionOutputValueID));

                    SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                    if (sqlDataReader.Read())
                    {
                        actionOutputValue = new ActionOutputValue(sqlDataReader.GetInt32(0));
                        if (sqlDataReader.IsDBNull(1) == false)
                            actionOutputValue.OutputGroupName = sqlDataReader.GetString(1);
                        if (sqlDataReader.IsDBNull(2) == false)
                            actionOutputValue.OutputName = sqlDataReader.GetString(2);
                        if (sqlDataReader.IsDBNull(3) == false)
                            actionOutputValue.OutputValue = sqlDataReader.GetString(3);
                    }

                    sqlDataReader.Close();
                }
                catch (Exception exc)
                {
                    actionOutputValue.ErrorsDetected = true;

                    MethodBase currentMethod = MethodBase.GetCurrentMethod(); base.LogError(currentMethod.DeclaringType.Name, currentMethod.Name, exc.Message);

                    actionOutputValue.ErrorDetails.Add(new ErrorDetail(-1, currentMethod.Name + " : " + exc.Message));
                }
                finally
                {
                    sqlCommand.Connection.Close(); sqlCommand.Connection.Dispose(); sqlCommand.Dispose();
                    sqlConnection.Close();
                    sqlConnection.Dispose();
                }
            }
            return actionOutputValue;
        }

        public ActionOutputValue[] List(int pActionLogID)
        {
            List<ActionOutputValue> actionOutputValues = new List<ActionOutputValue>();

            using (SqlConnection sqlConnection = new SqlConnection(base.ConnectionString))
            {
                SqlCommand sqlCommand = new SqlCommand("[ActionOutputValue_List]");
                try
                {
                    sqlConnection.Open();
                    sqlCommand.Connection = sqlConnection;
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.Add(new SqlParameter("@ActionLogID", pActionLogID));

                    SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                    while (sqlDataReader.Read())
                    {
                        ActionOutputValue actionOutputValue = new ActionOutputValue(sqlDataReader.GetInt32(0));
                        if (sqlDataReader.IsDBNull(1) == false)
                            actionOutputValue.OutputGroupName = sqlDataReader.GetString(1);
                        if (sqlDataReader.IsDBNull(2) == false)
                            actionOutputValue.OutputName = sqlDataReader.GetString(2);
                        if (sqlDataReader.IsDBNull(3) == false)
                            actionOutputValue.OutputValue = sqlDataReader.GetString(3);

                        actionOutputValues.Add(actionOutputValue);
                    }

                    sqlDataReader.Close();
                }
                catch (Exception exc)
                {
                    ActionOutputValue actionOutputValue = new ActionOutputValue();
                    actionOutputValue.ErrorsDetected = true;

                    MethodBase currentMethod = MethodBase.GetCurrentMethod(); base.LogError(currentMethod.DeclaringType.Name, currentMethod.Name, exc.Message);

                    actionOutputValue.ErrorDetails.Add(new ErrorDetail(-1, currentMethod.Name + " : " + exc.Message));
                    actionOutputValues.Add(actionOutputValue);
                }
                finally
                {
                    sqlCommand.Connection.Close(); sqlCommand.Connection.Dispose(); sqlCommand.Dispose();
                    sqlConnection.Close();
                    sqlConnection.Dispose();
                }
            }
            return actionOutputValues.ToArray();
        }

        public ActionOutputValue Insert(ActionOutputValue pActionOutputValue, int pActionLogID)
        {
            ActionOutputValue actionOutputValue = new ActionOutputValue();

            using (SqlConnection sqlConnection = new SqlConnection(base.ConnectionString))
            {
                SqlCommand sqlCommand = new SqlCommand("[ActionOutputValue_Insert]");
                try
                {
                    sqlConnection.Open();
                    sqlCommand.Connection = sqlConnection;
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.Add(new SqlParameter("@ActionLogID", pActionLogID));
                    sqlCommand.Parameters.Add(new SqlParameter("@OutputGroupName", pActionOutputValue.OutputGroupName));
                    sqlCommand.Parameters.Add(new SqlParameter("@OutputName", pActionOutputValue.OutputName));
                    sqlCommand.Parameters.Add(new SqlParameter("@OutputValue", pActionOutputValue.OutputValue));
                    SqlParameter actionOutputValueID = sqlCommand.Parameters.Add("@ActionOutputValueID", SqlDbType.Int);
                    actionOutputValueID.Direction = ParameterDirection.Output;

                    sqlCommand.ExecuteNonQuery();

                    actionOutputValue = new ActionOutputValue((Int32)actionOutputValueID.Value);
                    actionOutputValue.OutputGroupName = pActionOutputValue.OutputGroupName;
                    actionOutputValue.OutputName = pActionOutputValue.OutputName;
                    actionOutputValue.OutputValue = pActionOutputValue.OutputValue;
                }
                catch (Exception exc)
                {
                    actionOutputValue.ErrorsDetected = true;

                    MethodBase currentMethod = MethodBase.GetCurrentMethod(); base.LogError(currentMethod.DeclaringType.Name, currentMethod.Name, exc.Message);

                    actionOutputValue.ErrorDetails.Add(new ErrorDetail(-1, currentMethod.Name + " : " + exc.Message));
                }
                finally
                {
                    sqlCommand.Connection.Close(); sqlCommand.Connection.Dispose(); sqlCommand.Dispose();
                    sqlConnection.Close();
                    sqlConnection.Dispose();
                }
            }
            return actionOutputValue;
        }

        public ActionOutputValue Update(ActionOutputValue pActionOutputValue)
        {
            ActionOutputValue actionOutputValue = new ActionOutputValue();

            using (SqlConnection sqlConnection = new SqlConnection(base.ConnectionString))
            {
                SqlCommand sqlCommand = new SqlCommand("[ActionOutputValue_Update]");
                try
                {
                    sqlConnection.Open();
                    sqlCommand.Connection = sqlConnection;
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.Add(new SqlParameter("@ActionOutputValueID", pActionOutputValue.ActionOutputValueID));
                    sqlCommand.Parameters.Add(new SqlParameter("@OutputGroupName", pActionOutputValue.OutputGroupName));
                    sqlCommand.Parameters.Add(new SqlParameter("@OutputName", pActionOutputValue.OutputName));
                    sqlCommand.Parameters.Add(new SqlParameter("@OutputValue", pActionOutputValue.OutputValue));

                    sqlCommand.ExecuteNonQuery();

                    actionOutputValue = new ActionOutputValue(pActionOutputValue.ActionOutputValueID);
                    actionOutputValue.OutputGroupName = pActionOutputValue.OutputGroupName;
                    actionOutputValue.OutputName = pActionOutputValue.OutputName;
                    actionOutputValue.OutputValue = pActionOutputValue.OutputValue;
                }
                catch (Exception exc)
                {
                    actionOutputValue.ErrorsDetected = true;

                    MethodBase currentMethod = MethodBase.GetCurrentMethod(); base.LogError(currentMethod.DeclaringType.Name, currentMethod.Name, exc.Message);
                    base.LogError(currentMethod.DeclaringType.Name, currentMethod.Name, exc.ToString(), false);
                    base.LogError(currentMethod.DeclaringType.Name, currentMethod.Name, "ActionOutputValueID: " + pActionOutputValue.ActionOutputValueID, false);
                    base.LogError(currentMethod.DeclaringType.Name, currentMethod.Name, exc.StackTrace.ToString());
                    actionOutputValue.ErrorDetails.Add(new ErrorDetail(-1, currentMethod.Name + " : " + exc.Message));
                }
                finally
                {
                    sqlCommand.Connection.Close(); sqlCommand.Connection.Dispose(); sqlCommand.Dispose();
                    sqlConnection.Close();
                    sqlConnection.Dispose();
                }
            }
            return actionOutputValue;
        }

        public ActionOutputValue Delete(ActionOutputValue pActionOutputValue)
        {
            ActionOutputValue actionOutputValue = new ActionOutputValue();

            using (SqlConnection sqlConnection = new SqlConnection(base.ConnectionString))
            {
                SqlCommand sqlCommand = new SqlCommand("[ActionOutputValue_Delete]");
                try
                {
                    sqlConnection.Open();
                    sqlCommand.Connection = sqlConnection;
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.Add(new SqlParameter("@ActionOutputValueID", pActionOutputValue.ActionOutputValueID));

                    sqlCommand.ExecuteNonQuery();

                    actionOutputValue = new ActionOutputValue(pActionOutputValue.ActionOutputValueID);
                }
                catch (Exception exc)
                {
                    actionOutputValue.ErrorsDetected = true;

                    MethodBase currentMethod = MethodBase.GetCurrentMethod(); base.LogError(currentMethod.DeclaringType.Name, currentMethod.Name, exc.Message);

                    actionOutputValue.ErrorDetails.Add(new ErrorDetail(-1, currentMethod.Name + " : " + exc.Message));
                }
                finally
                {
                    sqlCommand.Connection.Close(); sqlCommand.Connection.Dispose(); sqlCommand.Dispose();
                    sqlConnection.Close();
                    sqlConnection.Dispose();
                }
            }
            return actionOutputValue;
        }
    }
}
