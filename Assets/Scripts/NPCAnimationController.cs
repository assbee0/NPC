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
    SVMExecute svmE;
    NPCAnimationController ownAnimCon;

    public int test_A = -1; // 仮
    public int test_V = -1; // 仮
    public bool isLegend;
    

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        svmE = GetComponent<SVMExecute>();
        ownAnimCon = gameObject.GetComponent<NPCAnimationController>();
        svmE.Predict();
        checkName(this.gameObject);
        //Execute();
    }

    // Update is called once per frame
    void Update()
    {
        // 一定間隔で実行する場合
        timeElapsed += Time.deltaTime;
        if (timeElapsed >= timeOut) {
            // 処理
            svmE.Predict();
            timeElapsed = 0.0f;
        }

        // A-V値の遷移
        if (isLegend == false) { // 通常時
            arousal = Mathf.Lerp(arousal, getInterpValue(svmE.result_A), Time.deltaTime * speed);
            valence = Mathf.Lerp(valence, getInterpValue(svmE.result_V), Time.deltaTime * speed);        
        } else {                 // 凡例時
            arousal = Mathf.Lerp(arousal, getInterpValue(test_A), Time.deltaTime * speed);        
            valence = Mathf.Lerp(valence, getInterpValue(test_V), Time.deltaTime * speed);
        }      
        //arousal = arousal + ownAnimCon.Arousal;
        //valence = valence + ownAnimCon.Valence;
        animator.SetFloat("Arousal",arousal);
        animator.SetFloat("Valence",valence);
        animator.SetInteger("Cat_A", test_A);
        animator.SetInteger("Cat_V", test_V);
    }

    public void Execute()
    {
        GetSVMParameter svmP = GetComponent<GetSVMParameter>();
        float[] arrP = svmP.ArrayParm;
        SVMExecute svme = GetComponent<SVMExecute>();
        svme.Predict();
        arousal = Mathf.Lerp(arousal, getInterpValue(test_A), Time.deltaTime * speed);        
        //arousal = Mathf.Lerp(arousal, getInterpValue(svme.result_A), Time.deltaTime * speed);
        //arousal = getInterpValue(svme.result_A);
        valence = Mathf.Lerp(valence, getInterpValue(test_V), Time.deltaTime * speed);
        //valence = Mathf.Lerp(valence, getInterpValue(svme.result_V), Time.deltaTime * speed);
        //valence = getInterpValue(svme.result_V);
        animator.SetFloat("Arousal",arousal);
        animator.SetFloat("Valence",valence);
    }

    public float getInterpValue(int catg) // 補間値を乱数で取得
    {
        float boundary = 256.0f / 3.0f;
        if (catg == 0) {
            return Random.Range(0.0f, boundary);
        } else if (catg == 1) {
            return Random.Range(boundary, boundary * 2.0f);
        } else if (catg == 2) {
            return Random.Range(boundary * 2.0f, 256.0f);
        } else {
            return -1;
        }
    }

    public void checkName(GameObject obj)
    {
        if (obj.name == "Ch20_nonPBR_Legend") {
            test_A = 0;
            test_V = 0;
        } else if (obj.name == "Ch20_nonPBR_Legend (1)") {
            test_A = 0;
            test_V = 1;
        } else if (obj.name == "Ch20_nonPBR_Legend (2)") {
            test_A = 0;
            test_V = 2;
        } else if (obj.name == "Ch20_nonPBR_Legend (3)") {
            test_A = 1;
            test_V = 0;
        } else if (obj.name == "Ch20_nonPBR_Legend (4)") {
            test_A = 1;
            test_V = 1;
        } else if (obj.name == "Ch20_nonPBR_Legend (5)") {
            test_A = 1;
            test_V = 2;
        } else if (obj.name == "Ch20_nonPBR_Legend (6)") {
            test_A = 2;
            test_V = 0;
        } else if (obj.name == "Ch20_nonPBR_Legend (7)") {
            test_A = 2;
            test_V = 1;
        } else if (obj.name == "Ch20_nonPBR_Legend (8)") {
            test_A = 2;
            test_V = 2;
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
