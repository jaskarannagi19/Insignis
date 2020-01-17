using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Insignis.Console.Apps.Data.Loader.Config
{
    public class StagedFilterDefinition
    {
        public string TableNameLike = "";
        public string ConnectionString = "";
        public string ErrorLog = "";

        public string AdditionalCriterion = "";

        public List<FieldDefinition> FieldDefinitions = new List<FieldDefinition>();

        public StagedFilterDefinition(XmlElement pRoot)
        {
            try
            {
                if (pRoot.Name.ToString().CompareTo("stagedFilterDefinition") == 0)
                {
                    foreach (XmlAttribute attribute in pRoot.Attributes)
                    {
                        if (attribute.Name.ToString().CompareTo("tableNameLike") == 0)
                            TableNameLike = attribute.Value;
                        else if (attribute.Name.ToString().CompareTo("connectionString") == 0)
                            ConnectionString = attribute.Value;
                        else if (attribute.Name.ToString().CompareTo("errorLog") == 0)
                            ErrorLog = attribute.Value;
                    }

                    foreach (XmlElement child in pRoot.ChildNodes)
                    {
                        if (child.Name.CompareTo("fieldDefinitions") == 0)
                        {
                            foreach (XmlElement definition in child.ChildNodes)
                                FieldDefinitions.Add(new FieldDefinition(definition));
                        }
                    }
                }
            }
            catch
            {
            }
        }

        public string ToXML()
        {
            string xml = "";
            xml += "<stagedFilterDefinition ";
            xml += "tableNameLike=\"" + TableNameLike + "\" ";
            xml += "connectionString=\"" + ConnectionString + "\" ";
            xml += "errorLog=\"" + ErrorLog + "\" ";
            xml += ">";
            if (FieldDefinitions.Count > 0)
            {
                xml += "<fieldDefinitions>";
                foreach (FieldDefinition fieldDefinition in FieldDefinitions)
                    xml += fieldDefinition.ToXML();
                xml += "</fieldDefinitions>";
            }
            xml += "</stagedFilterDefinition>";
            return xml;
        }
        private string ListVisibleFields()
        {
            string visibleFieldList = "";
            bool first = true;
            foreach (FieldDefinition fieldDefinition in FieldDefinitions)
            {
                if (fieldDefinition.IsVisible)
                {
                    if (first == true)
                        first = false;
                    else
                        visibleFieldList += ",";
                    visibleFieldList += fieldDefinition.Name;
                }
            }
            return visibleFieldList;
        }

        private string FilterCriterion()
        {
            string filterCriterion = "";
            bool first = true;
            foreach (FieldDefinition fieldDefinition in FieldDefinitions)
            {
                if (fieldDefinition.IncludeInFilter)
                {
                    if (first == true)
                    {
                        first = false;
                        filterCriterion += " WHERE ";
                    }
                    else
                        filterCriterion += " AND ";
                    filterCriterion += "(";
                    if (fieldDefinition.Like.Trim().Length > 0)
                        filterCriterion += fieldDefinition.Name + ((fieldDefinition.Not==true) ? " NOT" : "") + " LIKE '%" + fieldDefinition.Like + "%'";
                    else if (fieldDefinition.Equals.Trim().Length > 0)
                    {
                        if (fieldDefinition.Convert.Trim().Length > 0)
                            filterCriterion += "CONVERT(" + fieldDefinition.Convert + "," + fieldDefinition.Name + ") " + ((fieldDefinition.Not==true) ? "<>" : "=") + " " + fieldDefinition.Equals;
                        else
                            filterCriterion += fieldDefinition.Name + ((fieldDefinition.Not == true) ? "<>" : "=") + "'" + fieldDefinition.Equals + "'";
                    }
                    else if (fieldDefinition.In.Trim().Length > 0)
                    {
                        filterCriterion += fieldDefinition.Name;
                        if (fieldDefinition.Not)
                            filterCriterion += " NOT";
                        filterCriterion += " IN (";
                        if (fieldDefinition.In.Contains("&amp;"))
                            filterCriterion += fieldDefinition.In.Replace("&amp;", "&");
                        else
                            filterCriterion += fieldDefinition.In;
                        filterCriterion += ")";
                    }
                    else
                    {
                        if (fieldDefinition.FromRange.Trim().Length > 0)
                        {
                            if (fieldDefinition.Convert.Trim().Length > 0)
                                filterCriterion += "CONVERT(" + fieldDefinition.Convert + "," + fieldDefinition.Name + ") >= " + fieldDefinition.FromRange;
                            else
                                filterCriterion += fieldDefinition.Name + " >= '" + fieldDefinition.FromRange + "'";
                        }

                        if (fieldDefinition.FromRange.Trim().Length > 0 && fieldDefinition.ToRange.Trim().Length > 0)
                            filterCriterion += " AND ";

                        if (fieldDefinition.ToRange.Trim().Length > 0)
                        {
                            if (fieldDefinition.Convert.Trim().Length > 0)
                                filterCriterion += "CONVERT(" + fieldDefinition.Convert + "," + fieldDefinition.Name + ") <= " + fieldDefinition.ToRange;
                            else
                                filterCriterion += fieldDefinition.Name + " <= '" + fieldDefinition.ToRange + "'";
                        }
                    }
                    filterCriterion += ")";
                }
            }
            if (AdditionalCriterion.Length > 0)
                filterCriterion += AdditionalCriterion;
            return filterCriterion;
        }

        public string BuildFilteredSelectQuery(string pTableName)
        {
            string SQL = "";
            SQL += "SELECT ";
            SQL += ListVisibleFields();
            SQL += " FROM " + pTableName;
            SQL += FilterCriterion();
            SQL += ";";
            return SQL;
        }

        public string BuildFilteredDistinctSelectQuery(string pDistinctFieldName, string pTableName)
        {
            string SQL = "";

            SQL += "SELECT DISTINCT(" + pDistinctFieldName + ") ";
            SQL += " FROM " + pTableName;
            SQL += FilterCriterion();
            SQL += " ORDER BY " + pDistinctFieldName + ";";
            return SQL;
        }

        public string BuildDistinctSelectQuery(string pDistinctFieldName, string pTableName)
        {
            string SQL = "";

            SQL += "SELECT DISTINCT(" + pDistinctFieldName + ") ";
            SQL += " FROM " + pTableName;
            SQL += " ORDER BY " + pDistinctFieldName + ";";
            return SQL;
        }

        public string BuildFilteredSelectQuery(string pFieldList, string pTableName)
        {
            string SQL = "";
            SQL += "SELECT ";
            SQL += pFieldList;
            SQL += " FROM " + pTableName;
            SQL += FilterCriterion();
            SQL += ";";
            return SQL;
        }

        public string BuildSelectQuery(string pFieldList, string pTableName)
        {
            string SQL = "";
            SQL += "SELECT ";
            SQL += pFieldList;
            SQL += " FROM " + pTableName;
            SQL += ";";
            return SQL;
        }
    }
}
