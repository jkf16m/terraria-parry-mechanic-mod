namespace parry_mechanic.Content
{

    /**
     * <summary>
     * Register here all the types of messages, either CSC or SC.
     * Usually messages that start from the client, follow with the server and end in the clients (or client, if needed).
     * </summary>
     */
    public enum MessageType : byte
    {
        None,
        Parry,
        GiveParryBuff,
        GiveStrainedReflexesBuff,
        GiveHeightenedSensesBuff,
    }
}
