using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace ext_compiler.tests
{

    public static class MethodPrecompiler
    {
        public static void Precompile(string methodName)
        {
    
                var handle = FindMethodWithName(methodName).MethodHandle;
                RuntimeHelpers.PrepareMethod(handle);



        }

        public static void PrecompileClass<T>()
        {
            PrecompileClass(typeof(T));
        }

        public static void PrecompileClass(Type t)
        {
            var infos = t.GetMethods(MethodBindingFlags);
            foreach (var runtimeMethodHandle in infos)
            {
                RuntimeHelpers.PrepareMethod(runtimeMethodHandle.MethodHandle);
            }
        }



        private static MethodInfo FindMethodWithName(string methodName)
        {
            return
                Assembly.GetExecutingAssembly()
                    .GetTypes()
                    .SelectMany(type => type.GetMethods(MethodBindingFlags))
                    .FirstOrDefault(method => method.Name == methodName);
        }

        private static MethodInfo[] FindMethodsWithName(string methodName)
        {
            return
                Assembly.GetExecutingAssembly()
                    .GetTypes()
                    .SelectMany(type => type.GetMethods(MethodBindingFlags))
                    .Where(x => x.Name == methodName)
                    .ToArray();
        }

        private const BindingFlags MethodBindingFlags =
            BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic |
            BindingFlags.Instance | BindingFlags.Static;
    }
}