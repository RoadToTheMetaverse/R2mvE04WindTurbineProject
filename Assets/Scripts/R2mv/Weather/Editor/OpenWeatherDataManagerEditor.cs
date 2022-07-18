using UnityEditor;
using UnityEngine;

namespace R2mv.Weather.Editor
{
    
    [CustomEditor(typeof(OpenWeatherDataManager))]
    public class OpenWeatherDataManagerEditor : UnityEditor.Editor
    {
        
        private void OnEnable()
        {
            
        }

        public override void OnInspectorGUI()
        {
            
            serializedObject.Update();
            
            //DrawDefaultInspector();
            
            
            var manager = target as OpenWeatherDataManager;

            EditorGUILayout.Space();
            
            manager.ApiSettings =
                EditorGUILayout.ObjectField("API Settings", manager.ApiSettings, typeof(OpenWeatherAPISettings), true) as OpenWeatherAPISettings;

            if (manager.ApiSettings == null)
            {
                EditorGUILayout.HelpBox("You must create and/or assign an OpenWeather API Settings asset!", MessageType.Error);
                if (GUILayout.Button("Click to create a new API Settings asset", GUILayout.Height(40f)))
                {
                    OpenWeatherAPISettings asset = ScriptableObject.CreateInstance<OpenWeatherAPISettings>();
                    string name = UnityEditor.AssetDatabase.GenerateUniqueAssetPath("Assets/Settings/New OpenWeather API Settings.asset");
                    AssetDatabase.CreateAsset(asset, name);
                    AssetDatabase.SaveAssets();

                    manager.ApiSettings = asset;
                    
                    EditorUtility.FocusProjectWindow();
                    Selection.activeObject = asset;
                }
                return;
            }

            EditorGUILayout.Space();
            
            manager.Mode = (OpenWeatherDataManager.QueryMode) EditorGUILayout.EnumPopup("Query Mode", manager.Mode);

            if (manager.Mode == OpenWeatherDataManager.QueryMode.CityName)
            {
                EditorGUI.indentLevel++;
                manager.CityName = EditorGUILayout.TextField("City Name", manager.CityName);
            }
            else if (manager.Mode == OpenWeatherDataManager.QueryMode.LatLon)
            {
                EditorGUI.indentLevel++;
                manager.Lattitude = EditorGUILayout.FloatField("Lattitude", manager.Lattitude);
                manager.Longitude = EditorGUILayout.FloatField("Longitude", manager.Longitude);
            }
            
            EditorGUILayout.Space();
            
            EditorGUI.indentLevel--;
            manager.FetchOnStart = EditorGUILayout.Toggle("Fetch data on start", manager.FetchOnStart);
            

        }

    }
}