using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameAnalyticsSDK;

namespace JFoundation
{
    public enum CurrencyTransactionType
    {
        Gain,
        Lose
    }

    public enum ProgressStatus
    {
        Start,
        Complete,
        Fail
    }

    public class AnalyticsController : MonoBehaviour
    {

        Debugger debugger;

        void Awake()
        {
            GameAnalytics.Initialize();

            debugger = DependencyLoader.LoadGameObject<Debugger>("Debugger", this, gameObject) as Debugger;
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        // ----------------- Public Methods ----------------------
        public void LogProgress(ProgressStatus progressStatus, string progress1 = "", string progress2 = "", string progress3 = "", int? value = null)
        {
            GAProgressionStatus gaProgressStatus = GAProgressionStatus.Start;

            if (progressStatus == ProgressStatus.Complete)
            {
                gaProgressStatus = GAProgressionStatus.Complete;
            }
            else if (progressStatus == ProgressStatus.Fail)
            {
                gaProgressStatus = GAProgressionStatus.Fail;
            }
            else if (progressStatus == ProgressStatus.Start)
            {
                gaProgressStatus = GAProgressionStatus.Start;
            }

            // Log progress
            if (value != null)
            {
                GameAnalytics.NewProgressionEvent(gaProgressStatus, progress1, progress2, progress3, (int)value);
            }
            else
            {
                GameAnalytics.NewProgressionEvent(gaProgressStatus, progress1, progress2, progress3);
            }

            debugger.Log(string.Format("Logged Progress Analytic: status: {0}, seg1: {1}, seg2: {2}, seg3: {3}, value: {4}", gaProgressStatus, progress1, progress2, progress3, value));
        }

        public void LogCurrencyTransaction(CurrencyTransactionType transactionType, string currencyType, float currencyValue, string itemType = "", string itemId = "")
        {
            /* itemType and itemId are optional items that are involved in a transaction
            If currency is Gained, an item is counted as lost
            If currency is Lost, an item is counted as gained
            Can also add in IAP to itemType and itemId if resource was gained
            */

            if (transactionType == CurrencyTransactionType.Gain)
            {
                GameAnalytics.NewResourceEvent(GAResourceFlowType.Source, currencyType, currencyValue, itemType, itemId);
            }
            else if (transactionType == CurrencyTransactionType.Lose)
            {
                GameAnalytics.NewResourceEvent(GAResourceFlowType.Sink, currencyType, currencyValue, itemType, itemId);
            }

            debugger.Log(string.Format("Logged Currency Analytic: type: {0}, currency: {1}, value: {2}, itemType: {3}, itemId: {4}", transactionType, currencyType, currencyValue, itemType, itemId));
        }

        public void LogCustom(string eventName, string firstSegment = "", string secondSegment = "", string thirdSegment = "", string fourthSegment = "", float? value = null)
        {
            // Logged in the form of eventName:firstSegment:secondSegment:thirdSegment:fourthSegment
            string loggedString = eventName;

            if (fourthSegment != "")
            {
                loggedString = eventName+":"+firstSegment+":"+secondSegment+":"+thirdSegment+":"+fourthSegment;
            }
            else if (thirdSegment != "")
            {
                loggedString = eventName+":"+firstSegment+":"+secondSegment+":"+thirdSegment;
            }
            else if (secondSegment != "")
            {
                loggedString = eventName+":"+firstSegment+":"+secondSegment;
            }
            else if (firstSegment != "")
            {
                loggedString = eventName+":"+firstSegment;
            }

            // value is optional
            if (value != null)
            {
                // log value
                GameAnalytics.NewDesignEvent(loggedString, (float)value);
            }
            else
            {
                GameAnalytics.NewDesignEvent(loggedString);
            }

            debugger.Log("Logged Custom Analytic: " + loggedString + ", value: " + value);
        }
        // ----------------- End of Public Methods ----------------------
    }

}
