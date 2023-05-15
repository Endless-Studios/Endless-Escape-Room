using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<summary> A utility MonoBehaviour for debug logging results of UnityEvents.</summary>
public class LogResult : MonoBehaviour
{
    [SerializeField] private string logLabel = "Log";

    public void Log(string logString) => Debug.Log($"{logLabel}: {logString}");
    public void Log() => Debug.Log(logLabel);

    //All supported types have to be implemented seperately b/c <object> doesn't get serialized by Unity
    public void Log(Vector2Int result) => Log(result.ToString());
    public void Log(Vector2 result) => Log(result.ToString());
    public void Log(Vector3Int result) => Log(result.ToString());
    public void Log(Vector3 result) => Log(result.ToString());
    public void Log(bool result) => Log(result.ToString());
    public void Log(int result) => Log(result.ToString());
    public void Log(float result) => Log(result.ToString());
    public void Log(GameObject result) => Log(result.ToString());
    public void Log(MonoBehaviour result) => Log(result.ToString());
}
