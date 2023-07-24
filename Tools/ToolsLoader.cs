
using UnityEngine;

namespace Tools
{
    public class ToolsLoader
    {
        private static GameObject Load;

        public static void Init()
        {
            Debugger.Init();

            Load = new GameObject();
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
