using System.Diagnostics;
using System.IO;

namespace Tools
{
    public class Debugger 
    {
        readonly static string path = @"C:\Windows\Temp\CTB_Debug.txt";

        public static void Init()
        {
            using (StreamWriter sw = File.CreateText(path))
            {
                sw.WriteLine("ConvergenceToolbox_"+ ToolsManager.Instance.Version+ "_Debugger");
            }
        }

        public static void Log(string message)
        {
            if (!File.Exists(path))
            {
                // Create a file to write to.
                using (StreamWriter sw = File.CreateText(path))
                {
                    sw.WriteLine("ConvergenceToolbox_" + ToolsManager.Instance.Version + "_Debugger");
                    sw.WriteLine(message);
                    Debug.WriteLine("ConvergenceToolbox_" + ToolsManager.Instance.Version + "_Debugger");
                }
            }
            else
            {
                using (StreamWriter sw = File.AppendText(path))
                {
                    sw.WriteLine(message);
                }
            }
            Debug.WriteLine(message);
        }
    }
}