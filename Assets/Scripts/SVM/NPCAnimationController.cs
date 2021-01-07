using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Animations;
using System;
using System.Linq;

public class NPCAnimationController : MonoBehaviour
{
    private Animator animator;
    [SerializeField] private float arousal;
    [SerializeField] private float valence;
    [SerializeField] private float intAr;
    [SerializeField] private float intVa;
    private float nowAr;
    private float nowVa;
    private float pastAr;
    private float pastVa;
    [SerializeField] private float destAr;
    [SerializeField] private float destVa;
    [SerializeField] private float coheAr = 0;
    [SerializeField] private float coheVa = 0;

    private int catA;
    private int catV;

    public float timeOut = 20f; // 閾値
    private float timeElapsed = 0.0f; // 累計時間
    private float speed = 0.5f;　// Parameterの遷移スピード
    private float _progress = 0f;
    SVMExecute svmE;
    NPCAnimationController ownAnimCon;
    [SerializeField] RuntimeAnimatorController[] animConArr;

    public int test_A = -1; // 仮
    public int test_V = -1; // 仮
    public bool isLegend;

    private GameObject testManager;
    private EnvParameterGenerate envParaGen;

    [SerializeField] public float sensitivity; // 感応度合い
    public float appeal; // 注目度，魅力
    public float radius = 3;



    private GameObject targetObject; // 注視したいオブジェクト

    private float lookAtWeight;
    private float bodyWeight;
    private float headWeight;
    private float eyesWeight;

    private bool isInitialize;

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
        //sensitivity = UnityEngine.Random.Range(0.0f, 1.0f);
        //sensitivity = 1.0f; // 一定にしたいとき

        //appeal = 1f;

        //svmE.Predict();

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

