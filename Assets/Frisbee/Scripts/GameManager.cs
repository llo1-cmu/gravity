using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //TODO: add frisbee button pushes here
    private SteamVR_Input_Sources LeftInputSource = SteamVR_Input_Sources.LeftHand;
    private SteamVR_Input_Sources RightInputSource = SteamVR_Input_Sources.RightHand;
    [SerializeField] private string sceneName;

    void Update() {
        // If someone presses the grip, reset the game
         if(SteamVR_Actions._default.GrabGrip.GetState(RightInputSource)){
             SceneManager.LoadScene(sceneName);
         }
    }
}