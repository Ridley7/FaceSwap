using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    public UIScrollView scrollView;
    private float delta = -0.1f;

    // Start is called before the first frame update
    
    private void OnEnable() {
        
    }

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
        scrollView.Scroll(delta);
    }

    private void MoveBackward()
    {
        scrollView.Scroll(-delta);
    }


}
