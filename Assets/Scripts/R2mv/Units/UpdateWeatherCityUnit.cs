using R2mv.Weather;
using Unity.VisualScripting;

namespace R2mv.Units
{
    [UnitTitle("Update City")]
    [UnitCategory("Road to the Metaverse/Weather")]

    public class UpdateWeatherCityUnit : Unit
    {
        
        [DoNotSerialize]// Mandatory attribute, to make sure we don’t serialize data that should never be serialized.
        [PortLabelHidden]// Hiding the port label as we normally hide the label for default Input and Output triggers.
        public ControlInput inputTrigger { get; private set; }
        
        [DoNotSerialize]
        public ValueInput CityName;
        
        [DoNotSerialize]
        [PortLabelHidden]// Hiding the port label as we normally hide the label for default Input and Output triggers.
        public ControlOutput outputTrigger { get; private set; }

        protected override void Definition()
        {

            inputTrigger = ControlInput(nameof(inputTrigger), Trigger);
            CityName = ValueInput<string>(nameof(CityName),"London");
            outputTrigger = ControlOutput(nameof(outputTrigger));
            Succession(inputTrigger, outputTrigger);
        }

        //Sending the Event MyCustomEvent with the integer value from the ValueInput port myValueA.
        private ControlOutput Trigger(Flow flow)
        {
            EventBus.Trigger(OpenWeatherDataManager.UpdateCityEventName, flow.GetValue<string>(CityName));
            return outputTrigger;
        }
    }
}