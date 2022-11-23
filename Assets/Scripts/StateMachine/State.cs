using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class State 
{
    public string displayName = "State";

    public virtual void Enter()
    {
        // Debug.Log("Enter state: " + displayName);
    }

    public virtual void Update()
    {
        // Debug.Log("Update state: " + displayName);
    }

    public virtual void Exit()
    {
        // Debug.Log("Exit state: " + displayName);
    }
}
