using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu( menuName = "MyGame/Create ParameterTable", fileName = "ParameterTable" )]
public class ParameterTable : ScriptableObject
{
    [SerializeField] public int WIDTH = 8;
    [SerializeField] public int HEIGHT = 8;
} // class ParameterTable
