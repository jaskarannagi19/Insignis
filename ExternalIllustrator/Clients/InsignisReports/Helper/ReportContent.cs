using Octavo.Gate.Nabu.Entities;
using System.Collections.Generic;

namespace Insignis.Asset.Management.Reports.Helper
{
    public class ReportContent : BaseType
    {
        public string URI = "";
    }

    public class ExtendedReportContent : ReportContent
    {
        public List<string> OtherFiles = new List<string>();
    }
}
