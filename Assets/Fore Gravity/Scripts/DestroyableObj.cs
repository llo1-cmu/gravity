using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyableObj : MonoBehaviour
{

    //TODO: make prefabs instead of just individual objects!!
    // Assign frisbee score threshold in inspector
    #pragma warning disable 0649
    [SerializeField] private int threshold;
    #pragma warning restore 0649

    private void Disappear(){
        this.GetComponent<Renderer>().material.color = Color.black;
        StartCoroutine(DisappearEffect(1f));
    }

    void OnTriggerEnter(Collider other){
        if (other.tag == "Frisbee") {
            other.GetComponent<Frisbee>().IncreaseGravityField();
            if (GameManager.S.GetDestroyedScore() >= threshold) Disappear();
        }
    }

    void OnTriggerStay(Collider other) {
        if (other.tag == "gravity field") {
            transform.position = Vector3.MoveTowards(transform.position, other.transform.position,
            0.001f);
        }
    }

    IEnumerator DisappearEffect(float timeToWait) {
        // TODO: update shaders to dissolve first
        yield return new WaitForSeconds(timeToWait);
        GameManager.S.UpdateDestroyedScore();
        Destroy(gameObject);
    }
}
