using System;
using Unity.AI.Navigation;

namespace Ai
{
    /// <summary>
    /// This class is a data container for a NavMeshLink and the traversal type an Ai should use
    /// when navigating over it.
    /// </summary>
    [Serializable]
    public class LinkInfo
    {
        public NavMeshLink Link;
        public LinkTraversalType LinkTraversalType;
    }
}