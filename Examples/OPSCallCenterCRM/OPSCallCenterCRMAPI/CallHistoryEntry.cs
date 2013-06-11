using System;
using System.Runtime.Serialization;

namespace OPSCallCenterCRMAPI
{
    [DataContract]
    public class CallHistoryEntry : EventArgs
    {
        [DataMember]
        public DateTime StartDate { get; set; }

        [DataMember]
        public string Callee { get; set; }

        [DataMember]
        public string Caller { get; set; }

        [DataMember]
        public TimeSpan CallLength { get; set; }
    }
}
