using Assets;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// The Plugin used to read ALS sensor value is work of Kaoru Shoji and can be found here:-
//https://github.com/kshoji/Unity-Android-Sensor-Plugin

public class ShadowScript : MonoBehaviour
{
    private GameObject lightGameObject;
    // Add the light component
    private Light lightComp;
    private LocationService locationService;
    private AndroidJavaObject plugin;
    private WeatherApi weatherApi;
    private string skyCondition;
    private float latitude;
    private float longitude;
    private string message;
    private double azimuthAngle;
    private double altitudeAngle;

    // Start is called before the first frame update
    void Start()
    {
        lightGameObject = new GameObject("Light");
        lightComp = lightGameObject.AddComponent<Light>();
        locationService = lightGameObject.AddComponent<LocationService>();
        weatherApi = lightGameObject.AddComponent<WeatherApi>();
        

        //Make a instance of the plugin for reading ambient light sensor value
#if UNITY_ANDROID
        plugin = new AndroidJavaClass("jp.kshoji.unity.sensor.UnitySensorPlugin").CallStatic<AndroidJavaObject>("getInstance");
        if (plugin != null)
        {
            plugin.Call("startSensorListening", "light");
        }
#endif
    }

    // Update is called once per frame
    void Update()
    {
        lightComp.shadows = LightShadows.Hard;
        lightComp.type = LightType.Directional;
        lightComp.color = Color.blue;


        //Getting GPS coordinates of the device from Location Service
        latitude = LocationService.Instance.latitude;
        longitude = LocationService.Instance.longitude;
        Debug.Log("Latitude: " + latitude + "Longitude: " + longitude);

        //Getting Azimuth and Alitutde angle according to Sun's current position using SunPosition Service
        //PSA algorithm can also be used from class SunPositionPSA
        //PSA algorithm was valid from 1999-2015 after that it introduces a error in calculatin SunPosition

        SunPosition sp = new SunPosition();
        sp.CalculateSunPosition(DateTime.Now, latitude, longitude);
        azimuthAngle = sp.azimuth;
        altitudeAngle = sp.altitude;
        Debug.Log("Azimuth: " + azimuthAngle + " altitude: " + altitudeAngle);

        //Estimating the position of sun according to the users GPS coordinates(latitude,longitude)
        calculateSunDirection(azimuthAngle, altitudeAngle);

        //Hitting the web api to find out the current skycondition(Cloudy,Partly Cloudy,Clear sky)
        skyCondition = WeatherApi.Instance.skyCondition;
		skyCondition = "clear-day";
        float[] sensorValue = null;
        //Calculating shadow Strength according to SkyConditions
#if UNITY_ANDROID
        if (plugin != null)
        {
            //Initializing the plugin to read Ambient sensor light value in lux units
            sensorValue = plugin.Call<float[]>("getSensorValues", "light");
            if (sensorValue != null)
            {
                lightComp.shadowStrength = calculateShadowIntensity(sensorValue[0], skyCondition);
            }
            else
                lightComp.shadowStrength = 0F;
        }
    #endif

        message = "The shadow strength is: " + lightComp.shadowStrength + " ALS value is: " + sensorValue[0] +
           " Latitude is: " + latitude + " Longitude is: " + longitude + " Sky Condition is: " + skyCondition +
           " Azimuth is: " + azimuthAngle + " Altitude is: " + altitudeAngle;
    }

    public void calculateSunDirection(double currentAzimuthAngle, double currentAltitudeAngle)
    {
        float xrot = Math.Abs(Convert.ToSingle((currentAltitudeAngle)));
        float yrot = Math.Abs(Convert.ToSingle(90 - currentAzimuthAngle));
        Vector3 rotation = new Vector3();
        rotation.x = xrot;
        rotation.y = yrot;
        //Transforming rotation as Euler angles in degrees.
        //x, y, and z angles represent a rotation z degrees around the z axis, x degrees around the x axis, and y degrees around the y axis.
        lightGameObject.transform.localRotation = Quaternion.Euler(rotation);
    }


    //Adjusting shadow intensity according to the sky conditions
    public float calculateShadowIntensity(float currentALSValue, String currentSkyCondition)
    {

        float currentShadowIntensity;
        //Here currentShadowIntensity can be reduced to 100 (actual 6000) for testing purposes
        if (currentALSValue > 6000)
        {
            if (currentSkyCondition == "clear-day")
            {
                currentShadowIntensity = ((0.85f * currentALSValue) / 32768);
            }
            //value can be changed to 2500(actual 25000) for testing purposes    
            else if (currentSkyCondition == "partly-cloudy-day")
                currentShadowIntensity = ((0.50f * currentALSValue) / 25000);
            //Here currentShadowIntensity can be set to 0.2(actual 0) for testing purposes covering cases(clear-night,rain,snow,sleet,wind,fog,cloudy,partly-cloudy-night)
            else
                currentShadowIntensity = 0.0F;
        }
        //Here currentShadowIntensity can be set to 0.2(actual 0) for testing purposes
        else
            currentShadowIntensity = 0.0f;

        return currentShadowIntensity;
    }

    void OnGUI()
    {
        if (message != null)
        {
            GUI.skin.label.fontSize = 55;
            GUI.Label(new Rect(Screen.width / 5, 100f, 1000f, 1000f), message);
        }
    }

    void OnApplicationQuit()
    {
    #if UNITY_ANDROID
        if (plugin != null)
        {
            plugin.Call("terminate");
            plugin = null;
        }
    #endif
    }
}
