using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterParameter : MonoBehaviour
{
    public float valueA;
    public float valueV;
    public int categoryA;
    public int categoryV;
    public float sensitivity;
    public float appeal;
    public float[] envParameter;
    public int d_categoryA;
    public int d_categoryV;
    public GlobalParameter gp;

    void Start()
    {
        gp = gameObject.GetComponent<GlobalParameter>();
        envParameter = new float[gp.ENVPARAMNUM];
    }
}
