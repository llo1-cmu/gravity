using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    #pragma warning disable 0649
    // Steam VR things
    private SteamVR_Input_Sources LeftInputSource = SteamVR_Input_Sources.LeftHand;
    private SteamVR_Input_Sources RightInputSource = SteamVR_Input_Sources.RightHand;

    // Scene Names
    [SerializeField] private string startScene = "Big Trash Room";
    [SerializeField] private string endScene = "Hexagon_Lisa";

    // Game Manager Vars
    public static GameManager S;
    private static int destroyedObjects = 0;
    // TODO: auto populate this by having destroyable objs send mssg to gm
    private static int totalObjectsToWin = 0;
    bool won = false;

    // GameObject Components
    // Player is currently just the right glove!
    [SerializeField] public GameObject player;
    // [SerializeField] private GameObject frisbee;
    // [SerializeField] private GameObject winScreen;
    // [SerializeField] private GameObject cameraRig;
    #pragma warning restore 0649

    void Awake(){
        S = this;
    }

    void Start(){
        SteamVR_Fade.Start(Color.black, 0);
        SteamVR_Fade.Start(Color.clear, 2);
    }

    public void AddDestroyableObject(){
        totalObjectsToWin++;
    }

    public void UpdateDestroyedScore() {
        destroyedObjects++;
    }

    public int GetDestroyedScore() {
        return destroyedObjects;
    }

    void Update() {
        if(SteamVR_Actions._default.GrabGrip.GetState(RightInputSource) && 
            SteamVR_Actions._default.GrabGrip.GetState(LeftInputSource)) {
            // If someone holds both grips, reset the game
            destroyedObjects = 0; // reset the stats
            won = false;
            SceneManager.LoadScene(startScene);
        }

        if ((destroyedObjects >= totalObjectsToWin && !won) || Input.GetButtonUp("Fire3")) {
            if (SceneManager.GetActiveScene().name == startScene) {
                SoundManager.instance.PlayTrashFinish();
                won = true;
                StartCoroutine(blackOutScreen());
            }
            else /* we want ending to play */ {
                SoundManager.instance.PlayEnding();
            }
        }
    }

    IEnumerator blackOutScreen(){
        Tutorial.PlayWarning();
        //black out screen
        SteamVR_Fade.Start(Color.black, 7.0f);
        //GetComponent<SteamVR_LoadLevel>().levelName = "Hexagon";
        //GetComponent<SteamVR_LoadLevel>().Trigger();
        yield return new WaitForSeconds(7);
        Application.backgroundLoadingPriority = ThreadPriority.Low;
        AsyncOperation AO = SceneManager.LoadSceneAsync(endScene); 
        AO.allowSceneActivation = false;

        //frisbee.transform.position = frisbeePlaceToMove;
        //cameraRig.transform.position = placeToMove;
        //SceneManager.LoadScene("Hexagon");
        //SteamVR_Fade.Start(Color.clear, 2.0f);
        
        while(AO.progress < 0.9f)
        {
            yield return null;
        }
        AO.allowSceneActivation = true;
        destroyedObjects = 0;
        won = false;

        // TODO: move these to a SOLID start of the new room!
        SoundManager.instance.PlayHexEntrance();
        SoundManager.instance.HexLine();

        //SceneManager.UnloadSceneAsync(startScene);
    }
}