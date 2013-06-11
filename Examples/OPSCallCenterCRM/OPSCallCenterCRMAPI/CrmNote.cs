using System.Runtime.Serialization;

namespace OPSCallCenterCRMAPI
{
    [DataContract]
    public class CrmNote
    {
        [DataMember]
        public string Note { get; set; }

        [DataMember]
        public string OriginatorClient { get; set; }

        public override string ToString()
        {
            return string.Format("{0} (Added by {1})", Note, OriginatorClient);
        }
    }
}
