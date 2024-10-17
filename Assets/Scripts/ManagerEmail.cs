using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagerEmail : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        UnityGMail unityGMail = new UnityGMail();
        unityGMail.SendMailFromGoogle();

    }
}
