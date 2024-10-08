using System;
using UnityEngine;

namespace NekoLegends
{
    public class DemoCameraController : MonoBehaviour
    {


        [SerializeField] public Transform AutoDOFTarget;  // The target you want to keep in focus used for auto DOF

        public Camera mainCamera;
        public Transform target;
        public float rotationSpeed = 2.0f;
        public float zoomSpeed = 2.0f;
        public float panSpeed = 2.0f;
        public float zoomInMax = .5f;
        public float zoomOutMax = 5f;
        public bool invertAxisY = false; 

        public bool useNewInputSystem = false;  // Manually set this based on your project setup
      
        private Vector3 cameraOffset;
        private Vector3 panLastPosition;

        void Start()
        {
            if (mainCamera == null)
            {
                Debug.LogError("Camera not assigned!");
                return;
            }

            cameraOffset = mainCamera.transform.position - target.position;
        }

        void Update()
        {
            Vector2 mouseDelta;
            float scrollData;
            int invertY = -1;

            if (invertAxisY)
                invertY = 1;


            if (useNewInputSystem)
            {
                // New Input System (Using Reflection)
                var mouseType = Type.GetType("UnityEngine.InputSystem.Mouse, Unity.InputSystem");
                if (mouseType != null)
                {
                    var deltaProperty = mouseType.GetProperty("delta");
                    var scrollProperty = mouseType.GetProperty("scroll");

                    var mouseInstance = mouseType.GetProperty("current").GetValue(null);
                    mouseDelta = (Vector2)deltaProperty.GetValue(mouseInstance);
                    scrollData = ((Vector2)scrollProperty.GetValue(mouseInstance)).y;
                }
                else
                {
                    Debug.LogError("New Input System not detected. Please ensure it's installed.");
                    return;
                }
            }
            else
            {
                // Old Input System
                mouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
                scrollData = Input.GetAxis("Mouse ScrollWheel");
            }

            // Handle rotation (left mouse button)
            if (IsRightMouseButtonPressed())
            {
                Quaternion camTurnAngle = Quaternion.Euler(mouseDelta.y * rotationSpeed, mouseDelta.x * rotationSpeed, 0);
                cameraOffset = camTurnAngle * cameraOffset;
            }
            
            // Handle zoom (mouse scroll wheel and middle mouse button drag)
            float zoomAmount = scrollData * zoomSpeed;
            if (IsMiddleMouseButtonPressed())
            {
                zoomAmount = -mouseDelta.y * zoomSpeed * 0.1f; // Multiplied by 0.1 to make it less sensitive than regular scroll
       
            }

            // Handle pan (left mouse button)
            if (IsLeftMouseButtonPressed())
            {
                if (Input.GetMouseButtonDown(0)) // Check if left mouse button was just pressed
                {
                    panLastPosition = mainCamera.ScreenToViewportPoint(Input.mousePosition);
                }

                Vector3 deltaPosition = mainCamera.ScreenToViewportPoint(Input.mousePosition) - panLastPosition;

                // Adjust panning direction based on camera's orientation
                Vector3 horizontalDirection = Vector3.Cross(mainCamera.transform.forward, Vector3.up).normalized;
                Vector3 moveDirection = deltaPosition.x * horizontalDirection + invertY * deltaPosition.y * Vector3.up;
                Vector3 move = moveDirection * panSpeed;

                target.position += move;
                panLastPosition = mainCamera.ScreenToViewportPoint(Input.mousePosition);
            }

            // Handle pan (middle mouse button)
            if (IsMiddleMouseButtonPressed())
            {
                if (Input.GetMouseButtonDown(2)) // Check if middle mouse button was just pressed
                {
                    panLastPosition = mainCamera.ScreenToViewportPoint(Input.mousePosition);
                }

                Vector3 deltaPosition = mainCamera.ScreenToViewportPoint(Input.mousePosition) - panLastPosition;

                // Adjust panning direction based on camera's orientation
                Vector3 horizontalDirection = Vector3.Cross(mainCamera.transform.forward, Vector3.up).normalized;
                Vector3 moveDirection = deltaPosition.x * horizontalDirection + invertY * deltaPosition.y * Vector3.up;
                Vector3 move = moveDirection * panSpeed;

                target.position += move;
                panLastPosition = mainCamera.ScreenToViewportPoint(Input.mousePosition);
            }



            // Adjust DOF to keep AutoDOFTarget in focus
            if (AutoDOFTarget)
            {
                AdjustDOFForTarget(AutoDOFTarget);
            }

                float cameraDistance = cameraOffset.magnitude;
                cameraDistance += zoomAmount;
                cameraDistance = Mathf.Clamp(cameraDistance, zoomInMax, zoomOutMax); // Adjust these values as necessary
                cameraOffset = cameraOffset.normalized * cameraDistance;


                // Update camera position and look at target
                mainCamera.transform.position = target.position + cameraOffset;
                mainCamera.transform.LookAt(target.position);
            

        }

