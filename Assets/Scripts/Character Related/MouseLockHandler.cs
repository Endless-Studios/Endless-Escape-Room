using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLockHandler : MonoBehaviourSingleton<MouseLockHandler>
{
    List<object> reservedList = new List<object>();

    protected override void Awake()
    {
        base.Awake();
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void ClaimMouseCursor(object claimer)
    {
        reservedList.Add(claimer);
        if(reservedList.Count == 1)
        {
            Cursor.lockState = CursorLockMode.Confined;
        }
    }

    public void ReleaseMouseCursor(object claimer)
    {
        if(reservedList.Contains(claimer))
        {
            reservedList.Remove(claimer);
            if(reservedList.Count == 0)
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
    }

    private void Update()
    {
        /*if(Input.GetKeyDown(KeyCode.LeftAlt))
        {//Update if they press alt
            reservedList.Add(this);
            //force Update to handle alt tabbing and such?
            Cursor.lockState = CursorLockMode.Confined;
        }
        else if(Input.GetKeyUp(KeyCode.LeftAlt))
        {
            reservedList.Remove(this);
            //force Update to handle alt tabbing and such?
            Cursor.lockState = CursorLockMode.Locked;
        }*/

        if(reservedList.Count > 0)
            Cursor.lockState = CursorLockMode.Confined;
        else
            Cursor.lockState = CursorLockMode.Locked;

    }
}
