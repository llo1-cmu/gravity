﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Siren : MonoBehaviour
{

    [SerializeField] private float speed; //In degrees per second, can be negative to spin other direction
    [SerializeField] private bool isTriggered;
    [SerializeField] GameObject lightSource;
    [SerializeField] AudioSource sirenSource;

    private float currentRotationY = 0;

    // Start is called before the first frame update
    void Start()
    {
        currentRotationY = gameObject.transform.rotation.y;
    }

    // Update is called once per frame
    void Update()
    {
        if (isTriggered)
        {
            
            Spin();

        }
    }

    void sirenStart()
    {
        lightSource.SetActive(true);
        if (sirenSource != null)
        {
            sirenSource.Play();
            sirenSource.loop = true;
        }
       

    }
    
    void Spin()
    {
        transform.Rotate(new Vector3(0, speed * Time.deltaTime, 0), Space.World);
    }

    public void Activate()
    {
        isTriggered = true;
  
        sirenStart();
    } 

    public void Deactivate()
    {
        isTriggered = false;
    }
}
