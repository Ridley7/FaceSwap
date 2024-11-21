
using UnityEngine;
using r_core.coresystems.optionalsystems.messages;

public class UIController : MonoBehaviour
{
    public UIScrollView scrollView;
    public int delta;

    // Start is called before the first frame update
   

    public void ClickButtonMale()
    {
        MoveForward();
    }

    public void ClickButtonFemale()
    {
        MoveForward();
    }

    private void MoveForward()
    {
        //scrollView.Scroll(delta);
        scrollView.MoveAbsolute(Vector3.left * delta);
    }

    private void MoveBackward()
    {
        //scrollView.Scroll(-delta);
        scrollView.MoveAbsolute(Vector3.left * -delta);
    }

    private void OnEnable() 
    {
        R_MessagesController<R_MessageUI>.AddObserver((int)GameEnums.MessagesTypes.UI, HandleUI);
    }

    private void OnDisable() 
    {
        R_MessagesController<R_MessageUI>.RemoveObserver((int)GameEnums.MessagesTypes.UI, HandleUI);    
    }

    private void HandleUI(R_MessageUI message)
    {

        if (message.SenderId != (uint)GameEnums.Senders.ButtonCircularMenu) return;

        switch (message.actionUI)
        {
            case GameEnums.ActionUI.MoveForward:
                MoveForward();
                break;
            case GameEnums.ActionUI.MoveBackward:
                MoveBackward();
                break;
            default:
                break;
        }
    }

}
