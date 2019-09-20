using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    [SerializeField] AudioSource shipAudioSource, frisbeeAudioSource;
    [SerializeField] AudioClip initialWarning, frisbeeCalling, frisbeeCaught, frisbeeAffirm;
    private static Tutorial instance;
    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public static bool IsTutorial(){
        return instance != null ? true : false;
    }
    public static void PlayIntro(){
        instance.shipAudioSource.PlayOneShot(instance.initialWarning);
    }
    public static void PlayFrisbeePrompt(){
        instance.frisbeeAudioSource.PlayOneShot(instance.frisbeeCalling);
    }
    public static void PlayFrisbeeCaught(){
        instance.frisbeeAudioSource.PlayOneShot(instance.frisbeeCaught);
    }
    public static void PlaySphereHit(){
        instance.frisbeeAudioSource.PlayOneShot(instance.frisbeeAffirm);
    }
}
