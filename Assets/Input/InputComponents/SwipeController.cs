using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JFoundation
{
    /*
    How to use:
    1) In scope of TouchController creation, create new SwipeController Instance
    2) Set its SwipeDelegate dependency
    3) Place AddPoint() wherever a new point is registered - this is where swipe is detected.
    4) Place RemovePoints() wherever the touch is released.
    5) Interact with the delegate's swipe notification.

    Additional notes:
    - A pivot point is used to calculate distance between it and the current point
    - no arrays or anything required

    */
    public interface SwipeControllerDelegate
    {
        void DidSwipeWithDirectionVector(Vector2 startingPosition, Vector2 endingPosition, Vector2 direction);
    }

    public class SwipeController
    {
        Debugger debugger;
        SwipeControllerDelegate swipeDelegate;
        public InputSettings settings;

        float swipeDistanceMinimumThreshold;
        float subsequentSwipeDistanceMinimumThreshold;
        Vector2 firstPivotPoint;
        Vector2 previousPivotPoint;

        float previousPivotTime;
        float currentPivotTime;

        bool previousPivotPointExists = false;
        bool firstPivotPointExists = false;

        string WARNING_DELEGATE_NOT_SET = "Warning: Swipe Controller Delegate not set!";

        public SwipeController(float swipeDistanceMinimumThreshold, float subsequentSwipeDistanceMinimumThreshold)
        {
            this.swipeDistanceMinimumThreshold = swipeDistanceMinimumThreshold;
            this.subsequentSwipeDistanceMinimumThreshold = subsequentSwipeDistanceMinimumThreshold;
        }

        public void SetDependencies(Debugger debugger, SwipeControllerDelegate swipeDelegate, InputSettings settings)
        {
            this.debugger = debugger;
            this.swipeDelegate = swipeDelegate;
            this.settings = settings;
        }

        public void ReleasePoints()
        {
            // Release pivot points
            previousPivotPointExists = false;
            firstPivotPointExists = false;
        }

        public void AddPoint(Vector2 point)
        {
            // If array of points is empty, detect first swipe
            bool shouldDetectFirstSwipe = ShouldDetectFirstSwipe(point);

            if (shouldDetectFirstSwipe == true)
            {
                // 1) Detect Swipe from current point back to first point
                DetectFirstSwipe(point, swipeDistanceMinimumThreshold);
            }
            else
            {
                // Temporarily disable subsequent swipes
                // 2) Detect Swipe from current point to previous pivot point
                //DetectSubsequentSwipes(point, subsequentSwipeDistanceMinimumThreshold);
            }
        }

        bool ShouldDetectFirstSwipe(Vector2 point)
        {
            // If a pivot point does not exist, should detect first swipe
            if (previousPivotPointExists == false)
            {
                return true;
            }

            return false;
        }

        void DetectFirstSwipe(Vector2 point, float swipeDistanceMinimumThreshold)
        {
            // If there is no previous pivot point, set it
            if (firstPivotPointExists == false)
            {
                firstPivotPoint = point;
                firstPivotPointExists = true;
                previousPivotTime = Time.time;
            }
            // Otherwise, detect a swipe using the pivot point
            else
            {
                DetectSwipeWithPivotPoint(point, firstPivotPoint, swipeDistanceMinimumThreshold);
            }
        }

        void DetectSubsequentSwipes(Vector2 point, float subsequentSwipeDistanceMinimumThreshold)
        {
            // pivot point should already be set
            DetectSwipeWithPivotPoint(point, previousPivotPoint, subsequentSwipeDistanceMinimumThreshold);
        }

        void DetectSwipeWithPivotPoint(Vector2 point, Vector2 pivotPoint, float swipeMinimumThreshold)
        {
            // Calculate the distance between the point and the previousPivotPoint
            float distance = Vector2.Distance(point, pivotPoint);
            Vector2 directionVector;

            currentPivotTime = Time.time;

            // If the distance meets threshold, notify a swipe occurred, return w/ direction vector
            if (distance >= swipeMinimumThreshold)
            {
                directionVector = point - pivotPoint;

                // Set new pivot point b/c a swipe occurred
                this.previousPivotPoint = point;
                this.previousPivotPointExists = true;

                float deltaTime = currentPivotTime - previousPivotTime;
                float swipeRate = distance / deltaTime;
                debugger.Log("Swipe deltaTime: " + deltaTime + ", rate: " + swipeRate + ", threshold: " + settings.swipeRateMinThreshold);

                if(swipeRate >= settings.swipeRateMinThreshold)
                {
                    // *Note* This actually produces better expected behavior for swipes.
                    // We are registering the direction as the first point touched that causes a swipe to be read, instead of using the direction vector.
                    NotifyDelegateDidSwipeWithDirectionVector(pivotPoint, point, directionVector);
                }

                previousPivotTime = Time.time;
            }
        }

        // ------------- Notify delegate ------------------

        void NotifyDelegateDidSwipeWithDirectionVector(Vector2 startingPosition, Vector2 endingPosition, Vector2 direction)
        {
            if (swipeDelegate == null)
            {
                debugger.Log(WARNING_DELEGATE_NOT_SET);
            }
            else
            {
                swipeDelegate.DidSwipeWithDirectionVector(startingPosition, endingPosition, direction);
            }
        }
    }


}
