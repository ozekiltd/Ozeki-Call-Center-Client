using OPSIVRSystem.Config;
using OPSIVRSystem.IVRMenus;

namespace OPSIVRSystem.CommonViewModel
{
    public class VmIVRMenuElementVoiceMessageRecorder :VmIVRMenuElementBase
    {
        private string _postIntroduction;
        private string _postIntroductionAudio;


        public VmIVRMenuElementVoiceMessageRecorder()
        {
            Icon = "/IVRStudio;component/Resources/voicerecorder.png";
            Introduction = "This is a voice message recorder menu";
            Name = "Voice message recorder menu element";
            TypeText = "Voice message recorder menu";
            PostIntroduction = "Thank you bye";
            PostIntroductionAudio = "";
        }

        public string PostIntroduction
        {
            get { return _postIntroduction; }
            set { _postIntroduction = value; RaisePropertyChanged(()=> PostIntroduction); }
        }

        public string PostIntroductionAudio
        {
            get { return _postIntroductionAudio; }
            set { _postIntroductionAudio = value;  RaisePropertyChanged(()=>PostIntroductionAudio);}
        }

        private VmIVRMenuElementVoiceMessageRecorder(VmIVRMenuElementVoiceMessageRecorder original)
            : base(original)
        {
            PostIntroduction = original.PostIntroduction;
            PostIntroductionAudio = original.PostIntroductionAudio;
        }

        public VmIVRMenuElementVoiceMessageRecorder(IVRMenuElementVoiceMessageRecorder model) : base (model)
        {
            Icon = "/OPSIVRSystem;component/Resources/voicerecorder.png";
            PostIntroduction = model.PostIntroduction;
            PostIntroductionAudio = model.PostIntroductionAudio;
        }

        public VmIVRMenuElementVoiceMessageRecorder(IVRMenuVoiceMessageRecorderConfig config)
            : base(config)
        {
            Icon = "/OPSIVRSystem;component/Resources/voicerecorder.png";
            PostIntroduction = config.PostIntroduction;
            PostIntroductionAudio = config.PostIntroductionAudio;
        }

        public override VmIVRMenuElementBase GetAClone()
        {
            return new VmIVRMenuElementVoiceMessageRecorder(this);
        }

        public override IVRMenuElementBase GetModel()
        {
            var res = new IVRMenuElementVoiceMessageRecorder();
            InitModelCommonFields(res);
            res.PostIntroduction = PostIntroduction;
            res.PostIntroductionAudio = PostIntroductionAudio;
            return res;
        }
    }
}
