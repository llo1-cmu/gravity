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
        var source = this.GetComponent<AudioSource>();
        source.priority = 128;
        float len = SoundManager.instance.PlayDebrisHit(source);
        yield return new WaitForSeconds(len);
        Destroy(this.gameObject);
    }
}