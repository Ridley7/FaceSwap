using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using r_core.coresystems.optionalsystems.api;

public class AccessCamera : MonoBehaviour
{
    public UITexture uiTexture; // Referencia al UITexture donde se mostrará la cámara
    private WebCamTexture webCamTexture;
    public UITexture result;

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

        Debug.Log("AccessCamera Start");

        //Obtenemos las camaras disponibles
        WebCamDevice[] devices = WebCamTexture.devices;

        if (devices.Length > 0)
        {
            // Usa la cámara frontal si está disponible
            webCamTexture = new WebCamTexture(devices[1].name);

            // Asigna la textura al UITexture
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

    public void TakePhoto()
    {
        if (webCamTexture != null && webCamTexture.isPlaying)
        {
            
            // Captura la textura actual de la cámara
            
            Texture2D faceImageFile = new Texture2D(webCamTexture.width, webCamTexture.height);
            faceImageFile.SetPixels(webCamTexture.GetPixels());
            faceImageFile.Apply();
            
            //Texture2D photo = Resources.Load<Texture2D>("API_Configuration/face_image");
            //Pegamos la foto en el resultado
            result.mainTexture = faceImageFile;

            // Guarda la foto como PNG -- No me hace falta guardar la foto
            //string path = Application.persistentDataPath + "/photo.png";
            //System.IO.File.WriteAllBytes(path, photo.EncodeToPNG());

        
        //Debug.Log("Foto tomada y guardada en: " + path);
        //Convertimos la foto a base64
        Debug.Log("<color=green>Convertimos la foto a base64</color>");
        //Convertimos la imagen a un array de bytes
        byte[] imageBytesPhoto = faceImageFile.EncodeToPNG();

        if(imageBytesPhoto == null)
        {
            Debug.Log("No se ha podido convertir la imagen de la cara a bytes");
        }

        //Convertimos los bytes a una cadena Base64
        string photoBase64 = System.Convert.ToBase64String(imageBytesPhoto);

        //Cargamos la imagen que queremos fusionar
        Texture2D imageFile = Resources.Load<Texture2D>("API_Configuration/tribu_mesoamericana");

        //Codificamos la imagen a Base64
        if (imageFile != null)
        {
            //Convertimos la imagen a un array de bytes
            byte[] imageBytes = imageFile.EncodeToJPG();

            //Convertimos los bytes a una cadena Base64
            string imageFileBase64 = System.Convert.ToBase64String(imageBytes);

            StartCoroutine(CreateFaceFusion(photoBase64, imageFileBase64));
        }
    }

}


    //Facefusion
    private IEnumerator CreateFaceFusion(string faceImageFile, string imageFile)
    {
        Debug.Log("<color=green>Voy a hacer peticion a Novita AI</color>");

        yield return R_APIController.GetInstance().FaceFusion<ResponseTileImage>(faceImageFile, imageFile, lambda =>
        {
            Debug.Log("Respuesta de Novita AI: ");
            Debug.unityLogger.Log("Developer", "Novita AI Dice que:");
            Debug.Log(lambda.image_type);

            Texture2D image = Base64ToTexture2D(lambda.image_file);
            FixColorChannels(image);
            result.mainTexture = image;
        });
    }

    private Texture2D Base64ToTexture2D(string base64String)
    {
        //Decodificamos la cadeba Base64 a bytes
        byte[] imageBytes = System.Convert.FromBase64String(base64String);

        //Creamos una nueva textura (sin mipmas y con formato RGBA32 por defecto)
        Texture2D texture = new Texture2D(2, 2);

        //Cargamos los datos de la imagen desde los bytes;
        texture.LoadImage(imageBytes);

        return texture;

    }

    void FixColorChannels(Texture2D texture)
    {
        Color[] pixels = texture.GetPixels();

        for (int i = 0; i < pixels.Length; i++)
        {
            Color pixel = pixels[i];

            // Intercambiar el canal rojo con el azul
            float temp = pixel.r;
            pixel.r = pixel.b;
            pixel.b = temp;

            pixels[i] = pixel;
        }

        // Aplicar los cambios
        texture.SetPixels(pixels);
        texture.Apply();
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




    void OnDestroy()
    {
        if (webCamTexture != null)
        {
            webCamTexture.Stop();
        }
    }

}


/*
using UnityEngine;

public class NGUIWebCamManager : MonoBehaviour
{
    public UITexture uiTexture; // Referencia al UITexture donde se mostrará la cámara
    private WebCamTexture webCamTexture;

    void Start()
    {
        // Obtén las cámaras disponibles
        WebCamDevice[] devices = WebCamTexture.devices;

        if (devices.Length > 0)
        {
            // Usa la cámara frontal si está disponible
            webCamTexture = new WebCamTexture(devices[0].name);

            // Asigna la textura al UITexture
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

    public void TakePhoto()
    {
        if (webCamTexture != null && webCamTexture.isPlaying)
        {
            // Captura la textura actual de la cámara
            Texture2D photo = new Texture2D(webCamTexture.width, webCamTexture.height);
            photo.SetPixels(webCamTexture.GetPixels());
            photo.Apply();

            // Guarda la foto como PNG
            string path = Application.persistentDataPath + "/photo.png";
            System.IO.File.WriteAllBytes(path, photo.EncodeToPNG());

            Debug.Log("Foto tomada y guardada en: " + path);
        }
    }

    void OnDestroy()
    {
        if (webCamTexture != null)
        {
            webCamTexture.Stop();
        }
    }
}

*/