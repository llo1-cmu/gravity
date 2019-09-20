using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyableObj : MonoBehaviour
{

    //TODO: make prefabs instead of just individual objects!!
    // Assign frisbee score threshold in inspector
    #pragma warning disable 0649
    [SerializeField] private int threshold;
    private bool useGravity;
    private bool disappearing;
    #pragma warning restore 0649

    void Start(){
        if(GetComponent<Rigidbody>()){
            useGravity = GetComponent<Rigidbody>().useGravity;
        }
        else{
            useGravity = false;
        }
    }

    private void Disappear(){
        this.GetComponent<Renderer>().material.color = Color.black;
        if(!disappearing){
            StartCoroutine(DisappearEffect(0.5f));
            disappearing = true;
        }
    }

    void OnTriggerEnter(Collider other){
        if (other.tag == "Frisbee") {
            other.GetComponentInParent<Frisbee>().IncreaseGravityField();
            if (GameManager.S.GetDestroyedScore() >= threshold) Disappear();
        }
        //suspend enviornmental gravity when being pulled by the frisbee
        if (other.tag == "gravity field" && useGravity) {
            GetComponent<Rigidbody>().useGravity = false;
        }
    }
    void OnTriggerExit(Collider other){
        if (other.tag == "gravity field" && useGravity) {
            GetComponent<Rigidbody>().useGravity = true;
        }
    }

    void OnTriggerStay(Collider other) {
        if (other.tag == "gravity field" && GameManager.S.GetDestroyedScore() >= threshold) {
            transform.position = Vector3.MoveTowards(transform.position, other.transform.position,
            0.03f);
        }
    }

    IEnumerator DisappearEffect(float timeToWait) {
        // TODO: update shaders to dissolve first
        yield return new WaitForSeconds(timeToWait);
        GameManager.S.UpdateDestroyedScore();
        Destroy(gameObject);
    }
}
