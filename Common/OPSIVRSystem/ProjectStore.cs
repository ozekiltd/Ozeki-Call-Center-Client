using System;
using System.Diagnostics;
using System.IO;
using System.Xml.Serialization;
using OPSIVRSystem.Config;
using OPSIVRSystem.IVRMenus;

namespace OPSIVRSystem
{
    public class ProjectStore
    {
        public IVRProject LoadProject(string path)
        {
            IVRProjectConfig config;
            if (File.Exists(path))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(IVRProjectConfig));
                using (var file = File.OpenRead(path))
                {
                    config = (IVRProjectConfig)serializer.Deserialize(file);
                    return GenerateIVRProjectFromConfig(config);// project;
                }
            }
            else
            {
                throw new FileNotFoundException("The given file doesn't exist. ");
            }
        }

        public void SaveProject(string path, IVRProject project)
        {
            try
            {
                using (var file = File.Create(path))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(IVRProjectConfig));
                    serializer.Serialize(file, CreateIVRProjectConfig(project));
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private IVRProject GenerateIVRProjectFromConfig(IVRProjectConfig config)
        {
            IVRProject resproject = new IVRProject();
            resproject.Name = config.Name;
            foreach (var curMenuConfig in config.MenuList)
            {
                resproject.MenuList.Add(GetIVRMenuFromConfig(curMenuConfig));
            }
            return resproject;
        }

        private IVRMenuElementBase GetIVRMenuFromConfig(IVRMenuBaseConfig menu)
        {
            IVRMenuElementBase result;
            if (menu is IVRMenuCallTransferConfig)
            {
                result = new IVRMenuElementCallTransfer((IVRMenuCallTransferConfig)menu);
            }
            else if (menu is IVRMenuInfoReaderConfig)
            {
                result = new IVRMenuElementInfoReader((IVRMenuInfoReaderConfig)menu);
            }
            else
            {
                result = new IVRMenuElementVoiceMessageRecorder((IVRMenuVoiceMessageRecorderConfig)menu);
            }
            return result;
        }

        private IVRProjectConfig CreateIVRProjectConfig(IVRProject project)
        {
            IVRProjectConfig resProject= new IVRProjectConfig();
            resProject.Name = project.Name;
            foreach (var curMenu in project.MenuList)
            {
                resProject.MenuList.Add(curMenu.GetConfig());
            }
            return resProject;
        }
    }
}
