using Unity.VisualScripting;
using UnityEngine;

namespace R2mv.Units
{
    
    [UnitTitle("Get Vertical Wind Turbine Power")]
    [UnitCategory("Road to the Metaverse/Custom Nodes")]
    
    public class GetVerticalWindTurbinePowerUnit : Unit
    {
        
        [DoNotSerialize] // No need to serialize ports
        [PortLabelHidden] // Hiding Label
        public ControlInput inputTrigger; // Adding an input port

        [DoNotSerialize] // No need to serialize ports.
        [PortLabelHidden] // Hiding Label
        public ControlOutput outputTrigger; // Adding an output port
        
        
        
        [DoNotSerialize] // No need to serialize ports
        public ValueInput windSpeed; // Wind speed (Float)

        [DoNotSerialize] // No need to serialize ports
        public ValueInput diameter; // Wind Turbine diameter(Float)
        
        [DoNotSerialize] // No need to serialize ports
        public ValueInput height; // Wind Turbine height(Float)
        
        [DoNotSerialize] // No need to serialize ports
        public ValueInput efficiency; // Power generation efficiency (Float) [Usually around 40%]

        [DoNotSerialize] // No need to serialize ports
        public ValueInput airDensity; // Raw enegy output (FLoat)
        
        [DoNotSerialize] // No need to serialize ports
        public ValueInput tipSpeedRatio; // Raw enegy output (FLoat)

        
        [DoNotSerialize] // No need to serialize ports
        public ValueOutput windPower; // Raw enegy output (FLoat)

        [DoNotSerialize] // No need to serialize ports
        public ValueOutput powerOutput; // Amount of electrical power generated

        [DoNotSerialize] // No need to serialize ports
        public ValueOutput rpm; // Amount of electrical power generated

        [DoNotSerialize] // No need to serialize ports
        public ValueOutput rotationY; // Amount of electrical power generated

        
        private float _windPower;
        private float _powerOutput;
        private float _rpm;
        private float _rotationY;
        protected override void Definition()
        {
            
            //The lambda to execute our node action when the inputTrigger port is triggered.
            inputTrigger = ControlInput("inputTrigger", (flow) =>
            {
                var ws = flow.GetValue<float>(windSpeed);
                var d = flow.GetValue<float>(diameter);

                _windPower = 0.5f * flow.GetValue<float>(airDensity)
                                  * Mathf.Pow(ws, 3f)
                                  * (d * flow.GetValue<float>(height));

                _powerOutput = _windPower * flow.GetValue<float>(efficiency);

                _rpm = (ws * 60f * flow.GetValue<float>(tipSpeedRatio))
                        / (Mathf.PI * d);

                _rotationY = (_rpm * 6f) * Time.deltaTime;
                
                return outputTrigger;
            });
            
            // Initialize Inputs
            airDensity = ValueInput<float>("airDensity", 1.225f);
            windSpeed = ValueInput<float>("windSpeed", 1);
            diameter = ValueInput<float>("diameter", 1);
            height = ValueInput<float>("height", 1);
            efficiency = ValueInput<float>("efficiency" ,0.4f);
            tipSpeedRatio = ValueInput<float>("tipSpeedRatio" ,2f);
            
            // Initialize Outputs
            outputTrigger = ControlOutput("outputTrigger");
            windPower = ValueOutput<float>("windPower", (flow) => _windPower);
            powerOutput = ValueOutput<float>("powerOutput", (flow) => _powerOutput);
            rpm = ValueOutput<float>("rpm", (flow) => _rpm);
            rotationY = ValueOutput<float>("rotationY", (flow) => _rotationY);
        }
    }
}