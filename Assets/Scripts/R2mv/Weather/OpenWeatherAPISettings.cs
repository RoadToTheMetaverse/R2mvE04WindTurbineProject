using System;
using UnityEngine;

namespace R2mv.Weather
{
    [CreateAssetMenu(menuName = "Road to the Metaverse/OpenWeather API Settings")]
    public class OpenWeatherAPISettings : ScriptableObject
    {
        public enum TemperatureUnits
        {
            Kelvin,
            Celsius,
            Fahrenheit
            
        }
        
        public string AppID = "Enter your App ID Here!";
        public string BaseDataURL = "http://api.openweathermap.org/data/2.5/weather";
        public string BaseIconURL = "http://openweathermap.org/img/wn/";

        public TemperatureUnits Units = TemperatureUnits.Celsius;

        public string GetLatLonQueryURL(float lat, float lon)
        {
            return String.Format("{0}?lat={1}&lon={2}&appid={3}&units={4}", 
                BaseDataURL,
                lat, 
                lon, 
                AppID,
                GetTemperatureUnitsString());
        }
        
        public string GetCityQueryURL(string cityName)
        {
            return String.Format("{0}?q={1}&appid={2}&units={3}", 
                BaseDataURL,
                cityName, 
                AppID,
                GetTemperatureUnitsString());
        }
        
        public string GetWeatherIconURL(string icon)
        {
            return String.Format("{0}{1}@2x.png", 
                BaseIconURL, 
                icon);
        }

        private string GetTemperatureUnitsString()
        {
            switch (Units)
            {
                case TemperatureUnits.Celsius:
                    return "metric";
                case TemperatureUnits.Fahrenheit:
                    return "imperial";
                default:
                    return "standard";
            }
        }
        
    }
}