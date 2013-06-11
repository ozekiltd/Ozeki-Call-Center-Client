using OPSCallCenterCRMAPI.WCF;
using OPSCallCenterCRMServer.Model.Database;

namespace OPSCallCenterCRMServer.Model.Settings
{
    public class AppPreferences
    {
        public WcfConnectionProperties WcfConnectionProperties { get; set; }
        public DatabaseConnectionProperties DatabaseConnectionProperties { get; set; }
    }
}
