using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;

[CustomPropertyDrawer(typeof(Identifier))]
public class IdentifierInspector : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        string[] displayedOptions = IdentifierList.Instance.GetDisplayItems();

        SerializedProperty identifierProperty = property.FindPropertyRelative("identifier");
        int currentValue = System.Array.IndexOf(displayedOptions, identifierProperty.stringValue);
        Rect dropDownPosition = new Rect(position);
        dropDownPosition.width -= 20;
        int newChoice = EditorGUI.Popup(dropDownPosition, currentValue, displayedOptions);
        if(newChoice != currentValue)
        {
            identifierProperty.stringValue = displayedOptions[newChoice];
        }
        Rect buttonPosition = new Rect(position.x + dropDownPosition.width, position.y, 20, position.height);
        if(GUI.Button(buttonPosition, "+"))
            Selection.activeObject = IdentifierList.Instance;
    }
}
