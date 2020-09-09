using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JFoundation
{
    [CreateAssetMenu(fileName = "Input Settings", menuName = "ScriptableObjects/Input Settings")]
    public class InputSettings : ScriptableObject
    {
        public float swipeMinThreshold = 50.0f;
        public float subsequentSwipeMinThreshold = 25.0f;
        public float swipeRateMinThreshold = 2000.0f;

        public bool isPcInputEnabled = false;
        public bool isTouchInputEnabled = true;

        public bool showDebug = true;
    }
}
