using UnityEngine;
using System.Collections;

public class PreviewObject : MonoBehaviour
{


    void Update()
    {

        if (Input.GetMouseButton(0))
        {
            Vector3 pos = Camera.main.WorldToScreenPoint(transform.position);
            Vector3 dir = Input.mousePosition - pos;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }

    }

}
/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewObject : MonoBehaviour
{
    Vector3 mPrevPos = Vector3.zero;
    Vector3 mPosDelta = Vector3.zero;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButton(0))
        {
            mPosDelta = Input.mousePosition - mPrevPos;

            if(Vector3.Dot(transform.up, Vector3.up) >= 0)
                transform.Rotate(transform.up, -Vector3.Dot(mPosDelta, Camera.main.transform.right), Space.World);
            else
            {
                transform.Rotate(transform.up, Vector3.Dot(mPosDelta, Camera.main.transform.right), Space.World);
            }


            transform.Rotate(Vector3.up, -Vector3.Dot(mPosDelta, Camera.main.transform.right), Space.World);
            transform.Rotate(Camera.main.transform.right, Vector3.Dot(mPosDelta, Camera.main.transform.up), Space.World);
        }

        mPrevPos = Input.mousePosition;
    }
}
*/