using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO: reassign ship source, controller source, spark audio source in sound maanger/reactor scripts, add "Force Field" tag to force fields
    // GM tier system - if object is last tier don't remove gravity

    // green sign flips and turns red, plays gravity compactor on/off
    // - play gravity  noises from player when gravity explosion happens
    // - when sucked up, plays noise per object if collides w/ other object

public class SoundManager : MonoBehaviour
{
    //priority list: 0 ending, 1 trashroomending, 2 frisbeegrabbed, 3 newplace,
    // 4 lostfrisbeelines, 5 success, 6 fail, 7 trashroomlines, 8 hexroomlines
    #pragma warning disable 0649
    [SerializeField] AudioSource shipAudioSource, frisbeeAudioSource, sfxSource, leftControllerAudioSource, rightControllerAudioSource;
    [SerializeField] AudioClip frisbeeThrow, frisbeeRecall, frisbeeRecallController, frisbeeAbsorb, frisbeeCatch, sparkSound, gravityOn, gravityOff, forceFieldRebound;
    //beta clips 1-4
    [SerializeField] List<AudioClip> lostFrisbeeLines;
    //beta clip 5-7 
    [SerializeField] List<AudioClip> frisbeeGrabbedLines;
    // beta clips 8, 9 respectively
    // if you succeed in absorbing an object the first time
    [SerializeField] AudioClip successfulItem, failedItem;
    // beta clips 10, ship PA system
    // finishedP1 + finished P2 run when you finish trash room
    [SerializeField] List<AudioClip> trashFinishedLines;
    // random lines to choose from while throwing
    [SerializeField] List<AudioClip> trashRoomLines;
    // beta 12, beta 14
    // newPlace first line in hexroom, ending last line
    [SerializeField] List<AudioClip> newPlaceLines;
    [SerializeField] AudioClip ending, hexAmbience;
    // beta 13
    [SerializeField] List<AudioClip> hexRoomLines;

    // List with priorities - plays ending clips before others
    SortedList<int, AudioClip> queuedTracks;
    private bool isPlaying;
    public static SoundManager instance;
    static bool playedIntro = false;
    static bool playedSuccess = false;
    static bool playedFail = false;
    static bool isTrashRoom = false;
    #pragma warning restore 0649


    void Awake()
    {
        instance = this;
        queuedTracks = new SortedList<int, AudioClip>();
    }


    /*********************
        Small SFX
    *********************/
    public void PlayFrisbeeCatch() {
        sfxSource.PlayOneShot(frisbeeCatch);
    }

    public void PlayRecallRight() {
        sfxSource.Stop();
        sfxSource.PlayOneShot(frisbeeRecall);
        rightControllerAudioSource.PlayOneShot(frisbeeRecallController);
    }

    public void PlayRecallLeft() {
        sfxSource.Stop();
        sfxSource.PlayOneShot(frisbeeRecall);
        leftControllerAudioSource.PlayOneShot(frisbeeRecallController);
    }

    public void PlayThrow() {
        sfxSource.PlayOneShot(frisbeeThrow);
    }

    public void PlayAbsorb() {
        sfxSource.PlayOneShot(frisbeeAbsorb);
    }

    public void PlaySpark(AudioSource source) {
        source.PlayOneShot(sparkSound);
    }

    public void PlayHexAmbience() {
        AudioSource.PlayClipAtPoint(hexAmbience, transform.position);
    }

    public void PlayGravity(bool on) {
        shipAudioSource.Stop();
        if (on) shipAudioSource.PlayOneShot(gravityOn);
        else shipAudioSource.PlayOneShot(gravityOff);
    }

    public void PlayForceFieldRebound() {
        sfxSource.Stop();
        sfxSource.PlayOneShot(forceFieldRebound);
    }

    /*********************
        Longer Sounds
    *********************/

    // Continue to call these until the frisbee has been grabbed as it drifts towards the player in the intro scene
    public void PlayFrisbeePrompt()
    {
        if (playedIntro) {
            instance.queuedTracks.Add(3, newPlaceLines[0]);
            instance.HexLine();
        }
        else {
            instance.queuedTracks.Add(4, lostFrisbeeLines[0]);
        }
        instance.StartCoroutine(PlayRoutine());
    }

    // When we grab the frisbee for the first time
    public void PlayFrisbeeGrabbed()
    {
        if (playedIntro) return;
        instance.frisbeeAudioSource.Stop();
        instance.queuedTracks.Add(2, frisbeeGrabbedLines[0]);
        instance.StopAllCoroutines();
        playedIntro = true;
        isPlaying = false;
        instance.StartCoroutine(PlayRoutine());
    }

    // Plays when you throw for the first time but don't suck in an item
    public void PlayItemFail()
    {
        if (playedFail) return;
        frisbeeAudioSource.Stop();
        instance.StopAllCoroutines();
        queuedTracks.Add(6, failedItem);
        playedFail = true;
        isPlaying = false;
        instance.StartCoroutine(PlayRoutine());
    }

