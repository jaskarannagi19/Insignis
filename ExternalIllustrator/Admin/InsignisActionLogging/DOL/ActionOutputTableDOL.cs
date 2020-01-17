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
    public class ActionOutputTableDOL : BaseDOL
    {
        public ActionOutputTableDOL() : base()
        {
        }

        public ActionOutputTableDOL(string pConnectionString, string pErrorLogFile) : base(pConnectionString, pErrorLogFile)
        {
        }

        public ActionOutputTable Get(int pActionOutputTableID)
        {
            ActionOutputTable actionOutputTable = new ActionOutputTable();

            using (SqlConnection sqlConnection = new SqlConnection(base.ConnectionString))
            {
                SqlCommand sqlCommand = new SqlCommand("[ActionOutputTable_Get]");
                try
                {
                    sqlConnection.Open();
                    sqlCommand.Connection = sqlConnection;
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.Add(new SqlParameter("@ActionOutputTableID", pActionOutputTableID));

                    SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                    if (sqlDataReader.Read())
                    {
                        actionOutputTable = new ActionOutputTable(sqlDataReader.GetInt32(0));
                        if (sqlDataReader.IsDBNull(1) == false)
                            actionOutputTable.TableName = sqlDataReader.GetString(1);
                        if (sqlDataReader.IsDBNull(2) == false)
                            actionOutputTable.RowIndex = sqlDataReader.GetInt32(2);
                        if (sqlDataReader.IsDBNull(3) == false)
                            actionOutputTable.ColumnIndex = sqlDataReader.GetInt32(3);
                        if (sqlDataReader.IsDBNull(4) == false)
                            actionOutputTable.CellValue = sqlDataReader.GetString(4);
                    }

                    sqlDataReader.Close();
                }
                catch (Exception exc)
                {
                    actionOutputTable.ErrorsDetected = true;

                    MethodBase currentMethod = MethodBase.GetCurrentMethod(); base.LogError(currentMethod.DeclaringType.Name, currentMethod.Name, exc.Message);

                    actionOutputTable.ErrorDetails.Add(new ErrorDetail(-1, currentMethod.Name + " : " + exc.Message));
                }
                finally
                {
                    sqlCommand.Connection.Close(); sqlCommand.Connection.Dispose(); sqlCommand.Dispose();
                    sqlConnection.Close();
                    sqlConnection.Dispose();
                }
            }
            return actionOutputTable;
        }

        public ActionOutputTable[] List(int pActionLogID)
        {
            List<ActionOutputTable> actionOutputTables = new List<ActionOutputTable>();

            using (SqlConnection sqlConnection = new SqlConnection(base.ConnectionString))
            {
                SqlCommand sqlCommand = new SqlCommand("[ActionOutputTable_List]");
                try
                {
                    sqlConnection.Open();
                    sqlCommand.Connection = sqlConnection;
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.Add(new SqlParameter("@ActionLogID", pActionLogID));

                    SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                    while (sqlDataReader.Read())
                    {
                        ActionOutputTable actionOutputTable = new ActionOutputTable(sqlDataReader.GetInt32(0));
                        if (sqlDataReader.IsDBNull(1) == false)
                            actionOutputTable.TableName = sqlDataReader.GetString(1);
                        if (sqlDataReader.IsDBNull(2) == false)
                            actionOutputTable.RowIndex = sqlDataReader.GetInt32(2);
                        if (sqlDataReader.IsDBNull(3) == false)
                            actionOutputTable.ColumnIndex = sqlDataReader.GetInt32(3);
                        if (sqlDataReader.IsDBNull(4) == false)
                            actionOutputTable.CellValue = sqlDataReader.GetString(4);

                        actionOutputTables.Add(actionOutputTable);
                    }

                    sqlDataReader.Close();
                }
                catch (Exception exc)
                {
                    ActionOutputTable actionOutputTable = new ActionOutputTable();
                    actionOutputTable.ErrorsDetected = true;

                    MethodBase currentMethod = MethodBase.GetCurrentMethod(); base.LogError(currentMethod.DeclaringType.Name, currentMethod.Name, exc.Message);

                    actionOutputTable.ErrorDetails.Add(new ErrorDetail(-1, currentMethod.Name + " : " + exc.Message));
                    actionOutputTables.Add(actionOutputTable);
                }
                finally
                {
                    sqlCommand.Connection.Close(); sqlCommand.Connection.Dispose(); sqlCommand.Dispose();
                    sqlConnection.Close();
                    sqlConnection.Dispose();
                }
            }
            return actionOutputTables.ToArray();
        }

        public ActionOutputTable Insert(ActionOutputTable pActionOutputTable, int pActionLogID)
        {
            ActionOutputTable actionOutputTable = new ActionOutputTable();

            using (SqlConnection sqlConnection = new SqlConnection(base.ConnectionString))
            {
                SqlCommand sqlCommand = new SqlCommand("[ActionOutputTable_Insert]");
                try
                {
                    sqlConnection.Open();
                    sqlCommand.Connection = sqlConnection;
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.Add(new SqlParameter("@ActionLogID", pActionLogID));
                    sqlCommand.Parameters.Add(new SqlParameter("@TableName", pActionOutputTable.TableName));
                    sqlCommand.Parameters.Add(new SqlParameter("@RowIndex", pActionOutputTable.RowIndex));
                    sqlCommand.Parameters.Add(new SqlParameter("@ColumnIndex", pActionOutputTable.ColumnIndex));
                    sqlCommand.Parameters.Add(new SqlParameter("@CellValue", pActionOutputTable.CellValue));
                    SqlParameter actionOutputTableID = sqlCommand.Parameters.Add("@ActionOutputTableID", SqlDbType.Int);
                    actionOutputTableID.Direction = ParameterDirection.Output;

                    sqlCommand.ExecuteNonQuery();

                    actionOutputTable = new ActionOutputTable((Int32)actionOutputTableID.Value);
                    actionOutputTable.TableName = pActionOutputTable.TableName;
                    actionOutputTable.RowIndex = pActionOutputTable.RowIndex;
                    actionOutputTable.ColumnIndex = pActionOutputTable.ColumnIndex;
                    actionOutputTable.CellValue = pActionOutputTable.CellValue;
                }
                catch (Exception exc)
                {
                    actionOutputTable.ErrorsDetected = true;

                    MethodBase currentMethod = MethodBase.GetCurrentMethod(); base.LogError(currentMethod.DeclaringType.Name, currentMethod.Name, exc.Message);

                    actionOutputTable.ErrorDetails.Add(new ErrorDetail(-1, currentMethod.Name + " : " + exc.Message));
                }
                finally
                {
                    sqlCommand.Connection.Close(); sqlCommand.Connection.Dispose(); sqlCommand.Dispose();
                    sqlConnection.Close();
                    sqlConnection.Dispose();
                }
            }
            return actionOutputTable;
        }

        public ActionOutputTable Update(ActionOutputTable pActionOutputTable)
        {
            ActionOutputTable actionOutputTable = new ActionOutputTable();

            using (SqlConnection sqlConnection = new SqlConnection(base.ConnectionString))
            {
                SqlCommand sqlCommand = new SqlCommand("[ActionOutputTable_Update]");
                try
                {
                    sqlConnection.Open();
                    sqlCommand.Connection = sqlConnection;
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.Add(new SqlParameter("@ActionOutputTableID", pActionOutputTable.ActionOutputTableID));
                    sqlCommand.Parameters.Add(new SqlParameter("@TableName", pActionOutputTable.TableName));
                    sqlCommand.Parameters.Add(new SqlParameter("@RowIndex", pActionOutputTable.RowIndex));
                    sqlCommand.Parameters.Add(new SqlParameter("@ColumnIndex", pActionOutputTable.ColumnIndex));
                    sqlCommand.Parameters.Add(new SqlParameter("@CellValue", pActionOutputTable.CellValue));

                    sqlCommand.ExecuteNonQuery();

                    actionOutputTable = new ActionOutputTable(pActionOutputTable.ActionOutputTableID);
                    actionOutputTable.TableName = pActionOutputTable.TableName;
                    actionOutputTable.RowIndex = pActionOutputTable.RowIndex;
                    actionOutputTable.ColumnIndex = pActionOutputTable.ColumnIndex;
                    actionOutputTable.CellValue = pActionOutputTable.CellValue;
                }
                catch (Exception exc)
                {
                    actionOutputTable.ErrorsDetected = true;

                    MethodBase currentMethod = MethodBase.GetCurrentMethod(); base.LogError(currentMethod.DeclaringType.Name, currentMethod.Name, exc.Message);
                    base.LogError(currentMethod.DeclaringType.Name, currentMethod.Name, exc.ToString(), false);
                    base.LogError(currentMethod.DeclaringType.Name, currentMethod.Name, "ActionOutputTableID: " + pActionOutputTable.ActionOutputTableID, false);
                    base.LogError(currentMethod.DeclaringType.Name, currentMethod.Name, exc.StackTrace.ToString());
                    actionOutputTable.ErrorDetails.Add(new ErrorDetail(-1, currentMethod.Name + " : " + exc.Message));
                }
                finally
                {
                    sqlCommand.Connection.Close(); sqlCommand.Connection.Dispose(); sqlCommand.Dispose();
                    sqlConnection.Close();
                    sqlConnection.Dispose();
                }
            }
            return actionOutputTable;
        }

        public ActionOutputTable Delete(ActionOutputTable pActionOutputTable)
        {
            ActionOutputTable actionOutputTable = new ActionOutputTable();

            using (SqlConnection sqlConnection = new SqlConnection(base.ConnectionString))
            {
                SqlCommand sqlCommand = new SqlCommand("[ActionOutputTable_Delete]");
                try
                {
                    sqlConnection.Open();
                    sqlCommand.Connection = sqlConnection;
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.Add(new SqlParameter("@ActionOutputTableID", pActionOutputTable.ActionOutputTableID));

                    sqlCommand.ExecuteNonQuery();

                    actionOutputTable = new ActionOutputTable(pActionOutputTable.ActionOutputTableID);
                }
                catch (Exception exc)
                {
                    actionOutputTable.ErrorsDetected = true;

                    MethodBase currentMethod = MethodBase.GetCurrentMethod(); base.LogError(currentMethod.DeclaringType.Name, currentMethod.Name, exc.Message);

                    actionOutputTable.ErrorDetails.Add(new ErrorDetail(-1, currentMethod.Name + " : " + exc.Message));
                }
                finally
                {
                    sqlCommand.Connection.Close(); sqlCommand.Connection.Dispose(); sqlCommand.Dispose();
                    sqlConnection.Close();
                    sqlConnection.Dispose();
                }
            }
            return actionOutputTable;
        }
    }
}
