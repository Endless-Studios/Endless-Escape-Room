using System;
using Unity.AI.Navigation;

namespace Ai
{
    [Serializable]
    public class LinkInfo
    {
        public NavMeshLink Link;
        public LinkTraversalType LinkTraversalType;
    }
}