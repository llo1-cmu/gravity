using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    //priority list: 0 ending, 1 trashroomending, 2 frisbeegrabbed, 3 newplace,
    // 4 lostfrisbeelines, 5 success, 6 fail, 7 trashroomlines, 8 hexroomlines
    #pragma warning disable 0649
    [SerializeField] AudioSource shipAudioSource, frisbeeAudioSource;
    [SerializeField] AudioClip frisbeeThrow, frisbeeRecall, frisbeeAbsorb, frisbeeCatch;
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


    /*********************
        Small SFX
    *********************/
    public void PlayFrisbeeCatch() {
        frisbeeAudioSource.PlayOneShot(frisbeeCatch);
    }

    public void PlayRecall() {
        frisbeeAudioSource.PlayOneShot(frisbeeRecall);
    }

    public void PlayThrow() {
        frisbeeAudioSource.PlayOneShot(frisbeeThrow);
    }

    public void PlayAbsorb() {
        frisbeeAudioSource.PlayOneShot(frisbeeAbsorb);
    }

    /*********************
        Longer Sounds
    *********************/

    // Continue to call these until the frisbee has been grabbed as it drifts towards the player in the intro scene
    public void PlayFrisbeePrompt()
    {
        //print("something");
        queuedTracks.Add(4, lostFrisbeeLines[0]);
        instance.StartCoroutine(PlayRoutine());
    }

    // When we grab the frisbee for the first time
    public void PlayFrisbeeGrabbed()
    {
        frisbeeAudioSource.Stop();
        queuedTracks.Add(2, frisbeeGrabbedLines[0]);
        instance.StopAllCoroutines();
        isPlaying = false;
        instance.StartCoroutine(PlayRoutine());
    }

    // Plays when you throw for the first time but don't suck in an item
    public void PlayItemFail()
    {
        queuedTracks.Add(6, failedItem);
    }

    // Plays when you throw for the first time and do suck in an item
    public void PlayItemSucceed()
    {
        queuedTracks.Add(5, successfulItem);
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
        instance.StartCoroutine(PlayRoutine());
    }

    // Plays on occasion in the hex room
    public void HexLine()
    {
        queuedTracks.Add(8, hexRoomLines[0]);
    }

    // Plays when we enter the hex room for the first time
    public void PlayHexEntrance() {
        queuedTracks.Add(3, newPlaceLines[0]);
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
        isPlaying = true;
        instance.StopAllCoroutines();
        frisbeeAudioSource.PlayOneShot(clip);
        yield return new WaitForSeconds(clip.length);
        isPlaying = false;
        instance.StartCoroutine(PlayRoutine());
    }

    IEnumerator PlayFrisbeeListLines(List<AudioClip> listToPlay, int    
        bufferBetween, int priority) 
    {
        isPlaying = true;
        instance.StopAllCoroutines();
        int choose = Random.Range(0, listToPlay.Count);
        frisbeeAudioSource.PlayOneShot(listToPlay[choose]);

        yield return new WaitForSeconds(bufferBetween);
        yield return new WaitForSeconds(listToPlay[choose].length);

        queuedTracks.Add(priority, listToPlay[0]);
        isPlaying = false;
        instance.StartCoroutine(PlayRoutine());
    }

    IEnumerator PlayInSuccession(List<AudioClip> listToPlay) 
    {
        isPlaying = true;
        instance.StopAllCoroutines();
        foreach (AudioClip clip in listToPlay) {
            frisbeeAudioSource.PlayOneShot(clip);
            yield return new WaitForSeconds(clip.length);
        }

        isPlaying = false;
        instance.StartCoroutine(PlayRoutine());
    }

}


