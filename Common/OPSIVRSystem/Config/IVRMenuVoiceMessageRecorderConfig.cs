using System;

namespace OPSIVRSystem.Config
{
    [Serializable]
    public class IVRMenuVoiceMessageRecorderConfig : IVRMenuBaseConfig
    {
        public string PostIntroduction { get; set; }
        public string PostIntroductionAudio { get; set; }  

        public IVRMenuVoiceMessageRecorderConfig()
        {
            PostIntroduction = "";
            PostIntroductionAudio = "";
        }
    }
}
