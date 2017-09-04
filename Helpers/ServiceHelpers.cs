using System;
using System.ComponentModel;
using System.Configuration.Install;
using System.Reflection;

namespace Boerman.Core.Helpers
{
    internal static class ServiceHelpers
    {
        public static void Install()
        {
            try
            {
                AdminHelper.EnsureAdministratorAccess();
                ManagedInstallerClass.InstallHelper(new string[] {Assembly.GetExecutingAssembly().Location});
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null && ex.InnerException.GetType() == typeof(Win32Exception))
                {
                    Win32Exception wex = (Win32Exception)ex.InnerException;
                    Console.WriteLine("Error(0x{0:X}): Service already installed!", wex.ErrorCode);
                }
                else
                {
                    Console.WriteLine(ex.ToString());
                }
            }


            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Install succeeded");
            Console.ResetColor();
        }

        public static void Uninstall()
        {
            try
            {
                AdminHelper.EnsureAdministratorAccess();
                ManagedInstallerClass.InstallHelper(new string[] { "/u", Assembly.GetExecutingAssembly().Location });
            }
            catch (Exception ex)
            {
                if (ex.InnerException.GetType() == typeof(Win32Exception))
                {
                    Win32Exception wex = (Win32Exception)ex.InnerException;

                    Console.WriteLine("Error(0x{0:X}): Service not installed!", wex.ErrorCode);
                }
                else
                {
                    Console.WriteLine(ex.ToString());
                }
            }
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Uninstall succeeded");
            Console.ResetColor();
        }
    }
}
