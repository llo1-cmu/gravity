using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class RecallHint : MonoBehaviour
{
    [SerializeField] private GameObject rightHandHint, leftHandHint;
    [SerializeField] private GameObject mainCamera;
    [SerializeField] private GameObject rightHandObject, leftHandObject;
    private SteamVR_Input_Sources LeftInputSource = SteamVR_Input_Sources.LeftHand;
    private SteamVR_Input_Sources RightInputSource = SteamVR_Input_Sources.RightHand;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(!(SteamVR_Actions._default.GrabPinch.GetState(RightInputSource))
            && Vector3.Angle(mainCamera.transform.forward, rightHandObject.transform.position - mainCamera.transform.position) < 15.0f){
            rightHandHint.SetActive(true);
        }
        else{
            rightHandHint.SetActive(false);
        }

        if(!(SteamVR_Actions._default.GrabPinch.GetState(LeftInputSource))
            && Vector3.Angle(mainCamera.transform.forward, leftHandObject.transform.position - mainCamera.transform.position) < 15.0f){
            leftHandObject.SetActive(true);
        }
        else{
            leftHandObject.SetActive(false);
        }
    }
}
