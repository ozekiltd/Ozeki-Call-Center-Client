
namespace OzCommon.Model
{
    public class UserInfoSettingsRepository : GenericSettingsRepository<UserInfo>, IUserInfoSettingsRepository
    {
        public UserInfoSettingsRepository():base(null, "userSettings.xml")
        {
            
        }
    }
}
