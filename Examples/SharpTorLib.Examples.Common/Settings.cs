namespace SharpTorLib.Examples.Common
{
    /// <summary>
    /// Example settings.
    /// </summary>
    public static class Settings
    {
        /// <summary>
        /// Set this to the password used to authenticate to the tor control port.
        /// </summary>
        /// <remarks>Currently only no or clear-text passwords are supported (not cookie, ...) and only clear-text passwords were tested!</remarks>
        public const string TorPassword = "TOR_PASSWORD";
    }
}
