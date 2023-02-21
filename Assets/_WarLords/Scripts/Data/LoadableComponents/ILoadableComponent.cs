using System;
using UnityEngine;

[Serializable]
public abstract class ILoadableComponentBaseClass : MonoBehaviour, ILoadableComponent
{    public abstract bool HasLoaded();
    public abstract void Load();
}

public interface ILoadableComponent
{    
    public void Load();
    public bool HasLoaded();
}
