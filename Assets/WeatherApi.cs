using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class WeatherApi : MonoBehaviour
{
    //Coordinates of Montreal can be used dynamically from 
    //LocationService but introduces a delay of 5-10Secs
    private string url = "https://api.darksky.net/forecast/03ba53582316de16f0927d3e795d3aaf/45.50328,-73.58464";
    public string skyCondition;
    public static WeatherApi Instance { get; set; }

    void Start()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
        StartCoroutine(GetRequest(url));
    }

    IEnumerator GetRequest(string url)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError)
            {
                Debug.Log(": Error: " + webRequest.error);
            }
            else
            {
                //Converting the web api get data to JSON
                JSONObject json = new JSONObject(webRequest.downloadHandler.text);
                skyCondition = json.GetField("currently").GetField("icon").str;
            }
        }
    }
}
