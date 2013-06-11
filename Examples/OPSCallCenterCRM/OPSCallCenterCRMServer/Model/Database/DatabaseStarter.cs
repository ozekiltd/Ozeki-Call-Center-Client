using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace OPSCallCenterCRMServer.Model.Database
{
    internal class DatabaseStarter
    {
        private static int _databaseExecutablePid;

        public static string DatabasePath
        {
            get
            {
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Ozeki", "OPSCallCenterCrmServer", "Database");
            }
        }
        public static string ExecutablePath
        {
            get
            {
#if DEBUG
                return Path.Combine(Directory.GetParent(Assembly.GetExecutingAssembly().Location).Parent.Parent.FullName, "Resources", "Database", "OPSCallCenterCrmDatabase.exe");
#else
                return "OPSCallCenterCrmDatabase.exe";
#endif
            }
        }

        public static void StartDatabase(DatabaseConnectionProperties connectionProperties)
        {
            try
            {
                if (!Directory.Exists(DatabasePath))
                    Directory.CreateDirectory(DatabasePath);

                var lockFile = Path.Combine(DatabasePath, "mongod.lock");
                if (File.Exists(lockFile))
                    File.Delete(lockFile);

                var mongoDbProcess = new Process();
                mongoDbProcess.StartInfo.FileName = ExecutablePath;
                mongoDbProcess.StartInfo.Arguments = string.Format(" --port {0} --dbpath \"{1}\"",
                                                                   connectionProperties.Port,
                                                                   DatabasePath);
                mongoDbProcess.StartInfo.CreateNoWindow = true;
                mongoDbProcess.StartInfo.RedirectStandardError = true;
                mongoDbProcess.StartInfo.RedirectStandardOutput = true;
                mongoDbProcess.StartInfo.UseShellExecute = false;

                mongoDbProcess.Start();
                _databaseExecutablePid = mongoDbProcess.Id;
            }
            catch (Exception) { }
        }

        public static void StopDatabase()
        {
            try
            {
                if (_databaseExecutablePid != 0)
                    Process.GetProcessById(_databaseExecutablePid).Kill();
            }
            catch (Exception) { }
        }
    }
}
