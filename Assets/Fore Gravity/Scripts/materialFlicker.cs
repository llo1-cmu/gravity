using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class materialFlicker : MonoBehaviour
{
    //private Material[] materials;
    //[SerializeField] private MeshRenderer imageColor;
    [SerializeField] private Material [] blueMaterial;
    private List<Color> colors;
    
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(fadeInAndOutRepeat(.5f, 4f));
        foreach (Material mat in blueMaterial)
        {
            Color colour = mat.GetColor("_EmissionColor");
            colors.Add(colour);

        }

    // Update is called once per frame
    void Update()
    {

    }

    void OnDestroy()
    {
            for (int i = 0; i < blueMaterial.Length; i++)
            {
                blueMaterial[i].SetColor("_EmissionColor", colors[i]);
            }

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

            //Wait
            yield return waitForXSec;

            //Fade-in 
            yield return fadeInAndOut(true);

            yield return waitForXSec2;
        }
    }

   
}
