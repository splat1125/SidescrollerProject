using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class collectibleManager : MonoBehaviour
{
    public Text colText;
    public static int collectedBits;

    void Start()
    {
        colText.text = "0";
    }

    void Update()
    {
        colText.text = (""+collectedBits);
    }
}
