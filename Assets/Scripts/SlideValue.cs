using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlideValue : MonoBehaviour
{
    private Text tex;
    private Slider slide;
    // Start is called before the first frame update
    void Start()
    {
        tex = GetComponent<Text>();
        slide = GetComponentInParent<Slider>();
    }

    // Update is called once per frame
    void Update()
    {
        if (slide == null)
            return;
        tex.text = "" + slide.value;
    }

}
