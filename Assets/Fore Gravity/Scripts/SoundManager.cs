using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    #pragma warning disable 0649
    [SerializeField] AudioSource shipAudioSource, frisbeeAudioSource;
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
    [SerializeField] AudioClip ending;
    // beta 13
    [SerializeField] List<AudioClip> hexRoomLines;

    // List with priorities - plays ending clips before others
    SortedList<int, AudioClip> queuedTracks;
    private bool isPlaying;
    public static SoundManager instance;
    #pragma warning restore 0649


    void Awake()
    {
        instance = this;
        queuedTracks = new SortedList<int, AudioClip>();
        isPlaying = false;
    }

    // Continue to call these until the frisbee has been grabbed as it drifts towards the player in the intro scene
    public void PlayFrisbeePrompt()
    {
        queuedTracks.Add(2, lostFrisbeeLines[0]);
        instance.StartCoroutine(PlayRoutine());
    }

    // When we grab the frisbee for the first time
    public void PlayFrisbeeGrabbed()
    {
        queuedTracks.Add(0, frisbeeGrabbedLines[0]);
        frisbeeAudioSource.Stop();
        isPlaying = false;
        instance.StartCoroutine(PlayRoutine());
    }

    // Plays when you throw for the first time but don't suck in an item
    public void PlayItemFail()
    {
        queuedTracks.Add(1, failedItem);
    }

    // Plays when you throw for the first time and do suck in an item
    public void PlayItemSucceed()
    {
        queuedTracks.Add(1, successfulItem);
    }
    
    // Plays on occasion in the trash room
    public void TrashLine()
    {
        queuedTracks.Add(2, trashRoomLines[0]);
    }

    // Plays when we finish trash room
    public void PlayTrashFinish() {
        queuedTracks.Add(0, trashFinishedLines[0]);
    }

    // Plays on occasion in the hex room
    public void HexLine()
    {
        queuedTracks.Add(2, hexRoomLines[0]);
    }

    // Plays when we enter the hex room for the first time
    public void PlayHexEntrance() {
        queuedTracks.Add(0, newPlaceLines[0]);
    }

    // Play the ending clip
    public void PlayEnding() {
        queuedTracks.Add(0, ending);
    }

    IEnumerator PlayRoutine() {
        while (queuedTracks.Count <= 0 || isPlaying) { }

        // If we have things to play queued up
        AudioClip toPlay = queuedTracks.Values[0];
        queuedTracks.RemoveAt(0);

        switch (toPlay.name) {
            // When we play ending, empty the queue
            case "14 Ending":
            // We just entered the hex room
                queuedTracks = new SortedList<int, AudioClip>();
                StartCoroutine(PlayClip(toPlay));
                break;

            // We are exiting trash room, clear the queue
            case "10 Trash Finish":
                queuedTracks = new SortedList<int, AudioClip>();
                StartCoroutine(PlayInSuccession(trashFinishedLines));
                break;

            // We grabbed the frisbee for the first time
            case "5 Grabbed":
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
                StartCoroutine(PlayClip(toPlay));
                break;

            // We haven't grabbed the frisbee yet
            case "Intro0 Bicycle":
                StartCoroutine(PlayFrisbeeListLines(lostFrisbeeLines, 3));
                break;

            // We are idling in the hex room
            case "13 Hex Lines":
                StartCoroutine(PlayFrisbeeListLines(hexRoomLines, 5));
                break;

            // We are idling in the trash room
            // Nothing here rn
            case "trashLines":
                StartCoroutine(PlayFrisbeeListLines(trashRoomLines, 5));
                break;

            // God knows why we'd be here
            default:
                break;
        }

        yield return new WaitForSeconds(0);
    }

    IEnumerator PlayClip(AudioClip clip)
    {
        isPlaying = true;
        instance.StopCoroutine(PlayRoutine());
        frisbeeAudioSource.PlayOneShot(clip);
        yield return new WaitForSeconds(clip.length);
        isPlaying = false;
        instance.StartCoroutine(PlayRoutine());
    }

    IEnumerator PlayFrisbeeListLines(List<AudioClip> listToPlay, int    
        bufferBetween) 
    {
        isPlaying = true;
        instance.StopCoroutine(PlayRoutine());
        int choose = Random.Range(0, listToPlay.Count);
        frisbeeAudioSource.PlayOneShot(listToPlay[choose]);
        yield return new WaitForSeconds(listToPlay[choose].length + bufferBetween);

        queuedTracks.Add(2, listToPlay[0]);
        isPlaying = false;
        instance.StartCoroutine(PlayRoutine());
    }

    IEnumerator PlayInSuccession(List<AudioClip> listToPlay) 
    {
        isPlaying = true;
        instance.StopCoroutine(PlayRoutine());
        foreach (AudioClip clip in listToPlay) {
            frisbeeAudioSource.PlayOneShot(clip);
            yield return new WaitForSeconds(clip.length);
        }

        isPlaying = false;
        instance.StartCoroutine(PlayRoutine());
    }

}


