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

            if (autoSplit.isEnabled && !autoSplit.isActive)
                autoSplit.StartAutoSplit();
            else if (!autoSplit.isEnabled && autoSplit.isActive)
                autoSplit.StopAutoSplit();

        }

        // Use this for initialization
        void Start()
        {
            autoSplit = GetComponent<AutoSplit>();
            gizmos = GetComponent<Gizmos>();
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}