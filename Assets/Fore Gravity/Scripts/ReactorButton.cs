using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DestroyableObj))]
public class ReactorButton : MonoBehaviour
{
    // Assign doors in the inspector
    [SerializeField] private GameObject blockingPlane;
    [SerializeField] private Transform lightOnT;
    [SerializeField] private Transform lightOffT;
    [SerializeField] private GameObject lightOn;
    [SerializeField] private GameObject lightOff;

    DestroyableObj _DestroyableObj;
    void Start()
    {
        blockingPlane.SetActive(true);
        _DestroyableObj = this.GetComponent<DestroyableObj>();
    }

    private void OpenBlock(){
        blockingPlane.SetActive(false);
        // Attempt at fade, comment in to try and get fade.
        /* foreach (Light child in lightOnT)
        {
            FadeIn(child);
        }
        foreach (Light child in lightOffT)
        {
            FadeOut(child);
        }*/
        // Turns sections of light on and off. Use this if script breaks.
        lightOn.SetActive(true);
        lightOff.SetActive(false);
        
    }

    void OnTriggerEnter(Collider other){
        if (other.tag == "gravity field" && GameManager.S.GetDestroyedScore() >= _DestroyableObj.GetThreshold()) {
            OpenBlock();

            // If we want to disable gravity on all objects
            if (this.tag == "Disable Gravity") {
                GameManager.S.DisableGravity();
            }
        }
    }

    // Should fade lights in.
    IEnumerator FadeIn(Light lt)
    {
       
        float interval = 0.1f; //interval time between iterations of while loop
        lt.intensity = 0.0f;
        while (lt.intensity <= 3.0f)
        {
            lt.intensity += 0.02f;
            yield return new WaitForSeconds(interval);//the coroutine will wait for 0.2 secs
        }
    }

    // Should fade lights out. 
    IEnumerable FadeOut(Light lt)
    {
        float interval = 0.1f;
        lt.intensity = 3.0f;
        while (lt.intensity >= 0.0f)
        {
            lt.intensity -= 0.02f;
            yield return new WaitForSeconds(interval);//the coroutine will wait for 0.2 secs
        }

    }
}
