using System.IO;
using System.Linq;
using UdonSharp;
using UdonSharpEditor;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using Varneon.AssemblyDefintionGenerator.Editor.Callbacks;
using static Varneon.AssemblyDefintionGenerator.Editor.DirectoryUtilities;

namespace Varneon.AssemblyDefintionGenerator.Editor.Addons
{
    public class UdonSharpAssemblyGenerator : IAssemblyDefinitionGeneratedCallback
    {
        public void OnAssemblyDefinitionGenerated(AssemblyDefinitionAsset definition)
        {
            MonoScript[] scripts = GetScriptsInDirectory(Path.GetDirectoryName(AssetDatabase.GetAssetPath(definition))).Select(s => AssetDatabase.LoadAssetAtPath<MonoScript>(s)).ToArray();

            if(scripts.FirstOrDefault(s => s.GetClass().IsSubclassOf(typeof(UdonSharpBehaviour))) != null)
            {
                CreateUdonSharpAssemblyDefinition(definition);
            }
        }

        /// <summary>
        /// Creates an UdonSharpAssemblyDefinition that is tied to an AssemblyDefinitionAsset
        /// </summary>
        /// <param name="assemblyDefinition"></param>
        private static void CreateUdonSharpAssemblyDefinition(AssemblyDefinitionAsset assemblyDefinition)
        {
            // Replace the file extension
            string path = AssetDatabase.GetAssetPath(assemblyDefinition).Replace(".asmdef", ".asset");

            // Create a new instance of UdonSharpAssemblyDefinition
            UdonSharpAssemblyDefinition udonSharpAssemblyDefinition = ScriptableObject.CreateInstance<UdonSharpAssemblyDefinition>();

            // Assign the source assembly
            udonSharpAssemblyDefinition.sourceAssembly = assemblyDefinition;

            // Create the new asset
            AssetDatabase.CreateAsset(udonSharpAssemblyDefinition, path);

            AssetDatabase.Refresh();
        }
    }
}
