using System;
using System.Collections;
using SimpleJSON;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;

namespace R2mv.Weather
{
    public class OpenWeatherDataManager : MonoBehaviour
    {

        public const string OnWeatherUpdateEventName = "OpenWeatherDataManager.OnWeatherUpdate";
        public const string UpdateCityEventName = "OpenWeatherDataManager.UpdateCity";

        
        /// <summary>
        /// OpenWeatherDataManager is a singleton, use Instance to access it.
        /// </summary>
        public static OpenWeatherDataManager Instance;

        public enum QueryMode
        {
            CityName,
            LatLon
        }

        public QueryMode Mode = QueryMode.LatLon;
        
        /// <summary>
        /// Standard C# Unit Action called every time the weather data is updated.
        /// </summary>
        public Action<OpenWeatherData> OnWeatherDataUpdate;
        
        /// <summary>
        /// API settings OpenWeatherAPISettings (ScriptableObject), must be created using the Create Asset Menu > Road to the Metaverse > OpenWeather API Settings
        /// </summary>
        public OpenWeatherAPISettings ApiSettings;

        public string CityName = "London";
        
        /// <summary>
        /// Lattitude (default is London's)
        /// </summary>
        public float Lattitude = 51.5072f;
        
        /// <summary>
        /// Longiture (default is London's)
        /// </summary>
        public float Longitude = 0.1276f;
        
        /// <summary>
        /// If true, the manager will fetch weather data on Start   
        /// </summary>
        public bool FetchOnStart = true;
        
        /// <summary>
        /// Interval in Seconds
        /// If not zero, the manager will fetch weather data at that interval.
        /// </summary>
        [Range(0, 60)]
        public int FetchInterval = 0;

        /// <summary>
        /// Latest weather data
        /// </summary>
        public OpenWeatherData latestWeatherData = new OpenWeatherData();

        /// <summary>
        /// Shorthand to check if the manager should auto update the weather data
        /// </summary>
        private bool autoUpdate => FetchInterval != 0;

        private float remainingTime;
        private bool fetchingData;
        private void Awake()
        {
            
            // We enforce the singleton on awake 
            if (Instance != null)
            {
                Debug.LogFormat("{0} already a OpenWeatherDataManager loaded. Removing the one on {1}.", Instance.name, this.name);
                DestroyImmediate(this);
            }
            else Instance = this;
            
        }

        private void Start()
        {
            
            EventBus.Register<string>(UpdateCityEventName, UpdateCity);
            
            if (FetchOnStart)
            {
                StartCoroutine(FetchWeatherData());
            }

            remainingTime = (float) FetchInterval;
            
        }

        private void Update()
        {
            if (autoUpdate && !fetchingData)
            {
                remainingTime -= Time.deltaTime;

                if (remainingTime <= 0f)
                {
                    StartCoroutine(FetchWeatherData());
                    remainingTime = FetchInterval;
                }
            }
        }


        public void UpdateCity(string city)
        {

            Mode = QueryMode.CityName;
            CityName = city;
            
            StartCoroutine(FetchWeatherData());

        }
        
        private IEnumerator FetchWeatherData()
        {

            fetchingData = true;

            if (ApiSettings == null)
            {
                Debug.LogError("OpenWeatherDataManager: Please assign an OpenWeather API Settings asset. You can create one by using the Create Asset Menu > Road to the Metaverse > OpenWeather API Settings");
                yield break;
            }
            
            var url = Mode == QueryMode.LatLon
                ? ApiSettings.GetLatLonQueryURL(Lattitude, Longitude)
                : ApiSettings.GetCityQueryURL(CityName);
            
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

                fetchingData = false;

            }
            
        }

        private void UpdateWeatherData(string results)
        {
            var weatherJson = JSON.Parse(results);

            Debug.Log(weatherJson);
            
            // Update location data
            if (Mode == QueryMode.CityName)
            {
                latestWeatherData.city = CityName;    
                Lattitude = weatherJson["coord"]["lat"].AsFloat;
                Longitude = weatherJson["coord"]["lon"].AsFloat;
            }

            latestWeatherData.LatLon = new Vector2(Lattitude, Longitude);
            
            // Update weather data
            latestWeatherData.description = weatherJson["weather"][0]["description"].Value;
            latestWeatherData.id = weatherJson["weather"][0]["id"].AsInt;
            
            latestWeatherData.temp = weatherJson["main"]["temp"].AsFloat;
            latestWeatherData.tempMIN = weatherJson["main"]["temp_min"].AsFloat;
            latestWeatherData.tempMAX = weatherJson["main"]["temp_max"].AsFloat;

            // Update temperature units string
            switch (ApiSettings.Units)
            {
                case OpenWeatherAPISettings.TemperatureUnits.Celsius:
                    latestWeatherData.tempUnit = "C";
                    break;
                case OpenWeatherAPISettings.TemperatureUnits.Fahrenheit:
                    latestWeatherData.tempUnit = "F";
                    break;
                default:
                    latestWeatherData.tempUnit = "K";
                    break;
            }
            
            latestWeatherData.rain = weatherJson["rain"]["3h"].AsFloat;
            latestWeatherData.clouds = weatherJson["clouds"]["all"].AsInt;
            
            latestWeatherData.wind = weatherJson["wind"]["speed"].AsFloat;

           // Load weather icon in another coroutine
           StartCoroutine(GetWeatherIcon(weatherJson["weather"][0]["icon"].Value));

        }
        
        
        private IEnumerator GetWeatherIcon(string icon)
        {

            var url = ApiSettings.GetWeatherIconURL(icon);
            
            UnityWebRequest webRequest = UnityWebRequestTexture.GetTexture(url);
            yield return webRequest.SendWebRequest();

            if (webRequest.result != UnityWebRequest.Result.Success) {
                Debug.Log(webRequest.error);
            }
            else {
                Texture iconTexture = ((DownloadHandlerTexture)webRequest.downloadHandler).texture;
                latestWeatherData.icon = iconTexture;
            }
            
            // Data is loaded, send events to C# delegates...
            OnWeatherDataUpdate?.Invoke(latestWeatherData);
            
            // And send event to Visual Scripts handlers!
            EventBus.Trigger(OnWeatherUpdateEventName, latestWeatherData);
            
        }

    }

    
    [Serializable]
    public struct OpenWeatherData
    {
        // Location info
        public string city;
        public Vector2 LatLon;
        
        // Weather data
        public string description;
        public float temp;
        public float tempMIN;
        public float tempMAX;
        public string tempUnit;
        public float rain;
        public float wind;
        public int clouds;

        // Weather icon
        public int id;
        public Texture icon;

    }
    
    
}