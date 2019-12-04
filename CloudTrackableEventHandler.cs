/*===============================================================================
Copyright (c) 2015-2018 PTC Inc. All Rights Reserved.
 
Copyright (c) 2010-2015 Qualcomm Connected Experiences, Inc. All Rights Reserved.
 
Vuforia is a trademark of PTC Inc., registered in the United States and other 
countries.
===============================================================================*/
using UnityEngine;
using Vuforia;

public class CloudTrackableEventHandler : DefaultTrackableEventHandler
{
    #region PRIVATE_MEMBERS
    CloudRecoBehaviour m_CloudRecoBehaviour;
    CloudContentManager m_CloudContentManager;

    Transform currentArrow;
    Transform currentArrowParent;
    Transform CloudTarget = null;

    #endregion // PRIVATE_MEMBERS

    public GameObject Arrow;


    #region MONOBEHAVIOUR_METHODS
    protected override void Start()
    {
        base.Start();

        CloudTarget = this.transform;

        // hide arrow by default
        Arrow.SetActive(false);

        m_CloudRecoBehaviour = FindObjectOfType<CloudRecoBehaviour>();
        m_CloudContentManager = FindObjectOfType<CloudContentManager>();
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
        //m_CloudContentManager.HandleTargetFinderResult(targetSearchResult);

        if (Arrow != null)
        {
            if (Arrow.transform.parent != CloudTarget.transform)
            {
                Renderer[] augmentationRenderers;
                // TODO: compares if current augmentation if exists, is equal to one i want to add new on
                //if (currentArrow != null && currentArrow.parent == CloudTarget)
                //{
                //    currentArrow.SetParent(contentManagerParent);
                //    currentArrow.transform.localPosition = Vector3.zero;
                //    currentArrow.transform.localScale = Vector3.one;

                //    augmentationRenderers = currentAugmentation.GetComponentsInChildren<Renderer>();
                //    foreach (var objrenderer in augmentationRenderers)
                //    {
                //        objrenderer.gameObject.layer = LayerMask.NameToLayer("UI");
                //        objrenderer.enabled = true;
                //    }
                //}

                // store reference to content manager's parent object of the augmentation
                currentArrowParent = Arrow.transform.parent;
                // store reference to the current augmentation
                currentArrow = Arrow.transform;

                // set new target augmentation parent to cloud target
                Arrow.transform.SetParent(CloudTarget);
                Arrow.transform.localPosition = Vector3.zero;
                Arrow.transform.localScale = Vector3.one;

                augmentationRenderers = Arrow.GetComponentsInChildren<Renderer>();
                foreach (var objrenderer in augmentationRenderers)
                {
                    objrenderer.gameObject.layer = LayerMask.NameToLayer("Default");
                    objrenderer.enabled = true;
                }
                Arrow.gameObject.SetActive(true);
            }
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

        if (m_CloudContentManager)
        {
            m_CloudContentManager.ShowTargetInfo(true);
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

        if (m_CloudContentManager)
        {
            m_CloudContentManager.ShowTargetInfo(false);
        }
    }

    #endregion // PROTECTED_METHODS
}
