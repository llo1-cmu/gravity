using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyableObj : MonoBehaviour
{

    // Assign frisbee score threshold in inspector
    #pragma warning disable 0649
    [SerializeField] private int threshold;
    private bool rigidBodyExists;
    private bool useGravity;
    private Color matColor;
    #pragma warning restore 0649
    new Rigidbody rigidbody;
    new Renderer renderer;
    void Start(){
        rigidbody = GetComponent<Rigidbody>();
        renderer = GetComponent<Renderer>();
        GameManager.S.AddDestroyableObject();
        if(rigidbody){
            rigidBodyExists = true;
            //if(rigidbody.useGravity){
                useGravity = true;
            //}
        }
        else{
            rigidBodyExists = false;
        }
        if(renderer){
            matColor = renderer.material.color;
        }
    }

    public int GetThreshold(){
        return threshold;
    }

    void OnTriggerEnter(Collider other){
        // Once object gets close enough to touch frisbee, destroy it
        if (other.tag == "Frisbee") {
            other.GetComponentInParent<Frisbee>().IncreaseGravityField();
            if (GameManager.S.GetDestroyedScore() >= threshold) StartCoroutine(DisappearEffect(0.25f));
        }

        // Suspend enviornmental gravity when being pulled by the frisbee
        if (other.tag == "gravity field" && rigidBodyExists && GameManager.S.GetDestroyedScore() >= threshold) {
            //TODO: add cooler shader effect here
            if(renderer){
                renderer.material.color = Color.black;
            }
            if(useGravity){
                rigidbody.useGravity = false;
            }
            rigidbody.isKinematic = true;
            rigidbody.detectCollisions = false;
        }
    }
    void OnTriggerExit(Collider other){
        if (other.tag == "gravity field" && rigidBodyExists && GameManager.S.GetDestroyedScore() >= threshold) {
            if(renderer){
                matColor = renderer.material.color;
            }
            if(useGravity){
                rigidbody.useGravity = true;
            }
            rigidbody.isKinematic = false;
            rigidbody.detectCollisions = true;
        }
    }

    IEnumerator DisappearEffect(float timeToWait) {
        // TODO: update shaders to dissolve
        yield return new WaitForSeconds(timeToWait);
        GameManager.S.UpdateDestroyedScore();
        Destroy(gameObject);
    }
}
