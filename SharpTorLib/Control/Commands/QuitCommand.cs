namespace SharpTorLib.Control.Commands
{
    /// <summary>
    /// QUIT command.
    /// </summary>
    public sealed class QuitCommand : TorCommand
    {
        /// <summary>
        /// Construct a new AuthenticateCommand instance with an empty password.
        /// </summary>
        public QuitCommand(string signal)
            : base("QUIT")
        {

        }
    }
}