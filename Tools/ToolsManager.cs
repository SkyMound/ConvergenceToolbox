﻿using System.Collections;
using UnityEngine;

namespace Tools
{
    public class ToolsManager : MonoBehaviour
    {
        AutoSplit autoSplit;
        Gizmos gizmos;



        private void OnGUI()
        {
            GUI.Label(new Rect(10, 10, 150, 20), ToolsLoader.GetVersionCTB());

            // Autosplit checkbox
            autoSplit.isEnabled = GUI.Toggle(new Rect(10, 30, 150, 20), autoSplit.isEnabled, "Enable AutoSplit");

            if (autoSplit.isEnabled && !autoSplit.isActive){
                autoSplit.StartAutoSplit();
                gizmos.isEnabled = false;
                gizmos.UpdateGizmos();
            }
            else if (!autoSplit.isEnabled && autoSplit.isActive)
                autoSplit.StopAutoSplit();

            // Gizmos checkbox
            if(!autoSplit.isEnabled){

                gizmos.isEnabled = GUI.Toggle(new Rect(10, 50, 150, 20), gizmos.isEnabled, "Show Gizmos");

                if (gizmos.isEnabled != gizmos.isActive)
                    gizmos.UpdateGizmos();
            }

        }

        // Use this for initialization
        void Start()
        {
            autoSplit = GetComponent<AutoSplit>();
            gizmos = GetComponent<Gizmos>();
        }

    }
}