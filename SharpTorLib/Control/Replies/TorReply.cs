using System;
using System.Linq;

namespace SharpTorLib.Control.Replies
{
    /// <summary>
    /// Represents a single reply from tor control.
    /// </summary>
    public class TorReply
    {
        /// <summary>
        /// Code.
        /// </summary>
        public readonly int Code;

        /// <summary>
        /// Arguments
        /// </summary>
        public readonly string Arguments;

        /// <summary>
        /// Reply Type
        /// </summary>
        public readonly CommandReplyType Type = CommandReplyType.None;

        /// <summary>
        /// Raw message.
        /// </summary>
        public readonly string Raw;

        /// <summary>
        /// Construct a new TorReply instance based on the sourceReply instance. Should be used on extended TorReply classes.
        /// </summary>
        /// <param name="sourceReply">Source reply. Cannot be null.</param>
        /// <param name="extraReplies">(Optional) List of extra replies (if it is a set). Could include the sourceReply (filter it out manually)</param>
        /// <remarks>Create a class extending on TorReply that acceps a specific TorReply type and parse it.</remarks>
        public TorReply( params TorReply[] replies)
        {
            if (replies == null || replies.FirstOrDefault() == null)
            {
                return;
            }

            TorReply sourceReply = replies.First();

            Code = sourceReply.Code;
            Arguments = sourceReply.Arguments;
            Type = sourceReply.Type;
            Raw = sourceReply.Raw;
        }

        /// <summary>
        /// Construct a new TorReply instance.
        /// </summary>
        /// <param name="input">Input string.</param>
        /// <param name="isMultiReply">Is part of a multi-reply or not.</param>
        internal TorReply(string input, bool isMultiReply)
        {
            // Store the raw input
            Raw = input;

            if (isMultiReply && input == ".")
            {
                Type = CommandReplyType.EndOfMultiline;
                return;
            }

            if (isMultiReply)
            {
                Type = CommandReplyType.LineValue;
                return;
            }

            string code = string.Empty;

            // Go over the input
            foreach (var chr in input)
            {
                if ((chr != ' ') && (chr != '+') && (chr != '-'))
                {
                    code += chr;
                    continue;
                }

                if (chr == '+' && code == "250")
                {
                    Type = CommandReplyType.MultiLineValue;
                    Code = 250;
                }
                else if (chr == '-' && code == "250")
                {
                    Type = CommandReplyType.SingleLineValue;
                    Code = 250;

                }
                else if (int.TryParse(code, out Code))
                {
                    Type = CommandReplyType.Status;
                }

                break;
            }

            if (code.Length + 1 < input.Length)
            {
                Arguments = input.Substring(code.Length + 1);
            }

            // Basic sanity check
            if (Code >= 1000 || Code < 250)
            {
                throw new Exception("Invalid reply.");
            }

            // 650 - Notifications
            if (Code == 650)
            {
                Type = CommandReplyType.Notification;
            }
        }
    }
}
