namespace Ai
{
    /// <summary>
    /// Interface for providing an Interaction Type to components that require one.
    /// </summary>
    public interface IAiInteractionTypeSource
    {
        public InteractionType InteractionType { get; }
    }
}