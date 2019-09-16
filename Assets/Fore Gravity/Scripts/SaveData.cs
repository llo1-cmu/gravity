using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SaveData : MonoBehaviour
{
    static string filePath = @"/frisbee_data.csv";
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static void DataSave(List<Vector3> frisbeePositions, int target){
        // get feedback on bounciness of walls
        // create collider planes
        // if File.Exists (File.AppendAllText(filePath, string);
        string toWrite = "Target " + target + ",";
        foreach (Vector3 pos in frisbeePositions) {
            toWrite += pos.x + " " + pos.y + " " + pos.z + ",";
        }
        if (File.Exists(filePath)) {
            File.AppendAllText(filePath, toWrite);
        }
        else {
            File.WriteAllText(filePath, toWrite);
        }
    }
}
