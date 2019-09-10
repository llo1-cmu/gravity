using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private SteamVR_Input_Sources LeftInputSource = SteamVR_Input_Sources.LeftHand;
    private SteamVR_Input_Sources RightInputSource = SteamVR_Input_Sources.RightHand;
    [SerializeField] private string sceneName = "Empty Room";

    void Update() {
        if(SteamVR_Actions._default.GrabGrip.GetState(RightInputSource) && 
            SteamVR_Actions._default.GrabGrip.GetState(LeftInputSource)) {
            // If someone holds both grips, reset the game
            SceneManager.LoadScene(sceneName);
        }
    }
}