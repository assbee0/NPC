using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PerformanceSlider : MonoBehaviour
{
    private RealTimeGenerate rtg;
    private Slider slider;
    private int fever = 0;
    private int feverMax = 10;
    private int frameCount = 0;
    private int generateFrame = 10000;
    // Start is called before the first frame update
    void Start()
    {
        rtg = GameObject.Find("Custom Manager").GetComponent<RealTimeGenerate>();
        slider = GetComponent<Slider>();
        slider.interactable = false;
        slider.maxValue = 100;
        slider.minValue = 0;
        slider.value = 50;
    }

    // Update is called once per frame
    void Update()
    {
        slider.value += fever * Time.deltaTime / 5;
        ChangeText();

        if (frameCount % generateFrame == 0)
        {
            rtg.GenerateOneNPC();
            frameCount = 0;
        }
        
        frameCount++;

        if (slider.value <= 50)
            generateFrame = 10000;
        else
        generateFrame = (int)(300 - slider.value);
    }

    public void FeverPlus()
    {
        if (fever < feverMax)
            fever++;
    }

    public void FeverMinus()
    {
        if (fever > -feverMax)
            fever--;
    }

    private void ChangeText()
    {
        Text txt = GetComponentInChildren<Text>();
        txt.text = "Fever\n" + fever;
    }

    public int GetFever()
    {
        return fever;
    }

    public float GetScore()
    {
        return slider.value;
    }

    public void DestoryNPC()
    {
        rtg.NPCNumbers--;
    }
}
