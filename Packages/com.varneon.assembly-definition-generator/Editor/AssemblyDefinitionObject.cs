using System;

namespace Varneon.AssemblyDefintionGenerator.Editor
{
    /// <summary>
    /// Object representation of the raw AssemblyDefinitionAsset JSON
    /// </summary>
    [Serializable]
    public class AssemblyDefinitionObject
    {
        public string name;

        public string[] references;

        public string[] includePlatforms;

        public string[] excludePlatforms;

        public bool allowUnsafeCode;

        public bool overrideReferences;

        public string[] precompiledReferences;

        public bool autoReferenced = true;

        public string[] defineConstraints;

        public string[] versionDefines;

        public bool noEngineReferences;
    }
}
