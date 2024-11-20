using UnityEngine;

public class ButtonTrigger : MonoBehaviour
{
    private RotateParentOnDrag parentController;
    private Quaternion initialRotation;

    private void Start()
    {
        // Buscar el script del padre
        parentController = GetComponentInParent<RotateParentOnDrag>();
        initialRotation = transform.rotation;
    }

    private void LateUpdate() {
        // Mantener la rotación inicial
        transform.rotation = initialRotation;
    }

    private void OnPress(bool isPressed)
    {
        // Notificar al padre sobre el estado del botón
        parentController.OnPress(isPressed);
    }

    private void OnDrag(Vector2 delta)
    {
        // Pasar el movimiento del drag al padre
        parentController.StartDrag(delta);
    }
}
