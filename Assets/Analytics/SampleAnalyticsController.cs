using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameAnalyticsSDK;
using JFoundation;

public class SampleAnalyticsController : MonoBehaviour, WorldInputDelegate
{

    private Debugger debugger;
    public Camera screenCam;

    private WorldInputController worldInput;
    public InputSettings inputSettings;
    AnalyticsController analytics;

    int coalAmount = 0;
    string coalId = "Coal";
    string itemType = "Harvest"; // Treat item type as method of extraction
    string itemId = "Resource"; // Treat itemId as type
    int level = 0;
    int amountOfCoalToReachNewLevel = 5;

    void Awake()
    {
        debugger = DependencyLoader.LoadGameObject<Debugger>("Debugger", this, gameObject) as Debugger;
        analytics = gameObject.AddComponent<AnalyticsController>() as AnalyticsController;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (DependencyLoader.DependencyCheck<InputSettings>(inputSettings, this, gameObject, debugger))
        {
            worldInput = gameObject.AddComponent<WorldInputController>() as WorldInputController;
            worldInput.SetDependencies(this, screenCam);

        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    // ----------------- Private Sample implemention methods --------------
    void MineCoal()
    {
        coalAmount++;
        debugger.Log( "Mined some Coal: " + coalAmount);
    }

    bool DidMineEnoughCoal(int coalAmount)
    {
        if (coalAmount != 0 && coalAmount % amountOfCoalToReachNewLevel == 0)
        {
            return true;
        }

        return false;
    }

    // ----------------- End of Private Sample implemention methods --------------


    // ---------------------- User Input Delegates -----------------
    void WorldInputDelegate.WorldInputDidEnterPoint(Vector3 point, RaycastHit hit)
    {
        if (hit.collider.name == "UpperLeftCube")
        {
            MineCoal();

            if (DidMineEnoughCoal(coalAmount))
            {
                level++;
                debugger.Log("Reached new level!: " + level);

                analytics.LogProgress(ProgressStatus.Complete, "level" + level);
                analytics.LogCurrencyTransaction(CurrencyTransactionType.Gain, currencyType: coalId, currencyValue: amountOfCoalToReachNewLevel, itemType: itemType, itemId: itemId);
            }
        }
        else if (hit.collider.name == "UpperRightCube")
        {
            analytics.LogCustom(eventName: "CubeTouched", firstSegment: "UpperRight");
        }
        else if (hit.collider.name == "LowerLeftCube")
        {
            analytics.LogCustom(eventName: "CubeTouched", firstSegment: "LowerLeft");
        }



    }
    void WorldInputDelegate.WorldInputDidMovePoint(Vector3 point, RaycastHit hit)
    {
        //debugger.Log( "Moved a world point: " + point);
    }
    void WorldInputDelegate.WorldInputDidReleasePoint(Vector3 point, RaycastHit hit)
    {
        //debugger.Log( "Released a world point: " + point);
    }

    // ---------------------- End of User Input Delegates -----------------
}
