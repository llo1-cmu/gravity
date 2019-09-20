using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class Frisbee : MonoBehaviour
{

    #pragma warning disable 0649

    private List<Vector3> frisbeePositions;
    private bool recordingPos = false;
    [SerializeField] private Transform leftController, rightController;
    private Transform attachedController;
    private SteamVR_Input_Sources LeftInputSource = SteamVR_Input_Sources.LeftHand;
    private SteamVR_Input_Sources RightInputSource = SteamVR_Input_Sources.RightHand;
    private new Rigidbody rigidbody;
    private bool recalling;
    private Vector3 recallStartPosition;
    private float recallStartTime;
    private Valve.VR.InteractionSystem.VelocityEstimator velocityEstimator;
    private AudioSource audioSource;
    [SerializeField] private AudioClip catchSound, throwSound, recallSound;
    [SerializeField] private float recallSpeed = 10.0f;

    [SerializeField] private GameObject gravityField;
    private bool initialPromptPlayed, firstCaughtPlayed, firstSphereHit;

    #pragma warning restore 0649

    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        velocityEstimator = GetComponent<Valve.VR.InteractionSystem.VelocityEstimator>();
        audioSource = GetComponent<AudioSource>();
        if(Tutorial.IsTutorial()){
            Tutorial.PlayFrisbeePrompt();
            initialPromptPlayed = true;
        }
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
            recordingPos = false;
            //recallStartPosition = transform.position;
            recallStartTime = Time.time;
            // Clear velocity
            //rigidbody.velocity = Vector3.zero;

            audioSource.PlayOneShot(recallSound);
        }
        else if(recalling && (SteamVR_Actions._default.GrabPinch.GetState(RightInputSource) || SteamVR_Actions._default.GrabPinch.GetState(LeftInputSource))){
            // Non-physical recall
            Vector3 recallVector = attachedController.position - transform.position;
            rigidbody.MovePosition(transform.position + Vector3.Normalize(recallVector) * Mathf.Min(recallSpeed*Time.deltaTime*(Time.time-recallStartTime), recallVector.magnitude));
            //rigidbody.MovePosition(Vector3.Lerp(recallStartPosition, attachedController.position, (Time.time - recallStartTime)/0.2f ));
        }
        else if(recalling && !(SteamVR_Actions._default.GrabPinch.GetState(RightInputSource) || SteamVR_Actions._default.GrabPinch.GetState(LeftInputSource))){
            // Recall cancelled (dropped)
            recalling = false;
            rigidbody.useGravity = true;
            attachedController = null;
        }
        else if (!recalling && recordingPos){
            frisbeePositions.Add(transform.position);
        }

        if(!firstSphereHit && Tutorial.IsTutorial() && GameManager.S.GetDestroyedScore() > 0){
            Tutorial.PlaySphereHit();
            firstSphereHit = true;
        }
    }

    void OnTriggerEnter(Collider other){
        switch(other.tag) {
            case "VR Controller":
                if (!recalling) break;
                // Recall complete, clear all momentum
                recalling = false;
                rigidbody.velocity = Vector3.zero;
                rigidbody.angularVelocity = Vector3.zero;
                // Velocity is not calculated by VelocityEstimator
                velocityEstimator.BeginEstimatingVelocity();
                audioSource.PlayOneShot(catchSound);
                gravityField.SetActive(false);
                if(Tutorial.IsTutorial() && !firstCaughtPlayed){
                    Tutorial.PlayFrisbeeCaught();
                    firstCaughtPlayed = true;
                }
                break;
            case "Target Plane":
                SaveData.DataSave(frisbeePositions, 1);
                recordingPos = false;
                break;
            case "UI Button":
                other.GetComponent<UnityEngine.UI.Button>().onClick.Invoke();
                break;
            default:
                break;
        }
        // if(other.tag == "VR Controller" && recalling){
        //     // Recall complete, clear all momentum
        //     recalling = false;
        //     rigidbody.velocity = Vector3.zero;
        //     rigidbody.angularVelocity = Vector3.zero;
        //     // Velocity is not calculated by VelocityEstimator
        //     velocityEstimator.BeginEstimatingVelocity();
        // }
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
            }
            else if (attachedController != null){ // If frisbee is on hand but not grabbed (being thrown)
                frisbeePositions = new List<Vector3>();
                frisbeePositions.Add(transform.position);
                recordingPos = true;

                attachedController = null;
                rigidbody.useGravity = true;
                // Apply velocity estimated by VelocityEstimator
                velocityEstimator.FinishEstimatingVelocity();
                rigidbody.velocity = velocityEstimator.GetVelocityEstimate();
                rigidbody.angularVelocity = ScaleLocalAngularVelocity(velocityEstimator.GetAngularVelocityEstimate(), new Vector3(0.1f, 1.0f, 0.1f));

                audioSource.PlayOneShot(throwSound);
                gravityField.SetActive(true);
            }
        }
    }
    /*void OnTriggerExit(Collider other){
        if(attachedController != null && rigidbody.useGravity == false){
            // Recall the frisbee if it accidentally leaves the hand.
            recalling = true;
        }
    }*/

    void FixedUpdate(){
        rigidbody.angularVelocity = ScaleLocalAngularVelocity(rigidbody.angularVelocity, new Vector3(0.9f, 0.999f, 0.9f));
    }

    Vector3 ScaleLocalAngularVelocity(Vector3 angularVelocity, Vector3 scaleVector){
        return transform.TransformDirection(Vector3.Scale(transform.InverseTransformDirection(angularVelocity), scaleVector));
    }

    //TODO: this should scale based on size of object we destroy as param
    // also should probably be moved to FrisbeeGravity
    public void IncreaseGravityField(){
        gravityField.GetComponent<SphereCollider>().radius *= 1.1f;
    }
}
