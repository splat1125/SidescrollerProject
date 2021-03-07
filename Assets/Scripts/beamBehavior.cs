using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class beamBehavior : MonoBehaviour
{
    public float lifetime;
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    // Update is called once per frame
    void Update()
    {

    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag == "Enemy"){
            Destroy(other.gameObject);
            Destroy(gameObject);
        }
    }
}
