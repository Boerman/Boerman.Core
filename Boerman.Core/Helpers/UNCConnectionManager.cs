/*
 * ToDo: Add a method to check whether the connection is still accessible. If it isn't we'd either want to disconnect or reconnect to the resource.
 * ToDo: Add a timer method which either keeps the connection alive if it isn't used or just disposes the connection.
 * ToDo: If CheckNetworkPathAvailable returns false, check whether a simple command like ls works correctly. Another share with the same credentials may already be connected.
 * ToDo: Make EnsureNetworkPathAvailable thread safe (So that only one connection can be made at a time. (which is already the case but hey, prevent crashes and stuff)
 */

using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;

namespace Boerman.Core.Helpers
{
    public static class UNCConnectionHelper
    {
        // Source: http://stackoverflow.com/a/11787433/1720761
        // Mainly using this method because a `File.Exists` takes between 1 and 3 seconds.
        public static bool CheckNetworkPathAvailable(string path)
        {
            if (string.IsNullOrEmpty(path)) return false;
            var pathRoot = Path.GetPathRoot(path);
            if (string.IsNullOrEmpty(pathRoot)) return false;

            var pinfo = new ProcessStartInfo("net", "use")
            {
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                UseShellExecute = false
            };

            string output;

            using (var p = Process.Start(pinfo))
            {
                output = p.StandardOutput.ReadToEnd();
            }
            
            return output.Split(Environment.NewLine.ToCharArray()).Any(line => line.Contains(pathRoot) && line.Contains("OK"));
        }
        
        public static bool EnsureNetworkPathAvailable(this string location, NetworkCredential networkCredential)
        {
            try
            {
                if (CheckNetworkPathAvailable(location)) return true;

                UNCConnectionManager connectionManager = UNCConnectionManager.Instance;
                
                connectionManager.CreateNetworkConnection(location, networkCredential);

                return true;
            }
            catch (Exception e)
            {
#if DEBUG
                throw;
#endif
                // ToDo: Log this error
                return false;
            }
        }
    }

    public sealed class UNCConnectionManager
    {
        private static readonly Lazy<UNCConnectionManager> Lazy =
            new Lazy<UNCConnectionManager>(() => new UNCConnectionManager());

        public static UNCConnectionManager Instance => Lazy.Value;
        
        private readonly ConcurrentDictionary<string, NetworkConnection> _networkConnections =
            new ConcurrentDictionary<string, NetworkConnection>();

        private UNCConnectionManager()
        {
        }

        public NetworkConnection CreateNetworkConnection(string location, NetworkCredential networkCredential)
        {
            if (!_networkConnections.ContainsKey(location))
                _networkConnections.AddOrUpdate(location, new NetworkConnection(location, networkCredential), (s, connection) => connection);

            return GetNetworkConnection(location);
        }

        public NetworkConnection GetNetworkConnection(string location)
        {
            NetworkConnection networkConnection;
            _networkConnections.TryGetValue(location, out networkConnection);
            return networkConnection;
        }

        public void RemoveNetworkConnection(string location)
        {
            NetworkConnection networkConnection;
            _networkConnections.TryRemove(location, out networkConnection);
            networkConnection.Dispose();
        }
    }
}
