using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class button01 : MonoBehaviour
{
    public GameObject target;
    private riseCommander tgtScript;

    public Material mat;
    
    void Start()
    {
        tgtScript = target.GetComponent<riseCommander>();
    }

    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag == "Player"){
            Destroy(gameObject);
            tgtScript.active = true;
        }
    }
    
    /*void OnPostRender()
    {
        GL.PushMatrix();
        mat.SetPass(0);
        GL.LoadPixelMatrix();

        GL.Begin(GL.LINES);
        GL.Color(Color.red);
        GL.Vertex(target.transform.position);
        GL.Vertex(gameObject.transform.position);
        GL.End();

        GL.PopMatrix();
    }
    */ //too late to figure this out.. don't know what i'm doing and cannot ask
}
