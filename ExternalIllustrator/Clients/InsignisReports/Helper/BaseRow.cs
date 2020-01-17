using System.Collections.Generic;

namespace Insignis.Asset.Management.Reports.Helper
{
    public class BaseRow
    {
        public List<Cell> Cells = new List<Cell>();
        public List<KeyValuePair<string, string>> Styles = new List<KeyValuePair<string, string>>();
    }
}
