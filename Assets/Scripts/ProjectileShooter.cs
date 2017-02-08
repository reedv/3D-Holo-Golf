﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;

/// <summary>
/// Controls the launching and reseting of the attached-to projectile gameobject.
/// Also controls when the recent-bounce flag is placed.
/// </summary>
public class ProjectileShooter : MonoBehaviour {
    /// <summary>
    /// Reference this projectile launches towards when relaeased from handdragging
    /// </summary>
    public GameObject reference_point;

    /// <summary>
    /// Icon indicating where Projectile should be shot from next
    /// </summary>
    public GameObject flag;
    
    /// <summary>
    /// Trigger object that counts as this projectile's goal trigger
    /// </summary>
    public GameObject goal;

    /// <summary>
    /// Minimum speed the projectile must be moving to stop from freezing in its place after launched
    /// </summary>
    public float noMovmentthresh = 0.05f;

    public float forwardOffset = 0.5f;
    public const float throwRatio = 10.0f;
    public bool resting = true;

    int strokes;

    // Use this for initialization
    void Start () {
        strokes = 0;
        resting = true;
        gameObject.GetComponent<Rigidbody>().useGravity = false;

        gameObject.GetComponent<HandDraggable>().StartedDragging += ProjectileShooter_StartedDragging;
        gameObject.GetComponent<HandDraggable>().StoppedDragging += ProjectileShooter_StoppedDragging;
	}

    // Update is called once per frame
    void Update()
    {
        if (resting)
        {
            transform.position = Camera.main.transform.position + Vector3.Normalize(Camera.main.transform.forward)*forwardOffset;
            transform.rotation = Camera.main.transform.rotation;
        }
        else
        {
            Rigidbody rb = gameObject.GetComponent<Rigidbody>();
            float speed = rb.velocity.magnitude;
            if (speed < noMovmentthresh)
            {
                // freeze the projectile of this component
                rb.velocity = new Vector3(0, 0, 0);
                //Or
                //rb.constraints = RigidbodyConstraints.FreezeAll;

                PlaceFlag();
            }
        }
    }

    void OnReset()
    {
        Debug.Log("ProjectileShooter: OnReset()");
        gameObject.GetComponent<Rigidbody>().useGravity = false;
        resting = true;
    }

    /// <summary>
    /// Places the flag object of this component along the surface normal of the 
    /// object colliding with attached-to projectile when called.
    /// </summary>
    void PlaceFlag()
    {
        Debug.Log("ProjectileShooter: PlaceFlag()");

    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("ProjectileShooter: OnTriggerEntered()");
        if(other.transform.gameObject.name == goal.name)
        {
            Debug.Log("ProjectileShooter: goal entered");
            gameObject.SetActive(false);  // can't be activated once inactive (just for initial testing)
            SendMessageUpwards("GoalEntered", strokes);
        }
    }

    /// <summary>
    /// Prepares projectile to be dragged from resting position
    /// </summary>
    private void ProjectileShooter_StartedDragging()
    {
        resting = false;
    }

    /// <summary>
    /// Launch projectile towards its assigned reference_point gameobject after handdragging
    /// </summary>
    private void ProjectileShooter_StoppedDragging()
    {
        Debug.Log("ProjectileShooter: StoppedDragging event handler called");
        Rigidbody rb = gameObject.GetComponent<Rigidbody>();
        rb.useGravity = true;
        resting = false;

        Vector3 direction = reference_point.transform.position - gameObject.transform.position;
        float magnitude = direction.magnitude * throwRatio;
        rb.velocity = direction * magnitude;

        strokes++;
        SendMessageUpwards("ProjectileLaunched", strokes);
    }
}
