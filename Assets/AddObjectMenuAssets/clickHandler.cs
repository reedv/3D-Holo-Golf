﻿using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class clickHandler : MonoBehaviour,
                            IFocusable,
                            IInputClickHandler
{
    public Material active_material;
    public Material inactive_material;

    [Tooltip("Object to be created when this gameobject is clicked")]
    public GameObject selectionObject;

    [Tooltip("Parent menu of this obstacle selection button")]
    public GameObject parentMenu;

    private ObjectSelectionHandler objectSelectionHandler;

    // Use this for initialization
    void Start()
    {
        objectSelectionHandler = ObjectSelectionHandler.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        //in case the other buttons have been clicked
    }

    void IFocusable.OnFocusEnter()
    {
        Debug.Log(gameObject.name + ": " + this.GetType().Name + ": OnFocusEnter()");
        //System.Console.Write("ExitButtonHandler: OnFocusEnter()");
        gameObject.GetComponent<MeshRenderer>().material = active_material;
    }

    void IFocusable.OnFocusExit()
    {
        gameObject.GetComponent<MeshRenderer>().material = inactive_material;
    }

    void IInputClickHandler.OnInputClicked(InputClickedEventData eventData)
    {
        //on click function
        //Debug.Log(gameObject.name + ": OnInputClicked()");
        //System.Console.Write("ObjectButtons: OnInputClicked()");

        //The reason I do it before I actually create the objects, is so that it creates a "lock" on 
        //creating on object, however it doesn't actually make a lock so errors could happen with multiple
        //hololenses 
        //see if I can make an actual lock in c#
        objectSelectionHandler.objectsCreated += 1;

        // create associated object and place in front of player
        GameObject createdObject;
        createdObject = (GameObject)Instantiate(selectionObject);
        createdObject.transform.position = Camera.main.transform.position + Vector3.Normalize(Camera.main.transform.forward) * 2;
        createdObject.SetActive(true);

        // make object handdraggable and setup its relation to obstacleSelectionMenu
        createdObject.AddComponent<HandDraggable>();
        createdObject.GetComponent<HandDraggable>().StartedDragging += clickHandler_StartedDragging;
        // only bring back menu after dragging if not last player of current selection round
        if (objectSelectionHandler.objectsCreated < objectSelectionHandler.numPlayers) {
            createdObject.GetComponent<HandDraggable>().StoppedDragging += clickHandler_StoppedDragging;
        }

        // add this createdObject to the list of obstacles created during this round
        objectSelectionHandler.currentObjects.Enqueue(createdObject);

        // randomize selection menu for next player
        if (objectSelectionHandler.isPlayAndPassGame)
        {
            objectSelectionHandler.prepareGameObjectMenu();
        }
        
    }

    // TODO: these event handlers should probably be moved to the obstacle menu manager class (ObjectSelectionHandler.cs)
    public void clickHandler_StartedDragging()
    {
        parentMenu.SetActive(false);
    }

    public void clickHandler_StoppedDragging()
    {
        Debug.Log(gameObject.name + ": " + this.GetType().Name + ": StoppedDragging event handler called");
        parentMenu.SetActive(true);
    }
}
