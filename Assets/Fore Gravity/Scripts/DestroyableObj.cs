using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyableObj : MonoBehaviour
{

    //TODO: make prefabs instead of just individual objects!!
    // Assign frisbee score threshold in inspector
    [SerializeField] private int threshold;

    private void Disappear(){
        this.GetComponent<Renderer>().material.color = Color.black;
        StartCoroutine(DisappearEffect(1f));
    }

    void OnTriggerEnter(Collider other){
        if (other.tag == "Frisbee") {
            if (GameManager.S.GetDestroyedScore() >= threshold) Disappear();
        }
    }

    IEnumerator DisappearEffect(float timeToWait) {
        // TODO: update shaders to dissolve first
        yield return new WaitForSeconds(timeToWait);
        GameManager.S.UpdateDestroyedScore();
        Destroy(gameObject);
    }
}
