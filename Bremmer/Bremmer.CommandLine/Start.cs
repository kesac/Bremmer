using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bremmer;
using System.Security.Policy;
using System.Security;
using System.Reflection;
using System.Security.Permissions;

namespace Bremmer.CommandLine
{
    public class Start
    {
        public static void Main(string[] args)
        {
            if (AppDomain.CurrentDomain.IsDefaultAppDomain())
            {
                SetupAppDomain(args);
                return;
            }

            if (args.Length > 0)
            {
                string source = args[0];
                try
                {
                    SiteBuilder builder = new SiteBuilder(source);
                    builder.Processed += HandleProcessEvent;
                    builder.Build();
                }
                catch (Exception e)
                {
                    System.Console.WriteLine(e.Message);
                }

            }
            else
            {
                System.Console.Write("Provide the build directory path as the first argument");
            }
        }

        private static void HandleProcessEvent(object sender, EventArgs e)
        {
            System.Console.WriteLine(((ProcessEventArgs)e).Description);
        }

        private static void SetupAppDomain(string[] args)
        {
            // For temporary files (https://github.com/Antaris/RazorEngine#temporary-files)

            AppDomain currentDomain = AppDomain.CurrentDomain;

            AppDomainSetup ads = new AppDomainSetup();
            ads.ApplicationBase = currentDomain.SetupInformation.ApplicationBase;
            StrongName[] trustedAssemblies = new StrongName[0];

            AppDomain domain = AppDomain.CreateDomain(
                "Bremmer", 
                null,
                currentDomain.SetupInformation, 
                new PermissionSet(PermissionState.Unrestricted),
                trustedAssemblies
            );

            domain.ExecuteAssembly(Assembly.GetExecutingAssembly().Location, args);
            AppDomain.Unload(domain);
        }
    }
}
