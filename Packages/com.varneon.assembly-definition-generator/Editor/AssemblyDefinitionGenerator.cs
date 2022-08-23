using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEditorInternal;
using UnityEngine;
using static Varneon.AssemblyDefintionGenerator.Editor.DirectoryUtilities;
using Assembly = System.Reflection.Assembly;

namespace Varneon.AssemblyDefintionGenerator.Editor
{
    public static class AssemblyDefinitionGenerator
    {
        private static string directory;

        private static AssemblyDefinitionObject assemblyDefinitionObject;

        private static AssemblyBuilder assemblyBuilder;

        private static readonly string[] INCLUDED_UNITY_ASSEMBLIES = new string[]
        {
            "UnityEngine",
            "UnityEditor",
            "System",
            "mscorlib",
            "Microsoft.CSharp"
        };

        /// <summary>
        /// Path where the temporary assembly will be created
        /// </summary>
        private const string TEMP_ASSEMBLY_PATH = "Temp/AssemblyDefinitionGenerator/AssemblyDefinitionGenerator.Temp.dll";

        private const string LOG_PREFIX = "[<color=Cyan>Assembly Definition Generator</color>]:";

        [MenuItem("Assets/Create/Assembly Definition (Automatic)", false, 95)]
        private static void GenerateAssemblyDefinition()
        {
            if (EditorApplication.isCompiling) { Debug.Log($"{LOG_PREFIX} Can't create a new Assembly Definition while Editor is compiling!"); return; }

            directory = EditorUtility.SaveFilePanel("Save .asmdef", GetActiveProjectWindowDirectory(), "AssemblyDefinition", "asmdef");

            if (!string.IsNullOrEmpty(directory))
            {
                // Get the directory name and ensure all separators are forward slashes
                string directoryName = Path.GetDirectoryName(directory).Replace('\\', '/');

                // Check if the folder is an Editor folder
                bool isEditorFolder = IsEditorFolder(directoryName);

                // Create a new JSON object of the Assembly Definition
                assemblyDefinitionObject = new AssemblyDefinitionObject();

                // Assign the name of the assembly
                assemblyDefinitionObject.name = Path.GetFileNameWithoutExtension(directory);

                // Get the scripts contained in the folder where the Assembly Definition is being added
                string[] scriptsInDirectory = GetScriptsInDirectory(directoryName);

                // If the folder is Editor folder, only include Editor as a platform
                if (isEditorFolder)
                {
                    assemblyDefinitionObject.includePlatforms = new string[] { "Editor" };
                }
                // If the folder is not Editor folder, prevent generation if an Editor folder with scripts is present deeper down
                else if (!string.IsNullOrEmpty(scriptsInDirectory.FirstOrDefault(s => IsEditorFolder(s))))
                {
                    Debug.LogError($"{LOG_PREFIX} You are generating an assembly definition in non-Editor-only folder, but an Editor folder with scripts in it is present deeper down in this hierarchy. Make sure to separate Editor and Runtime scripts before generating Assembly Definitions.");

                    return;
                }

                // Get the full path of the temporary assembly
                string fullTempAssemblyPath = Path.GetFullPath(TEMP_ASSEMBLY_PATH);

                // Get the directory name of the temporary assembly path
                string fullTempAssemblyDirectory = Path.GetDirectoryName(fullTempAssemblyPath);

                // Create the temporary directory if it doesn't exist
                if (!Directory.Exists(fullTempAssemblyDirectory)) { Directory.CreateDirectory(fullTempAssemblyDirectory); }

                // Create a new temporary isolated assembly for the scripts
                assemblyBuilder = new AssemblyBuilder(TEMP_ASSEMBLY_PATH, scriptsInDirectory);

                assemblyBuilder.referencesOptions = ReferencesOptions.UseEngineModules;

                if (isEditorFolder)
                {
                    assemblyBuilder.flags = AssemblyBuilderFlags.EditorAssembly;
                }

                assemblyBuilder.buildFinished += PostProcessAssemblyDefinitionGeneration;

                // Build the temporary assembly
                assemblyBuilder.Build();
            }
        }

        private static void PostProcessAssemblyDefinitionGeneration(string path, CompilerMessage[] messages)
        {
            assemblyBuilder.buildFinished -= PostProcessAssemblyDefinitionGeneration;

            assemblyBuilder = null;

            // Get all precompiled assemblies from UnityEngine and UnityEditor
            IEnumerable<string> precompiledAutoReferences = CompilationPipeline.GetPrecompiledAssemblyPaths(
                CompilationPipeline.PrecompiledAssemblySources.UnityEngine |
                CompilationPipeline.PrecompiledAssemblySources.UnityEditor
                ).Select(a => Path.GetFileNameWithoutExtension(a));

            // Get the names of all of the precompiled assemblies
            string[] precompiledAssemblies = CompilationPipeline.GetPrecompiledAssemblyNames().Select(a => a.Replace(".dll", string.Empty)).ToArray();

            // Get the assembly references from the temporary assembly
            HashSet<string> references = GetAssemblyDefinitionReferences(path);

            // Exclude precompiled engine assemblies from the references
            references.ExceptWith(precompiledAutoReferences);

            // Create a copy of the references for precompiled references
            HashSet<string> precompiledReferences = new HashSet<string>(references);

            // Exclude the precompiled assemblies from the Assembly Definition References
            references.ExceptWith(precompiledAssemblies);

            // Exclude the Assembly Definition References from precompiled assembly references
            precompiledReferences.ExceptWith(references);

            // Assign the Assembly Definition References
            assemblyDefinitionObject.references = references.ToArray();

            // Assign the precompiled assembly references
            assemblyDefinitionObject.precompiledReferences = precompiledReferences.Select(a => string.Format("{0}.dll", a)).ToArray();

            // Write the raw JSON of the assembly definition
            using (StreamWriter writer = new StreamWriter(directory))
            {
                writer.Write(JsonUtility.ToJson(assemblyDefinitionObject, true));
            }

            AssetDatabase.Refresh();

            // Load the new AssemblyDefinitionAsset
            AssemblyDefinitionAsset assemblyDefinitionAsset = AssetDatabase.LoadAssetAtPath<AssemblyDefinitionAsset>(GetRelativePath(directory));

            // Invoke the callbacks
            AssemblyDefinitionGenerationCallbacks.OnAssemblyDefinitionGenerated(assemblyDefinitionAsset);
        }

        /// <summary>
        /// Gets the filtered assembly references of an assembly for an Assembly Definition
        /// </summary>
        /// <param name="assemblyPath"></param>
        /// <returns></returns>
        private static HashSet<string> GetAssemblyDefinitionReferences(string assemblyPath)
        {
            try
            {
                // Load the temporary assembly
                Assembly assembly = Assembly.LoadFile(assemblyPath);

                // Get the referenced assemblies of the temporary assembly
                AssemblyName[] referencedAssemblies = assembly.GetReferencedAssemblies();

                // Create a new HashSet for storing all of the references
                HashSet<string> references = new HashSet<string>(referencedAssemblies.Select(a => a.Name));

                // Remove the automatically included assemblies from the references
                references.RemoveWhere(r => !string.IsNullOrEmpty(INCLUDED_UNITY_ASSEMBLIES.FirstOrDefault(s => s == r || (r.Contains('.') && s == r.Substring(0, r.IndexOf('.'))))));

                return references;
            }
            catch
            {
                Debug.LogError($"{LOG_PREFIX} Could not load generated assembly! The generated Assembly Definition won't have any references assigned.");
            }

            return new HashSet<string>();
        }
    }
}
