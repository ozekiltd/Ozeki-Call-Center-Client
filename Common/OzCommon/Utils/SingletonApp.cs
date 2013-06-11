using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Windows;
using System.Windows.Threading;

namespace OzCommon.Utils
{
    public class SingletonApp
    {
        SingletonApplicationEnforcer enforcer;
        ServiceController sc;
        Dispatcher dispatcher;
        string serviceExe;

        public SingletonApp(string applicationName)
        {
            enforcer = new SingletonApplicationEnforcer(DisplayArgs, applicationName);
            sc = new ServiceController(applicationName);
            serviceExe = applicationName + ".exe";
        }

        public void OnStartup(StartupEventArgs e)
        {
            if (e.Args.Length == 0)
            {
                if (enforcer.ShouldApplicationExit())
                {
                    Environment.Exit(0);
                }
            }
        }

        void DisplayArgs(IEnumerable<string> args)
        {
            var aurguments = args.ToArray();

            if (aurguments.Length != 2) return;

            if (enforcer.ShouldApplicationExit())
                Environment.Exit(0);
        }

    }
}
