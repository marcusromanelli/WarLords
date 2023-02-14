using System;
using UnityEngine;

[Serializable]
public abstract class ILoadableBaseClass : MonoBehaviour, ILoadable
{    public abstract bool HasLoaded();
    public abstract void Load();
}

public interface ILoadable
{    
    public void Load();
    public bool HasLoaded();
}
