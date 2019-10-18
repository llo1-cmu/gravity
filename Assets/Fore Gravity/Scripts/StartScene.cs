using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class StartScene : MonoBehaviour
{
    private SteamVR_Input_Sources LeftInputSource = SteamVR_Input_Sources.LeftHand;
    private SteamVR_Input_Sources RightInputSource = SteamVR_Input_Sources.RightHand;
    private bool cooldown;
    [SerializeField] private SteamVR_Action_Vibration vibration;
    [SerializeField] private string secondScene = "Big Trash Room";
    [SerializeField] private Transform player, center;
    [SerializeField] private float maxDistance;
    [SerializeField] private AudioSource source, padSource;
    [SerializeField] private AudioClip introClip, transportClip, warningClip;
    private float repeatTime = 15;

    private float currTime = 15;
    private bool checkTime = true;
    private bool won = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        currTime += Time.deltaTime;
        if (currTime >= repeatTime && checkTime) {
            currTime = 0;
            source.PlayOneShot(introClip);
        }
        if(SteamVR_Actions._default.GrabPinch.GetState(RightInputSource) ||
            SteamVR_Actions._default.GrabPinch.GetState(LeftInputSource) && !cooldown) {
            
            if(IsCloseEnough() && !won){
                won = true;
                checkTime = false;
                source.Stop();
                padSource.Stop();
                padSource.PlayOneShot(transportClip);
                //UnityEngine.SceneManagement.SceneManager.LoadScene(secondScene);
                //print("success!");

                //Don't end cooldown in transition
                cooldown = true;
                StartCoroutine(blackOutScreen());
            }
            else{
                if(SteamVR_Actions._default.GrabPinch.GetState(RightInputSource)){
                    StartCoroutine(Vibrate(RightInputSource));
                }
                else{
                    StartCoroutine(Vibrate(LeftInputSource));
                }
                cooldown = true;
                StartCoroutine(Cooldown());
            }
        }
    }
    bool IsCloseEnough(){
        float xDist = Mathf.Pow(player.position.x - center.position.x, 2);
        float zDist = Mathf.Pow(player.position.z - center.position.z, 2);
        //print(xDist.ToString("f2") + " " + zDist.ToString("f2") + " " + Mathf.Pow(maxDistance, 2).ToString("f2"));
        if(xDist + zDist < Mathf.Pow(maxDistance, 2)){
            return true;
        }
        else{
            return false;
        }
    }
    IEnumerator Vibrate(SteamVR_Input_Sources hand){
        vibration.Execute(0.0f, 0.05f, 80.0f, 0.5f, hand);
        yield return new WaitForSeconds(0.1f);
        vibration.Execute(0.0f, 0.05f, 80.0f, 0.5f, hand);
    }
    IEnumerator Cooldown(){
        yield return new WaitForSeconds(1f);
        cooldown = false;
    }
    IEnumerator blackOutScreen(){
        source.PlayOneShot(warningClip);
        SteamVR_Fade.Start(Color.black, 5.0f);
        yield return new WaitForSeconds(Mathf.Max(5.0f, warningClip.length));
        Application.backgroundLoadingPriority = ThreadPriority.Low;
        AsyncOperation AO = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(secondScene); 
        AO.allowSceneActivation = false;
        while(AO.progress < 0.9f)
        {
            yield return null;
        }
        AO.allowSceneActivation = true;
    }
}
