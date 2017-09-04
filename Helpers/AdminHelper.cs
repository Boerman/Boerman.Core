using Boerman.Core.Extensions;
using System;
using System.Diagnostics;
using System.Security.Principal;

namespace Boerman.Core.Helpers
{
    public static class AdminHelper
    {
        /// <summary>
        /// This method will restart the program if there are not administrator rights available.
        /// </summary>
        public static void EnsureAdministratorAccess()
        {
            if (IsAdministrator()) return;

            // Restart program and run as admin. Please note that this completely breaks debuggability.
            var exeName = Process.GetCurrentProcess().MainModule.FileName;
            ProcessStartInfo startInfo = new ProcessStartInfo(exeName) { Verb = "runas", Arguments = Environment.GetCommandLineArgs().Join(" ") };
            Process.Start(startInfo);
            Environment.Exit(0);
        }


        /// <summary>
        /// Check if there are administrator rights for the current program instance.
        /// </summary>
        /// <returns>True if there are administrator rights available</returns>
        public static bool IsAdministrator()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }
    }
}
