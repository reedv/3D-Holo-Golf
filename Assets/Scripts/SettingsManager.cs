﻿using HoloToolkit.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Used to maintain settings information across scenes
/// </summary>
public class SettingsManager : Singleton<SettingsManager>
{
    /// <summary>
    /// number of players expected in game
    /// </summary>
    public int numberOfPlayers = 1;

	// Use this for initialization
	void Start () {
        // save settings for access in gameplay scenes
        Object.DontDestroyOnLoad(gameObject);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
