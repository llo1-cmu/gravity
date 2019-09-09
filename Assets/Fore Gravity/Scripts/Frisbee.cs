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
        Debug.Log("Right Trackpad value:" + SteamVR_Actions._default.Teleport.GetState(RightInputSource).ToString());
    }
    void OnTriggerEnter(Collider other){
        if (other.tag == "Button") {
            ((Button) other.GetComponent<Button>()).OpenDoor();
        }
    }

    void OnTriggerStay(Collider other){
        if(other.tag == "VR Controller"){
            // If someone grabs the frisbee with either hand
            if( (SteamVR_Actions._default.GrabPinch.GetState(RightInputSource) && other.transform == rightController)
                || (SteamVR_Actions._default.GrabPinch.GetState(LeftInputSource) && other.transform == leftController) )
            {
                // Move frisbee based on controller's position
                rigidbody.MovePosition(other.transform.position + (Vector3.Scale(other.transform.forward, Vector3.one * 0.2f)));
                // Rotate the frisbee to fit hand gesture
                rigidbody.MoveRotation(other.transform.rotation);
                // Clear all forces
                rigidbody.useGravity = false;
                // Add to the position record
                positionRecord.Enqueue(transform.position);
                if(positionRecord.Count > 20){
                    positionRecord.Dequeue();
                }
            }
            else{ // If frisbee is on hand but not grabbed
                if(positionRecord.Count == 20){ // was grabbed for at least 0.2 seconds (one physics frame is 0.02s by default)
                    Vector3 oldPosition = positionRecord.Dequeue();
                    positionRecord.Clear();
                    rigidbody.useGravity = true;
                    // Interpolate force from position 0.2 seconds ago
                    Vector3 forceVector = transform.position - oldPosition;
                    GetComponent<Rigidbody>().AddForce(Vector3.Scale(forceVector, new Vector3(1.0f, 0.0f, 1.0f)) * 1000.0f);
                }
                else{ // wasn't grabbed for at least 0.2 seconds
                    // just drops
                    rigidbody.useGravity = true;
                }
            }
        }
    }

}
