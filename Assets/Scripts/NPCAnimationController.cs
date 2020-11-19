using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCAnimationController : MonoBehaviour
{
    public float arousal;
    public float valence;
    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        Execute();
    }

    // Update is called once per frame
    void Update()
    {
        //Execute();
    }

    public void Execute()
    {
        SVMExecute svme = GetComponent<SVMExecute>();
        svme.Predict();
        arousal = getParameter(svme.result_A);
        valence = getParameter(svme.result_V);
        animator.SetFloat("Arousal",arousal);
        animator.SetFloat("Valence",valence);
        Debug.Log(arousal);
        Debug.Log(valence);
    }

    public float getParameter(int x)
    {
        float boundary = 256.0f / 3.0f;
        if (x == 0) {
            return Random.Range(0.0f, boundary);
        } else if (x == 1) {
            return Random.Range(boundary, boundary * 2.0f);
        } else if (x == 2) {
            return Random.Range(boundary * 2.0f, 256.0f);
        } else {
            return -1;
        }
    }
}
