using UnityEditorInternal;

namespace Varneon.AssemblyDefintionGenerator.Editor
{
    public interface IAssemblyDefinitionGeneratedCallback
    {
        void OnAssemblyDefinitionGenerated(AssemblyDefinitionAsset definition);
    }
}
