using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class button01 : MonoBehaviour
{
    public GameObject target;
    private riseCommander tgtScript;
    
    void Start(){
        tgtScript = target.GetComponent<riseCommander>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag == "Player"){
            Destroy(gameObject);
            tgtScript.active = true;
        }
    }
}
