using System.Text;

namespace SharpTorLib.Control.Commands
{
    /// <summary>
    /// AUTHENTICATE command.
    /// </summary>
    public sealed class AuthenticateCommand : TorCommand
    {
        /// <summary>
        /// The password used to authenticate.
        /// </summary>
        public readonly string Password = string.Empty;

        /// <summary>
        /// Construct a new AuthenticateCommand instance with an empty password.
        /// </summary>
        public AuthenticateCommand()
            : base("AUTHENTICATE")
        {

        }

        /// <summary>
        /// Construct a new AuthenticateCommand instance with the specified clear-text password.
        /// </summary>
        /// <param name="password">The password.</param>
        public AuthenticateCommand(string password)
            : base("AUTHENTICATE", string.IsNullOrEmpty(password) ? string.Empty : Quote(password))
        { 
            Password = password ?? string.Empty;
        }
    }
}
