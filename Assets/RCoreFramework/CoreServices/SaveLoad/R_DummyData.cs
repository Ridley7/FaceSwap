using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Esta clase es para ilustrar como se realiza el guardado y la carga de datos en el core
/// </summary>
public class R_DummyData {

	public string name;
	public float health;
	public float mana;
	public int level;

    public R_DummyData(string name, float health, float mana, int level)
    {
        this.name = name;
        this.health = health;
        this.mana = mana;
        this.level = level;
    }


}
