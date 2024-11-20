using System;
using UnityEngine;

public class RotateParentOnDrag : MonoBehaviour
{
    public Transform wheelTransform;
    private bool wheelBeingHeld = false;
    private Vector2 centerPoint;
    private float wheelAngle = 0f;
    private float wheelPrevAngle = 0f;

    public void StartDrag(Vector2 delta)
    {
        if (!wheelBeingHeld) return;

        // Obtenemos la posición actual del cursor
        Vector3 mousePos = Input.mousePosition;

        // Calcular el nuevo ángulo
        float wheelNewAngle = Vector2.Angle(Vector2.up, (Vector2)mousePos - centerPoint);

        // Ajustamos el ángulo basado en la posición del cursor
        if (mousePos.x > centerPoint.x)
            wheelAngle += wheelNewAngle - wheelPrevAngle;
        else
            wheelAngle -= wheelNewAngle - wheelPrevAngle;

        // Actualizar el ángulo previo
        wheelPrevAngle = wheelNewAngle;
    }

    private void Update()
    {
        if (wheelBeingHeld)
        {
            // Rotar el volante en base al ángulo calculado
            wheelTransform.localEulerAngles = new Vector3(0f, 0f, -wheelAngle);
        }
    }

    public void OnPress(bool isPressed)
    {
        if (isPressed)
        {
            wheelBeingHeld = true;

            // Obtener la posición central del volante en coordenadas locales
            UICamera uiCamera = UICamera.FindCameraForLayer(gameObject.layer);
            Vector3 worldPos = wheelTransform.position;
            centerPoint = uiCamera.cachedCamera.WorldToScreenPoint(worldPos);

            // Calcular el ángulo inicial del cursor
            Vector3 mousePos = Input.mousePosition;
            wheelPrevAngle = Vector2.Angle(Vector2.up, (Vector2)mousePos - centerPoint);
        }
        else
        {
            wheelBeingHeld = false;
        }
    }
}


/*
using System;
using UnityEngine;
public class RotateParentOnDrag : MonoBehaviour
{
    private Transform wheelTransform;
    private bool wheelBeingHeld = false;
    private Vector2 centerPoint;
    private float wheelAngle = 0f; 
    private float wheelPrevAngle = 0f;
    public float maximumSteeringAngle = 200f; // Máximo ángulo de giro
    public float wheelReleasedSpeed = 350f;  // Velocidad de retorno al centro
    private void Awake()
    {
        wheelTransform = transform;
    }

    private void OnDrag(Vector2 delta)
    {
        if(!wheelBeingHeld) return;

        //Obtenemos la posición actual del cursor
        Vector3 mousePos = Input.mousePosition;

        //Calcular el nuevo ángulo
        float wheelNewAngle = Vector2.Angle(Vector2.up, (Vector2)mousePos - centerPoint);
        
        //Ignorar si el cursor está demasiado cerca del centro
        //Ajustamos el angulo basado en la posicion del cursor
        if(mousePos.x > centerPoint.x)
            wheelAngle += wheelNewAngle - wheelPrevAngle;
        else
            wheelAngle -= wheelNewAngle - wheelPrevAngle;

        //Limitar el angulo al maximo permitido,
        wheelAngle = Mathf.Clamp(wheelAngle, -maximumSteeringAngle, maximumSteeringAngle);
        
        //Actualizar el angulo previo
        wheelPrevAngle = wheelNewAngle;
    }

    private void Update()
    {
        // Si no se sostiene el volante, vuelve al centro automáticamente
        if (!wheelBeingHeld && wheelAngle != 0f)
        {
            float deltaAngle = wheelReleasedSpeed * Time.deltaTime;

            if (Mathf.Abs(deltaAngle) > Mathf.Abs(wheelAngle))
                wheelAngle = 0f;
            else if (wheelAngle > 0f)
                wheelAngle -= deltaAngle;
            else
                wheelAngle += deltaAngle;
        }

        // Rotar el volante en base al ángulo calculado
        wheelTransform.localEulerAngles = new Vector3(0f, 0f, -wheelAngle);
    }
    
    private void OnPress(bool isPressed)
    {
        if (isPressed)
        {
            // Cuando el volante se presiona
            wheelBeingHeld = true;

            // Obtener la posición central del volante en coordenadas locales
            UICamera uiCamera = UICamera.FindCameraForLayer(gameObject.layer);
            Vector3 worldPos = wheelTransform.position;
            centerPoint = uiCamera.cachedCamera.WorldToScreenPoint(worldPos);

            // Calcular el ángulo inicial del cursor
            Vector3 mousePos = Input.mousePosition;
            Debug.Log("Mouse Pos: " + mousePos);
            wheelPrevAngle = Vector2.Angle(Vector2.up, (Vector2)mousePos - centerPoint);
           
        }
    }

    private void OnDragOut()
    {
        wheelBeingHeld = false;
    }

}
*/