using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyableObj : MonoBehaviour
{

    // Assign frisbee score threshold in inspector
    #pragma warning disable 0649
    [SerializeField] private int threshold;
    private bool rigidBodyExists;
    private Color matColor;
    #pragma warning restore 0649
    Rigidbody rigidbody;
    Renderer renderer;
    void Start(){
        rigidbody = GetComponent<Rigidbody>();
        renderer = GetComponent<Renderer>();
        GameManager.S.AddDestroyableObject();
        if(rigidbody){
            rigidBodyExists = true;
            rigidbody.useGravity = true;
        }
        else{
            rigidBodyExists = false;
        }
        matColor = renderer.material.color;
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
            this.renderer.material.color = Color.black;
            rigidbody.useGravity = false;
            rigidbody.isKinematic = false;
        }
    }
    void OnTriggerExit(Collider other){
        if (other.tag == "gravity field" && rigidBodyExists && GameManager.S.GetDestroyedScore() >= threshold) {
            this.renderer.material.color = matColor;
            rigidbody.useGravity = true;
            rigidbody.isKinematic = true;
        }
    }

    IEnumerator DisappearEffect(float timeToWait) {
        // TODO: update shaders to dissolve
        yield return new WaitForSeconds(timeToWait);
        GameManager.S.UpdateDestroyedScore();
        Destroy(gameObject);
    }
}
