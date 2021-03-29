using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class gameEnder : MonoBehaviour
{  
    void OnTriggerStay2D(Collider2D other)
    {
        if(other.gameObject.tag == "Player"){
            SceneManager.LoadScene("end", LoadSceneMode.Single);
        }
    }
}
