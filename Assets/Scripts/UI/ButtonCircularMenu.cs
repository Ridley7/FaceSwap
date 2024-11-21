
using UnityEngine;
using r_core.coresystems.optionalsystems.messages;

public class ButtonCircularMenu : MonoBehaviour
{
   private R_MessageUI messageUI = new R_MessageUI((int)GameEnums.Senders.ButtonCircularMenu);

   void OnClick()
   {
        messageUI.SetData(GameEnums.ActionUI.MoveForward);
        R_MessagesController<R_MessageUI>.Post((int)GameEnums.MessagesTypes.UI, messageUI);
   }
}
