using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Animations;
using System;

public class NPCAnimationController : MonoBehaviour
{
    private Animator animator;
    [SerializeField] private float arousal;
    [SerializeField] private float valence;
    private int catA;
    private int catV;

    public float timeOut = 2.0f; // 閾値
    private float timeElapsed; // 累計時間
    private float speed = 1.0f;　// Parameterの遷移スピード
    SVMExecute svmE;
    NPCAnimationController ownAnimCon;
    [SerializeField] RuntimeAnimatorController[] animConArr;

    public int test_A = -1; // 仮
    public int test_V = -1; // 仮
    public bool isLegend;

    private GameObject testManager;
    private EnvParameterGenerate envParaGen;

    [SerializeField] private float sensitivity; // 感応度合い
    public float radius = 1;

    private float coheAr = 0;
    private float coheVa = 0;

    private GameObject targetObject; // 注視したいオブジェクト

    private float lookAtWeight;
    private float bodyWeight;
    private float headWeight;
    private float eyesWeight;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        svmE = GetComponent<SVMExecute>();
        ownAnimCon = gameObject.GetComponent<NPCAnimationController>();
        //svmE.Predict();
        checkName(this.gameObject);
        //Execute();

        testManager = GameObject.Find("TestManager");
        envParaGen = testManager.GetComponent<EnvParameterGenerate>();

        targetObject = GameObject.Find("TargetObject");

        // 感応度：0～0.5 0.5より大きくするとみんな一緒になり始める
        sensitivity = UnityEngine.Random.Range(0.0f, 1.0f);
        //sensitivity = 0.2f; // 一定にしたいとき

        svmE.Predict();

        // 左利きにチェンジ．左利きの人の割合は10%らしい
        if (UnityEngine.Random.value > 0.9) 
        {
            animator.runtimeAnimatorController = animConArr[0];
        } else
        {
            animator.runtimeAnimatorController = animConArr[1];
        }

