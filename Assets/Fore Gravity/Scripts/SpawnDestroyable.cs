using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnDestroyable : MonoBehaviour
{
    [SerializeField] private Transform mainCamera;
    [SerializeField] private Collider spawnzone;
    private Bounds boundingVolume;
    [SerializeField] private GameObject spawnPrefab;
    [SerializeField] private int spawnScoreThreshold = 1;
    [SerializeField] private float spawnDelay = 5.0f;
    private float lastSpawnTime;
    // Start is called before the first frame update
    void Start()
    {
        boundingVolume = spawnzone.bounds;
    }

    // Update is called once per frame
    void Update()
    {
        if(GameManager.S.GetDestroyedScore() >= spawnScoreThreshold && Time.time > lastSpawnTime+spawnDelay){
            GameObject spawnedObject = Instantiate(spawnPrefab, new Vector3(Random.Range(boundingVolume.min.x, boundingVolume.max.x),
                                                                            Random.Range(boundingVolume.min.y, boundingVolume.max.y),
                                                                            Random.Range(boundingVolume.min.z, boundingVolume.max.z)),
                                                Quaternion.Euler(new Vector3(Random.Range(0.0f, 360.0f),
                                                                             Random.Range(0.0f, 360.0f),
                                                                             Random.Range(0.0f, 360.0f))));
            spawnedObject.GetComponent<Rigidbody>().angularVelocity = new Vector3(Random.Range(0.0f, 1.0f),
                                                                                  Random.Range(0.0f, 1.0f),
                                                                                  Random.Range(0.0f, 1.0f));
            spawnedObject.GetComponent<Rigidbody>().velocity = Vector3.Normalize(mainCamera.position - spawnedObject.transform.position);
            lastSpawnTime = Time.time;
        }
    }
}
