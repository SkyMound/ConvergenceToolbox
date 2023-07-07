using System;
using System.IO;
using SharpMonoInjector;

namespace ConvergenceToolbox
{
    
    class Program
    {

        private static void Main(string[] args)
        {
            string processName = "Convergence";
            string assemblyPath = "Tools.dll";
            string @namespace = "Tools";
            string className = "ToolsLoader";
            string methodName = "Init";

            Injector injector = new Injector(processName);
                
            Inject(injector, assemblyPath, @namespace, className, methodName);
        }

        private static void Inject(Injector injector, string assemblyPath, string @namespace, string className, string methodName)
        {
            byte[] assembly;
            try
            {
                assembly = File.ReadAllBytes(assemblyPath);
            }
            catch
            {
                System.Console.WriteLine("Could not read the file " + assemblyPath);
                return;
            }


            using (injector)
            {
                IntPtr remoteAssembly = IntPtr.Zero;

                try
                {
                    remoteAssembly = injector.Inject(assembly, @namespace, className, methodName);
                }
                catch (InjectorException ie)
                {
                    System.Console.WriteLine("Failed to inject assembly: " + ie);
                }
                catch (Exception exc)
                {
                    System.Console.WriteLine("Failed to inject assembly (unknown error): " + exc);
                }

                if (remoteAssembly == IntPtr.Zero)
                    return;

                System.Console.WriteLine($"{Path.GetFileName(assemblyPath)}: " + (injector.Is64Bit ? $"0x{remoteAssembly.ToInt64():X16}" : $"0x{remoteAssembly.ToInt32():X8}"));
            }
        }
    }
        
}
