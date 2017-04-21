﻿using HoloToolkit.Sharing;
using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows.Speech;


/// <summary>
/// TODO: see how the avatars remain persistant for more hints on how to share moving objects
/// </summary>
public class DynamicHologramPlacement : MonoBehaviour, IInputClickHandler
{
    /// <summary>
    /// Tracks if we have been sent a transform for the model.
    /// The model is rendered relative to the actual anchor.
    /// </summary>
    public bool GotTransform { get; private set; }

    /// <summary>
    /// Hooks messages that this component handles as well as initializing 
    /// kword recognizer for this component.
    /// </summary>
    void Start()
    {
        // We care about getting updates for the model transform.
        CustomMessages.Instance.MessageHandlers[CustomMessages.TestMessageID.StageTransformDynamic] = this.OnStageTransfrom;

        // And when a new user join we will send the model transform we have.
        SharingSessionTracker.Instance.SessionJoined += Instance_SessionJoined;
    }

    /// <summary>
    /// When a new user joins we want to send them the relative transform 
    /// for the attached-to model if we have it.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Instance_SessionJoined(object sender, SharingSessionTracker.SessionJoinedEventArgs e)
    {
        if (GotTransform)
        {
            CustomMessages.Instance.SendStageTransform(transform.localPosition, transform.localRotation);
        }
    }

    /// <summary>
    /// If the model has been established and avatar picked, activate EngeryHub component.
    /// Else, propose a location for the model to be placed.
    /// </summary>
    void Update()
    {
        if (!PlayerAvatarStore.Instance.PickerActive &&
            ImportExportAnchorManager.Instance.AnchorEstablished)
        {
            EnableModel();
            // And if we've already been sent the relative transform, we will use it.
             if (GotTransform)
            {
                // This triggers the animation sequence for the model and 
                // puts the cool materials on the model.
                //GetComponent<EnergyHubBase>().SendMessage("OnSelect");
            }
        }
        else if (GotTransform == false)
        {
            //transform.position = Vector3.Lerp(transform.position, ProposeTransformPosition(), 0.2f);
        }
    }

    /// <summary>
    /// Turns on all renderers that were disabled.
    /// </summary>
    void EnableModel()
    {
        foreach (MeshRenderer renderer in gameObject.GetComponentsInChildren<MeshRenderer>())
        {
            renderer.enabled = true;
        }

        foreach (MeshCollider collider in gameObject.GetComponentsInChildren<MeshCollider>())
        {
            collider.enabled = true;
        }
    }

    /// <summary>
    /// Propose a transform position based on the gaze direction of the placing user
    /// </summary>
    /// <returns></returns>
    Vector3 ProposeTransformPosition()
    {
        // Put the model 2m in front of the user.
        Vector3 retval = Camera.main.transform.position + Camera.main.transform.forward * 2;

        return retval;
    }

    /// <summary>
    /// Propagate transform location of model to all players
    /// </summary>
    void IInputClickHandler.OnInputClicked(InputClickedEventData eventData)
    {
        // Note that we have a transform.
        GotTransform = true;

        // And send it to our friends.
        CustomMessages.Instance.SendStageTransform(transform.localPosition, transform.localRotation);
    }

    /// <summary>
    /// When a remote system has a transform for us, we'll get it here.
    /// Handles StangeTransform custommessages.
    /// </summary>
    /// <param name="msg"></param>
    void OnStageTransfrom(NetworkInMessage msg)
    {
        // We read the user ID but we don't use it here.
        msg.ReadInt64();

        transform.localPosition = CustomMessages.Instance.ReadVector3(msg);
        transform.localRotation = CustomMessages.Instance.ReadQuaternion(msg);

        GotTransform = true;
    }
}
