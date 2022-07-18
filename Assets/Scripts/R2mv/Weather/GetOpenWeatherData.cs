using System;
using System.Collections;
using SimpleJSON;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace R2mv.Weather
{
    public class GetOpenWeatherData :MonoBehaviour
    {

        
        public string AppID;
        public string BaseDataURL; // http://api.openweathermap.org/data/2.5/weather
        public string BaseIconURL; // http://openweathermap.org/img/wn/
        public string CityName;
        public TextMeshProUGUI CityText;
        public TextMeshProUGUI TemperatureText;
        public TextMeshProUGUI DescriptionText;
        public RawImage WeatherIcon;

        private void Start()
        {
            StartCoroutine(FetchWeatherData());
        }

        private void Update()
        {
            if(Input.GetKeyUp(KeyCode.Space))
                StartCoroutine(FetchWeatherData());
        }

        private IEnumerator FetchWeatherData()
        {

            var url = String.Format("{0}?q={1}&appid={2}&units={3}",
                BaseDataURL,
                CityName,
                AppID,
                "metric");
            
            using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
            {
                // Request and wait for the desired page.
                yield return webRequest.SendWebRequest();

                switch (webRequest.result)
                {
                    case UnityWebRequest.Result.ConnectionError:
                    case UnityWebRequest.Result.DataProcessingError:
                        Debug.LogError("Error: " + webRequest.error);
                        break;
                    case UnityWebRequest.Result.ProtocolError:
                        Debug.LogError("HTTP Error: " + webRequest.error);
                        break;
                    case UnityWebRequest.Result.Success:
                        UpdateWeatherData(webRequest.downloadHandler.text);
                        break;
                }
            }
            
        }

        private void UpdateWeatherData(string results)
        {
            var weatherJson = JSON.Parse(results);

            Debug.Log(weatherJson);
            
            CityText.text = CityName.ToUpper();
            DescriptionText.text = weatherJson["weather"][0]["description"].Value.ToUpper();
            TemperatureText.text =  weatherJson["main"]["temp"].AsInt + "°c";

            StartCoroutine(GetTexture(weatherJson["weather"][0]["icon"].Value));
        }
        
        
        private IEnumerator GetTexture(string icon) {
            
            var url = String.Format("{0}{1}@2x.png", BaseIconURL, icon);
            
            UnityWebRequest webRequest = UnityWebRequestTexture.GetTexture(url);
            yield return webRequest.SendWebRequest();

            
            
            if (webRequest.result != UnityWebRequest.Result.Success) {
                Debug.Log(webRequest.error);
            }
            else {
                Texture iconTexture = ((DownloadHandlerTexture)webRequest.downloadHandler).texture;
                WeatherIcon.texture = iconTexture;
            }
        }
    }
}