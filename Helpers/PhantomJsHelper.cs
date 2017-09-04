// ToDo: Make the phantomjs helper generic so that we can run other processes besides highcharts on it. (Though highcharts is the only dependent we have right now)

using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;

namespace Boerman.Core.Helpers
{
    public class PhantomJsHelper
    {
        private Process _process;

        public PhantomJsHelper()
        {
            if (PhantomInstanceAvailable()) return;

            var currentPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            _process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    // ToDo: Set local paths
                    FileName = Path.Combine(currentPath, @"Dependencies\PhantomJS\phantomjs.exe"),
                    Arguments = $"\"{Path.Combine(currentPath, @"Dependencies\Highsoft\highcharts-convert.js")}\" -host 127.0.0.1 -port 3003",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };

            _process.Start();
        }

        public static bool PhantomInstanceAvailable()
        {
            return Process.GetProcessesByName("phantomjs").Any();
        }

        public Stream GetImage(string graphParams)
        {
            return GetImage(Encoding.Default.GetBytes(graphParams));
        }

        public Stream GetImage(byte[] graphParams)
        {
            // ToDo: Go use the HttpCommand because why not.
            //var request = new HttpCommand("http://127.0.0.1:3003")
            //    .Method("POST");
            var request = WebRequest.Create("http://127.0.0.1:3003");

            request.Method = "POST";
            request.ContentLength = graphParams.Length;
            request.ContentType = "application/json";

            var dataStream = request.GetRequestStream();
            dataStream.Write(graphParams, 0, graphParams.Length);
            dataStream.Close();

            var response = request.GetResponse();

            var memoryStream = new MemoryStream();
            response.GetResponseStream()?.CopyTo(memoryStream);
            memoryStream.Position = 0;
            response.Close();

            return memoryStream;
        }
    }
}
