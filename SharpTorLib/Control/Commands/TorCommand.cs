using System.Text;

namespace SharpTorLib.Control.Commands
{
    /// <summary>
    /// Represents a tor control command.
    /// </summary>
    public class TorCommand
    {
        #region Static
        /// <summary>
        /// Constant representing a newline (\r\n)
        /// </summary>
        protected const string CrLf = "\r\n";

        /// <summary>
        /// Quote a string to a tor command protocol-compatible string.
        /// </summary>
        /// <param name="input">Input string.</param>
        /// <returns>Quoted string.</returns>
        public static string Quote(string input)
        {
            return string.Format("\"{0}\"", input.Replace("\"", "\\\""));
        }

        /// <summary>
        /// Cast the TorCommand to a byte[] array. Will never return null.
        /// </summary>
        /// <param name="command">The TorCommand to get the binary data for.</param>
        /// <returns>Binary data.</returns>
        public static explicit operator byte[](TorCommand command)
        {
            return command == null ? new byte[0] : command.GetBytes();
        }
        #endregion

        /// <summary>
        /// Arguments that go with the command.
        /// </summary>
        protected readonly string Arguments = string.Empty;

        /// <summary>
        /// Name of the command.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Get the binary data representing this instance.
        /// </summary>
        /// <returns>Binary data.</returns>
        public virtual byte[] GetBytes()
        {
            if (!string.IsNullOrEmpty(Arguments))
            {
                return Encoding.ASCII.GetBytes(string.Format("{0} {1}{2}", Name, Arguments, CrLf));
            }

            return Encoding.ASCII.GetBytes(string.Format("{0}{1}", Name, CrLf));
        }

        /// <summary>
        /// Construct a new TorCommand instance with the specified name and arguments.
        /// </summary>
        public TorCommand(string name, string arguments)
        {
            Name = name;
            Arguments = arguments;
        }


        /// <summary>
        /// Construct a new TorCommand instance with the specified name and no arguments.
        /// </summary>
        public TorCommand(string name)
            : this(name, string.Empty)
        {

        }

        /// <summary>
        /// Construct a new TorCommand instance, setting none of the other fields.
        /// </summary>
        protected TorCommand()
        {

        }
    }
}
