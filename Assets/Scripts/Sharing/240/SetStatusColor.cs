﻿using UnityEngine;
using System.Collections;

/// <summary>
/// Changes the color on the CursorOffHolograms
/// to give visual feedback on anchor sharing states.
/// </summary>
public class SetStatusColor : MonoBehaviour
{
    public Color InitializingColor = Color.magenta;
    public Color ImportingOrExportingColor = Color.yellow;
    public Color UploadingColor = Color.blue;
    public Color DownloadingColor = Color.green;    
    public Color FailureColor = Color.red;

    ImportExportAnchorManager anchorManager;
    Color startColor;
    Light pointLight;

    void Start()
    {
        pointLight = GetComponent<Light>();
        startColor = pointLight.color;

        anchorManager = ImportExportAnchorManager.Instance;
    }

    void Update()
    {
        if (anchorManager != null)
        {
            switch (anchorManager.CurrentState)
            {
                case ImportExportAnchorManager.ImportExportState.AnchorStore_Initializing:
                case ImportExportAnchorManager.ImportExportState.Start:
                case ImportExportAnchorManager.ImportExportState.AnchorStore_Initialized:
                    Debug.LogFormat("{0}: Initialization color", this.GetType().Name);
                    pointLight.color = InitializingColor;
                    break;
                case ImportExportAnchorManager.ImportExportState.Importing:
                case ImportExportAnchorManager.ImportExportState.InitialAnchorRequired:
                case ImportExportAnchorManager.ImportExportState.CreatingInitialAnchor:
                case ImportExportAnchorManager.ImportExportState.DataReady:
                    Debug.LogFormat("{0}: ImportingOrExporting color", this.GetType().Name);
                    pointLight.color = ImportingOrExportingColor;
                    break;
                case ImportExportAnchorManager.ImportExportState.UploadingInitialAnchor:
                    Debug.LogFormat("{0}: Uploading color", this.GetType().Name);
                    pointLight.color = UploadingColor;
                    break;
                case ImportExportAnchorManager.ImportExportState.DataRequested:
                    Debug.LogFormat("{0}: Downloading color", this.GetType().Name);
                    pointLight.color = DownloadingColor;
                    break;
                case ImportExportAnchorManager.ImportExportState.Failed:
                    Debug.LogFormat("{0}: Failure color", this.GetType().Name);
                    pointLight.color = FailureColor;
                    break;
                default:
                    Debug.LogFormat("{0}: start color", this.GetType().Name);
                    pointLight.color = startColor;
                    break;
            }
        }

    }
}
