using System.Text;

namespace SharpTorLib.Control.Commands
{
    /// <summary>
    /// TAKEOWNERSHIP command.
    /// </summary>
    public sealed class TakeOwnershipCommand : TorCommand
    {
        /// <summary>
        /// Construct a new AuthenticateCommand instance with an empty password.
        /// </summary>
        public TakeOwnershipCommand(string signal)
            : base("TAKEOWNERSHIP")
        {

        }
    }
}
