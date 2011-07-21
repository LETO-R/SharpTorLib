using System.Linq;

namespace SharpTorLib.Control.Replies
{
    public class GetInfoTorReply : TorReply
    {
        public readonly bool IsOK;

        public readonly string Key;
        public readonly string Value;

        public GetInfoTorReply(params TorReply[] replies) 
            : base(replies)
        {
            if (replies == null || replies.FirstOrDefault() == null || replies.Length != 2)
            {
                return;
            }


            IsOK = replies[1].Code == 250 && replies[0].Code == 250 && replies[0].Arguments.Contains("=");

            if (!IsOK)
            {
                return;
            }

            // Extract Key / Value
            Key = replies[0].Arguments.Split(new[] { '=' }, 2)[0];
            Value = replies[0].Arguments.Split(new[] { '=' }, 2)[1];
        }
    }
}
