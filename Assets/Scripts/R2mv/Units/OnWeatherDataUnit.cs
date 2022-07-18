using R2mv.Weather;
using Unity.VisualScripting;
using UnityEngine;

namespace R2mv.Units
{


    [UnitTitle("On Weather Data")]
    [UnitCategory("Road to the Metaverse/Weather")]

    public class OnWeatherData : EventUnit<OpenWeatherData>
    {

        #region Value Outpurts

        [DoNotSerialize]
        public ValueOutput city { get; private set; }

        [DoNotSerialize]
        public ValueOutput geolocation { get; private set; }

        [DoNotSerialize]
        public ValueOutput description { get; private set; }

        [DoNotSerialize]
        public ValueOutput temp { get; private set; }

        [DoNotSerialize]
        public ValueOutput tempMIN { get; private set; }

        [DoNotSerialize]
        public ValueOutput tempMAX { get; private set; }

        [DoNotSerialize]
        public ValueOutput tempUnit { get; private set; }

        [DoNotSerialize]
        public ValueOutput rain { get; private set; }

        [DoNotSerialize]
        public ValueOutput clouds { get; private set; }

        [DoNotSerialize]
        public ValueOutput wind { get; private set; }

        [DoNotSerialize]
        public ValueOutput id { get; private set; }
        
        [DoNotSerialize]
        public ValueOutput icon { get; private set; }

        #endregion

        // Register for events
        protected override bool register => true;


        // Add an EventHook with the name of the Event to the list of Visual Scripting Events.
        public override EventHook GetHook(GraphReference reference)
        {
            return new EventHook(OpenWeatherDataManager.OnWeatherUpdateEventName);
        }
        
        protected override void Definition()
        {
            base.Definition();
            
            // Setting the location values
            city = ValueOutput<string>(nameof(city));
            geolocation = ValueOutput<Vector2>(nameof(geolocation));
            
            // Weather info
            description = ValueOutput<string>(nameof(description));
            
            temp = ValueOutput<float>(nameof(temp));
            tempMIN = ValueOutput<float>(nameof(tempMIN));
            tempMAX = ValueOutput<float>(nameof(tempMAX));
            tempUnit = ValueOutput<string>(nameof(tempUnit));
            
            rain = ValueOutput<float>(nameof(rain));
            clouds = ValueOutput<int>(nameof(clouds));
            wind = ValueOutput<float>(nameof(wind));
            
            // Weather icon
            id = ValueOutput<int>(nameof(id));
            icon = ValueOutput<Texture>(nameof(icon));
            
        }
        
        // Setting the value on our port when an event is received
        protected override void AssignArguments(Flow flow, OpenWeatherData data)
        {
            
            flow.SetValue(city, data.city);
            flow.SetValue(geolocation, data.LatLon);
            
            flow.SetValue(clouds, data.clouds);
            flow.SetValue(description, data.description);
            flow.SetValue(rain, data.rain);
            
            flow.SetValue(temp, data.temp);
            flow.SetValue(tempMAX, data.tempMAX);
            flow.SetValue(tempMIN, data.tempMIN);
            flow.SetValue(tempUnit, data.tempUnit);

            flow.SetValue(wind, data.wind);
            
            flow.SetValue(id, data.id);
            flow.SetValue(icon, data.icon);

        }
        
    }


}