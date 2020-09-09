using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameAnalyticsSDK;
using JFoundation;

public class SampleGameController : MonoBehaviour, WorldInputDelegate
{

    private Debugger debugger;
    public Camera screenCam;

    private WorldInputController worldInput;
    public InputSettings inputSettings;
    public AnalyticsController analytics;

    int coalAmount = 0;
    string coalId = "Coal";
    int level = 0;

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
            worldInput.SetDependencies(this, inputSettings, screenCam);

        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    void WorldInputDelegate.WorldInputDidEnterPoint(Vector3 point, RaycastHit hit)
    {
        if (hit.collider.name == "UpperLeftCube")
        {
            coalAmount++;
            debugger.Log( "Mined some Coal: " + coalAmount);
        }
        else if (hit.collider.name == "UpperRightCube")
        {
            analytics.LogCustom(eventName: "CubeTouched", firstSegment: "UpperRight");
        }
        else if (hit.collider.name == "LowerLeftCube")
        {
            analytics.LogCustom(eventName: "CubeTouched", firstSegment: "LowerLeft");
        }


        int amountToReachNewLevel = 5;

        if (coalAmount != 0 && coalAmount % amountToReachNewLevel == 0)
        {
            level++;
            debugger.Log("Reached new level!: " + level);

            analytics.LogProgress(ProgressStatus.Complete, "level" + level);
            analytics.LogCurrencyTransaction(CurrencyTransactionType.Gain, currencyType: coalId, currencyValue: amountToReachNewLevel);
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
}
