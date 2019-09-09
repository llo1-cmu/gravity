using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class Frisbee : MonoBehaviour
{
    [SerializeField] private Transform leftController, rightController;
    private Transform attachedController;
    private SteamVR_Input_Sources LeftInputSource = SteamVR_Input_Sources.LeftHand;
    private SteamVR_Input_Sources RightInputSource = SteamVR_Input_Sources.RightHand;
    private Queue<Vector3> positionRecord;
    private new Rigidbody rigidbody;
    private bool recalling;
    private Vector3 recallStartPosition;
    private float recallStartTime; 
    void Start()
    {
        positionRecord = new Queue<Vector3>(21);
        rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("Right Squeeze value:" + SteamVR_Actions._default.Squeeze.GetAxis(RightInputSource).ToString());
        //Debug.Log("Right GrabPinch value:" + SteamVR_Actions._default.GrabPinch.GetState(RightInputSource).ToString());
        //Debug.Log("Right Trackpad value:" + SteamVR_Actions._default.Teleport.GetState(RightInputSource).ToString());
        if((SteamVR_Actions._default.GrabPinch.GetState(RightInputSource) || SteamVR_Actions._default.GrabPinch.GetState(LeftInputSource)) && attachedController == null){
            // Trigger pulled when frisbee not on hand => Recall 
            if(SteamVR_Actions._default.GrabPinch.GetState(RightInputSource)){
                attachedController = rightController;
            }
            else{
                attachedController = leftController;
            }
            // Initialize the recall
            rigidbody.useGravity = false;
            recalling = true;
            recallStartPosition = transform.position;
            recallStartTime = Time.time;
            // Clear velocity
            rigidbody.velocity = Vector3.zero;
        }
        else if(recalling && (SteamVR_Actions._default.GrabPinch.GetState(RightInputSource) || SteamVR_Actions._default.GrabPinch.GetState(LeftInputSource))){
            // Non-physical recall
            rigidbody.MovePosition(Vector3.Lerp(recallStartPosition, attachedController.position, (Time.time - recallStartTime)/0.2f ));
        }
        else if(recalling && !(SteamVR_Actions._default.GrabPinch.GetState(RightInputSource) || SteamVR_Actions._default.GrabPinch.GetState(LeftInputSource))){
            // Recall cancelled (dropped)
            recalling = false;
            rigidbody.useGravity = true;
            attachedController = null;
        }
    }
    void OnTriggerEnter(Collider other){
        if (other.tag == "Button") {
            ((Button) other.GetComponent<Button>()).OpenDoor();
        }
        if(other.tag == "VR Controller" && recalling){
            // Recall complete, clear all momentum
            recalling = false;
            rigidbody.velocity = Vector3.zero;
            rigidbody.angularVelocity = Vector3.zero;
        }
    }

    void OnTriggerStay(Collider other){
        if(other.tag == "VR Controller"){
            // If someone grabs the frisbee with either hand
            if( (SteamVR_Actions._default.GrabPinch.GetState(RightInputSource) && attachedController == rightController)
                || (SteamVR_Actions._default.GrabPinch.GetState(LeftInputSource) && attachedController == leftController) )
            {
                // Move frisbee based on controller's position
                rigidbody.MovePosition(attachedController.position + (Vector3.Scale(attachedController.forward, Vector3.one * 0.2f)));
                // Rotate the frisbee to fit hand gesture
                rigidbody.MoveRotation(attachedController.rotation);
                // Add to the position record
                positionRecord.Enqueue(transform.position);
                if(positionRecord.Count > 20){
                    positionRecord.Dequeue();
                }
            }
            else if (attachedController != null){ // If frisbee is on hand but not grabbed
                if(positionRecord.Count == 20){ // was grabbed for at least 0.2 seconds (one physics frame is 0.02s by default)
                    Vector3 oldPosition = positionRecord.Dequeue();
                    positionRecord.Clear();
                    // Interpolate force from position 0.2 seconds ago
                    Vector3 forceVector = transform.position - oldPosition;
                    GetComponent<Rigidbody>().AddForce(Vector3.Scale(forceVector, Vector3.one * 1000.0f));
                }
                else{ // wasn't grabbed for at least 0.2 seconds
                    // just drops
                }
                attachedController = null;
                rigidbody.useGravity = true;
            }
        }
    }

}
