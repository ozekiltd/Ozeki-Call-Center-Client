using OPSCallCenterCRMAPI;
using OPSCallCenterCRMAPI.WCF;

namespace OPSCallCenterCRM.Model.Settings
{
    public class AppPreferences
    {
        public WcfConnectionProperties ConnectionProperties { get; set; }
        public ClientCredential ClientCredential { get; set; }

        public AppPreferences()
        {
            ClientCredential = new ClientCredential();
            ConnectionProperties = new WcfConnectionProperties();
        }
    }
}
