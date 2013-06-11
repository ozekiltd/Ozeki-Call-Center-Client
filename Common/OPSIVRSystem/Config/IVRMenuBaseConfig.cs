using System;
using System.Xml.Serialization;
using OPSIVRSystem.IVRMenus;

namespace OPSIVRSystem.Config
{
    [Serializable]
    [XmlInclude(typeof(IVRMenuInfoReaderConfig)), XmlInclude(typeof(IVRMenuVoiceMessageRecorderConfig)), XmlInclude(typeof(IVRMenuCallTransferConfig))]
    public class IVRMenuBaseConfig
    {

        public string Name { get; set; }

        public string Introduction { get; set; }

        public string AudioFile { get; set; }

        public string TouchToneKey { get; set; }

        public string Id { get; set; }

        public string ParentId { get; set; }

        public NarratorType NarratorType  { get; set; }

        public IVRMenuBaseConfig()
        {
            
        }
    }
}
