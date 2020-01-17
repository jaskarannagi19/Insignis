using Octavo.Gate.Nabu.Entities;

namespace Insignis.Asset.Management.Action.Logging.Entities
{
    public class ActionOutputTable : BaseType
    {
        public int? ActionOutputTableID = null;
        public string TableName = null;
        public int? RowIndex = null;
        public int? ColumnIndex = null;
        public string CellValue = null;

        public ActionOutputTable()
        {
        }
        public ActionOutputTable(int pActionOutputTableID)
        {
            ActionOutputTableID = pActionOutputTableID;
        }
        public ActionOutputTable(int? pActionOutputTableID)
        {
            ActionOutputTableID = pActionOutputTableID;
        }
        public ActionOutputTable(string pTableName, int pRowIndex, int pColumnIndex, string pCellValue)
        {
            TableName = pTableName;
            RowIndex = pRowIndex;
            ColumnIndex = pColumnIndex;
            CellValue = pCellValue;
        }
    }
}
