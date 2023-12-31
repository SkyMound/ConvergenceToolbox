﻿using System.Collections;
using UnityEngine;
using System.IO;
using System.IO.Pipes;
using System;
using DS.Game.Luna;

namespace Tools
{
    public class ToolsManager : MonoBehaviour
    {
        AutoSplit autoSplit;
        Gizmos gizmos;
        GodMode gm;
        AbilityLogger logger;
        RouteManager route;
        public Shader shader;
        public bool uiEnabled { get; private set; }

        public string Version { get; private set; } = "1.3.1";
        public string steamID { get; private set;} = "317573976";
        public string RoutesFolder { get; private set; }
        public string SteamSavesFolder {get; private set;}

        private static ToolsManager _instance;
        public static ToolsManager Instance { get { return _instance; } }

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                _instance = this;
            }
        }
        private void OnGUI()
        {
            uiEnabled = GUI.Toggle(new Rect(10, 10, 150, 20), uiEnabled, "CTB_"+Version);
            
            if(uiEnabled){
                // Autosplit checkbox
                autoSplit.isEnabled = GUI.Toggle(new Rect(10, 30, 150, 20), autoSplit.isEnabled, "Enable AutoSplit");

                if (autoSplit.isEnabled && !autoSplit.isActive)
                    autoSplit.StartAutoSplit();
                
                else if (!autoSplit.isEnabled && autoSplit.isActive)
                    autoSplit.StopAutoSplit();

                // Ability Logger checkbox
                logger.isEnabled = GUI.Toggle(new Rect(10, 50, 150, 20), logger.isEnabled, "Show Abilities");
                if (logger.isEnabled != logger.isActive)
                    logger.ToggleAbilityLogger();

                
                // Gizmos checkbox
                gizmos.isEnabled = GUI.Toggle(new Rect(10, 70, 150, 20), gizmos.isEnabled, "Show Gizmos");

                if (gizmos.isEnabled != gizmos.isActive)
                    gizmos.UpdateGizmos();

                // Route checkbox
                route.isEnabled = GUI.Toggle(new Rect(10, 90, 150, 20), route.isEnabled, "Show Route");
                if (route.isEnabled != route.enabled)
                {
                    route.enabled = route.isEnabled;
                }

                // GodMode checkbox
                gm.isEnabled = GUI.Toggle(new Rect(10, 110, 150, 20), gm.isEnabled, "Enable GodMode");
                if (gm.isEnabled != gm.isActive)
                    gm.ToggleGodMode();

                if (GUI.Button(new Rect(10, 130, 120, 20), "Speedrun Preset"))
                    gm.SetSpeedrunPreset();

                
            }
        }

        // Use this for initialization
        void Start()
        {
            Debugger.Init();

            StartCoroutine(Init());
        }

        private IEnumerator Init()
        {
            string steamFolder = Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(Directory.GetCurrentDirectory())));
            SteamSavesFolder = Path.Combine(new string[]{steamFolder,"userdata",steamID,"1276800","remote"});
            Debugger.Log(SteamSavesFolder);
            string projectPath = string.Empty;
            using (NamedPipeClientStream clientPipe = new NamedPipeClientStream(".", "PipeCTB", PipeDirection.In))
            {
                Debugger.Log("Connecting to the executable...");
                while(!clientPipe.IsConnected){
                    try
                    {
                        clientPipe.Connect();
                    }catch(Exception ex)
                    {
                        Debugger.Log(". ("+ex.Message+")");
                    }
                    yield return null;
                }

                using (StreamReader reader = new StreamReader(clientPipe))
                {
                    // Read the path of the 'target' folder from the named pipe
                    projectPath = reader.ReadLine();
                }
            }

            if (!string.IsNullOrEmpty(projectPath))
            {
                RoutesFolder = Path.Combine(projectPath, "Routes");
                Debugger.Log("Saves folder : " + RoutesFolder);
            }

            RetrieveShader();
            SBNetworkManager.Instance.Server_HeroesSpawned += RetrieveShader;

            autoSplit   = gameObject.AddComponent<AutoSplit>();
            gizmos      = gameObject.AddComponent<Gizmos>();
            logger      = gameObject.AddComponent<AbilityLogger>();
            route       = gameObject.AddComponent<RouteManager>();
            gm          = new GodMode();
            uiEnabled   = true;
        }

        public void RetrieveShader()
        {
            if (shader != null)
                return;
            MeshRenderer[] mesh = FindObjectsOfType<MeshRenderer>();
            for (int i = 0; i < mesh.Length; i++)
            {

                if (mesh[i].material.shader.ToString().IndexOf("Flat", StringComparison.OrdinalIgnoreCase) >= 0 ||
                    mesh[i].material.shader.ToString().IndexOf("Color", StringComparison.OrdinalIgnoreCase) >= 0 ||
                    mesh[i].material.shader.ToString().IndexOf("Sprites/Default", StringComparison.OrdinalIgnoreCase) >= 0 ||
                    mesh[i].material.shader.ToString().IndexOf("MultiAlpha_Unlit", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    shader = mesh[i].material.shader;
                    Debugger.Log("Found : " + shader.name);
                    return;
                }
            }
            Debugger.Log("No shader found !");
        }
    }


}