using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JFoundation
{
    /*

    */

    public interface ScreenInputDelegate
    {
        void ScreenInputDidEnterPoint(Vector2 point);
        void ScreenInputDidMovePoint(Vector2 currentPoint);
        void ScreenInputDidReleasePoint(Vector2 endingPoint);
    }

    public interface ScreenSpecialInputDelegate
    {
        // multitouch fingerIds are always 1 or higher (2nd touch or more)
        void ScreenInputMultiTouchDidEnterPoint(Vector2 point, int fingerId);
        void ScreenInputMultiTouchDidMovePoint(Vector2 currentPoint, int fingerId);
        void ScreenInputMultiTouchDidReleasePoint(Vector2 endingPoint, int fingerId);
        void ScreenInputDidLeftShiftMouseDown(Vector2 point);
    }

    // Not yet implemented
    public enum ScreenInputDirection{
        UP,
        DOWN,
        LEFT,
        RIGHT
    }

    public class ScreenInputController : MonoBehaviour, TouchControllerDelegate, SwipeControllerDelegate, PcInputDelegate
    {
        // Prefab dependencies
        public InputSettings inputSettings;
        string TOUCH_INPUT_SETTINGS_PATH = "Input/iOS - Input Settings";
        string PC_INPUT_SETTINGS_FILEPATH = "Input/PC - Input Settings";

        // Dependencies
        Debugger debugger;

        // Vars
        public TouchController touchController{get; private set;}
        public SwipeController swipeController{get; private set;}
        public PcInputController pcInputController{get; private set;}

        string TOUCH_INPUT_ENABLED = "Touch Input is enabled.";
        string PC_INPUT_ENABLED = "PC Input is enabled.";

        public ScreenInputDelegate inputDelegate;
        public ScreenSpecialInputDelegate specialInputDelegate;

        bool isBlockedByUiElements = true;

        void Awake()
        {
            debugger = DependencyLoader.LoadGameObject<Debugger>("Debugger", this, gameObject) as Debugger;
        }

        public void SetDependencies(ScreenInputDelegate inputDelegate)
        {
            this.inputDelegate = inputDelegate;
        }

        // Can be called during Awake()
        public void SetIsBlockedByUiElements(bool isBlocked)
        {
            // We need to set this and activate for PC/Touch controller because they may not be initialized at startup if this is called on Awake()
            isBlockedByUiElements = isBlocked;

            if (touchController)
            {
                touchController.isBlockedByUiElements = isBlocked;
            }

            if (pcInputController)
            {
                pcInputController.isBlockedByUiElements = isBlocked;
            }
        }

        // Has to happen after dependencies are set
        void InitializeVarsAfterDependencies()
        {
            if (inputSettings.isTouchInputEnabled)
            {
                this.swipeController = new SwipeController(inputSettings.swipeMinThreshold, inputSettings.subsequentSwipeMinThreshold);
                swipeController.SetDependencies(debugger, this, inputSettings);

                this.touchController = gameObject.AddComponent(typeof(TouchController)) as TouchController;
                touchController.SetDependencies(debugger, this);
                touchController.isBlockedByUiElements = isBlockedByUiElements;
            }

            if (inputSettings.isPcInputEnabled)
            {
                this.pcInputController = gameObject.AddComponent(typeof(PcInputController)) as PcInputController;
                pcInputController.SetDependencies(debugger, this);
                pcInputController.isBlockedByUiElements = isBlockedByUiElements;
            }
        }

        void SetInputSettingsAutomatically()
        {
            #if UNITY_IOS || UNITY_ANDROID
            // Load mobile touch input settings
            inputSettings = Resources.Load(TOUCH_INPUT_SETTINGS_PATH) as InputSettings;
            #else
            // Load PC input settings
            inputSettings = Resources.Load(PC_INPUT_SETTINGS_FILEPATH) as InputSettings;
            #endif

            if (Application.isEditor)
            {
                inputSettings = Resources.Load(PC_INPUT_SETTINGS_FILEPATH) as InputSettings;
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            SetInputSettingsAutomatically();

            DependencyLoader.DependencyCheck<InputSettings>(inputSettings, this, gameObject, debugger);

            if (inputSettings != null)
            {
                InitializeVarsAfterDependencies();

                if (inputSettings.isTouchInputEnabled)
                {
                    debugger.Log(TOUCH_INPUT_ENABLED);
                }
                if (inputSettings.isPcInputEnabled)
                {
                    debugger.Log(PC_INPUT_ENABLED);
                }
            }
        }

        // Update is called once per frame
        void Update()
        {

        }

        void NotifyDelegateDidEnterPoint(Vector2 point)
        {
            if (inputDelegate != null)
            {
                inputDelegate.ScreenInputDidEnterPoint(point);
            }
            else
            {
                debugger.WarnMissingDependency<ScreenInputController, ScreenInputDelegate>(gameObject, inputSettings.showDebug);
            }
        }

        void NotifyDelegateDidMovePoint(Vector2 currentPoint)
        {
            if (inputDelegate != null)
            {
                inputDelegate.ScreenInputDidMovePoint(currentPoint);
            }
            else
            {
                debugger.WarnMissingDependency<ScreenInputController, ScreenInputDelegate>(gameObject, inputSettings.showDebug);
            }
        }

        void NotifyDelegateDidReleasePoint(Vector2 point)
        {
            if (inputDelegate != null)
            {
                inputDelegate.ScreenInputDidReleasePoint(point);
            }
            else
            {
                debugger.WarnMissingDependency<ScreenInputController, ScreenInputDelegate>(gameObject, inputSettings.showDebug);
            }
        }

        void NotifyDelegateDidEnterMultiTouchPoint(Vector2 point, int fingerId)
        {
            if (specialInputDelegate != null)
            {
                specialInputDelegate.ScreenInputMultiTouchDidEnterPoint(point, fingerId);
            }
            else
            {
                debugger.WarnMissingDependency<ScreenInputController, ScreenSpecialInputDelegate>(gameObject, inputSettings.showDebug);
            }
        }

        void NotifyDelegateDidMoveMultiTouchPoint(Vector2 point, int fingerId)
        {
            if (specialInputDelegate != null)
            {
                specialInputDelegate.ScreenInputMultiTouchDidMovePoint(point, fingerId);
            }
            else
            {
                debugger.WarnMissingDependency<ScreenInputController, ScreenSpecialInputDelegate>(gameObject, inputSettings.showDebug);
            }
        }

        void NotifyDelegateDidReleaseMultiTouchPoint(Vector2 point, int fingerId)
        {
            if (specialInputDelegate != null)
            {
                specialInputDelegate.ScreenInputMultiTouchDidReleasePoint(point, fingerId);
            }
            else
            {
                debugger.WarnMissingDependency<ScreenInputController, ScreenSpecialInputDelegate>(gameObject, inputSettings.showDebug);
            }
        }

        void NotifyDelegateDidLeftShiftMouseDown(Vector2 point)
        {
            if (specialInputDelegate != null)
            {
                specialInputDelegate.ScreenInputDidLeftShiftMouseDown(point);
            }
            else
            {
                debugger.WarnMissingDependency<ScreenInputController, ScreenSpecialInputDelegate>(gameObject, inputSettings.showDebug);
            }
        }

        //------------------- Touch controller delegate----------------------------

        void TouchControllerDelegate.TouchDidEnterPoint(Vector2 point, int fingerId)
        {
            swipeController.AddPoint(point);

            if (fingerId == 0)
            {
                NotifyDelegateDidEnterPoint(point);
            }
            else
            {
                NotifyDelegateDidEnterMultiTouchPoint(point, fingerId);
            }

        }

        void TouchControllerDelegate.TouchDidMovePoint(Vector2 currentPoint, int fingerId)
        {
            swipeController.AddPoint(currentPoint);

            if (fingerId == 0)
            {
                NotifyDelegateDidMovePoint(currentPoint);
            }
            else
            {
                NotifyDelegateDidMoveMultiTouchPoint(currentPoint, fingerId);
            }

        }

        void TouchControllerDelegate.TouchDidReleasePoint(Vector2 endingPoint, int fingerId)
        {
            swipeController.ReleasePoints();

            if (fingerId == 0)
            {
                NotifyDelegateDidReleasePoint(endingPoint);
            }
            else
            {
                NotifyDelegateDidReleaseMultiTouchPoint(endingPoint, fingerId);
            }

        }

        // -------------------- Swipe Controller Delegate ----------------
        void SwipeControllerDelegate.DidSwipeWithDirectionVector(Vector2 startingPosition, Vector2 endingPosition, Vector2 dir)
        {
            // need something different here
        }
        // ------------------- PC Input Controller Delegate ------------------
        void PcInputDelegate.DidMouseDown(Vector2 pos)
        {
            NotifyDelegateDidEnterPoint(pos);
        }

        void PcInputDelegate.DidMoveWhileMouseDown(Vector2 pos)
        {
            NotifyDelegateDidMovePoint(pos);
        }

        void PcInputDelegate.DidMouseUp(Vector2 pos)
        {
            NotifyDelegateDidReleasePoint(pos);
        }

        void PcInputDelegate.DidLeftShiftMouseDown(Vector2 pos)
        {
            NotifyDelegateDidLeftShiftMouseDown(pos);
        }
    }
}
