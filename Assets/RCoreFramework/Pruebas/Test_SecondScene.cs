using r_core.core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_SecondScene : MonoBehaviour {

	// Use this for initialization
	void Start () {

		Debug.Log("La pool de sonidos solo ha creado 5 audio source, comprobamos que se cree una mas en tiempo de ejecucion");

		R_Core.GetInstance().PlaySound("Crumple", 1.0f);

		R_Core.GetInstance().StartTimer(1.0f, this, cacheAction =>
		{
			(cacheAction.Context as Test_SecondScene).PlaySoundOne(); 
		});

		R_Core.GetInstance().StartTimer(1.0f, this, cacheAction =>
		{
			(cacheAction.Context as Test_SecondScene).PlaySoundTwo();
		});

		R_Core.GetInstance().StartTimer(1.0f, this, cacheAction =>
		{
			(cacheAction.Context as Test_SecondScene).PlaySoundThree();
		});

		R_Core.GetInstance().StartTimer(1.0f, this, cacheAction =>
		{
			(cacheAction.Context as Test_SecondScene).PlaySoundFour();
		});

		R_Core.GetInstance().StartTimer(1.0f, this, cacheAction =>
		{
			(cacheAction.Context as Test_SecondScene).PlaySoundFive();
		});

		R_Core.GetInstance().StartTimer(1.0f, this, cacheAction =>
		{
			(cacheAction.Context as Test_SecondScene).PlaySoundSix();
		});

	}
	
	private void PlaySoundOne()
    {
		R_Core.GetInstance().PlaySound("Rumble", 1.0f);
	}

	private void PlaySoundTwo()
	{
		R_Core.GetInstance().PlaySound("Swipe", 1.0f);
	}
	private void PlaySoundThree()
	{
		R_Core.GetInstance().PlaySound("Tap", 1.0f);
	}

	private void PlaySoundFour()
	{
		R_Core.GetInstance().PlaySound("Crumple", 1.0f);
	}

	private void PlaySoundFive()
    {
		R_Core.GetInstance().PlaySound("Rumble", 1.0f);
    }

	private void PlaySoundSix()
	{
		R_Core.GetInstance().PlaySound("Swipe", 1.0f);
	}
}
