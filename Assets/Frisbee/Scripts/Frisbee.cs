using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class Frisbee : MonoBehaviour
{
    private Transform attachedController;
    private SteamVR_Input_Sources LeftInputSource = SteamVR_Input_Sources.LeftHand;
    private SteamVR_Input_Sources RightInputSource = SteamVR_Input_Sources.RightHand;
    private Queue<Vector3> positionRecord;
    // Start is called before the first frame update
    void Start()
    {
        positionRecord = new Queue<Vector3>(21);
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("Right Squeeze value:" + SteamVR_Actions._default.Squeeze.GetAxis(RightInputSource).ToString());
        //Debug.Log("Right GrabPinch value:" + SteamVR_Actions._default.GrabPinch.GetState(RightInputSource).ToString());
    }
    /*void OnTriggerEnter(Collider other){
        if(other.tag == "VR Controller"){
            Debug.Log("Controller Entered");
        }
    }
    void OnTriggerExit(Collider other){
        if(other.tag == "VR Controller"){
            Debug.Log("Controller Exited");
        }
    }*/
    void OnTriggerStay(Collider other){
        if(other.tag == "VR Controller"){
            if(SteamVR_Actions._default.GrabPinch.GetState(RightInputSource)){
                transform.position = other.transform.position + (Vector3.Scale(other.transform.forward, new Vector3(1.0f, 0.0f, 1.0f)) * 0.2f);
                positionRecord.Enqueue(transform.position);
                if(positionRecord.Count > 20){
                    positionRecord.Dequeue();
                }
            }
            else{
                if(positionRecord.Count > 0){
                    Vector3 oldPosition = positionRecord.Dequeue();
                    positionRecord.Clear();
                    Vector3 forceVector = transform.position - oldPosition;
                    GetComponent<Rigidbody>().AddForce(Vector3.Scale(forceVector, new Vector3(1.0f, 0.0f, 1.0f)) * 500.0f);
                }
            }
        }
    }

}
