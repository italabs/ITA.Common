﻿using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;

namespace ITA.Common.WCF.RestHelp.Data
{
    [DataContract]
    public class MethodDocumentation
    {
        public MethodInfo Method { get; set; }

        [DataMember]
        public string Summary { get; set; }

        [DataMember]
        public string Returns { get; set; }

        [DataMember]
        public MethodParamDocumentation OutputParameter { get; set; }

        [DataMember]
        public List<MethodParamDocumentation> InputParameters { get; set; }               
    }
}