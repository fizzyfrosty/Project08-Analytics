using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenDebugger : MonoBehaviour
{
    /*
        How to use:
        1) Attach to a gameObject
        2) Set to Debugger as the screen debugger
        3) Profit!

    */
    public string labelText{get;internal set;}

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 500, 500), labelText);
        /*if (GUI.Button(new Rect(10, 10, 150, 100), "I am a button"))
        {
            print("You clicked the button!");
        }*/
    }

    public void Log(string loggingString)
    {
        labelText = loggingString + "\n" + labelText;
    }
}
