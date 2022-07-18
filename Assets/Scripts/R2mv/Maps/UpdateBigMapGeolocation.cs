using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Geospatial;
using Microsoft.Maps.Unity;
using R2mv.Weather;
using UnityEngine;

// This is used to ensure there is a MapRender!
[RequireComponent(typeof(MapRenderer))]
public class UpdateBigMapGeolocation : MonoBehaviour
{

    private MapRenderer _mapRenderer;

    [SerializeField]
    private MapPin _mapPin;
    
    // Start is called before the first frame update
    void Start()
    {
        _mapRenderer = GetComponent<MapRenderer>();
        
        if (OpenWeatherDataManager.Instance != null)
            OpenWeatherDataManager.Instance.OnWeatherDataUpdate += OnWeatherDataUpdate;
    }

    private void OnWeatherDataUpdate(OpenWeatherData data)
    {
        _mapRenderer.Center = new LatLon( OpenWeatherDataManager.Instance.Lattitude, OpenWeatherDataManager.Instance.Longitude);

        if (_mapPin)
        {
            _mapPin.Location = _mapRenderer.Center;
        }
    }
    
}
