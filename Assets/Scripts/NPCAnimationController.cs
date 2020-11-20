using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCAnimationController : MonoBehaviour
{
    private Animator animator;
    [SerializeField] private float arousal;
    [SerializeField] private float valence;
    public float timeOut = 2.0f; // 閾値
    private float timeElapsed; // 累計時間
    private float speed = 1.0f;　// Parameterの遷移スピード
    

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        //Execute();
    }

    // Update is called once per frame
    void Update()
    {
        Execute();

        /* 一定間隔で実行する場合
        timeElapsed += Time.deltaTime;
        if (timeElapsed >= timeOut) {
            // 処理
            timeElapsed = 0.0f;
        }
        */
    }

    public void Execute()
    {
        SVMExecute svme = GetComponent<SVMExecute>();
        svme.Predict();
        arousal = Mathf.Lerp(arousal, getParameter(svme.result_A), Time.deltaTime * speed);
        //arousal = getParameter(svme.result_A);
        valence = Mathf.Lerp(valence, getParameter(svme.result_V), Time.deltaTime * speed);
        //valence = getParameter(svme.result_V);
        animator.SetFloat("Arousal",arousal);
        animator.SetFloat("Valence",valence);
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

    public float Arousal {
        get{ return arousal; }
        set{ arousal = value;}
    }

    public float Valence {
        get{ return valence; }
        set{ valence = value;}
    }    
}
