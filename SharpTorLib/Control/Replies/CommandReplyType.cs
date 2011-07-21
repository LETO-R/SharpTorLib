namespace SharpTorLib.Control.Replies
{
    /// <summary>
    /// List of possible command reply types.
    /// </summary>
    public enum CommandReplyType
    {
        None = 0,
        Status = 1,
        SingleLineValue = 2,
        MultiLineValue = 3,
        LineValue = 4,
        EndOfMultiline = 5,
        Notification = 6
    }
}
