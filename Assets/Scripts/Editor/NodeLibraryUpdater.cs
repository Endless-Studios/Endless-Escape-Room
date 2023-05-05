using Unity.VisualScripting;
using UnityEditor;

[InitializeOnLoad]
public class NodeLibraryUpdater 
{
    static NodeLibraryUpdater()
    {
        AssemblyReloadEvents.afterAssemblyReload += HandleAfterAssemblyReload;
    }

    private static void HandleAfterAssemblyReload()
    {
        UnitBase.Update();
    }
}
