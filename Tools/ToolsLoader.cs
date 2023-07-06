using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Tools
{
    public class ToolsLoader
    {
        private static GameObject Load;

        public static void Init()
        {
            Load = new GameObject();
            Load.AddComponent<AutoSplit>();
            UnityEngine.Object.DontDestroyOnLoad(Load);
        }

        public static void Unload()
        {
            GameObject.Destroy(Load);
        }        
    }
}
