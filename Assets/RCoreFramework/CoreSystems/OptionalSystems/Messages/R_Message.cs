namespace r_core.coresystems.optionalsystems.messages
{
    //
    // instructions are meant to be "quickly executable"
    // something like spawn a new NPC
    // or give player money, etc, etc
    // something that DOESN'T need a confirmation by the player
    // just something that "happens"
    //
    // in case of a network game we can add some fields like
    // ID for the character who sent this
    // bool to know if this instruction should be sent online (replication)
    //

    [System.Serializable]
    public abstract class R_Message
    {
        public bool IsLocal { get; protected set; }
        public uint SenderId { get; protected set; } //whose entity is using this message

        //lazy constructor
        public R_Message()
        {
            IsLocal = true;
            SenderId = 0;
        }

        public R_Message(uint _senderId)
        {
            IsLocal = true;
            SenderId = _senderId;
        }
    }
}