using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace JFoundation
{
    /*
    Setup:
    1) Add a new instance to parent with gameObject.AddComponent<>();
    2) Implement TouchDelegate interface in container class, set as dependency
    3) Interact with the TouchDelegate implementation events

    Additional notes:
    - Touches are "captured" to be preserved
    - Touches may not necessarily have same memory signature
    - To distinguish touches, must use fingerId, so do not rely on List.IndexOf()
    to find a touch!
    - Raycasting check will block touches captured by UI. In future, have option to disable?

    */

    public interface TouchControllerDelegate
    {
        void TouchDidEnterPoint(Vector2 point, int fingerId);
        void TouchDidMovePoint(Vector2 currentPoint, int fingerId);
        void TouchDidReleasePoint(Vector2 endingPoint, int fingerId);
    }

    public class TouchController : MonoBehaviour
    {
        // Start is called before the first frame update
        public Debugger debugger {get; private set;}
        string WARNING_DELEGATE_NOT_SET = "Warning: Touch Delegate not set!";

        public TouchControllerDelegate touchDelegate{get; private set;}

        List<Touch> capturedTouches;
        bool shouldProhibitFirstTouchRelease = false;
        public bool isBlockedByUiElements = true;

        public void SetDependencies(Debugger debugger, TouchControllerDelegate touchDelegate)
        {
            this.debugger = debugger;
            this.touchDelegate = touchDelegate;
        }

        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (Input.touchCount > 0)
            {
                foreach (Touch touch in Input.touches)
                {
                    switch (touch.phase)
                    {
                        case TouchPhase.Began:
                            if (IsTouchBlockedByRaycastedObject(touch) == false)
                            {
                                CaptureTouch(touch);
                                NotifyDelegateDidEnterPoint(touch.position, touch.fingerId);
                            }
                            break;

                        case TouchPhase.Moved:
                            if (TouchWasCaptured(touch))
                            {
                                NotifyDelegateDidMovePoint(touch.position, touch.fingerId);

                                // Moving of first touch also renables
                                if (touch.fingerId == 0)
                                {
                                    shouldProhibitFirstTouchRelease = false;
                                }

                            }
                            break;

                        case TouchPhase.Ended:
                            if (ReleaseTouch(touch) == true)
                            {
                                NotifyDelegateDidReleasePoint(touch.position, touch.fingerId);
                            }
                            break;
                        }
                }
            }
        }

        void initializeCapturedTouches()
        {
            if (capturedTouches == null)
            {
                capturedTouches = new List<Touch>();
            }
        }

        bool IsTouchBlockedByRaycastedObject(Touch touch)
        {
            if (isBlockedByUiElements == true)
            {
                if (EventSystem.current)
                {
                    if (EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                    {
                        return true;
                    }
                }
            }
            
            return false;
        }

        bool TouchWasCaptured(Touch touch)
        {
            initializeCapturedTouches();

            foreach(Touch capturedTouch in capturedTouches)
            {
                if (touch.fingerId == capturedTouch.fingerId)
                {
                    return true;
                }
            }
            return false;
        }

        void CaptureTouch(Touch touch)
        {
            initializeCapturedTouches();

            int touchIndex = capturedTouches.IndexOf(touch);

            if (touchIndex == -1)
            {
                capturedTouches.Add(touch);

                // First touch resets previous 2ndtouch register
                if (touch.fingerId == 0)
                {
                    shouldProhibitFirstTouchRelease = false;
                }
                else if (touch.fingerId == 1)
                {
                    shouldProhibitFirstTouchRelease = true;
                }
            }
        }

        /* ReleaseTouch returns a bool because it plays a special role.
        Normally, the system registers all touches, but because UI does not swallow touches,
        we have to implement a check to see if a UI object is in front of the InputController with:
            if (!EventSystem.current.IsPointerOverGameObject(touch.fingerId))
        For some reason, TouchPhase.Ended returns a false value even if it came from a touch that was in front of a UI.
        So to work-around this, we will only return a touch-released based not on the input,
        but based on whether or not we Released a touch.
        Remember, we can only release a touch if one was captured. So as long as no touches were captured,
        no touches will be released, which works around the false positive for TouchPhase.Ended.
        */
        bool ReleaseTouch(Touch touch)
        {
            initializeCapturedTouches();

            foreach(Touch capturedTouch in capturedTouches)
            {
                if (touch.fingerId == capturedTouch.fingerId)
                {
                    capturedTouches.Remove(capturedTouch);
                    //debugger.Log(RELEASED_A_TOUCH + touch.fingerId);

                    if (shouldProhibitFirstTouchRelease && capturedTouch.fingerId == 0)
                    {
                        // Do not notify a release of first touch if second touch was issued, behaviorally feels like 2nd touch should cancel out first.
                        return false;
                    }

                    return true;
                }
            }
            return false;
        }

        void NotifyDelegateDidEnterPoint(Vector2 point, int fingerId)
        {
            if (touchDelegate == null)
            {
                debugger.Log(WARNING_DELEGATE_NOT_SET);
            }
            else
            {
                touchDelegate.TouchDidEnterPoint(point, fingerId);
            }
        }

        void NotifyDelegateDidMovePoint(Vector2 currentPoint, int fingerId)
        {
            if (touchDelegate == null)
            {
                debugger.Log(WARNING_DELEGATE_NOT_SET);
            }
            else
            {
                touchDelegate.TouchDidMovePoint(currentPoint, fingerId);
            }
        }

        void NotifyDelegateDidReleasePoint(Vector2 point, int fingerId)
        {
            if (touchDelegate == null)
            {
                debugger.Log(WARNING_DELEGATE_NOT_SET);
            }
            else
            {
                touchDelegate.TouchDidReleasePoint(point, fingerId);
            }
        }
    }
}
