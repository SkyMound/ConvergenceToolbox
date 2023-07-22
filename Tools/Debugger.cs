using System.Diagnostics;
using System.IO;

namespace Tools
{
    public class Debugger 
    {
        readonly static string path = @"C:\Windows\Temp\CTB_Debug.txt";
        public static void Log(string message)
        {
            if (!File.Exists(path))
            {
                // Create a file to write to.
                using (StreamWriter sw = File.CreateText(path))
                {
                    sw.WriteLine("ConvergenceToolbox Debugger");
                    sw.WriteLine(ToolsLoader.GetVersionCTB());
                    sw.WriteLine(message);
                    Debug.WriteLine("ConvergenceToolbox Debugger");
                    Debug.WriteLine(ToolsLoader.GetVersionCTB());
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