using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace JFoundation
{
    /*  How to use:
        1) In container class, attach to a gameObject with gameObject.AddComponent<>();
        2) In container class, set dependencies - debugger and its delegate
        3) In container class, Implement the delegate to receive callbacks

        Additional notes:
        - Keyboard keypresses are not yet implemented
    */

    public interface PcInputDelegate
    {
        void DidMouseDown(Vector2 pos);
        void DidMoveWhileMouseDown(Vector2 pos);
        void DidMouseUp(Vector2 pos);

        void DidLeftShiftMouseDown(Vector2 pos);
    }


    public class PcInputController : MonoBehaviour
    {
        Debugger debugger;
        PcInputDelegate pcInputDelegate;

        enum MouseState
        {
            Clicked,
            Released,
        }

        MouseState mouseState = MouseState.Released;
        Vector2 previousClickPoint = new Vector2(0f,0f);

        string WARNING_DELEGATE_NOT_SET = "Warning: PC Input Controller Delegate not set!";

        // Start is called before the first frame update
        void Start()
        {

        }

        public void SetDependencies(Debugger debugger, PcInputDelegate pcInputDelegate)
        {
            this.debugger = debugger;
            this.pcInputDelegate = pcInputDelegate;
        }

        // Update is called once per frame
        void Update()
        {
            Vector2 mousePos = Input.mousePosition;
            // Notify Mouse Down

            if ((Input.GetKeyDown("left shift") && Input.GetMouseButton(0)) || (Input.GetKey("left shift") && Input.GetMouseButtonDown(0)))
            {
                //debugger.Log("Mouse was left shifted.");
                NotifyDelegateDidLeftShiftDown(mousePos);
            }
            else
            {
                if (Input.GetMouseButtonDown(0))
                {
                    if (IsMouseDownBlockedByRaycastedObject() == false)
                    {
                        //debugger.Log("Mouse was clicked.");
                        SetMouseClick(mousePos);
                        NotifyDelegateDidMouseDown(mousePos);
                    }
                }

                // Notify Mouse Moved
                if (Input.GetMouseButton(0))
                {
                    if (MouseWasClicked())
                    {
                        if (MouseWasMoved(mousePos))
                        {
                            //debugger.Log("Mouse was moved.");
                            SetMouseMoved(mousePos);
                            NotifyDelegateDidMoveWhileMouseDown(mousePos);
                        }
                    }
                }

                // Notify Mouse Released
                if (Input.GetMouseButtonUp(0))
                {
                    if (MouseWasClicked())
                    {
                        //debugger.Log("Mouse was released.");
                        ReleaseMouseClick();
                        NotifyDelegateDidMouseUp(mousePos);
                    }
                }
            }
        }

        bool IsMouseDownBlockedByRaycastedObject()
        {
            if (EventSystem.current)
            {
                if (EventSystem.current.IsPointerOverGameObject())
                {
                    return true;
                }
            }

            return false;
        }



        void SetMouseClick(Vector2 mousePos)
        {
            mouseState = MouseState.Clicked;
            previousClickPoint = mousePos;
        }

        void SetMouseMoved(Vector2 currentPos)
        {
            previousClickPoint = currentPos;
        }

        void ReleaseMouseClick()
        {
            mouseState = MouseState.Released;
        }

        bool MouseWasClicked()
        {
            if (mouseState == MouseState.Clicked)
            {
                return true;
            }
            return false;
        }

        bool MouseWasMoved(Vector2 pos)
        {
            if (previousClickPoint.Equals(pos) == false)
            {
                return true;
            }
            return false;
        }

        // ----------------- notify delegate methods ----------------
        void NotifyDelegateDidMouseDown(Vector2 pos)
        {
            if (pcInputDelegate == null)
            {
                debugger.Log(WARNING_DELEGATE_NOT_SET);
            }
            else
            {
                pcInputDelegate.DidMouseDown(pos);
            }
        }

        void NotifyDelegateDidMoveWhileMouseDown(Vector2 pos)
        {
            if (pcInputDelegate == null)
            {
                debugger.Log(WARNING_DELEGATE_NOT_SET);
            }
            else
            {
                pcInputDelegate.DidMoveWhileMouseDown(pos);
            }
        }

        void NotifyDelegateDidMouseUp(Vector2 pos)
        {
            if (pcInputDelegate == null)
            {
                debugger.Log(WARNING_DELEGATE_NOT_SET);
            }
            else
            {
                pcInputDelegate.DidMouseUp(pos);
            }
        }

        void NotifyDelegateDidLeftShiftDown(Vector2 pos)
        {
            if (pcInputDelegate == null)
            {
                debugger.Log(WARNING_DELEGATE_NOT_SET);
            }
            else
            {
                pcInputDelegate.DidLeftShiftMouseDown(pos);
            }
        }

    }
}
