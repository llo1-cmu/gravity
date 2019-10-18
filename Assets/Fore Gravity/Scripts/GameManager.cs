using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Valve.VR;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    #pragma warning disable 0649
    // Steam VR things
    private SteamVR_Input_Sources LeftInputSource = SteamVR_Input_Sources.LeftHand;
    private SteamVR_Input_Sources RightInputSource = SteamVR_Input_Sources.RightHand;

    // Scene Names
    [SerializeField] private string startScene = "Big Trash Room";
    [SerializeField] private string endScene = "2_Hexagon_Lisa";

    // Game Manager Vars
    public static GameManager S;
    [SerializeField] private int maxTier = 0;
    [SerializeField] private int currentTier = 0;
    private double tierFactor = 1;
    Dictionary<int, int> objectsPerTier;
    Dictionary<int, int> destroyedObjectsPerTier;
    private int destroyedObjects = 0;
    public int audioSourcesInstantiated = 0;

    public bool won = false;
    bool disableGravity = false;
    bool broadcastGravityDisabled = false;
    bool playingEnd = false;
    public bool FadeToBlackAfterEnding = false;

    [SerializeField] GameObject pobj;

    // GameObject Components
    // Player is currently just the right glove!
    [SerializeField] public GameObject player;
    // [SerializeField] private GameObject frisbee;
    // [SerializeField] private GameObject winScreen;
    // [SerializeField] private GameObject cameraRig;

    [SerializeField] private GameObject siren;

    [SerializeField] private Light [] lightList;
    public enum Reactor {Blue, Green, Orange}
    public static Reactor? currReactor = null;

    #pragma warning restore 0649

    void Awake(){
        S = this;
        objectsPerTier = new Dictionary<int, int>();
        destroyedObjectsPerTier = new Dictionary<int, int>();
    }

    void Start(){
        SteamVR_Fade.Start(Color.black, 0);
        SteamVR_Fade.Start(Color.clear, 2);
    }

    public void AddTieredObject(int tier){
        // Update our stats for tiered values
        int currVal = 0;
        if (! (objectsPerTier.TryGetValue(tier, out currVal))) {
            objectsPerTier.Add(tier, 1);
            destroyedObjectsPerTier.Add(tier, 0);
        } else {
            objectsPerTier.Remove(tier);
            objectsPerTier.Add(tier, currVal+1);
        }

        if (tier > maxTier) {
            maxTier = tier;
            tierFactor = Math.Pow(2, -maxTier);
        }
    }

    public int GetCurrentTier() {
        return currentTier;
    }

    public int GetMaxTier() {
        return maxTier;
    }

    public void UpdateDestroyedScore(int tier) {
        destroyedObjects += 1;
        
        // Update our destroyed values for the given tier (only care for objects
        // in the current tier)
        if (tier == currentTier) {
            int currDestroyed = 0;
            if (!(destroyedObjectsPerTier.TryGetValue(tier, out currDestroyed))) {
                //TODO: this shouldn't happen, do some error handling
                destroyedObjectsPerTier.Add(tier, 1);
            }
            currDestroyed += 1;
            destroyedObjectsPerTier.Remove(tier);
            destroyedObjectsPerTier.Add(tier, currDestroyed);
            int objectsInTier = 0;
            // TODO: error handling
            objectsPerTier.TryGetValue(currentTier, out objectsInTier);

            // If the objects destroyed in the CURRENT TIER is less than the max
            if (currDestroyed >= objectsInTier * tierFactor) {
                currentTier++;
                if (currentTier < maxTier) tierFactor *= 2;
            }
        }


        // If we've destroyed an object after calling disable gravity in HexRoom
        if (disableGravity) {
            disableGravity = false;
            //Play voice line for disable gravity
            broadcastGravityDisabled = true;
        }
    }

    public int GetDestroyedScore() {
        return destroyedObjects;
    }

    public void DisableGravity() {
        // disableGravity = true;
        // StartCoroutine(PowerDownGravity());
        BroadcastGravityDisabledTemp(true);
    }

    public void BroadcastGravityDisabledTemp(bool b) {
        broadcastGravityDisabled = b;
    }

    public bool GetBroadcastGravityDisabled() {
        return broadcastGravityDisabled;
    }

    void Update() {
        if(SteamVR_Actions._default.GrabGrip.GetState(RightInputSource) && 
            SteamVR_Actions._default.GrabGrip.GetState(LeftInputSource)) {
            // If someone holds both grips, reset the game
            destroyedObjects = 0; // reset the stats
            won = false;
            SceneManager.LoadScene(startScene);
        }
        if ((currentTier > maxTier && !won)) {
            if (SceneManager.GetActiveScene().name == startScene) {
                pobj.SetActive(true);
                StartCoroutine(fadeInAndOutTimes(lightList, 1f, 3f, 3));
                SoundManager.instance.PlayTrashFinish();
                siren.GetComponent<Siren>().Activate();
                won = true;
                currReactor = Reactor.Orange;
                StartCoroutine(blackOutScreen());
            }
            else /* we want ending to play */ {
                if (!playingEnd) {
                    Debug.Log(maxTier);
                    SoundManager.instance.PlayEnding();
                    playingEnd = true;
                    won = true;
                    StartCoroutine(blackOutScreen());
                }
            }
        }
    }

    // IEnumerator PowerDownGravity() {
    //     // float f = SoundManager.instance.PlayGravityPoweringDown();
    //     // yield return new WaitForSeconds(f);
    //     BroadcastGravityDisabledTemp(true);
    // }
      IEnumerator fadeInAndOut(Light [] lightToFade, bool fadeIn, float duration)
    {
        float minLuminosity = 0.6f; // min intensity
        float maxLuminosity = 1; // max intensity

        float counter = 0f;

        //Set Values depending on if fadeIn or fadeOut
        float a, b;

        if (fadeIn)
        {
            a = minLuminosity;
            b = maxLuminosity;
        }
        else
        {
            a = maxLuminosity;
            b = minLuminosity;
        }

        //float currentIntensity = lightToFade.intensity;

        while (counter < duration)
        {
            counter += Time.deltaTime;
            foreach (Light l in lightToFade)
                l.intensity = Mathf.Lerp(a, b, counter / duration);

            yield return null;
        }
    }
    IEnumerator fadeInAndOutTimes(Light [] lightToFade, float duration, float waitTime, int timesRun)
    {
        WaitForSeconds waitForXSec = new WaitForSeconds(waitTime);

        while (timesRun>0)
        {
            //Fade out

            yield return fadeInAndOut(lightToFade, false, duration);

            //Wait
            yield return waitForXSec;

            //Fade-in 
            yield return fadeInAndOut(lightToFade, true, duration);

			timesRun -= 1;
        }

    }
    IEnumerator blackOutScreen(){
        // Tutorial.PlayWarning();
        //black out screen
        if (SceneManager.GetActiveScene().name == endScene) {
            while (!FadeToBlackAfterEnding) {
                yield return null;
            }
            // yield return new WaitForSeconds(5);
            SteamVR_Fade.Start(Color.black, 5.0f);
            yield return new WaitForSeconds(5.0f);
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
        }
        else {
            yield return new WaitForSeconds(10);
            SteamVR_Fade.Start(Color.black, 7.0f);
            yield return new WaitForSeconds(8.0f);
            //GetComponent<SteamVR_LoadLevel>().levelName = "Hexagon";
            //GetComponent<SteamVR_LoadLevel>().Trigger();
            // yield return new WaitForSeconds(7);
            Application.backgroundLoadingPriority = ThreadPriority.Low;
            AsyncOperation AO = SceneManager.LoadSceneAsync(endScene); 
            AO.allowSceneActivation = false;

            //frisbee.transform.position = frisbeePlaceToMove;
            //cameraRig.transform.position = placeToMove;
            //SceneManager.LoadScene("Hexagon");
            //SteamVR_Fade.Start(Color.clear, 2.0f);
            
            while(AO.progress < 0.9f)
            {
                yield return null;
            }
            AO.allowSceneActivation = true;
            destroyedObjects = 0;
            won = false;   
            disableGravity = false;
            broadcastGravityDisabled = false;

            //SceneManager.UnloadSceneAsync(startScene);
        }
    }
}