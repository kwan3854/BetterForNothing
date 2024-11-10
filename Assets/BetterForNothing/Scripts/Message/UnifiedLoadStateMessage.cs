namespace BetterForNothing.Scripts.Message
{
    public class UnifiedLoadStateMessage
    {
        /// <summary>
        ///     Loading message.
        ///     Displayed on the loading screen.
        ///     Don't use this message for fast and frequent updates.
        /// </summary>
        public readonly string Message;

        public UnifiedLoadStateMessage(string message)
        {
            Message = message;
        }
    }
}