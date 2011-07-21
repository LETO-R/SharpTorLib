using System.IO;
using System.Net.Sockets;
using System.Text;

namespace SharpTorLib.Internal
{
    /// <summary>
    /// Internal Socket extensions.
    /// </summary>
    internal static class SocketEx
    {
        /// <summary>
        /// Read a single line from the socket using the specified encoding.
        /// </summary>
        public static string ReadLine(this Socket socket, Encoding encoding)
        {
            using (var ns = new NetworkStream(socket, FileAccess.Read, false))
            {
                using (var reader = new StreamReader(ns, encoding))
                {
                    return reader.ReadLine();
                }
            }
        }

        /// <summary>
        /// Read all lines from the socket using the specified encoding.
        /// </summary>
        public static string ReadAll(this Socket socket, Encoding encoding)
        {
            using (var ns = new NetworkStream(socket, FileAccess.Read, false))
            {
                using (var reader = new StreamReader(ns, encoding))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        /// <summary>
        /// Check if there is any data ready to be received.
        /// </summary>
        public static bool IsDataAvailable(this Socket socket)
        {
            using (var ns = new NetworkStream(socket, false))
            {
                return ns.DataAvailable;
            }
        }
    }
}
