using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using SharpTorLib.Control.Commands;
using SharpTorLib.Control.Exceptions;
using SharpTorLib.Control.Replies;
using SharpTorLib.Internal;

namespace SharpTorLib.Control
{
    /// <summary>
    /// Client that is compatible with the tor control protocol.
    /// </summary>
    public class TorControlClient
    {
        private Socket _socket;
        private NetworkStream _stream;
        private StreamReader _reader;

        /// <summary>
        /// Configured address to access the tor control protocol.
        /// </summary>
        public readonly IPAddress Address;

        /// <summary>
        /// Configured port to access the tor control protocol.
        /// </summary>
        public readonly short Port;

        /// <summary>
        /// Event raised when a notification is received. This can occur while receiving a reply of a previously send command.
        /// </summary>
        public event Action<TorControlClient, TorReply> OnNotification;

        /// <summary>
        /// Construct an instance pointing to the loopback ip address and default tor control port (9051)
        /// </summary>
        public TorControlClient()
            : this(IPAddress.Loopback, 9051)
        {

        }

        /// <summary>
        /// Construct an instance pointing to the loopback ip address and the specified tor control port.
        /// </summary>
        public TorControlClient(short port)
            : this(IPAddress.Loopback, port)
        {

        }

        /// <summary>
        /// Construct an instance pointing to the specified ip address and default tor control port (9051)
        /// </summary>
        public TorControlClient(IPAddress address)
            : this(address, 9051)
        {

        }

        /// <summary>
        /// Construct an instance pointing to the specified ip address and tor control port.
        /// </summary>
        public TorControlClient(IPAddress address, short port)
        {
            Address = address;
            Port = port;
        }

        /// <summary>
        /// Connect and authenticate using the specified passwords. Throws if 
        /// <remarks>Throws an exception if connecting or authentication failed.</remarks>
        /// </summary>
        /// <param name="password"></param>
        public bool Connect(string password)
        {
            try
            {
                if (_socket == null || !_socket.IsBound)
                {
                    _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    _socket.Connect(Address, Port);
                    _socket.Send((byte[])new AuthenticateCommand(password));
                    _stream = new NetworkStream(_socket, FileAccess.ReadWrite, false);
                    _reader = new StreamReader(_stream, Encoding.ASCII);

                    var reply = new TorReply(_reader.ReadLine(), false);

                    if (reply.Code != 250 && reply.Code != 251) // Not 'Ok' or 'Not Needed'
                    {
                        throw new ConnectFailedException(reply, "Unable to authenticate.");
                    }

                    // Success
                    return true;
                }
            }
            catch
            {
                // If we have a socket
                if (_socket != null)
                {
                    try
                    {
                        // Attempt to close it
                        _socket.Close();
                    }
                    catch
                    {
                        // Ignore any exception as it might already be closed
                    }

                    // Unset the field
                    _socket = null;
                }

                throw;
            }

            return false;
        }

        /// <summary>
        /// Attempt to send a TorComand
        /// </summary>
        /// <param name="command">Command to send. Cannot be null.</param>
        /// <returns>An array of TorReply instances.</returns>
        public TorReply[] Send(TorCommand command)
        {
            if (command == null)
            {
                throw new ArgumentNullException("command");
            }

            if (_socket == null || !_socket.IsBound)
            {
                throw new InvalidOperationException("Not connected.");
            }

            _socket.Send((byte[])command);

            return Read();
        }

        /// <summary>
        /// Read any incoming TorReply instances. Will block if no data is currently available.
        /// </summary>
        /// <returns>An array of TorReply instances.</returns>
        public TorReply[] Read()
        {
            return Read(true);
        }

