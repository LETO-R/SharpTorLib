using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using SharpTorLib.Control.Replies;

namespace SharpTorLib.Control.Exceptions
{
    /// <summary>
    /// Connect failed exception. 
    /// </summary>
    public sealed class ConnectFailedException : Exception
    {
        public readonly TorReply Reply;

        /// <summary>
        /// Initialize a new ConnectFailedException instance.
        /// </summary>
        public ConnectFailedException(TorReply reply)
        {
            Reply = reply;
        }

        /// <summary>
        /// Initialize a new ConnectFailedException instance.
        /// </summary>
        public ConnectFailedException(TorReply reply, string message) 
            : base(message)
        {
            Reply = reply;
        }

        /// <summary>
        /// Initialize a new ConnectFailedException instance.
        /// </summary>
        public ConnectFailedException(TorReply reply, string message, Exception innerException) 
            : base(message, innerException)
        {
            Reply = reply;
        }
    }
}
