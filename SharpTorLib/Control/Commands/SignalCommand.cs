using System.Text;

namespace SharpTorLib.Control.Commands
{
    /// <summary>
    /// SIGNAL command.
    /// </summary>
    public sealed class SignalCommand : TorCommand
    {
        /// <summary>
        /// The password used to authenticate.
        /// </summary>
        public readonly string Signal;

        /// <summary>
        /// Construct a new AuthenticateCommand instance with an empty password.
        /// </summary>
        public SignalCommand(string signal)
            : base("SIGNAl", signal)
        {
            Signal = signal;
        }
    }
}
