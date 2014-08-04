using System.Collections.Generic;

namespace CSMSL.IO.MzTab
{
    public class MzTabSoftware
    {
        public CVParamater Parameter { get; set; }
        public List<string> Settings { get; set; }

        public MzTabSoftware(CVParamater paramater)
        {
            Parameter = paramater;
            Settings = new List<string>();
        }

        public void AddSetting(string setting)
        {
            Settings.Add(setting);
        }

        public override string ToString()
        {
            return Parameter.ToString();
        }
    }
}
