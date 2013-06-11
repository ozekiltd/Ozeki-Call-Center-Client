using OzCommon.Model;

namespace OPSCallCenterCRM.Model.Settings
{
    class SettingsRepository : GenericSettingsRepository<AppPreferences>
    {
        public SettingsRepository()
            : base("OPSCallCenterCRM")
        { }
    }
}
