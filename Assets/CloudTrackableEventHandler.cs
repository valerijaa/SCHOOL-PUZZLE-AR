/*===============================================================================
Copyright (c) 2015-2018 PTC Inc. All Rights Reserved.
 
Copyright (c) 2010-2015 Qualcomm Connected Experiences, Inc. All Rights Reserved.
 
Vuforia is a trademark of PTC Inc., registered in the United States and other 
countries.
===============================================================================*/
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using Vuforia;

public class CloudTrackableEventHandler : DefaultTrackableEventHandler
{
    #region PRIVATE_MEMBERS
    CloudRecoBehaviour m_CloudRecoBehaviour;
    Transform CloudTarget = null;

    #endregion // PRIVATE_MEMBERS

    public GameObject OriginalArrow;
    public GameObject ArrowPrefab;

    #region MONOBEHAVIOUR_METHODS
    protected override void Start()
    {
        base.Start();

        CloudTarget = this.transform;

        // hide arrow by default
        OriginalArrow.SetActive(false);

        m_CloudRecoBehaviour = FindObjectOfType<CloudRecoBehaviour>();
    }
    #endregion // MONOBEHAVIOUR_METHODS


    #region BUTTON_METHODS
    public void OnReset()
    {
        Debug.Log("<color=blue>OnReset()</color>");

        OnTrackingLost();
        TrackerManager.Instance.GetTracker<ObjectTracker>().GetTargetFinder<ImageTargetFinder>().ClearTrackables(false);
    }
    #endregion BUTTON_METHODS


    #region PUBLIC_METHODS
    /// <summary>
    /// Method called from the CloudRecoEventHandler
    /// when a new target is created
    /// </summary>
    public void TargetCreated(TargetFinder.CloudRecoSearchResult targetSearchResult)
    {
        Debug.Log("<color=green>TargetCreated(): </color>" + targetSearchResult.TargetName);

        // if there are already any arrows visible, destroy them
        var existingArrows = GameObject.FindGameObjectsWithTag("Arrow");
        if (existingArrows.Any())
        {
            foreach(var existingArrow in existingArrows)
            {
                Debug.Log("<color=yellow>Delete existing arrow: </color>");
                Destroy(existingArrow);
            }
        }

        StartCoroutine(LoadSelectedStopDataAndShowArrow(targetSearchResult));
    }

    IEnumerator LoadSelectedStopDataAndShowArrow(TargetFinder.CloudRecoSearchResult targetSearchResult)
    {
        UnityWebRequest uwr = UnityWebRequest.Get(scoreKeeper.Hostname + "stop?name="+ targetSearchResult.TargetName);
        yield return uwr.SendWebRequest();

        if (!uwr.isNetworkError)
        {
            Debug.Log("<color=blue>" + uwr.downloadHandler.text + "</color>");
            var stopData = JsonUtility.FromJson<Stop>(uwr.downloadHandler.text);

            var Arrow = Instantiate(ArrowPrefab);
            var arrowClickHandlerComponent = Arrow.GetComponent<ArrowClickHandler>();
            arrowClickHandlerComponent.StopData = stopData;
            Arrow.transform.SetParent(CloudTarget);

            // set original scale and position, because its reset during SetParent above
            Arrow.transform.localPosition = OriginalArrow.transform.localPosition;
            Arrow.transform.localScale = OriginalArrow.transform.localScale;

            var augmentationRenderers = Arrow.GetComponentsInChildren<Renderer>();
            foreach (var objrenderer in augmentationRenderers)
            {
                objrenderer.gameObject.layer = LayerMask.NameToLayer("Default");
                objrenderer.enabled = true;
            }

            // if this stop was already scored, then display it with scored styles, same as on click on arrow
            if (scoreKeeper.IsStepScored(stopData.VuforiaName))
            {
                arrowClickHandlerComponent.StyleAsScored(Arrow.gameObject);
            }

            Arrow.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogError("Failed to fetch data");
        }
    }

    #endregion // PUBLIC_METHODS


    #region PROTECTED_METHODS

    protected override void OnTrackingFound()
    {
        Debug.Log("<color=blue>OnTrackingFound()</color>");

        base.OnTrackingFound();

        if (m_CloudRecoBehaviour)
        {
            // Changing CloudRecoBehaviour.CloudRecoEnabled to false will call TargetFinder.Stop()
            // and also call all registered ICloudRecoEventHandler.OnStateChanged() with false.
            m_CloudRecoBehaviour.CloudRecoEnabled = false;
        }
    }

    protected override void OnTrackingLost()
    {
        Debug.Log("<color=blue>OnTrackingLost()</color>");

        base.OnTrackingLost();

        if (m_CloudRecoBehaviour)
        {
            // Changing CloudRecoBehaviour.CloudRecoEnabled to true will call TargetFinder.StartRecognition()
            // and also call all registered ICloudRecoEventHandler.OnStateChanged() with true.
            m_CloudRecoBehaviour.CloudRecoEnabled = true;
        }
    }

    #endregion // PROTECTED_METHODS
}
