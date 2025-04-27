using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DebloaterTool
{
    internal class HelperModule
    {
        public static string[] ListModule()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var list = new List<string>();

            foreach (var type in assembly.GetTypes())
            {
                var methods = type
                    .GetMethods(BindingFlags.Public | BindingFlags.Static)
                    .Where(m => m.ReturnType == typeof(void) && m.GetParameters().Length == 0);

                foreach (var method in methods)
                {
                    list.Add($"{type.Name}.{method.Name}");
                }
            }

            return list.ToArray();
        }

        public static void RunModule(string input) 
        {
            // Remove "();" if present
            if (input.EndsWith("();"))
            {
                input = input.Substring(0, input.Length - 3);  // Remove the "();"
            }

            // Remove "()" if the user includes it
            if (input.EndsWith("()"))
            {
                input = input.Substring(0, input.Length - 2);
            }

            try
            {
                // Get the namespace of the current executing assembly
                string namespacePrefix = Assembly.GetExecutingAssembly().GetName().Name;

                // Split to extract class and method name
                int lastDot = input.LastIndexOf('.');
                if (lastDot == -1)
                {
                    Logger.Log("Invalid format. Use Class.Method.", Level.ERROR);
                    return;
                }

                string fullClassName = namespacePrefix + "." + input.Substring(0, lastDot);  // Full Class Name
                string methodName = input.Substring(lastDot + 1);    // Method

                // Search for the class in all loaded assemblies
                Type type = AppDomain.CurrentDomain
                    .GetAssemblies()
                    .SelectMany(a => a.GetTypes())
                    .FirstOrDefault(t => t.FullName == fullClassName);

                if (type == null)
                {
                    Logger.Log("Class not found.", Level.ERROR);
                    return;
                }

                // Create an instance of the class
                object instance = Activator.CreateInstance(type);

                // Find the method
                MethodInfo method = type.GetMethod(methodName);
                if (method == null)
                {
                    Logger.Log("Method not found.", Level.ERROR);
                    return;
                }

                // Invoke the method
                method.Invoke(instance, null);
                Logger.Log($"Successfully executed {methodName} in {fullClassName}", Level.SUCCESS);
            }
            catch (Exception ex)
            {
                Logger.Log("Error: " + ex.Message, Level.ERROR);
            }
        }
    }
}
