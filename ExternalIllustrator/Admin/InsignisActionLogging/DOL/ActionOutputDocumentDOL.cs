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
    public class ActionOutputDocumentDOL : BaseDOL
    {
        public ActionOutputDocumentDOL() : base()
        {
        }

        public ActionOutputDocumentDOL(string pConnectionString, string pErrorLogFile) : base(pConnectionString, pErrorLogFile)
        {
        }

        public ActionOutputDocument Get(int pActionOutputDocumentID)
        {
            ActionOutputDocument actionOutputDocument = new ActionOutputDocument();

            using (SqlConnection sqlConnection = new SqlConnection(base.ConnectionString))
            {
                SqlCommand sqlCommand = new SqlCommand("[ActionOutputDocument_Get]");
                try
                {
                    sqlConnection.Open();
                    sqlCommand.Connection = sqlConnection;
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.Add(new SqlParameter("@ActionOutputDocumentID", pActionOutputDocumentID));

                    SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                    if (sqlDataReader.Read())
                    {
                        actionOutputDocument = new ActionOutputDocument(sqlDataReader.GetInt32(0));
                        if (sqlDataReader.IsDBNull(1) == false)
                            actionOutputDocument.DocumentType = sqlDataReader.GetString(1);
                        if (sqlDataReader.IsDBNull(2) == false)
                            actionOutputDocument.PublicFacingFolder = sqlDataReader.GetString(2);
                        if (sqlDataReader.IsDBNull(3) == false)
                            actionOutputDocument.InternalFacingFolder = sqlDataReader.GetString(3);
                        if (sqlDataReader.IsDBNull(4) == false)
                            actionOutputDocument.Filename = sqlDataReader.GetString(4);
                    }

                    sqlDataReader.Close();
                }
                catch (Exception exc)
                {
                    actionOutputDocument.ErrorsDetected = true;

                    MethodBase currentMethod = MethodBase.GetCurrentMethod(); base.LogError(currentMethod.DeclaringType.Name, currentMethod.Name, exc.Message);

                    actionOutputDocument.ErrorDetails.Add(new ErrorDetail(-1, currentMethod.Name + " : " + exc.Message));
                }
                finally
                {
                    sqlCommand.Connection.Close(); sqlCommand.Connection.Dispose(); sqlCommand.Dispose();
                    sqlConnection.Close();
                    sqlConnection.Dispose();
                }
            }
            return actionOutputDocument;
        }

        public ActionOutputDocument[] List(int pActionLogID)
        {
            List<ActionOutputDocument> actionOutputDocuments = new List<ActionOutputDocument>();

            using (SqlConnection sqlConnection = new SqlConnection(base.ConnectionString))
            {
                SqlCommand sqlCommand = new SqlCommand("[ActionOutputDocument_List]");
                try
                {
                    sqlConnection.Open();
                    sqlCommand.Connection = sqlConnection;
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.Add(new SqlParameter("@ActionLogID", pActionLogID));

                    SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                    while (sqlDataReader.Read())
                    {
                        ActionOutputDocument actionOutputDocument = new ActionOutputDocument(sqlDataReader.GetInt32(0));
                        if (sqlDataReader.IsDBNull(1) == false)
                            actionOutputDocument.DocumentType = sqlDataReader.GetString(1);
                        if (sqlDataReader.IsDBNull(2) == false)
                            actionOutputDocument.PublicFacingFolder = sqlDataReader.GetString(2);
                        if (sqlDataReader.IsDBNull(3) == false)
                            actionOutputDocument.InternalFacingFolder = sqlDataReader.GetString(3);
                        if (sqlDataReader.IsDBNull(4) == false)
                            actionOutputDocument.Filename = sqlDataReader.GetString(4);

                        actionOutputDocuments.Add(actionOutputDocument);
                    }

                    sqlDataReader.Close();
                }
                catch (Exception exc)
                {
                    ActionOutputDocument actionOutputDocument = new ActionOutputDocument();
                    actionOutputDocument.ErrorsDetected = true;

                    MethodBase currentMethod = MethodBase.GetCurrentMethod(); base.LogError(currentMethod.DeclaringType.Name, currentMethod.Name, exc.Message);

                    actionOutputDocument.ErrorDetails.Add(new ErrorDetail(-1, currentMethod.Name + " : " + exc.Message));
                    actionOutputDocuments.Add(actionOutputDocument);
                }
                finally
                {
                    sqlCommand.Connection.Close(); sqlCommand.Connection.Dispose(); sqlCommand.Dispose();
                    sqlConnection.Close();
                    sqlConnection.Dispose();
                }
            }
            return actionOutputDocuments.ToArray();
        }

        public ActionOutputDocument Insert(ActionOutputDocument pActionOutputDocument, int pActionLogID)
        {
            ActionOutputDocument actionOutputDocument = new ActionOutputDocument();

            using (SqlConnection sqlConnection = new SqlConnection(base.ConnectionString))
            {
                SqlCommand sqlCommand = new SqlCommand("[ActionOutputDocument_Insert]");
                try
                {
                    sqlConnection.Open();
                    sqlCommand.Connection = sqlConnection;
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.Add(new SqlParameter("@ActionLogID", pActionLogID));
                    sqlCommand.Parameters.Add(new SqlParameter("@DocumentType", pActionOutputDocument.DocumentType));
                    sqlCommand.Parameters.Add(new SqlParameter("@PublicFacingFolder", pActionOutputDocument.PublicFacingFolder));
                    sqlCommand.Parameters.Add(new SqlParameter("@InternalFacingFolder", pActionOutputDocument.InternalFacingFolder));
                    sqlCommand.Parameters.Add(new SqlParameter("@Filename", pActionOutputDocument.Filename));
                    SqlParameter actionOutputDocumentID = sqlCommand.Parameters.Add("@ActionOutputDocumentID", SqlDbType.Int);
                    actionOutputDocumentID.Direction = ParameterDirection.Output;

                    sqlCommand.ExecuteNonQuery();

                    actionOutputDocument = new ActionOutputDocument((Int32)actionOutputDocumentID.Value);
                    actionOutputDocument.DocumentType = pActionOutputDocument.DocumentType;
                    actionOutputDocument.PublicFacingFolder = pActionOutputDocument.PublicFacingFolder;
                    actionOutputDocument.InternalFacingFolder = pActionOutputDocument.InternalFacingFolder;
                    actionOutputDocument.Filename = pActionOutputDocument.Filename;
                }
                catch (Exception exc)
                {
                    actionOutputDocument.ErrorsDetected = true;

                    MethodBase currentMethod = MethodBase.GetCurrentMethod(); base.LogError(currentMethod.DeclaringType.Name, currentMethod.Name, exc.Message);

                    actionOutputDocument.ErrorDetails.Add(new ErrorDetail(-1, currentMethod.Name + " : " + exc.Message));
                }
                finally
                {
                    sqlCommand.Connection.Close(); sqlCommand.Connection.Dispose(); sqlCommand.Dispose();
                    sqlConnection.Close();
                    sqlConnection.Dispose();
                }
            }
            return actionOutputDocument;
        }

        public ActionOutputDocument Update(ActionOutputDocument pActionOutputDocument)
        {
            ActionOutputDocument actionOutputDocument = new ActionOutputDocument();

            using (SqlConnection sqlConnection = new SqlConnection(base.ConnectionString))
            {
                SqlCommand sqlCommand = new SqlCommand("[ActionOutputDocument_Update]");
                try
                {
                    sqlConnection.Open();
                    sqlCommand.Connection = sqlConnection;
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.Add(new SqlParameter("@ActionOutputDocumentID", pActionOutputDocument.ActionOutputDocumentID));
                    sqlCommand.Parameters.Add(new SqlParameter("@DocumentType", pActionOutputDocument.DocumentType));
                    sqlCommand.Parameters.Add(new SqlParameter("@PublicFacingFolder", pActionOutputDocument.PublicFacingFolder));
                    sqlCommand.Parameters.Add(new SqlParameter("@InternalFacingFolder", pActionOutputDocument.InternalFacingFolder));
                    sqlCommand.Parameters.Add(new SqlParameter("@Filename", pActionOutputDocument.Filename));

                    sqlCommand.ExecuteNonQuery();

                    actionOutputDocument = new ActionOutputDocument(pActionOutputDocument.ActionOutputDocumentID);
                    actionOutputDocument.DocumentType = pActionOutputDocument.DocumentType;
                    actionOutputDocument.PublicFacingFolder = pActionOutputDocument.PublicFacingFolder;
                    actionOutputDocument.InternalFacingFolder = pActionOutputDocument.InternalFacingFolder;
                    actionOutputDocument.Filename = pActionOutputDocument.Filename;
                }
                catch (Exception exc)
                {
                    actionOutputDocument.ErrorsDetected = true;

                    MethodBase currentMethod = MethodBase.GetCurrentMethod(); base.LogError(currentMethod.DeclaringType.Name, currentMethod.Name, exc.Message);
                    base.LogError(currentMethod.DeclaringType.Name, currentMethod.Name, exc.ToString(), false);
                    base.LogError(currentMethod.DeclaringType.Name, currentMethod.Name, "ActionOutputDocumentID: " + pActionOutputDocument.ActionOutputDocumentID, false);
                    base.LogError(currentMethod.DeclaringType.Name, currentMethod.Name, exc.StackTrace.ToString());
                    actionOutputDocument.ErrorDetails.Add(new ErrorDetail(-1, currentMethod.Name + " : " + exc.Message));
                }
                finally
                {
                    sqlCommand.Connection.Close(); sqlCommand.Connection.Dispose(); sqlCommand.Dispose();
                    sqlConnection.Close();
                    sqlConnection.Dispose();
                }
            }
            return actionOutputDocument;
        }

        public ActionOutputDocument Delete(ActionOutputDocument pActionOutputDocument)
        {
            ActionOutputDocument actionOutputDocument = new ActionOutputDocument();

            using (SqlConnection sqlConnection = new SqlConnection(base.ConnectionString))
            {
                SqlCommand sqlCommand = new SqlCommand("[ActionOutputDocument_Delete]");
                try
                {
                    sqlConnection.Open();
                    sqlCommand.Connection = sqlConnection;
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.Add(new SqlParameter("@ActionOutputDocumentID", pActionOutputDocument.ActionOutputDocumentID));

                    sqlCommand.ExecuteNonQuery();

                    actionOutputDocument = new ActionOutputDocument(pActionOutputDocument.ActionOutputDocumentID);
                }
                catch (Exception exc)
                {
                    actionOutputDocument.ErrorsDetected = true;

                    MethodBase currentMethod = MethodBase.GetCurrentMethod(); base.LogError(currentMethod.DeclaringType.Name, currentMethod.Name, exc.Message);

                    actionOutputDocument.ErrorDetails.Add(new ErrorDetail(-1, currentMethod.Name + " : " + exc.Message));
                }
                finally
                {
                    sqlCommand.Connection.Close(); sqlCommand.Connection.Dispose(); sqlCommand.Dispose();
                    sqlConnection.Close();
                    sqlConnection.Dispose();
                }
            }
            return actionOutputDocument;
        }
    }
}
