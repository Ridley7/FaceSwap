using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenCamera : MonoBehaviour
{
    private WebCamTexture webCamTexture;
    public UITexture uiTexture; // Referencia al UITexture donde se mostrará la cámara

    // Start is called before the first frame update
    void Start()
    {
        #if UNITY_ANDROID
        // Verifica y solicita permisos
        if (!UnityEngine.Android.Permission.HasUserAuthorizedPermission(UnityEngine.Android.Permission.Camera))
        {
            UnityEngine.Android.Permission.RequestUserPermission(UnityEngine.Android.Permission.Camera);
        }

        if (!UnityEngine.Android.Permission.HasUserAuthorizedPermission(UnityEngine.Android.Permission.ExternalStorageWrite))
        {
            UnityEngine.Android.Permission.RequestUserPermission(UnityEngine.Android.Permission.ExternalStorageWrite);
        }
        #endif

        //Obtenemos las camaras disponibles
        WebCamDevice[] devices = WebCamTexture.devices;

        if(devices.Length > 0)
        {
            //Usamos la camara frontal si esta disponible
            webCamTexture = new WebCamTexture(devices[0].name);

            //Asignamos la textura al UITexture
            uiTexture.mainTexture = webCamTexture;

            // Ajusta las dimensiones para mantener la relación de aspecto
            AdjustAspectRatio();

            // Inicia la cámara
            webCamTexture.Play();
        }
        else
        {
            Debug.LogError("No se encontraron cámaras disponibles.");
        }
        
    }

    private void AdjustAspectRatio()
    {
        if (webCamTexture != null)
        {
            // Calcula la relación de aspecto
            float aspectRatio = (float)webCamTexture.width / webCamTexture.height;

            // Ajusta el tamaño del widget según la relación de aspecto
            uiTexture.width = Mathf.RoundToInt(uiTexture.height * aspectRatio);
        }
    }
}
