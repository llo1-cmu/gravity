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
        source.spatialBlend = 1.0f;
        source.priority = 128;
        float len = SoundManager.instance.PlayDebrisHit(source);
        yield return new WaitForSeconds(len);
        GameManager.S.audioSourcesInstantiated--;
        Destroy(this.gameObject);
    }
}