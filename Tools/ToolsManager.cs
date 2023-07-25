using System.Collections;
using UnityEngine;
using System.IO;
using System.IO.Pipes;
using System;

namespace Tools
{
    public class ToolsManager : MonoBehaviour
    {
        AutoSplit autoSplit;
        Gizmos gizmos;
        PersistentCheckpoint checkpoint;
        bool uiEnabled;
        GameObject Tools;

        public string Version { get; private set; } = "1.1.0";
        public string steamID { get; private set;} = "317573976";
        public string SavesFolder { get; private set; }
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
        }

        // Use this for initialization
        void Start()
        {
            Debugger.Init();

            StartCoroutine(Init());

            
        }

        private IEnumerator Init()
        {
            string steamFolder = Path.GetCurrentDirectory(Path.GetCurrentDirectory(Path.GetDirectoryName(Directory.GetCurrentDirectory())));
            SteamSavesFolder = Path.Combine(new string[]{steamFolder,'userdata',steamID,'1276800','remote'});

            string projectPath = string.Empty;
            yield return new WaitForSeconds(1f); // Making sure the server is up before connecting
            try
            { 
                using (NamedPipeClientStream clientPipe = new NamedPipeClientStream(".", "PipeCTB", PipeDirection.In))
                {
                    Debugger.Log("Connecting to the executable...");
                    clientPipe.Connect();

                    using (StreamReader reader = new StreamReader(clientPipe))
                    {
                        // Read the path of the 'target' folder from the named pipe
                        projectPath = reader.ReadLine();
                    }
                }

                if (!string.IsNullOrEmpty(projectPath))
                {
                    SavesFolder = Path.Combine(projectPath, "Saves");
                    Debugger.Log("Saves folder : " + SavesFolder);
                }

                Tools = new GameObject();
                autoSplit = Tools.AddComponent<AutoSplit>();
                gizmos = Tools.AddComponent<Gizmos>();
                checkpoint = Tools.AddComponent<PersistentCheckpoint>();
                GameObject.DontDestroyOnLoad(Tools);
                uiEnabled = true;
            }
            catch (Exception ex)
            {
                Debugger.Log(ex.Message);
            }
        }
    }
}