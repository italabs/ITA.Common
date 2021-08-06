using System;
using System.Diagnostics.Tracing;

namespace ITA.Common.ETW
{
    [Serializable]
    [EventData]
    public class EtwReturnParamCollection : EtwParamCollection
    {
        public string ReturnValue { get; set; }

        public override string ToString()
        {
            return string.Format("ReturnValue=\"{0}\" {1}", ReturnValue, base.ToString());
        }
    }
}
