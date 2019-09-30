using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    #pragma warning disable 0649
    private SteamVR_Input_Sources LeftInputSource = SteamVR_Input_Sources.LeftHand;
    private SteamVR_Input_Sources RightInputSource = SteamVR_Input_Sources.RightHand;
    [SerializeField] private string startScene = "Big Trash Room";

    public static GameManager S;
    private static int destroyedObjects = 0;
    // TODO: auto populate this by having destroyable objs send mssg to gm
    private static int totalObjectsToWin = 0;
    [SerializeField] private GameObject winScreen;
    [SerializeField] private GameObject cameraRig;
    [SerializeField] private GameObject frisbee;
    [SerializeField] private Vector3 placeToMove;
    [SerializeField] private Vector3 frisbeePlaceToMove;

    //TODO: player is currently just the right glove!
    [SerializeField] public GameObject player;
    bool won = false;
    #pragma warning restore 0649

    void Awake(){
        S = this;
    }

    void Start(){
        // Fade in effect
        //if(SceneManager.GetActiveScene().name != startScene){
			SteamVR_Fade.Start(Color.black, 0);
			SteamVR_Fade.Start(Color.clear, 2);
        //}
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

    public void LoadScene(string sceneName){
        winScreen.SetActive(false);
        Debug.Log("false");
        SceneManager.LoadScene(sceneName);

        //TODO: create resets function
        destroyedObjects = 0;
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
            // TODO: enable this if you guys decide on canvas screen!
            //winScreen.SetActive(true);
            //TODO: coroutine to black out screen
            won = true;
            StartCoroutine(blackOutScreen());
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
        AsyncOperation AO = SceneManager.LoadSceneAsync("Hexagon_Lisa"); 
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

        //SceneManager.UnloadSceneAsync(startScene);
    }
}