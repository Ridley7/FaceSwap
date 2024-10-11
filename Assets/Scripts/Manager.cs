using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using r_core.coresystems.optionalsystems.api;

public class Manager : MonoBehaviour
{
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

			//Faltaria:
			/*
			En lambda.image_file tenemos el contenido de la imagen codificado en base 64.
			Faltaria descodificar ese contenido y mostrarlo al usuario
			*/

		});
	}


}

[System.Serializable]
public class ResponseTileImage
{
	public TaskInfo task;
    public string image_file;
    public string image_type;
}

[System.Serializable]
public class TaskInfo
{
    public string task_id;
    public string status;
}

/*
{
    "task": {
        "task_id": "cabb8ef0-aac8-4443-84b1-cb663994e920",
        "status": "TASK_STATUS_SUCCEED"
    },
    "image_file": "iVBORw0KGgoAAAANSUhEUgAABAAAAAQACAYAAAB/HSuDAAAgAElEQVR4AbzB66",
    "image_type": "png"
}

*/