        if (envParaGen.isWave)
        {
            animator.SetBool("isWave", true);
        } else
        {
            animator.SetBool("isWave", false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // 一定間隔で実行する場合
        timeElapsed += Time.deltaTime;
        if (timeElapsed >= timeOut) {
            // 処理
            svmE.Predict();
            getAroundAVValue();
            timeElapsed = 0.0f;
        }

        //sensitivity = envParaGen.sensitivity; // 感応度合い
        //this.transform.LookAt(targetObject.transform);

        // A-V値の遷移
        if (isLegend == false) { // 通常時
            catA = svmE.result_A;
            catV = svmE.result_V;
            arousal = Mathf.Lerp(arousal, GetInterpValue(svmE.result_A, coheAr), Time.deltaTime * speed);
            valence = Mathf.Lerp(valence, GetInterpValue(svmE.result_V, coheVa), Time.deltaTime * speed);
            animator.SetInteger("Cat_A", svmE.result_A);
            animator.SetInteger("Cat_V", svmE.result_V);
        } else {                 // 凡例時
            //test_A = envParaGen.test_catA; // とりあえず
            //test_V = envParaGen.test_catV; // とりあえず
            catA = test_A;
            catV = test_V;
            arousal = Mathf.Lerp(arousal, GetInterpValue(test_A, coheAr), Time.deltaTime * speed);        
            valence = Mathf.Lerp(valence, GetInterpValue(test_V, coheVa), Time.deltaTime * speed);
            animator.SetInteger("Cat_A", test_A);
            animator.SetInteger("Cat_V", test_V);
        }
        animator.SetFloat("Arousal",arousal);
        animator.SetFloat("Valence",valence);
    }

    public void Execute()
    {
        GetSVMParameter svmP = GetComponent<GetSVMParameter>();
        float[] arrP = svmP.EnvParam;
        //SVMExecute svme = GetComponent<SVMExecute>();
        svmE.Predict();
        //arousal = Mathf.Lerp(arousal, getInterpValue(test_A, coheAr), Time.deltaTime * speed);        
        //arousal = Mathf.Lerp(arousal, getInterpValue(svme.result_A), Time.deltaTime * speed);
        //arousal = getInterpValue(svme.result_A);
        //valence = Mathf.Lerp(valence, getInterpValue(test_V, coheVa), Time.deltaTime * speed);
        //valence = Mathf.Lerp(valence, getInterpValue(svme.result_V), Time.deltaTime * speed);
        //valence = getInterpValue(svme.result_V);
        animator.SetFloat("Arousal",arousal);
        animator.SetFloat("Valence",valence);
    }

    public float GetInterpValue(int catg, float cohePara) // 補間値を乱数で取得
    {
        float boundary = 256.0f / 3.0f;
        float random = 0;
        if (catg == 0) {
            random = UnityEngine.Random.Range(0.0f, boundary);
        } else if (catg == 1) {
            random = UnityEngine.Random.Range(boundary, boundary * 2.0f);
        } else if (catg == 2) {
            random = UnityEngine.Random.Range(boundary * 2.0f, 256.0f);
        }

        float MIN = 0.0f;
        float MAX = 256.0f;
        return Mathf.Clamp(random - sensitivity * cohePara, MIN, MAX);
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

    public void getAroundAVValue()
    {
        // 中心:自分の位置, 半径:radiusの球内に存在するものを検出
        Collider[] arrayCld = Physics.OverlapSphere(transform.position, radius);
        List<GameObject> listObj = new List<GameObject>();
        // 検出したGameObjectの内、tagが"NPC"であるものをlistObjリストに追加
        foreach (Collider cld in arrayCld)
        {
            if (cld.tag == "NPC")
            {
                listObj.Add(cld.gameObject);
            }
        }

        float[] avValue = new float[2];
        foreach (GameObject obj in listObj)
        {
            // 対象となるGameObjectとの距離を調べる
            float dist = Vector3.Distance(obj.transform.position, transform.position);
            // 対象となるGameObjectのA-V値を調べる
            NPCAnimationController aroundAnimCon = obj.gameObject.GetComponent<NPCAnimationController>();
            float diffAr = arousal - aroundAnimCon.Arousal;
            float diffVa = valence - aroundAnimCon.Valence;

            // 周囲の累計A-V値を更新
            coheAr += calcValue(diffAr, dist);
            coheVa += calcValue(diffVa, dist);
        }
    }

    public float calcValue(float diff, float dist)
    {
        return diff * (float)Math.Exp(-1 * (double)dist);
    }

    public bool LookAtObject()
    {
        float threshold_value = 0;
        if (catA == 1 && catV == 1)
        {
            threshold_value = 0.4f;
        }
        else if (catA == 1 && catV == 2)
        {
            threshold_value = 0.6f;
        }
        else if (catA == 2 && catV == 1)
        {
            threshold_value = 0.6f;
        }
        else if (catA == 2 && catV == 2)
        {
            threshold_value = 0.8f;
        } else
        {
            threshold_value = 0.0f;
        }

        if (UnityEngine.Random.value < threshold_value)
        {
            return true;
        } else {
            return false;
        }
    }

    private void OnAnimatorIK(int layerIndex)
    {
        if (targetObject != null)
        {
            if (arousal > valence)
            {
                lookAtWeight = arousal / 256.0f;
                bodyWeight = arousal / 256.0f;
                headWeight = arousal / 256.0f;
                eyesWeight = arousal / 256.0f;
            } else {
                lookAtWeight = valence / 256.0f;
                bodyWeight = valence / 256.0f;
                headWeight = valence / 256.0f;
                eyesWeight = valence / 256.0f;
            }

            this.animator.SetLookAtWeight(lookAtWeight, bodyWeight, headWeight, eyesWeight);
            this.animator.SetLookAtPosition(targetObject.transform.position);
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
