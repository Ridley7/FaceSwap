using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Esta clase es para ilustrar como se realiza el guardado y la carga de datos en el core
/// </summary>
public class R_DummyInfo {

    public string year;
    public float month;
    public float day;
    public int century;

    public R_DummyInfo(string year, float month, float day, int century)
    {
        this.year = year;
        this.month = month;
        this.day = day;
        this.century = century;
    }

}
