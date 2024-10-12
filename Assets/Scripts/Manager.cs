using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using r_core.coresystems.optionalsystems.api;

public class Manager : MonoBehaviour
{
    private Texture2D image;
    public UITexture textura;

    // Start is called before the first frame update
    void Start()
    {
        string faceImageFileBase64 = string.Empty;
        string imageFileBase64 = string.Empty;

        //La imagen de la cara es faceImageFile.
        //Esta imagen esta en resources y tengo que cargarla
        Texture2D faceImageFile = Resources.Load<Texture2D>("API_Configuration/face_image");
        
        //Codificamos la imagen a Base64
        if(faceImageFile != null)
        {


            //Convertimos la imagen a un array de bytes
            byte[] imageBytes = faceImageFile.EncodeToJPG();
        

            //Convertimos los bytes a una cadena Base64
            faceImageFileBase64 = System.Convert.ToBase64String(imageBytes);

            Debug.Log("Imagen de la cara convertida a Base64");

        }else
        {
            Debug.Log("No se ha podido cargar la imagen de la cara");
        }

        //La imagen que queremos fusionar es imageFile
        //Esta imagen esta en resources y tengo que cargarla
        
        Texture2D imageFile = Resources.Load<Texture2D>("API_Configuration/tribu_mesoamericana");
      
      
        //Codificamos la imagen a Base64
        if(imageFile != null)
        {
            //Convertimos la imagen a un array de bytes
            byte[] imageBytes = imageFile.EncodeToJPG();

            //Convertimos los bytes a una cadena Base64
            imageFileBase64 = System.Convert.ToBase64String(imageBytes);

            Debug.Log("Imagen a fusionar convertida a Base64");

        }
        else
        {
            Debug.Log("No se ha podido cargar la imagen a fusionar");
        }

        if(faceImageFile != null && imageFile != null)
        {
            //Hacemos una llamada a una API
            StartCoroutine(CreateFaceFusion(faceImageFileBase64, imageFileBase64));
            Debug.Log("Llamada a la API de FaceFusion");
        }
        

        //Hacemos una llamada a una API
		//StartCoroutine(CreateTileImage());

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

    //Facefusion
    private IEnumerator CreateFaceFusion(string faceImageFile, string imageFile)
    {
        yield return R_APIController.GetInstance().FaceFusion<ResponseTileImage>(faceImageFile, imageFile, lambda =>
        {
            Debug.Log("Respuesta de Novita AI: ");
            Debug.Log(lambda.image_type);

            image = Base64ToTexture2D(lambda.image_file);
            FixColorChannels(image);
            textura.mainTexture = image;
        });
    }

	//Metodo para probar llamada a API
    private IEnumerator CreateTileImage()
	{
		yield return R_APIController.GetInstance().GetTileImage<ResponseTileImage>(lambda =>
		{
            image = Base64ToTexture2D(lambda.image_file);
            textura.mainTexture = image;
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


}

