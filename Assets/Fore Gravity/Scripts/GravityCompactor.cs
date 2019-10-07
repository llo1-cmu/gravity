using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityCompactor : MonoBehaviour
{
    private bool gravityOff = false;
    // green sign flips and turns red, plays gravity compactor on/off

    void OnTriggerEnter(Collider other) {
        if (other.tag == "Frisbee") {
            gravityOff = !gravityOff;
            SoundManager.instance.PlayGravity(gravityOff);
            GameManager.S.BroadcastGravityDisabledTemp(gravityOff);
        }
    }
}
