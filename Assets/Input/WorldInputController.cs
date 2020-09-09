using System;
﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JFoundation
{

    public interface WorldInputDelegate
    {
        void WorldInputDidEnterPoint(Vector3 point, RaycastHit raycastHit);
        void WorldInputDidMovePoint(Vector3 point, RaycastHit raycastHit);
        void WorldInputDidReleasePoint(Vector3 point, RaycastHit raycastHit);
    }

    // here, have to consider how to handle multitouch
    public interface WorldSpecialInputDelegate
    {
        // multitouch fingerIds are always 1 or higher (2nd touch or more)
        void WorldInputMultiTouchDidEnterPoint(Vector3 point, RaycastHit raycastHit, int fingerId);
        void WorldInputMultiTouchDidMovePoint(Vector3 point, RaycastHit raycastHit, int fingerId);
        void WorldInputMultiTouchDidReleasePoint(Vector3 point, RaycastHit raycastHit, int fingerId);
        void WorldInputDidLeftShiftMouseDown(Vector3 point, RaycastHit raycastHit);
    }

    public class WorldInputController : MonoBehaviour, ScreenInputDelegate, ScreenSpecialInputDelegate
    {

        Debugger debugger;
        public WorldInputDelegate inputDelegate;
        public WorldSpecialInputDelegate specialInputDelegate;

        InputSettings inputSettings;
        public ScreenInputController screenInput;
        Camera screenCam;

        string NO_RAYCAST_HIT_WARNING = "Warning: WorldInputController failed a raycast attempt.";

        void Awake()
        {
            debugger = DependencyLoader.LoadGameObject<Debugger>("Debugger", this, gameObject) as Debugger;
        }

        public void SetDependencies(WorldInputDelegate inputDelegate, InputSettings inputSettings, Camera screenCam)
        {
            this.inputDelegate = inputDelegate;
            this.inputSettings = inputSettings;
            this.screenCam = screenCam;
        }

        // Start is called before the first frame update
        void Start()
        {
            DependencyLoader.DependencyCheck<InputSettings>(inputSettings, this, gameObject, debugger);

            if (inputSettings)
            {
                screenInput = gameObject.AddComponent<ScreenInputController>() as ScreenInputController;
                screenInput.SetDependencies(this, inputSettings);
                screenInput.specialInputDelegate = this;
            }
        }

        // Update is called once per frame
        void Update()
        {

        }

        // ------------- Private Helper Methods -----------

        bool PerformRaycastHitFromScreenPosition(Vector2 screenPos, Action<RaycastHit> onSuccess)
        {
            // Cast ray
            Ray ray = screenCam.ScreenPointToRay(screenPos);
            RaycastHit raycastHit;

            if (Physics.Raycast(ray, out raycastHit))
            {
                onSuccess(raycastHit);
                return true;
            }
            else
            {
                debugger.Log(NO_RAYCAST_HIT_WARNING, inputSettings.showDebug);
                return false;
            }
        }

        // ------------- End of Private Helper Methods -----------

        // ---------------- Screen Input Callbacks ----------------

        void ScreenInputDelegate.ScreenInputDidEnterPoint(Vector2 point)
        {
            bool successfulRaycast = PerformRaycastHitFromScreenPosition(point, (RaycastHit returnedRaycastHit) => {
                NotifyDelegateDidEnterWorldPoint(returnedRaycastHit.point, returnedRaycastHit);
                });
        }

        void ScreenInputDelegate.ScreenInputDidMovePoint(Vector2 currentPoint)
        {
            bool successfulRaycast = PerformRaycastHitFromScreenPosition(currentPoint, (RaycastHit returnedRaycastHit) => {
                NotifyDelegateDidMoveWorldPoint(returnedRaycastHit.point, returnedRaycastHit);
                });
        }
        void ScreenInputDelegate.ScreenInputDidReleasePoint(Vector2 endingPoint)
        {
            bool successfulRaycast = PerformRaycastHitFromScreenPosition(endingPoint, (RaycastHit returnedRaycastHit) => {
                NotifyDelegateDidReleaseWorldPoint(returnedRaycastHit.point, returnedRaycastHit);
                });
        }
        // ---------------- End of Screen Input Callbacks ----------------

        // --------------- Screen Input Multi Touch special delegate callbacks ------------------

        void ScreenSpecialInputDelegate.ScreenInputMultiTouchDidEnterPoint(Vector2 point, int fingerId)
        {
            bool successfulRaycast = PerformRaycastHitFromScreenPosition(point, (RaycastHit returnedRaycastHit) => {
                NotifyDelegateDidEnterMultiTouchWorldPoint(returnedRaycastHit.point, returnedRaycastHit, fingerId);
                });
        }

        void ScreenSpecialInputDelegate.ScreenInputMultiTouchDidMovePoint(Vector2 point, int fingerId)
        {
            bool successfulRaycast = PerformRaycastHitFromScreenPosition(point, (RaycastHit returnedRaycastHit) => {
                NotifyDelegateDidMoveMultiTouchWorldPoint(returnedRaycastHit.point, returnedRaycastHit, fingerId);
                });
        }

        void ScreenSpecialInputDelegate.ScreenInputMultiTouchDidReleasePoint(Vector2 point, int fingerId)
        {
            bool successfulRaycast = PerformRaycastHitFromScreenPosition(point, (RaycastHit returnedRaycastHit) => {
                NotifyDelegateDidReleaseMultiTouchWorldPoint(returnedRaycastHit.point, returnedRaycastHit, fingerId);
                });
        }

        void ScreenSpecialInputDelegate.ScreenInputDidLeftShiftMouseDown(Vector2 point)
        {
            bool successfulRaycast = PerformRaycastHitFromScreenPosition(point, (RaycastHit returnedRaycastHit) => {
                NotifyDelegateDidLeftShiftMouseDown(returnedRaycastHit.point, returnedRaycastHit);
                });
        }
        // --------------- End of Screen Input Multi Touch special delegate callbacks ------------------

        // ------------- Notify Delegate Callbacks ----------------

        void NotifyDelegateDidEnterWorldPoint(Vector3 targetPoint, RaycastHit raycastHit)
        {
            if (inputDelegate != null)
            {
                inputDelegate.WorldInputDidEnterPoint(targetPoint, raycastHit);
            }
            else
            {
                debugger.WarnMissingDependency<WorldInputController, WorldInputDelegate>(gameObject, inputSettings.showDebug);
            }
        }

        void NotifyDelegateDidMoveWorldPoint(Vector3 targetPoint, RaycastHit raycastHit)
        {
            if (inputDelegate != null)
            {
                inputDelegate.WorldInputDidMovePoint(targetPoint, raycastHit);
            }
            else
            {
                debugger.WarnMissingDependency<WorldInputController, WorldInputDelegate>(gameObject, inputSettings.showDebug);
            }
        }

        void NotifyDelegateDidReleaseWorldPoint(Vector3 targetPoint, RaycastHit raycastHit)
        {
            if (inputDelegate != null)
            {
                inputDelegate.WorldInputDidReleasePoint(targetPoint, raycastHit);
            }
            else
            {
                debugger.WarnMissingDependency<WorldInputController, WorldInputDelegate>(gameObject, inputSettings.showDebug);
            }
        }

        // Special Input Multi touch notifications
        void NotifyDelegateDidEnterMultiTouchWorldPoint(Vector3 targetPoint, RaycastHit raycastHit, int fingerId)
        {
            if (specialInputDelegate != null)
            {
                specialInputDelegate.WorldInputMultiTouchDidEnterPoint(targetPoint, raycastHit, fingerId);
            }
            else
            {
                debugger.WarnMissingDependency<WorldInputController, WorldSpecialInputDelegate>(gameObject, inputSettings.showDebug);
            }
        }

        void NotifyDelegateDidMoveMultiTouchWorldPoint(Vector3 targetPoint, RaycastHit raycastHit, int fingerId)
        {
            if (specialInputDelegate != null)
            {
                specialInputDelegate.WorldInputMultiTouchDidMovePoint(targetPoint, raycastHit, fingerId);
            }
            else
            {
                debugger.WarnMissingDependency<WorldInputController, WorldSpecialInputDelegate>(gameObject, inputSettings.showDebug);
            }
        }

        void NotifyDelegateDidReleaseMultiTouchWorldPoint(Vector3 targetPoint, RaycastHit raycastHit, int fingerId)
        {
            if (specialInputDelegate != null)
            {
                specialInputDelegate.WorldInputMultiTouchDidReleasePoint(targetPoint, raycastHit, fingerId);
            }
            else
            {
                debugger.WarnMissingDependency<WorldInputController, WorldSpecialInputDelegate>(gameObject, inputSettings.showDebug);
            }
        }

        void NotifyDelegateDidLeftShiftMouseDown(Vector3 targetPoint, RaycastHit raycastHit)
        {
            if (specialInputDelegate != null)
            {
                specialInputDelegate.WorldInputDidLeftShiftMouseDown(targetPoint, raycastHit);
            }
            else
            {
                debugger.WarnMissingDependency<WorldInputController, WorldSpecialInputDelegate>(gameObject, inputSettings.showDebug);
            }
        }
        // ------------- End of Notify Delegate Callbacks  ----------------
    }
}
