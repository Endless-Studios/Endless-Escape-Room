using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class DrawTextInEditor : MonoBehaviour
{
    private enum DrawCondition { Selected, Always };

    [SerializeField] private DrawCondition drawCondition;
    [SerializeField, TextArea] private string text;    
    [SerializeField] private Vector3 localOffset;

#if UNITY_EDITOR
    private void DrawText()
    {
        Handles.matrix = transform.localToWorldMatrix;
        Handles.Label(localOffset, text);
    }

    private void OnDrawGizmosSelected()
    {
        if(drawCondition == DrawCondition.Selected)
            DrawText();
    }

    private void OnDrawGizmos()
    {
        if(drawCondition == DrawCondition.Always)
            DrawText();
    }
#endif
}
