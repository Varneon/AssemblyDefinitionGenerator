using UnityEditorInternal;

namespace Varneon.AssemblyDefintionGenerator.Editor.Callbacks
{
    public interface IAssemblyDefinitionGeneratedCallback
    {
        void OnAssemblyDefinitionGenerated(AssemblyDefinitionAsset definition);
    }
}
