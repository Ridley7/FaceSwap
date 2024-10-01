using r_core.core;
using r_core.coresystems.optionalsystems.api;
using r_core.coresystems.optionalsystems.messages;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TestTimers : MonoBehaviour {

	private R_MessageTest message_test = new R_MessageTest();


	// Use this for initialization
	void Start () {


        //StartCoroutine(PeticionTest());



        
		Debug.Log("En 3 segundos, ejecutaremos el metodo tonto");

		R_Core.GetInstance().StartTimer(3.0f, this, cacheAction =>
		{
			(cacheAction.Context as TestTimers).MetodoTonto();
		});
		


    }

	private IEnumerator PeticionTest()
	{
		yield return R_APIController.GetInstance().GetTest<List<ModelTest>>(lambda =>
		{
			for(int i = 0, max = lambda.Count; i < max; i++)
			{
				Debug.Log(lambda[i].id);
			}
		});
	}

	
    private void MetodoTonto()
	{
		Debug.Log("Ejecutando metodo el tonto.");
		Debug.Log("Ahora voy a enviar un mensaje con la mensajeria");
		message_test.SetData(1, "Mensaje que viaja");
		R_MessagesController<R_MessageTest>.Post((int)GameEnums.MessagesTypes.Test, message_test);
	}

	private void OnEnable()
	{
		R_MessagesController<R_MessageTest>.AddObserver((int)GameEnums.MessagesTypes.Test, MensajeTest);
	}

	private void OnDisable()
	{
		R_MessagesController<R_MessageTest>.RemoveObserver((int)GameEnums.MessagesTypes.Test, MensajeTest);
	}

	private void MensajeTest(R_MessageTest _mensaje)
	{
		Debug.Log("Tu mensaje es: " + _mensaje.message);
		Debug.Log("Ahora voy a usar el servicio de traduccion");

		Traducir();
	}

	private void Traducir()
    {
		Debug.Log(R_Core.GetInstance().GetString("PanelButton (1)TextOption2"));

		Debug.Log("Ahora lo dire en español");

		R_Core.GetInstance().SetNewLanguage(r_core.language.LanguageType.SPANISH);

		Debug.Log(R_Core.GetInstance().GetString("PanelButton (1)TextOption2"));

		Debug.Log("Esto funciona por que el lenguaje elegido es: " + R_Core.GetInstance().GetLanguage().ToString());

		GuardarCargar();

	}

	private void GuardarCargar()
    {

		Debug.Log("Vamos a probar el salvado y carga de datos");
		Debug.Log("Se van a guardar dos clases en disco: R_DummyData y R_DummyInfo. Estas han sido creadas solo para la desmotración");
		R_DummyData dummyData = new R_DummyData("Nico", 50.0f, 100.0f, 7);
		R_DummyInfo dummyInfo = new R_DummyInfo("2000", 12.0f, 2.0f, 20);

		Debug.Log("Convertimos ambas clases en JSON para almacenarlas");
		string jsonDummyData = JsonUtility.ToJson(dummyData);
		string jsonDummyInfo = JsonUtility.ToJson(dummyInfo);

		Debug.Log("Las clases en JSON son las siguientes: ");
		Debug.Log(jsonDummyData);
		Debug.Log(jsonDummyInfo);

		Debug.Log("Los JSON pasaran a ser almacenado en disco en los ficheros dummy_data.json y dummy_info.json ");
		R_Core.GetInstance().Save(jsonDummyData, "dummy_data.json");
		R_Core.GetInstance().Save(jsonDummyInfo, "dummy_info.json");

		Debug.Log("Ahora vamos a recuperar la información de ambos ficheros");
		R_DummyData dummyDataSaved;
		R_DummyInfo dummyInfoSaved;

		Debug.Log("Llamamos a nuestro core para obtener la información");
		dummyDataSaved = R_Core.GetInstance().LoadDummyData("dummy_data.json");
		dummyInfoSaved = R_Core.GetInstance().LoadDummyInfo("dummy_info.json");

		Debug.Log("La información recuperada de dummy data es: " + dummyDataSaved.name + " " + dummyDataSaved.mana);
		Debug.Log("La información recuperada de dummy info es: " + dummyInfoSaved.year + " " + dummyInfoSaved.day);

		PruebaSonido();
	}

	private void PruebaSonido()
    {
		Debug.Log("Ahora vamos a probar sonidos");

		R_Core.GetInstance().PlaySound("Crumple", 1.0f);

		Debug.Log("En 3 segundos cargo una escena para comprobar que la pool de sonidos esta viva");

		R_Core.GetInstance().StartTimer(3.0f, this, cacheAction =>
		{
			(cacheAction.Context as TestTimers).SonidoRetardado();
		});

	}

	private void SonidoRetardado()
    {
		Debug.Log("Cargando segunda escena...");
		SceneManager.LoadScene("SecondScene");		
	}

}

[System.Serializable]
public class ModelTest
{
    public int id;
    public string status;
    public string date_created;
    public string date_updated;
}