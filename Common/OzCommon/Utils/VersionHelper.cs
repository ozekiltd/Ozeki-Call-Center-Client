using System.Reflection;

namespace OzCommon.Utils
{
    public class VersionHelper
    {
        public static string GetVersionAsString()
        {
            return Assembly.GetEntryAssembly().GetName().Version.ToString(3);
        }
    }
}
