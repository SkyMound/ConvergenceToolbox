using System;
using System.IO;
using System.IO.Pipes;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.Reflection;
using System.Diagnostics;

namespace Tools
{
    public class ToolsLoader
    {
        private static GameObject Load;

        public static void Init()
        {
            Debugger.Init();

            RetrieveInformation();

            Load = new GameObject();
            Load.AddComponent<AutoSplit>();
            Load.AddComponent<Gizmos>();
            Load.AddComponent<PersistentCheckpoint>();
            Load.AddComponent<ToolsManager>();
            GameObject.DontDestroyOnLoad(Load);
        }

        public static void Unload()
        {
            GameObject.Destroy(Load);
        }

        public static string GetVersionCTB()
        {
            return "CTB_1.1.0";
        }

        private static RetrieveInformation(){
            string projectPath = string.Empty;

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
                Debugger.Log("Folder retrieved : "+ savesFolder);
            }
        }
    }
}
