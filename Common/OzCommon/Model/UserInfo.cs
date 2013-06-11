using System.ComponentModel;

namespace OzCommon.Model
{
    public class UserInfo : IDataErrorInfo
    {
        public string ServerAddress { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public string this[string columnName]
        {
            get
            {
                switch (columnName)
                {
                    case "ServerAddress":
                        if (string.IsNullOrEmpty(ServerAddress))
                            return "This field is required";
                        break;
                    case "Username":
                        if (string.IsNullOrWhiteSpace(Username))
                            return "This field is required";
                        break;
                    case "Password":
                        if (string.IsNullOrWhiteSpace(Password))
                            return "This field is required";
                        break;
                }
                return null;
            }
        }

        public string Error { get; set; }

        public bool IsValid
        {
            get
            {
                return !string.IsNullOrWhiteSpace(Username) && !string.IsNullOrWhiteSpace(Password) && !string.IsNullOrWhiteSpace(ServerAddress);
            }
        }
    }
}
