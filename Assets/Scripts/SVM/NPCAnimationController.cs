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
    [SerializeField] private int catAr;
    [SerializeField] private int catVa;
    private int pastCatA;
    private int pastCatV;
    private float TRANSITION_SPEED = 0.5f; // Parameterの遷移スピード
    [SerializeField] private float _progress = 0f;
    [SerializeField] private int cycle = 0;
    SVMExecute svmExecute;
    NPCAnimationController ownAnimCon;
    [SerializeField] RuntimeAnimatorController[] animConArr;
    public int catArTest = -1; // 仮
    public int catVaTest = -1; // 仮
    public bool isTest;
    private GameObject testManager;
    private EnvParameterGenerate envParaGen;
    [SerializeField] public float sensitivity; // 感応度合い
    public float appeal; // 注目度，魅力
    private const float SEARCH_RADIUS = 1.5f;
    private GameObject targetObject; // 注視したいオブジェクト
    private float lookAtWeight;
    private float bodyWeight;
    private float headWeight;
    private float eyesWeight;
    private bool isInitialize;
    private const double LAMBDA = 1 / 25;
    private bool isOnlyWave = true;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        svmExecute = GetComponent<SVMExecute>();
        ownAnimCon = gameObject.GetComponent<NPCAnimationController>();
        //svmExecute.Predict();
        checkName(this.gameObject);
        //Execute();

        testManager = GameObject.Find("TestManager");
        envParaGen = testManager.GetComponent<EnvParameterGenerate>();

        targetObject = GameObject.Find("TargetObject");



        //svmExecute.Predict();

        // 左利きにチェンジ．左利きの人の割合は10%らしい
        if (UnityEngine.Random.value > 0.9) 
            animator.runtimeAnimatorController = animConArr[0];  // SVM_Mirror
        else
            animator.runtimeAnimatorController = animConArr[1];  // SVM

        if (isOnlyWave)
            if (UnityEngine.Random.value > 0.9)
                animator.runtimeAnimatorController = animConArr[2];  // SVM_wave_mirror
            else
                animator.runtimeAnimatorController = animConArr[3];  // SVM_wave

        //Invoke("Initialize", 16);
        //Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        if (envParaGen.count == 15 && isInitialize == false)
        {
            //////////////////////////////////////////////////////////
            // 環境パラメタ取得 & A-Vカテゴリ決定 & A-V値決定 & 動作選定
            Initialize();
            //Execute();
        }
        else if (envParaGen.count == 17 && isInitialize == true)
        {
            //////////////////////////////////////////////////////////
            // A-Vコヒーレンスパラメタ取得
            getAroundAVValue();
            //////////////////////////////////////////////////////////
            // 収束A-V値決定
            destAr = GetTerminalValue(pastAr, coheAr);
            destVa = GetTerminalValue(pastVa, coheVa);
            isInitialize = false;
        }
        else if (envParaGen.count > 19)
        {
            Execute();
        }
    }

    void Initialize()
    {
        //////////////////////////////////////////////////////////
        // 環境パラメタ取得 & 初期A-Vカテゴリ決定
        svmExecute.Predict();

        // A-V値の遷移
        if (isTest == false)
        { // 通常時
            catAr = svmExecute.catArResult;
            catVa = svmExecute.catVaResult;
        }
        else
        {                 // 凡例時
            catAr = catArTest;  // catArTest: RealTimeGenerate.csで決められる
            catVa = catVaTest;
        }

        //////////////////////////////////////////////////////////
        // 初期A-V値決定
        intAr = GetRandomValueFromCategory(catAr);
        intVa = GetRandomValueFromCategory(catVa);
        animator.SetInteger("Cat_A", catAr);
        animator.SetInteger("Cat_V", catVa);
        animator.SetFloat("Arousal", intAr);
        animator.SetFloat("Valence", intVa);

        arousal = intAr;
        valence = intVa;
        pastAr = intAr;
        pastVa = intVa;
        pastCatA = catAr;
        pastCatV = catVa;

        //////////////////////////////////////////////////////////
        // 動作選定
        //////////////////////////////////////////////////////////
        // ステートマシン上で実行

        isInitialize = true;
    }

    public void Execute()
    {
        // 一定間隔で実行する場合
        if (_progress > 1f)
        {
            _progress = 0f;
            pastAr = arousal;
            pastVa = valence;
            destAr = GetTerminalValue(pastAr, coheAr);
            destVa = GetTerminalValue(pastVa, coheVa);
            cycle++;

            if (catAr != GetCategoryFromValue(arousal)) {  // Arousalカテゴリに変更があれば
                catAr = GetCategoryFromValue(arousal);
                animator.SetInteger("Cat_A", catAr);
            }
            if (catVa != GetCategoryFromValue(valence)) {  // Valenceカテゴリに変更があれば
                catVa = GetCategoryFromValue(valence);
                animator.SetInteger("Cat_V", catVa);
            }

            if (this.name.Equals("girl(Clone)_19"))
                Debug.Log("cycle: " + cycle);

            //////////////////////////////////////////////////////////
            // A-Vコヒーレンスパラメタ取得
            getAroundAVValue();

            if (this.name.Equals("girl(Clone)_18"))
                Debug.Log("success!!");
        }
        _progress += TRANSITION_SPEED * Time.deltaTime;

        arousal = Mathf.Lerp(pastAr, destAr, _progress);
        valence = Mathf.Lerp(pastVa, destVa, _progress);
        animator.SetFloat("Arousal", arousal);
        animator.SetFloat("Valence", valence);
    }

    float GetRandomValueFromCategory(int catg)
    {
        float boundary = 256.0f / 3.0f;
        float random = 0;
        if (catg == 0)
            random = UnityEngine.Random.Range(0.0f, boundary);
        else if (catg == 1)
            random = UnityEngine.Random.Range(boundary, boundary * 2.0f);
        else if (catg == 2)
            random = UnityEngine.Random.Range(boundary * 2.0f, 256.0f);
        return random;
    }

    public float GetTerminalValue(float pastAr, float coheParam)
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
        return Mathf.Clamp(pastAr - sensitivity * coheParam, MIN, MAX);
    }

    public void checkName(GameObject obj)
    {
        if (obj.name == "Ch20_nonPBR_Legend") {
            catArTest = 0;
            catVaTest = 0;
        } else if (obj.name == "Ch20_nonPBR_Legend (1)") {
            catArTest = 0;
            catVaTest = 1;
        } else if (obj.name == "Ch20_nonPBR_Legend (2)") {
            catArTest = 0;
            catVaTest = 2;
        } else if (obj.name == "Ch20_nonPBR_Legend (3)") {
            catArTest = 1;
            catVaTest = 0;
        } else if (obj.name == "Ch20_nonPBR_Legend (4)") {
            catArTest = 1;
            catVaTest = 1;
        } else if (obj.name == "Ch20_nonPBR_Legend (5)") {
            catArTest = 1;
            catVaTest = 2;
        } else if (obj.name == "Ch20_nonPBR_Legend (6)") {
            catArTest = 2;
            catVaTest = 0;
        } else if (obj.name == "Ch20_nonPBR_Legend (7)") {
            catArTest = 2;
            catVaTest = 1;
        } else if (obj.name == "Ch20_nonPBR_Legend (8)") {
            catArTest = 2;
            catVaTest = 2;
        }
    }

    public void getAroundAVValue()
    {
        coheAr = 0;
        coheVa = 0;

        // 中心:自分の位置, 半径:SEARCH_RADIUSの球内に存在するものを検出
        Collider[] arrayCld = Physics.OverlapSphere(transform.position, SEARCH_RADIUS);
        List<GameObject> listObj = new List<GameObject>();
        // 検出したGameObjectの内、tagが"NPC"であるものをlistObjリストに追加
        foreach (Collider cld in arrayCld)
            if (cld.tag == "NPC" && !cld.name.Equals(this.name))
                listObj.Add(cld.gameObject);

        float avgSum = 0f;
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
            // A-Vコヒーレンスパラメタを更新
            coheAr += diffAr * calculateDistanceAttenuation(dist) * aroundAnimCon.appeal / avgSum;
            coheVa += diffVa * calculateDistanceAttenuation(dist) * aroundAnimCon.appeal / avgSum;

            //if (this.name.Equals("girl(Clone)_19") && obj.name.Equals("girl(Clone)_27"))
            //{
            //    Debug.Log(gameObject.name + ": obj=" + obj);
            //    Debug.Log(gameObject.name + ": own.Ar=" + arousal);
            //    Debug.Log(gameObject.name + ": obj.Ar=" + aroundAnimCon.Arousal);
            //    Debug.Log(gameObject.name + ": diffAr=" + diffAr);
            //    Debug.Log(gameObject.name + ": coheAr=" + coheAr);
            //}
        }
        coheAr = coheAr / listObj.Count();
        coheVa = coheVa / listObj.Count();
    }

    public float calculateDistanceAttenuation(float dist)
    {
        //return diff * aroundAppeal * (float)Math.Exp(-1 / (1 + 6 * (sensitivity - 0.5)) * (double)dist);
        //return diff * aroundAppeal * (float)Math.Exp(-1 / 25 * ((double)dist - 1));
        return (float)Math.Exp(-1 * LAMBDA * ((double)dist - 1));
    }

    private int GetCategoryFromValue(float value)
    {
        float boundary = 256.0f / 3.0f;
        if (0.0f <= value && value < boundary)
            return 0;
        else if (boundary <= value && value < boundary * 2.0f)
            return 1;
        else if (boundary * 2.0f <= value && value <= 256.0f)
            return 2;
        else
            return -99;
    }

    public bool LookAtObject()
    {
        float threshold_value = 0;
        if (catAr == 1 && catVa == 1)
        {
            threshold_value = 0.4f;
        }
        else if (catAr == 1 && catVa == 2)
        {
            threshold_value = 0.6f;
        }
        else if (catAr == 2 && catVa == 1)
        {
            threshold_value = 0.6f;
        }
        else if (catAr == 2 && catVa == 2)
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
