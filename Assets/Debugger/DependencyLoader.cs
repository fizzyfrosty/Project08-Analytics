using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    How to use:
    1) Call the static method to load GameObjects by code
    2) The importance of this is to unify code, and debug print the caller when things go wrong.
    3) On LoadGameObject failure, returns null

*/

public class DependencyLoader
{
    //private static DependencyLoader dependencyLoaderInstance;

    //public static DependencyLoader Instance {get {return dependencyLoaderInstance}};

    // Also print's game object's name
    public static object LoadGameObject<ComponentClass>(string prefabName, object sender, GameObject gameObject)
    {
        try
        {
            ComponentClass objectToLoad = GameObject.Find(prefabName).GetComponent<ComponentClass>();
            return objectToLoad;
        }
        catch (System.NullReferenceException ex)
        {
            if (gameObject != null)
            {
                Debug.LogError(string.Format("DependencyLoader: GameObject '{3}''s component '{2}' failed to load dependency '{0}' from prefab named: '{1}'", typeof(ComponentClass).Name, prefabName, sender.GetType(), gameObject.name));
            }
            else{
                Debug.LogError(string.Format("DependencyLoader: Component '{2}' failed to load dependency '{0}' from prefab named: '{1}'", typeof(ComponentClass).Name, prefabName, sender.GetType()));
            }
            return null;
        }
    }

    public static bool DependencyCheck<Dependency>(Object dependency, object sender, GameObject gameObject, Debugger debugger)
    {
        string loggingString = string.Format("Warning: GameObject {2}'s component {1} is missing {0} dependency.", typeof(Dependency).ToString(), sender.GetType(), gameObject.name);

        if (dependency == null)
        {
            // Show error
            if (debugger != null)
            {
                bool shouldShow = true;
                debugger.LogError(loggingString, shouldShow);
            }
            else
            {
                Debug.LogError(loggingString);
            }

            return false;
        }
        else
        {
            return true;
        }
    }
}
