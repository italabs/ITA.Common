using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;

namespace ITA.Common.ETW
{
    [Serializable]
    [EventData]
    public class EtwParamCollection
    {
        public EtwParamCollection()
        {
            Parameters = new List<EtwParam>();
        }        

        public List<EtwParam> Parameters { get; set; }

        public override string ToString()
        {
            return Parameters == null ? "Parameters=\"\"" : string.Join(" ", Parameters.Select(p => p.ToString()));
        }
    }    
}
