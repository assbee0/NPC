using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class IndexValue : MonoBehaviour
{
    // Start is called before the first frame update
    private Text tex;
    void Start()
    {
        tex = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        tex.text = SaveLoad.number[0].ToString() + "   " + SaveLoad.number[1].ToString() + "   " + SaveLoad.number[2].ToString();
    }
}
