using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class materialFlicker : MonoBehaviour
{
    //private Material[] materials;
    //[SerializeField] private MeshRenderer imageColor;
    [SerializeField] private Material [] blueMaterial;
    bool status;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(fadeInAndOutRepeat(.5f, 4f));
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnDestroy()
    {
        if (!status)
            fadeInAndOut(true);
        else
            fadeInAndOut(false);

    }



    IEnumerator fadeInAndOut(bool on)
    {
        
            foreach (Material mat in blueMaterial)
            {
                Color colour = mat.GetColor("_EmissionColor");
                if (on)
                {
                    colour *= 2.0f;
                }
                else
                {
                    colour /= 2.0f;
                }
                mat.SetColor("_EmissionColor", colour);
         
            }
        yield return null;
      
    }

    IEnumerator fadeInAndOutRepeat(float waitTime, float waitTime2)
    {
        float w1 = Random.Range(0f, waitTime);
        float w2 = Random.Range(1f, waitTime2);
        WaitForSeconds waitForXSec = new WaitForSeconds(w1);
        WaitForSeconds waitForXSec2 = new WaitForSeconds(w2);

        while (true)
        {
            //Fade out
            yield return fadeInAndOut(false);
            status = false;

            //Wait
            yield return waitForXSec;

            //Fade-in 
            yield return fadeInAndOut(true);
            status = true;

            yield return waitForXSec2;
        }
    }

   
}
