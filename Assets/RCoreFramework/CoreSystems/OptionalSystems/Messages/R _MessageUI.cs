
using r_core.coresystems.optionalsystems.messages;

public class R_MessageUI : R_Message
{
    public GameEnums.ActionUI actionUI { get; private set; }


    public R_MessageUI(uint senderId)
    {
        IsLocal = true;
        SenderId = senderId;
    }

    public void SetData(GameEnums.ActionUI actionUI)
    {
        IsLocal = true;
        this.actionUI = actionUI;
    }

    public void SetData(uint senderId, GameEnums.ActionUI actionUI)
    {
        IsLocal = true;
        SenderId = senderId;
        this.actionUI = actionUI;
    }
}
