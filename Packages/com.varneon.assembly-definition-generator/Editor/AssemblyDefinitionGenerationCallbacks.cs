using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Varneon.AssemblyDefintionGenerator.Editor
{
    public static class AssemblyDefinitionGenerationCallbacks
    {
        private static readonly Type assemblyDefinitionGeneratedInterfaceType = typeof(IAssemblyDefinitionGeneratedCallback);

        private static readonly List<IAssemblyDefinitionGeneratedCallback> assemblyDefinitionGeneratedCallbacks = new List<IAssemblyDefinitionGeneratedCallback>();

        [InitializeOnLoadMethod]
        private static void Initialize()
        {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (Assembly assembly in assemblies)
            {
                try
                {
                    Type[] types = assembly.GetTypes();
                    foreach (Type type in types)
                    {
                        if (!type.IsAbstract && !type.IsInterface)
                        {
                            if (assemblyDefinitionGeneratedInterfaceType.IsAssignableFrom(type))
                            {
                                assemblyDefinitionGeneratedCallbacks.Add((IAssemblyDefinitionGeneratedCallback)Activator.CreateInstance(type));
                            }
                        }
                    }
                }
                catch (ReflectionTypeLoadException e)
                {
                    Debug.LogException(e);
                }
            }
        }

        public static void OnAssemblyDefinitionGenerated(AssemblyDefinitionAsset definition)
        {
            foreach (IAssemblyDefinitionGeneratedCallback callback in assemblyDefinitionGeneratedCallbacks)
            {
                try
                {
                    callback.OnAssemblyDefinitionGenerated(definition);
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }
        }
    }
}

