using System;
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
    }
}
