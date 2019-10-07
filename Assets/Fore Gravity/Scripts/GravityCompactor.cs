using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GravityCompactor : MonoBehaviour
{
    private bool gravityOff = false;
    private Material[] materials;
    [SerializeField] private MeshRenderer imageColor;
    [SerializeField] private Material redMaterial;
    [SerializeField] private Material greenMaterial;

    void Start(){
        materials = imageColor.materials;
        materials[1] = greenMaterial;
        imageColor.materials = materials;
    }

    void OnTriggerEnter(Collider other) {
        if (other.tag == "Frisbee") {
            gravityOff = !gravityOff;
            SoundManager.instance.PlayGravity(gravityOff);
            GameManager.S.BroadcastGravityDisabledTemp(gravityOff);
            this.transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, -1 * transform.localScale.z);

            // Change material color
            if (gravityOff) materials[1] = redMaterial;
            else materials[1] = greenMaterial;
            imageColor.materials = materials;
        }
    }
}
