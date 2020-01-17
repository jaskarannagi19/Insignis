using Octavo.Gate.Nabu.Entities;

namespace Insignis.Asset.Management.Action.Logging.Entities
{
    public class ActionSetting : BaseType
    {
        public int? ActionSettingID = null;
        public string SettingGroupName = null;
        public string SettingName = null;
        public string SettingValue = null;

        public ActionSetting()
        {
        }
        public ActionSetting(int pActionSettingID)
        {
            ActionSettingID = pActionSettingID;
        }
        public ActionSetting(int? pActionSettingID)
        {
            ActionSettingID = pActionSettingID;
        }
        public ActionSetting(string pSettingGroupName, string pSettingName, string pSettingValue)
        {
            SettingGroupName = pSettingGroupName;
            SettingName = pSettingName;
            SettingValue = pSettingValue;
        }
    }
}
