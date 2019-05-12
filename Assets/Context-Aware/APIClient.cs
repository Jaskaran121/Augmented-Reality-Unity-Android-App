using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Xml;
using UnityEngine;

public class APIClient 
{
    public string GetSkyConditions()
    {
        WebClient client = new WebClient();
        client.Headers["User-Agent"] = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/71.0.3578.98 Safari/537.36";

        XmlDocument doc = new XmlDocument();

        //string html = client.DownloadString("https://www.accuweather.com/en/ca/montreal/h3a/astronomy-hourly-forecast/56186");

        //doc.LoadXml(html);

        //string skyCondition = doc.SelectSingleNode(".//div[@class='hourly-table sky-hourly']//tr[2]/td[1]").InnerText;
        string skyCondition = "75%";
        skyCondition = skyCondition.Replace("%", "");

        return skyCondition;
    }

}
