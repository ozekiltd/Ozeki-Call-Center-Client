using System;
using System.Collections.Generic;
using GalaSoft.MvvmLight;
using OPSIVRSystem.Config;
using OPSIVRSystem.IVRMenus;

namespace OPSIVRSystem.CommonViewModel
{
    public abstract class VmIVRMenuElementBase : ViewModelBase
    {
        public string Name
        {
            get { return _name; }
            set { _name = value; RaisePropertyChanged(()=> Name); }
        }

        public string TypeText { get; protected set; }
       
        public String Icon { get; set; }
        public string Introduction
        {
            get { return _introduction; }
            set { _introduction = value; RaisePropertyChanged(() => Introduction); }
        }

        private string _audioFile;
        public string AudioFile
        {
            get { return _audioFile; }
            set { _audioFile = value; RaisePropertyChanged(() => AudioFile); }
        }

        public string TouchToneKey
        {
            get { return _touchToneKey; }
            set { _touchToneKey = value; RaisePropertyChanged(() => TouchToneKey); }
        }

        public string Id { get; set; }

        public string ParentId { get; set; }

        public NarratorType NarratorType
        {
            get { return _narratorType; }
            set { _narratorType = value; RaisePropertyChanged(() => NarratorType); }
        }

        /// <summary>
        /// The current menu's parent menu.
        /// </summary>
        public VmIVRMenuElementBase Parent { get; set; }

        public IList<VmIVRMenuElementBase> ChildMenus;
        private string _name;
        private string _introduction;
        private string _touchToneKey;
        private NarratorType _narratorType;


        public VmIVRMenuElementBase()
        {
            Id = Guid.NewGuid().ToString();
            ParentId = "";
            TypeText = "Adj meg típust";
            TouchToneKey = "";
            Introduction = "";
            AudioFile = "";
            ChildMenus = new List<VmIVRMenuElementBase>();
        }

        protected VmIVRMenuElementBase(VmIVRMenuElementBase original)
        {
            Id = original.Id;
            Name = original.Name;
            TypeText = original.TypeText;
            TouchToneKey = original.TouchToneKey;
            Introduction = original.Introduction;
            AudioFile = original.AudioFile;
            NarratorType = original.NarratorType;
            ChildMenus = new List<VmIVRMenuElementBase>(original.ChildMenus);
            Parent = original.Parent;
        }

        public VmIVRMenuElementBase(IVRMenuElementBase model)
        {
            Id = model.Id;
            ParentId = model.ParentId;
            Name = model.Name;
            TouchToneKey = model.TouchToneKey;
            Introduction = model.Introduction;
            AudioFile = model.AudioFile;
            NarratorType = model.NarratorType;
            ChildMenus = new List<VmIVRMenuElementBase>();
        }

        public VmIVRMenuElementBase(IVRMenuBaseConfig model)
        {
            Id = model.Id;
            ParentId = model.ParentId;
            Name = model.Name;
            TouchToneKey = model.TouchToneKey;
            Introduction = model.Introduction;
            AudioFile = model.AudioFile;
            NarratorType = model.NarratorType;
            ChildMenus = new List<VmIVRMenuElementBase>();
        }

        public abstract VmIVRMenuElementBase GetAClone();

        public abstract IVRMenuElementBase GetModel();

        protected void InitModelCommonFields(IVRMenuElementBase model)
        {
            model.Id = Id;
            model.Name = Name;
            model.TouchToneKey = TouchToneKey;
            model.Introduction = Introduction;
            model.AudioFile = AudioFile;
            model.NarratorType = this.NarratorType;
            model.ParentId = ParentId;
        }
    }
}
