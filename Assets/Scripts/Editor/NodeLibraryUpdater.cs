using Unity.VisualScripting;
using UnityEditor;

[InitializeOnLoad]
public class NodeLibraryUpdater 
{
    static NodeLibraryUpdater()
    {
        AssemblyReloadEvents.afterAssemblyReload += HandleAfterAssemblyReload;
        //SerializedPropertyProviderProvider.instance.GenerateProviderScripts();
    }

    private static void HandleAfterAssemblyReload()
    {
        UnitBase.Update();
    }
}
