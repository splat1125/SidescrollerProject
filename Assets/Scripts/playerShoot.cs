using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerShoot : MonoBehaviour
{
    public GameObject beam;
    public float shootSpeed;
    public float shootLifetime;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space)){
            GameObject newBeam = Instantiate(beam, transform.position, transform.rotation);
            newBeam.transform.SetParent(gameObject.transform);
            newBeam.transform.localPosition = new Vector2(0, -0.2f);
            float dir = 1;
            if(playerBehavior.faceRight){
                dir = -1;
                newBeam.GetComponent<SpriteRenderer>().flipX = true;
            } else{
                dir = 1;
                newBeam.GetComponent<SpriteRenderer>().flipX = false;
            }
            newBeam.GetComponent<Rigidbody2D>().velocity = new Vector3(dir * shootSpeed, newBeam.transform.localPosition.y);
            newBeam.GetComponent<beamBehavior>().lifetime = shootLifetime;
        }
    }
}