        /// <summary>
        /// Read any incoming TorReply instances. Will block if no data is currently available and 'blocking' is set to 'true'
        /// </summary>
        /// <param name="blocking">When set to true, will block when no data is available. When false, it'll return an empty array in when no data is available.</param>
        /// <returns>An array of TorReply instances.</returns>
        public TorReply[] Read(bool blocking)
        {
            if (_socket == null || !_socket.IsBound)
            {
                throw new InvalidOperationException("Not connected.");
            }

            if (!blocking && !_socket.IsDataAvailable())
            {
                return new TorReply[0];
            }

            var replies = new List<TorReply>();

            while (true)
            {
                string line = _reader.ReadLine();

                var torReply = new TorReply(line, replies.Count > 0 && replies[0].Type == CommandReplyType.MultiLineValue);

                // Notification - trigger the event if registered
                if (torReply.Type == CommandReplyType.Notification)
                {
                    if (OnNotification != null)
                    {
                        OnNotification(this, torReply);
                    }

                    continue;
                }

                // Add the reply to the list
                replies.Add(torReply);

                // If we receive a 'Status' reply, we're done receiving
                if (torReply.Type == CommandReplyType.Status)
                {
                    return replies.ToArray();
                }
            }
        }

        /// <summary>
        /// Register the specified HiddenService with Tor.
        /// </summary>
        /// <param name="service">Service to register. Cannot be null.</param>       
        /// <returns>An array of TorReply instances.</returns>
        public TorReply[] RegisterService(HiddenService service)
        {
            if (service == null)
            {
                throw new ArgumentNullException("service");
            }

            string arguments = service.ToString();

            // Loop over all current hidden services, and create the full configuration argument
            foreach (var s in HiddenServices)
            {
                arguments = string.Format("{0} {1}", arguments, s);
            }

            return Send(new TorCommand("setconf", arguments));
        }

        /// <summary>
        /// Save the configuration.
        /// </summary>   
        /// <returns>An array of TorReply instances.</returns>
        public TorReply[] SaveConfig()
        {
            Send(new TorCommand("saveconf", string.Empty));

            return Read();
        }

        /// <summary>
        /// Fetches the list of configured HiddenServices.
        /// </summary>
        public HiddenService[] HiddenServices
        {
            get
            {
                var replies = Send(new TorCommand("getconf", "HiddenServiceOptions"));

                // Perform some sanity checks
                if (replies.Length == 0 || replies[0].Code == 250 && replies[0].Arguments == "HiddenServiceOptions" && replies[0].Type == CommandReplyType.Status)
                {
                    return new HiddenService[0];
                }

                // Will store the list of hidden services
                var results = new List<HiddenService>();

                HiddenService currentService = null;
                foreach (var reply in replies.Where(r => r.Arguments.Contains("=")))
                {
                    if (reply.Arguments.StartsWith("HiddenServiceDir"))
                    {
                        currentService = new HiddenService(reply.Arguments.Split('=')[1]);

                        results.Add(currentService);
                    }

                    if (currentService == null)
                    {
                        continue;
                    }

                    if (reply.Arguments.StartsWith("HiddenServicePort"))
                    {
                        try
                        {
                            currentService.VirtualPort = short.Parse(reply.Arguments.Split('=')[1].Split(' ')[0]);
                            currentService.Address = IPAddress.Parse(reply.Arguments.Split('=')[1].Split(' ')[1].Split(':')[0]);
                            currentService.Port = short.Parse(reply.Arguments.Split('=')[1].Split(' ')[1].Split(':')[1]);
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(string.Format("Exception while trying to parse the hidden service configuration: {0}\r\nStacktrace:\r\n{1}", ex.Message, ex.StackTrace));
               
                            // An exception occured : reset all the values (check on this to detect failures of this kind)
                            currentService.VirtualPort = 0;
                            currentService.Address = null;
                            currentService.Port = 0;
                        }
                    }
                }

                // Return the resuls
                return results.ToArray();
            }
        }

        /// <summary>
        /// Closes the socket (if connected). Throws on error.
        /// </summary>
        public void Close()
        {
            try
            {
                if (_socket == null || !_socket.IsBound)
                {
                    return;
                }

                _socket.Close();
            }
            finally
            {
                _socket = null;
            }
        }
    }
}
