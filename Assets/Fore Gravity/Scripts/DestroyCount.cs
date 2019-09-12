using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DestroyCount : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        this.GetComponent<Text>().text = "Destroyed: " + 0;
    }

    // Update is called once per frame
    void Update()
    {
        this.GetComponent<Text>().text = "Destroyed: " + GameManager.S.GetDestroyedScore();
    }
}
