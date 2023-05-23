using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Identifier: System.IEquatable<Identifier>
{
    [SerializeField] string identifier = string.Empty;

    public Identifier()
    {
        identifier = string.Empty;
    }

    public static bool operator ==(Identifier lhs, Identifier rhs)
    {
        if(lhs is null)
            return rhs is null;
        else
            return lhs.Equals(rhs);
    }

    public static bool operator !=(Identifier lhs, Identifier rhs)
    {
        if(lhs is null)
            return rhs is null;
        else
            return !lhs.Equals(rhs);
    }

    public override bool Equals(object obj)
    {
        return obj is Identifier other ? string.Equals(other.identifier, identifier) : false;
    }

    public override int GetHashCode()
    {
        return identifier.GetHashCode();
    }

    public bool Equals(Identifier other)
    {
        if(other is null)
            return false;
        else
            return string.Equals(other.identifier, identifier);
    }
}

[CreateAssetMenu(fileName = "IdentifierList", menuName = "Scriptable Objects/Identifier List", order = 1)]
public class IdentifierList : ScriptableObject
{
    [HelpBox("Note that renaming or removing elements in this list can break existing identifier usage!", HelpBoxMessageType.Warning)]
    [SerializeField] string[] identifiers;
    static IdentifierList instance;

    public static IdentifierList Instance
    {
        get
        {
            if(instance == null)
                instance = Resources.Load<IdentifierList>("IdentifierList");
            return instance;
        }
    }

    public string[] GetDisplayItems()
    {
        return identifiers;
    }
}
