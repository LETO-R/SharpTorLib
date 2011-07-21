using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using SharpTorLib.Control.Commands;

namespace SharpTorLib.Control
{
    public sealed class HiddenService
    {
        public string Host { get; private set; }
        public string PrivateKey { get; private set; }
        public string Folder { get; private set; }
        public short VirtualPort { get; internal set; }
        public IPAddress Address { get; internal set; }
        public short Port { get; internal set; }

        public bool IsLoaded { get; private set; }

        public HiddenService(string folder)
        {
            Folder = folder;

            Reload();
        }

        /// <summary>
        /// Attempt to reload the private key and hostname from the hidden service directory.
        /// </summary>
        /// <returns>True on success, false otherwise.</returns>
        public bool Reload()
        {
            try
            {
                if ((File.Exists(Path.Combine(Folder, "private_key"))) &&
                    (File.Exists(Path.Combine(Folder, "hostname"))))
                {
                    PrivateKey = File.ReadAllText(Path.Combine(Folder, "private_key"));
                    Host = File.ReadAllText(Path.Combine(Folder, "hostname"));

                    IsLoaded = true;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(string.Format("Exception while trying to read the hidden service information: {0}\r\nStacktrace:\r\n{1}", ex.Message, ex.StackTrace));
                IsLoaded = false;
            }

            return IsLoaded;
        }

        /// <summary>
        /// Returns the configuration-argumens for this hidden service.
        /// </summary>
        /// <returns>The configuration-argument string.</returns>
        public override string ToString()
        {
            string conf = string.Format("HiddenServiceDir={0}", Folder);

            if (Port != 0)
            {
                conf = string.Format("{0} {1}", conf,
                                     "HiddenServicePort=" + TorCommand.Quote(string.Format("{0} {1}:{2}", VirtualPort, Address, Port)));
            }

            return conf;
        }


    }
}
