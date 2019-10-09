using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class DestroyableObj : MonoBehaviour
{

    //TODO: add destroyableobj to layer mask, add tier, add audiosource
    #pragma warning disable 0649
    [SerializeField] private int tier;
    [SerializeField] private int pointValue = 5;
    [SerializeField] private LayerMask destroyableObjMask;
    private bool rigidBodyExists;
    private bool useGravity;
    private bool suckedIn = false;
    private Color matColor;
    private Vector3 originalScale;
    private Transform frisbee;
    private Vector3 originalPosition;
    #pragma warning restore 0649
    private AudioSource audioSource;
    new Rigidbody rigidbody;
    new Renderer renderer;

    /* Copied from SteamVR's Interactable.cs */
    protected MeshRenderer[] highlightRenderers;
    protected MeshRenderer[] existingRenderers;
    protected GameObject highlightHolder;
    protected SkinnedMeshRenderer[] highlightSkinnedRenderers;
    protected SkinnedMeshRenderer[] existingSkinnedRenderers;
    protected static Material highlightMat;
    /* End copied code */
    private bool highlighted, gravityDisabled;
    void Start(){
        rigidbody = GetComponent<Rigidbody>();
        renderer = GetComponent<Renderer>();
        audioSource = GetComponent<AudioSource>();

        GameManager.S.AddTieredObject(tier);

        if(rigidbody){
            rigidBodyExists = true;
        }
        else{
            rigidBodyExists = false;
        }
        if(renderer){
            matColor = renderer.material.color;
        }
        originalScale = transform.localScale;
        audioSource.spatialBlend = 0.7f;
        audioSource.volume = 0.3f;
    }

    public int GetTier(){
        return tier;
    }
    

    void Update() {
        // If gravity is disabled and we aren't the max tier (aka final reactor)
        if (GameManager.S.GetBroadcastGravityDisabled() && !gravityDisabled && GameManager.S.GetMaxTier() != tier) {
            rigidbody.useGravity = false;
            rigidbody.isKinematic = false;
            useGravity = false;
            gravityDisabled = true;
        }
        if (GameManager.S.GetCurrentTier() >= tier){
            if(!highlighted){
                highlighted = true;
                CreateHighlightRenderers();
            }
            UpdateHighlightRenderers();
        }
    }

    // void OnTriggerStay(Collider other) {
    //     if (other.tag != "gravity field") return;

    //     // If we are strong enough to pick up the other object
    //     if (GameManager.S.GetDestroyedScore() >= threshold && !beingDestroyed) {
    //         // transform.parent = other.transform;
    //         // Vector faces towards the center of the gravity field
    //         // var vec = other.transform.position - transform.position;

    //         // Normalize the magnitude b/w 0 and 1 to put on the force curve
    //         // float normDist = Mathf.Clamp01(vec.magnitude / _Collider.radius);
    //         //if (vec.magnitude > _Collider.radius) normDist = 1;

    //         // Moves based on time and not distance
    //         // transform.position = Vector3.MoveTowards(transform.position, other.transform.position, Time.deltaTime * 5);

    //         // Decrease other object's size exponentially
    //         // TODO: why does size increase?
    //         // if (vec.magnitude < 1f && vec.magnitude > 0.05f && other.transform.localScale.magnitude > 0.001f) {
    //         //     other.transform.localScale *= vec.magnitude;
    //         // }

    //         // Moved moving and shrinking into the coroutine
            
    //     }
    // }

    void OnTriggerEnter(Collider other) {
        // If we clash into another object when gravity disable or we're 
        // being sucked in
        if ((suckedIn || gravityDisabled) && ((1 << other.gameObject.layer) & destroyableObjMask) != 0) {
            SoundManager.instance.PlayDebrisHit(audioSource);
        }
        // Once object gets close enough to touch frisbee, destroy it
        // if (other.tag == "Frisbee") {
        //     other.GetComponentInParent<Frisbee>().IncreaseGravityField();
        //     if (GameManager.S.GetDestroyedScore() >= threshold) StartCoroutine(DisappearEffect(0.25f));
        // }

        // Suspend enviornmental gravity when being pulled by the frisbee
        if (other.tag == "gravity field" && GameManager.S.GetCurrentTier() >= tier) {
            suckedIn = true;

            RaycastHit hit;
            // If there's no force field in the way
            if(!Physics.Raycast(other.transform.position, (transform.position - other.transform.position).normalized, out hit, (transform.position - other.transform.position).magnitude, LayerMask.GetMask("Force Field"))){
                if(rigidBodyExists){
                    if(useGravity){
                        rigidbody.useGravity = false;
                    }
                    rigidbody.isKinematic = true;
                    //rigidbody.detectCollisions = false;
                }

                frisbee = other.transform;
                originalPosition = transform.position;

                SoundManager.instance.PlayAbsorb();
                StartCoroutine(DisappearEffect(0.5f));
                frisbee.GetComponentInParent<Frisbee>().IncreaseGravityField();
                
                GravityField gf = other.GetComponent<GravityField>();
                if (gf.firstItemSucceed) return;
                gf.firstItemSucceed = true;
                SoundManager.instance.PlayItemSucceed();
            }
            else{
                print("Obstructed by force field: " + hit.transform.name);
            }
        }

        // Play fail-to-absorb sound if we haven't already
        else if (other.tag == "gravity field" && GameManager.S.GetCurrentTier() < tier) {
            suckedIn = false;
            GravityField gf = other.GetComponent<GravityField>();
            if (gf.firstItemFailed) return;
            gf.firstItemFailed = true;
            SoundManager.instance.PlayItemFail();
        }
    }

    IEnumerator DisappearEffect(float timeToWait) {
        float startTime = Time.time;
        //yield return new WaitForSeconds(timeToWait);
        while(Time.time - startTime < timeToWait){
            transform.position = Vector3.Lerp(originalPosition, frisbee.position, (Time.time-startTime)/timeToWait);
            transform.localScale = originalScale * (1f - Mathf.Pow((Time.time-startTime)/timeToWait, 2f) );
            yield return null;
        }

        GameManager.S.UpdateDestroyedScore(tier);
        if (highlightHolder != null) Destroy(highlightHolder);
        Destroy(gameObject);
    }

    /* Copied from SteamVR's Interactable.cs */
    protected virtual void CreateHighlightRenderers()
    {
        highlightMat = (Material)Resources.Load("SteamVR_HoverHighlight", typeof(Material));

        if (highlightMat == null)
            Debug.LogError("<b>[SteamVR Interaction]</b> Hover Highlight Material is missing. Please create a material named 'SteamVR_HoverHighlight' and place it in a Resources folder");

        existingSkinnedRenderers = this.GetComponentsInChildren<SkinnedMeshRenderer>(true);
        highlightHolder = new GameObject("Highlighter");
        highlightSkinnedRenderers = new SkinnedMeshRenderer[existingSkinnedRenderers.Length];

        for (int skinnedIndex = 0; skinnedIndex < existingSkinnedRenderers.Length; skinnedIndex++)
        {
            SkinnedMeshRenderer existingSkinned = existingSkinnedRenderers[skinnedIndex];

            // if (ShouldIgnoreHighlight(existingSkinned))
            //     continue;

            GameObject newSkinnedHolder = new GameObject("SkinnedHolder");
            newSkinnedHolder.transform.parent = highlightHolder.transform;
            SkinnedMeshRenderer newSkinned = newSkinnedHolder.AddComponent<SkinnedMeshRenderer>();
            Material[] materials = new Material[existingSkinned.sharedMaterials.Length];
            for (int materialIndex = 0; materialIndex < materials.Length; materialIndex++)
            {
                materials[materialIndex] = highlightMat;
            }

            newSkinned.sharedMaterials = materials;
            newSkinned.sharedMesh = existingSkinned.sharedMesh;
            newSkinned.rootBone = existingSkinned.rootBone;
            newSkinned.updateWhenOffscreen = existingSkinned.updateWhenOffscreen;
            newSkinned.bones = existingSkinned.bones;

            highlightSkinnedRenderers[skinnedIndex] = newSkinned;
        }

        MeshFilter[] existingFilters = this.GetComponentsInChildren<MeshFilter>(true);
        existingRenderers = new MeshRenderer[existingFilters.Length];
        highlightRenderers = new MeshRenderer[existingFilters.Length];

        for (int filterIndex = 0; filterIndex < existingFilters.Length; filterIndex++)
        {
            MeshFilter existingFilter = existingFilters[filterIndex];
            MeshRenderer existingRenderer = existingFilter.GetComponent<MeshRenderer>();

            if (existingFilter == null || existingRenderer == null /*|| ShouldIgnoreHighlight(existingFilter)*/)
                continue;

            GameObject newFilterHolder = new GameObject("FilterHolder");
            newFilterHolder.transform.parent = highlightHolder.transform;
            MeshFilter newFilter = newFilterHolder.AddComponent<MeshFilter>();
            newFilter.sharedMesh = existingFilter.sharedMesh;
            MeshRenderer newRenderer = newFilterHolder.AddComponent<MeshRenderer>();

            Material[] materials = new Material[existingRenderer.sharedMaterials.Length];
            for (int materialIndex = 0; materialIndex < materials.Length; materialIndex++)
            {
                materials[materialIndex] = highlightMat;
            }
            newRenderer.sharedMaterials = materials;

            highlightRenderers[filterIndex] = newRenderer;
            existingRenderers[filterIndex] = existingRenderer;
        }
    }

    protected virtual void UpdateHighlightRenderers()
    {
        if (highlightHolder == null)
            return;

        for (int skinnedIndex = 0; skinnedIndex < existingSkinnedRenderers.Length; skinnedIndex++)
        {
            SkinnedMeshRenderer existingSkinned = existingSkinnedRenderers[skinnedIndex];
            SkinnedMeshRenderer highlightSkinned = highlightSkinnedRenderers[skinnedIndex];

            if (existingSkinned != null && highlightSkinned != null /*&& attachedToHand == false*/)
            {
                highlightSkinned.transform.position = existingSkinned.transform.position;
                highlightSkinned.transform.rotation = existingSkinned.transform.rotation;
                highlightSkinned.transform.localScale = existingSkinned.transform.lossyScale;
                highlightSkinned.localBounds = existingSkinned.localBounds;
                highlightSkinned.enabled = /*isHovering &&*/ existingSkinned.enabled && existingSkinned.gameObject.activeInHierarchy;

                int blendShapeCount = existingSkinned.sharedMesh.blendShapeCount;
                for (int blendShapeIndex = 0; blendShapeIndex < blendShapeCount; blendShapeIndex++)
                {
                    highlightSkinned.SetBlendShapeWeight(blendShapeIndex, existingSkinned.GetBlendShapeWeight(blendShapeIndex));
                }
            }
            else if (highlightSkinned != null)
                highlightSkinned.enabled = false;

        }

        for (int rendererIndex = 0; rendererIndex < highlightRenderers.Length; rendererIndex++)
        {
            MeshRenderer existingRenderer = existingRenderers[rendererIndex];
            MeshRenderer highlightRenderer = highlightRenderers[rendererIndex];

            if (existingRenderer != null && highlightRenderer != null /*&& attachedToHand == false*/)
            {
                highlightRenderer.transform.position = existingRenderer.transform.position;
                highlightRenderer.transform.rotation = existingRenderer.transform.rotation;
                highlightRenderer.transform.localScale = existingRenderer.transform.lossyScale;
                highlightRenderer.enabled = /*isHovering &&*/ existingRenderer.enabled && existingRenderer.gameObject.activeInHierarchy;
            }
            else if (highlightRenderer != null)
                highlightRenderer.enabled = false;
        }
    }
    /* End copied code */
}
