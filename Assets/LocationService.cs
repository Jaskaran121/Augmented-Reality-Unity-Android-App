using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationService : MonoBehaviour
{

    public float latitude;
    public float longitude;
    public static LocationService Instance { get; set; }

    void Start()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
        StartCoroutine(StartLocationService());
    }

    private IEnumerator StartLocationService()
    {
        // Start service before querying location
        Input.location.Start();

        // Wait until service initializes
        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        // Service didn't initialize in 20 seconds
        if (maxWait < 1)
        {
            latitude = 45.50328f;
            longitude = -73.58464f;
            yield break;
        }

        // Connection has failed
        if (Input.location.status == LocationServiceStatus.Failed)
        {
            latitude = 45.50328f;
            longitude = -73.58464f;
            yield break;
        }

        // Access granted and location value could be retrieved
        latitude = Input.location.lastData.latitude;
        longitude = Input.location.lastData.longitude;
        yield break;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