        private void AdjustDOFForTarget(Transform autoDOFTarget)
        {
            float dynamicFocusDistance = Vector3.Distance(mainCamera.transform.position, autoDOFTarget.position);
           
            // Determine the current zoom level as a ratio between 0 (max zoom in) and 1 (max zoom out)
            float zoomRatio = (cameraOffset.magnitude - zoomInMax) / (zoomOutMax - zoomInMax);

            // Decide on min and max aperture values
            float minAperture = 1.4f; // max bokeh
            float maxAperture = 16.0f; // min bokeh

            // Interpolate based on the zoom ratio
            float currentAperture = Mathf.Lerp(minAperture, maxAperture, zoomRatio);
           
            DemoScenes.Instance.SetDOFImmediate(dynamicFocusDistance, currentAperture);
        }
    

        bool IsLeftMouseButtonPressed()
        {
            if (useNewInputSystem)
            {
                var mouseType = Type.GetType("UnityEngine.InputSystem.Mouse, Unity.InputSystem");
                if (mouseType != null)
                {
                    var leftButtonProperty = mouseType.GetProperty("leftButton");
                    var isPressedProperty = leftButtonProperty.PropertyType.GetProperty("isPressed");

                    var mouseInstance = mouseType.GetProperty("current").GetValue(null);
                    var leftButtonInstance = leftButtonProperty.GetValue(mouseInstance);
                    return (bool)isPressedProperty.GetValue(leftButtonInstance);
                }
                else
                {
                    Debug.LogError("New Input System not detected. Please ensure it's installed.");
                    return false;
                }
            }
            else
            {
                return Input.GetMouseButton(0);
            }
        }

        bool IsMiddleMouseButtonPressed()
        {
            if (useNewInputSystem)
            {
                // New Input System (Using Reflection)
                var mouseType = Type.GetType("UnityEngine.InputSystem.Mouse, Unity.InputSystem");
                if (mouseType != null)
                {
                    var middleButtonProperty = mouseType.GetProperty("middleButton");
                    var isPressedProperty = middleButtonProperty.PropertyType.GetProperty("isPressed");

                    var mouseInstance = mouseType.GetProperty("current").GetValue(null);
                    var middleButtonInstance = middleButtonProperty.GetValue(mouseInstance);
                    return (bool)isPressedProperty.GetValue(middleButtonInstance);
                }
                else
                {
                    Debug.LogError("New Input System not detected. Please ensure it's installed.");
                    return false;
                }
            }
            else
            {
                return Input.GetMouseButton(2);
            }
        }

        bool IsMiddleMouseButtonDown()
        {
            if (useNewInputSystem)
            {
                // New Input System (Using Reflection)
                var mouseType = Type.GetType("UnityEngine.InputSystem.Mouse, Unity.InputSystem");
                if (mouseType != null)
                {
                    var middleButtonProperty = mouseType.GetProperty("middleButton");
                    var wasPressedProperty = middleButtonProperty.PropertyType.GetProperty("wasPressedThisFrame");

                    var mouseInstance = mouseType.GetProperty("current").GetValue(null);
                    var middleButtonInstance = middleButtonProperty.GetValue(mouseInstance);
                    return (bool)wasPressedProperty.GetValue(middleButtonInstance);
                }
                else
                {
                    Debug.LogError("New Input System not detected. Please ensure it's installed.");
                    return false;
                }
            }
            else
            {
                return Input.GetMouseButtonDown(2);
            }
        }


        bool IsRightMouseButtonPressed()
        {
            if (useNewInputSystem)
            {
                var mouseType = Type.GetType("UnityEngine.InputSystem.Mouse, Unity.InputSystem");
                if (mouseType != null)
                {
                    var rightButtonProperty = mouseType.GetProperty("rightButton");
                    var isPressedProperty = rightButtonProperty.PropertyType.GetProperty("isPressed");

                    var mouseInstance = mouseType.GetProperty("current").GetValue(null);
                    var rightButtonInstance = rightButtonProperty.GetValue(mouseInstance);
                    return (bool)isPressedProperty.GetValue(rightButtonInstance);
                }
                else
                {
                    Debug.LogError("New Input System not detected. Please ensure it's installed.");
                    return false;
                }
            }
            else
            {
                return Input.GetMouseButton(1);
            }
        }

        bool IsRightMouseButtonDown()
        {
            if (useNewInputSystem)
            {
                var mouseType = Type.GetType("UnityEngine.InputSystem.Mouse, Unity.InputSystem");
                if (mouseType != null)
                {
                    var rightButtonProperty = mouseType.GetProperty("rightButton");
                    var wasPressedProperty = rightButtonProperty.PropertyType.GetProperty("wasPressedThisFrame");

                    var mouseInstance = mouseType.GetProperty("current").GetValue(null);
                    var rightButtonInstance = rightButtonProperty.GetValue(mouseInstance);
                    return (bool)wasPressedProperty.GetValue(rightButtonInstance);
                }
                else
                {
                    Debug.LogError("New Input System not detected. Please ensure it's installed.");
                    return false;
                }
            }
            else
            {
                return Input.GetMouseButtonDown(1);
            }
        }
    }

}