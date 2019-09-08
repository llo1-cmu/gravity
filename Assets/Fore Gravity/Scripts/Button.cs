using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private List<Door> currentDoors;
    void Start()
    {
        
    }

    public void OpenDoor(){
        foreach(Door door in currentDoors) {
            door.Open();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
