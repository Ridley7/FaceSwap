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
        //Hacemos una llamada a una API
        //StartCoroutine(PeticionTest());
		StartCoroutine(CreateTileImage());

    }

	//Metodo para probar llamada a API
    private IEnumerator CreateTileImage()
	{
		yield return R_APIController.GetInstance().GetTileImage<ResponseTileImage>(lambda =>
		{
			Debug.Log("Respuesta de Novita AI: ");
			Debug.Log(lambda.image_type);

            image = Base64ToTexture2D(lambda.image_file);
            textura.mainTexture = image;


			//Faltaria:
			/*
			En lambda.image_file tenemos el contenido de la imagen codificado en base 64.
			Faltaria descodificar ese contenido y mostrarlo al usuario
			*/

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

