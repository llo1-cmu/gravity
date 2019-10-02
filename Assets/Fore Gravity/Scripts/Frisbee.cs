using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class Frisbee : MonoBehaviour
{

    #pragma warning disable 0649
    // Debugging things
    private List<Vector3> frisbeePositions;
    private bool recordingPos = false;

    // Steam VR things
    [SerializeField] private Transform leftController, rightController;
    private Transform attachedController;
    private SteamVR_Input_Sources LeftInputSource = SteamVR_Input_Sources.LeftHand;
    private SteamVR_Input_Sources RightInputSource = SteamVR_Input_Sources.RightHand;
    [SerializeField] private SteamVR_Action_Vibration vibration;
    private Valve.VR.InteractionSystem.VelocityEstimator velocityEstimator;

    // Components
    private new Rigidbody rigidbody;
    [SerializeField] private GameObject gravityField;
    [SerializeField] private MeshRenderer FrisbeeSphere;

    [SerializeField] private Transform mainCamera;

    // Recall things
    private bool recalling, introScene;
    private Vector3 recallStartPosition;
    private float recallStartTime;
    [SerializeField] private float recallSpeed = 10.0f;
    [SerializeField] private Material originalMaterial, recallMaterial;

    // Sound manager bools
    private bool firstCaughtPlayed, firstItemSuccess;
    #pragma warning restore 0649

    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        velocityEstimator = GetComponent<Valve.VR.InteractionSystem.VelocityEstimator>();
        SoundManager.instance.PlayFrisbeePrompt();
        introScene = true;
    }

    void Update()
    {
        if((SteamVR_Actions._default.GrabPinch.GetState(RightInputSource) || SteamVR_Actions._default.GrabPinch.GetState(LeftInputSource)) && attachedController == null && !introScene){
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
            // Also estimate the velocity
            velocityEstimator.BeginEstimatingVelocity();
            SoundManager.instance.PlayRecall();
            FrisbeeSphere.material = recallMaterial;
        }
        else if (introScene) {
            // Frisbee drifts towards player
            transform.position = Vector3.MoveTowards(transform.position, GameManager.S.player.transform.position, 0.0008f);
        }
        else if(recalling && (SteamVR_Actions._default.GrabPinch.GetState(RightInputSource) || SteamVR_Actions._default.GrabPinch.GetState(LeftInputSource))){
            // Non-physical recall
            Vector3 recallVector = attachedController.position - transform.position;
            rigidbody.MovePosition(transform.position + Vector3.Normalize(recallVector) * Mathf.Min(recallSpeed*Time.deltaTime*(Time.time-recallStartTime), recallVector.magnitude));
            //rigidbody.MovePosition(Vector3.Lerp(recallStartPosition, attachedController.position, (Time.time - recallStartTime)/0.2f ));
            if(SteamVR_Actions._default.GrabPinch.GetState(RightInputSource)){
                vibration.Execute(0.0f, 0.1f, 40.0f, 0.1f, RightInputSource);
            }
            else if(SteamVR_Actions._default.GrabPinch.GetState(LeftInputSource)){
                vibration.Execute(0.0f, 0.1f, 40.0f, 0.1f, LeftInputSource);
            }
        }
        else if(recalling && !(SteamVR_Actions._default.GrabPinch.GetState(RightInputSource) || SteamVR_Actions._default.GrabPinch.GetState(LeftInputSource))){
            // Recall cancelled (dropped)
            recalling = false;
            rigidbody.useGravity = true;
            attachedController = null;
            // Apply estimated velocity
            velocityEstimator.FinishEstimatingVelocity();
            rigidbody.velocity = velocityEstimator.GetVelocityEstimate();
        }
        else if (!recalling && recordingPos){
            frisbeePositions.Add(transform.position);
        }

        if( !(SteamVR_Actions._default.GrabPinch.GetState(RightInputSource) && attachedController == rightController)
        && !(SteamVR_Actions._default.GrabPinch.GetState(LeftInputSource) && attachedController == leftController)
        && attachedController != null){
            // Throwing the frisbee
            frisbeePositions = new List<Vector3>();
            frisbeePositions.Add(transform.position);
            recordingPos = true;
            transform.parent = null;
            attachedController = null;
            rigidbody.useGravity = true;
            rigidbody.isKinematic = false;
            // Apply velocity estimated by VelocityEstimator
            velocityEstimator.FinishEstimatingVelocity();
            // Curve the frisbee towards the center
            float degreesFromCenter = Vector3.Angle(velocityEstimator.GetVelocityEstimate(), mainCamera.rotation.eulerAngles);
            if(degreesFromCenter < 45.0f){
                Vector3 adjustedDirection = (degreesFromCenter/45.0f) * velocityEstimator.GetVelocityEstimate().normalized + ((45.0f-degreesFromCenter)/45.0f) * mainCamera.forward;
                rigidbody.velocity = adjustedDirection * velocityEstimator.GetVelocityEstimate().magnitude * 1.5f;
            }
            else{
                rigidbody.velocity = velocityEstimator.GetVelocityEstimate()*1.5f;
            }
            
            rigidbody.angularVelocity = ScaleLocalAngularVelocity(velocityEstimator.GetAngularVelocityEstimate(), new Vector3(0.1f, 1.0f, 0.1f));

            SoundManager.instance.PlayThrow();
            gravityField.SetActive(true);
        }

        if(!firstItemSuccess /*&& Tutorial.IsTutorial()*/ && GameManager.S.GetDestroyedScore() > 0){
            SoundManager.instance.PlayItemSucceed();
            firstItemSuccess = true;
        }
    }

    void OnTriggerEnter(Collider other){
        switch(other.tag) {
            case "VR Controller":
                if (!recalling && !(introScene && (SteamVR_Actions._default.GrabPinch.GetState(RightInputSource) || SteamVR_Actions._default.GrabPinch.GetState(LeftInputSource)))) break;
                // Recall complete, clear all momentum
                RecallEffect();
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
    }

    void RecallEffect(){
        if (introScene){
            introScene = false;
            if(SteamVR_Actions._default.GrabPinch.GetState(RightInputSource)){
                attachedController = rightController;
            }
            else{
                attachedController = leftController;
            }
        }
        recalling = false;
        rigidbody.velocity = Vector3.zero;
        rigidbody.angularVelocity = Vector3.zero;
        rigidbody.isKinematic = true;
        transform.SetParent(attachedController, false);
        transform.position = attachedController.position;
        transform.rotation = attachedController.rotation;
        // Velocity is not calculated by VelocityEstimator
        velocityEstimator.BeginEstimatingVelocity();
        SoundManager.instance.PlayFrisbeeCatch();
        FrisbeeSphere.material = originalMaterial;
        gravityField.SetActive(false);

        if(SteamVR_Actions._default.GrabPinch.GetState(RightInputSource)){
            vibration.Execute(0.0f, 0.3f, 160.0f, 1.0f, RightInputSource);
        }
        else if(SteamVR_Actions._default.GrabPinch.GetState(LeftInputSource)){
            vibration.Execute(0.0f, 0.3f, 160.0f, 1.0f, LeftInputSource);
        }

        // We grab the frisbee for the first time
        if(/*Tutorial.IsTutorial() &&*/ !firstCaughtPlayed){
            SoundManager.instance.PlayFrisbeeGrabbed();
            firstCaughtPlayed = true;
            SoundManager.instance.TrashLine();
        }
    }

    void OnTriggerStay(Collider other){
        if(other.tag == "VR Controller"){
            // If someone grabs the frisbee with either hand
            if( (recalling || introScene)
                && (SteamVR_Actions._default.GrabPinch.GetState(RightInputSource) || SteamVR_Actions._default.GrabPinch.GetState(LeftInputSource) ))
            {
                RecallEffect();
            }
        }
    }

    void OnCollisionEnter(Collision collision){
        if(collision.collider.tag == "Target Plane"){
            Vector3 offsetVector = collision.contacts[0].point - collision.transform.position;
            print("Plane hit: " + collision.gameObject.name);
            print("Offset vector: " + offsetVector.ToString("f4"));
            print("Offset distance: " + offsetVector.magnitude.ToString("f4"));
        }
    }

    void FixedUpdate(){
        Vector3 newAngularVelocity = ScaleLocalAngularVelocity(rigidbody.angularVelocity, new Vector3(0.9f, 0.999f, 0.9f));
        if(float.IsNaN(newAngularVelocity.x)){
            print("Current angular velocity is " + rigidbody.angularVelocity.ToString("f4") + " when NaN happened");
        }
        else{
            rigidbody.angularVelocity = newAngularVelocity;
        }
    }

    Vector3 ScaleLocalAngularVelocity(Vector3 angularVelocity, Vector3 scaleVector){
        return transform.TransformDirection(Vector3.Scale(transform.InverseTransformDirection(angularVelocity), scaleVector));
    }

    public void IncreaseGravityField(){
        gravityField.GetComponent<GravityField>().IncreaseGravityField();
    }
}
