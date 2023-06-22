namespace Sound
{
    /// <summary>
    /// Defines the kinds of sounds that can be emitted
    /// </summary>
    public enum SoundType
    {
        //These could be more granular if the behaviour doesn't match expectations
        None,
        AiGenerated,
        Prop,
        Lure,
        PlayerGenerated
    }
}