    // Plays when you throw for the first time and do suck in an item
    public void PlayItemSucceed()
    {
        if (playedSuccess) return;
        frisbeeAudioSource.Stop();
        instance.StopAllCoroutines();
        queuedTracks.Add(5, successfulItem);
        playedSuccess = true;
        isPlaying = false;
        instance.StartCoroutine(PlayRoutine());
    }
    
    // Plays on occasion in the trash room
    public void TrashLine()
    {
        //queuedTracks.Add(7, trashRoomLines[0]);
    }

    // Plays when we finish trash room
    public void PlayTrashFinish() {
        frisbeeAudioSource.Stop();
        queuedTracks.Add(1, trashFinishedLines[0]);
        instance.StopAllCoroutines();
        isPlaying = false;
        isTrashRoom = true;
        instance.StartCoroutine(PlayRoutine());
    }

    // Plays on occasion in the hex room
    public void HexLine()
    {
        queuedTracks.Add(8, hexRoomLines[0]);
    }

    // Play the ending clip
    public void PlayEnding() {
        frisbeeAudioSource.Stop();
        queuedTracks.Add(0, ending);
        instance.StopAllCoroutines();
        isPlaying = false;
        instance.StartCoroutine(PlayRoutine());
    }

    IEnumerator PlayRoutine() {
        while (queuedTracks.Count <= 0 || isPlaying) {yield return null;}

        // If we have things to play queued up
        AudioClip toPlay = queuedTracks.Values[0];
        queuedTracks.RemoveAt(0);

        switch (toPlay.name) {
            // When we play ending, empty the queue
            case "14 Ending":
            // We just entered the hex room
                print("ending");
                queuedTracks = new SortedList<int, AudioClip>();
                StartCoroutine(PlayClip(toPlay));
                break;

            // We are exiting trash room, clear the queue
            case "10 Trash Finish":
                queuedTracks = new SortedList<int, AudioClip>();
                StartCoroutine(PlayInSuccession(trashFinishedLines));
                break;

            // We grabbed the frisbee for the first time
            case "4 Hello":
                print("playing grabbed");
                queuedTracks = new SortedList<int, AudioClip>();
                StartCoroutine(PlayInSuccession(frisbeeGrabbedLines));
                break;
            
            case "11 Where We":
                queuedTracks = new SortedList<int, AudioClip>();
                StartCoroutine(PlayInSuccession(newPlaceLines));
                break;

            // We just sucked up an item
            case "8 Success":
            // We just failed to suck up an item
            case "9 Fail":
                print("frisbeecall");
                StartCoroutine(PlayClip(toPlay));
                break;

            // We haven't grabbed the frisbee yet
            case "Intro0 Bicycle":
                StartCoroutine(PlayFrisbeeListLines(lostFrisbeeLines, 3, 4));
                break;

            // We are idling in the hex room
            case "13 Hex Lines":
                StartCoroutine(PlayFrisbeeListLines(hexRoomLines, 5, 8));
                break;

            // We are idling in the trash room
            // Nothing here rn
            case "trashLines":
                StartCoroutine(PlayFrisbeeListLines(trashRoomLines, 5, 7));
                break;

            // God knows why we'd be here
            default:
                break;
        }

        yield return new WaitForSeconds(0);
    }

    IEnumerator PlayClip(AudioClip clip)
    {
        instance.isPlaying = true;
        instance.StopCoroutine(PlayRoutine());
        instance.frisbeeAudioSource.PlayOneShot(clip);
        yield return new WaitForSeconds(clip.length);
        instance.isPlaying = false;
        instance.StartCoroutine(PlayRoutine());
    }

    IEnumerator PlayFrisbeeListLines(List<AudioClip> listToPlay, int     bufferBetween, int priority) 
    {
        instance.isPlaying = true;
        instance.StopCoroutine(PlayRoutine());
        int choose = Random.Range(0, listToPlay.Count);
        instance.frisbeeAudioSource.PlayOneShot(listToPlay[choose]);

        yield return new WaitForSeconds(bufferBetween);
        yield return new WaitForSeconds(listToPlay[choose].length);

        instance.queuedTracks.Add(priority, listToPlay[0]);
        instance.isPlaying = false;
        instance.StartCoroutine(PlayRoutine());
    }

    IEnumerator PlayInSuccession(List<AudioClip> listToPlay){
        isPlaying = true;
        instance.StopCoroutine(PlayRoutine());

        for (int i = 0; i < listToPlay.Count; i++) {
            if (i == listToPlay.Count - 1 && isTrashRoom) {
                instance.shipAudioSource.Stop();
                instance.shipAudioSource.PlayOneShot(listToPlay[i]);
                isTrashRoom = false;
            } else {
                instance.frisbeeAudioSource.PlayOneShot(listToPlay[i]);
            }
            yield return new WaitForSeconds(listToPlay[i].length);
        }

        isPlaying = false;
        instance.StartCoroutine(PlayRoutine());
    }


}


