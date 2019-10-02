using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyableObj : MonoBehaviour
{

    // Assign frisbee score threshold in inspector
    #pragma warning disable 0649
    [SerializeField] private int threshold;
    [SerializeField] private int pointValue = 5;
    private bool rigidBodyExists;
    private bool useGravity;
    private Color matColor;
    private Vector3 originalScale;
    private Transform frisbee;
    private Vector3 originalPosition;
    #pragma warning restore 0649
    new Rigidbody rigidbody;
    new Renderer renderer;
    
    void Start(){
        rigidbody = GetComponent<Rigidbody>();
        renderer = GetComponent<Renderer>();
        GameManager.S.AddDestroyableObject(pointValue);
        if(rigidbody){
            rigidBodyExists = true;
        }
        else{
            rigidBodyExists = false;
        }
        if(renderer){
            matColor = renderer.material.color;
        }
        originalScale = transform.localScale;
    }

    public int GetThreshold(){
        return threshold;
    }

    void Update() {
        if (GameManager.S.GetBroadcastGravityDisabled()) {
            rigidbody.useGravity = false;
            useGravity = false;
            return;
        }
    }

    // void OnTriggerStay(Collider other) {
    //     if (other.tag != "gravity field") return;

    //     // If we are strong enough to pick up the other object
    //     if (GameManager.S.GetDestroyedScore() >= threshold && !beingDestroyed) {
    //         // transform.parent = other.transform;
    //         // Vector faces towards the center of the gravity field
    //         // var vec = other.transform.position - transform.position;

    //         // Normalize the magnitude b/w 0 and 1 to put on the force curve
    //         // float normDist = Mathf.Clamp01(vec.magnitude / _Collider.radius);
    //         //if (vec.magnitude > _Collider.radius) normDist = 1;

    //         // Moves based on time and not distance
    //         // transform.position = Vector3.MoveTowards(transform.position, other.transform.position, Time.deltaTime * 5);

    //         // Decrease other object's size exponentially
    //         // TODO: why does size increase?
    //         // if (vec.magnitude < 1f && vec.magnitude > 0.05f && other.transform.localScale.magnitude > 0.001f) {
    //         //     other.transform.localScale *= vec.magnitude;
    //         // }

    //         // Moved moving and shrinking into the coroutine
            
    //     }
    // }

    void OnTriggerEnter(Collider other) {
        // Once object gets close enough to touch frisbee, destroy it
        // if (other.tag == "Frisbee") {
        //     other.GetComponentInParent<Frisbee>().IncreaseGravityField();
        //     if (GameManager.S.GetDestroyedScore() >= threshold) StartCoroutine(DisappearEffect(0.25f));
        // }

        // Suspend enviornmental gravity when being pulled by the frisbee
        if (other.tag == "gravity field" && GameManager.S.GetDestroyedScore() >= threshold) {
            if(rigidBodyExists)
            //TODO: add cooler shader effect here
            // if(renderer){
            //     renderer.material.color = Color.black;
            // }
            if(rigidBodyExists){
                if(useGravity){
                    rigidbody.useGravity = false;
                }
                rigidbody.isKinematic = true;
                //rigidbody.detectCollisions = false;
            }

            frisbee = other.transform;
            originalPosition = transform.position;

            SoundManager.instance.PlayAbsorb();
            StartCoroutine(DisappearEffect(0.5f));
            frisbee.GetComponent<Frisbee>().IncreaseGravityField();
        }

        // Play fail-to-absorb sound if we haven't already
        else if (other.tag == "gravity field" && GameManager.S.GetDestroyedScore() < threshold) {
            GravityField gf = other.GetComponent<GravityField>();
            if (gf.firstItemFailed) return;
            gf.firstItemFailed = true;
            SoundManager.instance.PlayItemFail();
        }
    }

    IEnumerator DisappearEffect(float timeToWait) {
        float startTime = Time.time;
        // TODO: update shaders to dissolve
        //yield return new WaitForSeconds(timeToWait);
        while(Time.time - startTime < timeToWait){
            transform.position = Vector3.Lerp(originalPosition, frisbee.position, (Time.time-startTime)/timeToWait);
            transform.localScale = originalScale * (1f - Mathf.Pow((Time.time-startTime)/timeToWait, 2f) );
            yield return null;
        }

        GameManager.S.UpdateDestroyedScore();
        Destroy(gameObject);
    }
}
