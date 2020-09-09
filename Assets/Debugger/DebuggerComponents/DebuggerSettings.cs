using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Debugger Settings", menuName = "ScriptableObjects/Debugger Settings")]
public class DebuggerSettings : ScriptableObject
{
    public bool isConsoleDebuggerEnabled = true;
    public bool isScreenDebuggerEnabled = true;
}
