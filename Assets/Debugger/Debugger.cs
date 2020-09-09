using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Debugger : MonoBehaviour
{
    /*
        Setup:
        1) Attach to Debugger.cs to GameObject prefab
        2) Attach a DebuggerSettings to prefab
        3) Place Debugger GameObject in scene

        How to use (in another Object):
        1) Create a public/private reference, and set it
        2) If private reference, call debugger = GameObject.Find(DEBUGGER_OBJECT_NAME).GetComponent<Debugger>() as Debugger;
    */

    // Prefab Dependencies
    public DebuggerSettings debuggerSettings;
    string DEBUG_SETTINGS_WARNING = "Warning: Debugger settings not set! (Debugger not initialized.)";

    ScreenDebugger screenDebugger;
    string SCREEN_DEBUGGER_NOT_SET_MESSAGE = "Warning: Screen Debugger not set!";
    string SCREEN_DEBUGGER_STARTED = "Screen Debugger started.";
    string SCREEN_DEBUGGER_STOPPED = "Screen Debugger stopped.";

    string CONSOLE_DEBUGGER_STARTED = "Console Debugger started.";
    string CONSOLE_DEBUGGER_STOPPED = "Console Debugger stopped.";

    bool isScreenDebuggerActivated = false;
    bool isConsoleDebuggerActivated = false;

    void Awake()
    {
        ActivateDebuggers();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    void ActivateDebuggers()
    {
        if(debuggerSettings)
        {
            if (debuggerSettings.isConsoleDebuggerEnabled)
            {
                ActivateConsoleDebugger();
            }

            if (debuggerSettings.isScreenDebuggerEnabled)
            {
                ActivateScreenDebugger();
            }
        }
        else
        {
            Debug.Log(DEBUG_SETTINGS_WARNING);
        }

    }

    // Update is called once per frame
    void Update()
    {

    }

    // Optional public methods to turn on
	public void ActivateConsoleDebugger()
    {
        isConsoleDebuggerActivated = true;
        this.Log(CONSOLE_DEBUGGER_STARTED);
    }

    // Optional public methods to turn on
    public void ActivateScreenDebugger()
    {
        if (screenDebugger == null)
        {
            screenDebugger = gameObject.AddComponent(typeof(ScreenDebugger)) as ScreenDebugger;
        }
        isScreenDebuggerActivated = true;
        this.Log(SCREEN_DEBUGGER_STARTED);
    }

    public void DeactivateConsoleDebugger()
    {
        this.Log(CONSOLE_DEBUGGER_STOPPED);
        isConsoleDebuggerActivated = false;
    }

    public void DeactivateScreenDebugger()
    {
        this.Log(SCREEN_DEBUGGER_STOPPED);
        isScreenDebuggerActivated = false;
    }

    // For use with testing, turning on/off in-game.
    public void ToggleConsoleDebugger()
    {
        if (isConsoleDebuggerActivated == true)
        {
            this.DeactivateConsoleDebugger();
        }
        else
        {
            this.ActivateConsoleDebugger();
        }
    }

    public void ToggleScreenDebugger()
    {
        if (isScreenDebuggerActivated == true)
        {
            this.DeactivateScreenDebugger();
        }
        else
        {
            this.ActivateScreenDebugger();
        }
    }

    // May be deprecated due to dependency loader's dependency checks
    public void WarnMissingDependency<Class, Dependency>(GameObject gameObject, bool optionalToggleOn)
    {
        if (optionalToggleOn)
        {
            string loggingString = string.Format("Warning: '{2}'s component {0} has no {1} set.", typeof(Class).ToString(), typeof(Dependency).ToString(), gameObject.name);

            Warn(loggingString, optionalToggleOn);
        }
    }

    public void LogError(string loggingString, bool optionalToggleOn)
    {
        if (optionalToggleOn)
        {
            if (isConsoleDebuggerActivated)
            {
                Debug.LogError(loggingString);
            }

            if (isScreenDebuggerActivated)
            {
                if (screenDebugger)
                {
                    screenDebugger.Log(loggingString);
                }
                else
                {
                    Debug.LogWarning(SCREEN_DEBUGGER_NOT_SET_MESSAGE);
                }
            }
        }
    }

    public void Warn(string loggingString, bool optionalToggleOn)
    {
        if (optionalToggleOn)
        {
            if (isConsoleDebuggerActivated)
            {
                Debug.LogWarning(loggingString);
            }

            if (isScreenDebuggerActivated)
            {
                if (screenDebugger)
                {
                    screenDebugger.Log(loggingString);
                }
                else
                {
                    Debug.LogWarning(SCREEN_DEBUGGER_NOT_SET_MESSAGE);
                }
            }
        }
    }

    public void Warn(string loggingString, object sender, GameObject gameObject)
    {
        string warningWithSource = string.Format("Warning for GameObject {1}'s component {0}:", sender.GetType(), gameObject.name);

        Warn(warningWithSource, true);
    }

    public void Log(string loggingString, object sender, GameObject gameObject)
    {
        string loggingWithSource = string.Format("{1}'s component {0}: {2}", sender.GetType(), gameObject.name, loggingString);

        Log(loggingWithSource);
    }

    public void Log(string loggingString, GameObject gameObject, bool optionalToggleOn)
    {
        if (optionalToggleOn)
        {
            string finalLoggingString = string.Format("{0}: " + loggingString, gameObject.name);

            Log(finalLoggingString);
        }
    }

    public void Log(string loggingString, bool optionalToggleOn)
    {
        if (optionalToggleOn)
        {
            Log(loggingString);
        }
    }

	public void Log(string loggingString)
	{
        // Print Screen Debug Message
        if (isScreenDebuggerActivated)
        {
            if (screenDebugger == null)
            {
                // If screen debugger somehow gets unhooked, show warning.
                Debug.Log(SCREEN_DEBUGGER_NOT_SET_MESSAGE);
            }
            else
            {
                // Normal conditions, set screen debugger, print
                screenDebugger.Log(loggingString);
            }
        }

        // Print Console Debug Message
        if (isConsoleDebuggerActivated)
        {
            Debug.Log(loggingString);
        }
	}

}
