
using System;
using System.IO;
using UnityEngine;

namespace Tools
{
    public class ToolsLoader
    {
        private static GameObject Load;

        public static void Init()
        {
            Load = new GameObject();
            Load.AddComponent<ToolsManager>();
            GameObject.DontDestroyOnLoad(Load);
        }

        public static void Unload()
        {
            GameObject.Destroy(Load);
        }

        

    }
}
