using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(HelpBoxAttribute))]
public class HelpBoxAttributeDrawer : DecoratorDrawer
{
    public override float GetHeight()
    {
        HelpBoxAttribute helpBoxAttribute = attribute as HelpBoxAttribute;
        if(helpBoxAttribute == null) 
            return base.GetHeight();
        GUIStyle helpBoxStyle = (GUI.skin != null) ? GUI.skin.GetStyle("helpbox") : null;
        if(helpBoxStyle == null) 
            return base.GetHeight();
        return Mathf.Max(40f, helpBoxStyle.CalcHeight(new GUIContent(helpBoxAttribute.text), EditorGUIUtility.currentViewWidth) + 4);
    }

    public override void OnGUI(Rect position)
    {
        HelpBoxAttribute helpBoxAttribute = attribute as HelpBoxAttribute;
        if(helpBoxAttribute != null)
            EditorGUI.HelpBox(position, helpBoxAttribute.text, GetMessageType(helpBoxAttribute.messageType));
    }

    private MessageType GetMessageType(HelpBoxMessageType helpBoxMessageType)
    {
        switch(helpBoxMessageType)
        {
            case HelpBoxMessageType.Error: 
                return MessageType.Error;
            case HelpBoxMessageType.Warning: 
                return MessageType.Warning;
            case HelpBoxMessageType.Info: 
                return MessageType.Info;
            case HelpBoxMessageType.None:
            default:
                return MessageType.None;
        }
    }
}