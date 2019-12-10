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
    GameObject textsContainer;

    #endregion // PRIVATE_MEMBERS

    public GameObject OriginalPreviewPointerPrefab;
    public GameObject PointerPrefab;

    #region MONOBEHAVIOUR_METHODS
    protected override void Start()
    {
        base.Start();

        CloudTarget = this.transform;

        // hide pointer by default
        OriginalPreviewPointerPrefab.SetActive(false);

        m_CloudRecoBehaviour = FindObjectOfType<CloudRecoBehaviour>();
        textsContainer = GameObject.Find("Texts");
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

        // if there are already any pointers (arrow + star) visible, destroy them
        var existingPointers = GameObject.FindGameObjectsWithTag("Pointer");
        if (existingPointers.Any())
        {
            foreach (var existingPointer in existingPointers)
            {
                Debug.Log("<color=yellow>Delete existing poiinter</color>");
                Destroy(existingPointer);
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

            var Pointer = Instantiate(PointerPrefab);
            var star = Pointer.transform.GetChild(1);
            var arrowClickHandlerComponent = star.GetComponent<StarClickHandler>();
            arrowClickHandlerComponent.StopData = stopData;
            Pointer.transform.SetParent(CloudTarget);

            // set original scale and position, because its reset during SetParent above
            Pointer.transform.localPosition = OriginalPreviewPointerPrefab.transform.localPosition;
            Pointer.transform.localScale = OriginalPreviewPointerPrefab.transform.localScale;

            var augmentationRenderers = Pointer.GetComponentsInChildren<Renderer>();
            foreach (var objrenderer in augmentationRenderers)
            {
                objrenderer.gameObject.layer = LayerMask.NameToLayer("Default");
                objrenderer.enabled = true;
            }

            // if this stop was already scored, then display it with scored styles, same as on click on arrow
            if (scoreKeeper.IsStepScored(stopData.VuforiaName))
            {
                arrowClickHandlerComponent.StyleAsScored(Pointer.gameObject);
            }

            Pointer.gameObject.SetActive(true);
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

            // Hide current 'Fact' container, we want it to be visible only when tracking is active
            var factContainer = textsContainer.transform.Find("FactContainer");
            factContainer.gameObject.SetActive(false);
        }
    }

    #endregion // PROTECTED_METHODS
}