        //Invoke("Initialize", 16);
        //Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        if (envParaGen.count == 15 && isInitialize == false)
        {
            Initialize();
            //Execute();
        }
        else if (envParaGen.count == 17 && isInitialize == true)
        {
            getAroundAVValue();
            destAr = GetInterpValue(pastAr, coheAr);
            destVa = GetInterpValue(pastVa, coheVa);
            isInitialize = false;
        }
        else if (envParaGen.count > 19)
        {
            Execute();
        }
    }

    void Initialize()
    {
        svmE.Predict();

        // A-V値の遷移
        if (isLegend == false)
        { // 通常時
            catA = svmE.result_A;
            catV = svmE.result_V;
        }
        else
        {                 // 凡例時
            //test_A = envParaGen.test_catA; // とりあえず
            //test_V = envParaGen.test_catV; // とりあえず
            catA = test_A; // RealTimeGenerate.csで決められる on 2021/01/07
            catV = test_V;
        }
        intAr = GetParameterFromCategory(catA);
        intVa = GetParameterFromCategory(catV);
        animator.SetInteger("Cat_A", catA);
        animator.SetInteger("Cat_V", catV);
        animator.SetFloat("Arousal", intAr);
        animator.SetFloat("Valence", intVa);

        arousal = intAr;
        valence = intVa;

        pastAr = intAr;
        pastVa = intVa;
        /*
        getAroundAVValue();
        destAr = GetInterpValue(pastAr, coheAr);
        destVa = GetInterpValue(pastVa, coheVa);
        */

        isInitialize = true;
    }

    public void Execute()
    {
        
        // 一定間隔で実行する場合
        timeElapsed += Time.deltaTime;
        if (timeElapsed >= timeOut)
        {
            // 処理
            //svmE.Predict();
            //getAroundAVValue();
            //getAroundAVValue();
            timeElapsed = 0.0f;
        }
        

        //sensitivity = envParaGen.sensitivity; // 感応度合い
        //this.transform.LookAt(targetObject.transform);


        if (_progress > 1f)
        {
            _progress = 0f;
            pastAr = arousal;
            pastVa = valence;
            destAr = GetInterpValue(pastAr, coheAr);
            destVa = GetInterpValue(pastVa, coheVa);
            coheAr = 0;
            coheVa = 0;
            getAroundAVValue();
            if (this.name.Equals("girl(Clone)_18"))
            {
                Debug.Log("success!!");
            }
        }
        _progress = _progress + speed * Time.deltaTime;
        //Debug.Log("_progress: " + _progress);

        // A-V値の遷移
        /*
        if (isLegend == false)
        { // 通常時
            catA = svmE.result_A;
            catV = svmE.result_V;
        }
        else
        {                 // 凡例時
            //test_A = envParaGen.test_catA; // とりあえず
            //test_V = envParaGen.test_catV; // とりあえず
            catA = test_A;
            catV = test_V;
        }
        */
        arousal = Mathf.Lerp(pastAr, destAr, _progress);
        valence = Mathf.Lerp(pastVa, destVa, _progress);
        //animator.SetInteger("Cat_A", catA);
        //animator.SetInteger("Cat_V", catV);

        animator.SetFloat("Arousal", arousal);
        animator.SetFloat("Valence", valence);

    }

    float GetParameterFromCategory(int catg)
    {
        float boundary = 256.0f / 3.0f;
        float random = 0;
        if (catg == 0)
        {
            random = UnityEngine.Random.Range(0.0f, boundary);
        }
        else if (catg == 1)
        {
            random = UnityEngine.Random.Range(boundary, boundary * 2.0f);
        }
        else if (catg == 2)
        {
            random = UnityEngine.Random.Range(boundary * 2.0f, 256.0f);
        }
        return random;
    }

    public float GetInterpValue(float pastAr, float cohePara) // 補間値を乱数で取得
    {
        /*
        float boundary = 256.0f / 3.0f;
        float random = 0;
        if (catg == 0) {
            random = UnityEngine.Random.Range(0.0f, boundary);
        } else if (catg == 1) {
            random = UnityEngine.Random.Range(boundary, boundary * 2.0f);
        } else if (catg == 2) {
            random = UnityEngine.Random.Range(boundary * 2.0f, 256.0f);
        }
        */

        float MIN = 0.0f;
        float MAX = 256.0f;
        return Mathf.Clamp(pastAr - sensitivity * cohePara, MIN, MAX);
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
        Collider[] arrayCld = Physics.OverlapSphere(transform.position, 1.8f);
        List<GameObject> listObj = new List<GameObject>();
        // 検出したGameObjectの内、tagが"NPC"であるものをlistObjリストに追加
        foreach (Collider cld in arrayCld)
        {
            if (cld.tag == "NPC" && !cld.name.Equals(this.name))
            {
                listObj.Add(cld.gameObject);
            }
        }

        float sum = listObj.Count();
        float avgSum = 0f;
        //Debug.Log(sum);
        float[] avValue = new float[2];
        foreach (GameObject obj in listObj)
        {
            NPCAnimationController aroundAnimCon = obj.gameObject.GetComponent<NPCAnimationController>();
            avgSum += aroundAnimCon.appeal;
        }

        foreach (GameObject obj in listObj)
        {
            // 対象となるGameObjectとの距離を調べる
            float dist = Vector3.Distance(obj.transform.position, transform.position);
            // 対象となるGameObjectのA-V値を調べる
            NPCAnimationController aroundAnimCon = obj.gameObject.GetComponent<NPCAnimationController>();
            float diffAr = arousal - aroundAnimCon.Arousal;
            float diffVa = valence - aroundAnimCon.Valence;
            //SetAppeal(obj);

            coheAr += diffAr * aroundAnimCon.appeal / avgSum;
            coheVa += diffVa * aroundAnimCon.appeal / avgSum;
            //coheAr += aroundAnimCon.Arousal * aroundAnimCon.appeal / avgSum;
            //coheVa += aroundAnimCon.Valence * aroundAnimCon.appeal / avgSum;


            // 周囲の累計A-V値を更新
            //coheAr += calcValue(aroundAnimCon.appeal, diffAr, dist) / sum;
            //coheVa += calcValue(aroundAnimCon.appeal, diffVa, dist) / sum;

            if (this.name.Equals("girl(Clone)_19") && obj.name.Equals("girl(Clone)_27"))
            {
                Debug.Log(gameObject.name + ": obj=" + obj);
                Debug.Log(gameObject.name + ": own.Ar=" + arousal);
                Debug.Log(gameObject.name + ": obj.Ar=" + aroundAnimCon.Arousal);
                Debug.Log(gameObject.name + ": diffAr=" + diffAr);
                Debug.Log(gameObject.name + ": coheAr=" + coheAr);
            }
 

        }
        coheAr = coheAr / sum;
        coheVa = coheVa / sum;

        //Debug.Log(gameObject.name + ": coheAr=" + coheAr);
    }

    public float calcValue(float aroundAppeal, float diff, float dist)
    {
        //return diff * aroundAppeal * (float)Math.Exp(-1 / (1 + 6 * (sensitivity - 0.5)) * (double)dist);
        return diff * aroundAppeal * (float)Math.Exp(-1 / 25 * (double)dist);
    }

    /*
    void SetAppeal(GameObject obj)
    {
        string name = obj.name;
        if (name.Equals("girl_19") || name.Equals("girl_20"))
        {
            appeal = 1.0f;
        }
        else
        {
            appeal = 0.5f;
            //appeal = UnityEngine.Random.Range(0f, 0.6f);
        }
    }
    */

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
