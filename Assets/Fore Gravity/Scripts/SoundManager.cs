using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    #pragma warning disable 0649
    [SerializeField] AudioSource shipAudioSource, frisbeeAudioSource;
    [SerializeField] AudioClip initialWarning, frisbeeCalling, frisbeeCaught, frisbeeAbsorb;
    //beta clips 1-4
    [SerializeField] List<AudioClip> lostFrisbeeLines;
    //beta clip 5-7 
    [SerializeField] AudioClip frisbeeCaught1, frisbeeCaught2, frisbeeCaught3;
    // beta clips 8, 9 respectively
    [SerializeField] AudioClip successfulItem, failedItem;
    // beta clips 10, ship PA system
    [SerializeField] AudioClip finishedP1, finishedPart2;
    // random lines to choose from while throwing
    [SerializeField] List<AudioClip> trashRoomLines;
    // beta 12, beta 14
    [SerializeField] AudioClip newPlace, ending;
    // beta 13
    [SerializeField] List<AudioClip> hexRoomLines;
    private bool isPlaying = false;
    public static SoundManager instance;
    #pragma warning restore 0649


    //TODO: is it alright if everything just plays at the same time????
    void Awake()
    {
        instance = this;
    }

    public void PlayWarning()
    {
        instance.StartCoroutine(PlayWarningTime());
    }

    // Continue to call these until the frisbee has been grabbed as it drifts towards the player.
    public void PlayFrisbeePrompt()
    {
        instance.StartCoroutine(PlayFrisbeeListLines(instance.lostFrisbeeLines, 3));
    }
    public void PlayFrisbeeCaught()
    {    
        instance.frisbeeAudioSource.PlayOneShot(instance.frisbeeCaught1);
        instance.StartCoroutine(PlayFrisbeeInitialLines());

    }
    public void PlayItemFail()
    {
        instance.frisbeeAudioSource.PlayOneShot(instance.failedItem);
    }

    public void PlayItemSucceed()
    {
        instance.frisbeeAudioSource.PlayOneShot(instance.successfulItem);
    }

    public void TrashLine()
    {
        instance.StartCoroutine(PlayFrisbeeListLines(instance.trashRoomLines, 3));
    }


    public void HexLine()
    {
        instance.StartCoroutine(PlayFrisbeeListLines(instance.hexRoomLines, 3));
    }

    public void PlaySphereHit(){
        instance.frisbeeAudioSource.PlayOneShot(instance.frisbeeAbsorb);
    }

    public bool IsPlaying(){
        return instance.isPlaying;
    }

    IEnumerator ArriveHex()
    {
        yield return new WaitForSeconds(2);
        instance.frisbeeAudioSource.PlayOneShot(instance.newPlace);
    }

    IEnumerator PlayFrisbeeListLines(List<AudioClip> listToPlay, int bufferBetween) {
        int choose = Random.Range(0, listToPlay.Count);
        isPlaying = true;
        instance.frisbeeAudioSource.PlayOneShot(listToPlay[choose]);
        yield return new WaitForSeconds(listToPlay[choose].length);
        //Buffer between clips being played
        yield return new WaitForSeconds(bufferBetween);
        isPlaying = false;
        //TODO: why is this here?
        //instance.frisbeeAudioSource.Play(instance.frisbeeCalling);
    }

    IEnumerator PlayFrisbeeInitialLines()
    {
        instance.frisbeeAudioSource.PlayOneShot(instance.frisbeeCaught2);
        yield return new WaitForSeconds(instance.frisbeeCaught2.length);
        instance.frisbeeAudioSource.PlayOneShot(instance.frisbeeCaught3);
        yield return new WaitForSeconds(instance.frisbeeCaught3.length);

    }

    IEnumerator PlayWarningTime()
    {
        instance.frisbeeAudioSource.PlayOneShot(instance.finishedP1);
        //TODO: why do we wait 7 seconds?
        yield return new WaitForSeconds(instance.finishedP1.length);
        instance.shipAudioSource.Stop();
        instance.shipAudioSource.PlayOneShot(instance.initialWarning);

    }
}


