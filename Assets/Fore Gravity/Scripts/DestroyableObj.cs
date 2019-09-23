using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyableObj : MonoBehaviour
{

    // Assign frisbee score threshold in inspector
    #pragma warning disable 0649
    [SerializeField] private int threshold;
    private bool useGravity;
    private Color matColor;
    #pragma warning restore 0649

    void Start(){
        GameManager.S.AddDestroyableObject();
        if(GetComponent<Rigidbody>()){
            useGravity = GetComponent<Rigidbody>().useGravity;
        }
        else{
            useGravity = false;
        }
        matColor = GetComponent<Renderer>().material.color;
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
        if (other.tag == "gravity field" && useGravity) {
            //TODO: add cooler shader effect here
            this.GetComponent<Renderer>().material.color = Color.black;
            GetComponent<Rigidbody>().useGravity = false;
        }
    }
    void OnTriggerExit(Collider other){
        if (other.tag == "gravity field" && useGravity) {
            this.GetComponent<Renderer>().material.color = matColor;
            GetComponent<Rigidbody>().useGravity = true;
        }
    }

    IEnumerator DisappearEffect(float timeToWait) {
        // TODO: update shaders to dissolve
        yield return new WaitForSeconds(timeToWait);
        GameManager.S.UpdateDestroyedScore();
        Destroy(gameObject);
    }
}
