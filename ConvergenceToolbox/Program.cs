using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvergenceToolbox
{
    class Program
    {
        private static void Main(string[] args)
        {
            string targetProcessName = "Convergence.exe";
            string assemblyPath = "Tools.dll";
            string namespace = "Tools";
            string className = "ToolsLoader";
            string methodName = "Init";

            Injector injector = new Injector(targetProcessName);

            // Perform the injection
            Inject(injector, assemblyPath, namespace, className, methodName);

        }

        private static void Inject(Injector injector, string assemblyPath, string namespace, string className, string methodName)
        {
            byte[] assembly;
            using(injector){

                try
                {
                    assembly = File.ReadAllBytes(assemblyPath);
                }
                catch
                {
                    System.Console.WriteLine("Could not read the file " + assemblyPath);
                    return;
                }

                IntPtr remoteAssembly = IntPtr.Zero;

                try
                {
                    remoteAssembly = injector.Inject(assembly, namespace, className, methodName);
                }
                catch (InjectorException ie)
                {
                    System.Console.WriteLine("Failed to inject assembly: " + ie);
                    return;
                }
                catch (Exception exc)
                {
                    System.Console.WriteLine("Failed to inject assembly (unknown error): " + exc);
                    return;
                }

                if (remoteAssembly == IntPtr.Zero)
                    return;

                System.Console.WriteLine($"{Path.GetFileName(assemblyPath)}: " +
                    (injector.Is64Bit
                    ? $"0x{remoteAssembly.ToInt64():X16}"
                    : $"0x{remoteAssembly.ToInt32():X8}"));
            }
        }
    }
}
