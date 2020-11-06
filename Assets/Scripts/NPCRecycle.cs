using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCRecycle : MonoBehaviour
{
    public GameObject manager;
    private RealTimeGenerate rtg;
    // Start is called before the first frame update
    void Start()
    {
        rtg = manager.GetComponent<RealTimeGenerate>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        Destroy(other.gameObject);
        rtg.Rebirth();
    }
}
