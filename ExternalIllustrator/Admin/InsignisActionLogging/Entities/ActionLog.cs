using Octavo.Gate.Nabu.Entities;
using System;
using System.Collections.Generic;

namespace Insignis.Asset.Management.Action.Logging.Entities
{
    public class ActionLog : BaseType
    {
        public int? ActionLogID = null;
        public string Reference = null;
        public string System = null;
        public string Module = null;
        public string Method = null;
        public string Action = null;
        public string ActionInitiatedByOrganisationName = null;
        public string ActionInitiatedByContactName = null;
        public string ActionInitiatedByContactEmail = null;
        public int? ActionInitatedOn = null;
        public int? ActionInitiatedAt = null;
        public string ActionStatus = null;
        public int? ActionCompletedOn = null;
        public int? ActionCompletedAt = null;

        public List<ActionSetting> Settings = new List<ActionSetting>();
        public List<ActionOutputValue> Values = new List<ActionOutputValue>();
        public List<ActionOutputTable> Tables = new List<ActionOutputTable>();
        public List<ActionOutputDocument> Documents = new List<ActionOutputDocument>();

        public ActionLog()
        {
        }
        public ActionLog(int pActionLogID)
        {
            ActionLogID = pActionLogID;
        }
        public ActionLog(int? pActionLogID)
        {
            ActionLogID = pActionLogID;
        }
    }
}
