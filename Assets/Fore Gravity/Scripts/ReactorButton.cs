using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DestroyableObj))]
public class ReactorButton : MonoBehaviour
{
    // Assign doors in the inspector
    [SerializeField] private GameObject blockingPlane;
    DestroyableObj _DestroyableObj;
    void Start()
    {
        blockingPlane.SetActive(true);
        _DestroyableObj = this.GetComponent<DestroyableObj>();
    }

    private void OpenBlock(){
        blockingPlane.SetActive(false);
    }

    void OnTriggerEnter(Collider other){
        if (other.tag == "gravity field" && GameManager.S.GetDestroyedScore() >= _DestroyableObj.GetThreshold()) {
            OpenBlock();

            // If we want to disable gravity on all objects
            if (this.tag == "Disable Gravity") {
                GameManager.S.DisableGravity();
            }
        }
    }
}
