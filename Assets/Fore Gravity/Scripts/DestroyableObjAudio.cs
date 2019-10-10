using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyableObjAudio : MonoBehaviour
{
    void Start () {

    }

    public void PlayHit() {
        StartCoroutine(PlayDebrisNoise());
    }

    IEnumerator PlayDebrisNoise() {
        float len = SoundManager.instance.PlayDebrisHit(this.GetComponent<AudioSource>());
        yield return new WaitForSeconds(len);
        Destroy(this.gameObject);
    }
}