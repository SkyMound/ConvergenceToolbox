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


        private void OnGUI()
        {
            uiEnabled = GUI.Toggle(new Rect(10, 10, 150, 20), uiEnabled, ToolsLoader.GetVersionCTB());
            
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
            StartCoroutine(RetrieveInformation());

            Tools = new GameObject();
            autoSplit = Tools.AddComponent<AutoSplit>();
            gizmos = Tools.AddComponent<Gizmos>();
            checkpoint = Tools.AddComponent<PersistentCheckpoint>();
            uiEnabled = true;
        }



        private static IEnumerator RetrieveInformation()
        {
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
                    string savesFolder = Path.Combine(projectPath, "Saves");
                    Debugger.Log("Saves folder : " + savesFolder);
                }
            }
            catch (Exception ex)
            {
                Debugger.Log(ex.Message);
            }
        }
    }
}