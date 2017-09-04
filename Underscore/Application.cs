using System.Diagnostics;

namespace Boerman.Core.Underscore
{
    public partial class _
    {
        public static class Application
        {
            /// <summary>
            /// Reliably restart the currently running application with a few seconds for termination logic to execute. (See https://stackoverflow.com/a/9615697/1720761 for more information)
            /// </summary>
            public static void ReliableRestart()
            {
                ProcessStartInfo Info = new ProcessStartInfo();
                Info.Arguments = "/C ping 127.0.0.1 -n 2 && \"" + System.Windows.Forms.Application.ExecutablePath + "\"";
                Info.WindowStyle = ProcessWindowStyle.Hidden;
                Info.CreateNoWindow = true;
                Info.FileName = "cmd.exe";
                Process.Start(Info);
                System.Windows.Forms.Application.Exit();
            }

            /// <summary>
            /// Restarts the current running application
            /// </summary>
            public static void Restart()
            {
                System.Windows.Forms.Application.Restart();
            }
        }
    }
}
