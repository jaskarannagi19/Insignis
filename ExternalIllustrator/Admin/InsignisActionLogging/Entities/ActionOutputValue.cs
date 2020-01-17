using Octavo.Gate.Nabu.Entities;

namespace Insignis.Asset.Management.Action.Logging.Entities
{
    public class ActionOutputValue : BaseType
    {
        public int? ActionOutputValueID = null;
        public string OutputGroupName = null;
        public string OutputName = null;
        public string OutputValue = null;

        public ActionOutputValue()
        {
        }
        public ActionOutputValue(int pActionOutputValueID)
        {
            ActionOutputValueID = pActionOutputValueID;
        }
        public ActionOutputValue(int? pActionOutputValueID)
        {
            ActionOutputValueID = pActionOutputValueID;
        }
        public ActionOutputValue(string pOutputGroupName, string pOutputName, string pOutputValue)
        {
            OutputGroupName = pOutputGroupName;
            OutputName = pOutputName;
            OutputValue = pOutputValue;
        }
    }
}
