using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class endManager : MonoBehaviour
{
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space)) //press space to continue
        {
            collectibleManager.collectedBits = 0;
            SceneManager.LoadScene("game", LoadSceneMode.Single);
        }
    }
}
