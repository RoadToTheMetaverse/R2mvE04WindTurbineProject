using Cinemachine;
using UnityEngine;

namespace R2mv.Utils
{
    /// <summary>
    /// Originally created by WarpZone 
    /// Source: https://forum.unity.com/threads/how-do-i-make-a-cinemachinefreelook-orbiting-camera-that-only-orbits-when-the-mouse-key-is-down.527634/#post-3468444
    /// </summary>
    public class CMFreelookOnlyWhenMouseDown : MonoBehaviour
    {
    
        public enum MouseButton
        {
            Left = 0,
            Right = 1,
            Middle = 2
        }

        [SerializeField] private MouseButton _mouseButton = MouseButton.Left;

        // Start is called before the first frame update
        void Start()
        {
            CinemachineCore.GetInputAxis = GetAxisCustom;
        }

        public float GetAxisCustom(string axisName){
            if(axisName == "Mouse X"){
                if (Input.GetMouseButton((int) _mouseButton)){
                    return UnityEngine.Input.GetAxis("Mouse X");
                } else{
                    return 0;
                }
            }
            else if (axisName == "Mouse Y"){
                if (Input.GetMouseButton((int) _mouseButton)){
                    return UnityEngine.Input.GetAxis("Mouse Y");
                } else{
                    return 0;
                }
            }
            return UnityEngine.Input.GetAxis(axisName);
        }
    }
}
