using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour
{
    // Assign doors in the inspector
    [SerializeField] private List<Door> currentDoors = null;
    void Start()
    {
        
    }

    private void OpenDoor(){
        foreach(Door door in currentDoors) {
            door.Open();
        }
    }

    void OnTriggerEnter(Collider other){
        if (other.tag == "Frisbee") {
            OpenDoor();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
