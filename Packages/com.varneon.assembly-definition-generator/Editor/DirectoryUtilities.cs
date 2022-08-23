using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Varneon.AssemblyDefintionGenerator.Editor
{
    public static class DirectoryUtilities
    {
        /// <summary>
        /// Gets the current active directory in the Project window
        /// </summary>
        /// <returns></returns>
        public static string GetActiveProjectWindowDirectory()
        {
            MethodInfo getActiveFolderPathMethod = typeof(ProjectWindowUtil).GetMethod("GetActiveFolderPath", BindingFlags.Static | BindingFlags.NonPublic);

            return (string)getActiveFolderPathMethod.Invoke(null, null);
        }

        /// <summary>
        /// Gets all .cs scripts in a directory
        /// </summary>
        /// <param name="directory"></param>
        /// <returns>Paths of the scripts</returns>
        public static string[] GetScriptsInDirectory(string directory)
        {
            return Directory.GetFiles(directory, "*", SearchOption.AllDirectories).Where(f => f.EndsWith(".cs")).Select(f => GetRelativePath(f)).ToArray();
        }

        /// <summary>
        /// Gets the relative path to the current project
        /// </summary>
        /// <param name="fullPath"></param>
        /// <returns></returns>
        public static string GetRelativePath(string fullPath)
        {
            // Ensure that all backslashes get swapped to forward slashes
            fullPath = fullPath.Replace('\\', '/');

            // Get the current Assets directory
            string assetsDirectory = Application.dataPath;

            // Trim the "Assets" from the end of the directory
            assetsDirectory = assetsDirectory.Substring(0, assetsDirectory.Length - 6);

            // If the full path starts with the project directory, only return the Assets/... or Projects/...
            if (fullPath.StartsWith(assetsDirectory))
            {
                return fullPath.Substring(assetsDirectory.Length);
            }

            return fullPath;
        }

        /// <summary>
        /// Checks if any of the folders in the path is "Editor"
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool IsEditorFolder(string path)
        {
            return path.Replace('\\', '/').Split('/').Contains("Editor");
        }
    }
}
