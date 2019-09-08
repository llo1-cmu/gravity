using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Open(){
        // Lift the door for its height
        StartCoroutine(LiftDoor(GetComponent<MeshFilter>().mesh.bounds.extents.y * 2));
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator LiftDoor(float amountToLift){
        float locationToReach = transform.position.y + amountToLift;
        while (transform.position.y < locationToReach) {
            transform.position = new Vector3(transform.position.x, transform.position.y + 0.05f, transform.position.z);
            yield return new WaitForSeconds(0.01f);
        }
    }
}
