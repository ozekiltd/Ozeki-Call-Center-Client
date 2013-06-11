using System.Xml.Serialization;

namespace OPSIVRSystem.IVRMenus
{
    
    public enum NarratorType
    {
        [XmlEnum(Name = "TextToSpeech")]
        TextToSpeech,
        [XmlEnum(Name = "FilePlayback")]
        FilePlayback
    }
}
