using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using OPSIVRSystem.Config;
using Ozeki.Media.MediaHandlers;
using Ozeki.VoIP;

namespace OPSIVRSystem.IVRMenus
{
    [Serializable]
    [XmlInclude(typeof(IVRMenuElementInfoReader)), XmlInclude(typeof(IVRMenuElementVoiceMessageRecorder)), XmlInclude(typeof(IVRMenuElementCallTransfer))]
    public abstract class IVRMenuElementBase
    {
        public static string RootIdentifier = "";

        public const int BACK_TO_PARENT_MENU = 10;
        public const int EXIT = 0;
        public const int INPUT_END = 11;
 
        public string Name { get; set; }
      
        public string Introduction { get; set; }

        public string AudioFile { get; set; }

        public string TouchToneKey { get; set; }

        public string Id { get; set; }

        public string ParentId { get; set; }

        public NarratorType NarratorType
        {
            get { return _narratorType; }
            set { _narratorType = value; }
        }

        [XmlIgnore]
        protected IVRMenuNarrator Narrator
        {
            get
            {
                return _narrator;
            }
            set { _narrator = value;}
        }

        /// <summary>
        /// The current menu's parent menu.
        /// </summary>
        [XmlIgnore]
        public IVRMenuElementBase Parent { get; set; }

        [XmlIgnore]
        public IList<IVRMenuElementBase> ChildMenus;

        private IVRMenuNarrator _narrator;
        private NarratorType _narratorType;

        public IVRMenuElementBase()
        {
            Id = Guid.NewGuid().ToString(); //GetId();
            //ParentId = -1;
            ParentId = "";
            TouchToneKey = "";
            Introduction = "";
            ChildMenus = new List<IVRMenuElementBase>();
        }



        protected IVRMenuElementBase(IVRMenuElementBase original)
        {
            Id = original.Id;
            Name = original.Name;
            TouchToneKey = original.TouchToneKey;
            Introduction = original.Introduction;
            ChildMenus = new List<IVRMenuElementBase>(original.ChildMenus);
            Parent = original.Parent;
        }

        public IVRMenuElementBase(IVRMenuBaseConfig config)
        {
            Id = config.Id;
            ParentId = config.ParentId;
            Name = config.Name;
            TouchToneKey = config.TouchToneKey;
            Introduction = config.Introduction;
            AudioFile = config.AudioFile;
            NarratorType = config.NarratorType;
            ChildMenus = new List<IVRMenuElementBase>();
        }

        public abstract IVRMenuElementBase GetAClone();

        public abstract IVRMenuBaseConfig GetConfig();

        protected void SetConfigCommonField(IVRMenuBaseConfig config)
        {
            config.Id = Id;
            config.ParentId = ParentId;
            config.Name = Name;
            config.TouchToneKey = TouchToneKey;
            config.Introduction = Introduction;
            config.AudioFile = AudioFile;
            config.NarratorType = NarratorType;
        }
    
        public event EventHandler<VoIPEventArgs<VoIPMediaHandler>> IntroductionStarting;
        public event EventHandler<VoIPEventArgs<VoIPMediaHandler>> IntroductionStoped;
        /// <summary>
        /// Indicates the caller steps to an other menu or exit.
        /// </summary>
        public event EventHandler<VoIPEventArgs<IVRMenuElementBase>> StepIntoMenu;

        /// <summary>
        /// Indicates the introduction reading has finished.
        /// </summary>
        public event EventHandler IntroductionFinished;

        public event EventHandler<VoIPEventArgs<string>>  CallTransferRequired;


        public abstract void StartIntroduction();

        public abstract void StopIntroduction();

        public abstract void RestartIntroduction();

        /// <summary>
        /// Interprets the received DTMF sign.
        /// </summary>
        /// <param name="signal"></param>
        public abstract void CommandReceived(int signal);


        /// <summary>
        /// Indicates the current menu starts the reading.
        /// </summary>
        /// <param name="voiphandler"></param>
        protected void OnIntroductionStarting(VoIPMediaHandler voiphandler)
        {
            if (IntroductionStarting != null)
                IntroductionStarting(this, new VoIPEventArgs<VoIPMediaHandler>(voiphandler));
        }

        /// <summary>
        /// Indicates the current menu stops the reading.
        /// </summary>
        /// <param name="voiphandler"></param>
        protected void OnIntroductionStoped(VoIPMediaHandler voiphandler)
        {
            if (IntroductionStoped != null)
                IntroductionStoped(this, new VoIPEventArgs<VoIPMediaHandler>(voiphandler));
        }

        /// <summary>
        /// Indicates the current menu introduction reading has finished.
        /// </summary>
        protected void OnIntroductionFinished()
        {
            if (IntroductionFinished != null)
                IntroductionFinished(this, new EventArgs());
        }

        /// <summary>
        /// Indicates you must step to the desitaniton menu.
        /// </summary>
        /// <param name="destination"></param>
        protected void OnStepIntoMenu(IVRMenuElementBase destination)
        {
            if (StepIntoMenu != null)
                StepIntoMenu(this, new VoIPEventArgs<IVRMenuElementBase>(destination));
        }

        protected void OnCallTransferRequired(string transferTo)
        {
            if (CallTransferRequired != null)
            {
                CallTransferRequired(this, new VoIPEventArgs<string>(transferTo));
            }
        }

        protected void InitNarrator()
        {
            if (Narrator== null)
            {
                Narrator = IVRMenuNarrator.CreateNarrator(NarratorType);
            }
            
        }
    }
}
