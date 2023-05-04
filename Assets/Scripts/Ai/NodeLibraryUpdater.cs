using Unity.VisualScripting;
using UnityEngine;

public static class NodeLibraryUpdater 
{
    [RuntimeInitializeOnLoadMethod]
    private static void BindToAssemblyEvents()
    {
        UnityEditor.AssemblyReloadEvents.afterAssemblyReload += UnitBase.Update;
    }
}
