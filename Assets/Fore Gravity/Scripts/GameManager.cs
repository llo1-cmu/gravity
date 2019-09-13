using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private SteamVR_Input_Sources LeftInputSource = SteamVR_Input_Sources.LeftHand;
    private SteamVR_Input_Sources RightInputSource = SteamVR_Input_Sources.RightHand;
    [SerializeField] private string startScene = "Empty Room";

    public static GameManager S;
    private static int destroyedObjects = 0;
    // TODO: auto populate this by having destroyable objs send mssg to gm
    private static int totalObjectsToWin = 4;
    [SerializeField] private GameObject winScreen = null;

    void Start(){
        S = this;
    }

    public void UpdateDestroyedScore() {
        destroyedObjects++;
    }

    public int GetDestroyedScore() {
        return destroyedObjects;
    }

    public void LoadScene(string sceneName){
        winScreen.SetActive(false);
        Debug.Log("false");
        SceneManager.LoadScene(sceneName);

        //TODO: create resets function
        destroyedObjects = 0;
    }

    void Update() {
        if(SteamVR_Actions._default.GrabGrip.GetState(RightInputSource) && 
            SteamVR_Actions._default.GrabGrip.GetState(LeftInputSource)) {
            // If someone holds both grips, reset the game
            destroyedObjects = 0; // reset the stats
            SceneManager.LoadScene(startScene);
        }

        if (destroyedObjects >= totalObjectsToWin) {
            // TODO: enable this if you guys decide on canvas screen!
            //winScreen.SetActive(true);
        }
    }
}