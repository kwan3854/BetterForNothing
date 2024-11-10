namespace BetterForNothing.Scripts.Message
{
    public class UnifiedLoadProgressMessage
    {
        public UnifiedLoadProgressMessage(float progress)
        {
            Progress = progress;
        }

        /// <summary>
        ///     Represents the progress of a loading operation.
        ///     0.0 ~ 1.0 range.
        /// </summary>
        public float Progress { get; }
    }
}