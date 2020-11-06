using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class EmotionControll : MonoBehaviour
{
    public float emotion = 0;
    private Animator animator = null;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        animator.SetFloat("Emotion", emotion/100);
    }
    
    public void changeEmotion(Slider slider)
    {
        emotion = slider.value;
    }
}
