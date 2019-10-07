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
    private Light[] lightsOnArray;
    private Light[] lightsOffArray;
    DestroyableObj _DestroyableObj;

    [SerializeField] private GameObject siren;
    [SerializeField] private GameObject siren2;
    [SerializeField] private AudioSource sparkAudioSource;
    [SerializeField] private GameObject cable;
    private Rigidbody cableBody;


    void Start()
    {
        blockingPlane.SetActive(true);
        _DestroyableObj = this.GetComponent<DestroyableObj>();
        lightsOnArray = lightOn.GetComponentsInChildren<Light>();
        lightsOffArray = lightOff.GetComponentsInChildren<Light>();
        cableBody = cable.GetComponent<Rigidbody>();

    }

    private void OpenBlock(){
        cableBody.isKinematic = false;
        cableBody.useGravity = true;
        cable.transform.parent = null;

        blockingPlane.SetActive(false);
        // Attempt at fade, comment in to try and get fade.
        Debug.Log(lightsOnArray.Length);
        foreach (Light child in lightsOnArray)
        {
            SoundManager.instance.StartCoroutine(FadeIn(child));

        }
        foreach (Light child in lightsOffArray)
        {
            SoundManager.instance.StartCoroutine(FadeOut(child));
        }
      
        if (siren != null)
        {
            siren.GetComponent<Siren>().Activate();
            siren2.GetComponent<Siren>().Activate();
        }
            
        // Turns sections of light on and off. Use this if script breaks.
        //lightOn.SetActive(true);
        //lightOff.SetActiveT(false);
        
    }

    void OnTriggerEnter(Collider other){
        if (other.tag == "gravity field" && GameManager.S.GetDestroyedScore() >= _DestroyableObj.GetThreshold()) {
            OpenBlock();
            SoundManager.instance.PlaySpark(sparkAudioSource);

            // If we want to disable gravity on all objects
            if (this.tag == "Disable Gravity") {
                GameManager.S.DisableGravity();
                SoundManager.instance.PlayExplosionNoises();
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
            lt.intensity += Time.deltaTime;
            // yield return new WaitForSeconds(interval);//the coroutine will wait for 0.2 secs
            yield return null;
        }
      
    }

    // Should fade lights out. 
    IEnumerator FadeOut(Light lt)
    {
        float interval = 0.1f;
        lt.intensity = 3.0f;
        while (lt.intensity >= 0.0f)
        {
            lt.intensity -= Time.deltaTime;
            // yield return new WaitForSeconds(interval);//the coroutine will wait for 0.2 secs
            yield return null;
        }
        yield return null;

    }
}
