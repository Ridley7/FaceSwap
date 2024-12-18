using UnityEngine;

[AddComponentMenu("NGUI/Examples/Spin With Mouse")]
public class SpinWithMouse : MonoBehaviour
{
	public Transform target;
	public float speed = 1f;

	Transform mTrans;

	void Start ()
	{
		mTrans = transform;
	}

	void OnDrag (Vector2 delta)
	{
		UICamera.currentTouch.clickNotification = UICamera.ClickNotification.None;

		if (target != null)
		{
			//target.localRotation = Quaternion.Euler(0f, -0.5f * delta.x * speed, 0f) * target.localRotation;
			target.localRotation = Quaternion.Euler(0f, 0f, 0.5f * delta.y * speed) * mTrans.localRotation;
			//Debug.Log("Rotando ");
		}
		else
		{
			//mTrans.localRotation = Quaternion.Euler(0f, -0.5f * delta.x * speed, 0f) * mTrans.localRotation;
			mTrans.localRotation = Quaternion.Euler(0f, 0f, 0.5f * delta.y * speed) * mTrans.localRotation;
		}
	}
}