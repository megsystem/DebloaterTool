using System;
using System.Linq;
using System.Reflection;

namespace DebloaterTool
{
    internal class HelperModule
    {
        public static void ListModule(bool dontshowinmodule = true)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            foreach (var type in assembly.GetTypes())
            {
                var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Static)
                                  .Where(m => m.ReturnType == typeof(void) && m.GetParameters().Length == 0);

                if (methods.Any())
                {
                    // Display the class name without the namespace in a custom color (e.g., Cyan)
                    HelperDisplay.DisplayMessage($"Class: {type.Name}", ConsoleColor.Cyan);

                    foreach (var method in methods)
                    {
                        // Display the method name with the color Green
                        HelperDisplay.DisplayMessage($"  - Method: {method.Name}()", ConsoleColor.Green);
                    }
                }
            }
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
